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

            int maxPage;
            int page;


            public int LinesPerPage { get; set; }

            public abstract int Size { get; }

            protected abstract void DrawElement(int i);

            public abstract void AddElement();


            public void Draw(string header = "Entries")
            {
                var state = GUI.enabled;
                EditorGUILayout.LabelField($"{header} ({page * LinesPerPage}/{Size}): ");
                OnDrawHeader?.Invoke();


                GUILayout.BeginVertical(
                    SetRegionColor(Color.white * (80 / 255f), out var color),
                    GUILayout.MinHeight(windowMinWidth + 60)
                );
                GUI.backgroundColor = color;

                EditorGUI.indentLevel++;
                maxPage = (Size - 1) / LinesPerPage;
                for (int i = page * LinesPerPage; i < page * LinesPerPage + LinesPerPage && i < Size; i++)
                {
                    DrawElement(i);
                }
                EditorGUI.indentLevel--;

                GUILayout.FlexibleSpace();


                GUILayout.BeginHorizontal();
                GUI.enabled = page > 0;
                if (GUILayout.Button("Last", GUILayout.MaxWidth(80))) page--;
                GUI.enabled = state;
                EditorGUILayout.LabelField("Page", GUILayout.MaxWidth(30));
                if (maxPage != 0) page = EditorGUILayout.IntSlider(page, 0, maxPage);
                else EditorGUILayout.LabelField("-");
                GUILayout.Label($"of {maxPage}", GUILayout.MaxWidth(40));
                page = Mathf.Max(Mathf.Min(maxPage, page), 0);
                GUI.enabled = page < maxPage;
                if (GUILayout.Button("Next", GUILayout.MaxWidth(80))) page++;
                GUI.enabled = state;
                GUILayout.EndHorizontal();

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Add"))
                {
                    AddElement();
                }
                if (OnSortList != null && GUILayout.Button("Sort")) OnSortList?.Invoke();
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
            }
        }
    }
}