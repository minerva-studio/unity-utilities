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
    public static class EditorFieldDrawers
    {
        /// <summary>
        /// Draw field by <paramref name="field"/>, with label <paramref name="labelName"/>
        /// </summary>
        /// <param name="labelName"> label fo the field </param>
        /// <param name="field"> the field info </param>
        /// <param name="target"> the instance </param>
        public static void DrawField(string labelName, FieldInfo field, object target)
        {
            if (field is null)
            {
                EditorGUILayout.LabelField(labelName, "Field not instantiated");
                return;
            }
            object value = DrawField(labelName, field.GetValue(target));
            if (value is not null) field.SetValue(target, value);
        }

        /// <summary>
        /// Draw given value
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="value"></param>
        /// <param name="isReadOnly"></param>
        /// <param name="drawWarning"> if the field type is not supported, draw not supported message in editor</param>
        /// <returns> new value if changed, old if no change </returns>
        public static object DrawField(string labelName, object value, bool isReadOnly, bool drawWarning = true)
        {
            var GUIState = GUI.enabled;
            if (isReadOnly)
            {
                GUI.enabled = false;
            }
            var ret = DrawField(labelName, value, drawWarning);
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
        public static object DrawField(string labelName, object value, bool drawWarning = true)
        {
            if (value is string s)
            {
                return EditorGUILayout.TextField(labelName, s);
            }
            else if (value is int i)
            {
                return EditorGUILayout.IntField(labelName, i);
            }
            else if (value is float f)
            {
                return EditorGUILayout.FloatField(labelName, f);
            }
            else if (value is double d)
            {
                return EditorGUILayout.DoubleField(labelName, d);
            }
            else if (value is bool b)
            {
                return EditorGUILayout.Toggle(labelName, b);
            }
            else if (value is Vector2 v2)
            {
                return EditorGUILayout.Vector2Field(labelName, v2);
            }
            else if (value is Vector2Int v2i)
            {
                return EditorGUILayout.Vector2IntField(labelName, v2i);
            }
            else if (value is Vector3 v3)
            {
                return EditorGUILayout.Vector3Field(labelName, v3);
            }
            else if (value is Vector3Int v3i)
            {
                return EditorGUILayout.Vector3IntField(labelName, v3i);
            }
            else if (value is UUID uUID)
            {
                EditorGUILayout.LabelField(labelName, uUID.Value);
                return uUID;
            }
            else if (value is Enum e)
            {
                if (Attribute.GetCustomAttribute(value.GetType(), typeof(FlagsAttribute)) != null)
                {
                    return EditorGUILayout.EnumFlagsField(labelName, e);
                }
                else return EditorGUILayout.EnumPopup(labelName, e);
            }
            else if (value is UnityEngine.Object uo)
            {
                return EditorGUILayout.ObjectField(labelName, uo, value.GetType(), false);
            }
            else if (value is RangeInt r)
            {
                return DrawRangeField(labelName, r);
            }
            else if (value is IList list && value.GetType().IsGenericType)
            {
                var itemType = value.GetType().GenericTypeArguments[0];
                DrawList(list, itemType, labelName);
            }
            else if (drawWarning) EditorGUILayout.LabelField("Do not support drawing type " + value?.GetType().Name ?? "");
            return value;
        }



        public static RangeInt DrawRangeField(string labelName, RangeInt value)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(labelName, GUILayout.MaxWidth(150));
            EditorGUILayout.LabelField("Min", GUILayout.MaxWidth(30));
            int min = EditorGUILayout.IntField(value.min);
            EditorGUILayout.LabelField("Max", GUILayout.MaxWidth(30));
            int max = EditorGUILayout.IntField(value.max);
            EditorGUILayout.EndHorizontal();
            RangeInt ret = new RangeInt(min, max);
            return ret;
        }

        public static ReorderableList DrawList(IList list, Type type, string labelName, ElementCallbackDelegate elementDrawer = null)
        {
            ReorderableList r = new(list, type, true, true, true, true);
            ElementCallbackDelegate drawer = elementDrawer ?? new ElementCallbackDelegate((rect, index, isActive, isFocused) => { DrawField(index.ToString(), list[index]); });
            r.elementHeight = EditorGUIUtility.singleLineHeight;
            r.drawElementCallback += drawer;
            r.onAddCallback += (r) => r.list.Add(Activator.CreateInstance(type));
            r.drawHeaderCallback += (rect) => GUI.Label(rect, labelName);
            r.DoLayoutList();
            return r;
        }
    }
}
