using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.Utils;

namespace UI.AnchorCalculator.Services;

public class CalculateService
{
	const double STEEL_DENSITY = 7850;

	private readonly LoggerManager m_Logger;
	private readonly IWebHostEnvironment m_AppEnvironment;

	public CalculateService(IWebHostEnvironment appEnvironment, LoggerManager logger)
	{
		m_AppEnvironment = appEnvironment;
		m_Logger = logger;
	}

	public async Task Calculate(Anchor anchor)
	{
		WorkCost workCost = new();
		try
		{
			workCost = await workCost.GetWorkCost(m_AppEnvironment);
		}
		catch (Exception ex)
		{
			string exception = $"Error:{ex.Message}";
			m_Logger.LogDebug(exception);
			throw;
		}

		anchor.BilletLengthMillimeters = CalculParams.GetLengthBillet(anchor);
		anchor.FullLengthMeters = anchor.BilletLengthMillimeters * anchor.Quantity / 1000; // Length of material of anchor's batch in meters
		double BilletWeight = anchor.BilletLengthMillimeters * (Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) / Math.Pow(10, 9) * STEEL_DENSITY; // Weight of anchor's billet
		anchor.WeightKg = BilletWeight - ((Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameterMillimeters, 2) / 4)) * anchor.ThreadLengthMillimeters / Math.Pow(10, 9) * STEEL_DENSITY; // Weight of anchor
		if (anchor.Kind == AnchorKind.DoubleBend)
			anchor.WeightKg = BilletWeight - 2 * ((((Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameterMillimeters, 2) / 4)) * anchor.ThreadLengthMillimeters) / Math.Pow(10, 9)) * STEEL_DENSITY;

		anchor.BatchWeightKg = anchor.WeightKg * anchor.Quantity; // Weight of anchor's batch

		int quantityBend = anchor.Kind switch
		{
			AnchorKind.SingleBend => 1,
			AnchorKind.DoubleBend => 2,
			_ => 0,
		};

		double priceBend = workCost.BendHours * workCost.PnrBendingAnchorDollars * quantityBend; // Price of bending in $
		double priceThreadRolling = 0;
		double priceThreadCutting = 0;
		double timeProductionThread = 0;
		double timeProductionBend = 0;
		double timeProductionBandSaw = 0;
		double timeProduction = 0;
		double additPriceCutWithoutBindThreadMaterial = 0; // Additional work cost if is it necessary cutting diameter material before thread diameter
		double timeCutWithoutBindThreadMaterial = 0;

		timeProduction += workCost.BendHours * quantityBend;
		timeProductionBend += workCost.BendHours * quantityBend;

		if (anchor.ThreadLengthMillimeters > 0)
		{
			if (anchor.Kind == AnchorKind.DoubleBend)
			{
				priceThreadRolling = anchor.Material.ThreadRollingHours * (2 * anchor.ThreadLengthMillimeters / workCost.EffectiveLengthMillimeters) * workCost.PnrRollingThreadDollars; // Price of thread rolling in $
				priceThreadCutting = anchor.Material.ThreadCuttingHours * (2 * anchor.ThreadLengthMillimeters / workCost.EffectiveLengthMillimeters) * workCost.AreaLockSmithDollars
							+ anchor.Material.CutterCount * workCost.CutterPriceDollars + anchor.Material.PlashkaCount * workCost.PlashkaPriceDollars; // Price of thread cutting in $
				if (anchor.ProductionId != 0)
				{
					timeProduction += anchor.Material.ThreadRollingHours * (2 * anchor.ThreadLengthMillimeters / workCost.EffectiveLengthMillimeters);
					timeProduction += workCost.SetThreadRollingHours / anchor.Quantity;
					timeProductionThread += anchor.Material.ThreadRollingHours * (2 * anchor.ThreadLengthMillimeters / workCost.EffectiveLengthMillimeters);
					timeProductionThread += workCost.SetThreadRollingHours / anchor.Quantity;
				}
				else
				{
					timeProduction += anchor.Material.ThreadCuttingHours * (2 * anchor.ThreadLengthMillimeters / workCost.EffectiveLengthMillimeters);
					timeProductionThread += anchor.Material.ThreadCuttingHours * (2 * anchor.ThreadLengthMillimeters / workCost.EffectiveLengthMillimeters);
				}
				if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
					timeCutWithoutBindThreadMaterial = anchor.Material.ThreadRollingHours * (2 * anchor.ThreadLengthMillimeters / (workCost.EffectiveLengthMillimeters / anchor.Quantity));
			}
			else
			{
				priceThreadRolling = anchor.Material.ThreadRollingHours * ((anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters) / workCost.EffectiveLengthMillimeters) * workCost.PnrRollingThreadDollars; // Price of thread rolling in $
				priceThreadCutting = anchor.Material.ThreadCuttingHours * ((anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters) / workCost.EffectiveLengthMillimeters) * workCost.AreaLockSmithDollars
					+ anchor.Material.CutterCount * workCost.CutterPriceDollars + anchor.Material.PlashkaCount * workCost.PlashkaPriceDollars; // Price of thread cutting in $
				if (anchor.ProductionId != 0)
				{
					timeProduction += anchor.Material.ThreadRollingHours * ((anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters) / workCost.EffectiveLengthMillimeters);
					timeProduction += workCost.SetThreadRollingHours / anchor.Quantity;
					timeProductionThread += anchor.Material.ThreadRollingHours * ((anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters) / workCost.EffectiveLengthMillimeters);
					timeProductionThread += workCost.SetThreadRollingHours / anchor.Quantity;
				}
				else
				{
					timeProduction += anchor.Material.ThreadCuttingHours * ((anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters) / workCost.EffectiveLengthMillimeters);
					timeProductionThread += anchor.Material.ThreadCuttingHours * ((anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters) / workCost.EffectiveLengthMillimeters);
				}
				if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
					timeCutWithoutBindThreadMaterial = anchor.Material.ThreadRollingHours * ((anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters) / (workCost.EffectiveLengthMillimeters * anchor.Quantity));
			}
			if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
				additPriceCutWithoutBindThreadMaterial = priceThreadCutting;
		}
		else
		{
			workCost.SetThreadRollingHours = 0;
			workCost.PnrRollingThreadDollars = 0;
		}

		double BandSawPriceDollars = anchor.Material.BandSawHours * workCost.PnrBandSawDollars + anchor.Material.BandSawBladeLengthMeters * workCost.BandSawPriceDollars; // Price of band saw in $
		double priceMaterialAnchor = ((anchor.BilletLengthMillimeters / 1000) * anchor.Material.PricePerMeter) / workCost.ExchangeDollar; // Price of anchor's material in $

		timeProduction += anchor.Material.BandSawHours;
		timeProductionBandSaw += anchor.Material.BandSawHours;

		double setBend = 0;
		if (anchor.Kind != AnchorKind.Straight)
		{
			setBend = workCost.SetBendHours * workCost.PnrBendingAnchorDollars;
			timeProduction += workCost.SetBendHours;
			timeProductionBend += workCost.SetBendHours / anchor.Quantity;
		}

		double workCostInterm;
		if (anchor.ProductionId != 0)
		{
			workCostInterm = (priceBend + priceThreadRolling + BandSawPriceDollars + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity
					+ workCost.SetThreadRollingHours * workCost.PnrRollingThreadDollars + setBend;
			anchor.BatchPriceProductionThread = ((priceThreadRolling + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity + workCost.SetThreadRollingHours * workCost.PnrRollingThreadDollars) * workCost.ExchangeDollar * (1 + workCost.MarginPercent) * 1.1; // Sebes of batch anchor's thread production in som
		}
		else
		{
			workCostInterm = (priceBend + priceThreadCutting + BandSawPriceDollars + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity + setBend;
			anchor.BatchPriceProductionThread = (priceThreadCutting + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity * workCost.ExchangeDollar * (1 + workCost.MarginPercent) * 1.1; // Sebes of batch anchor's thread production in som
		}

		anchor.BatchSebes = (workCostInterm + priceMaterialAnchor * anchor.Quantity) * workCost.ExchangeDollar * 1.1; // Sebes of batch anchor in som
		anchor.Sebes = anchor.BatchSebes / anchor.Quantity; // Sebes of 1 anchor in som
		anchor.BatchPriceMaterial = priceMaterialAnchor * workCost.ExchangeDollar * anchor.Quantity * 1.1; // Price of anchor's material batch in som
		anchor.BatchPriceProductionBend = (priceBend * anchor.Quantity + setBend) * workCost.ExchangeDollar * (1 + workCost.MarginPercent) * 1.1; // Sebes of batch anchor's bend production in som
		anchor.BatchPriceProductionBandSaw = BandSawPriceDollars * anchor.Quantity * workCost.ExchangeDollar * (1 + workCost.MarginPercent) * 1.1; // Sebes of batch anchor's bandSaw production in som
		if (anchor.ThreadDiameterMillimeters < 30)
			anchor.TotalCost = (workCostInterm * (1 + workCost.MarginPercent) + priceMaterialAnchor * anchor.Quantity) * workCost.ExchangeDollar * 1.1; // Total cost in som
		else
			anchor.TotalCost = (workCostInterm * (1 + workCost.MarginFBPercent) + priceMaterialAnchor * anchor.Quantity) * workCost.ExchangeDollar * 1.1; // Total cost in som

		anchor.Price = anchor.TotalCost / anchor.Quantity; // Cost of 1 anchor in som
		anchor.ProductionThreadHours = (timeProductionThread + timeCutWithoutBindThreadMaterial) * anchor.Quantity; // Time of anchor's thread production in hours
		anchor.ProductionBendHours = timeProductionBend * anchor.Quantity; // Time of anchor's bend production in hours
		anchor.ProductionBandSawHours = timeProductionBandSaw * anchor.Quantity; // Time of anchor's bandSaw production in hours
	}
}
