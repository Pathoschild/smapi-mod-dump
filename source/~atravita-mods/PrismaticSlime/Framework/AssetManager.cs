/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using AtraCore;
using AtraCore.Models;
using AtraShared.ConstantsAndEnums;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;

namespace PrismaticSlime.Framework;

/// <summary>
/// Handles asset editing for this mod.
/// </summary>
internal static class AssetManager
{
    private static readonly string OBJECTDATA = PathUtilities.NormalizeAssetName("Data/ObjectInformation");
    private static readonly string RINGMASK = PathUtilities.NormalizeAssetName("Mods/atravita_Prismatic_Ring/Texture");

    /// <summary>
    /// Applies the requested asset edits and loads.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (ModEntry.PrismaticSlimeEgg != -1 && e.NameWithoutLocale.IsEquivalentTo(OBJECTDATA))
        {
            e.Edit(EditObjects);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(AtraCoreConstants.PrismaticMaskData))
        {
            e.Edit(EditPrismaticMasks);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(RINGMASK))
        {
            e.LoadFromModFile<Texture2D>("assets/json-assets/Objects/PrismaticSlimeRing/mask.png", AssetLoadPriority.Exclusive);
        }
    }

    private static void EditObjects(IAssetData asset)
    {
        IAssetDataForDictionary<int, string>? editor = asset.AsDictionary<int, string>();
        if (editor.Data.TryGetValue(ModEntry.PrismaticSlimeEgg, out string? val))
        {
            editor.Data[ModEntry.PrismaticSlimeEgg] = val.Replace("Basic -20", "Basic");
        }
        else
        {
            ModEntry.ModMonitor.Log($"Could not find {ModEntry.PrismaticSlimeEgg} in ObjectInformation to edit! This mod may not function properly.", LogLevel.Error);
        }
    }

    private static void EditPrismaticMasks(IAssetData asset)
    {
        IAssetDataForDictionary<string, DrawPrismaticModel>? editor = asset.AsDictionary<string, DrawPrismaticModel>();

        DrawPrismaticModel? ring = new()
        {
            ItemType = ItemTypeEnum.Ring,
            Identifier = "atravita.PrismaticSlimeRing",
            Mask = RINGMASK,
        };

        DrawPrismaticModel? egg = new()
        {
            ItemType = ItemTypeEnum.SObject,
            Identifier = "atravita.PrismaticSlime Egg",
        };

        if (!editor.Data.TryAdd(ring.Identifier, ring))
        {
            ModEntry.ModMonitor.Log("Could not add prismatic slime ring to DrawPrismatic", LogLevel.Warn);
        }

        if (!editor.Data.TryAdd(egg.Identifier, egg))
        {
            ModEntry.ModMonitor.Log("Could not add prismatic slime egg to DrawPrismatic", LogLevel.Warn);
        }
    }
}
