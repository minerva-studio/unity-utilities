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
    }
}