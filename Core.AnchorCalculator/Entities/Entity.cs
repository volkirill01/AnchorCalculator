namespace Core.AnchorCalculator.Entities;

public abstract class Entity : IEquatable<Entity>
{
	public int Id { get; set; }

	public bool Equals(Entity? other)
	{
		if (other == null) return false;
		if (ReferenceEquals(this, other)) return true;

		return Id == other.Id;
	}
	public override bool Equals(object? obj)
	{
		if (obj.GetType() != GetType()) return false;

		return Equals((Entity)obj);
	}

	public override int GetHashCode() => Id;
}
