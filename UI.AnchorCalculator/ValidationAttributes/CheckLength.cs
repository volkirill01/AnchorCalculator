using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using UI.AnchorCalculator.Utils;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class CheckLength : ValidationAttribute
{
	//https://makolyte.com/aspnetcore-client-side-custom-validation-attributes/
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		AnchorViewModel? model = validationContext.ObjectInstance as AnchorViewModel;
		double bendRadiusMillimeters = Convert.ToDouble(model.BendRadiusMillimeters);
		double firstLengthMillimeters = Convert.ToDouble(model.LengthMillimeters);
		double bendRadiusMillimetersMax = bendRadiusMillimeters + 60;
		AnchorKind kindAnchor = Enum.TryParse<AnchorKind>(model.Kind, out var kind) ? kind : 0;
		int maxBilletLengthForDoubleBend = 400;

		if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
			return null;

		int length = (int)value;
		switch (kindAnchor)
		{
			case AnchorKind.DoubleBend:
			{
				var anchorForBilletLength = new Anchor
				{
					DiameterMillimeters = float.TryParse(model.DiameterMillimeters, out float diameterVal) ? diameterVal : 0,
					BendRadiusMillimeters = model.BendRadiusMillimeters,
					Kind = kindAnchor,
					LengthMillimeters = model.LengthMillimeters,
					SecondLengthMillimeters = model.SecondLengthMillimeters != 0 ? model.SecondLengthMillimeters : model.LengthMillimeters,
					BendLengthMillimeters = model.BendLengthMillimeters
				};

				var billetLength = CalculParams.GetBilletLength(anchorForBilletLength);

				if (length <= 0)
					return new ValidationResult("Длина должна быть больше 0 мм");

				if (billetLength < maxBilletLengthForDoubleBend)
					return new ValidationResult($"Длина заготовки не должна быть меньше {maxBilletLengthForDoubleBend} мм. Текущая длина: {billetLength:F2} мм.");

				return ValidationResult.Success;
			}
			default:
			{
				if (length > firstLengthMillimeters)
					return new ValidationResult($"Длина второго конца должна быть меньше или равна длине первого");

				if (model.HasThread)
				{
					if (length < 400)
						return new ValidationResult("Укажите длину от 400");
				}
				else
				{
					if (length < bendRadiusMillimetersMax)
						return new ValidationResult($"Укажите длину от {bendRadiusMillimetersMax}");
				}
				return ValidationResult.Success;
			}
		}
	}
}
