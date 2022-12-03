using UnityEngine;

namespace Minerva.Module
{
    public class ReadOnlyIfAttribute : ConditionalFieldAttribute
    {
        /// <summary>
        /// if field match with any value
        /// </summary>
        /// <param name="listPath"></param>
        /// <param name="expectValues"></param>
        public ReadOnlyIfAttribute(string listPath, bool result, params object[] expectValues) : base(listPath, result, expectValues)
        {
        }

        /// <summary>
        /// if field match with any value
        /// </summary>
        /// <param name="listPath"></param>
        /// <param name="expectValues"></param>
        public ReadOnlyIfAttribute(string listPath, bool result) : base(listPath, true, result)
        {
        }

        /// <summary>
        /// if field is true
        /// </summary>
        /// <param name="listPath"></param>
        public ReadOnlyIfAttribute(string listPath) : base(listPath)
        {
        }
    }
}