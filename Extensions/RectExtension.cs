using UnityEngine;

namespace Minerva.Module
{
    public static class RectExtension
    {
        public static float DistanceTo(this Rect rect, Vector2 position)
        {
            var dx = Mathf.Max(rect.min.x - position.x, 0, position.x - rect.max.x);
            var dy = Mathf.Max(rect.min.y - position.y, 0, position.y - rect.max.y);
            return Mathf.Sqrt(dx * dx + dy * dy);
        }

        public static float DistanceTo(this RectInt rect, Vector2 position)
        {
            var dx = Mathf.Max(rect.min.x - position.x, 0, position.x - rect.max.x);
            var dy = Mathf.Max(rect.min.y - position.y, 0, position.y - rect.max.y);
            return Mathf.Sqrt(dx * dx + dy * dy);
        }
    }
}