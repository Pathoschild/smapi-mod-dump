/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Core;

#region using directives

using DaLion.Shared.Integrations.GMCM.Attributes;
using Newtonsoft.Json;
using StardewModdingAPI.Utilities;

#endregion using directives

/// <summary>Config schema for the Core mod.</summary>
public sealed class CoreConfig
{
    /// <summary>Gets the chance a crop may wither per day left un-watered.</summary>
    [JsonProperty]
    [GMCMRange(0f, 1f)]
    [GMCMStep(0.05f)]
    public float CropWitherChance { get; internal set; } = 0f;

    /// <summary>Gets the key used to engage Debug Mode.</summary>
    [JsonProperty]
    [GMCMIgnore]
    public KeybindList DebugKey { get; internal set; } = KeybindList.Parse("OemQuotes, OemTilde");
}
