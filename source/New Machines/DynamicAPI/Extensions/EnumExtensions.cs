/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Igorious.StardewValley.DynamicAPI.Extensions
{
    public static class EnumExtensions
    {
        public static TEnum ToEnum<TEnum>(this string s)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), s, true);
        }

        public static string GetDescription(this Enum e)
        {
            var type = e.GetType();
            var memberInfo = type.GetMember(e.ToString()).First();
            var attr = memberInfo.GetCustomAttribute<DescriptionAttribute>();
            return (attr != null)? attr.Description : e.ToString();
        }

        public static string ToLower(this Enum e)
        {
            return e.ToString().ToLower();
        }
    }
}
