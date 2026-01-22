using Core.AnchorCalculator.Entities.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Routing.Constraints;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = true)]
    public class CheckThreadLength : ValidationAttribute
    {
        private readonly string _hasCuttingThreadlPropertyName;

        public CheckThreadLength(string hasCuttingThreadlPropertyName)
        {
            _hasCuttingThreadlPropertyName = hasCuttingThreadlPropertyName;
        }

        //https://makolyte.com/aspnetcore-client-side-custom-validation-attributes/
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            static float GetLengthWithoutBend(float length, float bendRadius, float diameter)  => length - bendRadius - diameter;
  
            var model = validationContext.ObjectInstance as AnchorViewModel;
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;
            else
            {
                var treadLength = (int)value;
                var treadLengthFloat = (float)treadLength;
                var diameter = float.TryParse(model.Diameter, out float diameterVal) ? diameterVal : 0;
                var length = (float)model.Length; 
                var lengthSecond = model.LengthSecond != 0 ? (float)model.LengthSecond : (float)model.Length;                
                var bendRadius = (float)model.BendRadius;
                var LengthWithoutBendFirst = GetLengthWithoutBend(length, bendRadius, diameter);
                var LengthWithoutBendSecond = GetLengthWithoutBend(lengthSecond, bendRadius, diameter);
                var kindAnchor = Enum.TryParse<Kind>(model.Kind, out var kind) ? kind : 0;

                if (kindAnchor == Kind.BendDouble)
                {
                    if (treadLengthFloat > LengthWithoutBendFirst)
                        return new ValidationResult($"Укажите длину не менее {LengthWithoutBendFirst} мм");
                    if (treadLengthFloat > LengthWithoutBendSecond)
                        return new ValidationResult($"Укажите длину не менее {LengthWithoutBendSecond} мм");
                }
                else
                { 
                    if (treadLength < 50 || treadLength > 6000)
                        return new ValidationResult("Укажите длину от 50 до 6000");                
                }
            }               
            return ValidationResult.Success;
        }
    }
}
