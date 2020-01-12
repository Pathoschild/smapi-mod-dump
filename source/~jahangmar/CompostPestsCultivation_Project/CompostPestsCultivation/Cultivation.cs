//Copyright (c) 2019 Jahangmar

//This program is free software: you can redistribute it and/or modify
//it under the terms of the GNU Lesser General Public License as published by
//the Free Software Foundation, either version 3 of the License, or
//(at your option) any later version.

//This program is distributed in the hope that it will be useful,
//but WITHOUT ANY WARRANTY; without even the implied warranty of
//MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
//GNU Lesser General Public License for more details.

//You should have received a copy of the GNU Lesser General Public License
//along with this program. If not, see <https://www.gnu.org/licenses/>.

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using xTile.Dimensions;

namespace CompostPestsCultivation
{
    public class Cultivation : ModComponent
    {
        public static Dictionary<int, List<CropTrait>> CropTraits = new Dictionary<int, List<CropTrait>>();
        public static Dictionary<int, int> CropSeeds = new Dictionary<int, int>();

        private static Config config;

        public static void Init(Config conf)
        {
            config = conf;
        }

        public static void Load(SaveData data)
        {
            CropTraits = data.CropTraits;
            CropSeeds = data.CropSeeds;
            TempQualityFertilizer = data.TempQualityFertilizer;
            TempQualityIIFertilizer = data.TempQualityIIFertilizer;
            TempWaterStop = data.TempWaterStop;
            TempWaterOneDay = data.TempWaterOneDay;
            TempWaterTwoDays = data.TempWaterTwoDays;

            ModEntry.GetMonitor().Log("Cultivation.Load() executed", LogLevel.Trace);
            ModEntry.GetMonitor().Log($"loaded {CropTraits.Count} crop traits and {CropSeeds.Count} crop seeds", LogLevel.Trace);
        }

        public static void Save(SaveData data)
        {
            data.CropTraits = CropTraits;
            data.CropSeeds = CropSeeds;
            data.TempQualityFertilizer = TempQualityFertilizer;
            data.TempQualityIIFertilizer = TempQualityIIFertilizer;
            data.TempWaterStop = TempWaterStop;
            data.TempWaterOneDay = TempWaterOneDay;
            data.TempWaterTwoDays = TempWaterTwoDays;

            ModEntry.GetMonitor().Log("Cultivation.Save() executed", LogLevel.Trace);
        }

        /// <param name="id">ParentSheetIndex of seeds</param>
        public static void NewSeeds(int id)
        {
            string seedName = Game1.objectInformation[id].Split('/')[0];
            ModEntry.GetMonitor().Log($"Processing new seeds with id {id} and name {seedName}", LogLevel.Trace);

            //ModEntry.GetMonitor().Log("checking key in cropseeds...");
            if (id == Crop.mixedSeedIndex || new Crop(id, 0, 0).isWildSeedCrop())
                return;

            if (CropSeeds.ContainsKey(id))
                CropSeeds[id] += 1;
            else
                CropSeeds.Add(id, 1);

            //ModEntry.GetMonitor().Log("checking if max is reached...");
            if (CropSeeds[id] >= config.processed_crops_for_cultivation_level)
            {
                if (!CropTraits.ContainsKey(id))
                    CropTraits.Add(id, new List<CropTrait>());

                CropSeeds[id] = 0;

                List<CropTrait> traits;
                CropTraits.TryGetValue(id, out traits);

                if (traits.Count >= System.Enum.GetValues(typeof(CropTrait)).Length)
                {
                    ModEntry.GetMonitor().Log($"tried to add trait to {CropSeedsDisplayName(id)}, but traits are maxed", LogLevel.Trace);
                    return;
                }

                AddTrait(GetNewRandomTrait(traits), traits, id);

            }
        }

        public static bool CropCanBeHarvested(Crop crop) =>
            crop != null && crop.currentPhase.Value >= crop.phaseDays.Count - 1 && (!crop.fullyGrown || crop.dayOfCurrentPhase.Value <= 0);

        private static CropTrait GetNewRandomTrait(List<CropTrait> traits)
        {
            CropTrait result = CropTrait.PestResistanceI;
            for (int i = 0; i < 100; i++)
            {
                result = GetRandomTrait();
                if (!traits.Contains(result))
                    break;
            }
            return result;
        }

        private static CropTrait GetRandomTrait()
        {
            List<CropTrait> traits = new List<CropTrait>()
            {
                CropTrait.PestResistanceI,
                CropTrait.PestResistanceII,
                CropTrait.QualityI,
                CropTrait.QualityII,
                CropTrait.SpeedI,
                CropTrait.SpeedII,
                CropTrait.WaterI,
                CropTrait.SpeedII
            };
            return traits[GetRandomInt(traits.Count)];
        }

        private static string CropSeedsDisplayName(int id) => ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, id, 1).DisplayName;

        public static string GetTraitDescr(CropTrait trait)
        {
            ITranslationHelper trans = ModEntry.GetHelper().Translation;
            switch (trait)
            {
                case CropTrait.PestResistanceNo:
                case CropTrait.PestResistanceI:
                case CropTrait.PestResistanceII:
                    return trans.Get("cult.msg_traitdesc_pestres");
                case CropTrait.QualityNo:
                case CropTrait.QualityI:
                case CropTrait.QualityII:
                    return trans.Get("cult.msg_traitdesc_quality");
                case CropTrait.WaterNo:
                case CropTrait.WaterI:
                case CropTrait.WaterII:
                case CropTrait.WaterIII:
                    return trans.Get("cult.msg_traitdesc_water");
                case CropTrait.SpeedNo:
                case CropTrait.SpeedI:
                case CropTrait.SpeedII:
                case CropTrait.SpeedIII:
                    return trans.Get("cult.msg_traitdesc_speed");
                default:
                    return "ERROR gettraitdescr";
            }
        }

        public static string GetTraitLongDescr(CropTrait trait)
        {
            ITranslationHelper trans = ModEntry.GetHelper().Translation;
            switch (trait)
            {
                case CropTrait.PestResistanceNo:
                    return trans.Get("cult.msg_traitlongdesc_pestresno");
                case CropTrait.PestResistanceI:
                    return trans.Get("cult.msg_traitlongdesc_pestresI");
                case CropTrait.PestResistanceII:
                    return trans.Get("cult.msg_traitlongdesc_pestresII");
                case CropTrait.QualityNo:
                    return trans.Get("cult.msg_traitlongdesc_qualityno");
                case CropTrait.QualityI:
                    return trans.Get("cult.msg_traitlongdesc_qualityI");
                case CropTrait.QualityII:
                    return trans.Get("cult.msg_traitlongdesc_qualityII");
                case CropTrait.WaterNo:
                    return trans.Get("cult.msg_traitlongdesc_waterno");
                case CropTrait.WaterI:
                    return trans.Get("cult.msg_traitlongdesc_waterI");
                case CropTrait.WaterII:
                    return trans.Get("cult.msg_traitlongdesc_waterII");
                case CropTrait.WaterIII:
                    return trans.Get("cult.msg_traitlongdesc_waterIII");
                case CropTrait.SpeedNo:
                    return trans.Get("cult.msg_traitlongdesc_speedno");
                case CropTrait.SpeedI:
                    return trans.Get("cult.msg_traitlongdesc_speedI");
                case CropTrait.SpeedII:
                    return trans.Get("cult.msg_traitlongdesc_speedII");
                case CropTrait.SpeedIII:
                    return trans.Get("cult.msg_traitlongdesc_speedIII");
                default:
                    return "ERROR gettraitlongdescr";
            }
        }

        public static string GetTraitName(CropTrait trait)
        {
            ITranslationHelper trans = ModEntry.GetHelper().Translation;
            switch (trait)
            {
                case CropTrait.PestResistanceNo:
                    return trans.Get("cult.msg_trait_pestresno");
                case CropTrait.PestResistanceI:
                    return trans.Get("cult.msg_trait_pestresI");
                case CropTrait.PestResistanceII:
                    return trans.Get("cult.msg_trait_pestresII");
                case CropTrait.QualityNo:
                    return trans.Get("cult.msg_trait_qualityno");
                case CropTrait.QualityI:
                    return trans.Get("cult.msg_trait_qualityI");
                case CropTrait.QualityII:
                    return trans.Get("cult.msg_trait_qualityII");
                case CropTrait.WaterNo:
                    return trans.Get("cult.msg_trait_waterno");
                case CropTrait.WaterI:
                    return trans.Get("cult.msg_trait_waterI");
                case CropTrait.WaterII:
                    return trans.Get("cult.msg_trait_waterII");
                case CropTrait.WaterIII:
                    return trans.Get("cult.msg_trait_waterIII");
                case CropTrait.SpeedNo:
                    return trans.Get("cult.msg_trait_speedno");
                case CropTrait.SpeedI:
                    return trans.Get("cult.msg_trait_speedI");
                case CropTrait.SpeedII:
                    return trans.Get("cult.msg_trait_speedII");
                case CropTrait.SpeedIII:
                    return trans.Get("cult.msg_trait_speedIII");
                default:
                    return "ERROR gettraitname";
            }
        }

        public static void AddTrait(CropTrait trait, List<CropTrait> traits, int id)
        {
            ITranslationHelper trans = ModEntry.GetHelper().Translation;

            void add()
            {
                ModEntry.GetMonitor().Log($"Added {trait} to {CropSeedsDisplayName(id)}", LogLevel.Trace);
                Game1.showGlobalMessage(trans.Get("cult.msg_traitinc", new { traitdesc = GetTraitDescr(trait), seed = CropSeedsDisplayName(id), trait = GetTraitName(trait) }));
                Game1.playSound("achievement");
                traits.Add(trait);
                CropTraits[id] = traits;
            }

            bool alreadyContains() => traits.Contains(trait);

            CropTrait newTrait = GetNewRandomTrait(traits);
            if (traits.Contains(newTrait))
            {
                ModEntry.GetMonitor().Log("Bug: tried to find new trait with full list or returned wrong trait", LogLevel.Error);
                return;
            }

            if (alreadyContains())
            {
                switch (trait)
                {
                    case CropTrait.PestResistanceI:
                        AddTrait(CropTrait.PestResistanceII, traits, id);
                        break;
                    case CropTrait.WaterI:
                        AddTrait(CropTrait.WaterII, traits, id);
                        break;
                    case CropTrait.QualityI:
                        AddTrait(CropTrait.QualityII, traits, id);
                        break;
                    case CropTrait.SpeedI:
                        AddTrait(CropTrait.SpeedII, traits, id);
                        break;
                    default:
                        AddTrait(newTrait, traits, id);
                        break;
                }
            }
            else
            {
                switch (trait)
                {
                    case CropTrait.PestResistanceII:
                        if (traits.Contains(CropTrait.PestResistanceI))
                            add();
                        else
                        {
                            trait = CropTrait.PestResistanceI;
                            add();
                        }
                        break;
                    case CropTrait.WaterII:
                        if (traits.Contains(CropTrait.WaterI))
                            add();
                        else
                        {
                            trait = CropTrait.WaterI;
                            add();
                        }

                        break;
                    case CropTrait.QualityII:
                        if (traits.Contains(CropTrait.QualityI))
                            add();
                        else
                        {
                            trait = CropTrait.QualityI;
                            add();
                        }

                        break;
                    case CropTrait.SpeedII:
                        if (traits.Contains(CropTrait.SpeedI))
                            add();
                        else
                        {
                            trait = CropTrait.SpeedI;
                            add();
                        }
                        break;
                    default:
                        add();
                        break;
                }
            }

        }

        public const string FarmName = "Farm";
        public const string GreenhouseName = "Greenhouse";
        private static Dictionary<string, List<Vector2>> TempQualityFertilizer = new Dictionary<string, List<Vector2>>() { { FarmName, new List<Vector2>() }, { GreenhouseName, new List<Vector2>() } },
                              TempQualityIIFertilizer = new Dictionary<string, List<Vector2>>() { { FarmName, new List<Vector2>() }, { GreenhouseName, new List<Vector2>() } };
        private static Dictionary<string, List<Vector2>> TempWaterOneDay = new Dictionary<string, List<Vector2>>() { { FarmName, new List<Vector2>() }, { GreenhouseName, new List<Vector2>() } },
                              TempWaterTwoDays = new Dictionary<string, List<Vector2>>() { { FarmName, new List<Vector2>() }, { GreenhouseName, new List<Vector2>() } },
                                TempWaterStop = new Dictionary<string, List<Vector2>>() { { FarmName, new List<Vector2>() }, { GreenhouseName, new List<Vector2>() } };
        /*                       
        private static void AddToTempDic(string s, Vector2 vec, Dictionary<string, List<Vector2>> dic)
        {
            dic[s].Add(vec);
        }

        private static List<Vector2> GetFromTempDic(string s, Dictionary<string, List<Vector2>> dic) =>
            dic[s];

        private static void ClearTempDic(string s, Dictionary<string, List<Vector2>> dic)
        {
            dic[s].Clear();
        }
        */
        private static int GetQuality(Vector2 tile, List<CropTrait> traits, bool greenhouse)
        {
            int level = 0;
            if (!greenhouse && Composting.AffectedByCompost(tile))
                level += 1;
            if (traits.Contains(CropTrait.QualityI))
                level += 1;
            if (traits.Contains(CropTrait.QualityII) && level <= 1)
                level += 1;
            return level;
        }

        public static int GetPestRes(Vector2 tile, List<CropTrait> traits, bool greenhouse)
        {
            int level = 0;
            if (greenhouse || Composting.AffectedByCompost(tile))
                level += 1;
            if (greenhouse || traits.Contains(CropTrait.PestResistanceI))
                level += 1;
            if (traits.Contains(CropTrait.PestResistanceII) && level <= 1)
                level += 1;
            return level;
        }

        private static int GetWater(Vector2 tile, List<CropTrait> traits, bool greenhouse)
        {
            int level = 0;
            if (greenhouse || Composting.AffectedByCompost(tile))
                level += 1;
            if (traits.Contains(CropTrait.WaterI))
                level += 1;
            if (traits.Contains(CropTrait.WaterII))
                level += 1;
            return level;
        }

        private static int GetSpeed(Vector2 tile, List<CropTrait> traits, bool greenhouse)
        {
            int level = 0;
            if (greenhouse || Composting.AffectedByCompost(tile))
                level += 1;
            if (traits.Contains(CropTrait.SpeedI))
                level += 1;
            if (traits.Contains(CropTrait.SpeedII))
                level += 1;
            return level;
        }

        public static bool UngrowCrop(Crop crop, Vector2 pos, string src, int dec = 2)
        {
            if (crop.dead.Value)
                return true;

            if (crop.currentPhase == crop.phaseDays.Count - 1)
                return false;

            ModEntry.GetMonitor().Log($"Before: Crop at {pos} has day/phase ({crop.dayOfCurrentPhase}/{crop.currentPhase})", LogLevel.Trace);

            int rem = (dec - crop.dayOfCurrentPhase.Value) - 1; //if phase is decreased, there might be some remaining days that have to be subtracted below (-1 due to the phase that is already taken into account)

            crop.dayOfCurrentPhase.Value -= dec;
            //ModEntry.GetMonitor().Log(src + " decreased day of current phase of crop at " + pos, LogLevel.Trace);
            if (crop.dayOfCurrentPhase.Value < 0)
            {
                crop.currentPhase.Value -= 1;
                if (crop.currentPhase.Value < 0)
                {
                    ModEntry.GetMonitor().Log(src + " killed crop at " + pos, LogLevel.Trace);
                    crop.dead.Value = true;
                    crop.currentPhase.Value = 0;
                }
                else
                {
                    crop.dayOfCurrentPhase.Value = crop.phaseDays.Count > 0 ? crop.phaseDays[crop.currentPhase.Value] - (rem) : 0;
                    ModEntry.GetMonitor().Log(src + $" decreased day/phase of crop at " + pos, LogLevel.Trace);
                }

            }
            ModEntry.GetMonitor().Log($"After: Crop at {pos} has day/phase ({crop.dayOfCurrentPhase}/{crop.currentPhase})", LogLevel.Trace);
            return crop.dead.Value;
        }

        public static List<CropTrait> GetTraits(StardewValley.Object seeds)
        {
            if (seeds.Category == StardewValley.Object.SeedsCategory)
            {
                if (!Cultivation.CropTraits.TryGetValue(seeds.ParentSheetIndex, out List<CropTrait> traits))
                {
                    traits = new List<CropTrait>();
                }
                return traits;
            }
            return new List<CropTrait>();
        }

        public static List<CropTrait> GetTraits(Crop crop)
        {
            int seedIdx = GetSeedIdxFromCrop(crop);
            if (!Cultivation.CropTraits.TryGetValue(seedIdx, out List<CropTrait> traits))
            {
                traits = new List<CropTrait>();
                //ModEntry.GetMonitor().Log($"No traits for idx {seedIdx} found", LogLevel.Trace);
            }
            else
            {
                //ModEntry.GetMonitor().Log($"Traits for idx {seedIdx} found", LogLevel.Trace);
            }
            return traits;
        }

        public static StardewValley.Object GetCropItemFromSeeds(StardewValley.Object seeds)
        {
            //maps seed idx -> info containing crop idx
            Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
            foreach (KeyValuePair<int, string> item in dictionary)
            {
                if (item.Key == seeds.ParentSheetIndex)
                {
                    Item it = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, System.Convert.ToInt32(item.Value.Split('/')[3]), 1);
                    return it as StardewValley.Object;
                }
            }
            return null;
        }

        public static int GetSeedIdxFromCrop(Crop crop)
        {
            //maps seed idx -> info containing crop idx
            Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
            foreach (KeyValuePair<int, string> item in dictionary)
            {
                int seedIdx = item.Key;
                int cropIdx = System.Convert.ToInt32(item.Value.Split('/')[3]);
                if (cropIdx == crop.indexOfHarvest.Value)
                {
                    return seedIdx;
                }
            }
            return -1;
        }

        public static void OnEndDay()
        {

            OnEndDayForLocation(Game1.getFarm());
            OnEndDayForLocation(Game1.getLocationFromName(GreenhouseName));

            void OnEndDayForLocation(GameLocation location)
            {
                string locationName = FarmName;
                if (location.IsFarm)
                    locationName = FarmName;
                else if (location.IsGreenhouse)
                    locationName = GreenhouseName;

                ModEntry.GetMonitor().Log("Running Cultivation.OnEndDay for " + locationName, LogLevel.Trace);

                foreach (KeyValuePair<Vector2, TerrainFeature> pair in location.terrainFeatures.Pairs)
                {
                    if (pair.Key is Vector2 vec && pair.Value is HoeDirt hd && hd.crop is Crop crop && crop != null)
                    {
                        if (crop.isWildSeedCrop())
                        {

                        }
                        else
                        {
                            List<CropTrait> traits = GetTraits(crop);

                            //ModEntry.GetMonitor().Log($"Traits for {vec}: ");
                            //traits.ForEach((CropTrait obj) => ModEntry.GetMonitor().Log(obj.ToString()));
                            //ModEntry.GetMonitor().Log($"GetWater returns "+GetWater(vec, traits, location.IsGreenhouse));

                            switch (GetQuality(vec, traits, location.IsGreenhouse))
                            {
                                case 0: //normal
                                    break;
                                case 1: //better
                                    if (hd.fertilizer.Value == HoeDirt.noFertilizer)
                                    {
                                        TempQualityFertilizer[locationName].Add(vec);
                                        hd.fertilizer.Value = HoeDirt.fertilizerLowQuality;
                                    }
                                    break;
                                case 2: //best
                                    if (hd.fertilizer.Value == HoeDirt.noFertilizer)
                                    {
                                        TempQualityIIFertilizer[locationName].Add(vec);
                                        hd.fertilizer.Value = HoeDirt.fertilizerHighQuality;
                                    }
                                    break;
                                default:
                                    ModEntry.GetMonitor().Log("Bug: GetQuality returned wrong value", LogLevel.Error);
                                    break;
                            }

                            switch (GetSpeed(vec, traits, location.IsGreenhouse))
                            {
                                case 0: //reduced
                                    if (GetRandomInt(100) <= config.minimal_speed_ungrow_chance)
                                        UngrowCrop(hd.crop, vec, "lacking speed trait or compost");
                                    break;
                                case 1: //normal
                                    break;
                                case 2: //better
                                    if (GetRandomInt(100) <= config.speed_i_trait_grow_chance)
                                        hd.crop.newDay(HoeDirt.watered, HoeDirt.noFertilizer, (int)vec.X, (int)vec.Y, location);
                                    break;
                                case 3: //best
                                    if (GetRandomInt(100) <= config.speed_ii_trait_grow_chance)
                                        hd.crop.newDay(HoeDirt.watered, HoeDirt.noFertilizer, (int)vec.X, (int)vec.Y, location);
                                    break;
                                default:
                                    ModEntry.GetMonitor().Log("Bug: GetSpeed returned wrong value", LogLevel.Error);
                                    break;
                            }

                            switch (GetWater(vec, traits, location.IsGreenhouse))
                            {
                                case 0: //reduced
                                    if (hd.state.Value != HoeDirt.watered)
                                        UngrowCrop(hd.crop, vec, "lacking water trait or compost");
                                    break;
                                case 1: //normal
                                    break;
                                case 2: //better
                                    if (hd.state.Value == HoeDirt.watered && !TempWaterOneDay[locationName].Contains(vec) && !TempWaterStop[locationName].Contains(vec))
                                        TempWaterOneDay[locationName].Add(vec);
                                    break;
                                case 3: //best
                                    if (hd.state.Value == HoeDirt.watered && !TempWaterOneDay[locationName].Contains(vec) && !TempWaterStop[locationName].Contains(vec))
                                        TempWaterTwoDays[locationName].Add(vec);
                                    break;
                                default:
                                    ModEntry.GetMonitor().Log("Bug: GetWater returned wrong value", LogLevel.Error);
                                    break;
                            }
                        }

                    }
                }

            }
        }

        public static void OnNewDay()
        {

            OnNewDayForLocation(Game1.getFarm());
            OnNewDayForLocation(Game1.getLocationFromName(GreenhouseName));

            void OnNewDayForLocation(GameLocation location)
            {
                string locationName = FarmName;
                if (location.IsFarm)
                    locationName = FarmName;
                else if (location.IsGreenhouse)
                    locationName = GreenhouseName;

                ModEntry.GetMonitor().Log("Running Cultivation.OnNewDay for " + locationName, LogLevel.Trace);

                //Spawn weeds
                if (location.IsFarm && !Game1.IsWinter)
                {
                    List<Vector2> adjacents = new List<Vector2>();
                    foreach (Vector2 compostTile in Composting.CompostAppliedDays.Keys)
                    {
                        adjacents.AddRange(GetAdjacentTiles(compostTile));
                        adjacents.Add(compostTile);
                    }

                    foreach (Vector2 vec in adjacents)
                    {
                        if (CheckChance(Game1.wasRainingYesterday ? config.fertilized_rain_weed_grow_chance : config.fertilized_weed_grow_chance) && location.doesTileHaveProperty((int)vec.X, (int)vec.Y, "Diggable", "Back") != null && !location.isTileOccupied(vec, "") && location.isTilePassable(new Location((int)vec.X, (int)vec.Y), Game1.viewport))
                        {
                            int weedId = GameLocation.getWeedForSeason(random, Game1.currentSeason);
                            location.objects.Add(vec, new Object(vec, weedId, 1));
                        }
                    }
                }

                TempWaterStop[locationName].Clear();

                foreach (Vector2 vec in TempWaterOneDay[locationName])
                {
                    if (location.terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                    {
                        hd.state.Value = HoeDirt.watered;
                        TempWaterStop[locationName].Add(vec);
                    }
                }

                TempWaterOneDay[locationName].Clear();

                foreach (Vector2 vec in TempWaterTwoDays[locationName])
                {
                    if (location.terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                    {
                        hd.state.Value = HoeDirt.watered;
                        TempWaterOneDay[locationName].Add(vec);
                    }
                }

                TempWaterTwoDays[locationName].Clear();


                foreach (Vector2 vec in TempQualityFertilizer[locationName])
                {
                    if (location.terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                    {
                        hd.fertilizer.Value = HoeDirt.noFertilizer;
                    }
                }
                TempQualityFertilizer[locationName].Clear();

                foreach (Vector2 vec in TempQualityIIFertilizer[locationName])
                {
                    if (location.terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                    {
                        hd.fertilizer.Value = HoeDirt.noFertilizer;
                    }
                }
                TempQualityIIFertilizer[locationName].Clear();
            }
        }
    }
}
