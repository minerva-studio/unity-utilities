using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Author : Wendi Cai
    /// </summary>
    public static class ColorExtension
    {
        /// <summary>
        /// get random color between two values (linearly)
        /// </summary>
        /// <param name="color"></param>
        /// <param name="color1"></param>
        /// <returns></returns>
        public static Color RandomTo(this Color color, Color color1)
        {
            return new Color(Random.Range(color.r, color1.r), Random.Range(color.g, color1.g), Random.Range(color.b, color1.b), Random.Range(color.a, color1.a));
        }

        /// <summary>
        /// Get color hex code
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static string ToHexString(this Color color)
        {
            return $"#{(int)(color.r * 255f):X2}{(int)(color.g * 255f):X2}{(int)(color.b * 255f):X2}{(int)(color.a * 255f):X2}";
        }

        /// <summary>
        /// Get color hex code
        /// </summary>
        /// <param name="color"></param>
        /// <returns></returns>
        public static int ToHex(this Color color)
        {
            return (int)(color.r * 255f) << 16 + (int)(color.g * 255f) << 8 + (int)(color.b * 255f);
        }

    }
}