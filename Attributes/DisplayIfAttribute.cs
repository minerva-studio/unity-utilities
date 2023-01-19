using System;

namespace Minerva.Module
{
    /// <summary>
    /// Allow Field not to display in the inspector is the given field in the instance match value
    /// </summary>
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DisplayIfAttribute : ConditionalFieldAttribute
    {
        /// <summary>
        /// if field match with any value
        /// </summary>
        /// <param name="listPath"></param>
        /// <param name="expectValues"></param>
        public DisplayIfAttribute(string listPath = "", params object[] expectValues) : base(listPath, true, expectValues)
        {
        }
        /// <summary>
        /// if field match or not match with any value
        /// </summary>
        /// <param name="listPath"></param>
        /// <param name="expectValues"></param>
        public DisplayIfAttribute(string listPath, bool result, params object[] expectValues) : base(listPath, result, expectValues)
        {
        }

        /// <summary>
        /// if field match given truth value
        /// </summary>
        /// <param name="listPath"></param>
        /// <param name="expectValues"></param>
        public DisplayIfAttribute(string listPath, bool result) : base(listPath, true, result)
        {
        }

        /// <summary>
        /// if field is true
        /// </summary>
        /// <param name="listPath"></param>
        public DisplayIfAttribute(string listPath) : base(listPath)
        {
        }
    }
}