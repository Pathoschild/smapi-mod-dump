/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using StardewModdingAPI.Events;
using StardewValley.Menus;

namespace SeeAnimalPronouns;

internal class EventHookManager
{
    internal static void InitializeEventHooks()
    {
        Globals.EventHelper.Content.AssetRequested += LoadAssets;
        Globals.EventHelper.Content.AssetsInvalidated += DirtyAssets;

        Globals.EventHelper.Display.MenuChanged += ClearCacheIfNecessary;
    }

    private static void ClearCacheIfNecessary(object sender, MenuChangedEventArgs e)
    {
        if (e.OldMenu is not null && e.OldMenu.GetType() == typeof(AnimalQueryMenu))
            Helpers.ClearCache();
    }

    private static void DirtyAssets(object sender, AssetsInvalidatedEventArgs e)
    {
        foreach (var nameWithoutLocale in e.NamesWithoutLocale)
            if (nameWithoutLocale.IsEquivalentTo(Globals.BasePronounsAssetPath))
            {
                Helpers.BasePronounsDictDirty = true;
            }
    }

    private static void LoadAssets(object sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo(Globals.BasePronounsAssetPath))
        {
            e.LoadFromModFile<Dictionary<string, Helpers.PronounSet>>(Globals.BasePronounsAssetFilePath, AssetLoadPriority.Medium);
            Helpers.BasePronounsDictDirty = true;
        }
    }


}
