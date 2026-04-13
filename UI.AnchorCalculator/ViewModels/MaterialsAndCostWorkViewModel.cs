using Core.AnchorCalculator.Entities;
using UI.AnchorCalculator.Extensions;

namespace UI.AnchorCalculator.ViewModels;

public class MaterialsAndWorkCostViewModel
{
	public List<Material>? Materials { get; set; }

	public WorkCost? WorkCost { get; set; }

	public double MarkupPercent
	{
		get => WorkCost.MarkupPercent * 100;
		set => WorkCost.MarkupPercent = value / 100;
	}

	public double AdditionalMarkupPercent_DiameterMoreThan30
	{
		get => WorkCost.AdditionalMarkupPercent_DiameterMoreThan30 * 100;
		set => WorkCost.AdditionalMarkupPercent_DiameterMoreThan30 = value / 100;
	}
}
