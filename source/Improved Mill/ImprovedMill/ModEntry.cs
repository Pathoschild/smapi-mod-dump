/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AustinYQM/ImprovedMill
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Buildings;
using System.Linq;

namespace ImprovedMill
{
    public class ModEntry : Mod
    {
        private ModConfig Config;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            InputEvents.ButtonPressed += this.InputEvents_ButtonPressed; // Track Button Presses to force Mill use.
            
            SaveEvents.BeforeSave += this.SaveEvents_BeforeSave; // Do the mill at night.
            this.Config = helper.ReadConfig<ModConfig>(); // Load the config
            if(this.Config != null) { this.Monitor.Log("Config loaded.", LogLevel.Info); }
        }

        /// <summary>Checks to see if the tile is a building.</summary>
        /// <param name="tile">Proves a tile to inspect.</param>
        public Building GetBuildingAt(Vector2 tile)
        {
            if(Game1.currentLocation is BuildableGameLocation location) // Check to see if the player is in a location where a mill could be.
            {
                foreach (Building building in location.buildings) // for each building in the location....
                {
                    Rectangle area = new Rectangle(building.tileX, building.tileY, building.tilesWide, building.tilesHigh); // make a rectangle the size of the building.
                    if (area.Contains((int)tile.X, (int)tile.Y)) // see if that rectangle contains tile.
                        return building; // return the building.
                }
            }
            return null; // return null
        } // See what building we are pointing at.
        //<summary> Delete the object the player is currently holding </summary>
        public void DeleteHeld()
        {
            Game1.player.removeItemFromInventory(Game1.player.CurrentItem);
        } 

        /*********
        ** Private methods
        *********/
        /// <summary>The method invoked when the player presses a controller, keyboard, or mouse button.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            if (!e.IsActionButton && !(e.Button == SButton.ControllerA) && !(e.Button == SButton.ControllerX))
                return;
            Mill mill = this.GetBuildingAt(e.Cursor.GrabTile) as Mill;
            if (mill == null)
                return;

            //if(!e.IsSuppressed)
                e.SuppressButton();
            

            if (Game1.player.CurrentItem is StardewValley.Object currentObj && currentObj.category == -75)
            {
                if (currentObj.parentSheetIndex == 262 && this.Config.ProcessWheat) // Do things for Wheat.
                {
                    Game1.addHUDMessage(new HUDMessage(currentObj.stack + " " + currentObj.displayName + " added to the mill.", 3) { noIcon = true, timeLeft = HUDMessage.defaultTime });
                    Item remaining = mill.input.addItem(currentObj);
                    if (remaining == null)
                        DeleteHeld();
                    //this.Helper.Reflection.GetField<bool>(mill, "hasLoadedToday").SetValue(true);
                    Game1.playSound("Ship");
                }
                //else if (currentObj.parentSheetIndex == 262 && !this.Config.ProcessWheat)
                //{
                //    Game1.showRedMessage(Game1.content.LoadString("Strings\\Buildings:CantMill"));
                //}
                if (currentObj.parentSheetIndex == 270 && this.Config.ProcessCorn) // Do things for Corn.
                {
                   Game1.addHUDMessage(new HUDMessage(currentObj.stack + " " + currentObj.displayName + " added to the mill.", 3) { noIcon = true, timeLeft = HUDMessage.defaultTime });
                    Item remaining = mill.input.addItem(currentObj);
                    if (remaining == null)
                        DeleteHeld();
                    //this.Helper.Reflection.GetField<bool>(mill, "hasLoadedToday").SetValue(true);
                    Game1.playSound("Ship");
                }
                if (currentObj.parentSheetIndex == 300 && this.Config.ProcessAmaranth) // Do things for Amaranth.
                {
                    Game1.addHUDMessage(new HUDMessage(currentObj.stack + " " + currentObj.displayName + " added to the mill.", 3) { noIcon = true, timeLeft = HUDMessage.defaultTime });
                    Item remaining = mill.input.addItem(currentObj);
                    if (remaining == null)
                        DeleteHeld();
                    //this.Helper.Reflection.GetField<bool>(mill, "hasLoadedToday").SetValue(true);
                    Game1.playSound("Ship");
                }
                if (currentObj.parentSheetIndex == 284 && this.Config.ProcessBeet) // Do things for beets.
                {
                    Game1.addHUDMessage(new HUDMessage(currentObj.stack + " " + currentObj.displayName + " added to the mill.", 3) { noIcon = true, timeLeft = HUDMessage.defaultTime });
                    Item remaining = mill.input.addItem(currentObj);
                    if(remaining == null)
                        DeleteHeld();
                    //this.Helper.Reflection.GetField<bool>(mill, "hasLoadedToday").SetValue(true);
                    Game1.playSound("Ship");
                }
             
            }
            


            //if (Game1.player.CurrentItem is StardewValley.Object currentObj && currentObj.parentSheetIndex == 262 && currentObj.category == -75) { }// wheat
        }
        private void SaveEvents_BeforeSave(object sender, EventArgs e)
        {

            foreach (BuildableGameLocation location in Game1.locations.OfType<BuildableGameLocation>())
            {
                foreach (Mill mill in location.buildings.OfType<Mill>())
                {
                    if (mill.daysOfConstructionLeft > 0)
                        continue;

                    float sugarAdd = 0, flourAdd = 0;

                    foreach(StardewValley.Object item in mill.input.items)
                    {
                        if (item == null || item.category != -75)
                          continue;

                        switch(item.parentSheetIndex)
                        {
                            case 262: // Wheat
                                if (this.Config.ProcessWheat)
                                {
                                    sugarAdd += this.Config.SugarForWheat * item.stack;
                                    flourAdd += this.Config.FlourForWheat * item.stack;
                                    this.Monitor.Log(item.displayName + "(x" + item.stack + ") processed.", LogLevel.Info);
                                }
                                else // do SDV Default
                                {
                                    sugarAdd += 0 * item.stack;
                                    flourAdd += 1 * item.stack;
                                    this.Monitor.Log(item.displayName + "(x" + item.stack + ") processed.", LogLevel.Info);
                                }
                                break;
                            case 270: // Corn
                                if (this.Config.ProcessCorn)
                                {
                                    sugarAdd += this.Config.SugarForCorn * item.stack;
                                    flourAdd += this.Config.FlourForCorn * item.stack;
                                    this.Monitor.Log(item.displayName + "(x" + item.stack + ") processed.", LogLevel.Info);
                                }
                                break;
                            case 284: // Beets
                                if (this.Config.ProcessBeet)
                                {
                                    sugarAdd += this.Config.SugarForBeet * item.stack;
                                    flourAdd += this.Config.FlourForBeet * item.stack;
                                    this.Monitor.Log(item.displayName + "(x" + item.stack + ") processed.", LogLevel.Info);
                                }
                                else // do SDV default
                                {
                                    sugarAdd += 3 * item.stack;
                                    flourAdd += 0 * item.stack;
                                    this.Monitor.Log(item.displayName + "(x" + item.stack + ") processed.", LogLevel.Info);
                                }
                                break;
                            case 300: // Amaranth
                                if (this.Config.ProcessAmaranth)
                                {
                                    sugarAdd += this.Config.SugarForAmaranth * item.stack;
                                    flourAdd += this.Config.FlourForAmaranth * item.stack;
                                    this.Monitor.Log(item.displayName + "(x" + item.stack + ") processed.", LogLevel.Info);
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    mill.input.items.Clear();
                    while(sugarAdd > 0)
                    {
                        if (sugarAdd >= 999)
                        {
                            mill.output.items.Add(new StardewValley.Object(245, 999));
                            sugarAdd -= 999 ;
                        }
                        else
                        {
                            mill.output.items.Add(new StardewValley.Object(245, (int)sugarAdd));
                            sugarAdd = 0;
                        }
                    }
                    while (flourAdd > 0)
                    {
                        if (flourAdd >= 999)
                        {
                            mill.output.items.Add(new StardewValley.Object(246, 999));
                            flourAdd -= 999;
                        }
                        else
                        {
                            mill.output.items.Add(new StardewValley.Object(246, (int)flourAdd));
                            flourAdd = 0;
                        }
                    }

                }
            }

        }
        
    }
}
