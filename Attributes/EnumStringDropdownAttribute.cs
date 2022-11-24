using System;
using UnityEngine;

namespace Minerva.Module
{
    public sealed class EnumStringDropdownAttribute : PropertyAttribute
    {
        public Type Type = null;
        public int SelectedID = -1;


        public EnumStringDropdownAttribute(Type enumType)
        {
            Type = enumType;
        }
    }
}