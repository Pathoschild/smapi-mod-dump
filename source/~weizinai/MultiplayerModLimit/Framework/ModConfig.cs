/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

namespace weizinai.StardewValleyMod.MultiplayerModLimit.Framework;

internal class ModConfig
{
    public bool EnableMod { get; set; } = true;
    public int KickPlayerDelayTime { get; set; } = 1;
    public bool RequireSMAPI { get; set; } = true;
    public LimitMode LimitMode { get; set; } = LimitMode.WhiteListMode;
    public string AllowedModListSelected { get; set; } = "Default";
    public string RequiredModListSelected { get; set; } = "Default";
    public string BannedModListSelected { get; set; } = "Default";

    public Dictionary<string, List<string>> AllowedModList { get; set; } = new() { { "Default", new List<string>() } };
    public Dictionary<string, List<string>> RequiredModList { get; set; } = new() { { "Default", new List<string>() } };
    public Dictionary<string, List<string>> BannedModList { get; set; } = new() { { "Default", new List<string>() } };
}