using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Tools;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using StardewValley.Menus;
using Microsoft.Xna.Framework.Graphics;

namespace Fischievements
{
    internal class ModConfig
    {
        public SButton DisplayFishMessage { get; set; } = SButton.RightShift;
    }

    public class ModEntry : Mod, IAssetEditor
    {
        private ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.SaveLoaded += this.GameIsLoaded;
            helper.Events.Player.InventoryChanged += OnInventoryChange;
            Config = this.Helper.ReadConfig<ModConfig>();
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }

        /* ========== FISH COUNT ========== */

        public int NormalFishCount()
        {
            int uniqueFish = 0;
            foreach (var kvp in (IEnumerable<KeyValuePair<int, string>>)Game1.objectInformation)
            {
                if (!kvp.Value.Split('/')[3].Contains("Fish")) continue;
                var blacklist = new string[] { "Green Algae", "White Algae", "Seaweed", "Glacierfish", "Legend", "Crimsonfish", "Angler", "Mutant Carp" };
                if (blacklist.Contains(kvp.Value.Split('/')[0])) continue;
                if (!Game1.player.fishCaught.ContainsKey(kvp.Key)) continue;
                uniqueFish++;
            }
            return uniqueFish;
        }
        
        public int NormalFishTotal() // Grab fish count from the fish data
        {
            Dictionary<int, string> fishData = Helper.Content.Load<Dictionary<int, string>>("Data/Fish", ContentSource.GameContent);
            HashSet<int> notNormal = new HashSet<int> { 152, 153, 157, 160, 159, 775, 163, 682 };
            int NormalFish = fishData.Where(kv => !notNormal.Contains(kv.Key)).Count();
            return NormalFish;
        }

        public int LegendFishCount()
        {
            int uniqueLegendFish = 0;
            foreach (var kvp in (IEnumerable<KeyValuePair<int, string>>)Game1.objectInformation)
            {
                if (!kvp.Value.Split('/')[3].Contains("Fish")) continue;
                var whitelist = new string[] { "Glacierfish", "Legend", "Crimsonfish", "Angler", "Mutant Carp" };
                if (!whitelist.Contains(kvp.Value.Split('/')[0])) continue;
                if (!Game1.player.fishCaught.ContainsKey(kvp.Key)) continue;
                uniqueLegendFish++;
            }
            return uniqueLegendFish;
        }

        public int LegendFishTotal()
        {
            Dictionary<int, string> legfishData = Helper.Content.Load<Dictionary<int, string>>("Data/Fish", ContentSource.GameContent);  // Load fish as a dictionary
            HashSet<int> legendFish = new HashSet<int> { 160, 159, 775, 163, 682 };  // Create a set of all legend fish from indexes
            int LegendFishTotals = legfishData.Where(kv => legendFish.Contains(kv.Key)).Count(); // Grab only legend fish
            return LegendFishTotals;
        }

        /* ========== INVENTORY CHANGED ========== */

        private void OnInventoryChange(object sender, InventoryChangedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            ModData moddData = new ModData();

            int RandomNumber(int min, int max)
            {
                Random random = new Random();
                return random.Next(min, max);
            }

            if (NormalFishCount() >= 55)
            {
                if (moddData.PlayerGotMF) { }
                else
                {
                    Game1.getAchievement(35);
                    if (!Game1.player.hasOrWillReceiveMail("F_MF"))
                    {
                        Game1.addMailForTomorrow("F_MF", false, false);
                    }
                }
            }

            if (LegendFishCount() >= 5)
            {
                if (moddData.PlayerGotLF) { }
                else
                {
                    Game1.getAchievement(36);
                    if (!Game1.player.hasOrWillReceiveMail("F_LF"))
                    {
                        Game1.addMailForTomorrow("F_LF", false, false);
                    }
                }
            }

            if (NormalFishCount() >= 55 && LegendFishCount() >= 5)
            {
                if (moddData.PlayerGotLA) { }
                else
                {
                    Game1.getAchievement(37);
                    if (!Game1.player.hasOrWillReceiveMail("F_LA"))
                    {
                        Game1.addMailForTomorrow("F_LA", false, false);
                    }
                }
            }
        }

        /* ========== ADD FISHING ACHIEVEMENTS ========== */

        private void GameIsLoaded(object sender, EventArgs e)
        {
            ModData modData = new ModData();

            if (modData.CheckAchieveMF)
            {
                // Achievement is there already, do nothing
            }
            else
            {
                // Add the achievement
                Game1.achievements.Add(35,"Master Fisher^You have caught every normal fish type!^true^-1^4");
            }

            if (modData.CheckAchieveLF)
            {
                // Achievement is there already, do nothing
            }
            else
            {
                // Add the achievement
                Game1.achievements.Add(36,"Legendary Fisher^You have caught every legendary fish type!^true^-1^4");
            }

            if (modData.CheckAchieveLA)
            {
                // Achievement is there already, do nothing
            }
            else
            {
                // Add the achievement
                Game1.achievements.Add(37,"Legendary Angler^You are known as a legend amongst fishers!^true^-1^4");
            }
        }

        /* ========== EDITING FILES ========== */

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Data/Achievements") || asset.AssetNameEquals("Data/Fish") || asset.AssetNameEquals("Data/ObjectInformation") || asset.AssetNameEquals("Data/Achievements") || asset.AssetNameEquals("Data/Mail") || asset.AssetNameEquals("Data/Locations") || asset.AssetNameEquals("Data/BigCraftablesInformation") || asset.AssetNameEquals("TileSheets/Craftables");
        }

        public void Edit<T>(IAssetData asset)
        {
            // Make the clam a fish

            if (asset.AssetNameEquals("Data/Fish"))
            {
                IDictionary<Int32, string> data = asset.AsDictionary<Int32, string>().Data;
                data[372] = "Clam/15/smooth/5/8/600 2200/summer/sunny/690 .9/1/.4/.3/0";
            }

            if (asset.AssetNameEquals("Data/ObjectInformation"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                data[372] = "Clam/50/-300/Fish -4/Clam/Now I'm a fish!";
            }

            // Add the mail for each achievement

            if (asset.AssetNameEquals("Data/Mail"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                data["F_MF"] = "Dear @,^You appear to have caught every normal fish type!^^You are a true legendary fisher!       ^   -Willy";
                data["F_LF"] = "Dear @,^You appear to have caught every legendary fish type!^^You are a true legendary fisher!       ^   -Willy";
                data["F_LA"] = "Dear @,^You appear to have emptied the ocean of all fish^^Although your skills as an angler remain crap^Here's a little something^^ -Willy   ^  %item bigobject 167 %%";
            }

            // Add the new achievements

            if (asset.AssetNameEquals("Data/Achievements"))
            {
                IDictionary<Int32, string> data = asset.AsDictionary<Int32, string>().Data;
                data[35] = "Master Fisher^You've caught every normal fish type there is!^true^-1^17";
                data[36] = "Legend Fisher^You've caught every legendary fish type there is!^true^-1^24";
                data[37] = "Legendary Angler^You're a legend amongst fisherman!^true^-1^36";
            }

            // Add clams into the beach fishing location

            if (asset.AssetNameEquals("Data/Locations"))
            {
                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;

                string[] fishes = data["Beach"].Split('/');
                var fishspec1 = new List<string>(fishes[0].Split(' ')); // Foragables - Spring
                var fishspec2 = new List<string>(fishes[1].Split(' ')); // Foragables - Summer
                var fishspec3 = new List<string>(fishes[2].Split(' ')); // Foragables - Fall
                var fishspec4 = new List<string>(fishes[3].Split(' ')); // Foragables - Winter
                var fishspec5 = new List<string>(fishes[5].Split(' ')); // Fishing - Summer
                fishspec1.Remove("372"); fishspec1.Remove(".9"); // Remove clams from foragables
                fishspec2.Remove("372"); fishspec2.Remove(".9");
                fishspec3.Remove("372"); fishspec3.Remove(".9");
                fishspec4.Remove("372"); fishspec4.Remove(".4");
                fishspec5.Add("372"); fishspec5.Add("-1"); // Add the clam to summer fishing
                fishes[0] = string.Join(" ", fishspec1.ToArray());
                fishes[1] = string.Join(" ", fishspec2.ToArray());
                fishes[2] = string.Join(" ", fishspec3.ToArray());
                fishes[3] = string.Join(" ", fishspec4.ToArray());
                fishes[5] = string.Join(" ", fishspec5.ToArray());
                data["Beach"] = string.Join("/", fishes);
            }

            // Add the trophy

            if (asset.AssetNameEquals("Data/BigCraftablesInformation"))
            {
                IDictionary<Int32, string> data = asset.AsDictionary<Int32, string>().Data;
                data[167] = "Fishing Trophy/0/-300/Crafting -9/You caught all the fish!/true/true/0";
            }

            if (asset.AssetNameEquals("TileSheets/Craftables"))
            {
                Texture2D customTexture = this.Helper.Content.Load<Texture2D>("assets/trophy.png", ContentSource.ModFolder);
                asset
                    .AsImage()
                    .PatchImage(customTexture, targetArea: new Rectangle(112, 640, 16, 32));
            }
        }

        /* ========== BUTTON EVENTS ========== */

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.IsDown(Config.DisplayFishMessage))
            {
                string NormalFishText = "";
                string LegendFishText = "";

                // Normal fish
                if (NormalFishCount() != 0) { NormalFishText = $"Caught {NormalFishCount().ToString()}/{NormalFishTotal().ToString()} Normal Fish!"; }
                if (NormalFishCount() == 0) { NormalFishText = $"No Normal Fish Caught *"; }
                if (NormalFishCount() >= 55) { NormalFishText = $"Caught all {NormalFishTotal().ToString()} Normal Fish!"; }

                // Legendary fish
                if (LegendFishCount() != 0) { LegendFishText = $"Caught {LegendFishCount().ToString()} Legendary Fish!"; }
                if (LegendFishCount() >= 5) { LegendFishText = $"Caught all {LegendFishCount().ToString()} Legendary Fish! *"; }
                if (LegendFishCount() >= 5 && NormalFishCount() >= 55) { NormalFishText = $"You have caught every fish there is...you legend! *"; }

                Game1.showGlobalMessage($"{NormalFishText}");
                if (LegendFishCount() != 0)
                {
                    Game1.showGlobalMessage($"{LegendFishText}"); // If no legendary fish are caught, don't show
                }
            }
        }
    }
}
