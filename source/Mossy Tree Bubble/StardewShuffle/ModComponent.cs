/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tocseoj/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace Tocseoj.Stardew.StardewShuffle;
internal abstract class ModComponent(IMonitor monitor, IManifest modManifest, IModHelper helper, ModConfig config)
{
    internal IMonitor Monitor { get; } = monitor;
    internal IManifest ModManifest { get; } = modManifest;
    internal IModHelper Helper { get; } = helper;
    internal ModConfig Config { get; set; } = config;
}