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
using StardewValley.Extensions;
using StardewValley.Monsters;

namespace AutoGrabTruffles
{
    internal class DigUpProducePatcher
    {
        #pragma warning disable CS8618 // Non-nullable field must contain a non-null value
        private static IMonitor MONITOR;
        private static AutoGrabTrufflesConfig CONFIG;
        #pragma warning restore CS8618 // Non-nullable field must contain a non-null value

        private static readonly string TRUFFLE_QUALIFIED_ID = "(O)430";
        private static readonly string AUTO_GRABBER_QUALIFIED_ID = "(BC)165";

        internal static void Initialize(IMonitor monitor, AutoGrabTrufflesConfig config)
        {
            MONITOR = monitor;
            CONFIG = config;
        }

        internal static void ApplyPatch(Harmony harmony)
        {
            harmony.Patch(
                AccessTools.Method(typeof(FarmAnimal), nameof(FarmAnimal.DigUpProduce)),
                new HarmonyMethod(typeof(DigUpProducePatcher), nameof(DigUpProduce_Prefix)));
        }

        internal static bool DigUpProduce_Prefix(FarmAnimal __instance, GameLocation location, StardewValley.Object produce)
        {
            try
            {
                Random random = Utility.CreateRandom(__instance.myID.Value / 2.0, Game1.stats.DaysPlayed, Game1.timeOfDay);
                bool isTruffleCrab = false;
                if (produce.QualifiedItemId == TRUFFLE_QUALIFIED_ID && random.NextDouble() < 0.002)
                {
                    RockCrab rockCrab = new(__instance.Tile, "Truffle Crab");
                    Vector2 vector = Utility.recursiveFindOpenTileForCharacter(rockCrab, location, __instance.Tile, 50, allowOffMap: false);
                    if (vector != Vector2.Zero)
                    {
                        rockCrab.setTileLocation(vector);
                        location.addCharacter(rockCrab);
                        isTruffleCrab = true;
                    }
                }

                if (!isTruffleCrab)
                {
                    if (!DidHarvestProduce(__instance, produce))
                    {
                        if (Utility.spawnObjectAround(Utility.getTranslatedVector2(__instance.Tile, __instance.FacingDirection, 1f), produce, __instance.currentLocation) && produce.QualifiedItemId == TRUFFLE_QUALIFIED_ID)
                        {
                            Game1.stats.TrufflesFound++;
                        }
                    }
                }

                if (!random.NextBool(__instance.friendshipTowardFarmer.Value / 1500.0)) __instance.currentProduce.Value = null;

                return false;
            }
            catch (Exception ex)
            {
                MONITOR.Log($"Failed in {nameof(DigUpProduce_Prefix)}:\n{ex.Message}", LogLevel.Error);
                return true;
            }
        }

        private static bool DidHarvestProduce(FarmAnimal animal, StardewValley.Object produce)
        {
            bool didHarvest = false;

            Item produceItem = produce.getOne();
            Random random = Utility.CreateRandom(animal.myID.Value / 2.0, Game1.stats.DaysPlayed, Game1.timeOfDay);

            bool doHarvestTwo = false;
            if (produce.QualifiedItemId.Equals(TRUFFLE_QUALIFIED_ID))
            {
                produceItem.Quality = GetTruffleQuality(random, animal.ownerID.Get());
                doHarvestTwo = DoesTruffleDropTwo(random, animal.ownerID.Get());
                if (doHarvestTwo) produceItem.Stack++;
            }

            List<StardewValley.Object> autoGrabbers = GetAutoGrabbersInAnimalHome(animal.home);

            if (autoGrabbers.Count > 0 && DidAddItemToAutoGrabberWithSpace(autoGrabbers, produceItem))
            {
                didHarvest = true;

                if (CONFIG.UpdateGameStats) {
                    Game1.stats.TrufflesFound++;
                    Game1.stats.ItemsForaged++;
                    if (doHarvestTwo) Game1.stats.ItemsForaged++;
                }

                if (CONFIG.GainExperience)
                {
                    if (CONFIG.WhoGainsExperience.Equals("Everyone"))
                    {
                        List<Farmer> farmers = Game1.getAllFarmers().ToList();
                        foreach (Farmer farmer in farmers)
                        {
                            farmer.gainExperience(Farmer.foragingSkill, 7);
                            if (doHarvestTwo) farmer.gainExperience(Farmer.foragingSkill, 7);
                        }
                    }
                    else
                    {
                        Farmer farmer = Game1.getFarmer(animal.ownerID.Get());
                        farmer.gainExperience(Farmer.foragingSkill, 7);
                        if (doHarvestTwo) farmer.gainExperience(Farmer.foragingSkill, 7);
                    }
                }
            }

            return didHarvest;
        }

        private static List<StardewValley.Object> GetAutoGrabbersInAnimalHome(StardewValley.Buildings.Building building)
        {
            List<StardewValley.Object> autoGrabbers = new();

            List<StardewValley.Object> buildingObjects = building.GetIndoors().objects.Values.ToList();
            foreach (StardewValley.Object buildingObject in buildingObjects)
            {
                if (buildingObject.QualifiedItemId.Equals(AUTO_GRABBER_QUALIFIED_ID)) autoGrabbers.Add(buildingObject);
            }

            return autoGrabbers;
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

        private static bool DidAddItemToAutoGrabberWithSpace(List<StardewValley.Object> autoGrabbers, Item item)
        {
            int i = autoGrabbers.Count - 1;
            bool itemAdded = false;
            while (i >= 0 && !itemAdded)
            {
                StardewValley.Objects.Chest autoGrabber = (StardewValley.Objects.Chest) autoGrabbers[i].heldObject.Value;
                // a returned null value means the item was added successfully
                if (autoGrabber.addItem(item) is null) itemAdded = true;
                else i--;
            }

            return itemAdded;
        }
    }
}
