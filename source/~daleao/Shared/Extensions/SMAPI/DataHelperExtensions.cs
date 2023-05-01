/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Extensions.SMAPI;

#region using directives

using System.Reflection;
using Newtonsoft.Json;

#endregion using directives

/// <summary>Extensions for the <see cref="IDataHelper"/> interface.</summary>
public static class DataHelperExtensions
{
    /// <summary>Gets the <see cref="JsonSerializerSettings"/> from the data <paramref name="helper"/> instance.</summary>
    /// <param name="helper">The <see cref="IModHelper"/> of the current <see cref="IMod"/>.</param>
    /// <returns>The <see cref="JsonSerializerSettings"/>.</returns>
    public static JsonSerializerSettings GetJsonSerializerSettings(this IDataHelper helper)
    {
        var dataHelperType = Type.GetType("StardewModdingAPI.Framework.ModHelpers.DataHelper, StardewModdingAPI")!;
        var jsonHelperType = Type.GetType("StardewModdingAPI.Toolkit.Serialization.JsonHelper, SMAPI.Toolkit")!;
        var jsonHelperField = dataHelperType.GetField(
            "JsonHelper",
            BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!;
        var jsonSettingsGetter =
            jsonHelperType.GetProperty(
                "JsonSettings",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)!.GetGetMethod()!;
        var jsonHelper = jsonHelperField.GetValue(helper)!;
        return (JsonSerializerSettings)jsonSettingsGetter.Invoke(jsonHelper, null)!;
    }
}
