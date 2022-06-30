/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/MuseumRewardsIn
**
*************************************************/

using System.Text.RegularExpressions;
using AtraShared.ItemManagement;
using AtraShared.Utils.Extensions;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using xTile.Dimensions;
using xTile.ObjectModel;

using AtraUtils = AtraShared.Utils.Utils;

namespace MuseumRewardsIn;

[HarmonyPatch(typeof(Utility))]
internal class ModEntry : Mod
{
    private const string BUILDING = "Buildings";
    private const string SHOPNAME = "atravita.MuseumShop";

    private static readonly Regex MuseumObject = new("museumCollectedReward(?<type>[a-zA-Z]+)_(?<id>[0-9]+)_", RegexOptions.Compiled, TimeSpan.FromMilliseconds(250));

    private static IMonitor ModMonitor = null!;

    /// <inheritdoc />
    public override void Entry(IModHelper helper)
    {
        ModMonitor = this.Monitor;
        helper.Events.Input.ButtonPressed += this.Input_ButtonPressed;
        helper.Events.Player.Warped += this.OnWarped;
        helper.Events.Content.AssetRequested += this.OnAssetRequested;

        I18n.Init(helper.Translation);

        Harmony harmony = new(this.ModManifest.UniqueID);
        harmony.PatchAll();
    }

    /// <summary>
    /// Postfix to add furniture to the catalog.
    /// </summary>
    /// <param name="__result">shop inventory to add to.</param>
    [HarmonyPatch(nameof(Utility.getAllFurnituresForFree))]
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
                        ModMonitor.DebugOnlyLog($"Adding {item.Name} to catalogue!", LogLevel.Info);
                    }
                    else
                    {
                        ModMonitor.Log($"Could not add {item.Name} to catalogue, may be a duplicate!", LogLevel.Warn);
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
    private void Input_ButtonPressed(object? sender, ButtonPressedEventArgs e)
    {
        if (!e.Button.IsActionButton() && !e.Button.IsUseToolButton())
        {
            return;
        }

        if (!Context.IsWorldReady || !Context.CanPlayerMove || Game1.player.isRidingHorse()
            || Game1.currentLocation is null || Game1.eventUp || Game1.isFestival() || Game1.IsFading()
            || Game1.activeClickableMenu is not null)
        {
            return;
        }

        this.Monitor.DebugOnlyLog(Game1.currentLocation?.doesTileHaveProperty((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", BUILDING) ?? string.Empty);

        if (Game1.currentLocation is not LibraryMuseum museum
            || museum.doesTileHavePropertyNoNull((int)e.Cursor.GrabTile.X, (int)e.Cursor.GrabTile.Y, "Action", BUILDING) != SHOPNAME)
        {
            return;
        }

        Dictionary<ISalable, int[]> sellables = new();

        foreach (string mailflag in Game1.player.mailReceived)
        {
            Match match = MuseumObject.Match(mailflag);
            if (match.Success && int.TryParse(match.Groups["id"].Value, out int id))
            {
                if (ItemUtils.GetItemFromIdentifier(match.Groups["type"].Value, id) is Item item
                    && !(item is SObject obj && (obj.Category == SObject.SeedsCategory || obj.IsRecipe)))
                {
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
                (asset) =>
                {
                    var map = asset.AsMap();
                    var tile = map.Data.GetLayer(BUILDING).PickTile(new Location(4 * 64, 9 * 64), Game1.viewport.Size);
                    tile.Properties.Add("Action", new PropertyValue(SHOPNAME));
                },
                AssetEditPriority.Early);
        }
    }

    private void OnWarped(object? sender, WarpedEventArgs e)
    {
        if (e.NewLocation is LibraryMuseum)
        {
            Vector2 tile = new(4f, 9f); // default location of shop.
            foreach (Vector2 v in AtraUtils.YieldAllTiles(e.NewLocation))
            { // find the shop tile - a mod may have moved it.
                if (e.NewLocation.doesTileHaveProperty((int)v.X, (int)v.Y, "Action", BUILDING)?.Contains(SHOPNAME) == true)
                {
                    tile = v;
                    break;
                }
            }

            // add box
            e.NewLocation.temporarySprites.Add(new TemporaryAnimatedSprite
            {
                texture = Game1.mouseCursors2,
                sourceRect = new Microsoft.Xna.Framework.Rectangle(129, 210, 13, 16),
                animationLength = 1,
                sourceRectStartingPos = new Vector2(129f, 210f),
                interval = 50000f,
                totalNumberOfLoops = 9999,
                position = (new Vector2(tile.X, tile.Y - 1) * Game1.tileSize) + (new Vector2(3f, 0f) * 4f),
                scale = 4f,
                layerDepth = (((tile.Y - 0.5f) * Game1.tileSize) / 10000f) + 0.01f, // a little offset so it doesn't show up on the floor.
                id = 777f,
            });
        }
    }
}
