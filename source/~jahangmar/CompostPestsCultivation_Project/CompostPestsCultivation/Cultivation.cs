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

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;

namespace CompostPestsCultivation
{
    public class Cultivation : ModComponent
    {
        private static Dictionary<int, List<CropTrait>> CropTraits = new Dictionary<int, List<CropTrait>>();
        private static Dictionary<int, int> CropSeeds = new Dictionary<int, int>();

        private static Config config;
        private static Random rand = new Random();

        public static void Init(Config conf)
        {
            config = conf;
        }

        public static void Load()
        {
            Dictionary<int, List<CropTrait>> loadedCropTraits = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._CropTraits)?.CropTraits;
            if (loadedCropTraits != null)
                CropTraits = loadedCropTraits;

            Dictionary<int, int> loadedCropSeeds = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._CropSeeds)?.CropSeeds;
            if (loadedCropSeeds != null)
                CropSeeds = loadedCropSeeds;

            List<Vector2> loadedTempQualityFertilizer = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._TempQualityFertilizer)?.TempQualityFertilizer;
            if (loadedTempQualityFertilizer != null)
                TempQualityFertilizer = loadedTempQualityFertilizer;

            List<Vector2> loadedTempQualityIIFertilizer = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._TempQualityIIFertilizer)?.TempQualityIIFertilizer;
            if (loadedTempQualityIIFertilizer != null)
                TempQualityIIFertilizer = loadedTempQualityIIFertilizer;

            List<Vector2> loadedTempWaterStop = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._TempWaterStop)?.TempWaterStop;
            if (loadedTempWaterStop != null)
                TempWaterStop = loadedTempWaterStop;

            List<Vector2> loadedTempWaterOneDay = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._TempWaterOneDay)?.TempWaterOneDay;
            if (loadedTempWaterOneDay != null)
                TempWaterOneDay = loadedTempWaterOneDay;

            List<Vector2> loadedTempWaterTwoDays = ModEntry.GetHelper().Data.ReadSaveData<SaveData>(SaveData._TempWaterTwoDays)?.TempWaterTwoDays;
            if (loadedTempWaterTwoDays != null)
                TempWaterTwoDays = loadedTempWaterTwoDays;

            ModEntry.GetMonitor().Log("Cultivation.Load() executed", LogLevel.Trace);
            ModEntry.GetMonitor().Log($"loaded {CropTraits.Count} crop traits and {CropSeeds.Count} crop seeds", LogLevel.Trace);
        }

        public static void Save()
        {
            SaveData dat = new SaveData()
            {
                CropTraits = CropTraits,
                CropSeeds = CropSeeds,
                TempQualityFertilizer = TempQualityFertilizer,
                TempQualityIIFertilizer = TempQualityIIFertilizer,
                TempWaterStop = TempWaterStop,
                TempWaterOneDay = TempWaterOneDay,
                TempWaterTwoDays = TempWaterTwoDays
            };

            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._CropTraits, dat);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._CropSeeds, dat);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._TempQualityFertilizer, dat);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._TempQualityIIFertilizer, dat);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._TempWaterStop, dat);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._TempWaterOneDay, dat);
            ModEntry.GetHelper().Data.WriteSaveData<SaveData>(SaveData._TempWaterTwoDays, dat);
            ModEntry.GetMonitor().Log("Cultivation.Save() executed", LogLevel.Trace);
        }

        public static void NewSeeds(int id)
        {
            //ModEntry.GetMonitor().Log("checking key in cropseeds...");
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

                if (traits.Count >= Enum.GetValues(typeof(CropTrait)).Length)
                {
                    ModEntry.GetMonitor().Log($"tried to add trait to {CropSeedsDisplayName(id)}, but traits are maxed", LogLevel.Trace);
                    return;
                }

                AddTrait(GetNewRandomTrait(traits), traits, id);

            }
        }

        private static CropTrait GetNewRandomTrait(List<CropTrait> traits)
        {
            CropTrait result = CropTrait.PestResistanceI;
            for (int i=0; i<100; i++)
            {
                result = GetRandomTrait();
                if (!traits.Contains(result))
                    break;
            }
            return result;
        }

        private static CropTrait GetRandomTrait() => (CropTrait)Enum.GetValues(typeof(CropTrait)).GetValue(GetRandomInt(Enum.GetValues(typeof(CropTrait)).Length));

        private static string CropSeedsDisplayName(int id) => ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, id, 1).DisplayName;

        public static string GetTraitDescr(CropTrait trait)
        {
            ITranslationHelper trans = ModEntry.GetHelper().Translation;
            switch (trait)
            {
                case CropTrait.PestResistanceI:
                case CropTrait.PestResistanceII:
                    return trans.Get("cult.msg_traitdesc_pestres");
                case CropTrait.QualityI:
                case CropTrait.QualityII:
                    return trans.Get("cult.msg_traitdesc_quality");
                case CropTrait.WaterI:
                case CropTrait.WaterII:
                    return trans.Get("cult.msg_traitdesc_water");
                case CropTrait.SpeedI:
                case CropTrait.SpeedII:
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
                case CropTrait.PestResistanceI:
                    return trans.Get("cult.msg_traitlongdesc_pestresI");
                case CropTrait.PestResistanceII:
                    return trans.Get("cult.msg_traitlongdesc_pestresII");
                case CropTrait.QualityI:
                    return trans.Get("cult.msg_traitlongdesc_qualityI");
                case CropTrait.QualityII:
                    return trans.Get("cult.msg_traitlongdesc_qualityII");
                case CropTrait.WaterI:
                    return trans.Get("cult.msg_traitlongdesc_waterI");
                case CropTrait.WaterII:
                    return trans.Get("cult.msg_traitlongdesc_waterII");
                case CropTrait.SpeedI:
                    return trans.Get("cult.msg_traitlongdesc_speedI");
                case CropTrait.SpeedII:
                    return trans.Get("cult.msg_traitlongdesc_speedII");
                default:
                    return "ERROR gettraitlongdescr";
            }
        }

        public static string GetTraitName(CropTrait trait)
        {
            ITranslationHelper trans = ModEntry.GetHelper().Translation;
            switch (trait)
            {
                case CropTrait.PestResistanceI:
                    return trans.Get("cult.msg_trait_pestresI");
                case CropTrait.PestResistanceII:
                    return trans.Get("cult.msg_trait_pestresII");
                case CropTrait.QualityI:
                    return trans.Get("cult.msg_trait_qualityI");
                case CropTrait.QualityII:
                    return trans.Get("cult.msg_trait_qualityII");
                case CropTrait.WaterI:
                    return trans.Get("cult.msg_trait_waterI");
                case CropTrait.WaterII:
                    return trans.Get("cult.msg_trait_waterII");
                case CropTrait.SpeedI:
                    return trans.Get("cult.msg_trait_speedI");
                case CropTrait.SpeedII:
                    return trans.Get("cult.msg_trait_speedII");
                default:
                    return "ERROR gettraitname";
            }
        }

        public static void AddTrait(CropTrait trait, List<CropTrait> traits, int id)
        {
            ITranslationHelper trans = ModEntry.GetHelper().Translation;
             
            void add() {
                ModEntry.GetMonitor().Log($"Added {trait} to {CropSeedsDisplayName(id)}", LogLevel.Trace);
                Game1.showGlobalMessage(trans.Get("cult.msg_traitinc", new { traitdesc = GetTraitDescr(trait), seed = CropSeedsDisplayName(id), trait = GetTraitName(trait) }));
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

        public static int GetRandomInt(int max)
        {
            return rand.Next(max);
        }


        private static List<Vector2> TempQualityFertilizer = new List<Vector2>(),
                              TempQualityIIFertilizer = new List<Vector2>();
        private static List<Vector2> TempWaterOneDay = new List<Vector2>(),
                              TempWaterTwoDays = new List<Vector2>(),
                                TempWaterStop = new List<Vector2>();
                               


        private static int GetQuality(Vector2 tile, List<CropTrait> traits)
        {
            int level = 0;
            if (Composting.AffectedByCompost(tile))
                level += 1;
            if (traits.Contains(CropTrait.QualityI))
                level += 1;
            if (traits.Contains(CropTrait.QualityII) && level <= 1)
                level += 1;
            return level;
        }

        public static int GetPestRes(Vector2 tile, List<CropTrait> traits)
        {
            int level = 0;
            if (Composting.AffectedByCompost(tile))
                level += 1;
            if (traits.Contains(CropTrait.PestResistanceI))
                level += 1;
            if (traits.Contains(CropTrait.PestResistanceII) && level <= 1)
                level += 1;
            return level;
        }

        private static int GetWater(Vector2 tile, List<CropTrait> traits)
        {
            int level = 0;
            if (Composting.AffectedByCompost(tile))
                level += 1;
            if (traits.Contains(CropTrait.WaterI))
                level += 1;
            if (traits.Contains(CropTrait.WaterII))
                level += 1;
            return level;
        }

        private static int GetSpeed(Vector2 tile, List<CropTrait> traits)
        {
            int level = 0;
            if (Composting.AffectedByCompost(tile))
                level += 1;
            if (traits.Contains(CropTrait.SpeedI))
                level += 1;
            if (traits.Contains(CropTrait.SpeedII))
                level += 1;
            return level;
        }

        public static bool UngrowCrop(Crop crop, Vector2 pos, string src)
        {
            crop.fullyGrown.Value = false;
            crop.dayOfCurrentPhase.Value -= 2;
            ModEntry.GetMonitor().Log(src + " decreased day of current phase of crop at " + pos, LogLevel.Trace);
            if (crop.dayOfCurrentPhase.Value <= 0)
            {
                crop.currentPhase.Value -= 1;
                if (crop.currentPhase <= 0)
                {
                    ModEntry.GetMonitor().Log(src+ " killed crop at " + pos, LogLevel.Trace);
                    crop.dead.Value = true;
                    crop.currentPhase.Value = 0;
                }
                else
                    ModEntry.GetMonitor().Log(src+ " decreased phase of crop at " + pos, LogLevel.Trace);
            }

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
            int seedIdx = crop.netSeedIndex.Value;
            if (!Cultivation.CropTraits.TryGetValue(seedIdx, out List<CropTrait> traits))
            {
                traits = new List<CropTrait>();
            }
            return traits;
        }

        public static StardewValley.Object GetCropItemFromSeeds(StardewValley.Object seeds)
        {
            Dictionary<int, string> dictionary = Game1.temporaryContent.Load<Dictionary<int, string>>("Data\\Crops");
            foreach (KeyValuePair<int, string> item in dictionary)
            {
                if (item.Key == seeds.ParentSheetIndex)
                {
                    Item it = ObjectFactory.getItemFromDescription(ObjectFactory.regularObject, Convert.ToInt32(item.Value.Split('/')[3]), 1);
                    return it as StardewValley.Object;
                }
            }
            return null;
        }

        public static void OnEndDay()
        {
            foreach (KeyValuePair<Vector2, TerrainFeature> pair in Game1.getFarm().terrainFeatures.Pairs)
            {
                if (pair.Key is Vector2 vec && pair.Value is HoeDirt hd && hd.crop is Crop crop && crop != null)
                {
                    if (crop.isWildSeedCrop())
                    {
                        //TODO
                    }
                    else
                    {
                        List<CropTrait> traits = GetTraits(crop);

                        switch (GetQuality(vec, traits))
                        {
                            case 0: //normal
                                break;
                            case 1: //better
                                if (hd.fertilizer.Value == HoeDirt.noFertilizer)
                                {
                                    TempQualityFertilizer.Add(vec);
                                    hd.fertilizer.Value = HoeDirt.fertilizerLowQuality;
                                }
                                break;
                            case 2: //best
                                if (hd.fertilizer.Value == HoeDirt.noFertilizer)
                                {
                                    TempQualityIIFertilizer.Add(vec);
                                    hd.fertilizer.Value = HoeDirt.fertilizerHighQuality;
                                }
                                break;
                            default:
                                ModEntry.GetMonitor().Log("Bug: GetQuality returned wrong value", LogLevel.Error);
                                break;
                        }

                        switch (GetSpeed(vec, traits))
                        {
                            case 0: //reduced
                                if (GetRandomInt(100) <= config.minimal_speed_ungrow_chance)
                                    UngrowCrop(hd.crop, vec, "lacking speed trait or compost");
                                break;
                            case 1: //normal
                                break;
                            case 2: //better
                                if (GetRandomInt(100) <= config.speed_i_trait_grow_chance)
                                    hd.crop.newDay(HoeDirt.watered, HoeDirt.noFertilizer, (int)vec.X, (int)vec.Y, Game1.getFarm());
                                    break;
                            case 3: //best
                                if (GetRandomInt(100) <= config.speed_ii_trait_grow_chance)
                                    hd.crop.newDay(HoeDirt.watered, HoeDirt.noFertilizer, (int)vec.X, (int)vec.Y, Game1.getFarm());
                                break;
                            default:
                                ModEntry.GetMonitor().Log("Bug: GetSpeed returned wrong value", LogLevel.Error);
                                break;
                        }

                        switch (GetWater(vec, traits))
                        {
                            case 0: //reduced
                                if (hd.state.Value != HoeDirt.watered)
                                    UngrowCrop(hd.crop, vec, "lacking water trait or compost");
                                break;
                            case 1: //normal
                                break;
                            case 2: //better
                                if (hd.state.Value == HoeDirt.watered && !TempWaterOneDay.Contains(vec) && !TempWaterStop.Contains(vec))
                                    TempWaterOneDay.Add(vec);
                                break;
                            case 3: //best
                                if (hd.state.Value == HoeDirt.watered && !TempWaterOneDay.Contains(vec) && !TempWaterStop.Contains(vec))
                                    TempWaterTwoDays.Add(vec);
                                break;
                            default:
                                ModEntry.GetMonitor().Log("Bug: GetWater returned wrong value", LogLevel.Error);
                                break;
                        }
                    }

                }
            }
        }

        public static void OnNewDay()
        {
            TempWaterStop.Clear();

            foreach (Vector2 vec in TempWaterOneDay)
            {
                if (Game1.getFarm().terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                {
                    hd.state.Value = HoeDirt.watered;
                    TempWaterStop.Add(vec);
                }
            }

            TempWaterOneDay.Clear();

            foreach (Vector2 vec in TempWaterTwoDays)
            {
                if (Game1.getFarm().terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                {
                    hd.state.Value = HoeDirt.watered;
                    TempWaterOneDay.Add(vec);
                }
            }

            TempWaterTwoDays.Clear();


            foreach (Vector2 vec in TempQualityFertilizer)
            {
                if (Game1.getFarm().terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                {
                    hd.fertilizer.Value = HoeDirt.noFertilizer;
                }
            }
            TempQualityFertilizer.Clear();

            foreach (Vector2 vec in TempQualityIIFertilizer)
            {
                if (Game1.getFarm().terrainFeatures.TryGetValue(vec, out TerrainFeature tf) && tf is HoeDirt hd)
                {
                    hd.fertilizer.Value = HoeDirt.noFertilizer;
                }
            }
            TempQualityIIFertilizer.Clear();

        }
    }
}
