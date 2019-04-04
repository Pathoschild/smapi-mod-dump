using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using xTile.Dimensions;
using static DeepWoodsMod.DeepWoodsEnterExit;
using static DeepWoodsMod.DeepWoodsRandom;

namespace DeepWoodsMod.API.Impl
{
    public class DeepWoodsAPI : IDeepWoodsAPI
    {
        private static Random rng = new Random();
        public static List<T> ToShuffledList<T>(IEnumerable<T> thingsToShuffle)
        {
            List<T> list = new List<T>(thingsToShuffle);
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
            return list;
        }

        public event Action<IDeepWoodsLocation> OnCreate;

        public event Action<IDeepWoodsLocation> BeforeMapGeneration;
        public event Action<IDeepWoodsLocation> AfterMapGeneration;
        public event Action<IDeepWoodsLocation> BeforeFill;
        public event Action<IDeepWoodsLocation> AfterFill;
        public event Action<IDeepWoodsLocation> BeforeMonsterGeneration;
        public event Action<IDeepWoodsLocation> AfterMonsterGeneration;
        public event Action<IDeepWoodsLocation> BeforeDebrisCreation;
        public event Action<IDeepWoodsLocation> AfterDebrisCreation;

        public event Func<IDeepWoodsLocation, bool> OverrideMapGeneration;
        public event Func<IDeepWoodsLocation, bool> OverrideFill;
        public event Func<IDeepWoodsLocation, bool> OverrideMonsterGeneration;
        public event Func<IDeepWoodsLocation, bool> OverrideDebrisCreation;

        public List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<TerrainFeature>>> TerrainFeatures { get; } = new List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<TerrainFeature>>>();
        public List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<LargeTerrainFeature>>> LargeTerrainFeatures { get; } = new List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<LargeTerrainFeature>>>();
        public List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<ResourceClump>>> ResourceClumps { get; } = new List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<ResourceClump>>>();
        public List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<StardewValley.Object>>> Objects { get; } = new List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<StardewValley.Object>>>();
        public List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<Monster>>> Monsters { get; } = new List<Tuple<Func<IDeepWoodsLocation, Vector2, bool>, Func<Monster>>>();

        public IDeepWoodsTextures Textures
        {
            get
            {
                return DeepWoodsTextures.Textures;
            }
        }

        public void RegisterTerrainFeature(Func<IDeepWoodsLocation, Vector2, bool> decisionCallback, Func<TerrainFeature> creationCallback)
        {
            TerrainFeatures.Add(Tuple.Create(decisionCallback, creationCallback));
        }

        public void RegisterLargeTerrainFeature(Func<IDeepWoodsLocation, Vector2, bool> decisionCallback, Func<LargeTerrainFeature> creationCallback)
        {
            LargeTerrainFeatures.Add(Tuple.Create(decisionCallback, creationCallback));
        }

        public void RegisterResourceClump(Func<IDeepWoodsLocation, Vector2, bool> decisionCallback, Func<ResourceClump> creationCallback)
        {
            ResourceClumps.Add(Tuple.Create(decisionCallback, creationCallback));
        }

        public void RegisterObject(Func<IDeepWoodsLocation, Vector2, bool> decisionCallback, Func<StardewValley.Object> creationCallback)
        {
            Objects.Add(Tuple.Create(decisionCallback, creationCallback));
        }

        public void RegisterMonster(Func<IDeepWoodsLocation, Vector2, bool> decisionCallback, Func<Monster> creationCallback)
        {
            Monsters.Add(Tuple.Create(decisionCallback, creationCallback));
        }

        public void AddExitLocation(IDeepWoodsLocation deepWoodsLocation, IDeepWoodsExit exit, Location tile)
        {
            DeepWoodsManager.AddExitLocation(deepWoodsLocation as DeepWoods, tile, exit as DeepWoodsExit);
        }

        public void RemoveExitLocation(IDeepWoodsLocation deepWoodsLocation, Location tile)
        {
            DeepWoodsManager.RemoveExitLocation(deepWoodsLocation as DeepWoods, tile);
        }

        public void CallOnCreate(DeepWoods deepWoods)
        {
            if (OnCreate == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in OnCreate.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallBeforeMapGeneration(DeepWoods deepWoods)
        {
            if (BeforeMapGeneration == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in BeforeMapGeneration.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallAfterMapGeneration(DeepWoods deepWoods)
        {
            if (AfterMapGeneration == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in AfterMapGeneration.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallBeforeFill(DeepWoods deepWoods)
        {
            if (BeforeFill == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in BeforeFill.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallAfterFill(DeepWoods deepWoods)
        {
            if (AfterFill == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in AfterFill.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallBeforeMonsterGeneration(DeepWoods deepWoods)
        {
            if (BeforeMonsterGeneration == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in BeforeMonsterGeneration.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallAfterMonsterGeneration(DeepWoods deepWoods)
        {
            if (AfterMonsterGeneration == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in AfterMonsterGeneration.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallBeforeDebrisCreation(DeepWoods deepWoods)
        {
            if (BeforeDebrisCreation == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in BeforeDebrisCreation.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public void CallAfterDebrisCreation(DeepWoods deepWoods)
        {
            if (AfterDebrisCreation == null)
                return;

            foreach (Action<IDeepWoodsLocation> callback in AfterDebrisCreation.GetInvocationList())
            {
                try
                {
                    callback(deepWoods);
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }
        }

        public bool CallOverrideMapGeneration(DeepWoods deepWoods)
        {
            if (OverrideMapGeneration == null)
                return false;

            // If multiple mods add an override, we shuffle the overrides and the first one "wins":
            foreach (Func<IDeepWoodsLocation, bool> callback in ToShuffledList(OverrideMapGeneration.GetInvocationList()))
            {
                try
                {
                    if (callback(deepWoods))
                        return true;
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }

            return false;
        }

        public bool CallOverrideFill(DeepWoods deepWoods)
        {
            if (OverrideFill == null)
                return false;

            // If multiple mods add an override, we shuffle the overrides and the first one "wins":
            foreach (Func<IDeepWoodsLocation, bool> callback in ToShuffledList(OverrideFill.GetInvocationList()))
            {
                try
                {
                    if (callback(deepWoods))
                        return true;
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }

            return false;
        }

        public bool CallOverrideMonsterGeneration(DeepWoods deepWoods)
        {
            if (OverrideMonsterGeneration == null)
                return false;

            // If multiple mods add an override, we shuffle the overrides and the first one "wins":
            foreach (Func<IDeepWoodsLocation, bool> callback in ToShuffledList(OverrideMonsterGeneration.GetInvocationList()))
            {
                try
                {
                    if (callback(deepWoods))
                        return true;
                }
                catch(Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }

            return false;
        }

        public bool CallOverrideDebrisCreation(DeepWoods deepWoods)
        {
            if (OverrideDebrisCreation == null)
                return false;

            // If multiple mods add an override, we shuffle the overrides and the first one "wins":
            foreach (Func<IDeepWoodsLocation, bool> callback in ToShuffledList(OverrideDebrisCreation.GetInvocationList()))
            {
                try
                {
                    if (callback(deepWoods))
                        return true;
                }
                catch (Exception e)
                {
                    ModEntry.Log("[THIS IS NOT A BUG IN DEEPWOODS] Exception caught while calling callback from another mod: " + e, StardewModdingAPI.LogLevel.Warn);
                }
            }

            return false;
        }
    }
}
