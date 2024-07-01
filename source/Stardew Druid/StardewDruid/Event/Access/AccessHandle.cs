/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Location;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Layers;
using xTile.Tiles;

namespace StardewDruid.Event.Access
{
    public class AccessHandle
    {

        public GameLocation location;

        public Vector2 stair;

        public Vector2 exit;

        public string entrance;

        public string access;

        public bool manualSet;

        public AccessHandle() { }

        public void AccessSetup(string Entrance, string Access, Vector2 Stair, Vector2 Exit)
        {

            stair = Stair;

            entrance = Entrance;

            access = Access;

            exit = Exit;

        }

        public virtual void AccessCheck(GameLocation Location)
        {

            location = Location;

            if (CheckStair())
            {

                return;

            }

            if (Utility.isOnScreen(stair * 64,64))
            {

                ModUtility.AnimateHands(Game1.player, Game1.player.FacingDirection, 600);

                Mod.instance.iconData.ImpactIndicator(location, stair * 64 + new Vector2(64, 0), IconData.impacts.impact, 6, new());

                location.playSound("boulderBreak");

            }

            AccessStair();

            AccessWarps();

        }

        public virtual bool CheckStair()
        {

            foreach (Warp warp in location.warps)
            {

                if (warp.X == stair.X)
                {

                    return true;

                }

            }

            return false;

        }

        public void AccessStair()
        {

            TileSheet tileSheet = new(
                location.map,
                IconData.chapel_assetName,
                new(
                    Mod.instance.iconData.sheetTextures[IconData.tilesheets.chapel].Width / 16,
                    Mod.instance.iconData.sheetTextures[IconData.tilesheets.chapel].Height / 16
                ),
                new(16, 16)
            );

            location.map.AddTileSheet(tileSheet);

            location.map.LoadTileSheets(Game1.mapDisplayDevice);

            int tilex = (int)stair.X;

            int tiley = (int)stair.Y;

            Layer buildings = location.map.GetLayer("Buildings");

            buildings.Tiles[tilex, tiley] = new StaticTile(buildings, tileSheet, BlendMode.Alpha, 56);

            buildings.Tiles[tilex, tiley].TileIndexProperties.Add("Passable", new(true));

            buildings.Tiles[tilex + 1, tiley] = new StaticTile(buildings, tileSheet, BlendMode.Alpha, 57);

            buildings.Tiles[tilex + 2, tiley] = new StaticTile(buildings, tileSheet, BlendMode.Alpha, 58);

            buildings.Tiles[tilex, tiley + 1] = new StaticTile(buildings, tileSheet, BlendMode.Alpha, 70);

            buildings.Tiles[tilex, tiley + 1].TileIndexProperties.Add("Passable", new(true));

            buildings.Tiles[tilex + 1, tiley + 1] = new StaticTile(buildings, tileSheet, BlendMode.Alpha, 71);

            buildings.Tiles[tilex + 2, tiley + 1] = new StaticTile(buildings, tileSheet, BlendMode.Alpha, 72);

            location.localSound("secret1");

        }

        public void AccessWarps()
        {

            int tilex = (int)stair.X;

            int tiley = (int)stair.Y;

            location.warps.Add(new Warp(tilex, tiley, access, (int)exit.X, (int)exit.Y, flipFarmer: false));

            location.warps.Add(new Warp(tilex, tiley + 1, access, (int)exit.X, (int)exit.Y, flipFarmer: false));

        }

    }

}
