/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/skuldomg/freeDusty
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Menus;

namespace freeDusty
{
    public class ModEntry : Mod
    {
        Dusty doggie;
        GameLocation spawnMap;
        String prefix = "";

        public override void Entry(IModHelper helper)
        {
            if(!Context.IsMultiplayer) { 
                helper.Content.AssetLoaders.Add(new DustyLoader(this.Helper));         

                Helper.Events.GameLoop.DayStarted += this.AfterDayStarted;
                Helper.Events.GameLoop.Saving += this.BeforeSave;

                Helper.Events.Display.MenuChanged += this.MenuChanged;
                Helper.Events.Player.Warped += this.Warped;

                //GameEvents.EighthUpdateTick += this.Second;
            }
        }

        // Reload town sprites
        public void Warped(object sender, EventArgs e)
        {
            Helper.Content.InvalidateCache(prefix + "_town");
            Helper.Content.InvalidateCache(@"/Maps/" + prefix + "_town");
        }

        // Prevent Dusty from getting angry when the player digs through trash near him
        public void MenuChanged(object sender, EventArgs e)
        {
            if(Game1.activeClickableMenu is DialogueBox dialogue)
            {                
                //this.Monitor.Log("Dialogue is up with contents " + dialogue.getCurrentString());

                if (dialogue.getCurrentString().Equals("Hey, Stop that! ...Yuck!"))
                { 
                    dialogue.closeDialogue();
                    doggie.isEmoting = false;
                    //this.Monitor.Log("Abort, abort!");                    
                }
            }
        }

        public void Second(object sender, EventArgs e)
        {
            //if(spawnMap != null && spawnMap.characters.Contains(doggie))
                //this.Monitor.Log("Dusty is at " + doggie.Position.X/64 + "/" + doggie.Position.Y/64);
        }

        public void AfterDayStarted(object sender, EventArgs e)
        {
            prefix = "";

            if (Game1.currentSeason.ToLower().Equals("spring") || Game1.currentSeason.ToLower().Equals("summer"))
                prefix = "spring";
            else if (Game1.currentSeason.ToLower().Equals("fall"))
                prefix = "fall";
            else
                prefix = "winter";

            // Determine spawn map
            // If player is married to Alex, make Dusty spawn on the farm or in the farm house
            if (Game1.getCharacterFromName("Alex", true).isMarried())
            {
                // If it's raining or snowing
                if (Game1.isRaining || Game1.isSnowing)
                    spawnMap = Game1.getLocationFromName("FarmHouse");
                else
                    spawnMap = Game1.getLocationFromName("Farm");
            }
            else if(!Game1.isRaining && !Game1.isSnowing)
                spawnMap = Game1.getLocationFromName("Town");

            // Find spawn point and spawn Dusty
            if(Game1.player.IsMainPlayer && spawnMap != null)
            {
                doggie = new Dusty(new AnimatedSprite("Dusty.xnb", 0, 29, 25), this.GetDustySpawn(), 0, "Dusty");                
                spawnMap.addCharacter(doggie);
                Helper.Content.AssetEditors.Add(new BoxEditor(this.Helper, prefix, false));

                //this.Monitor.Log("Added Dusty to the spawn map.");
            }
            // If no spawn map could be determined, Dusty should be in his box
            else if(spawnMap == null)
                Helper.Content.AssetEditors.Add(new BoxEditor(this.Helper, prefix, true));

            if (spawnMap == null)
            {
                //this.Monitor.Log("Did not spawn Dusty today... or did I?");

                foreach(NPC n in Game1.getLocationFromName("Farm").getCharacters()) {
                    //this.Monitor.Log(n.getName());

                    // If for some arcane reason Dusty ends up spawning EVEN THOUGH THERE IS NO WAY HE SHOULD, remove him again
                    if (n.getName().Equals("Dusty"))
                    {
                        Game1.getLocationFromName("Farm").characters.Remove(n);
                        //this.Monitor.Log("Found a stray Dusty and removed him.");
                        break;
                    }
                }
            }
            else
            {
                //this.Monitor.Log("Spawned Dusty at " + spawnMap.Name + " (" + doggie.Position.X / 64 + "/" + doggie.Position.Y / 64 + ")");
                //this.Monitor.Log("Is Dusty at " + spawnMap + "? -> " + spawnMap.characters.Contains(doggie));
            }



            /* // TODO: Figure this out
                /* 
                 * if(spawnMap.Name.Equals("Town")) {
                 * for(int i=51; i<=54; i++)
                 {
                     for(int j=68; j<=70; j++)
                     {
                         spawnMap.setTileProperty(i, j, "Buildings", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Back", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Front", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "AlwaysFront", "Passable", "true");
                         spawnMap.setTileProperty(i, j, "Paths", "Passable", "true");

                         //this.Monitor.Log("Made tile " + i + "/" + j + " walkable");
                     }
                 }
                 }
               */

            Helper.Content.InvalidateCache(prefix + "_town");
            Helper.Content.InvalidateCache(@"/Maps/" + prefix + "_town");
        }

        // Remove Dusty NPC at the end of the day to avoid serialization issues
        public void BeforeSave(object sender, EventArgs e)
        {    
            if (spawnMap != null && doggie != null && doggie.currentLocation.characters.Contains(doggie))
            {
                //this.Monitor.Log("Removing Dusty from " + doggie.currentLocation.Name + " ... is he on that map? " + doggie.currentLocation.characters.Contains(doggie) + " ... he is at " + doggie.currentLocation.ToString());
                //this.Monitor.Log("Removing Dusty to escape evil serialization");
                doggie.currentLocation.characters.Remove(doggie);
            }
        }

        private Vector2 GetDustySpawn()
        {
            //this.Monitor.Log("Finding spawn point...");

            Vector2 spawn = spawnMap.getRandomTile();

            // Must be within suitable areas (roughly at community center or around town, and not within boarded off area above Clint's)
            bool posFound = false;
            while (!posFound)
            {
                //this.Monitor.Log("Checking tile " + spawn.X + "/" + spawn.Y + " on map "+spawnMap+"...");
                                
                if(spawnMap.Name.Equals("Town")) { 
                    // Acceptable areas (in front of community center and town area)
                    if ((spawn.X >= 12 && spawn.X <= 65) && (spawn.Y >= 10 && spawn.Y <= 39))
                        posFound = true;
                    else if ((spawn.X >= 4 && spawn.X <= 110) && (spawn.Y >= 54 && spawn.Y <= 96))
                        posFound = true;

                    // Not in the cordoned off area above Clint's shop
                    if (posFound && ((spawn.X >= 88 && spawn.X <= 106) && (spawn.Y >= 63 && spawn.Y <= 75)))
                        posFound = false;
                }
                else if(spawnMap.Name.Equals("Farm"))
                {
                    // Acceptable area around the farm
                    if ((spawn.X >= 42 && spawn.X <= 73) && (spawn.Y >= 10 && spawn.Y <= 30))
                        posFound = true;                    
                }
                else if(spawnMap.Name.Equals("FarmHouse"))
                {
                    // Take max upgrade and also check for .isonmap
                    if (spawnMap.isTileOnMap(spawn) && ((spawn.X >= 1 && spawn.X <= 40) && (spawn.Y >= 4 && spawn.Y <= 22)))
                        posFound = true;
                }                

                if (!posFound)
                    spawn = spawnMap.getRandomTile();
            }

            //this.Monitor.Log("Found position " + spawn.X + "/" + spawn.Y + " on map " + spawnMap);

            if (!spawnMap.isTileLocationTotallyClearAndPlaceable(spawn))
            {
                //this.Monitor.Log("Spawn location isn't clear, finding nearby clear location...");
                spawn = FindSafePosition(spawn);
            }

            // 70% chance to discard the random position and just spawn him in his pen
            if (Game1.random.Next(1, 10) <= 7)
            {
                spawn.X = 53;
                spawn.Y = 68;
            }           

            //this.Monitor.Log("Seems to be clear for spawn.");

            return spawn * 64f;
        }

        private Vector2 FindSafePosition(Vector2 pos)
        {
            // Find a clear location around the specified position
            GameLocation theLocation = spawnMap;

            // Check the 5 surrounding circles
            for (int i = 1; i <= 5; i++)
            {
                // Check above
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X, (int)pos.Y - i))// (new xTile.Dimensions.Location((int)pos.X, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y - i);

                // Check below
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X, pos.Y + i);

                // Check left
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y);

                // Check right
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y);

                // Check top right
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y - i)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y - i);

                // Check top left
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y - i)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y - i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y - i);

                // Check below right
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X + i, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X + i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X + i, pos.Y + i);

                // Check below left
                if (theLocation.isTileLocationTotallyClearAndPlaceable((int)pos.X - i, (int)pos.Y + i)) //(new xTile.Dimensions.Location((int)pos.X - i, (int)pos.Y + i), Game1.viewport))
                    return new Vector2(pos.X - i, pos.Y + i);
            }
            //this.Monitor.Log("Didn't find a nearby safe position");
            return pos;
        }
    }
}