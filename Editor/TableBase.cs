using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    public static partial class EditorFieldDrawers
    {
        public abstract class TableBase
        {
            private int rowBaseIndex;
            private Vector2 tableScrollView;

            public int EntryCount { get; set; }
            public float TableEntryWidth { get; set; } = 200;
            public float TableEntryHeight { get; set; } = 20;
            public abstract int Size { get; }
            public abstract int ColumnCount { get; }


            protected abstract void DrawElement(int row, int col);
            protected abstract void DrawColumnLabel(int col);
            protected abstract void DrawRowLabel(int row);
            protected abstract bool TryRemoveRow(int row);


            public void Draw()
            {
                if (Event.current.type == EventType.ScrollWheel)
                {
                    rowBaseIndex += Event.current.delta.y > 0 ? 1 : -1;
                    rowBaseIndex = Mathf.Min(Size, Mathf.Max(0, rowBaseIndex));
                    EditorWindow.focusedWindow.Repaint();
                }

                GUILayout.BeginVertical();
                // shoud display at least 1 element
                EntryCount = Mathf.Max(1, EntryCount);
                int upperTableIndex = Mathf.Min(Size, rowBaseIndex + EntryCount);
                EditorGUILayout.LabelField($"{rowBaseIndex + 1}~{upperTableIndex} of {Size}");
                var keyEntrykeyWidth = GUILayout.Width(300);
                var keyEntryWidth = GUILayout.Width(TableEntryWidth);
                var keyEntryHeight = GUILayout.Height(TableEntryHeight);
                GUILayout.BeginHorizontal();
                tableScrollView = GUILayout.BeginScrollView(tableScrollView, GUI.skin.horizontalScrollbar, GUIStyle.none);
                {
                    GUILayout.BeginHorizontal();
                    GUILayout.Button("...", GUILayout.Width(EditorGUIUtility.singleLineHeight));
                    GUILayout.Label("Entries", keyEntrykeyWidth);
                    for (int i = 0; i < ColumnCount; i++)
                    {
                        DrawColumnLabel(i);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.BeginVertical();
                for (int row = rowBaseIndex; row < upperTableIndex; row++)
                {
                    GUILayout.BeginHorizontal(keyEntryHeight);
                    // try remove
                    if (GUILayout.Button("x", GUILayout.Width(EditorGUIUtility.singleLineHeight)))
                    {
                        if (TryRemoveRow(row)) break;
                    }

                    // draw all entries
                    DrawRowLabel(row);
                    for (int j = 0; j < ColumnCount; j++)
                    {
                        DrawElement(row, j);
                    }
                    GUILayout.EndHorizontal();
                }
                GUILayout.EndVertical();
                // try remove key

                GUILayout.EndScrollView();
                rowBaseIndex = (int)GUILayout.VerticalScrollbar(rowBaseIndex, EntryCount, 0, Size, GUILayout.ExpandHeight(true));
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
        }
    }
}