/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Rafseazz/Ridgeside-Village-Mod
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RidgesideVillage
{
    class Patcher
    {
        static IModHelper Helper;
        static IManifest Manifest;
        static IJsonAssetsApi JsonAssetsAPI;

        public Patcher(IMod mod)
        {
            Helper = mod.Helper;
            Manifest = mod.ModManifest;
        }

        public void PerformPatching()
        {
            var harmony = new Harmony(Manifest.UniqueID);

            HarmonyPatch_EventMessage.ApplyPatch(harmony, Helper);
            HarmonyPatch_Obelisk.ApplyPatch(harmony, Helper);
            HarmonyPatch_UntimedSO.ApplyPatch(harmony, Helper);
            HarmonyPatch_EventDetection.ApplyPatch(harmony, Helper);
            HarmonyPatch_Fish.ApplyPatch(harmony, Helper);
            HarmonyPatch_TreasureItems.ApplyPatch(harmony, Helper);
            HarmonyPatch_SummitFarm.ApplyPatch(harmony, Helper);
            HarmonyPatch_WeddingGuests.ApplyPatch(harmony, Helper);
        }
    }        
}
