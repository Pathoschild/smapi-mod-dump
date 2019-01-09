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
                if ((bool)((NetFieldBase<bool, NetBool>)pair.Value.bigCraftable) && pair.Value.Name.Contains("arecrow"))
                {
                    List<int> listOfPossibleRarecrows = new List<int> { 140, 139, 138, 137, 136, 126, 113, 110 };

                    if (listOfPossibleRarecrows.Contains(pair.Value.parentSheetIndex) && !CurrentRarecrows.Contains(pair.Value.parentSheetIndex))
                    {
                        ModEntry.CurrentRarecrows.Add(pair.Value.parentSheetIndex);
                    }
                }
            }

            if (ModEntry.CurrentRarecrows.Count() == 8)
            {
                ModEntry.ModMonitor.Log("All 8 rarecrows found on farm", LogLevel.Trace);
                return false;
            }
            else
            {
                ModEntry.ModMonitor.Log($"Only {CurrentRarecrows.Count()} rarecrows found on farm", LogLevel.Trace);
                return true;
            }
        }
    }
}
