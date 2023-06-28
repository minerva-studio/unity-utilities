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
                page = Mathf.Max(Mathf.Min(MaxPage, page), 1);

                var state = GUI.enabled;
                EditorGUILayout.LabelField($"{header} ({FirstIndex + 1}/{Size}): ");
                OnDrawHeader?.Invoke();


                GUILayout.BeginVertical(
                    SetRegionColor(Color.white * (80 / 255f), out var color),
                    GUILayout.MinHeight(windowMinWidth + 60)
                );
                GUI.backgroundColor = color;

                EditorGUI.indentLevel++;
                for (int i = FirstIndex; i < FirstIndex + LinesPerPage && i < Size; i++)
                {
                    DrawElement(i);
                }
                EditorGUI.indentLevel--;

                GUILayout.FlexibleSpace();


                GUILayout.BeginHorizontal();
                GUI.enabled = page > 1;
                if (GUILayout.Button("Last", GUILayout.MaxWidth(80))) page--;
                GUI.enabled = state;
                EditorGUILayout.LabelField("Page", GUILayout.MaxWidth(30));
                if (MaxPage != 0) page = EditorGUILayout.IntSlider(page, 1, MaxPage);
                else EditorGUILayout.LabelField("-");
                GUILayout.Label($"of {MaxPage}", GUILayout.MaxWidth(40));
                page = Mathf.Max(Mathf.Min(MaxPage, page), 1);
                GUI.enabled = page <= MaxPage;
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