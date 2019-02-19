using System;
using System.Collections.Generic;
using System.Linq;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewVoidEffects.Framework;

namespace StardewVoidEffects
{
    /// <summary>The mod entry class called by SMAPI.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        private bool hasEatenVoid;
        private bool isMenuOpen;
        private bool modEnabled;
        private ModConfig Config;
        private int fiveSecondTimer = 5;
        private bool recentlyPassedOutInMP = false;
        private int passedOutMPtimer = 60;
        private float voidToleranceUntilSleep;
        private int daysSinceReceivingIntroLetter;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.modEnabled = Config.modEnabledOnStartup;

            SpaceEvents.OnItemEaten += this.SpaceEvents_ItemEaten;
            helper.Events.GameLoop.DayStarted += this.OnDayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicked += this.OnOneSecondUpdateTicked;
            helper.Events.Display.MenuChanged += this.OnMenuChanged;

            helper.ConsoleCommands.Add("void_tolerance", "Checks how many void items you have consumed. (Currently Unused)", this.Void_Tolerance);
            helper.ConsoleCommands.Add("togglevoid", "Turns the void effects on/off", this.Toggle_Mod);
        }

        /// <summary>Get whether this instance can edit the given asset.</summary>
        /// <param name="asset">Basic metadata about the asset being loaded.</param>
        public bool CanEdit<T>(IAssetInfo asset)
        {
            return
                asset.AssetNameEquals("Data/ObjectInformation")
                || asset.AssetNameEquals("Data/mail");
        }

        /// <summary>Edit a matched asset.</summary>
        /// <param name="asset">A helper which encapsulates metadata about an asset and enables changes to it.</param>
        public void Edit<T>(IAssetData asset)
        {
            int[] validItems = { 305, 308, 174, 176, 180, 182, 184, 186, 442, 436, 438, 440, 444, 446, 306, 307, 424, 426, 428, 769, 795 };
            int[] validItemsVanilla = { 305, 308, 769, 795 };
            bool isVoidRanchLoaded = this.Helper.ModRegistry.IsLoaded("Taelende.VoidRanch");
            this.Config = this.Helper.ReadConfig<ModConfig>();
            float priceIncrease = this.Config.VoidItemPriceIncrease;

            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                if (isVoidRanchLoaded && modEnabled)
                {
                    IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                    foreach (int id in validItems)
                    {
                        if (data.TryGetValue(id, out string entry))
                        {
                            string[] fields = entry.Split('/');
                            int currentPrice = int.Parse(fields[1]);
                            fields[1] = (currentPrice * priceIncrease).ToString();
                            data[id] = string.Join("/", fields);
                        }
                    }
                }
                else
                {
                    if (modEnabled)
                    {
                        IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                        foreach (int id in validItemsVanilla)
                        {
                            if (data.TryGetValue(id, out string entry))
                            {
                                string[] fields = entry.Split('/');
                                int currentPrice = int.Parse(fields[1]);
                                fields[1] = (currentPrice * priceIncrease).ToString();
                                data[id] = string.Join("/", fields);
                            }
                        }
                    }
                }
            }

            if (asset.AssetNameEquals("Data/mail"))
            {
                asset.AsDictionary<string, string>().Data.Add("wizardsInfoOnVoid", "@,^I have been researching strange phenomenon relating to items of dark origin.^It seems to be that if you combine the forces of light and dark into a physical form, you can shield yourself from the aura that is emitted from said dark items!^Yes! I am talking about the Iridium Band!^Seek one of these out and please confirm that my hypothesis is correct.^ -M. Rasmodius, Wizard");
                asset.AsDictionary<string, string>().Data.Add("wizardsIntroVoid", "Greetings young adept.^I have detected some kind of dark aura radiating off items of dark origin and it appears to be draining the willpower of my test subjects.^The item that I have tested this with is Void Essence however, I believe that all items of the same origin have the same effects.^Please stay away from these items until I have conducted more research.^ -M. Rasmodius, Wizard");
            }
        }

        private void Toggle_Mod(string command, string[] args)
        {
            modEnabled = !modEnabled;
            this.Monitor.Log($"Void Effects toggled {(modEnabled ? "on" : "off")}.");
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (!modEnabled)
                return;

            isMenuOpen = e.NewMenu != null;
        }

        /// <summary>Raised once per second after the game state is updated.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnOneSecondUpdateTicked(object sender, EventArgs e)
        {
            if (!modEnabled || !Context.IsWorldReady)
                return;

            // apply void drain
            bool voidInInventory = Game1.player.Items.Any(item => item?.Name.ToLower().Contains("void") ?? false);
            bool isIridiumRingEquippedL = Game1.player.leftRing.Any(name => name?.Name.Equals("Iridium Band") ?? false);
            bool isIridiumRingEquippedR = Game1.player.rightRing.Any(name => name?.Name.Equals("Iridium Band") ?? false);

            this.Config = this.Helper.ReadConfig<ModConfig>();

            if (recentlyPassedOutInMP && !isMenuOpen)
            {
                passedOutMPtimer--;
                if (passedOutMPtimer <= 0)
                {
                    recentlyPassedOutInMP = false;
                    passedOutMPtimer = 60;
                }
            }

            if (Game1.player.stamina == 0 && Game1.IsMultiplayer)
            {
                recentlyPassedOutInMP = true;
            }

            if (!isMenuOpen && !recentlyPassedOutInMP)
            {
                fiveSecondTimer--;
                if (voidInInventory)
                {
                    if (fiveSecondTimer <= 0 && !recentlyPassedOutInMP)
                    {
                        if (!isIridiumRingEquippedL && !isIridiumRingEquippedR)
                        {
                            int voidDecay = Config.VoidDecay;
                            int decayedHealth = Game1.player.health - (voidDecay / 2);
                            float decayedStamina = Game1.player.stamina - voidDecay;
                            Game1.player.health = decayedHealth;
                            Game1.player.stamina = decayedStamina;
                            fiveSecondTimer = 5;
                            voidToleranceUntilSleep = voidToleranceUntilSleep + 0.1f;
                        }
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void Void_Tolerance(string command, string[] args)
        {
            if (!modEnabled || !Context.IsWorldReady)
                return;

            SavedData savedData = this.Helper.Data.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            this.Monitor.Log($"You have consumed {savedData.Tolerance + voidToleranceUntilSleep} void items.");
        }

        private void SpaceEvents_ItemEaten(object sender, EventArgs args)
        {
            if (!modEnabled || !Context.IsWorldReady)
                return;

            //this.Monitor.Log($"{Game1.player.Name} has eaten a {Game1.player.itemToEat.Name}");
            string foodJustEaten = Game1.player.itemToEat.Name;

            if (foodJustEaten.ToLower().Contains("void"))
            {
                //this.Monitor.Log($"{Game1.player.Name} just ate a Void item!");
                if (Context.IsMultiplayer)
                {
                    Increase_Tolerance();
                    if (Game1.player.stamina > 10)
                    {
                        Game1.player.stamina = 10;
                    }
                    hasEatenVoid = false;
                }
                else
                {
                    Increase_Tolerance();
                    Game1.player.stamina = 0;
                    hasEatenVoid = true;
                }
            }
        }

        private void Increase_Tolerance()
        {
            if (!modEnabled || !Context.IsWorldReady)
                return;

            voidToleranceUntilSleep++;
        }

        /// <summary>Raised after the game begins a new day (including when the player loads a save).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (!modEnabled || !Context.IsWorldReady)
                return;

            // receive mail from Wizard
            if (!Game1.player.mailReceived.Contains("wizardsIntroVoid") || !Game1.player.mailReceived.Contains("wizardsInfoOnVoid"))
            {
                if (SDate.Now() > new SDate(season: "summer", day: 1, year: 1) && !Game1.player.mailReceived.Contains("wizardsIntroVoid"))
                    Game1.mailbox.Add("wizardsIntroVoid");

                if (Game1.player.mailReceived.Contains("wizardsIntroVoid"))
                    daysSinceReceivingIntroLetter++;

                if (Game1.year >= 2 && !Game1.player.mailReceived.Contains("wizardsInfoOnVoid") && Game1.player.mailReceived.Contains("wizardsIntroVoid") && daysSinceReceivingIntroLetter >= 7)
                    Game1.mailbox.Add("wizardsInfoOnVoid");
            }

            // update void effects
            int noOfPlayers = Game1.getOnlineFarmers().Count();
            if (voidToleranceUntilSleep > 0)
            {
                var savedData = this.Helper.Data.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
                savedData.Tolerance = savedData.Tolerance + voidToleranceUntilSleep;
                voidToleranceUntilSleep = 0;
            }
            if (hasEatenVoid && !Game1.IsMultiplayer)
            {
                this.Monitor.Log($"There are currently {noOfPlayers} players in game. (The number should be 1, if not send halp)");
                Random rnd = new Random();
                int daysToPass = rnd.Next(1, 4);
                Game1.dayOfMonth = (Game1.dayOfMonth + daysToPass);
                hasEatenVoid = false;
            }
            else if (hasEatenVoid && Game1.IsMultiplayer)
            {
                this.Monitor.Log($"There are currently {noOfPlayers} number of players in your game. \nIf you receive this and you're in singleplayer, something went wrong.");
                hasEatenVoid = false;
            }
        }
    }
}