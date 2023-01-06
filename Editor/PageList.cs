using System;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    public static partial class EditorFieldDrawers
    {
        public class PageList
        {
            public int windowMinWidth;

            public Action OnSortList;
            public Action OnDrawHeader;

            int maxPage;
            int page;
            int linesPerPage;
            SerializedProperty entryList;


            public PageList(SerializedProperty entryList, int linesPerPage = 10)
            {
                this.windowMinWidth = 100;
                this.entryList = entryList;
                this.linesPerPage = linesPerPage;
            }

            public void Draw(string header = "Entries")
            {
                var state = GUI.enabled;
                EditorGUILayout.LabelField($"{header} ({page * linesPerPage}/{entryList.arraySize}): ");
                OnDrawHeader?.Invoke();


                GUILayout.BeginVertical(
                    SetRegionColor(Color.white * (80 / 255f), out var color),
                    GUILayout.MinHeight(windowMinWidth + 60)
                );
                GUI.backgroundColor = color;

                EditorGUI.indentLevel++;
                maxPage = (entryList.arraySize - 1) / linesPerPage;
                for (int i = page * linesPerPage; i < page * linesPerPage + linesPerPage && i < entryList.arraySize; i++)
                {
                    SerializedProperty element = entryList.GetArrayElementAtIndex(i);
                    element.isExpanded = true;
                    EditorGUILayout.PropertyField(element, true);
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
                    entryList.InsertArrayElementAtIndex(entryList.arraySize);
                }
                if (OnSortList != null && GUILayout.Button("Sort")) OnSortList?.Invoke();
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
            }
        }
    }
}
