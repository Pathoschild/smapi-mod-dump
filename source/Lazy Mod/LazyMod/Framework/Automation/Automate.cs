/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/weizinai/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Tools;
using xTile.Dimensions;
using Rectangle = Microsoft.Xna.Framework.Rectangle;

namespace LazyMod.Framework.Automation;

public abstract class Automate
{
    protected readonly Lazy<MeleeWeapon> FakeScythe = new(() => new MeleeWeapon("47"));

    public abstract void AutoDoFunction(GameLocation? location, Farmer player, Tool? tool, Item? item);

    protected IEnumerable<Vector2> GetTileGrid(Vector2 origin, int range)
    {
        for (var x = -range; x <= range; x++)
        for (var y = -range; y <= range; y++)
            yield return new Vector2(origin.X + x, origin.Y + y);
    }

    protected T? FindToolFromInventory<T>(bool findScythe = false) where T : Tool
    {
        var player = Game1.player;
        if (player.CurrentTool is T tool)
        {
            if (findScythe && tool is MeleeWeapon scythe && scythe.isScythe())
                return tool;
            return tool;
        }

        foreach (var item in player.Items)
            if (findScythe && item is MeleeWeapon scythe && scythe.isScythe())
                return scythe as T;

        return player.Items.FirstOrDefault(item => item is T) as T;
    }

    protected void UseToolOnTile(GameLocation location, Farmer player, Tool tool, Vector2 tile)
    {
        var tilePixelPosition = GetTilePixelPosition(tile);
        tool.DoFunction(location, (int)tilePixelPosition.X, (int)tilePixelPosition.Y, 1, player);
    }

    protected bool StopAutomate(Farmer player, float stopAutomateStamina, ref bool hasAddMessage)
    {
        if (player.Stamina <= stopAutomateStamina)
        {
            if (!hasAddMessage)
                Game1.showRedMessage(I18n.MessageStamina());
            return true;
        }

        hasAddMessage = false;
        return false;
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
}