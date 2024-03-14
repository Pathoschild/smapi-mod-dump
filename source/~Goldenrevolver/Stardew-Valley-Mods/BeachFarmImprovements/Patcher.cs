/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace BeachFarmImprovements
{
    using HarmonyLib;
    using StardewValley;
    using StardewValley.Locations;
    using System;
    using xTile.Dimensions;

    internal class Patcher
    {
        private static BeachFarmImprovements mod;

        public static void PatchAll(BeachFarmImprovements beachFarmImprovements)
        {
            mod = beachFarmImprovements;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            harmony.Patch(
               original: AccessTools.Method(typeof(Farm), nameof(Farm.MakeMapModifications)),
               postfix: new HarmonyMethod(typeof(Patcher), nameof(SpawnShip)));
        }

        // TODO
        public static bool PerformAction_Patch(IslandSouthEastCave __instance, string action, Farmer who, Location tileLocation)
        {
            if (action != null && who.IsLocalPlayer)
            {
                string a = action.Split(' ', StringSplitOptions.None)[0];
                string b = action.Split(' ', StringSplitOptions.None)[1];
                if (a == "MessageSpeech" && b == "Pirates6")
                {
                    var question_prompt = Game1.content.LoadString("Strings\\StringsFromMaps:Pirates7_0");

                    __instance.createQuestionDialogue(question_prompt, __instance.createYesNoResponses(), "GoldenBeachFarmImprovement");
                }
            }

            return true;
        }

        // Pirates4
        // base.setTileProperty(23, 8, "Buildings", "Action", "MessageSpeech Pirates4");

        public static void SpawnShip(Farm __instance)
        {
            if (Game1.whichFarm != Farm.beach_layout)
            {
                return;
            }

            //if (!BeachFarmImprovements.HasUnlockedPirateVisits)
            //{
            //    return;
            //}

            __instance.updateMap();

            // TODO this is placeholder, the beach bridge
            __instance.setMapTile(58, 13, 301, "Buildings", null);
            __instance.setMapTile(59, 13, 301, "Buildings", null);
            __instance.setMapTile(60, 13, 301, "Buildings", null);
            __instance.setMapTile(61, 13, 301, "Buildings", null);
            __instance.setMapTile(58, 14, 336, "Back", null);
            __instance.setMapTile(59, 14, 336, "Back", null);
            __instance.setMapTile(60, 14, 336, "Back", null);
            __instance.setMapTile(61, 14, 336, "Back", null);
        }
    }
}