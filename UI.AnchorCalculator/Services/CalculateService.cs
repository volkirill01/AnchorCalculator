using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using DAL.AnchorCalculator;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.Utils;

namespace UI.AnchorCalculator.Services
{
    public class CalculateService
    {
        private readonly LoggerManager _logger;
        const double dencitySteel = 7850; // dencity of steel
        private readonly IWebHostEnvironment appEnvironment;

        public CalculateService(IWebHostEnvironment appEnvironment, LoggerManager logger)
        {
            this.appEnvironment = appEnvironment;
            _logger = logger;
        }

        public async Task Calculate(Anchor anchor)
        {
            CostWork costWork = new();
            try
            {
                costWork = await costWork.GetCostWork(appEnvironment);
            }
            catch (Exception ex)
            {
                string exception = $"Error:{ex.Message}";
                _logger.LogDebug(exception);
                throw;
            }

            anchor.BilletLength = CalculParams.GetLengthBillet(anchor);
            anchor.LengthFull = anchor.BilletLength * anchor.Quantity / 1000; // length of material of anchor's batch in metres
            double BilletWeight = anchor.BilletLength * (Math.PI * Math.Pow(anchor.Diameter, 2) / 4) / Math.Pow(10, 9) * dencitySteel; // weight of anchor's billet
            anchor.Weight = Math.Round(BilletWeight - ((Math.PI * Math.Pow(anchor.Diameter, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameter, 2) / 4)) * anchor.ThreadLength / Math.Pow(10, 9) * dencitySteel, 2); // weight of anchor
            if (anchor.Kind == Kind.BendDouble)
                anchor.Weight = Math.Round(BilletWeight - 2 * ((((Math.PI * Math.Pow(anchor.Diameter, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameter, 2) / 4)) * anchor.ThreadLength) / Math.Pow(10, 9)) * dencitySteel, 2);

            anchor.BatchWeight =  Math.Round(anchor.Weight * anchor.Quantity, 2); // weight of anchor's batch  

            int quantityBend = anchor.Kind switch
            {
                Kind.Bend => 1,
                Kind.BendDouble => 2,
                _ => 0,
            };

            double priceBend = costWork.TimeBend * costWork.PnrBendingAnchor * quantityBend; // price of bending in $
            double priceThreadRolling = 0;
            double priceThreadCutting = 0;
            double timeProductionThread = 0;
            double timeProductionBend = 0;
            double timeProductionBandSaw = 0;
            double timeProduction = 0;
            double additPriceCutWithoutBindThreadMaterial = 0; // additional costwork if is it neccessary cutting diameter material before thread diameter
            double timeCutWithoutBindThreadMaterial = 0;

            timeProduction += costWork.TimeBend * quantityBend;
            timeProductionBend += costWork.TimeBend * quantityBend;

            if (anchor.ThreadLength > 0)
            {
                if (anchor.Kind == Kind.BendDouble)
                {
                    priceThreadRolling = anchor.Material.TimeThreadRolling * (2 * anchor.ThreadLength / costWork.LengthEffective) * costWork.PnrRollingThread; // price of threadrolling in $
                    priceThreadCutting = anchor.Material.TimeThreadCutting * (2 * anchor.ThreadLength / costWork.LengthEffective) * costWork.AreaLockSmith
                             + anchor.Material.Cutter * costWork.PriceCutter + anchor.Material.Plashka * costWork.PricePlashka; // price of threadcutting in $ 
                    if (anchor.ProductionId != 0)
                    {
                        timeProduction += anchor.Material.TimeThreadRolling * (2 * anchor.ThreadLength / costWork.LengthEffective);
                        timeProduction += costWork.TimeSetThreadRolling / anchor.Quantity;
                        timeProductionThread += anchor.Material.TimeThreadRolling * (2 * anchor.ThreadLength / costWork.LengthEffective);
                        timeProductionThread += costWork.TimeSetThreadRolling / anchor.Quantity;
                    }
                    else
                    {
                        timeProduction += anchor.Material.TimeThreadCutting * (2 * anchor.ThreadLength / costWork.LengthEffective);
                        timeProductionThread += anchor.Material.TimeThreadCutting * (2 * anchor.ThreadLength / costWork.LengthEffective);
                    }
                    if (anchor.WithoutBindThreadDiamMatetial && anchor.ThreadDiameter < anchor.Diameter - 1)
                        timeCutWithoutBindThreadMaterial = anchor.Material.TimeThreadRolling * (2 * anchor.ThreadLength / (costWork.LengthEffective / anchor.Quantity));
                }
                else
                {
                    priceThreadRolling = anchor.Material.TimeThreadRolling * ((anchor.ThreadLength + anchor.ThreadLengthSecond) / costWork.LengthEffective) * costWork.PnrRollingThread; // price of threadrolling in $ 
                    priceThreadCutting = anchor.Material.TimeThreadCutting * ((anchor.ThreadLength + anchor.ThreadLengthSecond) / costWork.LengthEffective) * costWork.AreaLockSmith
                        + anchor.Material.Cutter * costWork.PriceCutter + anchor.Material.Plashka * costWork.PricePlashka; // price of threadcutting in $ 
                    if (anchor.ProductionId != 0)
                    {
                        timeProduction += anchor.Material.TimeThreadRolling * ((anchor.ThreadLength + anchor.ThreadLengthSecond) / costWork.LengthEffective);
                        timeProduction += costWork.TimeSetThreadRolling / anchor.Quantity;
                        timeProductionThread += anchor.Material.TimeThreadRolling * ((anchor.ThreadLength + anchor.ThreadLengthSecond) / costWork.LengthEffective);
                        timeProductionThread += costWork.TimeSetThreadRolling / anchor.Quantity;
                    }
                    else
                    {
                        timeProduction += anchor.Material.TimeThreadCutting * ((anchor.ThreadLength + anchor.ThreadLengthSecond) / costWork.LengthEffective);
                        timeProductionThread += anchor.Material.TimeThreadCutting * ((anchor.ThreadLength + anchor.ThreadLengthSecond) / costWork.LengthEffective);
                    }
                    if (anchor.WithoutBindThreadDiamMatetial && anchor.ThreadDiameter < anchor.Diameter - 1)
                        timeCutWithoutBindThreadMaterial = anchor.Material.TimeThreadRolling * ((anchor.ThreadLength + anchor.ThreadLengthSecond) / (costWork.LengthEffective * anchor.Quantity));
                }
                if (anchor.WithoutBindThreadDiamMatetial && anchor.ThreadDiameter < anchor.Diameter - 1)
                    additPriceCutWithoutBindThreadMaterial = priceThreadCutting;
            }
            else
            {
                costWork.TimeSetThreadRolling = 0;
                costWork.PnrRollingThread = 0;
            }

            double priceBandSaw = anchor.Material.TimeBandSaw * costWork.PnrBandSaw + anchor.Material.LengthBladeBandSaw * costWork.PriceBandSaw; // price of band saw in $
            double priceMaterialAnchor = ((anchor.BilletLength / 1000) * anchor.Material.PricePerMetr) / costWork.ExchangeDollar; // price of anchor's material in $

            timeProduction += anchor.Material.TimeBandSaw;
            timeProductionBandSaw += anchor.Material.TimeBandSaw;

            double setBend = 0;
            if (anchor.Kind != Kind.Straight)
            { 
                setBend = costWork.TimeSetBend * costWork.PnrBendingAnchor;
                timeProduction += costWork.TimeSetBend;
                timeProductionBend += costWork.TimeSetBend / anchor.Quantity;
            }

            double costWorkInterm;
            if (anchor.ProductionId != 0)
            {
                costWorkInterm = (priceBend + priceThreadRolling + priceBandSaw + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity
                     + costWork.TimeSetThreadRolling * costWork.PnrRollingThread + setBend;
                anchor.BatchPriceProductionThread = ((priceThreadRolling + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity + costWork.TimeSetThreadRolling * costWork.PnrRollingThread) * costWork.ExchangeDollar * (1 + costWork.Margin) * 1.1; // sebes of batch anchor's thread production in som              
            }
            else
            { 
                costWorkInterm = (priceBend + priceThreadCutting + priceBandSaw + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity + setBend;
                anchor.BatchPriceProductionThread = (priceThreadCutting + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity * costWork.ExchangeDollar * (1 + costWork.Margin) * 1.1; // sebes of batch anchor's thread production in som
            }

            anchor.BatchSebes = (costWorkInterm + priceMaterialAnchor * anchor.Quantity) * costWork.ExchangeDollar * 1.1; // sebes of batch anchor in som
            anchor.Sebes = anchor.BatchSebes / anchor.Quantity; // sebes of 1 anchor in som         
            anchor.BatchPriceMaterial = priceMaterialAnchor * costWork.ExchangeDollar * anchor.Quantity * 1.1; // price of anchor's material batch in som
            anchor.BatchPriceProductionBend = (priceBend * anchor.Quantity + setBend) * costWork.ExchangeDollar * (1 + costWork.Margin) * 1.1; // sebes of batch anchor's bend production in som        
            anchor.BatchPriceProductionBandSaw = priceBandSaw * anchor.Quantity * costWork.ExchangeDollar * (1 + costWork.Margin) * 1.1; // sebes of batch anchor's bandSaw production in som
            if (anchor.ThreadDiameter < 30)
                anchor.Amount = (costWorkInterm * (1 + costWork.Margin) + priceMaterialAnchor * anchor.Quantity) * costWork.ExchangeDollar * 1.1; // total amount in som
            else
                anchor.Amount = (costWorkInterm * (1 + costWork.MarginFB) + priceMaterialAnchor * anchor.Quantity) * costWork.ExchangeDollar * 1.1; // total amount in som            
            //if (anchor.Quantity < 50)
            //{ 
            //    anchor.Amount *= 2;
            //    anchor.BatchPriceMaterial *= 2;
            //    anchor.BatchPriceProductionThread *= 2;
            //    anchor.BatchPriceProductionBend *= 2;
            //    anchor.BatchPriceProductionBandSaw *= 2;
            //}
            anchor.Price = anchor.Amount / anchor.Quantity; // amount of 1 anchor in som
            anchor.TimeProductionThread = (timeProductionThread + timeCutWithoutBindThreadMaterial) * anchor.Quantity; // time of anchor's thread production in hours
            anchor.TimeProductionBend = timeProductionBend * anchor.Quantity; // time of anchor's bend production in hours
            anchor.TimeProductionBandSaw = timeProductionBandSaw * anchor.Quantity; // time of anchor's bandSaw production in hours
        }           
    }
}
