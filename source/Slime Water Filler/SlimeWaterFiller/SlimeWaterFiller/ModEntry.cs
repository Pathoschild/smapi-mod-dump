/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Speshkitty/SlimeWaterFiller
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SlimeWaterFiller
{
    public class ModEntry : Mod
    {
        static IMonitor monitor;
        new static IModHelper Helper;
        JsonAssets.IApi japi;

        public override void Entry(IModHelper helper)
        {
            monitor = Monitor;
            Helper = helper;

            HarmonyInstance harmony = HarmonyInstance.Create("speshkitty.slimewaterfiller.harmony");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;

            
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            if(e.Ticks > 1)
            {
                japi = Helper.ModRegistry.GetApi<JsonAssets.IApi>("spacechase0.JsonAssets");

                if (Helper.ModRegistry.IsLoaded("FlashShifter.MarlonSVE"))
                {
                    japi.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", "SVE"));
                }
                else
                {
                    japi.LoadAssets(Path.Combine(Helper.DirectoryPath, "assets", "NSVE"));
                }

                Helper.Events.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            }
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            foreach(Building b in Game1.getFarm().buildings)
            {
                if (b.nameOfIndoorsWithoutUnique != "SlimeHutch")
                {
                    continue;
                }

                ((SlimeHutch)b.indoors).TryRefillWater();
            }
        }

        public static void LogData(object stringToLog)
        {
            monitor.Log(stringToLog.ToString(), LogLevel.Info);
        }

        [HarmonyPatch(typeof(SlimeHutch))]
        [HarmonyPatch("DayUpdate")]
        public class SlimePatches
        {
            public static void Prefix(SlimeHutch __instance)
            {
                __instance.TryRefillWater();
            }

            public static void Postfix(SlimeHutch __instance)
            {
                __instance.TryRefillWater();
            }
        }


    }

    public static class Extensions
    {
        public static void TryRefillWater(this SlimeHutch hutch) 
        {
            if (hutch.numberOfObjectsWithName("Slime Waterer") > 0)
            {
                hutch.waterSpots.Set(new[] { true, true, true, true });
            }
        }
    }
}
