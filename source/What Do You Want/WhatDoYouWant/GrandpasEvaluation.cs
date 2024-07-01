/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/emurphy42/WhatDoYouWant
**
*************************************************/


using StardewValley;

namespace WhatDoYouWant
{
    internal class ScoreData
    {
        public string? DescriptionToken { get; set; }
        public int Points { get; set; }
    }

    enum MoneyTier
    {
        None,
        FiftyThousand,
        OneHundredThousand,
        TwoHundredThousand,
        ThreeHundredThousand,
        FiveHundredThousand,
        OneMillion
    }

    internal class GrandpasEvaluation
    {
        public static void ShowGrandpasEvaluationList(ModEntry modInstance)
        {
            var title = modInstance.GetTitle_GrandpasEvaluation();

            // based on base game scoring logic
            var ScoresAchieved = new List<ScoreData>();
            var ScoresAvailable = new List<ScoreData>();

            // Total Earnings

            MoneyTier currentMoneyTier;
            
            if (Game1.player.totalMoneyEarned >= 1000000U)
            {
                currentMoneyTier = MoneyTier.OneMillion;
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnOneMillion", Points = 7 });
            }
            else if (Game1.player.totalMoneyEarned >= 500000U)
            {
                currentMoneyTier = MoneyTier.FiveHundredThousand;
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnFiveHundredThousand", Points = 5 });
            }
            else if (Game1.player.totalMoneyEarned >= 300000U)
            {
                currentMoneyTier = MoneyTier.ThreeHundredThousand;
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnThreeHundredThousand", Points = 4 });
            }
            else if (Game1.player.totalMoneyEarned >= 200000U)
            {
                currentMoneyTier = MoneyTier.TwoHundredThousand;
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnTwoHundredThousand", Points = 3 });
            }
            else if (Game1.player.totalMoneyEarned >= 100000U)
            {
                currentMoneyTier = MoneyTier.OneHundredThousand;
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnOneHundredThousand", Points = 2 });
            }
            else if (Game1.player.totalMoneyEarned >= 50000U)
            {
                currentMoneyTier = MoneyTier.FiftyThousand;
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnFiftyThousand", Points = 1 });
            }
            else
            {
                currentMoneyTier = MoneyTier.None;
            }

            // C# switch() limits fall-through to empty blocks, so instead we rely on the enum values being in the right order
            if (currentMoneyTier < MoneyTier.FiftyThousand)
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnFiftyThousand", Points = 1 });
            }
            if (currentMoneyTier < MoneyTier.OneHundredThousand)
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnOneHundredThousand", Points = 1 });
            }
            if (currentMoneyTier < MoneyTier.TwoHundredThousand)
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnTwoHundredThousand", Points = 1 });
            }
            if (currentMoneyTier < MoneyTier.ThreeHundredThousand)
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnThreeHundredThousand", Points = 1 });
            }
            if (currentMoneyTier < MoneyTier.FiveHundredThousand)
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnFiveHundredThousand", Points = 1 });
            }
            if (currentMoneyTier < MoneyTier.OneMillion)
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_EarnOneMillion", Points = 2 });
            }

            // Player Level (https://stardewvalleywiki.com/Skills#Skill-Based_Title)

            if (Game1.player.Level >= 25)
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_PlayerLevel25", Points = 2 });
            }
            else if (Game1.player.Level >= 15)
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_PlayerLevel15", Points = 1 });
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_PlayerLevel25", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_PlayerLevel15", Points = 1 });
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_PlayerLevel25", Points = 1 });
            }

            // Achievements

            if (Game1.player.achievements.Contains(ModEntry.AchievementID_Museum))
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_AchievementMuseum", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_AchievementMuseum", Points = 1 });
            }

            if (Game1.player.achievements.Contains(ModEntry.AchievementID_Fishing))
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_AchievementFishing", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_AchievementFishing", Points = 1 });
            }

            if (Game1.player.achievements.Contains(ModEntry.AchievementID_Shipping))
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_AchievementShipping", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_AchievementShipping", Points = 1 });
            }

            // Friendship

            if (Game1.player.isMarriedOrRoommates() && Utility.getHomeOfFarmer(Game1.player).upgradeLevel >= 2)
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipMarriedTwoHouseUpgrades", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipMarriedTwoHouseUpgrades", Points = 1 });
            }

            var numberFriendsWithEnoughHearts = Utility.getNumberOfFriendsWithinThisRange(who: Game1.player, minFriendshipPoints: 1975, maxFriendshipPoints: 999999);
            if (numberFriendsWithEnoughHearts >= 10)
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipTenFriends", Points = 2 });
            }
            else if (numberFriendsWithEnoughHearts >= 5)
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipFiveFriends", Points = 1 });
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipTenFriends", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipFiveFriends", Points = 1 });
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipTenFriends", Points = 1 });
            }

            if (Game1.player.mailReceived.Contains("petLoveMessage"))
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipPet", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_FriendshipPet", Points = 1 });
            }

            // Other

            if (Game1.isLocationAccessible("CommunityCenter"))
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_CommunityCenterAvailable", Points = 3 });
            }
            else if (Game1.player.hasCompletedCommunityCenter())
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_CommunityCenterCompleted", Points = 1 });
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_CommunityCenterAvailable", Points = 2 });
            }
            else if (!Game1.player.hasOrWillReceiveMail("JojaMember"))
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_CommunityCenterCompleted", Points = 1 });
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_CommunityCenterAvailable", Points = 2 });
            }

            if (Game1.player.hasSkullKey)
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_SkullKey", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_SkullKey", Points = 1 });
            }

            if (Game1.player.hasRustyKey)
            {
                ScoresAchieved.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_RustyKey", Points = 1 });
            }
            else
            {
                ScoresAvailable.Add(new ScoreData { DescriptionToken = "GrandpasEvaluation_RustyKey", Points = 1 });
            }

            // Calculate total score

            var totalScore = 0;
            foreach (var ScoreAchieved in ScoresAchieved)
            {
                totalScore += ScoreAchieved.Points;
            }
            var numberCandles = Utility.getGrandpaCandlesFromScore(totalScore);
            if (numberCandles == 4)
            {
                var completeDescription = modInstance.Helper.Translation.Get("GrandpasEvaluation_Complete", new { title = title });
                Game1.drawDialogueNoTyping(completeDescription);
                return;
            }

            var linesToDisplay = new List<string>();

            var incompleteDescription = modInstance.Helper.Translation.Get("GrandpasEvaluation_Incomplete", new {
                title = title,
                totalScore = totalScore,
                numberCandles = numberCandles
            });
            linesToDisplay.Add($"{incompleteDescription}{ModEntry.LineBreak}");

            if (numberCandles < 2)
            {
                var twoCandlesDescription = modInstance.Helper.Translation.Get("GrandpasEvaluation_PointsNeededForCandles", new {
                    totalPointsNeeded = 4,
                    targetCandles = 2
                });
                linesToDisplay.Add($"{twoCandlesDescription}{ModEntry.LineBreak}");
            }
            if (numberCandles < 3)
            {
                var threeCandlesDescription = modInstance.Helper.Translation.Get("GrandpasEvaluation_PointsNeededForCandles", new
                {
                    totalPointsNeeded = 8,
                    targetCandles = 3
                });
                linesToDisplay.Add($"{threeCandlesDescription}{ModEntry.LineBreak}");
            }

            var fourCandlesDescription = modInstance.Helper.Translation.Get("GrandpasEvaluation_PointsNeededForCandles", new
            {
                totalPointsNeeded = 12,
                targetCandles = 4
            });
            linesToDisplay.Add($"{fourCandlesDescription}{ModEntry.LineBreak}");

            foreach (var ScoreAvailable in ScoresAvailable)
            {
                var description = modInstance.Helper.Translation.Get(ScoreAvailable.DescriptionToken ?? "");
                linesToDisplay.Add($"* {ScoreAvailable.Points} - {description}{ModEntry.LineBreak}");
            }

            modInstance.ShowLines(linesToDisplay, title: title, longLinesExpected: true);
        }
    }
}
