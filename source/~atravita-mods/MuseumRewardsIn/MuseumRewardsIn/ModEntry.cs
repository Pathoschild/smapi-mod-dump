/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System.Text.RegularExpressions;
using AtraShared.Integrations;
using AtraShared.ItemManagement;
using AtraShared.Menuing;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;
using xTile.ObjectModel;
using AtraUtils = AtraShared.Utils.Utils;
using XTile = xTile.Tiles.Tile;

namespace MuseumRewardsIn;

/// <inheritdoc />
[HarmonyPatch(typeof(Utility))]
internal sealed class ModEntry : Mod
{
    private const string BUILDING = "Buildings";
    private const string SHOPNAME = "atravita.MuseumShop";

    private static readonly Regex MuseumObject = new(
        pattern: "museumCollectedReward(?<type>[a-zA-Z]+)_(?<id>[0-9]+)_",
        options: RegexOptions.Compiled,
        matchTimeout: TimeSpan.FromMilliseconds(250));

    private static IMonitor modMonitor = null!;

    private static Vector2 shopLoc = new(4, 9);

    /// <summary>
    /// The config class for this mod.
    /// </summary>
    /// <remarks>WARNING: NOT SET IN ENTRY.</remarks>
    private static ModConfig config = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        modMonitor = this.Monitor;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.Player.Warped += this.OnWarped;
        helper.Events.Content.AssetRequested += this.OnAssetRequested;

        I18n.Init(helper.Translation);

        Harmony harmony = new(this.ModManifest.UniqueID);
        harmony.PatchAll();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        // move the default one to the left for SVE.
        if (this.Helper.ModRegistry.IsLoaded("FlashShifter.SVECode"))
        {
            shopLoc = new(3, 9);
        }

        config = AtraUtils.GetConfigOrDefault<ModConfig>(this.Helper, this.Monitor);
        if (config.BoxLocation == new Vector2(-1, -1))
        {
            config.BoxLocation = shopLoc;
            this.Helper.AsyncWriteConfig(this.Monitor, config);
        }

        GMCMHelper gmcm = new(this.Monitor, this.Helper.Translation, this.Helper.ModRegistry, this.ModManifest);
        if (gmcm.TryGetAPI())
        {
            gmcm.Register(
                reset: static () => config = new(),
                save: () => this.Helper.AsyncWriteConfig(this.Monitor, config))
            .AddTextOption(
                name: I18n.BoxLocation_Name,
                getValue: static () => config.BoxLocation.X + ", " + config.BoxLocation.Y,
                setValue: static (str) => config.BoxLocation = str.TryParseVector2(out Vector2 vec) ? vec : shopLoc,
                tooltip: I18n.BoxLocation_Description);
        }
    }

    /// <summary>
    /// Postfix to add furniture to the catalog.
    /// </summary>
    /// <param name="__result">shop inventory to add to.</param>
    [HarmonyPatch(nameof(Utility.getAllFurnituresForFree))]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1313:Parameter names should begin with lower-case letter", Justification = "Harmony convention.")]
    [SuppressMessage("StyleCop.CSharp.OrderingRules", "SA1204:Static elements should appear before instance elements", Justification = "Reviewed.")]
    private static void Postfix(Dictionary<ISalable, int[]> __result)
    {
        foreach (string mailflag in Game1.player.mailReceived)
        {
            Match match = MuseumObject.Match(mailflag);
            if (match.Success && int.TryParse(match.Groups["id"].Value, out int id)
                && match.Groups["type"].Value is "F" or "f")
            {
                if (ItemUtils.GetItemFromIdentifier(match.Groups["type"].Value, id) is Item item)
                {
                    if (__result.TryAdd(item, new int[] { 0, int.MaxValue }))
                    {
                        modMonitor.DebugOnlyLog($"Adding {item.Name} to catalogue!", LogLevel.Info);
                    }
                    else
                    {
                        modMonitor.Log($"Could not add {item.Name} to catalogue, may be a duplicate!", LogLevel.Warn);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Handles opening the shop menu in the museum.
    /// </summary>
    /// <param name="sender">SMAPI.</param>
    /// <param name="e">event args.</param>
    private void OnButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!MenuingExtensions.IsNormalGameplay() || (!e.Button.IsActionButton() && !e.Button.IsUseToolButton()))
        {
            return;
        }

        this.Monitor.DebugOnlyLog(Game1.currentLocation?.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", BUILDING) ?? string.Empty);

        if (Game1.currentLocation is not LibraryMuseum museum
            || museum.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", BUILDING) != SHOPNAME)
        {
            return;
        }

        this.Helper.Input.SurpressClickInput();

        Dictionary<ISalable, int[]> sellables = new();

        foreach (string mailflag in Game1.player.mailReceived)
        {
            Match match = MuseumObject.Match(mailflag);
            if (match.Success && int.TryParse(match.Groups["id"].Value, out int id))
            {
                if (ItemUtils.GetItemFromIdentifier(match.Groups["type"].Value, id) is Item item
                    && !(item is SObject obj && (obj.Category == SObject.SeedsCategory || obj.IsRecipe)))
                {
                    if (item.Name.StartsWith("Dwarvish Translation Guide"))
                    {
                        continue;
                    }
                    int[] selldata = new int[] { Math.Max(item.salePrice() * 2, 2000), int.MaxValue };
                    sellables.Add(item, selldata);
                }
            }
        }

        var shop = new ShopMenu(sellables, who: "Gunther");
        if (Game1.getCharacterFromName("Gunther") is NPC gunter)
        {
            shop.portraitPerson = gunter;
        }
        shop.potraitPersonDialogue = I18n.ShopMessage();
        Game1.activeClickableMenu = shop;
    }

    private void OnAssetRequested(object? sender, AssetRequestedEventArgs e)
    {
        if (e.NameWithoutLocale.IsEquivalentTo("Maps/ArchaeologyHouse"))
        {
            e.Edit(
                static (asset) =>
                {
                    IAssetDataForMap? map = asset.AsMap();
                    XTile? tile = map.Data.GetLayer(BUILDING).PickTile(new Location((int)config.BoxLocation.X * 64, (int)config.BoxLocation.Y * 64), Game1.viewport.Size);
                    if (tile is null)
                    {
                        modMonitor.Log($"Tile could not be edited for shop, please let atra know!", LogLevel.Warn);
                        return;
                    }
                    tile.Properties["Action"] = new PropertyValue(SHOPNAME);
                },
                AssetEditPriority.Default + 10);
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation is LibraryMuseum)
        {
            Vector2 tile = config.BoxLocation; // default location of shop.
            foreach (Vector2 v in AtraUtils.YieldAllTiles(e.NewLocation))
            { // find the shop tile - a mod may have moved it.
                if (e.NewLocation.doesTileHaveProperty((int)v.X, (int)v.Y, "Action", BUILDING)?.Equals(SHOPNAME, StringComparison.OrdinalIgnoreCase) == true)
                {
                    tile = v;
                    break;
                }
            }

            this.Monitor.DebugOnlyLog($"Adding boxen to {tile}", LogLevel.Info);

            // add box
            e.NewLocation.temporarySprites.Add(new TemporaryAnimatedSprite
            {
                texture = Game1.mouseCursors2,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16),
                animationLength = 1,
                sourceRectStartingPos = new Vector2(129f, 210f),
                interval = 50000f,
                totalNumberOfLoops = 9999,
                position = (new Vector2(tile.X, tile.Y - 1) * Game1.tileSize) + (new Vector2(3f, 0f) * Game1.pixelZoom),
                scale = Game1.pixelZoom,
                layerDepth = Math.Clamp((((tile.Y - 0.5f) * Game1.tileSize) / 10000f) + 0.15f, 0f, 1.0f), // a little offset so it doesn't show up on the floor.
                id = 777f,
            });
        }
    }
}
