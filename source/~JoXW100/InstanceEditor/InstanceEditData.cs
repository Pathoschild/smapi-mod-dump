/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace InstanceEditor
{
    public class InstanceEditData
    {
        public string className = "";
        public Dictionary<string, FieldEditData> matchFields = new Dictionary<string, FieldEditData>();
        public Dictionary<string, FieldEditData> changeFields = new Dictionary<string, FieldEditData>();
        public string[] checks = new string[] { };
    }

    public class FieldEditData
    {
        public object value;
        public FieldInfo fieldInfo;
        public Dictionary<string, object> fields;
    }
}