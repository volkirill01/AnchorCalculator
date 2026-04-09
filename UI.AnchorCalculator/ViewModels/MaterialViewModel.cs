using System.ComponentModel.DataAnnotations;

namespace UI.AnchorCalculator.ViewModels;

public class MaterialViewModel
{
	[Display(Name = "Название")]
	public string? Name { get; set; }

	[Range(6, Double.MaxValue, ErrorMessage = "Размер должен быть не меньше 6")]
	[Display(Name = "Размер,мм")]
	public double Size { get; set; }

	[Display(Name = "Тип сечения")]
	public int TypeId { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите цену за метр")]
	[Display(Name = "Цена за метр,сом")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double PricePerMeter { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите время накатки резьбы")]
	[Display(Name = "Время накатки резьбы,ч")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double ThreadRollingHours { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите время нарезки резьбы")]
	[Display(Name = "Время нарезки резьбы,ч")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double ThreadCuttingHours { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите кол-во плашки")]
	[Display(Name = "Кол-во плашки, шт")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double PlashkaCount { get; set; } // TODO: Possibly make an int

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите кол-во резца")]
	[Display(Name = "Кол-во резца, шт")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double CutterCount { get; set; } // TODO: Possibly make an int

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите время лентопила")]
	[Display(Name = "Время лентопила,ч")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double BandSawHours { get; set; }

	[Range(0, Double.MaxValue, ErrorMessage = "Укажите длину полотна лентопила")]
	[Display(Name = "Длина полотна лентопила,м")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public double BandSawBladeLengthMeters { get; set; }

	public Array? Types { get; set; }
	public string[]? Names { get; set; }
}
