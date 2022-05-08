using UnityEngine;

namespace Aseprite
{
    public static class ColorExtensions
    {
        public static Color NormalBlend(this Color color, Color newColor) => Color.Lerp(color, newColor, newColor.a);
    }
}