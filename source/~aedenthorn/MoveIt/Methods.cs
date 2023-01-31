/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;
using Object = StardewValley.Object;

namespace MoveIt
{
    public partial class ModEntry
    {

        private void PickupObject()
        {
            if (Config.ModKey != SButton.None && !Helper.Input.IsDown(Config.ModKey))
            {
                if (movingObject is not null)
                {
                    PlaceObject();
                }
                return;
            }
            Vector2 cursorTile = Game1.lastCursorTile;
            var mp = Game1.getMousePosition() + new Point(Game1.viewport.Location.X, Game1.viewport.Location.Y);

            foreach (var c in Game1.currentLocation.characters)
            {
                var bb = c.GetBoundingBox();
                if(c is NPC)
                    bb = new Rectangle(bb.Location - new Point(0, 64), new Point(64, 128));
                if (bb.Contains(mp))
                {
                    Pickup(c, cursorTile, c.Name);
                    return;
                }
            }
            if (Game1.currentLocation is Farm)
            {
                foreach (var a in (Game1.currentLocation as Farm).animals.Values)
                {
                    if (a.GetBoundingBox().Contains(mp))
                    {
                        Pickup(a, cursorTile, a.Name);
                        return;
                    }
                }
            }
            if (Game1.currentLocation is AnimalHouse)
            {
                foreach (var a in (Game1.currentLocation as AnimalHouse).animals.Values)
                {
                    if (a.GetBoundingBox().Contains(mp))
                    {
                        Pickup(a, cursorTile, a.Name);
                        return;
                    }
                }
            }
            if (Game1.currentLocation is Forest)
            {
                foreach (var a in (Game1.currentLocation as Forest).marniesLivestock)
                {
                    if (a.GetBoundingBox().Contains(mp))
                    {
                        Pickup(a, cursorTile, a.Name);
                        return;
                    }
                }
                if ((Game1.currentLocation as Forest).log?.occupiesTile((int)cursorTile.X, (int)cursorTile.Y) == true)
                {
                    Pickup((Game1.currentLocation as Forest).log, cursorTile, (Game1.currentLocation as Forest).log.GetType().ToString());
                    return;
                }
            }
            if (Game1.currentLocation is Woods)
            {

                foreach (var rc in (Game1.currentLocation as Woods).stumps)
                {
                    if (rc.occupiesTile((int)cursorTile.X, (int)cursorTile.Y))
                    {
                        Pickup(rc, cursorTile, rc.GetType().ToString());
                        return;
                    }
                }
            }
            if (Game1.currentLocation.objects.TryGetValue(cursorTile, out var obj))
            {
                Pickup(obj, cursorTile, obj.Name);
                return;
            }
            foreach (var rc in Game1.currentLocation.resourceClumps)
            {
                if (rc.occupiesTile((int)cursorTile.X, (int)cursorTile.Y))
                {
                    Pickup(rc, cursorTile, rc.GetType().ToString());
                    return;
                }
            }
            if (Game1.currentLocation.largeTerrainFeatures is not null)
            {

                foreach (var ltf in Game1.currentLocation.largeTerrainFeatures)
                {
                    if (ltf.getBoundingBox().Contains((int)cursorTile.X * 64, (int)cursorTile.Y * 64))
                    {
                        Pickup(ltf, cursorTile, ltf.GetType().ToString());
                        return;
                    }
                }
            }
            if (Game1.currentLocation.terrainFeatures.TryGetValue(cursorTile, out var tf))
            {
                Pickup(tf, cursorTile, tf.GetType().ToString());
                return;

            }
        }
        public static void PlaceObject()
        {
            if (!Config.ModEnabled)
            {
                movingObject = null;
                return;
            }
            if (movingObject is null)
                return;
            SHelper.Input.Suppress(Config.MoveKey);
            if (movingObject is Character)
            {
                (movingObject as Character).Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                movingObject = null;
            }
            else if (movingObject is Object)
            {
                if (Game1.currentLocation.objects.ContainsKey(movingTile))
                {
                    if (Config.ProtectOverwrite && Game1.currentLocation.objects.ContainsKey(Game1.currentCursorTile))
                    {
                        Game1.playSound("cancel");
                        SMonitor.Log($"Preventing overwrite", StardewModdingAPI.LogLevel.Info);
                        return;
                    }
                    var obj = Game1.currentLocation.objects[movingTile];
                    Game1.currentLocation.objects.Remove(movingTile);
                    Game1.currentLocation.objects[Game1.currentCursorTile] = obj;
                    Game1.currentLocation.objects[Game1.currentCursorTile].TileLocation = Game1.currentCursorTile;
                    movingObject = null;
                }
            }
            else if (movingObject is FarmAnimal)
            {
                (movingObject as FarmAnimal).Position = (Game1.getMousePosition() + new Point(Game1.viewport.Location.X - 32, Game1.viewport.Location.Y - 32)).ToVector2();
                movingObject = null;
            }
            else if (movingObject is ResourceClump)
            {
                if (Game1.currentLocation is Forest && (Game1.currentLocation as Forest).log == movingObject)
                {
                    (Game1.currentLocation as Forest).log.currentTileLocation = Game1.lastCursorTile;
                    (Game1.currentLocation as Forest).log.tile.Value = Game1.lastCursorTile;
                    movingObject = null;
                }
                else if (Game1.currentLocation is Woods && (Game1.currentLocation as Woods).stumps.IndexOf(movingObject as ResourceClump) >= 0)
                {
                    var index = (Game1.currentLocation as Woods).stumps.IndexOf(movingObject as ResourceClump);
                    (Game1.currentLocation as Woods).stumps[index].currentTileLocation = Game1.lastCursorTile;
                    (Game1.currentLocation as Woods).stumps[index].tile.Value = Game1.lastCursorTile;
                    movingObject = null;
                }
                else
                {
                    var index = Game1.currentLocation.resourceClumps.IndexOf(movingObject as ResourceClump);
                    if (index >= 0)
                    {
                        Game1.currentLocation.resourceClumps[index].currentTileLocation = Game1.lastCursorTile;
                        Game1.currentLocation.resourceClumps[index].tile.Value = Game1.lastCursorTile;
                        movingObject = null;
                    }
                }
            }
            else if (movingObject is TerrainFeature)
            {
                if (movingObject is LargeTerrainFeature)
                {
                    var index = Game1.currentLocation.largeTerrainFeatures.IndexOf(movingObject as LargeTerrainFeature);
                    if (index >= 0)
                    {
                        Game1.currentLocation.largeTerrainFeatures[index].currentTileLocation = Game1.lastCursorTile;
                        Game1.currentLocation.largeTerrainFeatures[index].tilePosition.Value = Game1.lastCursorTile;
                        movingObject = null;
                    }
                }
                else if (Game1.currentLocation.terrainFeatures.ContainsKey(movingTile))
                {
                    if (Config.ProtectOverwrite && Game1.currentLocation.terrainFeatures.ContainsKey(Game1.currentCursorTile))
                    {
                        Game1.playSound("cancel");
                        SMonitor.Log($"Preventing overwrite", StardewModdingAPI.LogLevel.Info);
                        return;
                    }
                    var tf = Game1.currentLocation.terrainFeatures[movingTile];
                    Game1.currentLocation.terrainFeatures.Remove(movingTile);
                    Game1.currentLocation.terrainFeatures[Game1.currentCursorTile] = tf;
                    if (Game1.currentLocation.terrainFeatures[Game1.currentCursorTile] is HoeDirt)
                    {
                        (Game1.currentLocation.terrainFeatures[Game1.currentCursorTile] as HoeDirt).updateNeighbors(Game1.currentLocation, Game1.currentCursorTile);
                        if ((Game1.currentLocation.terrainFeatures[Game1.currentCursorTile] as HoeDirt).crop is not null)
                            (Game1.currentLocation.terrainFeatures[Game1.currentCursorTile] as HoeDirt).crop.updateDrawMath(Game1.currentCursorTile);
                    }
                    movingObject = null;
                }
            }
            if (movingObject is null)
            {
                PlaySound();
            }
        }

        private static void PlaySound()
        {
            if(!string.IsNullOrEmpty(Config.Sound))
                Game1.playSound(Config.Sound);
        }

        private void Pickup(object obj, Vector2 cursorTile, string name)
        {
            movingObject = obj;
            movingTile = cursorTile;
            SMonitor.Log($"Picked up {name}");
            Helper.Input.Suppress(Config.MoveKey);
            PlaySound();
        }
    }
}