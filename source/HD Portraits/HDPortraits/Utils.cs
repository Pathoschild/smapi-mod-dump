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
using StardewModdingAPI.Utilities;
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
            ModEntry.monitor.Log($"Attempting to load asset from {path}.");
            try
            {
                value = ModEntry.helper.GameContent.Load<T>(path);
                return true;
            } catch(Exception e)
            {
                ModEntry.monitor.LogOnce(e.Message);
                value = default;
                return false;
            }
        }

        public static string WithoutPath(this IAssetName name, string path)
        {
            if (!name.StartsWith(path, false))
                return null;

            int count = PathUtilities.GetSegments(path).Length;
            return string.Join(PathUtilities.PreferredAssetSeparator, PathUtilities.GetSegments(name.ToString()).Skip(count));
        }
    }
}
