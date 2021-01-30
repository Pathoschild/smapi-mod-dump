/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ha1fdaew/iTile
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;

namespace iTile.Core.Logic.Action
{
    [ActionAttr(ActionManager.Action.Delete)]
    public class DeleteTile : IAction
    {
        public Tile Execute(Vector2 pos, Tile tile = null)
        {
            Layer layer = Game1.currentLocation.map.GetLayer(CoreManager.SelectedLayer);
            if (layer == null)
            {
                CoreManager.ShowNotification(CoreManager.defaultNotificationTime, "Unable to identify layer");
                return null;
            }

            Tile t = layer.PickTile(new Location((int)pos.X, (int)pos.Y) * Game1.tileSize, Game1.viewport.Size);
            if (t == null)
            {
                CoreManager.ShowNotification(CoreManager.defaultNotificationTime, "There's no tile at this position\non layer " + layer.Id);
                return null;
            }

            CoreManager.Instance.saveManager.session.GetLocationProfileSafe(Game1.currentLocation).HandleTileReplacement(null, layer.Id, pos);
            CoreManager.ShowNotification(CoreManager.defaultNotificationTime, "Successfully deleted a tile");
            return null;
        }
    }
}