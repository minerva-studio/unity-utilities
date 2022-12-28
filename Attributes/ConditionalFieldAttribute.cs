using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace Minerva.Module
{
    /// <summary>
    /// Base class for all conditional field
    /// </summary>
    public abstract class ConditionalFieldAttribute : PropertyAttribute
    {
        public string path = "";
        public object[] expectValues;
        public bool result;


        /// <summary>
        /// if field match with any value
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expectValues"></param>
        public ConditionalFieldAttribute(string path, bool result, params object[] expectValues)
        {
            this.path = path;
            this.result = result;
            this.expectValues = expectValues;
        }

        /// <summary>
        /// if field is true
        /// </summary>
        /// <param name="listPath"></param>
        public ConditionalFieldAttribute(string listPath) : this(listPath, true, true)
        {
        }

        /// <summary>
        /// check whether given object value matches any value stored in attribute
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public virtual bool EqualsAny(object value)
        {
            return expectValues.Any(expect => MatchWithExpect(value, expect));
        }

        /// <summary>
        /// is condition specified in the attribute true?
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="field"></param>
        /// <returns></returns>
        public static bool IsTrue(object obj, FieldInfo field)
        {
            var type = obj.GetType();
            if (IsDefined(field, typeof(ConditionalFieldAttribute)))
            {
                var attrs = (ConditionalFieldAttribute[])GetCustomAttributes(field, typeof(ConditionalFieldAttribute));
                foreach (var attr in attrs)
                {
                    string dependent = attr.path;
                    if (attr.EqualsAny(type.GetField(dependent).GetValue(obj)) != attr.result)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private static bool MatchWithExpect(object value, object expect)
        {
            if (expect is Enum e1 && expect.GetType() == value.GetType() && GetCustomAttribute(expect.GetType(), typeof(FlagsAttribute)) != null)
            {
                //Debug.Log(value);
                //Debug.Log(e1);
                if (((Enum)value).HasFlag(e1))
                {
                    return true;
                }
            }

            if (double.TryParse(expect.ToString(), out var a) && double.TryParse(value.ToString(), out var b))
            {
                return a == b;
            }
            if (expect is IComparable) return value is IComparable c && Comparer.Default.Compare(c, expect) == 0;
            return value.Equals(expect);
        }
    }
}