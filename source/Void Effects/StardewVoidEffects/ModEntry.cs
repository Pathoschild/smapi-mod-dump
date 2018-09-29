using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace StardewVoidEffects
{
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

        public override void Entry(IModHelper helper)
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            if (Config.modEnabledOnStartup)
            {
                modEnabled = true;
            }
            else { modEnabled = false; }
            SpaceEvents.OnItemEaten += this.SpaceEvents_ItemEaten;
            TimeEvents.AfterDayStarted += this.TimeEvents_DayAdvance;
            GameEvents.OneSecondTick += this.Void_Drain;
            MenuEvents.MenuChanged += this.drainMenu_Open;
            MenuEvents.MenuClosed += this.drainMenu_Closed;
            TimeEvents.AfterDayStarted += this.Receive_Mail_From_Wizard;
            helper.ConsoleCommands.Add("void_tolerance", "Checks how many void items you have consumed. (Currently Unused)", this.Void_Tolerance);
            helper.ConsoleCommands.Add("togglevoid", "Turns the void effects on/off", this.Toggle_Mod);
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                return true;
            }

            if (asset.AssetNameEquals("Data/mail"))
            {
                return true;
            }

            return false;
        }

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

        public void Receive_Mail_From_Wizard(object sender, EventArgs args)
        {
            if (modEnabled && Game1.player.mailReceived.Contains("wizardsIntroVoid") && Game1.player.mailReceived.Contains("wizardsInfoOnVoid"))
                return;
            {
            }

            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            int currentYear = Game1.year;
            int currentDay = Game1.dayOfMonth;
            string currentSeason = Game1.currentSeason;
            SDate year2 = new SDate(day: 1, season: "spring", year: 2);
            SDate summerDay1Year1 = new SDate(season: "summer", day: 1, year: 1);

            if (modEnabled && SDate.Now() > summerDay1Year1 && !Game1.player.mailReceived.Contains("wizardsIntroVoid"))
            {
                Game1.mailbox.Add("wizardsIntroVoid");
            }

            if (modEnabled && Game1.player.mailReceived.Contains("wizardsIntroVoid"))
            {
                daysSinceReceivingIntroLetter++;
            }

            if (modEnabled && currentYear >= 2 && !Game1.player.mailReceived.Contains("wizardsInfoOnVoid") && Game1.player.mailReceived.Contains("wizardsIntroVoid") && daysSinceReceivingIntroLetter >= 7)
            {
                Game1.mailbox.Add("wizardsInfoOnVoid");
            }
        }

        private void Toggle_Mod(string command, string[] args)
        {
            modEnabled = !modEnabled;
            if (modEnabled)
            {
                this.Monitor.Log("Void Effects toggled on.");
            }
            else
            {
                this.Monitor.Log("Void Effects toggled off.");
            }
        }

        private void drainMenu_Open(object sender, EventArgsClickableMenuChanged args)
        {
            if (!modEnabled) { return; }
            isMenuOpen = true;
        }

        private void drainMenu_Closed(object sender, EventArgsClickableMenuClosed args)
        {
            if (!modEnabled) { return; }
            isMenuOpen = false;
        }

        private void Void_Drain(object sender, EventArgs args)
        {
            if (!modEnabled) { return; }

            bool voidInInventory = Game1.player.items.Any(item => item?.Name.ToLower().Contains("void") ?? false);
            bool isIridiumRingEquippedL = Game1.player.leftRing.Any(name => name?.Name.Equals("Iridium Band") ?? false);
            bool isIridiumRingEquippedR = Game1.player.rightRing.Any(name => name?.Name.Equals("Iridium Band") ?? false);

            this.Config = this.Helper.ReadConfig<ModConfig>();

            if (recentlyPassedOutInMP == true && isMenuOpen == false)
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

            if (isMenuOpen == false && recentlyPassedOutInMP == false)
            {
                fiveSecondTimer--;
                if (voidInInventory)
                {
                    if (fiveSecondTimer <= 0 && recentlyPassedOutInMP == false)
                    {
                        if (isIridiumRingEquippedL == false && isIridiumRingEquippedR == false)
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
            if (!modEnabled) { return; }

            if (!Context.IsWorldReady)
                return;

            var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
            this.Monitor.Log($"You have consumed {savedData.Tolerance + voidToleranceUntilSleep} void items.");
        }

        private void SpaceEvents_ItemEaten(object sender, EventArgs args)
        {
            if (!modEnabled) { return; }

            if (!Context.IsWorldReady)
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
            if (!modEnabled) { return; }

            if (!Context.IsWorldReady)
                return;

            voidToleranceUntilSleep++;
        }

        private void TimeEvents_DayAdvance(object sender, EventArgs args)
        {
            if (!modEnabled) { return; }

            int noOfPlayers = Game1.getOnlineFarmers().Count<Farmer>();
            if (!Context.IsWorldReady)
            {
                return;
            }

            if (voidToleranceUntilSleep > 0)
            {
                var savedData = this.Helper.ReadJsonFile<SavedData>($"data/{Constants.SaveFolderName}.json") ?? new SavedData();
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

    internal class SavedData
    {
        public float Tolerance { get; set; }
    }

    internal class ModConfig
    {
        public bool modEnabledOnStartup { get; set; } = true;
        public float VoidItemPriceIncrease { get; set; } = 2.0f;
        public int VoidDecay { get; set; } = 10;
    }
}