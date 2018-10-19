using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using static DeepWoodsMod.DeepWoodsRandom;

namespace DeepWoodsMod
{
    public class DeepWoodsAPI : IDeepWoodsAPI
    {
        private static Dictionary<TypeInfo, Chance> customDeepWoodsLevelBuilders = new Dictionary<TypeInfo, Chance>();
        private static Dictionary<TypeInfo, Chance> customDeepWoodsClearingFillers = new Dictionary<TypeInfo, Chance>();
        private static Dictionary<TypeInfo, Chance> customDeepWoodsTerrainFeatures = new Dictionary<TypeInfo, Chance>();
        private static Dictionary<TypeInfo, Chance> customDeepWoodsResourceClumps = new Dictionary<TypeInfo, Chance>();

        public void RegisterCustomDeepWoodsLevelBuilder(TypeInfo type, string chance, int minLevel = 0, int levelModulo = 1)
        {
            customDeepWoodsLevelBuilders.Add(type, JsonConvert.DeserializeObject<Chance>(chance));
        }

        public void RegisterCustomDeepWoodsClearingFiller(TypeInfo type, string chance, int minLevel = 0)
        {
            customDeepWoodsClearingFillers.Add(type, JsonConvert.DeserializeObject<Chance>(chance));
        }

        public void RegisterCustomDeepWoodsTerrainFeature(TypeInfo type, string chance, int minLevel = 0, int levelModulo = 1)
        {
            customDeepWoodsTerrainFeatures.Add(type, JsonConvert.DeserializeObject<Chance>(chance));
        }

        public void RegisterCustomDeepWoodsResourceClump(TypeInfo type, string chance, int minLevel = 0, int levelModulo = 1)
        {
            customDeepWoodsResourceClumps.Add(type, JsonConvert.DeserializeObject<Chance>(chance));
        }

        private static GameLocation GetCustomDeepWoodsLevel(DeepWoods parent, DeepWoodsRandom random, int level, int enterDir)
        {
            foreach (var builderTypeWithChance in customDeepWoodsLevelBuilders.OrderBy(n => Game1.random.Next()))
            {
                if (random.CheckChance(builderTypeWithChance.Value))
                {
                    return (GameLocation)builderTypeWithChance.Key.GetMethod("build", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { parent, level, enterDir });
                }
            }
            return null;
        }

        private static bool CustomFillClearing(DeepWoods clearing, DeepWoodsRandom random, int level)
        {
            foreach (var clearingFillerTypeWithChance in customDeepWoodsClearingFillers.OrderBy(n => Game1.random.Next()))
            {
                if (random.CheckChance(clearingFillerTypeWithChance.Value))
                {
                    clearingFillerTypeWithChance.Key.GetMethod("fill", BindingFlags.Static | BindingFlags.Public).Invoke(null, new object[] { clearing, level });
                    return true;
                }
            }
            return false;
        }

        private static TerrainFeature GetCustomTerrainFeature(DeepWoodsRandom random, int level)
        {
            foreach (var customDeepWoodsTerrainFeature in customDeepWoodsTerrainFeatures.OrderBy(n => Game1.random.Next()))
            {
                if (random.CheckChance(customDeepWoodsTerrainFeature.Value))
                {
                    return (TerrainFeature)Activator.CreateInstance(customDeepWoodsTerrainFeature.Key);
                }
            }
            return null;
        }

        private static TerrainFeature GetCustomResourceClump(DeepWoodsRandom random, Vector2 tile, int level)
        {
            foreach (var customDeepWoodsResourceClump in customDeepWoodsResourceClumps.OrderBy(n => Game1.random.Next()))
            {
                if (random.CheckChance(customDeepWoodsResourceClump.Value))
                {
                    return (ResourceClump)Activator.CreateInstance(customDeepWoodsResourceClump.Key, new object[] { tile });
                }
            }
            return null;
        }
    }
}
