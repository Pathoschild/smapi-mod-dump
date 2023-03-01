/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Shockah/Stardew-Valley-Mods
**
*************************************************/

using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Reflection;

namespace Shockah.Kokoro.SMAPI
{
	public static class JsonSerializerExt
    {
		public static JsonSerializerSettings GetSMAPISerializerSettings(IDataHelper dataHelper)
		{
			Type dataHelperType = Type.GetType("StardewModdingAPI.Framework.ModHelpers.DataHelper, StardewModdingAPI")!;
			Type jsonHelperType = Type.GetType("StardewModdingAPI.Toolkit.Serialization.JsonHelper, SMAPI.Toolkit")!;

			FieldInfo jsonHelperField = dataHelperType.GetField("JsonHelper", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
			MethodInfo jsonSettingsGetter = jsonHelperType.GetProperty("JsonSettings", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.GetGetMethod()!;

			var jsonHelper = jsonHelperField.GetValue(dataHelper)!;
			return (JsonSerializerSettings)jsonSettingsGetter.Invoke(jsonHelper, null)!;
		}
    }
}