using System;
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

            Canvas rootCanvas = canvas.rootCanvas; // 获取最顶层 Canvas
            RectTransform canvasRect = rootCanvas.transform as RectTransform;

            if (rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                ForceRebuildLayoutImmediateRecursive(rectTransform);
                // **1️⃣ 先把 rectTransform 的局部坐标转换到 rootCanvas 坐标**
                Vector3 worldPos = rectTransform.position; // 获取 UI 在世界坐标的位置
                Vector3 localPos = canvasRect.InverseTransformPoint(worldPos); // 转换到 rootCanvas 的本地坐标

                // **2️⃣ 计算 UI 的边界（转换到 rootCanvas 坐标）**
                Vector2 size = rectTransform.sizeDelta;
                Vector2 pivotOffset = rectTransform.pivot * size;

                Vector2 uiMin = (Vector2)localPos - pivotOffset;
                Vector2 uiMax = uiMin + size;

                // **3️⃣ 计算屏幕范围（rootCanvas 坐标）**
                float screenMinX = canvasRect.rect.xMin;
                float screenMaxX = canvasRect.rect.xMax;
                float screenMinY = canvasRect.rect.yMin;
                float screenMaxY = canvasRect.rect.yMax;

                // **4️⃣ 计算需要调整的位置**
                Vector2 adjustment = Vector2.zero;
                if (uiMin.x < screenMinX) adjustment.x = screenMinX - uiMin.x; // 左侧超出
                if (uiMax.x > screenMaxX) adjustment.x = screenMaxX - uiMax.x; // 右侧超出
                if (uiMin.y < screenMinY) adjustment.y = screenMinY - uiMin.y; // 底部超出
                if (uiMax.y > screenMaxY) adjustment.y = screenMaxY - uiMax.y; // 顶部超出

                // **5️⃣ 计算新的位置**
                Vector3 newLocalPos = localPos + (Vector3)adjustment;

                // **6️⃣ 把调整后的坐标转换回 rectTransform 的局部坐标**
                rectTransform.position = canvasRect.TransformPoint(newLocalPos);
            }
            else if (rootCanvas.renderMode == RenderMode.ScreenSpaceCamera)
            {
                ForceRebuildLayoutImmediateRecursive(rectTransform);
                var parent = rectTransform.parent;
                var localCamPos = LocalToCameraPosition(parent, rectTransform.localPosition);

                if (!rectTransform.IsUIInScreen(out Vector2 llCamPos, out Vector2 urCamPos))
                {
                    if (llCamPos.x < 0) localCamPos.x += -llCamPos.x;
                    if (urCamPos.x > Screen.width) localCamPos.x += Screen.width - urCamPos.x;
                    if (llCamPos.y < 0) localCamPos.y += -llCamPos.y;
                    if (urCamPos.y > Screen.height) localCamPos.y += Screen.height - urCamPos.y;

                    rectTransform.localPosition = CameraToLocalPosition(parent, localCamPos);
                }
            }
            else
            {
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
        /// Get the min and max respect to camera
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <returns></returns>
        public static (Vector2, Vector2) GetRectRespectToCamera(this RectTransform rectTransform, Camera camera)
        {
            var parent = rectTransform.parent;

            var lowerLeft = (Vector2)rectTransform.localPosition - rectTransform.pivot * rectTransform.sizeDelta;
            var upperRight = lowerLeft + rectTransform.sizeDelta;

            // lower left
            var llCamPos = parent ? LocalToCameraPosition(parent, lowerLeft, camera) : lowerLeft;
            // upper right
            var urCamPos = parent ? LocalToCameraPosition(parent, upperRight, camera) : upperRight;

            return (llCamPos, urCamPos);
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
            var worldPos = Camera.main.ScreenToWorldPoint(camPos);
            var localPos = parent.InverseTransformPoint(worldPos);
            //Debug.Log(worldPos);
            //Debug.Log(localPos);
            return localPos;
        }

        public static Vector3 GetMouseWorldPosition(RectTransform uiElement) => uiElement ? Vector3.zero : GetMouseWorldPosition(uiElement.GetComponentInParent<Canvas>());
        public static Vector3 GetMouseWorldPosition(Canvas canvas)
        {
            Vector2 mouseScreenPosition = Input.mousePosition; // 获取鼠标屏幕坐标
            RectTransform canvasRect = canvas.transform as RectTransform;

            if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            {
                // 🔹 Screen Space - Overlay (直接转换)
                RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, mouseScreenPosition, null, out Vector2 localPoint);
                return canvas.transform.TransformPoint(localPoint);
            }
            else
            {
                // 🔹 Screen Space - Camera 或 World Space (需要相机转换)
                Camera cam = canvas.worldCamera ?? Camera.main; // 获取 Canvas 关联的相机
                if (cam == null)
                {
                    Debug.LogError("Canvas 没有绑定 Camera，并且没有可用的 Camera.main");
                    return Vector3.zero;
                }

                // 将鼠标屏幕坐标转换为世界坐标
                Ray ray = cam.ScreenPointToRay(mouseScreenPosition);
                Plane plane = new Plane(canvas.transform.forward, canvas.transform.position);
                if (plane.Raycast(ray, out float distance))
                {
                    return ray.GetPoint(distance); // 获取射线与 Canvas 平面的交点
                }
            }

            return Vector3.zero; // 如果无法计算，返回 (0,0,0)
        }
    }
}