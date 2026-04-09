using Core.AnchorCalculator.Entities;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using UI.AnchorCalculator.ValidationAttributes;

namespace UI.AnchorCalculator.ViewModels;

public class AnchorViewModel
{
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	[CheckLength()] // Min 1000 for hydraulic, 400 for rolling and cutting, min bendRadius + 60 for nonThread, BilletLength >= 400 мм for double bend radius anchor
	[Display(Name = "Длина, мм:")]
	public int LengthMillimeters { get; set; }

	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	[CheckLength()] // Min 1000 for hydraulic, 400 for rolling and cutting, min bendRadius + 60 for nonThread, BilletLength >= 400 мм for double bend radius anchor
	[Display(Name = "Длина вторая, мм:")]
	public int SecondLengthMillimeters { get; set; }

	[Required(ErrorMessage = "Выберите материал")]
	[Display(Name = "Диаметр, мм:")]
	public string DiameterMillimeters { get; set; }

	public string? WeightKg { get; set; }

	public string? Price { get; set; } // TODO: Figure out units

	[Display(Name = "Длина загиба, мм:")]
	public int BendLengthMillimeters { get; set; }

	[Display(Name = "Радиус загиба, мм:")]
	public int BendRadiusMillimeters { get; set; }

	[CheckThreadLength]
	[Display(Name = "Длина резьбы, мм:")]
	public int ThreadLengthMillimeters { get; set; }

	[CheckThreadLength]
	[Display(Name = "Длина резьбы, мм:")]
	public int ThreadSecondLengthMillimeters { get; set; }

	[ValidateNever]
	public bool HasSecondThread { get; set; }

	[ValidateNever]
	public bool HasThread { get; set; }

	[Display(Name = "Диаметр резьбы, мм:")]
	public int ThreadDiameterMillimeters { get; set; }

	[Display(Name = "Шаг резьбы, мм:")]
	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	public string ThreadStepMillimeters { get; set; }

	public string? TotalCost { get; set; } // TODO: Possibly make it int

	[Required(ErrorMessage = "Поле обязательно для заполнения")]
	[Range(1, Int32.MaxValue, ErrorMessage = "Кол-во не может быть равно 0")]
	[Display(Name = "Кол-во, шт:")]
	public int Quantity { get; set; } // TODO: Make uint

	public int TypeProfileId { get; set; }
	public DateTime DateCreate { get; set; }
	public string? Material { get; set; }
	public string? SvgElement { get; set; }
	public string? BatchWeightKg { get; set; } // TODO: Possibly make it float
	public string? BilletLengthMillimeters { get; set; } // TODO: Possibly make it float
	public double BatchPriceMaterial { get; set; } // TODO: Figure out units
	public double BatchPriceProductionThread { get; set; } // TODO: Figure out units
	public double BatchPriceProductionBend { get; set; } // TODO: Figure out units
	public double BatchPriceProductionBandSaw { get; set; } // TODO: Figure out units
	public double RollerPathLengthMillimeters { get; set; }
	public double RollerPathLengthMillimetersBeforeEnd { get; set; }
	public string? Sebes { get; set; } // TODO: Figure out units
	public string? BatchSebes { get; set; } // TODO: Figure out units
	public string? UserName { get; set; }
	public string? Kind { get; set; }
	public int ProductionId { get; set; }
	[ValidateNever]
	public bool WithoutBindThreadDiamMaterial { get; set; } // TODO: Whats this name?
	[ValidateNever]
	public bool WithoutBindRadiusBendDiamMaterial { get; set; } // TODO: Whats this name?
	[ValidateNever]
	public bool HasCuttingThread { get; set; }
	[ValidateNever]
	public bool HasVariableLength { get; set; } // TODO: Figure out units
	public double ProductionThreadHours { get; set; }
	public double ProductionBendHours { get; set; }
	public double ProductionBandSawHours { get; set; }
	public double FullLengthMeters { get; set; }
	public string? MaterialName { get; set; }
	[Display(Name = "Материал")]
	public int MaterialId { get; set; }
	public List<Material>? Materials { get; set; }
	public Anchor? Anchor { get; set; }
}
