using System;
using System.Collections;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;
using static UnityEditorInternal.ReorderableList;

namespace Minerva.Module.Editor
{
    /// <summary>
    /// Helper class for drawing any type of field in editor
    /// </summary>
    public static partial class EditorFieldDrawers
    {
        /// <summary>
        /// Draw field by <paramref name="field"/>, with label <paramref name="labelName"/>
        /// </summary>
        /// <param name="labelName"> label fo the field </param>
        /// <param name="field"> the field info </param>
        /// <param name="target"> the instance </param>
        public static void DrawField(string labelName, FieldInfo field, object target) => DrawField(new GUIContent(labelName), field, target);

        /// <summary>
        /// Draw field by <paramref name="field"/>, with label <paramref name="label"/>
        /// </summary>
        /// <param name="label"> label fo the field </param>
        /// <param name="field"> the field info </param>
        /// <param name="target"> the instance </param>
        public static void DrawField(GUIContent label, FieldInfo field, object target)
        {
            if (field is null)
            {
                EditorGUILayout.LabelField(label, "Field not instantiated");
                return;
            }
            label = new(label);
            if (Attribute.IsDefined(field, typeof(TooltipAttribute)))
            {
                label.tooltip = ((TooltipAttribute)Attribute.GetCustomAttribute(field, typeof(TooltipAttribute))).tooltip;
            }

            object oldValue = field.GetValue(target);
            if (field.FieldType == typeof(string) && oldValue == null) oldValue = string.Empty;
            object value = DrawField(label, oldValue);
            if (value is not null) field.SetValue(target, value);
        }


        /// <summary>
        /// Draw given value
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="value"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="displayUnsupportInfo"> if the field type is not supported, draw not supported message in editor</param>
        /// <returns> new value if changed, old if no change </returns>
        public static object DrawField(string labelName, object value, bool isReadOnly, bool displayUnsupportInfo = true) => DrawField(new GUIContent(labelName), value, isReadOnly, displayUnsupportInfo);
        /// <summary>
        /// Draw given value
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="value"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="displayUnsupportInfo"> if the field type is not supported, draw not supported message in editor</param>
        /// <returns> new value if changed, old if no change </returns>
        public static object DrawField(GUIContent label, object value, bool isReadOnly, bool displayUnsupportInfo = true)
        {
            var GUIState = GUI.enabled;
            if (isReadOnly)
            {
                GUI.enabled = false;
            }
            var ret = DrawField(label, value, displayUnsupportInfo);
            if (isReadOnly)
            {
                GUI.enabled = GUIState;
            }
            return ret;
        }



        /// <summary>
        /// Draw given value
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="value"></param>
        /// <returns> new value if changed, old if no change </returns>
        public static object DrawField(string labelName, object value, bool displayUnsupportInfo = true) => DrawField(new GUIContent(labelName), value, displayUnsupportInfo);
        /// <summary>
        /// Draw given value
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="value"></param>
        /// <returns> new value if changed, old if no change </returns>
        public static object DrawField(GUIContent label, object value, bool displayUnsupportInfo = true)
        {
            if (value is string s)
            {
                return EditorGUILayout.TextField(label, s);
            }
            else if (value is int i)
            {
                return EditorGUILayout.IntField(label, i);
            }
            else if (value is float f)
            {
                return EditorGUILayout.FloatField(label, f);
            }
            else if (value is double d)
            {
                return EditorGUILayout.DoubleField(label, d);
            }
            else if (value is bool b)
            {
                return EditorGUILayout.Toggle(label, b);
            }
            else if (value is Vector2 v2)
            {
                return EditorGUILayout.Vector2Field(label, v2);
            }
            else if (value is Vector2Int v2i)
            {
                return EditorGUILayout.Vector2IntField(label, v2i);
            }
            else if (value is Vector3 v3)
            {
                return EditorGUILayout.Vector3Field(label, v3);
            }
            else if (value is Vector3Int v3i)
            {
                return EditorGUILayout.Vector3IntField(label, v3i);
            }
            else if (value is UUID uUID)
            {
                EditorGUILayout.LabelField(label, uUID.Value);
                return uUID;
            }
            else if (value is Color color)
            {
                return EditorGUILayout.ColorField(label, color);
            }
            else if (value is Gradient gradient)
            {
                return EditorGUILayout.GradientField(label, gradient);
            }
            else if (value is Enum e)
            {
                if (Attribute.GetCustomAttribute(value.GetType(), typeof(FlagsAttribute)) != null)
                {
                    return EditorGUILayout.EnumFlagsField(label, e);
                }
                else return EditorGUILayout.EnumPopup(label, e);
            }
            else if (value is UnityEngine.Object uo)
            {
                return EditorGUILayout.ObjectField(label, uo, value.GetType(), false);
            }
            else if (value is RangeInt r)
            {
                return DrawRangeField(label, r);
            }
            else if (value is IList list && value.GetType().IsGenericType)
            {
                var itemType = value.GetType().GenericTypeArguments[0];
                DrawList(label, list, itemType);
            }
            else if (value is null)
            {
                EditorGUILayout.LabelField(label.text, "null");
            }
            else if (displayUnsupportInfo) EditorGUILayout.LabelField(label.text, $"({value.GetType().Name})");
            return value;
        }



        /// <summary>
        /// Check a value is supported
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool IsSupported(object value)
        {
            if (value == null) return false;
            if (value is string or int or float or double or bool or Vector2 or Vector2Int or Vector3 or Vector3Int or UUID or Enum or UnityEngine.Object or RangeInt)
                return true;
            else if (value is IList && value.GetType().IsGenericType)
                return true;
            return false;
        }


        public static RangeInt DrawRangeField(string labelName, RangeInt value) => DrawRangeField(new GUIContent(labelName), value);
        public static RangeInt DrawRangeField(GUIContent label, RangeInt value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(label, GUILayout.MaxWidth(150));
            EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(50));
            int min = EditorGUILayout.IntField(value.min);
            EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(50));
            int max = EditorGUILayout.IntField(value.max);
            EditorGUILayout.EndHorizontal();
            RangeInt ret = new(min, max);
            return ret;
        }

        public static ReorderableList DrawList(string labelName, IList list, Type type, ElementCallbackDelegate elementDrawer = null) => DrawList(new GUIContent(labelName), list, type, elementDrawer);
        public static ReorderableList DrawList(GUIContent label, IList list, Type type, ElementCallbackDelegate elementDrawer = null)
        {
            ReorderableList r = new(list, type, true, true, true, true);
            ElementCallbackDelegate drawer = elementDrawer ?? new ElementCallbackDelegate((rect, index, isActive, isFocused) => { DrawField(index.ToString(), list[index]); });
            r.elementHeight = EditorGUIUtility.singleLineHeight;
            r.drawElementCallback += drawer;
            r.onAddCallback += (r) => r.list.Add(Activator.CreateInstance(type));
            r.drawHeaderCallback += (rect) => GUI.Label(rect, label);
            r.DoLayoutList();
            return r;
        }





        public static PageList DrawListPage(SerializedProperty serializedProperty)
        {
            return new PageList(serializedProperty);
        }





        public static GUIStyle SetRegionColor(Color color, out Color baseColor)
        {
            GUIStyle colorStyle = new();
            colorStyle.normal.background = Texture2D.whiteTexture;

            baseColor = GUI.backgroundColor;
            GUI.backgroundColor = color;
            return colorStyle;
        }
    }
}
