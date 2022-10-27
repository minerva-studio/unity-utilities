using UnityEngine;

namespace Minerva.Module
{
    public class ReadOnlyIfAttribute : PropertyAttribute
    {
        public string listPath = "";
        public object expectValue;
        public bool allowPathNotFoundEdit;

        public ReadOnlyIfAttribute(string listPath, object expectValue)
        {//With property name to get name
         //[Dropdown("SkillDatabase.Instance.SkillList", "skillID")]
            this.listPath = listPath;
            this.expectValue = expectValue;
        }

        public ReadOnlyIfAttribute(string listPath, object expectValue, bool allowPathNotFoundEdit)
        {//With property name to get name
         //[Dropdown("SkillDatabase.Instance.SkillList", "skillID")]
            this.listPath = listPath;
            this.expectValue = expectValue;
            this.allowPathNotFoundEdit = allowPathNotFoundEdit;
        }

        public ReadOnlyIfAttribute(string listPath)
        {
            this.listPath = listPath;
            expectValue = true;
        }
    }
}