using Core.AnchorCalculator.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.AnchorCalculator.Entities;

public class Anchor : Entity
{
	public int LengthMillimeters { get; set; }
	public int SecondLengthMillimeters { get; set; }
	public float DiameterMillimeters { get; set; }
	public double WeightKg { get; set; }
	public double Sebes { get; set; }
	public double BatchSebes { get; set; }
	public double Price { get; set; }

	public int BendLengthMillimeters { get; set; }
	public int BendRadiusMillimeters { get; set; }
	public int ThreadLengthMillimeters { get; set; }
	public int ThreadSecondLengthMillimeters { get; set; }
	public int ThreadDiameterMillimeters { get; set; }
	public float ThreadStepMillimeters { get; set; }
	public double TotalCost { get; set; }
	public int Quantity { get; set; }
	public DateTime DateCreate { get; set; }
	public string? SvgElement { get; set; }
	public double BatchWeightKg { get; set; }
	public double BilletLengthMillimeters { get; set; }
	public string MaterialJson { get; set; }
	public string UserJson { get; set; }
	public virtual int KindId
	{
		get => (int)Kind;
		set => Kind = (AnchorKind)value;
	}

	[EnumDataType(typeof(AnchorKind))]
	public AnchorKind Kind { get; set; }
	public virtual int ProductionId
	{
		get => (int)Production;
		set => Production = (Production)value;
	}
	public Production Production { get; set; }
	public double PricePerMeter { get; set; }
	[NotMapped]
	public double PriceMaterial => BatchPriceMaterial / Quantity;
	public double BatchPriceMaterial { get; set; }
	public double PriceProductionThread => BatchPriceProductionThread / Quantity;
	public double BatchPriceProductionThread { get; set; }
	public double PriceProductionBend => BatchPriceProductionBend / Quantity;
	public double BatchPriceProductionBend { get; set; }
	public double PriceProductionBandSaw => BatchPriceProductionBandSaw / Quantity;
	public double BatchPriceProductionBandSaw { get; set; }
	public double RollerPathLengthMillimeters { get; set; }
	public double RollerPathLengthMillimetersBeforeEnd { get; set; }
	public double ProductionThreadHours { get; set; }
	public double ProductionBendHours { get; set; }
	public double ProductionBandSawHours { get; set; }
	public double FullLengthMeters { get; set; } // TODO: Maybe this is millimeters
	[NotMapped]
	public bool HasCuttingThread { get; set; }
	public bool WithoutBindThreadDiamMaterial { get; set; }

	public int? MaterialId { get; set; }
	public Material? Material { get; set; }
	public string? UserId { get; set; }
	public User? User { get; set; }
}
