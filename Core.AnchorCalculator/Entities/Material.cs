using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.AnchorCalculator.Entities;

public class Material : Entity
{
	public string? Name { get; set; }

	[NotMapped]
	public string? FullName => $"{Name} ⌀{Size} {Type}";

	public double Size { get; set; }
	public double ThreadRollingHours { get; set; }
	public double ThreadCuttingHours { get; set; }
	public double PlashkaCount { get; set; }
	public double CutterCount { get; set; }
	public double BandSawHours { get; set; }
	public double BandSawBladeCount { get; set; }

	public virtual int TypeId
	{
		get => (int)Type;
		set => Type = (Enums.Type)value;
	}

	[EnumDataType(typeof(Enums.Type))]
	public Enums.Type Type { get; set; }

	public double PricePerMeter { get; set; }

	public DateTime DateUpdate { get; set; }

	public HashSet<Anchor> Anchors { get; private set; } = new();
}
