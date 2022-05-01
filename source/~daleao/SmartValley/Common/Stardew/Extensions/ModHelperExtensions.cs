/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

namespace DaLion.Common.Stardew.Extensions;

#region using directives

using System;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;

#endregion using directives

/// <summary>Extensions for the <see cref="IModHelper"/> interface.</summary>
public static class ModHelperExtensions
{
    /// <inheritdoc cref="IModHelper.ReadConfig"/>
    public static JObject ReadConfigExt(this IModHelper helper, string uniqueId, Action<string, LogLevel> log)
    {
        var modInfo = helper.ModRegistry.Get(uniqueId);
        if (modInfo is null)
        {
            log($"{uniqueId} mod not found. You can install it for additional features.", LogLevel.Info);
            return null;
        }

        log($"{uniqueId} mod found. Enabling integrations...", LogLevel.Info);
        var modEntry = (IMod) modInfo.GetType().GetProperty("Mod")!.GetValue(modInfo);
        return modEntry!.Helper.ReadConfig<JObject>();
    }
}