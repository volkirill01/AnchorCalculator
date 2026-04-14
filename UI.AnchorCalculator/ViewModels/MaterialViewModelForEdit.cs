using System.ComponentModel.DataAnnotations;

namespace UI.AnchorCalculator.ViewModels;

public class MaterialViewModelForEdit
{
	public int Id { get; set; }

	[Display(Name = "Название")]
	public string? Name { get; set; }

	[Range(6, Double.MaxValue, ErrorMessage = "Размер должен быть не меньше 6")]
	[Display(Name = "Размер")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double Size { get; set; } // TODO: Figure out units

	[Display(Name = "Тип сечения")]
	public int TypeId { get; set; }

	public Core.AnchorCalculator.Entities.Enums.Type Type { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите цену за метр")]
	[Display(Name = "Цена за метр (сом)")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double PricePerMeter { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите время накатки резьбы")]
	[Display(Name = "Время накатки резьбы (ч)")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double TheradRollingHours { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите время нарезки резьбы")]
	[Display(Name = "Время нарезки резьбы (ч)")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double ThreadCuttingHours { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите кол-во плашки")]
	[Display(Name = "Кол-во плашки (шт)")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double PlashkaCount { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите кол-во резца")]
	[Display(Name = "Кол-во резца (шт)")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double CutterCount { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите время лентопила")]
	[Display(Name = "Время лентопила (ч)")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double BandSawHours { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите длину полотна лентопила")]
	[Display(Name = "Полотно лентопила (шт)")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double BandSawBladeCount { get; set; }

	public Array? Types { get; set; }
	public string[]? Names { get; set; }
}
