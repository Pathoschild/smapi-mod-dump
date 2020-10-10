/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/millerscout/StardewMillerMods
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RetroactiveStardew
{
    public class RetroAchievements
    {
        private IModHelper helper;
        private IMonitor Monitor;
        private ModConfig Config { get; }

        public RetroAchievements(IModHelper helper, IMonitor monitor, ModConfig Config)
        {
            this.helper = helper;
            this.Monitor = monitor;
            this.Config = Config;
        }

        public void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!this.Config.AchievementsCheck) return;

            this.CheckForAdventureAchievements();
            this.CheckForCookingAchievements();
            this.CheckForMoneyEarned();
            this.CheckForMuseumAchievement();
            this.CheckForFriendshipAchievements();
            this.CheckForHouseUpgradesAchievements();
            this.CheckForQuestAchievements();
            this.CheckForShippingAchievements();
            this.CheckForShippedEveryItemAchievement();
            this.CheckForFishingAchievements();
            this.CheckForCraftedRecipesAchievements();
            this.CheckForCommunityCenterAchievement();
            this.CheckForJojaAchievement();
            this.CheckForStardropAchievement();
            this.CheckForFullHouseAchievement();
            this.CheckForReachedBottomMine();
            this.CheckForSkillsAchievements();
        }

        private void CheckForMoneyEarned()
        {
            if (Game1.player.totalMoneyEarned >= 10000000)
            {
                this.Monitor.Log($"[Achievement] 10mil Money earned unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(4) && !this.Config.OnlyLog)
                    Game1.getAchievement(4);
            }
            if (Game1.player.totalMoneyEarned >= 1000000)
            {
                this.Monitor.Log($"[Achievement] 1mil Money earned unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(3) && !this.Config.OnlyLog)
                    Game1.getAchievement(3);
            }
            if (Game1.player.totalMoneyEarned >= 250000)
            {
                this.Monitor.Log($"[Achievement] 250k Money earned unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(2) && !this.Config.OnlyLog)
                    Game1.getAchievement(2);
            }
            if (Game1.player.totalMoneyEarned >= 50000)
            {
                this.Monitor.Log($"[Achievement] 50k Money earned unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(1) && !this.Config.OnlyLog)
                    Game1.getAchievement(1);
            }
            if (Game1.player.totalMoneyEarned >= 15000)
            {
                this.Monitor.Log($"[Achievement] 15k Money earned unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(0) && !this.Config.OnlyLog)
                    Game1.getAchievement(0);
            }
        }

        private void CheckForSkillsAchievements()
        {
            if ((int)Game1.player.farmingLevel == 10 || (int)Game1.player.miningLevel == 10 || (int)Game1.player.fishingLevel == 10 || (int)Game1.player.foragingLevel == 10 || (int)Game1.player.combatLevel == 10)
            {
                this.Monitor.Log($"[Achievement] Singular Talent unlocked.", LogLevel.Info);

                Game1.getSteamAchievement("Achievement_SingularTalent");
            }

            if ((int)Game1.player.farmingLevel == 10 && (int)Game1.player.miningLevel == 10 && (int)Game1.player.fishingLevel == 10 && (int)Game1.player.foragingLevel == 10 && (int)Game1.player.combatLevel == 10)
            {
                this.Monitor.Log($"[Achievement] Master of the Five Ways unlocked.", LogLevel.Info);
                Game1.getSteamAchievement("Achievement_MasterOfTheFiveWays");
            }
        }

        private void CheckForReachedBottomMine()
        {
            if (Game1.player.hasSkullKey)
            {
                this.Monitor.Log($"[Achievement] The Bottom unlocked.", LogLevel.Info);
                Game1.getSteamAchievement("Achievement_TheBottom");
            }
        }

        private void CheckForFullHouseAchievement()
        {
            if (Game1.player.getChildrenCount() >= 2)
            {
                this.Monitor.Log($"[Achievement] Full House unlocked.", LogLevel.Info);
                Game1.getSteamAchievement("Achievement_FullHouse");
            }
        }

        private void CheckForStardropAchievement()
        {
            if (Utility.foundAllStardrops())
            {
                this.Monitor.Log($"[Achievement] Stardrop unlocked.", LogLevel.Info);
                Game1.getSteamAchievement("Achievement_Stardrop");
            }
        }

        private void CheckForJojaAchievement()
        {
            if (Game1.MasterPlayer.mailReceived.Contains("JojaMember"))
            {
                this.Monitor.Log($"[Achievement] Joja CC unlocked. (poor you :( )", LogLevel.Info);
                Game1.getSteamAchievement("Achievement_Joja");
            }
        }

        private void CheckForCommunityCenterAchievement()
        {
            if (Game1.player.eventsSeen.Contains(191393))
            {
                this.Monitor.Log($"[Achievement] Local Legend unlocked.", LogLevel.Info);
                Game1.getSteamAchievement("Achievement_LocalLegend");
            }
        }

        private void CheckForCraftedRecipesAchievements()
        {
            Dictionary<string, string> recipes = Game1.content.Load<Dictionary<string, string>>("Data\\CraftingRecipes");
            int numberOfRecipesMade = 0;
            int numberOfItemsCrafted = 0;
            foreach (string s in recipes.Keys)
            {
                if (!(s == "Wedding Ring") && Game1.player.craftingRecipes.ContainsKey(s))
                {
                    numberOfItemsCrafted += Game1.player.craftingRecipes[s];
                    if (Game1.player.craftingRecipes[s] > 0)
                    {
                        numberOfRecipesMade++;
                    }
                }
            }
            if (numberOfRecipesMade >= recipes.Count - 1 || Config.ArtisanAchievementShouldUseRecipeCount && numberOfRecipesMade >= Config.RecipeCountForArtisanAchievement)
            {
                this.Monitor.Log($"[Achievement] Craft Master unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(22) && !this.Config.OnlyLog)
                    Game1.getAchievement(22);
            }
            if (numberOfRecipesMade >= 30)
            {
                this.Monitor.Log($"[Achievement] Artisan unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(21) && !this.Config.OnlyLog)
                    Game1.getAchievement(21);
            }
            if (numberOfRecipesMade >= 15)
            {
                this.Monitor.Log($"[Achievement] D.I.Y unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(20) && !this.Config.OnlyLog)
                    Game1.getAchievement(20);
            }
        }

        private void CheckForFishingAchievements()
        {
            int numberOfFishCaught = 0;
            int differentKindsOfFishCaught = 0;
            int totalKindsOfFish = 0;
            foreach (KeyValuePair<int, string> v in Game1.objectInformation)
            {
                if (v.Value.Split('/')[3].Contains("Fish") && (v.Key < 167 || v.Key >= 173))
                {
                    totalKindsOfFish++;
                    if (Game1.player.fishCaught.ContainsKey(v.Key))
                    {
                        numberOfFishCaught += Game1.player.fishCaught[v.Key][0];
                        differentKindsOfFishCaught++;
                    }
                }
            }
            if (numberOfFishCaught >= 100)
            {
                this.Monitor.Log($"[Achievement] Mother Catch unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(27) && !this.Config.OnlyLog)
                    Game1.getAchievement(27);
            }
            if (differentKindsOfFishCaught == totalKindsOfFish)
            {
                this.Monitor.Log($"[Achievement] Master Angler unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(26) && !this.Config.OnlyLog)
                    Game1.getAchievement(26);
            }

            if (differentKindsOfFishCaught >= 24)
            {
                this.Monitor.Log($"[Achievement] Ol' Mariner unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(25) && !this.Config.OnlyLog)
                    Game1.getAchievement(25);
            }
            if (differentKindsOfFishCaught >= 10)
            {
                this.Monitor.Log($"[Achievement] Fisherman unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(24) && !this.Config.OnlyLog)
                    Game1.getAchievement(24);
            }
        }

        private void CheckForShippedEveryItemAchievement()
        {
            if (Utility.hasFarmerShippedAllItems())
            {
                this.Monitor.Log($"[Achievement] Full Shipment unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(34) && !this.Config.OnlyLog)
                    Game1.getAchievement(34);
            };
        }

        private void CheckForShippingAchievements()
        {
            if (farmerShipped(24, 15) && farmerShipped(188, 15) && farmerShipped(190, 15) && farmerShipped(192, 15) && farmerShipped(248, 15) && farmerShipped(250, 15) && farmerShipped(252, 15) && farmerShipped(254, 15) && farmerShipped(256, 15) && farmerShipped(258, 15) && farmerShipped(260, 15) && farmerShipped(262, 15) && farmerShipped(264, 15) && farmerShipped(266, 15) && farmerShipped(268, 15) && farmerShipped(270, 15) && farmerShipped(272, 15) && farmerShipped(274, 15) && farmerShipped(276, 15) && farmerShipped(278, 15) && farmerShipped(280, 15) && farmerShipped(282, 15) && farmerShipped(284, 15) && farmerShipped(300, 15) && farmerShipped(304, 15) && farmerShipped(398, 15) && farmerShipped(400, 15) && farmerShipped(433, 15))
            {
                this.Monitor.Log($"[Achievement] Polyculture unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(31) && !this.Config.OnlyLog)
                    Game1.getAchievement(31);
            }
            if (farmerShipped(24, 300) || farmerShipped(188, 300) || farmerShipped(190, 300) || farmerShipped(192, 300) || farmerShipped(248, 300) || farmerShipped(250, 300) || farmerShipped(252, 300) || farmerShipped(254, 300) || farmerShipped(256, 300) || farmerShipped(258, 300) || farmerShipped(260, 300) || farmerShipped(262, 300) || farmerShipped(264, 300) || farmerShipped(266, 300) || farmerShipped(268, 300) || farmerShipped(270, 300) || farmerShipped(272, 300) || farmerShipped(274, 300) || farmerShipped(276, 300) || farmerShipped(278, 300) || farmerShipped(280, 300) || farmerShipped(282, 300) || farmerShipped(284, 300) || farmerShipped(454, 300) || farmerShipped(300, 300) || farmerShipped(304, 300) || (farmerShipped(398, 300) | farmerShipped(433, 300)) || farmerShipped(400, 300) || farmerShipped(591, 300) || farmerShipped(593, 300) || farmerShipped(595, 300) || farmerShipped(597, 300))
            {
                this.Monitor.Log($"[Achievement] Monoculture unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(32) && !this.Config.OnlyLog)
                    Game1.getAchievement(32);
            }

            bool farmerShipped(int index, int number)
            {
                if (Game1.player.basicShipped.ContainsKey(index) && Game1.player.basicShipped[index] >= number)
                {
                    return true;
                }
                return false;
            }
        }

        private void CheckForQuestAchievements()
        {
            if (Game1.stats.questsCompleted >= 40)
            {
                this.Monitor.Log($"[Achievement] A Big unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(30) && !this.Config.OnlyLog)
                    Game1.getAchievement(30);
            }
            if (Game1.stats.questsCompleted >= 10)
            {
                this.Monitor.Log($"[Achievement] Gofer unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(29) && !this.Config.OnlyLog)
                    Game1.getAchievement(29);
            }
        }

        private void CheckForHouseUpgradesAchievements()
        {
            if (Game1.player.HouseUpgradeLevel >= 2)
            {
                this.Monitor.Log($"[Achievement] Living Large unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(19) && !this.Config.OnlyLog)
                    Game1.getAchievement(19);
            }
            if (Game1.player.HouseUpgradeLevel >= 1)
            {
                this.Monitor.Log($"[Achievement] Moving Up unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(18) && !this.Config.OnlyLog)
                    Game1.getAchievement(18);
            }
        }

        private void CheckForFriendshipAchievements()
        {
            uint numberOf5Level = 0u;
            uint numberOf8Level = 0u;
            uint numberOf10Level = 0u;
            foreach (Friendship value in Game1.player.friendshipData.Values)
            {
                if (value.Points >= 2500)
                {
                    numberOf10Level++;
                }
                if (value.Points >= 2000)
                {
                    numberOf8Level++;
                }
                if (value.Points >= 1250)
                {
                    numberOf5Level++;
                }
            }

            if (numberOf5Level >= 20)
            {
                this.Monitor.Log($"[Achievement] Popular unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(13) && !this.Config.OnlyLog)
                    Game1.getAchievement(13);
            }
            if (numberOf5Level >= 10)
            {
                this.Monitor.Log($"[Achievement] Networking unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(12) && !this.Config.OnlyLog)
                    Game1.getAchievement(12);
            }
            if (numberOf5Level >= 4)
            {
                this.Monitor.Log($"[Achievement] Cliques 5 hearts with 4 people unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(11) && !this.Config.OnlyLog)
                    Game1.getAchievement(11);
            }
            if (numberOf5Level >= 1)
            {
                this.Monitor.Log($"[Achievement] A New Friend unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(6) && !this.Config.OnlyLog)
                    Game1.getAchievement(6);
            }
            if (numberOf10Level >= 8)
            {
                this.Monitor.Log($"[Achievement] The Beloved Farmer unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(9) && !this.Config.OnlyLog)
                    Game1.getAchievement(9);
            }
            if (numberOf10Level >= 1)
            {
                this.Monitor.Log($"[Achievement] Best Friends unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(7) && !this.Config.OnlyLog)
                    Game1.getAchievement(7);
            }
        }

        private void CheckForMuseumAchievement()
        {
            int num = Game1.netWorldState.Value.MuseumPieces.Count();
            if (num >= 95)
            {
                this.Monitor.Log($"[Achievement] Complete Collection unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(5) && !this.Config.OnlyLog)
                    Game1.getAchievement(5);
            }
            if (num >= 40)
            {
                this.Monitor.Log($"[Achievement] Treasure Trove unlocked.", LogLevel.Info);

                if (!Game1.player.achievements.Contains(28) && !this.Config.OnlyLog)
                    Game1.getAchievement(28);
            }
        }

        private void CheckForCookingAchievements()
        {
            Dictionary<string, string> recipes = Game1.content.Load<Dictionary<string, string>>("Data\\CookingRecipes");
            int numberOfRecipesCooked = 0;
            int numberOfMealsMade = 0;
            foreach (KeyValuePair<string, string> v in recipes)
            {
                if (Game1.player.cookingRecipes.ContainsKey(v.Key))
                {
                    int recipe = Convert.ToInt32(v.Value.Split('/')[2].Split(' ')[0]);
                    if (Game1.player.recipesCooked.ContainsKey(recipe))
                    {
                        numberOfMealsMade += Game1.player.recipesCooked[recipe];
                        numberOfRecipesCooked++;
                    }
                }
            }
            if (numberOfRecipesCooked == recipes.Count)
            {
                this.Monitor.Log($"[Achievement] Every Recipe Cooked unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(17) && !this.Config.OnlyLog)
                    Game1.getAchievement(17);
            }
            if (numberOfRecipesCooked >= 25)
            {
                this.Monitor.Log($"[Achievement] Sous Chef should Be unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(16) && !this.Config.OnlyLog)
                    Game1.getAchievement(16);
            }
            if (numberOfRecipesCooked >= 10)
            {
                this.Monitor.Log($"[Achievement] 10 recipes should Be unlocked.", LogLevel.Info);
                if (!Game1.player.achievements.Contains(15) && !this.Config.OnlyLog)
                    Game1.getAchievement(15);
            }
        }

        private void CheckForAdventureAchievements()
        {
            int slimesCount = Game1.stats.getMonstersKilled("Green Slime") + Game1.stats.getMonstersKilled("Frost Jelly") + Game1.stats.getMonstersKilled("Sludge");
            int ShadowCount = Game1.stats.getMonstersKilled("Shadow Guy") + Game1.stats.getMonstersKilled("Shadow Shaman") + Game1.stats.getMonstersKilled("Shadow Brute");
            int SkeletonsCount = Game1.stats.getMonstersKilled("Skeleton") + Game1.stats.getMonstersKilled("Skeleton Mage");
            int CrabsCount = Game1.stats.getMonstersKilled("Rock Crab") + Game1.stats.getMonstersKilled("Lava Crab") + Game1.stats.getMonstersKilled("Iridium Crab");
            int BugsCount = Game1.stats.getMonstersKilled("Grub") + Game1.stats.getMonstersKilled("Fly") + Game1.stats.getMonstersKilled("Bug");
            int BatsCount = Game1.stats.getMonstersKilled("Bat") + Game1.stats.getMonstersKilled("Frost Bat") + Game1.stats.getMonstersKilled("Lava Bat");
            int DuggyCount = Game1.stats.getMonstersKilled("Duggy");
            int DustCount = Game1.stats.getMonstersKilled("Dust Spirit");
            int MummyCount = Game1.stats.getMonstersKilled("Mummy");
            int DinoCount = Game1.stats.getMonstersKilled("Pepper Rex");
            int SerpentCount = Game1.stats.getMonstersKilled("Serpent");

            if (slimesCount >= 1000 &&
                ShadowCount >= 150 &&
                SkeletonsCount >= 50 &&
                BugsCount >= 125 &&
                BatsCount >= 200 &&
                DuggyCount >= 30 &&
                DustCount >= 500 &&
                CrabsCount >= 60 &&
                MummyCount >= 100 &&
                DinoCount >= 50 &&
                SerpentCount >= 250)
            {
                this.Monitor.Log($"[Achievement] AdventureAchievement unlocked.", LogLevel.Info);
                Game1.getSteamAchievement("Achievement_KeeperOfTheMysticRings");
            }
        }
    }
}
