using Core.AnchorCalculator.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.AnchorCalculator.Entities;

public class Anchor : Entity
{
	public int LengthMillimeters { get; set; }
	public int SecondLengthMillimeters { get; set; }
	public float DiameterMillimeters { get; set; }
	public double WeightKg_SingleAnchor { get; set; }
	public double SebesSom_SingleAnchor { get; set; }
	public double SebesSom_Total { get; set; }
	public double PriceSom_SingleAnchor { get; set; }

	public int BendLengthMillimeters { get; set; }
	public int BendRadiusMillimeters { get; set; }
	public int ThreadLengthMillimeters { get; set; }
	public int ThreadSecondLengthMillimeters { get; set; }
	public int ThreadDiameterMillimeters { get; set; }
	public float ThreadStepMillimeters { get; set; }
	public double PriceSom_Total { get; set; }
	public int Quantity { get; set; }
	public DateTime DateCreate { get; set; }
	public string? SvgElement { get; set; }
	public double WeightKg_Total { get; set; }
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
	public virtual int ThreadProductionTypeId
	{
		get => (int)ThreadProductionType;
		set => ThreadProductionType = (ThreadProductionType)value;
	}
	public ThreadProductionType ThreadProductionType { get; set; }
	public double PriceSom_PerMeter { get; set; }

	public double PriceSom_Material_Total { get; set; }
	[NotMapped]
	public double PriceSom_Material_SingleAnchor => PriceSom_Material_Total / Quantity;

	public double PriceSom_ProductionThread_Total { get; set; }
	public double PriceSom_ProductionThread_SingleAnchor => PriceSom_ProductionThread_Total / Quantity;

	public double PriceSom_ProductionBend_Total { get; set; }
	public double PriceSom_ProductionBend_SingleAnchor => PriceSom_ProductionBend_Total / Quantity;

	public double PriceSom_ProductionBandSaw_Total { get; set; }
	public double PriceSom_ProductionBandSaw_SingleAnchor => PriceSom_ProductionBandSaw_Total / Quantity;

	public double RollerPathLengthMillimeters { get; set; }
	public double RollerPathLengthMillimetersBeforeEnd { get; set; }

	public double ProductionHours_Thread { get; set; }
	public double ProductionHours_Bend { get; set; }
	public double ProductionHours_BandSaw { get; set; }

	public double FullLengthMeters { get; set; } // TODO: Maybe this is millimeters
	[NotMapped]
	public bool HasCuttingThread { get; set; }
	public bool WithoutBindThreadDiamMaterial { get; set; }

	public int? MaterialId { get; set; }
	public Material? Material { get; set; }
	public string? UserId { get; set; }
	public User? User { get; set; }
}
