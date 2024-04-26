/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Hunter-Chambers/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;

namespace AutoGrabTruffles
{
    internal class DigUpProducePatcher
    {
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value
        private static IMonitor MONITOR;
        private static AutoGrabTrufflesConfig CONFIG;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value

        internal static void Initialize(IMonitor monitor, AutoGrabTrufflesConfig config)
        {
            MONITOR = monitor;
            CONFIG = config;
        }

        internal static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.DigUpProduce)),
                null,
                new HarmonyMethod(typeof(DigUpProducePatcher), nameof(DigUpProduce_Postfix)));
        }

        internal static void DigUpProduce_Postfix(FarmAnimal __instance)
        {
            try
            {
                IEnumerator<KeyValuePair<Vector2, StardewValley.Object>> farmObjects = Game1.getFarm().Objects.Pairs.ToList().GetEnumerator();
                bool found = false;

                while (!found && farmObjects.MoveNext())
                {
                    if (farmObjects.Current.Value.ParentSheetIndex == 430) // 430 = truffle
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    Item truffle = farmObjects.Current.Value;
                    Vector2 tilePosition = farmObjects.Current.Key;
                    TryHarvestTruffle(__instance, truffle, tilePosition);
                }
            }
            catch (Exception e)
            {
                MONITOR.Log($"Failed in {nameof(DigUpProduce_Postfix)}:\n{e}", LogLevel.Error);
            }
        }

        private static void TryHarvestTruffle(FarmAnimal pig, Item originalTruffle, Vector2 tileLocation)
        {
            Item truffle = originalTruffle.getOne();

            Random random = Utility.CreateDaySaveRandom();
            truffle.Quality = GetTruffleQuality(random, pig.ownerID.Get());
            bool doDropTwo = DoesTruffleDropTwo(random, pig.ownerID.Get());
            if (doDropTwo)
            {
                truffle.Stack++;
            }

            List<StardewValley.Object> autoGrabbers = GetBarnAutoGrabbers(pig.home);
            if (autoGrabbers.Count > 0 && DidAddItemToAutoGrabberWithSpace(autoGrabbers, truffle))
            {
                Game1.getFarm().removeObject(tileLocation, false);

                if (CONFIG.UpdateGameStats) {
                    Game1.stats.ItemsForaged++;
                    if (doDropTwo)
                    {
                        Game1.stats.ItemsForaged++;
                    }
                }

                if (CONFIG.GainExperience)
                {
                    if (CONFIG.WhoGainsExperience.Equals("Everyone"))
                    {
                        List<Farmer> farmers = Game1.getAllFarmers().ToList();
                        foreach (Farmer farmer in farmers)
                        {
                            if (doDropTwo)
                            {
                                farmer.gainExperience(Farmer.foragingSkill, 14);
                            }
                            else
                            {
                                farmer.gainExperience(Farmer.foragingSkill, 7);
                            }
                        }
                    }
                    else
                    {
                        Farmer farmer = Game1.getFarmer(pig.ownerID.Get());
                        if (doDropTwo)
                        {
                            farmer.gainExperience(Farmer.foragingSkill, 14);
                        }
                        else
                        {
                            farmer.gainExperience(Farmer.foragingSkill, 7);
                        }
                    }
                }
            }
        }

        private static int GetTruffleQuality(Random random, long pigOwnerID)
        {
            bool hasBotanist;
            int foragingLevel;
            if (!CONFIG.ApplyBotanistProfession)
            {
                hasBotanist = false;
                Farmer farmer = Game1.getFarmer(pigOwnerID);
                foragingLevel = farmer.ForagingLevel;
            }
            else if (CONFIG.WhoseBotanistProfessionToUse.Equals("Anyone"))
            {
                List<Farmer> farmers = Game1.getAllFarmers().ToList();
                int i = farmers.Count - 1;
                hasBotanist = false;
                while (i >= 0 && !hasBotanist)
                {
                    if (farmers[i].professions.Contains(Farmer.botanist)) {
                        hasBotanist = true;
                    }
                    else
                    {
                        i--;
                    }
                }
                foragingLevel = 0;
                foreach (Farmer farmer in Game1.getAllFarmers())
                {
                    if (farmer.GetSkillLevel(Farmer.foragingSkill) > foragingLevel)
                    {
                        foragingLevel = farmer.GetSkillLevel(Farmer.foragingSkill);
                    }
                }
            }
            else
            {
                Farmer farmer = Game1.getFarmer(pigOwnerID);
                hasBotanist = farmer.professions.Contains(Farmer.botanist);
                foragingLevel = farmer.ForagingLevel;
            }


            float goldChance = foragingLevel / 30f;
            float silverChance = (1 - goldChance) * (foragingLevel / 15);

            if (hasBotanist)
            {
                return StardewValley.Object.bestQuality;
            }
            else if (random.NextDouble() < goldChance)
            {
                return StardewValley.Object.highQuality;
            }
            else if (random.NextDouble() < silverChance)
            {
                return StardewValley.Object.medQuality;
            }
            else
            {
                return StardewValley.Object.lowQuality;
            }
        }

        private static bool DoesTruffleDropTwo(Random random, long pigOwnerID)
        {
            if (!CONFIG.ApplyGathererProfession)
            {
                return false;
            }

            bool hasGatherer;
            if (CONFIG.WhoseGathererProfessionToUse.Equals("Anyone"))
            {
                List<Farmer> farmers = Game1.getAllFarmers().ToList();
                int i = farmers.Count - 1;
                hasGatherer = false;
                while (i >= 0 && !hasGatherer)
                {
                    if (farmers[i].professions.Contains(Farmer.gatherer))
                    {
                        hasGatherer = true;
                    }
                    else
                    {
                        i--;
                    }
                }
            }
            else
            {
                Farmer farmer = Game1.getFarmer(pigOwnerID);
                hasGatherer = farmer.professions.Contains(Farmer.gatherer);
            }

            return hasGatherer && random.NextDouble() < 0.2;
        }

        private static List<StardewValley.Object> GetBarnAutoGrabbers(StardewValley.Buildings.Building barn)
        {
            List<StardewValley.Object> autoGrabbers = new();

            List<StardewValley.Object> barnObjects = barn.GetIndoors().objects.Values.ToList();
            foreach (StardewValley.Object barnObject in barnObjects)
            {
                if (barnObject.ParentSheetIndex == 165) // 165 = auto-grabber
                {
                    autoGrabbers.Add(barnObject);
                }
            }

            return autoGrabbers;
        }

        private static bool DidAddItemToAutoGrabberWithSpace(List<StardewValley.Object> autoGrabbers, Item item)
        {
            int i = autoGrabbers.Count - 1;
            bool itemAdded = false;
            while (i >= 0 && !itemAdded)
            {
                StardewValley.Objects.Chest autoGrabber = (StardewValley.Objects.Chest) autoGrabbers[i].heldObject.Value;
                if (autoGrabber.addItem(item) == null) // a returned null value means the item was added successfully
                {
                    itemAdded = true;
                }
                else
                {
                    i--;
                }
            }

            return itemAdded;
        }
    }
}
