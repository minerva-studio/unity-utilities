
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Minerva.Module
{
    public delegate string GetItemNameCallback(object baseMaster, object master);
    /// <summary>
    /// [Dropdown(path, displayProperty)]
    /// 
    /// - path:            the path of the List
    /// - displayProperty: the property you want to display in the dropdown selection
    /// 
    /// </summary>
    public class DropdownAttribute : PropertyAttribute
    {
        public Type type = null;
        public string listPath = "";
        public string itemNameProperty = "";
        public List<Object> list = new List<Object>();
        public int selectedID = -1;
        public bool hasItemName;

        public DropdownAttribute(string listPath, string ItemNameProperty)
        {//With property name to get name
         //[Dropdown("SkillDatabase.Instance.SkillList", "skillID")]
            this.listPath = listPath;
            this.itemNameProperty = ItemNameProperty;
            hasItemName = true;
        }
        public DropdownAttribute(string listPath)
        {
            this.listPath = listPath;
            hasItemName = false;
        }
    } 
}