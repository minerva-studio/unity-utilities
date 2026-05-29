using System;
using UnityEngine;
using UnityEngine.InputSystem;
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
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                llCamPos = Vector2.zero;
                urCamPos = Vector2.zero;
                return false;
            }

            Canvas rootCanvas = canvas.rootCanvas; // Get the uppermost canvas

            switch (rootCanvas.renderMode)
            {
                case RenderMode.ScreenSpaceOverlay:
                    {
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.transform as RectTransform, Vector2.zero, null, out llCamPos);
                        RectTransformUtility.ScreenPointToLocalPointInRectangle(rootCanvas.transform as RectTransform, new Vector2(Screen.width, Screen.height), null, out urCamPos);

                        Vector2 uiMin = rectTransform.anchoredPosition - rectTransform.pivot * rectTransform.sizeDelta;
                        Vector2 uiMax = uiMin + rectTransform.sizeDelta;

                        return uiMin.x >= llCamPos.x && uiMax.x <= urCamPos.x && uiMin.y >= llCamPos.y && uiMax.y <= urCamPos.y;
                    }
                case RenderMode.ScreenSpaceCamera:
                    {
                        (llCamPos, urCamPos) = rectTransform.GetRectRespectToCamera(rootCanvas.worldCamera);
                        Rect rect = Screen.safeArea;
                        return rect.Contains(llCamPos) && rect.Contains(urCamPos);
                    }
                default:
                    {
                        (llCamPos, urCamPos) = rectTransform.GetRectRespectToCamera();
                        Rect rect = Screen.safeArea;
                        return rect.Contains(llCamPos) && rect.Contains(urCamPos);
                    }
            }
        }


        /// <summary>
        /// Move a RectTransform Into Screen view
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns> 
        public static void MoveRectTransformInScreen(this RectTransform rectTransform)
        {
            Canvas canvas = rectTransform.GetComponentInParent<Canvas>();
            if (canvas == null) return;

            Canvas rootCanvas = canvas.rootCanvas; // Get the top-level canvas
            RectTransform canvasRect = rootCanvas.transform as RectTransform;

            if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                ForceRebuildLayoutImmediateRecursive(rectTransform);
                // 1) Convert rectTransform position to rootCanvas local space.
                Vector3 worldPos = rectTransform.position; // UI world position
                Vector3 localPos = canvasRect.InverseTransformPoint(worldPos); // rootCanvas local position

                // 2) Compute UI bounds in rootCanvas local space.
                Vector2 size = rectTransform.sizeDelta;
                Vector2 pivotOffset = rectTransform.pivot * size;

                Vector2 uiMin = (Vector2)localPos - pivotOffset;
                Vector2 uiMax = uiMin + size;

                // 3) Compute visible bounds in rootCanvas local space.
                float screenMinX = canvasRect.rect.xMin;
                float screenMaxX = canvasRect.rect.xMax;
                float screenMinY = canvasRect.rect.yMin;
                float screenMaxY = canvasRect.rect.yMax;

                // 4) Compute required position adjustment.
                Vector2 adjustment = Vector2.zero;
                if (uiMin.x < screenMinX) adjustment.x = screenMinX - uiMin.x; // Exceeded left bound
                if (uiMax.x > screenMaxX) adjustment.x = screenMaxX - uiMax.x; // Exceeded right bound
                if (uiMin.y < screenMinY) adjustment.y = screenMinY - uiMin.y; // Exceeded bottom bound
                if (uiMax.y > screenMaxY) adjustment.y = screenMaxY - uiMax.y; // Exceeded top bound

                // 5) Apply adjustment.
                Vector3 newLocalPos = localPos + (Vector3)adjustment;

                // 6) Convert back to world position and assign.
                rectTransform.position = canvasRect.TransformPoint(newLocalPos);
            }
            else if (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                ForceRebuildLayoutImmediateRecursive(rectTransform);
                var parent = rectTransform.parent;
                var uiCamera = rootCanvas.worldCamera;
                if (uiCamera == null) uiCamera = Camera.main;
                if (uiCamera == null) return;

                var localCamPos = LocalToCameraPosition(parent, rectTransform.localPosition, uiCamera);

                if (!rectTransform.IsUIInScreen(out Vector2 llCamPos, out Vector2 urCamPos))
                {
                    if (llCamPos.x < 0) localCamPos.x += -llCamPos.x;
                    if (urCamPos.x > Screen.width) localCamPos.x += Screen.width - urCamPos.x;
                    if (llCamPos.y < 0) localCamPos.y += -llCamPos.y;
                    if (urCamPos.y > Screen.height) localCamPos.y += Screen.height - urCamPos.y;

                    rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos, uiCamera);
                }
            }
            else
            {
                var parent = rectTransform.parent;
                // Local position in screen space
                var localCamPos = LocalToCameraPosition(parent, rectTransform.localPosition);
                if (!rectTransform.IsUIInScreen(out Vector2 llCamPos, out Vector2 urCamPos))
                {
                    //Debug.LogWarning($"{rectTransform} is out of screen!", rectTransform);
                    if (llCamPos.x < 0) // Move right into screen bounds
                    {
                        localCamPos.x += -llCamPos.x;
                        rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                    }
                    if (urCamPos.x > Screen.width) // Move left into screen bounds
                    {
                        localCamPos.x += Screen.width - urCamPos.x;
                        rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                    }
                    if (llCamPos.y < 0) // Move up into screen bounds
                    {
                        localCamPos.y += -llCamPos.y;
                        rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                    }
                    if (urCamPos.y > Screen.height) // Move down into screen bounds
                    {
                        localCamPos.y += Screen.height - urCamPos.y;
                        rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                    }
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
            return GetRectRespectToCamera(rectTransform, Camera.main);
        }

        /// <summary>
        /// Get the min and max respect to camera
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static (Vector2, Vector2) GetRectRespectToCamera(this RectTransform rectTransform, Camera camera)
        {
            Vector3[] corners = new Vector3[4];
            rectTransform.GetWorldCorners(corners);

            Vector2 min = Vector2.positiveInfinity;
            Vector2 max = Vector2.negativeInfinity;
            for (int i = 0; i < corners.Length; i++)
            {
                Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(camera, corners[i]);
                min = Vector2.Min(min, screenPoint);
                max = Vector2.Max(max, screenPoint);
            }

            return (min, max);
        }

        /// <summary>
        /// Convert a local position to camera's position
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="localPos">position in local position respect to parent</param>
        /// <returns></returns>
        public static Vector2 LocalToCameraPosition(Transform parent, Vector2 localPos) => Camera.main.WorldToScreenPoint(parent.TransformPoint(localPos));

        /// <summary>
        /// Convert a local position to camera's position
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="localPos">position in local position respect to parent</param>
        /// <returns></returns>
        public static Vector2 LocalToCameraPosition(Transform parent, Vector2 localPos, Camera camera) => camera.WorldToScreenPoint(parent.TransformPoint(localPos));

        /// <summary>
        /// Convert a camera's position to a local position 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="camPos">position in camera</param>
        /// <returns></returns>
        public static Vector2 CameraToLocalPosition(Transform parent, Vector2 camPos)
        {
            var mainCamera = Camera.main;
            if (mainCamera == null) return Vector2.zero;

            return CameraToLocalPosition(parent, camPos, mainCamera);
        }

        /// <summary>
        /// Convert a camera's position to a local position.
        /// </summary>
        /// <param name="parent">The transform that local position is relative to.</param>
        /// <param name="camPos">Position in camera (screen) space.</param>
        /// <param name="camera">Camera used for the conversion.</param>
        /// <returns>Position in local space.</returns>
        public static Vector2 CameraToLocalPosition(Transform parent, Vector2 camPos, Camera camera)
        {
            if (!parent || !camera) return Vector2.zero;

            if (parent is RectTransform parentRect &&
                RectTransformUtility.ScreenPointToWorldPointInRectangle(parentRect, camPos, camera, out Vector3 uiWorldPos))
            {
                return parent.InverseTransformPoint(uiWorldPos);
            }

            Ray ray = camera.ScreenPointToRay(camPos);
            Plane plane = new Plane(parent.forward, parent.position);
            if (!plane.Raycast(ray, out float distance)) return Vector2.zero;

            Vector3 worldPos = ray.GetPoint(distance);
            return parent.InverseTransformPoint(worldPos);
        }

        public static Vector3 GetMouseWorldPosition(RectTransform uiElement) => uiElement ? GetMouseWorldPosition(uiElement.GetComponentInParent<Canvas>()) : Vector3.zero;
        public static Vector3 GetMouseWorldPosition(Canvas canvas)
        {
#if ENABLE_INPUT_SYSTEM
            Vector2 mouseScreenPosition = Mouse.current?.position.ReadValue() ?? Vector2.zero;
#else
            Vector2 mouseScreenPosition = Input.mousePosition;
#endif
            RectTransform canvasRect = canvas.transform as RectTransform;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // Screen Space - Overlay (direct conversion)
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mouseScreenPosition, null, out Vector2 localPoint);
                return canvas.transform.TransformPoint(localPoint);
            }
            else
            {
                // Screen Space - Camera / World Space (camera conversion required)
                Camera cam = canvas.worldCamera;    // Canvas camera 
                if (cam == null) cam = Camera.main;
                if (cam == null)
                {
                    Debug.LogError("Canvas has no assigned camera and no available Camera.main.");
                    return Vector3.zero;
                }

                // Convert mouse screen position to world position
                Ray ray = cam.ScreenPointToRay(mouseScreenPosition);
                Plane plane = new Plane(canvas.transform.forward, canvas.transform.position);
                if (plane.Raycast(ray, out float distance))
                {
                    return ray.GetPoint(distance); // Ray-plane intersection point
                }
            }

            return Vector3.zero; // Return (0,0,0) if conversion fails
        }
    }
}
