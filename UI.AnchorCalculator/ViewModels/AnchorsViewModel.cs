using Core.AnchorCalculator.Entities;

namespace UI.AnchorCalculator.ViewModels;

public class AnchorsViewModel
{
	public List<Anchor> Anchors { get; set; }
	public FilterViewModelAnchors FilterView { get; set; }
	public PageViewModelAnchors PageViewModelAnchors { get; set; }
}
