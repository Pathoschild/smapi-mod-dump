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
        private ModConfig config;

        public static readonly PerScreen<bool> PerScreenStolen = new PerScreen<bool>(createNewState: () => false);

        public static readonly PerScreen<int> PerScreenShopliftCounter = new PerScreen<int>(createNewState: () => 0);

        public static readonly PerScreen<Dictionary<string,int>> PerScreenShopliftedShops = new PerScreen<Dictionary<string, int>>(createNewState: () => new Dictionary<string, int>());

        public static readonly PerScreen<ArrayList> PerScreenShopsBannedFrom = new PerScreen<ArrayList>(createNewState: () => new ArrayList());

        public static readonly string[] shops = { "SeedShop", "FishShop", "AnimalShop", "ScienceHouse", "Hospital", "Blacksmith", "Saloon", "SandyHouse" };
     

        public override void Entry(IModHelper helper)
        {          
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.GameLaunched += this.Launched;
            helper.Events.Input.ButtonPressed += this.Action;
            helper.ConsoleCommands.Add("shoplifter_resetsave", "Removes and readds save data added by the mod to fix broken save data, only use if you're getting errors", this.ResetSave);
            try
            {
                this.config = helper.ReadConfig<ModConfig>();
            }
            catch
            {
                this.config = new ModConfig();
                this.Monitor.Log("Failed to parse config file, default options will be used. Ensure only positive whole numbers are entered in config", LogLevel.Warn);
            }
            
            ShopMenuUtilities.gethelpers(this.Monitor, this.ModManifest, this.config);
            i18n.gethelpers(this.Helper.Translation, this.config);
        }
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset perscreenstolentoday boolean so player can shoplift again when the new day starts
            PerScreenStolen.Value = false;
            PerScreenShopliftCounter.Value = 0;
            PerScreenShopliftedShops.Value.Clear();

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

                string[] values = data[shopliftingdata]?.Split('/') ?? new string[] { };            
                string[] fields = shopliftingdata?.Split('_') ?? new string[] { };

                // Player has finished certain number of days ban, remove shop from list, also reset first day caught
                if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) <= -this.config.DaysBannedFor && values.Length == 2)
                {
                    values[0] = "0";
                    values[1] = "0";

                    PerScreenShopsBannedFrom.Value.Remove(fields[1]);
                    this.Monitor.Log($"You're no longer banned from {fields[1]}, steal away!", LogLevel.Info);
                }

                // Player is currently banned, add shop to list
                else if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) < 0 && values.Length == 2)
                {
                    values[0] = (int.Parse(values[0]) - 1).ToString();
                    PerScreenShopsBannedFrom.Value.Add(fields[1]);
                    this.Monitor.Log($"You're currently banned from {fields[1]}", LogLevel.Info);
                }

                // If 28 days have past and player was not caught a certain number of times, reset both fields
                if (shopliftingdata.StartsWith($"{this.ModManifest.UniqueID}") && int.Parse(values[0]) > 0 && values[1] == Game1.dayOfMonth.ToString() && values.Length == 2)
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
            if (this.config.MaxShopliftsPerStore == 0)
            {
                this.config.MaxShopliftsPerStore = 1;
            }

            if (this.config.MaxShopliftsPerDay == 0)
            {
                this.config.MaxShopliftsPerDay = 1;
            }

            if (this.config.CatchesBeforeBan == 0)
            {
                this.config.CatchesBeforeBan = 1;
            }

            if (this.config.CaughtRadius == 0)
            {
                this.config.CaughtRadius = 1;
            }

            this.BuildConfigMenu();
        }

        private void BuildConfigMenu()
        {
            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null) return;

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.config)
            );

            configMenu.AddSectionTitle(this.ModManifest, () => i18n.string_GMCM_PeriodSection());
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxDay(),
                tooltip: () => i18n.string_GMCM_MaxDayTooltip(),
                getValue: () => (int)this.config.MaxShopliftsPerDay,
                setValue: value => this.config.MaxShopliftsPerDay = (uint)value,
                min: 1
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxShop(),
                tooltip: () => i18n.string_GMCM_MaxShopTooltip(),
                getValue: () => (int)this.config.MaxShopliftsPerStore,
                setValue: value => this.config.MaxShopliftsPerStore = (uint)value,
                min: 1
            );
            configMenu.AddSectionTitle(this.ModManifest, () => i18n.string_GMCM_PenaltySection());
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxFine(),
                tooltip: () => i18n.string_GMCM_MaxFineTooltip(),
                getValue: () => (int)this.config.MaxFine,
                setValue: value => this.config.MaxFine = (uint)value,
                min: 0
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxFriendship(),
                tooltip: () => i18n.string_GMCM_MaxFriendshipTooltip(),
                getValue: () => (int)this.config.FriendshipPenalty,
                setValue: value => this.config.FriendshipPenalty = (uint)value,
                min: 0
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxCatches(),
                tooltip: () => i18n.string_GMCM_MaxCatchesTooltip(),
                getValue: () => (int)this.config.CatchesBeforeBan,
                setValue: value => this.config.CatchesBeforeBan = (uint)value,
                min: 1, max: 100
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxBanned(),
                tooltip: () => i18n.string_GMCM_MaxBannedTooltip(),
                getValue: () => (int)this.config.DaysBannedFor,
                setValue: value => this.config.DaysBannedFor = (uint)value,
                min: 0, max: 28
            );
            configMenu.AddNumberOption(
                mod: this.ModManifest,
                name: () => i18n.string_GMCM_MaxRadius(),
                tooltip: () => i18n.string_GMCM_MaxRadiusTooltip(),
                getValue: () => (int)this.config.CaughtRadius,
                setValue: value => this.config.CaughtRadius = (uint)value,
                min: 0, max: 20
            );
        }

        private void Action(object sender, ButtonPressedEventArgs e)
        {
            GameLocation location = Game1.player.currentLocation;

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

                                Game1.drawObjectDialogue(i18n.string_Banned());
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
                        case "IceCreamStand":
                            ShopMenuUtilities.IceCreamShopliftingMenu(location, tilelocation);
                            break;

                        case "Buy":
                            if (split[1] == "Fish")
                            {
                                ShopMenuUtilities.FishShopShopliftingMenu(location);
                            }
                            else if (location is SeedShop && PerScreenShopliftCounter.Value < config.MaxShopliftsPerDay)
                            {
                                ShopMenuUtilities.SeedShopShopliftingMenu(location);
                            }
                            else if (location.Name.Equals("SandyHouse"))
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
