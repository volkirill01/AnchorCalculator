using Core.AnchorCalculator.Entities;
using Core.AnchorCalculator.Entities.Enums;

namespace UI.AnchorCalculator.Utils
{
    static class CalculParams
    {
        public static double GetLengthBillet(Anchor? anchor = null)
        {
            anchor ??= new Anchor();
            double lengthBillet;
            double kFactor = 1 / (Math.Log(1 + (double)anchor.Diameter / anchor.BendRadius)) - anchor.BendRadius / anchor.Diameter; // K-factor
            anchor.LengthPathRoller = Math.PI * anchor.BendRadius * 1 / 2;

            if (anchor.Kind != Kind.Straight)
                anchor.LengthBeforeEndPathRoller = anchor.Length - anchor.BendRadius + anchor.LengthPathRoller;
            else
                anchor.LengthBeforeEndPathRoller = 0;

            if (anchor.Kind == Kind.Bend)
            {
                lengthBillet = anchor.Length - anchor.BendRadius - anchor.Diameter
                    + ((Math.PI * (anchor.BendRadius + kFactor * anchor.Diameter) * 1 / 2)
                    + (anchor.BendLength - (anchor.Diameter + anchor.BendRadius)));
            }
            else if (anchor.Kind == Kind.BendDouble)
            {
                lengthBillet = anchor.Length + anchor.LengthSecond
                    - 2 * (anchor.BendRadius + anchor.Diameter - (Math.PI * (anchor.BendRadius + kFactor * anchor.Diameter) * 1 / 2))
                    + anchor.BendLength - 2 * (anchor.Diameter + anchor.BendRadius);
            }
            else
                lengthBillet = anchor.Length;
            return lengthBillet;
        }
    }
}
