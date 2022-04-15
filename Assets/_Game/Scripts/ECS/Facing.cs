using System;
using System.Numerics;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public enum Facing
    {
        None,
        North,
        South,
        East,
        West,
    }

    public static class FacingExtensions
    {
        public static (int x, int y) ToOffset(this Facing facing) => facing switch
        {
            Facing.None => (0, 0),
            Facing.North => (0, 1),
            Facing.South => (0, -1),
            Facing.East => (1, 0),
            Facing.West => (-1, 0),
            _ => throw new ArgumentOutOfRangeException(nameof(facing), facing, null)
        };

        public static Facing FromOffset(int x, int y)
        {
            var allValues = (Facing[]) Enum.GetValues(typeof(Facing));
            var toReturn = Facing.None;
            var highestDot = 0;
            foreach (var f in allValues)
            {
                var (ox, oy) = ToOffset(f);
                var dot = ox * x + oy * y;
                if (dot <= highestDot) 
                    continue;
                
                toReturn = f;
                highestDot = dot;
            }

            return toReturn;
        }
    }
}