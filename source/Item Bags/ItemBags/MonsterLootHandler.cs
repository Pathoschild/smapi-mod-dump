/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-ItemBags
**
*************************************************/

using Harmony;
using ItemBags.Bags;
using ItemBags.Persistence;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ItemBags
{
    public static class MonsterLootHandler
    {
        private static IModHelper Helper { get; set; }
        private static IMonitor Monitor { get { return ItemBagsMod.ModInstance.Monitor; } }
        private static MonsterLootSettings MonsterLootSettings { get { return ItemBagsMod.UserConfig.MonsterLootSettings; } }

        public static readonly Random Randomizer = new Random();
        /// <summary>Returns true if a randomly generated number between 0.0 and 1.0 is less than or equal to the given ChanceOfSuccess</summary>
        /// <param name="ChanceOfSuccess">A value between 0.0 and 1.0. EX: if 0.7, there is a 70% chance that this function returns true.</param>
        /// <returns></returns>
        public static bool RollDice(double ChanceOfSuccess)
        {
            return Randomizer.NextDouble() <= ChanceOfSuccess;
        }

        /// <summary>Returns a random number between the given Minimum and Maximum values.</summary>
        public static double GetRandomNumber(double Minimum, double Maximum)
        {
            return Randomizer.NextDouble() * (Maximum - Minimum) + Minimum;
        }

        /// <summary>Rounds the given double up or down to an integer. The result is more likely to be rounded to whichever is closer.<para/>
        /// EX: If Value=4.3, there is a 70% chance of rounding down to 4, 30% chance of rounding up to 5.</summary>
        public static int WeightedRound(double Value)
        {
            int BaseAmount = (int)Value;
            double RoundUpChance = Value - BaseAmount;
            int NewValue = BaseAmount + Convert.ToInt32(RollDice(RoundUpChance));
            return NewValue;
        }

        /// <summary>Handles patching monster drops to allow receiving ItemBags as rare loot</summary>
        internal static void OnModEntry(IModHelper Helper)
        {
            MonsterLootHandler.Helper = Helper;

            HarmonyInstance Harmony = HarmonyInstance.Create(ItemBagsMod.ModInstance.ModManifest.UniqueID);

            //  Patch GameLocation.monsterDrop, so that we can give a small chance of making monsters also drop ItemBags
            Harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.monsterDrop)),
                postfix: new HarmonyMethod(typeof(MonsterLootHandler), nameof(MonsterLootHandler.MonsterDrop_Postfix))
            );
        }

        public static void MonsterDrop_Postfix(GameLocation __instance, Monster monster, int x, int y, Farmer who)
        {
            try
            {
                MonsterLootSettings LootSettings = MonsterLootSettings;
                if (who.UniqueMultiplayerID == Game1.player.UniqueMultiplayerID && LootSettings.CanReceiveBagsAsDrops)
                {
                    //  Roll chance at receiving an ItemBag
                    double Chance = LootSettings.GetItemBagDropChance(__instance, monster, out double BaseChance, out double LocationMultiplier, out double ExpMultiplier, out double HPMultiplier);
                    bool Success = Randomizer.NextDouble() <= Chance;
                    string LogMessage;
                    if (Success)
                    {
                        if (MonsterLootSettings.LogDropChancesToConsole)
                        {
                            LogMessage = string.Format("Succeeded drop chance: Location = {0}, monster.ExperienceGained = {1}, monster.MaxHealth = {2}\n"
                                + "BaseChance = {3} ({4}%), LocationMultiplier = {5} (+{6}%), ExpMultiplier = {7}, HPMultiplier = {8} (+{9}%), TotalChance = {10} ({11}%)",
                                __instance.Name, monster.ExperienceGained, monster.MaxHealth,
                                BaseChance, (BaseChance * 100.0).ToString("0.##"), LocationMultiplier, ((LocationMultiplier - 1.0) * 100.0).ToString("0.##"),
                                ExpMultiplier.ToString("#.####"), HPMultiplier, ((HPMultiplier - 1.0) * 100.0).ToString("0.##"), Chance, (Chance * 100.0).ToString("0.###"));
                            Monitor.Log(LogMessage, LogLevel.Info);
                        }

                        int SpawnDirection = Randomizer.Next(4);

                        List<ItemBag> OwnedBags = ItemBag.GetAllBags(true);

                        //  Compute the odds of receiving each type of bag
                        bool CanReceiveRucksack = LootSettings.RucksackDropSettings.SizeWeights.Any(size => size.Value > 0);
                        int RucksackWeight = CanReceiveRucksack ? LootSettings.RucksackDropSettings.TypeWeight : 0;
                        bool CanReceiveOmniBag = LootSettings.OmniBagDropSettings.SizeWeights.Any(size => size.Value > 0);
                        int OmniBagWeight = CanReceiveOmniBag ? LootSettings.OmniBagDropSettings.TypeWeight : 0;
                        bool CanReceiveBundleBag = LootSettings.BundleBagDropSettings.SizeWeights.Any(size => BundleBag.ValidSizes.Contains(size.Key) && size.Value > 0 && !IsSizeObsolete(OwnedBags, BundleBag.BundleBagTypeId, size.Key));
                        int BundleBagWeight = CanReceiveBundleBag ? LootSettings.BundleBagDropSettings.TypeWeight : 0;
                        bool CanReceiveStandardBag = LootSettings.StandardBagDropSettings.SizeWeights.Any(size => size.Value > 0);
                        int StandardBagWeight = CanReceiveStandardBag ? LootSettings.StandardBagDropSettings.TypeWeight : 0;

                        int TotalTypeWeight = RucksackWeight + OmniBagWeight + BundleBagWeight + StandardBagWeight;
                        if (TotalTypeWeight > 0)
                        {
                            ItemBag ChosenBag;

                            //  Pick the type of bag to spawn (Rucksack, OmniBag, BundleBag or a standard BoundedBag)
                            int ChosenTypeWeight = Randomizer.Next(0, TotalTypeWeight);
                            if (ChosenTypeWeight < RucksackWeight)
                            {
                                ContainerSize CurrentSize = GetWeightedRandomSize(LootSettings.RucksackDropSettings.SizeWeights);

                                //  Try to force a non-obsolete bag to spawn
                                if (RollDice(LootSettings.ForceNewBagTypeChance))
                                {
                                    List<ContainerSize> ValidSizes = LootSettings.RucksackDropSettings.SizeWeights.Where(size => size.Value > 0).Select(size => size.Key).ToList();
                                    ContainerSize MaxSize = ValidSizes.DefaultIfEmpty(ContainerSize.Small).Max();

                                    while (CurrentSize < MaxSize && IsSizeObsolete(OwnedBags, Rucksack.RucksackTypeId, CurrentSize))
                                    {
                                        CurrentSize = ValidSizes.Where(size => size > CurrentSize).OrderBy(size => size).First();
                                    }
                                }

                                //  Spawn a Rucksack
                                ChosenBag = new Rucksack(CurrentSize, false);
                            }
                            else if (ChosenTypeWeight < RucksackWeight + OmniBagWeight)
                            {
                                ContainerSize CurrentSize = GetWeightedRandomSize(LootSettings.OmniBagDropSettings.SizeWeights);

                                //  Try to force a non-obsolete bag to spawn
                                if (RollDice(LootSettings.ForceNewBagTypeChance))
                                {
                                    List<ContainerSize> ValidSizes = LootSettings.OmniBagDropSettings.SizeWeights.Where(size => size.Value > 0).Select(size => size.Key).ToList();
                                    ContainerSize MaxSize = ValidSizes.DefaultIfEmpty(ContainerSize.Small).Max();

                                    while (CurrentSize < MaxSize && IsSizeObsolete(OwnedBags, OmniBag.OmniBagTypeId, CurrentSize))
                                    {
                                        CurrentSize = ValidSizes.Where(size => size > CurrentSize).OrderBy(size => size).First();
                                    }
                                }

                                //  Spawn an OmniBag
                                ChosenBag = new OmniBag(CurrentSize);
                            }
                            else if (ChosenTypeWeight < RucksackWeight + OmniBagWeight + BundleBagWeight)
                            {
                                //  Spawn a BundleBag
                                ContainerSize Size = GetWeightedRandomSize(LootSettings.BundleBagDropSettings.SizeWeights.Where(size => BundleBag.ValidSizes.Contains(size.Key) && !IsSizeObsolete(OwnedBags, BundleBag.BundleBagTypeId, size.Key)));
                                ChosenBag = new BundleBag(Size, true);
                            }
                            else
                            {
                                ContainerSize CurrentSize = GetWeightedRandomSize(LootSettings.StandardBagDropSettings.SizeWeights);

                                //  Get all standard BagTypes that are available in the chosen size
                                List<BagType> StandardTypes = ItemBagsMod.BagConfig.BagTypes.Where(type => type.SizeSettings.Any(sizeCfg => sizeCfg.Size == CurrentSize)).ToList();

                                //  Try to force a non-obsolete bag to spawn
                                if (RollDice(LootSettings.ForceNewBagTypeChance))
                                {
                                    StandardTypes.RemoveAll(type => IsSizeObsolete(OwnedBags, type.Id, CurrentSize));

                                    //  If all bag types were obsolete, then keep incrementing the size until we find a non-obsolete bag to spawn
                                    if (!StandardTypes.Any())
                                    {
                                        List<ContainerSize> ValidSizes = LootSettings.StandardBagDropSettings.SizeWeights.Where(size => size.Value > 0).Select(size => size.Key).ToList();
                                        ContainerSize MaxSize = ValidSizes.DefaultIfEmpty(ContainerSize.Small).Max();

                                        while (CurrentSize < MaxSize && !StandardTypes.Any())
                                        {
                                            CurrentSize = ValidSizes.Where(size => size > CurrentSize).OrderBy(size => size).First();
                                            StandardTypes = ItemBagsMod.BagConfig.BagTypes.Where(type => type.SizeSettings.Any(sizeCfg => sizeCfg.Size == CurrentSize) && !IsSizeObsolete(OwnedBags, type.Id, CurrentSize)).ToList();
                                        }
                                    }
                                }

                                if (StandardTypes.Any())
                                {
                                    //  Spawn a standard BoundedBag
                                    int ChosenTypeIndex = Randomizer.Next(StandardTypes.Count);
                                    ChosenBag = new BoundedBag(StandardTypes[ChosenTypeIndex], CurrentSize, false);
                                }
                                else
                                    ChosenBag = null;
                            }

                            if (ChosenBag != null)
                                Game1.createItemDebris(ChosenBag, Game1.player.getStandingPosition(), SpawnDirection, null, -1);
                        }
                    }
                    else if (MonsterLootSettings.LogDropChancesToConsole)
                    {
                        LogMessage = string.Format("Failed drop chance: Location = {0}, monster.ExperienceGained = {1}, monster.MaxHealth = {2}\n"
                            + "BaseChance = {3} ({4}%), LocationMultiplier = {5} (+{6}%), ExpMultiplier = {7}, HPMultiplier = {8} (+{9}%), TotalChance = {10} ({11}%)",
                            __instance.Name, monster.ExperienceGained, monster.MaxHealth,
                            BaseChance, (BaseChance * 100.0).ToString("0.##"), LocationMultiplier, ((LocationMultiplier - 1.0) * 100.0).ToString("0.##"),
                            ExpMultiplier.ToString("#.####"), HPMultiplier, ((HPMultiplier - 1.0) * 100.0).ToString("0.##"), Chance, (Chance * 100.0).ToString("0.###"));
                        Monitor.Log(LogMessage, LogLevel.Info);
                    }
                }
            }
            catch (Exception ex)
            {
                Monitor.Log(string.Format("Unhandled Error in {0}:\n{1}", nameof(MonsterDrop_Postfix), ex), LogLevel.Error);
            }
        }

        private static ContainerSize GetWeightedRandomSize(IEnumerable<System.Collections.Generic.KeyValuePair<ContainerSize, int>> Weights)
        {
            if (Weights.Any(x => x.Value < 0))
                throw new InvalidOperationException("Weights cannot be negative in GetWeightedRandomSize.");

            int TotalWeight = Weights.Sum(x => x.Value);
            if (TotalWeight <= 0)
                throw new InvalidOperationException(string.Format("The sum of the weights cannot be <= 0 in GetWeightedRandomSize. Weights = {0}", string.Join(",", Weights.Select(x => x.Value))));
            int ChosenWeight = Randomizer.Next(TotalWeight);

            //  EX: If the weight are = { { Small, 3 }, { Medium, 6 }, { Large, 2 } }, picks random number from 0 to (3+6+2-1) inclusive.
            //  Then the first is selected if ChosenWeight is 0-2 (3/11 chance), second is selected if 3-8 (6/11 chance), third is selected if 9-10 (2/11 chance)
            int CurrentSum = 0;
            foreach (var KVP in Weights)
            {
                CurrentSum += KVP.Value;
                if (ChosenWeight < CurrentSum)
                {
                    return KVP.Key;
                }
            }

            throw new InvalidOperationException(string.Format("GetWeightedRandomSize did not return a value. Input weights = {0}, Chosen weight = {1}", string.Join(",", Weights.Select(x => x.Value)), ChosenWeight));
        }

        /// <summary>Returns true if there is already a bag within <paramref name="OwnedBags"/> that belongs to the given <paramref name="TypeId"/> and whose size is >= the given <paramref name="Size"/></summary>
        /// <param name="OwnedBags"></param>
        private static bool IsSizeObsolete(List<ItemBag> OwnedBags, string TypeId, ContainerSize Size)
        {
            return OwnedBags.Any(x => x.GetTypeId() == TypeId && x.Size >= Size);
        }
    }
}
