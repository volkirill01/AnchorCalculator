using Core.AnchorCalculator.Entities;
using UI.AnchorCalculator.Extensions;

namespace UI.AnchorCalculator.ViewModels;

public class MaterialsAndWorkCostViewModel
{
	public List<Material>? Materials { get; set; }

	public WorkCost? WorkCost { get; set; }

	public double MarginPercent
	{
		get => WorkCost.MarginPercent * 100;
		set => WorkCost.MarginPercent = value / 100;
	}

	public double MarginFBPercent
	{
		get => WorkCost.MarginFBPercent * 100;
		set => WorkCost.MarginFBPercent = value / 100;
	}
}
