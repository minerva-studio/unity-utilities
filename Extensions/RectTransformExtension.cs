using UnityEngine;
using UnityEngine.UI;

namespace Minerva.Module
{

    /// <summary>
    /// UI Helper class
    /// </summary>
    public static class RectTransformExtension
    {
        /// <summary>
        /// Determine whether ui component is in screen
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static bool IsUIInScreen(this RectTransform rectTransform)
        {
            (Vector2 llCamPos, Vector2 urCamPos) = GetRectRespectToCamera(rectTransform);
            Rect rect = Screen.safeArea;
            return rect.Contains(llCamPos) && rect.Contains(urCamPos);
        }

        /// <summary>
        /// Determine whether ui component is in screen
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="llCamPos">lower left camera position of the ui</param>
        /// <param name="urCamPos">upper right camera position of the uiparam>
        /// <returns></returns>
        public static bool IsUIInScreen(this RectTransform rectTransform, out Vector2 llCamPos, out Vector2 urCamPos)
        {
            (llCamPos, urCamPos) = GetRectRespectToCamera(rectTransform);
            Rect rect = Screen.safeArea;
            return rect.Contains(llCamPos) && rect.Contains(urCamPos);
        }

        /// <summary>
        /// Move a RectTransform Into Screen view
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static void MoveRectTransformInScreen(this RectTransform rectTransform)
        {
            ForceRebuildLayoutImmediateRecursive(rectTransform);
            var parent = rectTransform.parent;
            // local position in camera
            var localCamPos = LocalToCameraPosition(parent, rectTransform.localPosition);
            if (!rectTransform.IsUIInScreen(out Vector2 llCamPos, out Vector2 urCamPos))
            {
                //Debug.LogWarning($"{rectTransform} is out of screen!", rectTransform);
                if (llCamPos.x < 0)//需要右移进入屏幕范围
                {
                    localCamPos.x += -llCamPos.x;
                    rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                }
                if (urCamPos.x > Screen.width)//需要左移进入屏幕范围
                {
                    localCamPos.x += Screen.width - urCamPos.x;
                    rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                }
                if (llCamPos.y < 0)//需要上移进入屏幕范围
                {
                    localCamPos.y += -llCamPos.y;
                    rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                }
                if (urCamPos.y > Screen.height)//需要下移进入屏幕范围
                {
                    localCamPos.y += Screen.height - urCamPos.y;
                    rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                }
            }
        }

        public static void ForceRebuildLayoutImmediateRecursive(this RectTransform rectTransform)
        {
            foreach (var item in rectTransform.GetComponentsInChildren<RectTransform>())
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(item);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        /// <summary>
        /// Get the min and max respect to camera
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static (Vector2, Vector2) GetRectRespectToCamera(this RectTransform rectTransform)
        {
            var parent = rectTransform.parent;

            var lowerLeft = (Vector2)rectTransform.localPosition - rectTransform.pivot * rectTransform.sizeDelta;
            var upperRight = lowerLeft + rectTransform.sizeDelta;

            // lower left
            var llCamPos = parent ? LocalToCameraPosition(parent, lowerLeft) : lowerLeft;
            // upper right
            var urCamPos = parent ? LocalToCameraPosition(parent, upperRight) : upperRight;

            return (llCamPos, urCamPos);
        }

        /// <summary>
        /// Convert a local position to camera's position
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="localPos">position in local position respect to parent</param>
        /// <returns></returns>
        public static Vector2 LocalToCameraPosition(Transform parent, Vector2 localPos)
        {
            var worldPos = parent.TransformPoint(localPos);
            var camPos = Camera.main.WorldToScreenPoint(worldPos);
            //Debug.Log(worldPos);
            //Debug.Log(camPos);
            return camPos;
        }

        /// <summary>
        /// Convert a camera's position to a local position 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="camPos">position in camera</param>
        /// <returns></returns>
        public static Vector2 CameraToLocalPosition(Transform parent, Vector2 camPos)
        {
            var worldPos = Camera.main.ScreenToWorldPoint(camPos);
            var localPos = parent.InverseTransformPoint(worldPos);
            //Debug.Log(worldPos);
            //Debug.Log(localPos);
            return localPos;
        }
    }
}