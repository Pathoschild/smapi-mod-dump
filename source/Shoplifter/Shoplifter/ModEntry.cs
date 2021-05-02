/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TheMightyAmondee/Shoplifter
**
*************************************************/

using System.Collections.Generic;
using System.Collections;
using StardewModdingAPI;
using StardewValley;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley.Locations;
using xTile.Dimensions;

namespace Shoplifter
{
    public class ModEntry
        : Mod
    {
        public static readonly PerScreen<bool> PerScreenStolenToday = new PerScreen<bool>(createNewState: () => false);

        public static Dictionary<string, string> shopliftingstrings = new Dictionary<string, string>();

        public static readonly PerScreen<ArrayList> PerScreenShopsBannedFrom = new PerScreen<ArrayList>(createNewState: () => new ArrayList());

        public static string[] shops = { "SeedShop", "FishShop", "AnimalShop", "ScienceHouse", "Hospital", "Blacksmith", "Saloon", "SandyHouse" };

        public override void Entry(IModHelper helper)
        {
            ShopMenuUtilities.gethelpers(this.Monitor, this.ModManifest);
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.Launched;
            helper.Events.Input.ButtonPressed += this.Action;
            helper.ConsoleCommands.Add("shoplifter_resetsave", "Removes and readds save data added by the mod to fix broken save data, only use if you're getting errors", this.ResetSave);
        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset perscreenstolentoday boolean so player can shoplift again when the new day starts
            PerScreenStolenToday.Value = false;

            // Clear banned shops
            if (PerScreenShopsBannedFrom.Value.Count > 0)
            {
                PerScreenShopsBannedFrom.Value.Clear();
            }

            var data = Game1.player.modData;

            // Add mod data if it isn't present
            foreach (string shop in shops)
            {
                if (data.ContainsKey($"{this.ModManifest.UniqueID}_{shop}") == false)
                {
                    data.Add($"{this.ModManifest.UniqueID}_{shop}", "0/0");
                    this.Monitor.Log($"Adding mod data... {this.ModManifest.UniqueID}_{shop}");
                }
                
            }

            foreach (string shopliftingdata in new List<string>(data.Keys))
            {

                string[] values = data[shopliftingdata].Split('/');
                string[] fields = shopliftingdata.Split('_');

                // Player has finished three day ban, remove shop from list, also reset first day caught
                if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && values[0] == "-3")
                {
                    values[0] = "0";
                    values[1] = "0";

                    PerScreenShopsBannedFrom.Value.Remove(fields[1]);
                    this.Monitor.Log($"You're no longer banned from {fields[1]}, steal away!", LogLevel.Info);
                }

                // Player is currently banned, add shop to list
                else if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) < 0)
                {
                    values[0] = (int.Parse(values[0]) - 1).ToString();
                    PerScreenShopsBannedFrom.Value.Add(fields[1]);
                    this.Monitor.Log($"You're currently banned from {fields[1]}", LogLevel.Info);
                }

                // If 28 days have past and player was not caught 3 times, reset both fields
                if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) > 0 && values[1] == Game1.dayOfMonth.ToString())
                {
                    values[0] = "0";
                    values[1] = "0";

                    this.Monitor.Log($"It's been 28 days since you first shoplifted {fields[1]}, they've forgotten about it now...", LogLevel.Info);
                }

                // After manipulation, join fields back together with "/" seperator
                data[shopliftingdata] = string.Join("/", values);
            }

        }

        private void Launched(object sender, GameLaunchedEventArgs e)
        {
            // Add placeholder for missing strings
            shopliftingstrings.Add("Placeholder", "There's a string missing here...");

            try
            {
                // Get strings from assets folder and add them to a new dictionary
                Dictionary<string, string> strings = this.Helper.Content.Load<Dictionary<string, string>>("assets\\Strings.json", ContentSource.ModFolder);

                if (strings != null)
                {
                    foreach (string key in new List<string>(strings.Keys))
                    {
                        shopliftingstrings.Add(key, strings[key]);
                    }
                }

                this.Monitor.Log("Strings loaded from assets, ready to go!");

                if (shopliftingstrings.Count < 20)
                {
                    this.Monitor.Log("The number of strings loaded seem a bit low, you may get some missing string problems...\nCheck that all strings are present in the Strings.json", LogLevel.Warn);
                }
            }
            catch
            {             
                this.Monitor.Log("Could not load strings... This will result in missing string problems, (Are you missing the Strings.json file?)", LogLevel.Error);
            }
           
        }

        private void Action(object sender, ButtonPressedEventArgs e)
        {
            GameLocation location = Game1.currentLocation;

            if ((e.Button.IsActionButton() == true || e.Button == SButton.ControllerA) && Game1.dialogueUp == false && Context.CanPlayerMove == true && Context.IsWorldReady == true)
            {
                var TileX = e.Cursor.GrabTile.X;
                var TileY = e.Cursor.GrabTile.Y;

                // If using a controller, don't use cursor position if not facing wrong direction, check player is one tile under (Y - 1) tile with property
                if (e.Button == SButton.ControllerA && Game1.player.FacingDirection != 2)
                { 
                    TileX = Game1.player.getTileX();
                    TileY = Game1.player.getTileY() - 1;
                }

                Location tilelocation = new Location((int)TileX, (int)TileY);

                // Get whether tile has action property and its' parameters
                string[] split = location.doesTileHavePropertyNoNull((int)TileX, (int)TileY, "Action", "Buildings").Split(' ');

                // Tile has desired property
                if (split != null)
                {
                    switch (split[0])
                    {
                        // If the door is a locked warp, check player can enter
                        case "LockedDoorWarp":
                            // Player is banned from location they would warp to otherwise
                            if (PerScreenShopsBannedFrom.Value.Contains($"{split[3]}"))
                            {
                                // Supress button so game doesn't warp player (they're banned)
                                Helper.Input.Suppress(e.Button);

                                // Show string to say player is banned if it is available, else a placeholder
                                if (shopliftingstrings.ContainsKey("TheMightyAmondee.Shoplifter/Banned") == true)
                                {
                                    Game1.drawObjectDialogue(shopliftingstrings["TheMightyAmondee.Shoplifter/Banned"]);
                                }

                                else
                                {
                                    Game1.drawObjectDialogue(shopliftingstrings["Placeholder"]);
                                }
                            }
                            break;
                        // For each action that would open a shop that can be shoplifted, check if it can be shoplifted and take appropriate action
                        case "HospitalShop":
                            ShopMenuUtilities.HospitalShopliftingMenu(location, Game1.player);
                            break;

                        case "Carpenter":
                            ShopMenuUtilities.CarpenterShopliftingMenu(location, Game1.player, tilelocation);
                            break;

                        case "AnimalShop":
                            ShopMenuUtilities.AnimalShopShopliftingMenu(location, Game1.player, tilelocation);
                            break;

                        case "Blacksmith":
                            ShopMenuUtilities.BlacksmithShopliftingMenu(location, tilelocation);
                            break;

                        case "Saloon":
                            ShopMenuUtilities.SaloonShopliftingMenu(location, tilelocation);
                            break;

                        case "Buy":
                            if (split[1] == "Fish")
                            {
                                ShopMenuUtilities.FishShopShopliftingMenu(location);
                            }
                            else if (location is SeedShop)
                            {
                                ShopMenuUtilities.SeedShopShopliftingMenu(location);
                            }
                            else if (location.name.Equals("SandyHouse"))
                            {
                                ShopMenuUtilities.SandyShopShopliftingMenu(location);
                            }
                            break;
                    }
                }               
            }
        }

        private void ResetSave (string command, string[] arg)
        {
            try
            {
                var data = Game1.player.modData;

                foreach (string moddata in new List<string>(data.Keys))
                {
                    if (moddata.StartsWith($"{this.ModManifest.UniqueID}"))
                    {
                        data.Remove(moddata);
                        data.Add(moddata,"0/0");
                    }
                }

                this.Monitor.Log("Reset shoplifting data... If this didn't fix the error on a new day, please report the error to the mod page", LogLevel.Info);
            }
            catch
            {
                this.Monitor.Log("Unable to execute command, check formatting is correct", LogLevel.Error);
            }
        }
    }
}
