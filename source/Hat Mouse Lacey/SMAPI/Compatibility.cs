/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/HatMouseLacey
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Collections.Generic;
using xTile;
using xTile.Layers;
using xTile.Tiles;

namespace ichortower_HatMouseLacey
{
    /*
     * This class holds functions that handle compatibility with other mods.
     * Put something here to handle it if it's impossible or slow via other
     * methods.
     */
    internal class LCCompat
    {
        public static void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            /*
             * EditMap patch for the forest map.
             * Doing it here in C# is more powerful than via Content Patcher
             * (since we can check source tiles) and a little less verbose.
             * TODO extend this for SVE support
             *
             * The loop here checks the Back layer at specific locations and
             * maps tile index values to new ones, mostly to turn yucky grass
             * into nice grass or to remove fence holes.
             * It's extremely lucky that I can omit 1966 in the map to avoid
             * special-casing the new door location for SVR3.
             */
            if (e.Name.IsEquivalentTo("Maps/Forest")) {
                e.Edit(asset => {
                    var mapref = asset.AsMap().Data;
                    Layer back = mapref.GetLayer("Back");
                    //Layer buildings = mapref.GetLayer("Buildings");
                    //Layer front = mapref.GetLayer("Front");
                    var backList = new List<Vector2>() {
                        new Vector2(28, 99),
                        new Vector2(31, 96),
                        new Vector2(32, 98),
                        new Vector2(36, 98),
                        new Vector2(37, 98),
                        new Vector2(37, 96),
                        new Vector2(38, 96),
                        new Vector2(38, 95),
                        new Vector2(37, 93),
                        new Vector2(38, 93),
                        new Vector2(38, 92),
                        new Vector2(45, 98),
                        new Vector2(47, 97),
                        new Vector2(48, 97),
                        new Vector2(49, 99),
                    };
                    //var buildingsList = new List<Vector2>() {
                        //new Vector2(37, 93), new Vector2(38, 93)
                    //};
                    //var frontList = new List<Vector2>() {
                        //new Vector2(37, 92), new Vector2(38, 92)
                    //};
                    /* -1 in the value here means delete (null) */
                    var convertDict = new Dictionary<int, int>() {
                        {256, 175},
                        {400, 175},
                        {401, 175},
                        {1964, 175},
                        {1965, 175},
                        {329, 351},
                        {405, 999},
                        //{383, -1},
                        //{384, -1},
                        //{385, -1},
                        //{358, -1},
                        //{359, -1},
                        //{360, -1},
                    };
                    /* saved delegate here for if the other two layers are restored
                    Action<Vector2, Layer> mutate = delegate(Vector2 coords, Layer layer) */
                    foreach (var coords in backList) {
                        Tile t = back.Tiles[(int)coords.X, (int)coords.Y];
                        if (t is null) {
                            continue;
                        }
                        int target;
                        if (!convertDict.TryGetValue(t.TileIndex, out target)) {
                            continue;
                        }
                        /* delete if the value is -1, as above */
                        if (target == -1) {
                            back.Tiles[(int)coords.X, (int)coords.Y] = null;
                        }
                        else {
                            t.TileIndex = target;
                        }
                    };
                }, AssetEditPriority.Late);
            }
        }
    }
}
