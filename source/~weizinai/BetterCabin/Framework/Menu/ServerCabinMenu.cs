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
using StardewValley.Menus;

namespace weizinai.StardewValleyMod.BetterCabin.Framework.Menu;

internal class ServerCabinMenu : CarpenterMenu
{
    public ServerCabinMenu() : base("Robin")
    {
        this.Blueprints.Clear();
        var index = 0;
        foreach (var (id, data) in Game1.buildingData)
        {
            if (data.IndoorMapType == "StardewValley.Locations.Cabin")
            {
                this.Blueprints.Add(new BlueprintEntry(index++, id, data, null));
            }
        }
        this.SetNewActiveBlueprint(0);
    }

    public override void performHoverAction(int x, int y)
    {
        base.performHoverAction(x, y);

        if (this.onFarm)
        {
            var tile = PositionHelper.GetTilePositionFromMousePosition();
            var building = this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y))
                ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 1))
                ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 2))
                ?? this.TargetLocation.getBuildingAt(new Vector2(tile.X, tile.Y + 3));
            if (building?.isCabin == false) building.color = Color.White;
        }
    }

    public override void receiveLeftClick(int x, int y, bool playSound = true)
    {
        if (this.demolishing || this.painting)
        {
            var building = this.TargetLocation.getBuildingAt(PositionHelper.GetTilePositionFromMousePosition());
            if (building?.isCabin == false) return;
        }
        
        base.receiveLeftClick(x, y, playSound);

        if (this.moving && this.buildingToMove?.isCabin == false)
        {
            this.buildingToMove.isMoving = false;
            this.buildingToMove = null;
        }
    }
}