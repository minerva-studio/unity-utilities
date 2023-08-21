using UnityEditor;

namespace Minerva.Module.Editor
{
    public static partial class EditorFieldDrawers
    {
        public class SerializedPropertyPageList : PageList
        {
            public SerializedProperty entryList;

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
    }
}
