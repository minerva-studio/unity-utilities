using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Minerva.Module.Editor
{
    public static partial class EditorFieldDrawers
    {
        public class SerializedPropertyPageList : PageList
        {
            SerializedProperty entryList;

            public override int Size => entryList.arraySize;

            public SerializedPropertyPageList(SerializedProperty entryList, int linesPerPage = 10)
            {
                this.windowMinWidth = 100;
                this.entryList = entryList;
                this.LinesPerPage = linesPerPage;
            }

            protected override void DrawElement(int i)
            {
                SerializedProperty element = entryList.GetArrayElementAtIndex(i);
                element.isExpanded = true;
                EditorGUILayout.PropertyField(element, true);
            }

            public override void AddElement()
            {
                entryList.InsertArrayElementAtIndex(Size);
            }
        }

        public class GenericListPageList<T> : PageList
        {
            List<T> entryList;
            Action<T> drawer;

            public override int Size => entryList.Count;

            public GenericListPageList(List<T> entryList, Action<T> drawer, int linesPerPage = 10)
            {
                this.windowMinWidth = 100;
                this.entryList = entryList;
                this.drawer = drawer;
                this.LinesPerPage = linesPerPage;
            }

            protected override void DrawElement(int i)
            {
                T element = entryList[i];
                drawer.Invoke(element);
            }

            public override void AddElement()
            {
                entryList.Add(entryList[Size - 1]);
            }
        }
    }
}
