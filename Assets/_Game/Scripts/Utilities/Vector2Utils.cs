namespace Tofunaut.TofuECS_Rogue
{
    public class Vector2Utils
    {
        public static System.Numerics.Vector2 ToSystemVector2(UnityEngine.Vector2 v) =>
            new (v.x, v.y);

        public static UnityEngine.Vector2 ToUnityVector2(System.Numerics.Vector2 v) =>
            new(v.X, v.Y);
    }
}