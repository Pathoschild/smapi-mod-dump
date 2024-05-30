/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BirbCore.Attributes;
using MoonShared.APIs;
using Netcode;
using SpaceCore;
using SpaceCore.Events;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Quests;
using StardewValley.TerrainFeatures;

namespace LuckSkill.Core
{
    [SEvent]
    internal class Events
    {

        [SEvent.GameLaunchedLate]
        private static void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Log.Trace("Luck: Trying to Register skill.");
            SpaceCore.Skills.RegisterSkill(new Luck_Skill());


            SpaceEvents.ChooseNightlyFarmEvent += ChangeFarmEvent;
        }


        [SEvent.TimeChanged]
        private static void TimeChanged(object sender, TimeChangedEventArgs e)
        {
            LuckSkill(Game1.player);
            Log.Warn("Luck: Player luck level is: " + Game1.player.LuckLevel.ToString());
        }

        [SEvent.SaveCreated]
        private static void SaveCreated(object sender, SaveCreatedEventArgs e)
        {
            LuckSkill(Game1.player);
        }

        [SEvent.SaveLoaded]
        private static void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            LuckSkill(Game1.player);
        }

        [SEvent.DayStarted]
        private static void DayStarted(object sender, DayStartedEventArgs e)
        {
            if (!Game1.player.IsLocalPlayer)
                return;

            Farmer farmer = Game1.getFarmer(Game1.player.UniqueMultiplayerID);
            int exp = (int)(farmer.team.sharedDailyLuck.Value * ModEntry.Config.DailyLuckExpBonus);
            Utilities.AddEXP(farmer, Math.Max(0, exp));

            if (farmer.HasCustomProfession(Luck_Skill.Luck5a))
            {
                farmer.team.sharedDailyLuck.Value += 0.01;

                //The player can only ever have profession 10a1 if they have profession 5a. 
                if (farmer.HasCustomProfession(Luck_Skill.Luck10a1))
                {
                    Random r = new Random((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed * 3));
                    if (r.NextDouble() <= 0.20)
                    {
                        farmer.team.sharedDailyLuck.Value = Math.Max(farmer.team.sharedDailyLuck.Value, 0.12);
                    }
                }

                //The player can only ever have profession 10a2 if they have profession 5a. So we do this check in here.
                if (farmer.HasCustomProfession(Luck_Skill.Luck10a2))
                {
                    if (farmer.team.sharedDailyLuck.Value < 0)
                        farmer.team.sharedDailyLuck.Value = 0;
                }
            }



            if (farmer.HasCustomProfession(Luck_Skill.Luck5b) && Game1.questOfTheDay == null)
            {
                if (!(Utility.isFestivalDay(Game1.dayOfMonth, Game1.season) || Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season)))
                {
                    Quest quest = null;
                    for (uint i = 1; i < 3 && quest == null; ++i)
                    {
                        quest = GetQuestOfTheDay(i);
                    }

                    if (quest != null)
                    {
                        Log.Info($"Applying quest {quest} for today, due to having PROFESSION_MOREQUESTS.");
                        Game1.netWorldState.Value.SetQuestOfTheDay(quest);
                    }
                }
            }

            LuckSkill(farmer);
        }

        private static Quest GetQuestOfTheDay(uint seedEnhancer = 0)
        {
            if (Game1.stats.DaysPlayed <= 1)
                return null;

            double num = Utility.CreateDaySaveRandom(100.0, Game1.stats.DaysPlayed * 777, seedEnhancer * 999).NextDouble();

            if (num < 0.08)
                return new ResourceCollectionQuest();

            if (num < 0.2 && MineShaft.lowestLevelReached > 0 && Game1.stats.DaysPlayed > 5)
                return new SlayMonsterQuest();

            if (num < 0.6)
                return new ItemDeliveryQuest();

            if (num < 0.66 && Game1.shortDayNameFromDayOfSeason(Game1.dayOfMonth).Equals("Mon"))
            {
                foreach (Farmer allFarmer in Game1.getAllFarmers())
                {
                    foreach (Quest item in allFarmer.questLog)
                    {
                        if (item is SocializeQuest)
                            return new ItemDeliveryQuest();
                    }
                }
                return new SocializeQuest();
            }

            return new ItemDeliveryQuest();
        }



        private static void ChangeFarmEvent(object sender, EventArgsChooseNightlyFarmEvent args)
        {

            if (Game1.player.HasCustomProfession(Luck_Skill.Luck10b1) && !Game1.weddingToday &&
                    (args.NightEvent == null || (args.NightEvent is SoundInTheNightEvent &&
                    ModEntry.Instance.Helper.Reflection.GetField<NetInt>(args.NightEvent, "behavior").GetValue().Value == 2)))
            {
                //Log.Async("Doing event check");
                FarmEvent ev = null;
                ev = PickFarmEvent(99999);

                if (ev != null && ev.setUp())
                {
                    ev = null;
                }

                if (ev != null)
                {
                    Log.Info($"Applying {ev} as tonight's nightly event, due to having PROFESSION_NIGHTLY_EVENTS");
                    args.NightEvent = ev;
                }
            }
        }

        private static FarmEvent PickFarmEvent(int seedEnhancer)
        {
            Random random = new Random((int)Game1.stats.DaysPlayed + (int)Game1.uniqueIDForThisGame / 2 + seedEnhancer);

            if (Game1.weddingToday || Game1.getOnlineFarmers().Any(farmer => farmer.GetSpouseFriendship()?.IsMarried() ?? false))
                return null;

            if (Game1.stats.DaysPlayed == 31U)
                return new SoundInTheNightEvent(4);

            var mailForTomorrow = Game1.player.mailForTomorrow;
            switch (mailForTomorrow)
            {
                case var mail when mail.Contains("jojaPantry"):
                    return new WorldChangeEvent(0);
                case var mail when mail.Contains("ccPantry"):
                    return new WorldChangeEvent(1);
                case var mail when mail.Contains("jojaVault"):
                    return new WorldChangeEvent(6);
                case var mail when mail.Contains("ccVault"):
                    return new WorldChangeEvent(7);
                case var mail when mail.Contains("jojaBoilerRoom"):
                    return new WorldChangeEvent(2);
                case var mail when mail.Contains("ccBoilerRoom"):
                    return new WorldChangeEvent(3);
                case var mail when mail.Contains("jojaCraftsRoom"):
                    return new WorldChangeEvent(4);
                case var mail when mail.Contains("ccCraftsRoom"):
                    return new WorldChangeEvent(5);
                case var mail when mail.Contains("jojaFishTank"):
                    return new WorldChangeEvent(8);
                case var mail when mail.Contains("ccFishTank"):
                    return new WorldChangeEvent(9);
                case var mail when mail.Contains("ccMovieTheaterJoja"):
                    return new WorldChangeEvent(10);
                case var mail when mail.Contains("ccMovieTheater"):
                    return new WorldChangeEvent(11);
            }

            if (Game1.MasterPlayer.eventsSeen.Contains("191393") && (Game1.isRaining || Game1.isLightning) && (!Game1.MasterPlayer.mailReceived.Contains("abandonedJojaMartAccessible") && !Game1.MasterPlayer.mailReceived.Contains("ccMovieTheater")))
                return new WorldChangeEvent(12);

            if (random.NextDouble() < 0.01 && !Game1.currentSeason.Equals("winter"))
                return new FairyEvent();
            if (random.NextDouble() < 0.01)
                return new WitchEvent();
            if (random.NextDouble() < 0.01)
                return new SoundInTheNightEvent(1);
            if (random.NextDouble() < 0.01 && Game1.year > 1)
                return new SoundInTheNightEvent(0);
            if (random.NextDouble() < 0.01)
                return new SoundInTheNightEvent(3);

            return null;
        }

        [SEvent.DayEnding]
        private void OnDayEnding(object sender, DayEndingEventArgs args)
        {
            if (!Game1.player.IsLocalPlayer)
                return;

            var farmer = Game1.getFarmer(Game1.player.UniqueMultiplayerID);

            if (!farmer.HasCustomProfession(Luck_Skill.Luck10b2))
                return;

            Random random = new Random((int)(Game1.uniqueIDForThisGame + Game1.stats.DaysPlayed));
            int rolls = 0;

            foreach (var data in farmer.friendshipData.Values)
            {
                if (data.GiftsToday > 0)
                    rolls++;
            }

            while (rolls-- > 0)
            {
                if (random.NextDouble() <= 0.15)
                {
                    void AdvanceCrops()
                    {
                        List<GameLocation> locs = new List<GameLocation>();
                        locs.AddRange(Game1.locations);
                        for (int i = 0; i < locs.Count; ++i)
                        {
                            GameLocation loc = locs[i];
                            if (loc == null) // From buildings without a valid indoors
                                continue;
                            if (loc.IsBuildableLocation())
                                locs.AddRange(loc.buildings.Select(b => b.indoors.Value));

                            foreach (var entry in loc.objects.Pairs.ToList())
                            {
                                var obj = entry.Value;
                                if (obj is IndoorPot pot)
                                {
                                    var dirt = pot.hoeDirt.Value;
                                    if (dirt.crop == null || dirt.crop.fullyGrown.Value)
                                        continue;

                                    dirt.crop.newDay(1); //1 is the state for watered, check isWatered() to see
                                }
                            }

                            foreach (var entry in loc.terrainFeatures.Pairs.ToList())
                            {
                                var tf = entry.Value;
                                if (tf is HoeDirt dirt)
                                {
                                    if (dirt.crop == null || dirt.crop.fullyGrown.Value)
                                        continue;

                                    dirt.crop.newDay(1);
                                }
                                else if (tf is FruitTree ftree)
                                {
                                    ftree.dayUpdate();
                                }
                                else if (tf is Tree tree)
                                {
                                    tree.dayUpdate();
                                }
                            }
                        }

                        Game1.showGlobalMessage(ModEntry.Instance.I18N.Get("JunimoRewards_GrowCrops"));
                    }

                    void AdvanceBarn(AnimalHouse house)
                    {
                        foreach (var animal in house.Animals.Values)
                        {
                            animal.friendshipTowardFarmer.Value = Math.Min(1000, animal.friendshipTowardFarmer.Value + 100);
                        }

                        Game1.showGlobalMessage(ModEntry.Instance.I18N.Get("JunimoRewards_AnimalFriendship"));
                    }

                    void GrassAndFences()
                    {
                        var farm = Game1.getFarm();
                        foreach (var entry in farm.terrainFeatures.Values)
                        {
                            if (entry is Grass grass)
                            {
                                grass.numberOfWeeds.Value = 4;
                            }
                        }

                        foreach (var entry in farm.Objects.Values)
                        {
                            if (entry is Fence fence)
                            {
                                fence.repair();
                            }
                        }

                        Game1.showGlobalMessage(ModEntry.Instance.I18N.Get("JunimoRewards_GrowGrass"));
                    }

                    if (random.NextDouble() <= 0.05 && Game1.player.addItemToInventoryBool(new StardewValley.Object(StardewValley.Object.prismaticShardID, 1)))
                    {
                        Game1.showGlobalMessage(ModEntry.Instance.I18N.Get("JunimoRewards_PrismaticShard"));
                        continue;
                    }

                    List<Action> choices = new List<Action> { AdvanceCrops, AdvanceCrops, AdvanceCrops };

                    foreach (var loc in Game1.locations)
                    {
                        if (loc.IsBuildableLocation())
                        {
                            foreach (var building in loc.buildings)
                            {
                                if (building.indoors.Value is AnimalHouse ah)
                                {
                                    bool foundAnimalWithRoomForGrowth = false;

                                    foreach (var animal in ah.Animals.Values)
                                    {
                                        if (animal.friendshipTowardFarmer.Value < 1000)
                                        {
                                            foundAnimalWithRoomForGrowth = true;
                                            break;
                                        }
                                    }

                                    if (foundAnimalWithRoomForGrowth)
                                        choices.Add(() => AdvanceBarn(ah));
                                }
                            }
                        }
                    }

                    choices.Add(GrassAndFences);
                    choices[random.Next(choices.Count)]();
                    break;
                }
            }
        }



        private static void LuckSkill(Farmer farmer)
        {
            if (farmer != null && farmer.IsLocalPlayer)
            {
                if (farmer.modDataForSerialization.TryGetValue("moonslime.LuckSkill.skillValue", out string storedSkillValue))
                {
                    int storedSkillLevel = int.Parse(storedSkillValue);
                    int currentSkillLevel = Utilities.GetLevel(farmer);

                    if (currentSkillLevel != storedSkillLevel)
                    {
                        farmer.luckLevel.Value -= storedSkillLevel;
                        farmer.luckLevel.Value += currentSkillLevel;
                        farmer.modDataForSerialization["moonslime.LuckSkill.skillValue"] = currentSkillLevel.ToString();
                    }
                }
                else
                {
                    farmer.modDataForSerialization["moonslime.LuckSkill.skillValue"] = Utilities.GetLevel(farmer).ToString();
                }
            }
        }
    }
}
