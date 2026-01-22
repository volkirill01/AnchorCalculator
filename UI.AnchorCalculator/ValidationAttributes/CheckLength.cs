using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using UI.AnchorCalculator.Utils;
using UI.AnchorCalculator.ViewModels;

namespace UI.AnchorCalculator.ValidationAttributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter,
        AllowMultiple = false, Inherited = true)]
    public class CheckLength : ValidationAttribute
    {       
        //https://makolyte.com/aspnetcore-client-side-custom-validation-attributes/
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var model = validationContext.ObjectInstance as AnchorViewModel;           
            var bendRadiusPropertyVal = Convert.ToDouble(model.BendRadius);
            var firstLengthPropertyVal = Convert.ToDouble(model.Length);
            double bendRadiusMax = bendRadiusPropertyVal + 60;
            var kindAnchor = Enum.TryParse<Kind>(model.Kind, out var kind) ? kind : 0;
            var maxBilletLengthForDoubleBend = 400;

            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
                return null;
            else
            {
                var length = (int)value;
                if (kindAnchor == Kind.BendDouble)
                {
                    var anchorForBilletLength = new Anchor
                    {
                        Diameter = float.TryParse(model.Diameter, out float diameterVal) ? diameterVal : 0,
                        BendRadius = model.BendRadius,
                        Kind = kindAnchor,
                        Length = model.Length,
                        LengthSecond = model.LengthSecond != 0 ? model.LengthSecond : model.Length,
                        BendLength = model.BendLength
                    };
                    var billetLength = CalculParams.GetLengthBillet(anchorForBilletLength);
                    if (length <= 0)
                        return new ValidationResult($"Длина должна быть больше 0 мм");
                    if (billetLength < maxBilletLengthForDoubleBend)
                        return new ValidationResult($"Длина заготовки не должна быть меньше {maxBilletLengthForDoubleBend} мм. Текущая длина: {billetLength:F2} мм.");                    
                }
                else
                { 
                    if (length > firstLengthPropertyVal)
                        return new ValidationResult($"Длина второго конца должна быть меньше или равна длине первого");
                    if (model.HasThread)
                    {
                        //if (length < 400 || length > 6000)
                        //    return new ValidationResult("Укажите длину от 400 до 6000");
                        if (length < 400)
                            return new ValidationResult("Укажите длину от 400");
                    }
                    else
                    {
                        //if (length < bendRadiusMax || length > 6000)
                        //    return new ValidationResult($"Укажите длину от {bendRadiusMax} до 6000");
                        if (length < bendRadiusMax)
                            return new ValidationResult($"Укажите длину от {bendRadiusMax}");
                    }                
                }
            }               
            return ValidationResult.Success;
        }
    }
}
