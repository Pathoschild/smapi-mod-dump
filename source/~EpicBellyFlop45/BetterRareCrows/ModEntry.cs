using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace BetterRarecrows
{
    class ModEntry : Mod
    {
        public static List<int> CurrentRarecrows;
        public static int PreviousDate = 0;
        public static IMonitor ModMonitor;
        public static ModConfig Config;

        public override void Entry(IModHelper helper)
        {
            // Create a new Harmony instance for patching source code
            HarmonyInstance harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // Get the method we want to patch
            MethodInfo targetMethod = AccessTools.Method(typeof(Farm), nameof(Farm.addCrows));

            // Get the patch that was created
            MethodInfo prefix = AccessTools.Method(typeof(ModEntry), nameof(ModEntry.Prefix));

            // Apply the patch
            harmony.Patch(targetMethod, prefix: new HarmonyMethod(prefix));

            ModMonitor = this.Monitor;

            Config = this.Helper.ReadConfig<ModConfig>();
        }

        private static bool Prefix(ref Farm __instance)
        {
            if (ModEntry.PreviousDate != Game1.dayOfMonth)
            {
                ModEntry.PreviousDate = Game1.dayOfMonth;
                ModEntry.CurrentRarecrows = new List<int>();
            }

            // Check if all rare crows have been placed
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

            if (ModEntry.CurrentRarecrows.Count() == Config.NumberOfRequiredRareCrows)
            {
                ModEntry.ModMonitor.Log($"All {Config.NumberOfRequiredRareCrows} rarecrows found on farm", LogLevel.Trace);
                return false;
            }
            else
            {
                ModEntry.ModMonitor.Log($"Only {CurrentRarecrows.Count()} out of {Config.NumberOfRequiredRareCrows} rarecrows found on the farm", LogLevel.Trace);
                return true;
            }
        }
    }
}
