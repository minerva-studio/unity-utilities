using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    public static partial class EditorFieldDrawers
    {
        public abstract class PageList
        {
            public Action OnDrawHeader;
            public Action OnSortList;

            public int windowMinWidth;
            int page;


            private int FirstIndex => (page - 1) * LinesPerPage;
            private int MaxPage => (Size - 1) / LinesPerPage + 1;
            public int LinesPerPage { get; set; }
            public abstract int Size { get; }


            protected abstract void DrawElement(int i);

            public abstract void AddElement();


            public void Draw(string header = "Entries")
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    page += Event.current.delta.y > 0 ? 1 : -1;
                    page = Mathf.Min(MaxPage, Mathf.Max(0, page));
                    if (EditorWindow.focusedWindow) EditorWindow.focusedWindow.Repaint();
                    return;
                }

                page = Mathf.Max(Mathf.Min(MaxPage, page), 1);
                DrawHeader(header);

                GUILayout.BeginVertical(SetRegionColor(Color.white * (80 / 255f), out var color), GUILayout.MinHeight(windowMinWidth + 60));
                GUI.backgroundColor = color;
                EditorGUI.indentLevel++;
                for (int i = FirstIndex; i < FirstIndex + LinesPerPage && i < Size; i++)
                {
                    DrawElement(i);
                }
                EditorGUI.indentLevel--;
                GUILayout.FlexibleSpace();

                DrawPageScroll();
                DrawButtom();
                GUILayout.EndVertical();
            }

            private void DrawPageScroll()
            {
                GUILayout.BeginHorizontal();
                using (new GUIEnable(page > 1))
                    if (GUILayout.Button("Last", GUILayout.MaxWidth(80))) page--;

                using (new GUIEnable())
                {
                    EditorGUILayout.LabelField("Page", GUILayout.MaxWidth(30));
                    if (MaxPage != 0) page = EditorGUILayout.IntSlider(page, 1, MaxPage);
                    else EditorGUILayout.LabelField("-");
                    GUILayout.Label($"of {MaxPage}", GUILayout.MaxWidth(40));
                    page = Mathf.Max(Mathf.Min(MaxPage, page), 1);
                }

                using (new GUIEnable(page <= MaxPage))
                    if (GUILayout.Button("Next", GUILayout.MaxWidth(80))) page++;

                GUILayout.EndHorizontal();
            }

            private void DrawHeader(string header)
            {
                // empty
                if (Size == 0)
                {
                    EditorGUILayout.LabelField($"{header} (-/0): ");
                }
                else
                {
                    int maxIndex = Mathf.Min(Size, FirstIndex + LinesPerPage);
                    EditorGUILayout.LabelField($"{header} ({FirstIndex + 1}~{maxIndex}/{Size}): ");
                }

                OnDrawHeader?.Invoke();
            }

            private void DrawButtom()
            {
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add"))
                {
                    AddElement();
                }
                if (OnSortList != null && GUILayout.Button("Sort")) OnSortList?.Invoke();
                GUILayout.EndHorizontal();
            }
        }
    }
}