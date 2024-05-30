/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using LazyMod.Framework.Config;
using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;
using SObject = StardewValley.Object;

namespace LazyMod.Framework.Automation;

internal abstract class Automate
{
    protected ModConfig Config;
    protected readonly Func<int, List<Vector2>> GetTileGrid;

    protected Automate(ModConfig config, Func<int, List<Vector2>> getTileGrid)
    {
        Config = config;
        GetTileGrid = getTileGrid;
    }

    public abstract void AutoDoFunction(GameLocation location, Farmer player, Tool? tool, Item? item);
    
    public void UpdateConfig(ModConfig config)
    {
        Config = config;
    }

    protected T? FindToolFromInventory<T>() where T : Tool
    {
        var player = Game1.player;
        if (player.CurrentTool is T tool) return tool;
        return player.Items.FirstOrDefault(item => item is T) as T;
    }

    protected void UseToolOnTile(GameLocation location, Farmer player, Tool tool, Vector2 tile)
    {
        var tilePixelPosition = GetTilePixelPosition(tile);
        tool.swingTicker++;
        tool.DoFunction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, 1, player);
    }

    protected void ConsumeItem(Farmer player, Item item)
    {
        item.Stack--;
        if (item.Stack <= 0) player.removeItemFromInventory(item);
    }


    protected Vector2 GetTilePixelPosition(Vector2 tile, bool center = true)
    {
        return tile * Game1.tileSize + (center ? new Vector2(Game1.tileSize / 2f) : Vector2.Zero);
    }

    protected Rectangle GetTileBoundingBox(Vector2 tile)
    {
        var tilePixelPosition = GetTilePixelPosition(tile, false);
        return new Rectangle((int)tilePixelPosition.X, (int)tilePixelPosition.Y, Game1.tileSize, Game1.tileSize);
    }

    protected void CheckTileAction(GameLocation location, Farmer player, Vector2 tile)
    {
        location.checkAction(new Location((int)tile.X, (int)tile.Y), Game1.viewport, player);
    }

    protected void HarvestMachine(Farmer player, SObject? machine)
    {
        if (machine is null) return;

        var heldObject = machine.heldObject.Value;
        if (machine.readyForHarvest.Value && heldObject is not null)
        {
            if (player.freeSpotsInInventory() == 0 && !player.Items.ContainsId(heldObject.ItemId)) return;
            machine.checkForAction(player);
        }
    }

    protected bool CanAddItemToInventory(Item item)
    {
        return Game1.player.freeSpotsInInventory() > 0 || Game1.player.Items.Any(item.canStackWith);
    }
}