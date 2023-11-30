/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI.Events;

namespace CommunityUpgradeFramework;

internal static class AssetManager
{
    internal static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.CommunityUpgradesPath))
        {
            e.LoadFrom(
                () => new Dictionary<string, CommunityUpgrade>(),
                AssetLoadPriority.Medium
            );
        }
    }
}
