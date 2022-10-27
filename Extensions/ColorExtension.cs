using UnityEngine;
/// <summary>
/// Author : Wendi Cai
/// </summary>
namespace Minerva.Module
{
    public static class ColorExtension
    {
        public static Color RandomTo(this Color color, Color color1)
        {
            return new Color(Random.Range(color.r, color1.r), Random.Range(color.g, color1.g), Random.Range(color.b, color1.b), Random.Range(color.a, color1.a));
        }
    }
}