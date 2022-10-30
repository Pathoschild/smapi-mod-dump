/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/NeroYuki/StardewSurvivalProject
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using HarmonyLib;
using StardewValley;
using SObject = StardewValley.Object;
using StardewModdingAPI;

namespace StardewSurvivalProject.source.harmony_patches
{
    public class HarmonyPatches
    {
        public static void InitPatches(string uniqueModId, IMonitor monitor)
        {
            FarmerPatches.Initialize(monitor);
            ObjectPatches.Initialize(monitor);
            UIDrawPatches.Initialize(monitor);
            NPCPatches.Initialize(monitor);

            var harmony = new Harmony(uniqueModId);

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doneEating)),
               postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.DoneEating_PostFix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.EndUsingTool)),
                postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.EndUsingTool_PostFix))
             );

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.receiveGift), new Type[] { typeof(SObject), typeof(Farmer), typeof(bool), typeof(float), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.GiftGiving_PostFix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.healthRecoveredOnConsumption)),
                prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.CalculateHPGain_Prefix))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.Menus.IClickableMenu), nameof(StardewValley.Menus.IClickableMenu.drawHoverText),
                new Type[] { typeof(SpriteBatch), typeof(StringBuilder), typeof(SpriteFont), typeof(int), typeof(int), typeof(int), typeof(string), typeof(int), typeof(string[]), typeof(Item), typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(CraftingRecipe), typeof(IList<Item>) }),
                postfix: new HarmonyMethod(typeof(UIDrawPatches), nameof(UIDrawPatches.DrawHoverText_Postfix))
            );
        }
    }
}
