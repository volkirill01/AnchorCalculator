using Core.AnchorCalculator.Entities;

namespace UI.AnchorCalculator.ViewModels;

public class FilterViewModelAnchors
{
	public List<Material> MaterialList { get; private set; }
	public string? SelectedMaterial { get; private set; }
	public string SelectedUserName { get; private set; }
	public DateTime DateTimeFrom { get; private set; }
	public DateTime DateTimeTill { get; private set; }
	public double PriceFrom { get; private set; }
	public double PriceTill { get; private set; }

	public FilterViewModelAnchors(List<Material> materialList, string? selectedMaterial, string selectedUserName, DateTime dateTimeFrom, DateTime dateTimeTill, double priceFrom, double priceTill)
	{
		MaterialList = materialList;
		SelectedMaterial = selectedMaterial;
		SelectedUserName = selectedUserName;
		DateTimeFrom = dateTimeFrom;
		DateTimeTill = dateTimeTill;
		PriceFrom = priceFrom;
		PriceTill = priceTill;
	}
}
