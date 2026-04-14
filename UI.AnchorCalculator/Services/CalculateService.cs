using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using UI.AnchorCalculator.Extensions;
using UI.AnchorCalculator.Utils;

namespace UI.AnchorCalculator.Services;

public class CalculateService
{
	const double STEEL_DENSITY = 7850;
	const double HIDDEN_MARKUP_PERCENT = 110;

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
			m_Logger.LogDebug($"Error: {ex.Message}");
			throw;
		}

		double hiddenMarkupPercent = HIDDEN_MARKUP_PERCENT / 100.0;

		anchor.BilletLengthMillimeters = CalculParams.GetBilletLength(anchor);
		anchor.FullLengthMeters = anchor.BilletLengthMillimeters * anchor.Quantity / 1000; // Length of material of anchor's batch in meters
		double billetWeight = anchor.BilletLengthMillimeters * (Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) / Math.Pow(10, 9) * STEEL_DENSITY; // Weight of anchor's billet
		anchor.WeightKg = billetWeight - ((Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameterMillimeters, 2) / 4)) * anchor.ThreadLengthMillimeters / Math.Pow(10, 9) * STEEL_DENSITY; // Weight of anchor
		if (anchor.Kind == AnchorKind.DoubleBend)
			anchor.WeightKg = billetWeight - 2 * ((((Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameterMillimeters, 2) / 4)) * anchor.ThreadLengthMillimeters) / Math.Pow(10, 9)) * STEEL_DENSITY;

		anchor.BatchWeightKg = anchor.WeightKg * anchor.Quantity; // Weight of anchor's batch

		int bendCount = anchor.Kind switch
		{
			AnchorKind.SingleBend => 1,
			AnchorKind.DoubleBend => 2,
			_ => 0,
		};

		double bendPriceDollars = workCost.BendHours * workCost.PnrBendingAnchorDollars * bendCount;
		double threadRollingPriceDollars = 0;
		double threadCuttingPriceDollars = 0;

		double productionHours = 0;

		double additPriceCutWithoutBindThreadMaterial = 0; // Additional work cost if it is necessary cutting diameter material before thread diameter
		double timeCutWithoutBindThreadMaterial = 0;

		double productionBendHours = workCost.BendHours * bendCount;
		productionHours += productionBendHours;

		double productionThreadHours = 0;
		if (anchor.ThreadLengthMillimeters > 0)
		{
			double threadLengths = 0;

			if (anchor.Kind == AnchorKind.DoubleBend)
			{
				threadLengths = 2 * anchor.ThreadLengthMillimeters;

				if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
					timeCutWithoutBindThreadMaterial = anchor.Material.ThreadRollingHours * (threadLengths / (workCost.EffectiveLengthMillimeters / anchor.Quantity)); // TODO: Why one condition has / and other has *
			}
			else
			{
				threadLengths = anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters;

				if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
					timeCutWithoutBindThreadMaterial = anchor.Material.ThreadRollingHours * (threadLengths / (workCost.EffectiveLengthMillimeters * anchor.Quantity)); // TODO: Why one condition has * and other has /
			}

			threadLengths /= workCost.EffectiveLengthMillimeters;

			threadRollingPriceDollars = anchor.Material.ThreadRollingHours * threadLengths * workCost.PnrRollingThreadDollars;
			threadCuttingPriceDollars = anchor.Material.ThreadCuttingHours * threadLengths * workCost.PnrMetalworkingAreaDollars + anchor.Material.CutterCount * workCost.CutterPriceDollars + anchor.Material.PlashkaCount * workCost.PlashkaPriceDollars;

			if (anchor.ProductionId != 0)
			{
				productionHours += anchor.Material.ThreadRollingHours * (threadLengths / workCost.EffectiveLengthMillimeters);
				productionHours += workCost.SetThreadRollingHours / anchor.Quantity;

				productionThreadHours += anchor.Material.ThreadRollingHours * (threadLengths / workCost.EffectiveLengthMillimeters);
				productionThreadHours += workCost.SetThreadRollingHours / anchor.Quantity;
			}
			else
			{
				productionHours += anchor.Material.ThreadCuttingHours * (threadLengths / workCost.EffectiveLengthMillimeters);

				productionThreadHours += anchor.Material.ThreadCuttingHours * (threadLengths / workCost.EffectiveLengthMillimeters);
			}

			if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
				additPriceCutWithoutBindThreadMaterial = threadCuttingPriceDollars;
		}
		else
		{
			workCost.SetThreadRollingHours = 0;

			workCost.PnrRollingThreadDollars = 0;
		}

		double bandSawPriceDollars = anchor.Material.BandSawHours * workCost.PnrBandSawDollars + anchor.Material.BandSawBladeCount * workCost.BandSawPriceDollars;
		double materialAnchorPriceDollars = ((anchor.BilletLengthMillimeters / 1000) * anchor.Material.PricePerMeter) / workCost.ExchangeDollar;

		double productionBandSawHours = anchor.Material.BandSawHours;
		productionHours += productionBandSawHours;

		double setBend = 0;
		if (anchor.Kind != AnchorKind.Straight)
		{
			setBend = workCost.SetBendHours * workCost.PnrBendingAnchorDollars;
			productionHours += workCost.SetBendHours;
			productionBendHours += workCost.SetBendHours / anchor.Quantity;
		}

		double totalExchangeWithMarkup = workCost.ExchangeDollar * (1.0 + workCost.MarkupPercent) * hiddenMarkupPercent;

		double workCostInterm;
		if (anchor.ProductionId != 0)
		{
			workCostInterm = (bendPriceDollars + threadRollingPriceDollars + bandSawPriceDollars + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity + workCost.SetThreadRollingHours * workCost.PnrRollingThreadDollars + setBend;
			anchor.BatchPriceProductionThread = ((threadRollingPriceDollars + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity + workCost.SetThreadRollingHours * workCost.PnrRollingThreadDollars) * totalExchangeWithMarkup; // Sebes of batch anchor's thread production in som
		}
		else
		{
			workCostInterm = (bendPriceDollars + threadCuttingPriceDollars + bandSawPriceDollars + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity + setBend;
			anchor.BatchPriceProductionThread = (threadCuttingPriceDollars + additPriceCutWithoutBindThreadMaterial) * anchor.Quantity * totalExchangeWithMarkup; // Sebes of batch anchor's thread production in som
		}

		anchor.BatchSebes = (workCostInterm + materialAnchorPriceDollars * anchor.Quantity) * workCost.ExchangeDollar * hiddenMarkupPercent; // Sebes of batch anchor in som
		anchor.Sebes = anchor.BatchSebes / anchor.Quantity; // Sebes of 1 anchor in som
		anchor.BatchPriceMaterial = materialAnchorPriceDollars * workCost.ExchangeDollar * anchor.Quantity * hiddenMarkupPercent; // Price of anchor's material batch in som
		anchor.BatchPriceProductionBend = (bendPriceDollars * anchor.Quantity + setBend) * totalExchangeWithMarkup; // Sebes of batch anchor's bend production in som
		anchor.BatchPriceProductionBandSaw = bandSawPriceDollars * anchor.Quantity * totalExchangeWithMarkup; // Sebes of batch anchor's bandSaw production in som
		double markupPercent = 1.0 + workCost.MarkupPercent;
		if (anchor.ThreadDiameterMillimeters > 30) markupPercent += workCost.AdditionalMarkupPercent_DiameterMoreThan30;
		anchor.TotalCost = (workCostInterm * markupPercent + materialAnchorPriceDollars * anchor.Quantity) * workCost.ExchangeDollar * hiddenMarkupPercent; // Total cost in som

		anchor.Price = anchor.TotalCost / anchor.Quantity; // Cost of 1 anchor in som
		anchor.ProductionThreadHours = (productionThreadHours + timeCutWithoutBindThreadMaterial) * anchor.Quantity; // Time of anchor's thread production in hours
		anchor.ProductionBendHours = productionBendHours * anchor.Quantity; // Time of anchor's bend production in hours
		anchor.ProductionBandSawHours = productionBandSawHours * anchor.Quantity;
	}
}
