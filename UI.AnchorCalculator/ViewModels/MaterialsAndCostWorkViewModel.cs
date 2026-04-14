using Core.AnchorCalculator.Entities;
using UI.AnchorCalculator.Extensions;

namespace UI.AnchorCalculator.ViewModels;

public class MaterialsAndWorkPriceViewModel
{
	public List<Material>? Materials { get; set; }

	public WorkPrice? WorkPrice { get; set; }

	public double MarkupPercent
	{
		get => WorkPrice.MarkupPercent * 100;
		set => WorkPrice.MarkupPercent = value / 100;
	}

	public double AdditionalMarkupPercent_DiameterMoreThan30
	{
		get => WorkPrice.AdditionalMarkupPercent_DiameterMoreThan30 * 100;
		set => WorkPrice.AdditionalMarkupPercent_DiameterMoreThan30 = value / 100;
	}
}
