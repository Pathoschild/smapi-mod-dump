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

using AtraShared.Caching;
using AtraShared.ConstantsAndEnums;
using AtraShared.Utils;

using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

namespace PrismaticSlime.Framework;

/// <summary>
/// Handles asset editing for this mod.
/// </summary>
internal static class AssetManager
{
    private static IAssetName objectData = null!;

    private static IAssetName maskLocation = null!;
    private static IAssetName ringMask = null!;
    private static IAssetName toastMask = null!;

    private static IAssetName buffTextureLocation = null!;

    private static Lazy<Texture2D> buffTex = new(() => Game1.content.Load<Texture2D>(buffTextureLocation.BaseName));

    internal static Texture2D BuffTexture => buffTex.Value;

    /// <summary>
    /// Initializes the asset manager.
    /// </summary>
    /// <param name="parser">Game content helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        objectData = parser.ParseAssetName("Data/ObjectInformation");

        ringMask = parser.ParseAssetName("Mods/atravita_Prismatic_Ring/Texture");
        toastMask = parser.ParseAssetName("Mods/atravita_Prismatic_Toast/Texture");
        maskLocation = parser.ParseAssetName(AtraCoreConstants.PrismaticMaskData);

        buffTextureLocation = parser.ParseAssetName("Mods/atravita_Prismatic_Buff/Texture");
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated"/>
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if (buffTex.IsValueCreated && (assets is null || assets.Contains(buffTextureLocation)))
        {
            buffTex = new(static () => Game1.content.Load<Texture2D>(buffTextureLocation.BaseName));
        }
    }

    /// <summary>
    /// Applies the requested asset edits and loads.
    /// </summary>
    /// <param name="e">Event arguments.</param>
    internal static void Apply(AssetRequestedEventArgs e)
    {
        if (ModEntry.PrismaticSlimeEgg != -1 && e.NameWithoutLocale.IsEquivalentTo(objectData))
        {
            e.Edit(EditObjects);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(maskLocation))
        {
            e.Edit(EditPrismaticMasks);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(ringMask))
        {
            e.LoadFromModFile<Texture2D>("assets/json-assets/Objects/PrismaticSlimeRing/mask.png", AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(toastMask))
        {
            e.LoadFromModFile<Texture2D>("assets/json-assets/Objects/PrismaticJellyToast/mask.png", AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(buffTextureLocation))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/buff.png", AssetLoadPriority.Exclusive);
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
            Mask = ringMask.BaseName,
        };

        DrawPrismaticModel? egg = new()
        {
            ItemType = ItemTypeEnum.SObject,
            Identifier = "atravita.PrismaticSlime Egg",
        };

        DrawPrismaticModel? toast = new()
        {
            ItemType = ItemTypeEnum.SObject,
            Identifier = "atravita.PrismaticJellyToast",
            Mask = toastMask.BaseName,
        };

        if (!editor.Data.TryAdd(ring.Identifier, ring))
        {
            ModEntry.ModMonitor.Log("Could not add prismatic slime ring to DrawPrismatic", LogLevel.Warn);
        }

        if (!editor.Data.TryAdd(egg.Identifier, egg))
        {
            ModEntry.ModMonitor.Log("Could not add prismatic slime egg to DrawPrismatic", LogLevel.Warn);
        }

        if (!editor.Data.TryAdd(toast.Identifier, toast))
        {
            ModEntry.ModMonitor.Log("Could not add prismatic jelly toast to DrawPrismatic", LogLevel.Warn);
        }
    }
}
