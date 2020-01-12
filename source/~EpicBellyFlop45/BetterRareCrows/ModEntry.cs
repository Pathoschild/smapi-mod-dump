using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace BetterRarecrows
{
    /// <summary>Mod entry point.</summary>
    class ModEntry : Mod
    {
        /// <summary>The current rarecrows the player has placed on their farm.</summary>
        public static List<int> CurrentRarecrows;

        /// <summary>This is the data the game was when crows last attempted the eat crops (This is a new CurrentRarecrows list can be created each day)</summary>
        public static int PreviousDate = 0;

        /// <summary>Provides methods for logging to the console.</summary>
        public static IMonitor ModMonitor;

        /// <summary>The mod configuration.</summary>
        public static ModConfig Config;

        /// <summary>The mod entry point.</summary>
        /// <param name="helper">Provides methods for interacting with the mod directory as well as the modding api.</param>
        public override void Entry(IModHelper helper)
        {
            ApplyHarmonyPatches(this.ModManifest.UniqueID);

            ModMonitor = this.Monitor;
            Config = this.Helper.ReadConfig<ModConfig>();
        }

        /// <summary>The method that applies the harmony patches for replacing game code.</summary>
        /// <param name="uniqueId">The mod unique id.</param>
        private void ApplyHarmonyPatches(string uniqueId)
        {
            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(uniqueId);

            // Get the method we want to patch
            MethodInfo targetMethod = AccessTools.Method(typeof(Farm), nameof(Farm.addCrows));

            // Get the patch that was created
            MethodInfo prefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Prefix));

            // Apply the patch
            harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix));
        }

        /// <summary>The code that get's ran before the Farm.addCrows game method gets ran</summary>
        /// <param name="__instance">The instance of the farm, used for checking which rarecrows have been placed</param>
        /// <returns>If there are enough rarecrows placed, return false. (Game method doesn't get ran) If there aren't enough rarecrows placed, return true (Game method gets ran)</returns>
        private static bool Prefix(ref Farm __instance)
        {
            if (ModEntry.PreviousDate != Game1.dayOfMonth)
            {
                ModEntry.PreviousDate = Game1.dayOfMonth;
                ModEntry.CurrentRarecrows = new List<int>();
            }

            // Check how many rarecrows have been placed
            foreach (KeyValuePair<Vector2, StardewValley.Object> pair in __instance.objects.Pairs)
            {
                if ((bool)((NetFieldBase<bool, NetBool>)pair.Value.bigCraftable) && pair.Value.Name.Contains("Rarecrow"))
                {
                    if (!CurrentRarecrows.Contains(pair.Value.ParentSheetIndex))
                    {
                        ModEntry.CurrentRarecrows.Add(pair.Value.ParentSheetIndex);
                    }
                }
            }

            if (ModEntry.CurrentRarecrows.Count() >= Config.NumberOfRequiredRareCrows)
            {
                ModEntry.ModMonitor.Log($"All {CurrentRarecrows.Count()} out of {Config.NumberOfRequiredRareCrows} rarecrows found on the farm", LogLevel.Trace);
                return false;
            }
            else
            {
                ModEntry.ModMonitor.Log($"Only {CurrentRarecrows.Count()} out of {Config.NumberOfRequiredRareCrows} rarecrows found on the farm", LogLevel.Trace);
                
                if (Config.EnableProgressiveMode)
                {
                    ModEntry.ModMonitor.Log($"Passive mod enabled", LogLevel.Trace);

                    // Calculate the a random chance to determine if the crows should be able to spawn
                    int chanceUpperBound = Math.Min(100, Config.ProgressivePercentPerRarecrow * ModEntry.CurrentRarecrows.Count());
                    int chance = Math.Max(0, chanceUpperBound);

                    double randomChance = Game1.random.NextDouble();

                    if (chance / 100d < randomChance)
                    {
                        return false;
                    }
                }
                else
                {
                    ModEntry.ModMonitor.Log($"Passive mod disabled", LogLevel.Trace);
                }

                return true;
            }
        }
    }
}
