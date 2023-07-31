/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Objects.Items.Rewards;
using FishingTrawler.Framework.Objects.Items.Tools;
using FishingTrawler.Framework.Utilities;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using Object = StardewValley.Object;

namespace FishingTrawler.Objects
{
    internal class TrawlerRewards
    {
        internal static readonly int[] forbiddenFish = new int[] { 159, 160, 163, 775, 682, 898, 899, 900, 901 };

        private Chest _rewardChest;
        private Farmer _farmer;

        internal float fishCatchChanceOffset;  // Higher this number is, the more unlikely the player is to get harder to catch fish (default 0)
        internal bool isGambling; // If true, the rewards will have 50% of being doubled (along with XP), but 25% of losing it all
        internal bool hasMermaidsBlessing; // If true, 5% chance of consuming fish but getting treasure chest rewards instead
        internal bool hasPatronSaint; // If true, 25% chance of consuming fish but gives full XP
        internal bool hasWorldly; // If true, allows catching of non-ocean fish
        internal bool hasSlimeKing; // If true, consumes all fish but gives a 75% chance of converting each fish into some slime, 50% chance to convert to a Slimejack and a 1% chance to convert into a random slime egg 
        internal bool hasKingCrab; // If true, causes the trawler to only catch crab pot based creatures and higher chance of crab

        public TrawlerRewards(Chest rewardChest)
        {
            _rewardChest = rewardChest;

            Reset(Game1.player); // Main player will get XP by default, though it can be overridden
        }

        public void Reset(Farmer farmer)
        {
            _farmer = farmer;

            fishCatchChanceOffset = 0f;
            isGambling = false;
            hasMermaidsBlessing = false;
            hasPatronSaint = false;
            hasWorldly = false;
            hasSlimeKing = false;
            hasKingCrab = false;
        }

        private int[] GetEligibleFishIds(bool allowCatchingOfNonOceanFish = false)
        {
            List<int> eligibleFishIds = new List<int>();

            // Iterate through any valid locations to find the fish eligible for rewarding (fish need to be in season and player must have minimum level for it)
            Dictionary<string, string> locationData = Game1.content.Load<Dictionary<string, string>>("Data\\Locations");
            foreach (GameLocation location in Game1.locations.Where(l => l.Name == (allowCatchingOfNonOceanFish ? l.Name : "Beach")))
            {
                if (!locationData.ContainsKey(location.Name))
                {
                    continue;
                }

                string[] rawFishData = locationData[location.Name].Split('/')[4 + Utility.getSeasonNumber(Game1.currentSeason)].Split(' ');
                Dictionary<int, string> rawFishDataWithLocation = new Dictionary<int, string>();
                if (rawFishData.Length > 1)
                {
                    for (int j = 0; j < rawFishData.Length; j += 2)
                    {
                        try
                        {
                            rawFishDataWithLocation[Convert.ToInt32(rawFishData[j])] = rawFishData[j + 1];
                        }
                        catch (IndexOutOfRangeException ex)
                        {
                            FishingTrawler.monitor.Log($"Failed to extract rawFishData for index {j} at location {location.Name}");
                        }
                    }
                }

                eligibleFishIds.AddRange(rawFishDataWithLocation.Keys);
            }

            Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            eligibleFishIds.AddRange(fishData.Where(f => f.Value.Split('/')[1] == "trap").Select(f => f.Key).Where(i => !forbiddenFish.Contains(i)));

            return eligibleFishIds.Distinct().ToArray();
        }

        private int AttemptGamble(int amountOfFish)
        {
            if (Game1.random.NextDouble() <= 0.5)
            {
                Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.gamblers_crest_effect.success"), null));
                return amountOfFish *= 2;
            }

            if (Game1.random.NextDouble() <= 0.25)
            {
                Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.gamblers_crest_effect.failed"), null));
                return 0;
            }

            Game1.addHUDMessage(new HUDMessage(FishingTrawler.i18n.Get("game_message.gamblers_crest_effect.neutral"), null));
            return amountOfFish;
        }

        private void AddMermaidTreasure(int clearWaterDistance)
        {
            List<Item> treasures = new List<Item>();

            float chance = 1f;
            while (Game1.random.NextDouble() <= (double)chance)
            {
                chance *= 0.4f;
                switch (Game1.random.Next(4))
                {
                    case 0:
                        {
                            if (clearWaterDistance >= 5 && Game1.random.NextDouble() < 0.03)
                            {
                                treasures.Add(new Object(386, Game1.random.Next(1, 3)));
                                break;
                            }
                            List<int> possibles = new List<int>();
                            if (clearWaterDistance >= 4)
                            {
                                possibles.Add(384);
                            }
                            if (clearWaterDistance >= 3 && (possibles.Count == 0 || Game1.random.NextDouble() < 0.6))
                            {
                                possibles.Add(380);
                            }
                            if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
                            {
                                possibles.Add(378);
                            }
                            if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
                            {
                                possibles.Add(388);
                            }
                            if (possibles.Count == 0 || Game1.random.NextDouble() < 0.6)
                            {
                                possibles.Add(390);
                            }
                            possibles.Add(382);
                            treasures.Add(new Object(possibles.ElementAt(Game1.random.Next(possibles.Count)), Game1.random.Next(2, 7) * (!(Game1.random.NextDouble() < 0.05 + (int)_farmer.luckLevel * 0.015) ? 1 : 2)));
                            if (Game1.random.NextDouble() < 0.05 + _farmer.LuckLevel * 0.03)
                            {
                                treasures.Last().Stack *= 2;
                            }
                            break;
                        }
                    case 1:
                        if (clearWaterDistance >= 4 && Game1.random.NextDouble() < 0.1 && _farmer.FishingLevel >= 6)
                        {
                            treasures.Add(new Object(687, 1));
                        }
                        else if (Game1.random.NextDouble() < 0.25 && _farmer.craftingRecipes.ContainsKey("Wild Bait"))
                        {
                            treasures.Add(new Object(774, 5 + (Game1.random.NextDouble() < 0.25 ? 5 : 0)));
                        }
                        else if (_farmer.FishingLevel >= 6)
                        {
                            treasures.Add(new Object(685, 1));
                        }
                        else
                        {
                            treasures.Add(new Object(685, 10));
                        }
                        break;
                    case 2:
                        if (Game1.random.NextDouble() < 0.1 && (int)Game1.netWorldState.Value.LostBooksFound < 21 && _farmer != null && _farmer.hasOrWillReceiveMail("lostBookFound"))
                        {
                            treasures.Add(new Object(102, 1));
                        }
                        else if (_farmer.archaeologyFound.Count() > 0)
                        {
                            if (Game1.random.NextDouble() < 0.25 && _farmer.FishingLevel > 1)
                            {
                                treasures.Add(new Object(Game1.random.Next(585, 588), 1));
                            }
                            else if (Game1.random.NextDouble() < 0.5 && _farmer.FishingLevel > 1)
                            {
                                treasures.Add(new Object(Game1.random.Next(103, 120), 1));
                            }
                            else
                            {
                                treasures.Add(new Object(535, 1));
                            }
                        }
                        else
                        {
                            treasures.Add(new Object(382, Game1.random.Next(1, 3)));
                        }
                        break;
                    case 3:
                        switch (Game1.random.Next(3))
                        {
                            case 0:
                                if (clearWaterDistance >= 4)
                                {
                                    treasures.Add(new Object(537 + (Game1.random.NextDouble() < 0.4 ? Game1.random.Next(-2, 0) : 0), Game1.random.Next(1, 4)));
                                }
                                else if (clearWaterDistance >= 3)
                                {
                                    treasures.Add(new Object(536 + (Game1.random.NextDouble() < 0.4 ? -1 : 0), Game1.random.Next(1, 4)));
                                }
                                else
                                {
                                    treasures.Add(new Object(535, Game1.random.Next(1, 4)));
                                }
                                if (Game1.random.NextDouble() < 0.05 + _farmer.LuckLevel * 0.03)
                                {
                                    treasures.Last().Stack *= 2;
                                }
                                break;
                            case 1:
                                if (_farmer.FishingLevel < 2)
                                {
                                    treasures.Add(new Object(382, Game1.random.Next(1, 4)));
                                    break;
                                }
                                if (clearWaterDistance >= 4)
                                {
                                    treasures.Add(new Object(Game1.random.NextDouble() < 0.3 ? 82 : Game1.random.NextDouble() < 0.5 ? 64 : 60, Game1.random.Next(1, 3)));
                                }
                                else if (clearWaterDistance >= 3)
                                {
                                    treasures.Add(new Object(Game1.random.NextDouble() < 0.3 ? 84 : Game1.random.NextDouble() < 0.5 ? 70 : 62, Game1.random.Next(1, 3)));
                                }
                                else
                                {
                                    treasures.Add(new Object(Game1.random.NextDouble() < 0.3 ? 86 : Game1.random.NextDouble() < 0.5 ? 66 : 68, Game1.random.Next(1, 3)));
                                }
                                if (Game1.random.NextDouble() < 0.028 * (double)(clearWaterDistance / 5f))
                                {
                                    treasures.Add(new Object(72, 1));
                                }
                                if (Game1.random.NextDouble() < 0.05)
                                {
                                    treasures.Last().Stack *= 2;
                                }
                                break;
                            case 2:
                                {
                                    if (_farmer.FishingLevel < 2)
                                    {
                                        treasures.Add(new Object(770, Game1.random.Next(1, 4)));
                                        break;
                                    }
                                    float luckModifier = (1f + (float)_farmer.DailyLuck) * (clearWaterDistance / 5f);
                                    if (Game1.random.NextDouble() < 0.05 * (double)luckModifier && !_farmer.specialItems.Contains(14))
                                    {
                                        treasures.Add(new MeleeWeapon(14)
                                        {
                                            specialItem = true
                                        });
                                    }
                                    if (Game1.random.NextDouble() < 0.05 * (double)luckModifier && !_farmer.specialItems.Contains(51))
                                    {
                                        treasures.Add(new MeleeWeapon(51)
                                        {
                                            specialItem = true
                                        });
                                    }
                                    if (Game1.random.NextDouble() < 0.07 * (double)luckModifier)
                                    {
                                        switch (Game1.random.Next(3))
                                        {
                                            case 0:
                                                treasures.Add(new Ring(516 + (Game1.random.NextDouble() < (double)(_farmer.LuckLevel / 11f) ? 1 : 0)));
                                                break;
                                            case 1:
                                                treasures.Add(new Ring(518 + (Game1.random.NextDouble() < (double)(_farmer.LuckLevel / 11f) ? 1 : 0)));
                                                break;
                                            case 2:
                                                treasures.Add(new Ring(Game1.random.Next(529, 535)));
                                                break;
                                        }
                                    }
                                    if (Game1.random.NextDouble() < 0.02 * (double)luckModifier)
                                    {
                                        treasures.Add(new Object(166, 1));
                                    }
                                    if (_farmer.FishingLevel > 5 && Game1.random.NextDouble() < 0.001 * (double)luckModifier)
                                    {
                                        treasures.Add(new Object(74, 1));
                                    }
                                    if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                    {
                                        treasures.Add(new Object(127, 1));
                                    }
                                    if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                    {
                                        treasures.Add(new Object(126, 1));
                                    }
                                    if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                    {
                                        treasures.Add(new Ring(527));
                                    }
                                    if (Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                    {
                                        treasures.Add(new Boots(Game1.random.Next(504, 514)));
                                    }
                                    if (Game1.MasterPlayer.mailReceived.Contains("Farm_Eternal") && Game1.random.NextDouble() < 0.01 * (double)luckModifier)
                                    {
                                        treasures.Add(new Object(928, 1));
                                    }
                                    if (treasures.Count == 1)
                                    {
                                        treasures.Add(new Object(72, 1));
                                    }
                                    break;
                                }
                        }
                        break;
                }
            }
            if (treasures.Count == 0)
            {
                _rewardChest.addItem(new Object(685, Game1.random.Next(1, 4) * 5));
            }

            // Add the determined rewards
            foreach (var item in treasures)
            {
                _rewardChest.addItem(item);
            }
        }

        private Object GetRandomSlimeEgg()
        {
            switch (Game1.random.Next(0, 5))
            {
                case 1:
                    return new Object(437, 1);
                case 2:
                    return new Object(439, 1);
                case 3:
                    return new Object(680, 1);
                case 4:
                    // If player has not unlocked Willy's boat / the Island, then return a green slime egg. Otherwise return the tiger slime egg
                    return Game1.MasterPlayer.hasOrWillReceiveMail("willyBoatFixed") ? new Object(857, 1) : new Object(413, 1);
                default:
                    return new Object(413, 1);
            }
        }

        internal bool HasGottenAllSpecialRewards()
        {
            if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_ANGLER_RING) is false)
            {
                return false;
            }
            else if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_LOST_FISHING_CHARM) is false)
            {
                return false;
            }
            else if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_GUIDING_NARWHAL) is false)
            {
                return false;
            }
            else if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_RUSTY_CAGE) is false)
            {
                return false;
            }
            else if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_ALLURING_JELLYFISH) is false)
            {
                return false;
            }
            else if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_WEIGHTED_TREASURE) is false)
            {
                return false;
            }
            else if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_STARRY_BOBBER) is false)
            {
                return false;
            }
            else if (_farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_TRIDENT_TOOL) is false)
            {
                return false;
            }

            return true;
        }

        internal Dictionary<string, Item> GetEligibleSpecialRewards()
        {
            Dictionary<string, Item> rewards = new Dictionary<string, Item>();

            bool hasGottenAllRewards = HasGottenAllSpecialRewards();
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_ANGLER_RING) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_ANGLER_RING, AnglerRing.CreateInstance());
            }
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_LOST_FISHING_CHARM) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_LOST_FISHING_CHARM, LostFishingCharm.CreateInstance());
            }
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_GUIDING_NARWHAL) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_GUIDING_NARWHAL, SeaborneTackle.CreateInstance(TackleType.GuidingNarwhal));
            }
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_RUSTY_CAGE) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_RUSTY_CAGE, SeaborneTackle.CreateInstance(TackleType.RustyCage));
            }
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_ALLURING_JELLYFISH) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_ALLURING_JELLYFISH, SeaborneTackle.CreateInstance(TackleType.AlluringJellyfish));
            }
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_WEIGHTED_TREASURE) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_WEIGHTED_TREASURE, SeaborneTackle.CreateInstance(TackleType.WeightedTreasure));
            }
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_STARRY_BOBBER) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_SEABORNE_TACKLE_STARRY_BOBBER, SeaborneTackle.CreateInstance(TackleType.StarryBobber));
            }
            if (hasGottenAllRewards || _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_TRIDENT_TOOL) is false)
            {
                rewards.Add(ModDataKeys.HAS_FARMER_GOTTEN_TRIDENT_TOOL, Trident.CreateInstance());
            }

            return rewards;
        }

        internal void AddSpecialReward()
        {
            var rewards = GetEligibleSpecialRewards();
            var selectedReward = rewards.ElementAt(Game1.random.Next(rewards.Count));

            FishingTrawler.monitor.Log($"Player was rewarded with {selectedReward.Value.DisplayName}!", LogLevel.Trace);
            _rewardChest.addItem(selectedReward.Value);
            _farmer.modData[selectedReward.Key] = true.ToString();
        }

        internal void CalculateAndPopulateReward(int amountOfFish, int baseXpReduction = 5)
        {
            FishingTrawler.monitor.Log($"Calculating rewards for {Game1.player.Name} with {amountOfFish} fish caught!", LogLevel.Trace);

            int[] keys = GetEligibleFishIds(hasWorldly);
            Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");

            // Attempt gamble, if the effect is active
            if (isGambling)
            {
                amountOfFish = AttemptGamble(amountOfFish);
            }

            // See if this run generates an unidentified ancient flag
            FishingTrawler.monitor.Log($"Odds for getting ancient flag during this run: {amountOfFish}, {_farmer.FishingLevel} : {amountOfFish / 500f + _farmer.FishingLevel / 100f}", LogLevel.Trace);
            if (Game1.random.NextDouble() <= amountOfFish / 500f + _farmer.FishingLevel / 100f)
            {
                FishingTrawler.monitor.Log($"Player got lucky and was rewarded an ancient flag!", LogLevel.Trace);
                _rewardChest.addItem(AncientFlag.CreateInstance(FlagType.Unknown));
            }

            // See if this run generates an special reward
            FishingTrawler.monitor.Log($"Odds for getting special reward during this run: {_farmer.modData[ModDataKeys.MURPHY_TRIPS_COMPLETED]} : {Math.Min(int.Parse(_farmer.modData[ModDataKeys.MURPHY_TRIPS_COMPLETED]) + 1, 100) / 400f}", LogLevel.Trace);
            if (Game1.random.NextDouble() <= Math.Min(int.Parse(_farmer.modData[ModDataKeys.MURPHY_TRIPS_COMPLETED]) + 1, 100) / 400f)
            {
                FishingTrawler.monitor.Log($"Player got lucky and has a gotten a special reward!", LogLevel.Trace);
                AddSpecialReward();
            }

            // If it has been over 10 trips with Murphy and the player has not received a trident, then give them one
            if (int.TryParse(_farmer.modData[ModDataKeys.MURPHY_TRIPS_COMPLETED], out int murphyTripsCompleted) && murphyTripsCompleted >= 10 && _farmer.modData.ContainsKey(ModDataKeys.HAS_FARMER_GOTTEN_TRIDENT_TOOL) is false)
            {
                FishingTrawler.monitor.Log($"{_farmer.Name} has reached pity for Trident, gifting it to them!", LogLevel.Trace);
                _rewardChest.addItem(Trident.CreateInstance());
                _farmer.modData[ModDataKeys.HAS_FARMER_GOTTEN_TRIDENT_TOOL] = true.ToString();
            }

            // Calculate the fishing experience to be gained
            float bonusXP = 0f;
            float totalRewardXP = 3f;
            for (int x = 0; x < amountOfFish; x++)
            {
                int minWaterDistance = 1; // Used for Mermaid's Blessing, using min distance required to catch fish (traps are 1 distance)
                float caughtXP = 0f;
                bool caughtFish = false;

                int randomQuantity = Math.Min(Game1.random.Next(0, amountOfFish / 3), 99);
                Item selectedReward = new Object(Game1.random.Next(167, 173), randomQuantity); // Default is random trash item

                Utility.Shuffle(Game1.random, keys);
                for (int i = 0; i < keys.Length; i++)
                {
                    if (!fishData.ContainsKey(Convert.ToInt32(keys[i])))
                    {
                        FishingTrawler.monitor.Log($"Failed to find fish ID {Convert.ToInt32(keys[i])} in fishData, skipping!", LogLevel.Trace);
                        continue;
                    }

                    string[] specificFishData = fishData[Convert.ToInt32(keys[i])].Split('/');

                    if (specificFishData[1] != "trap" && hasKingCrab)
                    {
                        // Skip any non-crab pot based creatures if King Crab is currently active
                        continue;
                    }

                    if (specificFishData[1] == "trap")
                    {
                        double chance = Convert.ToDouble(specificFishData[2]);
                        chance += (double)(_farmer.FishingLevel / 50f);
                        chance /= 1.2f;  // Reduce chance of trap-based catches by 1.2
                        chance = Math.Min(chance, 0.89999997615814209);

                        if (hasKingCrab && keys[i] == 717) // 717 is crab
                        {
                            randomQuantity = Math.Min(Game1.random.Next(randomQuantity, amountOfFish), 99); // Attempt to boost the amount of crabs caught, but still limited to available fish to reward
                            chance = 1; // Always give crab if it is selected as an option, as we want preference towards them
                        }

                        if (Game1.random.NextDouble() <= chance)
                        {
                            caughtFish = true;
                            selectedReward = new Object(Convert.ToInt32(keys[i]), randomQuantity);
                            caughtXP = 5f; // Crab pot always give 5 XP per Vanilla
                            break;
                        }
                    }
                    else if (_farmer.FishingLevel >= Convert.ToInt32(specificFishData[12]))
                    {
                        int difficulty = Convert.ToInt32(specificFishData[1]);
                        double chance = Convert.ToDouble(specificFishData[10]);
                        double dropOffAmount = Convert.ToDouble(specificFishData[11]) * chance;
                        minWaterDistance = Convert.ToInt32(specificFishData[9]);
                        chance -= Math.Max(0, minWaterDistance - 5) * dropOffAmount;
                        chance += (double)(_farmer.FishingLevel / 50f);

                        chance = Math.Min(chance, 0.89999997615814209);
                        if (Game1.random.NextDouble() <= chance - fishCatchChanceOffset)
                        {
                            caughtFish = true;
                            selectedReward = new Object(Convert.ToInt32(keys[i]), randomQuantity);
                            caughtXP = 3f + difficulty / 3;
                            break;
                        }
                    }
                }

                // Check if no fish was selected, if so then give trash
                if (!caughtFish)
                {
                    _rewardChest.addItem(selectedReward);
                    amountOfFish -= randomQuantity;
                    continue;
                }

                // Check if a consuming flag ability is active
                for (int i = 0; i < randomQuantity; i++)
                {
                    if (hasMermaidsBlessing && Game1.random.NextDouble() <= 0.10)
                    {
                        AddMermaidTreasure(minWaterDistance);
                        selectedReward.Stack--;
                        continue;
                    }

                    if (hasPatronSaint && Game1.random.NextDouble() <= 0.25)
                    {
                        bonusXP += caughtXP * ((100 - baseXpReduction) / 100f);
                        selectedReward.Stack--;
                        continue;
                    }

                    if (hasSlimeKing)
                    {
                        switch (Game1.random.NextDouble())
                        {
                            case var chance when chance <= 0.01:
                                _rewardChest.addItem(GetRandomSlimeEgg());
                                selectedReward.Stack--;
                                continue;
                            case var chance when chance <= 0.50:
                                _rewardChest.addItem(new Object(796, 1));
                                selectedReward.Stack--;
                                continue;
                            case var chance when chance <= 0.75:
                                _rewardChest.addItem(new Object(766, 1));
                                selectedReward.Stack--;
                                continue;
                            default:
                                // Lucked out, consume this fish with no replacement
                                selectedReward.Stack--;
                                continue;
                        }
                    }
                }

                // Reduce total fish to be awarded in next pass
                amountOfFish -= randomQuantity;

                // Add selected fish if it hasn't been consumed
                if (selectedReward.Stack > 0)
                {
                    _rewardChest.addItem(selectedReward);
                    totalRewardXP += caughtXP * randomQuantity;
                }
            }

            // Now give XP reward (give 5% of total caught XP)
            //_farmer.gainExperience(1, (int)((totalRewardXP % (100 - baseXpReduction)) + bonusXP));
            int xpGained = (int)(totalRewardXP % (100 - baseXpReduction));
            _farmer.gainExperience(1, xpGained + (int)bonusXP);
            Game1.addHUDMessage(new HUDMessage(string.Format(FishingTrawler.i18n.Get("game_message.xp_gained"), xpGained), null));

            FishingTrawler.monitor.Log($"Gave player {bonusXP} bonus XP, {xpGained} normal XP", LogLevel.Trace);
            if (bonusXP > 0f)
            {
                Game1.addHUDMessage(new HUDMessage(string.Format(FishingTrawler.i18n.Get("game_message.bonus_xp_gained"), bonusXP), null));
            }
        }
    }
}
