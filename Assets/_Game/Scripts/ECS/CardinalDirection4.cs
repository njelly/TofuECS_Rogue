using System.Numerics;

namespace Tofunaut.TofuECS_Rogue.ECS
{
    public enum CardinalDirection4
    {
        None = 0,
        North = 1,
        East = 2,
        South = 3,
        West = 4,
    }

    public static class CardinalDirection4Utils
    {
        public static Vector2 ToVector2(this CardinalDirection4 dir)
        {
            return dir switch
            {
                CardinalDirection4.North => Vector2.UnitY,
                CardinalDirection4.East => Vector2.UnitX,
                CardinalDirection4.South => -Vector2.UnitY,
                CardinalDirection4.West => -Vector2.UnitX,
                _ => Vector2.Zero
            };
        }

        public static CardinalDirection4 ToCardinalDirection4(this Vector2 v)
        {
            var vectors = new[]
            {
                CardinalDirection4.None.ToVector2(),
                CardinalDirection4.North.ToVector2(),
                CardinalDirection4.East.ToVector2(),
                CardinalDirection4.South.ToVector2(),
                CardinalDirection4.West.ToVector2()
            };

            var highestDot = 0f;
            var index = 0;
            for (var i = 0; i < vectors.Length; i++)
            {
                var dot = Vector2.Dot(v, vectors[i]);
                if (dot < highestDot)
                    continue;

                highestDot = dot;
                index = i;
            }

            return (CardinalDirection4) index;
        }
    }
}