using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;

namespace UI.AnchorCalculator.Utils;

static class CalculParams
{
	public static double GetLengthBillet(Anchor? anchor = null)
	{
		anchor ??= new Anchor();

		double lengthBillet;
		double kFactor = 1 / (Math.Log(1 + (double)anchor.DiameterMillimeters / anchor.BendRadiusMillimeters)) - anchor.BendRadiusMillimeters / anchor.DiameterMillimeters;
		anchor.RollerPathLengthMillimeters = Math.PI * anchor.BendRadiusMillimeters * 1 / 2;

		if (anchor.Kind == AnchorKind.Straight)
			anchor.RollerPathLengthMillimetersBeforeEnd = 0;
		else
			anchor.RollerPathLengthMillimetersBeforeEnd = anchor.LengthMillimeters - anchor.BendRadiusMillimeters + anchor.RollerPathLengthMillimeters;

		switch (anchor.Kind)
		{
			case AnchorKind.SingleBend:
			{
				lengthBillet = anchor.LengthMillimeters - anchor.BendRadiusMillimeters - anchor.DiameterMillimeters
					+ ((Math.PI * (anchor.BendRadiusMillimeters + kFactor * anchor.DiameterMillimeters) * 1 / 2)
					+ (anchor.BendLengthMillimeters - (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters)));
				break;
			}
			case AnchorKind.DoubleBend:
			{
				lengthBillet = anchor.LengthMillimeters + anchor.SecondLengthMillimeters
					- 2 * (anchor.BendRadiusMillimeters + anchor.DiameterMillimeters - (Math.PI * (anchor.BendRadiusMillimeters + kFactor * anchor.DiameterMillimeters) * 1 / 2))
					+ anchor.BendLengthMillimeters - 2 * (anchor.DiameterMillimeters + anchor.BendRadiusMillimeters);
				break;
			}
			default:
			{
				lengthBillet = anchor.LengthMillimeters;
				break;
			}
		}
		return lengthBillet;
	}
}
