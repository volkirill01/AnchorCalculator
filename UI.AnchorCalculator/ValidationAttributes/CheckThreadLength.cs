using Core.AnchorCalculator.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.ValidationAttributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
public class CheckThreadLength : ValidationAttribute
{
	//https://makolyte.com/aspnetcore-client-side-custom-validation-attributes/
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		static float GetLengthWithoutBend(float length, float bendRadius, float diameter)  => length - bendRadius - diameter;

		AnchorViewModel? model = validationContext.ObjectInstance as AnchorViewModel;
		if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
			return null;

		int treadLength = (int)value;
		float treadLengthFloat = (float)treadLength;
		float diameter = float.TryParse(model.DiameterMillimeters, out float diameterVal) ? diameterVal : 0;
		float length = (float)model.LengthMillimeters;
		float SecondLengthMillimeters = model.SecondLengthMillimeters != 0 ? (float)model.SecondLengthMillimeters : (float)model.LengthMillimeters;
		float bendRadius = (float)model.BendRadiusMillimeters;
		float lengthWithoutBendFirst = GetLengthWithoutBend(length, bendRadius, diameter);
		float lengthWithoutBendSecond = GetLengthWithoutBend(SecondLengthMillimeters, bendRadius, diameter);
		AnchorKind anchorKind = Enum.TryParse<AnchorKind>(model.Kind, out var kind) ? kind : 0;

		switch (anchorKind)
		{
			case AnchorKind.DoubleBend:
			{
				if (treadLengthFloat > lengthWithoutBendFirst)
					return new ValidationResult($"Укажите длину не менее {lengthWithoutBendFirst} мм");

				if (treadLengthFloat > lengthWithoutBendSecond)
					return new ValidationResult($"Укажите длину не менее {lengthWithoutBendSecond} мм");

				return ValidationResult.Success;
			}
			default:
			{
				if (treadLength < 50 || treadLength > 6000)
					return new ValidationResult("Укажите длину от 50 до 6000");

				return ValidationResult.Success;
			}
		}
	}
}
