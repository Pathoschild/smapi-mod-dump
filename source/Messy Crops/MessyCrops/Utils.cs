/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/MessyCrops
**
*************************************************/

using HarmonyLib;
using System;
using System.Reflection;

namespace MessyCrops
{
	internal static class Utils
	{
		public static MethodInfo MethodNamed(this Type type, string name)
			=> AccessTools.Method(type, name);
		public static MethodInfo MethodNamed(this Type type, string name, Type[] args)
			=> AccessTools.Method(type, name, args);
		public static FieldInfo FieldNamed(this Type type, string name)
			=> AccessTools.Field(type, name);
	}
}
