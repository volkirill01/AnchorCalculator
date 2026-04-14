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
		WorkPrice workPrice = new();
		try
		{
			workPrice = await workPrice.GetWorkPrice(m_AppEnvironment);
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
		anchor.WeightKg_SingleAnchor = billetWeight - ((Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameterMillimeters, 2) / 4)) * anchor.ThreadLengthMillimeters / Math.Pow(10, 9) * STEEL_DENSITY; // Weight of anchor
		if (anchor.Kind == AnchorKind.DoubleBend)
			anchor.WeightKg_SingleAnchor = billetWeight - 2 * ((((Math.PI * Math.Pow(anchor.DiameterMillimeters, 2) / 4) - (Math.PI * Math.Pow(anchor.ThreadDiameterMillimeters, 2) / 4)) * anchor.ThreadLengthMillimeters) / Math.Pow(10, 9)) * STEEL_DENSITY;
		anchor.WeightKg_SingleAnchor = Math.Round(anchor.WeightKg_SingleAnchor, 2);

		anchor.WeightKg_Total = Math.Round(anchor.WeightKg_SingleAnchor * anchor.Quantity, 2); // Weight of anchor's batch

		int bendCount = anchor.Kind switch
		{
			AnchorKind.SingleBend => 1,
			AnchorKind.DoubleBend => 2,
			_ => 0,
		};

		double bendPriceDollars = workPrice.BendHours * workPrice.PnrBendingAnchorDollars * bendCount;
		double threadRollingPriceDollars = 0;
		double threadCuttingPriceDollars = 0;

		double productionHours = 0;

		double additionalPriceCutWithoutBindThreadMaterial = 0; // Additional work price if it is necessary cutting diameter material before thread diameter
		double timeCutWithoutBindThreadMaterial = 0;

		double productionThreadHours = 0;
		if (anchor.ThreadLengthMillimeters > 0)
		{
			double threadLengths = 0;

			if (anchor.Kind == AnchorKind.DoubleBend)
			{
				threadLengths = 2 * anchor.ThreadLengthMillimeters;

				if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
					timeCutWithoutBindThreadMaterial = anchor.Material.ThreadRollingHours * (threadLengths / (workPrice.EffectiveLengthMillimeters / anchor.Quantity)); // TODO: Why one condition has / and other has *
			}
			else
			{
				threadLengths = anchor.ThreadLengthMillimeters + anchor.ThreadSecondLengthMillimeters;

				if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
					timeCutWithoutBindThreadMaterial = anchor.Material.ThreadRollingHours * (threadLengths / (workPrice.EffectiveLengthMillimeters * anchor.Quantity)); // TODO: Why one condition has * and other has /
			}

			threadLengths /= workPrice.EffectiveLengthMillimeters;

			threadRollingPriceDollars = anchor.Material.ThreadRollingHours * threadLengths * workPrice.PnrRollingThreadDollars;
			threadCuttingPriceDollars = anchor.Material.ThreadCuttingHours * threadLengths * workPrice.PnrMetalworkingAreaDollars + anchor.Material.CutterCount * workPrice.CutterPriceDollars + anchor.Material.PlashkaCount * workPrice.PlashkaPriceDollars;

			if (anchor.ThreadProductionTypeId != 0)
			{
				productionHours += anchor.Material.ThreadRollingHours * (threadLengths / workPrice.EffectiveLengthMillimeters);
				productionHours += workPrice.ThreadRollingSettingHours / anchor.Quantity;

				productionThreadHours = anchor.Material.ThreadRollingHours * (threadLengths / workPrice.EffectiveLengthMillimeters) + workPrice.ThreadRollingSettingHours / anchor.Quantity;
			}
			else
			{
				productionHours += anchor.Material.ThreadCuttingHours * (threadLengths / workPrice.EffectiveLengthMillimeters);

				productionThreadHours = anchor.Material.ThreadCuttingHours * (threadLengths / workPrice.EffectiveLengthMillimeters);
			}

			if (anchor.WithoutBindThreadDiamMaterial && anchor.ThreadDiameterMillimeters < anchor.DiameterMillimeters - 1)
				additionalPriceCutWithoutBindThreadMaterial = threadCuttingPriceDollars;
		}
		else
		{
			workPrice.ThreadRollingSettingHours = 0;

			workPrice.PnrRollingThreadDollars = 0;
		}

		double bandSawPriceDollars = anchor.Material.BandSawHours * workPrice.PnrBandSawDollars + anchor.Material.BandSawBladeCount * workPrice.BandSawPriceDollars;
		double materialAnchorPriceDollars = ((anchor.BilletLengthMillimeters / 1000) * anchor.Material.PricePerMeter) / workPrice.ExchangeDollar;

		double productionBandSawHours = anchor.Material.BandSawHours;
		productionHours += productionBandSawHours;

		double productionBendHours = workPrice.BendHours * bendCount;
		productionHours += productionBendHours;

		double bendSettingHours = 0;
		if (anchor.Kind != AnchorKind.Straight)
		{
			bendSettingHours = workPrice.BendSettingHours * workPrice.PnrBendingAnchorDollars;
			productionHours += workPrice.BendSettingHours;
			productionBendHours += workPrice.BendSettingHours / anchor.Quantity;
		}

		double totalExchangeWithMarkup = workPrice.ExchangeDollar * (1.0 + workPrice.MarkupPercent) * hiddenMarkupPercent;

		double workPriceInterm;
		if (anchor.ThreadProductionTypeId != 0)
		{
			workPriceInterm = (bendPriceDollars + threadRollingPriceDollars + bandSawPriceDollars + additionalPriceCutWithoutBindThreadMaterial) * anchor.Quantity + workPrice.ThreadRollingSettingHours * workPrice.PnrRollingThreadDollars + bendSettingHours;
			anchor.PriceSom_ProductionThread_Total = ((threadRollingPriceDollars + additionalPriceCutWithoutBindThreadMaterial) * anchor.Quantity + workPrice.ThreadRollingSettingHours * workPrice.PnrRollingThreadDollars) * totalExchangeWithMarkup;
		}
		else
		{
			workPriceInterm = (bendPriceDollars + threadCuttingPriceDollars + bandSawPriceDollars + additionalPriceCutWithoutBindThreadMaterial) * anchor.Quantity + bendSettingHours;
			anchor.PriceSom_ProductionThread_Total = (threadCuttingPriceDollars + additionalPriceCutWithoutBindThreadMaterial) * anchor.Quantity * totalExchangeWithMarkup;
		}
		anchor.PriceSom_ProductionThread_Total = Math.Ceiling(anchor.PriceSom_ProductionThread_Total);

		anchor.SebesSom_Total = Math.Ceiling((workPriceInterm + materialAnchorPriceDollars * anchor.Quantity) * workPrice.ExchangeDollar * hiddenMarkupPercent);
		anchor.SebesSom_SingleAnchor = Math.Ceiling(anchor.SebesSom_Total / anchor.Quantity);
		anchor.PriceSom_Material_Total = Math.Ceiling(materialAnchorPriceDollars * workPrice.ExchangeDollar * anchor.Quantity * hiddenMarkupPercent);
		anchor.PriceSom_ProductionBend_Total = Math.Ceiling((bendPriceDollars * anchor.Quantity + bendSettingHours) * totalExchangeWithMarkup);
		anchor.PriceSom_ProductionBandSaw_Total = Math.Ceiling(bandSawPriceDollars * anchor.Quantity * totalExchangeWithMarkup);
		double markupPercent = 1.0 + workPrice.MarkupPercent;
		if (anchor.ThreadDiameterMillimeters > 30) markupPercent += workPrice.AdditionalMarkupPercent_DiameterMoreThan30;
		anchor.PriceSom_Total = Math.Ceiling((workPriceInterm * markupPercent + materialAnchorPriceDollars * anchor.Quantity) * workPrice.ExchangeDollar * hiddenMarkupPercent);

		anchor.PriceSom_SingleAnchor = Math.Ceiling(anchor.PriceSom_Total / anchor.Quantity);
		anchor.ProductionHours_Thread = Math.Round((productionThreadHours + timeCutWithoutBindThreadMaterial) * anchor.Quantity, 2);
		anchor.ProductionHours_Bend = Math.Round(productionBendHours * anchor.Quantity, 2);
		anchor.ProductionHours_BandSaw = Math.Round(productionBandSawHours * anchor.Quantity, 2);
	}
}
