using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Minerva.Module
{
    /// <summary>
    /// Allow Field not to display in the inspector is the given field in the instance match value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisplayIfAttribute : PropertyAttribute
    {
        public string name = "";
        public object[] expectValues;
        public bool match; 

        /// <summary>
        /// if field match with any value
        /// </summary>
        /// <param name="listPath"></param>
        /// <param name="expectValues"></param>
        public DisplayIfAttribute(string listPath = "", params object[] expectValues)
        {//With property name to get name
         //[Dropdown("SkillDatabase.Instance.SkillList", "skillID")]
            name = listPath;
            this.expectValues = expectValues;
        }

        /// <summary>
        /// if field is true
        /// </summary>
        /// <param name="listPath"></param>
        public DisplayIfAttribute(string listPath) : this(listPath, true)
        {
        }

        public bool EqualsAny(object value)
        {
            return EqualsAny(value, expectValues);
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
            if (expect is IComparable) return value is IComparable b && b.CompareTo(expect) == 0;
            return value.Equals(expect);
        }


        public static bool EqualsAny(object value, object[] expects)
        {
            return expects.Any(expect => MatchWithExpect(value, expect));
        }
    }
}