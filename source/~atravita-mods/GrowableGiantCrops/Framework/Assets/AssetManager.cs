/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Buffers;

using AtraBase.Toolkit.Extensions;

using AtraShared.Utils.Extensions;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI.Events;

using StardewValley.TerrainFeatures;

namespace GrowableGiantCrops.Framework.Assets;

/// <summary>
/// Manages loading and editing assets for this mod.
/// </summary>
internal static class AssetManager
{
    /// <summary>
    /// The const string that starts for running JA/MGC textures through the content pipeline.
    /// </summary>
    internal const string GiantCropPrefix = "Mods/atravita.GrowableGiantCrops/";

    private static IAssetName fruitTreeData = null!;

    private static Lazy<Dictionary<int, int>> reverseFruitTreeMap = new(GenerateFruitTreeMap);

    /// <summary>
    /// An error texture, used to fill in if a JA/MGC texture is not found.
    /// </summary>
    private static Texture2D errorTex = null!;

    private static Lazy<Texture2D> toolTex = new(() => Game1.content.Load<Texture2D>(ToolTextureName!.BaseName));

    /// <summary>
    /// Gets the tool texture.
    /// </summary>
    internal static Texture2D ToolTexture => toolTex.Value;

    /// <summary>
    /// Gets the IAssetName corresponding to the shovel's texture.
    /// </summary>
    internal static IAssetName ToolTextureName { get; private set; } = null!;

    /// <summary>
    /// Gets the IAssetName corresponding to the shop graphics.
    /// </summary>
    internal static IAssetName ShopGraphics { get; private set; } = null!;

    #region palm trees
    internal static IAssetName WinterBigPalm { get; private set; } = null!;

    internal static IAssetName WinterPalm { get; private set; } = null!;

    internal static IAssetName FallBigPalm { get; private set; } = null!;

    internal static IAssetName FallPalm { get; private set; } = null!;

    #endregion

    /// <summary>
    /// Initializes the AssetManager.
    /// </summary>
    /// <param name="parser">GameContent helper.</param>
    internal static void Initialize(IGameContentHelper parser)
    {
        ToolTextureName = parser.ParseAssetName($"{GiantCropPrefix}Shovel");
        ShopGraphics = parser.ParseAssetName($"{GiantCropPrefix}Shop");
        fruitTreeData = parser.ParseAssetName(@"Data\fruitTrees");

        WinterBigPalm = parser.ParseAssetName($"{GiantCropPrefix}WinterBigPalm");
        WinterPalm = parser.ParseAssetName($"{GiantCropPrefix}WinterPalm");

        FallBigPalm = parser.ParseAssetName($"{GiantCropPrefix}FallBigPalm");
        FallPalm = parser.ParseAssetName($"{GiantCropPrefix}FallPalm");

        const int TEX_WIDTH = 48;
        const int TEX_HEIGHT = 64;
        Color[] buffer = ArrayPool<Color>.Shared.Rent(TEX_WIDTH * TEX_HEIGHT);
        try
        {
            Array.Fill(buffer, Color.MonoGameOrange, 0, TEX_WIDTH * TEX_HEIGHT);
            Texture2D tex = new(Game1.graphics.GraphicsDevice, TEX_WIDTH, TEX_HEIGHT) { Name = GiantCropPrefix + "ErrorTex" };
            tex.SetData(0, new Rectangle(0, 0, TEX_WIDTH, TEX_HEIGHT), buffer, 0, TEX_WIDTH * TEX_HEIGHT);
            errorTex = tex;
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed while creating error tex:\n\n{ex}", LogLevel.Error);
        }
        finally
        {
            ArrayPool<Color>.Shared.Return(buffer);
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetRequested" />
    internal static void OnAssetRequested(AssetRequestedEventArgs e)
    {
        if (!e.NameWithoutLocale.StartsWith(GiantCropPrefix, false, false))
        {
            return;
        }

        if (int.TryParse(e.NameWithoutLocale.BaseName.GetNthChunk('/', 2), out int idx))
        {
            if (ModEntry.JaAPI?.TryGetGiantCropSprite(idx, out Lazy<Texture2D>? lazy) == true)
            {
                e.LoadFrom(() => lazy.Value, AssetLoadPriority.Exclusive);
            }
            else if (ModEntry.MoreGiantCropsAPI?.GetTexture(idx) is Texture2D tex)
            {
                e.LoadFrom(() => tex, AssetLoadPriority.Exclusive);
            }
            else
            {
                e.LoadFrom(() => errorTex, AssetLoadPriority.Exclusive);
            }
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(ToolTextureName))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/shovel.png", AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(ShopGraphics))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/void_grass_box.png", AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(WinterBigPalm))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/winter_palm2.png", AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(WinterPalm))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/winter_palm.png", AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(FallBigPalm))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/fall_palm2.png", AssetLoadPriority.Exclusive);
        }
        else if (e.NameWithoutLocale.IsEquivalentTo(FallPalm))
        {
            e.LoadFromModFile<Texture2D>("assets/textures/fall_palm.png", AssetLoadPriority.Exclusive);
        }
    }

    /// <inheritdoc cref="IContentEvents.AssetsInvalidated" />
    internal static void Reset(IReadOnlySet<IAssetName>? assets = null)
    {
        if ((assets is null || assets.Contains(ToolTextureName)) && toolTex.IsValueCreated)
        {
            toolTex = new(() => Game1.content.Load<Texture2D>(ToolTextureName.BaseName));
        }
        if ((assets is null || assets.Contains(fruitTreeData)) && reverseFruitTreeMap.IsValueCreated)
        {
            reverseFruitTreeMap = new(GenerateFruitTreeMap);
        }
    }

    /// <summary>
    /// Given a fruit tree index, looks up the sapling index.
    /// </summary>
    /// <param name="treeIndex"><see cref="FruitTree.treeType"/>.</param>
    /// <returns>Sapling index, or null for not found.</returns>
    internal static int? GetMatchingSaplingIndex(int treeIndex)
        => reverseFruitTreeMap.Value.TryGetValue(treeIndex, out int saplingIndex) ? saplingIndex : null;

    private static Dictionary<int, int> GenerateFruitTreeMap()
    {
        ModEntry.ModMonitor.DebugOnlyLog($"Generating reverse tree map");
        Dictionary<int, string>? data;
        try
        {
            data = Game1.content.Load<Dictionary<int, string>>(fruitTreeData.BaseName);
        }
        catch (Exception ex)
        {
            ModEntry.ModMonitor.Log($"Failed to generate reverse fruit tree lookup map:\n\n{ex}", LogLevel.Error);
            return new Dictionary<int, int>();
        }
        Dictionary<int, int> map = new(data.Count);

        foreach ((int saplingIndex, string s) in data)
        {
            if (!int.TryParse(s.GetNthChunk('/'), out int treeIndex))
            {
                ModEntry.ModMonitor.Log($"Malformed tree data: {saplingIndex} - {s}, skipping", LogLevel.Warn);
            }
            if (!map.TryAdd(treeIndex, saplingIndex))
            {
                ModEntry.ModMonitor.Log($"Duplicate fruit tree saplingIndex: {saplingIndex} - {s}, skipping", LogLevel.Warn);
            }
        }

        return map;
    }
}
