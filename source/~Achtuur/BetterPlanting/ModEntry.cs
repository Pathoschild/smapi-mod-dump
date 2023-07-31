/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using AchtuurCore.Extensions;
using BetterPlanting.Extensions;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System.Collections.Generic;
using System.Linq;
using SObject = StardewValley.Object;

namespace BetterPlanting;

public class ModEntry : Mod
{
    internal static ModEntry Instance;
    internal ModConfig Config;

    internal TilePlaceOverlay UIOverlay;
    internal TileFiller TileFiller;

    internal static bool IsObjectAtTileGardenPot(Vector2 tile)
    {
        SObject sobj = Game1.currentLocation.getObjectAtTile(tile);
        return IsObjectGardenPot(sobj);
    }
    internal static bool IsObjectGardenPot(SObject sobj)
    {
        return sobj is not null && sobj.Name == "Garden Pot";
    }

    internal static bool PlayerIsHoldingPlantableObject()
    {
        return Game1.player.IsHoldingCategory(SObject.SeedsCategory) || Game1.player.IsHoldingCategory(SObject.fertilizerCategory);
    }

    internal static bool CanPlantHeldObject(Vector2 tile)
    {
        if (Context.IsWorldReady && !Game1.currentLocation.isTileHoeDirt(tile))
            return false;

        if (!PlayerIsHoldingPlantableObject())
            return false;

        StardewValley.Item held_object = Game1.player.CurrentItem;
        bool isFertilizer = held_object.Category == SObject.fertilizerCategory;

        // Tilled dirt
        if (Game1.currentLocation.terrainFeatures.ContainsKey(tile))
        {
            HoeDirt tileFeature = Game1.currentLocation.terrainFeatures[tile] as HoeDirt;
            return tileFeature.canPlantThisSeedHere(held_object.ParentSheetIndex, (int)tile.X, (int)tile.Y, isFertilizer)
                && !Game1.currentLocation.isObjectAtTile(tile);
        }

        // Garden pot
        else if (IsObjectAtTileGardenPot(tile))
        {
            IndoorPot pot = Game1.currentLocation.getObjectAtTile(tile) as IndoorPot;
            return pot.hoeDirt.Value.canPlantThisSeedHere(held_object.ParentSheetIndex, (int)tile.X, (int)tile.Y, isFertilizer);
        }

        return false;
    }

    internal static bool TileContainsAliveCrop(Vector2 tile)
    {
        // Garden pot
        if (IsObjectAtTileGardenPot(tile))
        {
            IndoorPot pot = Game1.currentLocation.getObjectAtTile(tile) as IndoorPot;
            return pot.hoeDirt.Value.crop is not null && !pot.hoeDirt.Value.crop.dead.Value;
        }

        // If no crop -> no alive crop
        if (!Game1.currentLocation.isCropAtTile((int)tile.X, (int)tile.Y))
            return false;

        // Check if crop is dead
        HoeDirt tileFeature = Game1.currentLocation.terrainFeatures[tile] as HoeDirt;
        return !tileFeature.crop.dead.Value;
    }

    internal static bool IsCursorTilePlantable()
    {
        // cursor has to be on ring of 8 tiles around player and object must be plantable
        float player_cursor_distance = (Game1.currentCursorTile - Game1.player.getTileLocation()).Length();
        return player_cursor_distance < 1.5f && CanPlantHeldObject(Game1.currentCursorTile);
    }

    public override void Entry(IModHelper helper)
    {

        I18n.Init(helper.Translation);
        ModEntry.Instance = this;

        UIOverlay = new TilePlaceOverlay();
        UIOverlay.Enable();
        TileFiller = new TileFiller();

        this.Config = this.Helper.ReadConfig<ModConfig>();

        helper.Events.Display.RenderedWorld += this.OnRenderedWorld;
        helper.Events.GameLoop.GameLaunched += this.OnGameLaunch;
        helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        helper.Events.Player.Warped += this.OnPlayerWarped;
    }

    private void OnPlayerWarped(object sender, WarpedEventArgs e)
    {
        this.TileFiller.SetFillMode(FillMode.Disabled);
    }

    private void OnRenderedWorld(object sender, RenderedWorldEventArgs e)
    {
        this.UIOverlay.DrawOverlay(e.SpriteBatch);
    }

    private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
    {

        if (e.Button == SButton.MouseLeft)
        {
            this.TryPlantSeed(Game1.player.CurrentItem, Game1.currentCursorTile);
        }
        else if (e.Button == Config.IncrementModeKey)
        {
            this.TileFiller.IncrementFillMode(1);

        }
        else if (e.Button == Config.DecrementModeKey)
        {
            this.TileFiller.IncrementFillMode(-1);
        }
    }

    private void OnGameLaunch(object sender, GameLaunchedEventArgs e)
    {
        this.Config.createMenu();
    }

    private void TryPlantSeed(Item held_object, Vector2 CursorTile)
    {
        if (!PlayerIsHoldingPlantableObject() || !Context.IsPlayerFree)
            return;

        IEnumerable<FillTile> tiles = TileFiller.GetFillTiles(Game1.player.getTileLocation(), CursorTile)
            .Where(t => t.IsPlantable());

        if (IsCursorTilePlantable())
            tiles = tiles.Where(t => t.Location != CursorTile);

        foreach (FillTile tile in tiles)
        {
            bool planted_hoedirt = TryPlantSeedHoeDirt(tile, held_object);
            bool planted_gardenpot = TryPlantSeedGardenPot(tile, held_object);

            if (planted_hoedirt || planted_gardenpot)
                Game1.player.ActiveObject.ConsumeInventoryItem(Game1.player, held_object.ParentSheetIndex, 1);
        }

    }

    /// <summary>
    /// Try planting on <paramref name="tile"/> by assuming it is a <see cref="HoeDirt"/> tile.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="held_object"></param>
    /// <returns>True if sucessfully planted, false if not</returns>
    private bool TryPlantSeedHoeDirt(FillTile tile, Item held_object)
    {
        if (!tile.IsHoeDirt())
            return false;

        HoeDirt tileFeature = Game1.currentLocation.terrainFeatures[tile.Location] as HoeDirt;
        bool isFertilizer = held_object.Category == SObject.fertilizerCategory;

        return tileFeature.plant(held_object.ParentSheetIndex, (int)tile.Location.X, (int)tile.Location.Y, Game1.player, isFertilizer, Game1.currentLocation);

        
    }

    /// <summary>
    /// Try planting on <paramref name="tile"/> by assuming it is an <see cref="IndoorPot"/> (Garden Pot)
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="held_object"></param>
    /// <returns>Whether seed had been sucessfully planted</returns>
    private bool TryPlantSeedGardenPot(FillTile tile, Item held_object)
    {
        if (!tile.IsGardenPot())
            return false;

        IndoorPot gardenPot = Game1.currentLocation.getObjectAtTile(tile.Location) as IndoorPot;
        bool isFertilizer = held_object.Category == SObject.fertilizerCategory;

        return gardenPot.hoeDirt.Value.plant(held_object.ParentSheetIndex, (int)tile.Location.X, (int)tile.Location.Y, Game1.player, isFertilizer, Game1.currentLocation);
    }
}
