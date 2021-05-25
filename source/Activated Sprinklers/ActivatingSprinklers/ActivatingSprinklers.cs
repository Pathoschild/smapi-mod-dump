/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JudeRV/Stardew-ActivatingSprinklers
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using StardewValley;
using StardewModdingAPI;
using StardewValley.Tools;

namespace ActivatingSprinklers
{
    public class ActivatingSprinklers : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }
        private void OnUpdateTicked(object sender, EventArgs e)
        {
            if (Game1.currentLocation == null) return;
            DoAction();
        }
        private void DoAction()
        {
            if (Helper.Input.GetState(SButton.MouseRight) == SButtonState.Pressed)
            {
                Vector2 tile = Game1.currentCursorTile;
                if (Game1.currentLocation.objects.TryGetValue(tile, out StardewValley.Object check))
                {
                    if (check.name.ToLower().Contains("sprinkler"))
                    {
                        //Defines radius & checks for pressure nozzle
                        int sprinklerRadius = 0;
                        if (check.heldObject.Value != null && Utility.IsNormalObjectAtParentSheetIndex(check.heldObject, 915)) sprinklerRadius++;
                        //Determines sprinkler type, adjusts radius accordingly
                        if (check.name.ToLower().Contains("iridium")) sprinklerRadius += 2;
                        else if (check.name.ToLower().Contains("quality")) sprinklerRadius++;
                        //Creates area that needs to be watered
                        List<Vector2> tileNeedWater = MakeVector2TileGrid(tile, sprinklerRadius);
                        //Waters the area specified
                        WateringCan waterCan = new WateringCan
                        {
                            WaterLeft = 100
                        };
                        float stamina = Game1.player.Stamina;
                        foreach (Vector2 waterTile in tileNeedWater)
                        {
                            waterCan.DoFunction(Game1.currentLocation, (int)(waterTile.X * Game1.tileSize), (int)(waterTile.Y * Game1.tileSize), 1, Game1.player);
                            waterCan.WaterLeft++;
                            Game1.player.Stamina = stamina;
                        }
                    }
                }
            }
        }
        //Gets a list of all tiles needed to be watered
        static List<Vector2> MakeVector2TileGrid(Vector2 origin, int size)
        {
            List<Vector2> grid = new List<Vector2>();
            //If it's a normal sprinkler, just water the four adjacent tiles around it
            if (size == 0)
            {
                grid.Add(new Vector2(origin.X + 1, origin.Y));
                grid.Add(new Vector2(origin.X - 1, origin.Y));
                grid.Add(new Vector2(origin.X, origin.Y + 1));
                grid.Add(new Vector2(origin.X, origin.Y - 1));
                return grid;
            }
            //Otherwise, create a square of tiles centered around selected tile to water
            for (int i = 0; i < 2 * size + 1; i++)
            {
                for (int j = 0; j < 2 * size + 1; j++)
                {
                    Vector2 newVec = new Vector2(origin.X - size, origin.Y - size);
                    newVec.X += (float)i;
                    newVec.Y += (float)j;
                    //Don't water the tile that the sprinkler's on
                    if (newVec.X == origin.X && newVec.Y == origin.Y) continue;
                    else grid.Add(newVec);
                }
            }
            return grid;
        }
    }
}