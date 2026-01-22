using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using UI.AnchorCalculator.ValidationAttributes;

namespace UI.AnchorCalculator.ViewModels
{
    public class AnchorViewModel 
    {
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [CheckLength()] // min 1000 for hydraulic , 400 for rolling and cutting, min bendRadius + 60 for nonThread, BilletLength >= 400 мм for double bend radius anchor   
        [Display(Name = "Длина, мм:")]
        public int Length { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [CheckLength()] // min 1000 for hydraulic , 400 for rolling and cutting, min bendRadius + 60 for nonThread, BilletLength >= 400 мм for double bend radius anchor
        [Display(Name = "Длина вторая, мм:")]
        public int LengthSecond { get; set; }
        [Required(ErrorMessage = "Выберите материал")]
        [Display(Name = "Диаметр, мм:")]
        public string Diameter { get; set; }
        public string? Weight { get; set; }
        public string? Price { get; set; }
        /// <summary>
        /// Get or set anchhor's billet 
        /// </summary>
        [Display(Name = "Длина загиба, мм:")]
        public int BendLength { get; set; }
        [Display(Name = "Радиус загиба, мм:")]
        public int BendRadius { get; set; }
        [CheckThreadLength("HasCuttingThread")]       
        [Display(Name = "Длина резьбы, мм:")]
        public int ThreadLength { get; set; }
        [CheckThreadLength("HasCuttingThread")]
        [Display(Name = "Длина резьбы, мм:")]
        public int ThreadLengthSecond { get; set; }
        [ValidateNever]
        public bool HasThreadSecond { get; set; }
        [ValidateNever]
        public bool HasThread { get; set; }
        [Display(Name = "Диаметр резьбы, мм:")]
        public int ThreadDiameter { get; set; }
        [Display(Name = "Шаг резьбы, мм:")]
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        public string ThreadStep { get; set; }
        public string? Amount { get; set; }
        [Required(ErrorMessage = "Поле обязательно для заполнения")]
        [Range(1, Int32.MaxValue, ErrorMessage = "Кол-во не может быть равно 0")]
        [Display(Name = "Кол-во, шт:")]
        public int Quantity { get; set; }
        public int TypeProfileId { get; set; }
        public DateTime DateCreate { get; set; }
        public string? Material { get; set; }
        public string? SvgElement { get; set; }
        public string? BatchWeight { get; set; }
        public string? BilletLength { get; set; }
        public double BatchPriceMaterial { get; set; }
        public double BatchPriceProductionThread { get; set; }
        public double BatchPriceProductionBend { get; set; }
        public double BatchPriceProductionBandSaw { get; set; }
        public double LengthPathRoller { get; set; }
        public double LengthBeforeEndPathRoller { get; set; }
        public string? Sebes { get; set; }
        public string? BatchSebes { get; set; }
        public string? UserName { get; set; }
        public string? Kind { get; set; }
        public int ProductionId { get; set; }
        [ValidateNever]
        public bool WithoutBindThreadDiamMatetial { get; set; }
        [ValidateNever]
        public bool WithoutBindRadiusBendDiamMatetial { get; set; }
        [ValidateNever]
        public bool HasCuttingThread { get; set; }
        [ValidateNever]
        public bool HasVariableLength { get; set; }
        public double TimeProductionThread { get; set; }
        public double TimeProductionBend { get; set; }
        public double TimeProductionBandSaw { get; set; }
        public double LengthFull { get; set; }
        public string? MaterialName { get; set; }
        [Display(Name = "Материал")]
        public int MaterialId { get; set; }
        public List<Material>? Materials { get; set; }
        public Anchor? Anchor { get; set; }
    }
}
