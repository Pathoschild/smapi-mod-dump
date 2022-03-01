/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HDPortraits
**
*************************************************/

using HarmonyLib;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace HDPortraits
{
    static class Utils
    {
        public static MethodInfo MethodNamed(this Type type, string name, Type[] args = null)
        {
            return (args == null) ? AccessTools.Method(type, name) : AccessTools.Method(type, name, args);
        }
        public static FieldInfo FieldNamed(this Type type, string name)
        {
            return AccessTools.Field(type, name);
        }
        public static Dictionary<string, int> MapDict(this Dictionary<string, string> source)
        {
            Dictionary<string, int> ret = new();
            foreach ((string key, string val) in source)
                if (int.TryParse(val, out int num))
                    ret[key] = num;
            return ret;
        }

        public static bool TryLoadAsset<T>(string path, out T value)
        {
            try
            {
                value = ModEntry.helper.Content.Load<T>(path, ContentSource.GameContent);
                return true;
            } catch(Exception _)
            {
                value = default;
                return false;
            }
        }
    }
}
