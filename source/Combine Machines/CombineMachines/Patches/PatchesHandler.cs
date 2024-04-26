/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using CombineMachines.Helpers;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SObject = StardewValley.Object;

namespace CombineMachines.Patches
{
    internal static class PatchesHandler
    {
        /// <summary>Intended to be invoked once during the mod entry, to apply Harmony patches to the game code</summary>
        internal static void Entry(IModHelper helper)
        {
            Harmony Harmony = new Harmony(ModEntry.ModInstance.ModManifest.UniqueID);

            //  Patch StardewValley.Object.maximumStackSize to always return 1 if a machine has been combined with other machines
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.maximumStackSize)),
                prefix: new HarmonyMethod(typeof(MaximumStackSizePatch), nameof(MaximumStackSizePatch.Prefix))
            );

            //  Patch StardewValley.Object.canStackWith to always return false if either one has already been combined with other machines
            Harmony.Patch(
                original: AccessTools.Method(typeof(Item), nameof(Item.canStackWith)),
                prefix: new HarmonyMethod(typeof(CanStackWithPatch), nameof(CanStackWithPatch.Prefix))
            );

            //  Patch StardewValley.Object.performRemoveAction to detect when a combined machine is removed from a game tile (such as by hitting a furnace with a pickaxe)
            //  to then refund the other copies of the machine that were combined with the one that just got removed
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performRemoveAction)),
                prefix: new HarmonyMethod(typeof(PerformRemoveActionPatch), nameof(PerformRemoveActionPatch.Prefix))
            );

            //  Patch StardewValley.Object.draw and StardewValley.Object.drawInMenu to also draw a number in the bottom-right corner of the tile the machine is being drawn to.
            //  The number indicates how many copies of the machine are combined into one.
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                postfix: new HarmonyMethod(typeof(DrawPatch), nameof(DrawPatch.Postfix))
            );
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.drawInMenu), new Type[] { typeof(SpriteBatch), typeof(Vector2), typeof(float), typeof(float), typeof(float), typeof(StackDrawType), typeof(Color), typeof(bool) }),
                postfix: new HarmonyMethod(typeof(DrawInMenuPatch), nameof(DrawInMenuPatch.Postfix))
            );
            //  Also patch the overridden draw methods of some Object sub-types like Crab Pots and Wood Chippers
            Harmony.Patch(
                original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                postfix: new HarmonyMethod(typeof(DrawPatch), nameof(DrawPatch.Postfix))
            );
            Harmony.Patch(
                original: AccessTools.Method(typeof(WoodChipper), nameof(WoodChipper.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
                postfix: new HarmonyMethod(typeof(DrawPatch), nameof(DrawPatch.Postfix))
            );

            //  Patch StardewValley.Menus.InventoryMenu.draw to also draw a yellow border around other machines that the current CursorSlotItem can be combined with.
            Harmony.Patch(
                original: AccessTools.Method(typeof(InventoryMenu), nameof(InventoryMenu.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(InventoryMenuDrawPatch), nameof(InventoryMenuDrawPatch.Postfix))
            );

            ProcessingPatches.Entry(helper, Harmony);

            //  Patch StardewValley.Object.minutesElapsed to detect when a machines output is ready for collecting, and when that happens, increase the output quantity
            //  to account for how many machines were combined
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.minutesElapsed)),
                prefix: new HarmonyMethod(typeof(MinutesElapsedPatch), nameof(MinutesElapsedPatch.Prefix)),
                postfix: new HarmonyMethod(typeof(MinutesElapsedPatch), nameof(MinutesElapsedPatch.Postfix))
            );

            //  Patch StardewValley.Object.initNetFields to detect when StardewValley.Object.MinutesUntilReady/StardewValley.Object.Cask.agingRate changes (by subscribing to NetIntDelta.fieldChangeEvent), 
            //  and modify the new MinutesUntilReady/agingRate based on the combined machine's processing power
            //  (See also: UserConfig.ProcessingMode / UserConfig.ProcessingModeExclusions)
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), "initNetFields"),
                postfix: new HarmonyMethod(typeof(MinutesUntilReadyPatch), nameof(MinutesUntilReadyPatch.Postfix))
            );

            //  Patch StardewValley.Objects.CrabPot.DayUpdate to not execute on combined crab pots so we can manually call the DayUpdate logic periodically, at our own calculated interval 
            //  (Unlike other machines, CrabPots have their output item set during DayUpdate, which is normally only called once per day at the start of the day)
            //  We will also set the output quantity in a postfix if Crab Pots are using UserConfig.ProcessingMode==ProcessingMode.MultiplyItems
            Harmony.Patch(
                original: AccessTools.Method(typeof(CrabPot), nameof(CrabPot.DayUpdate)),
                prefix: new HarmonyMethod(typeof(CrabPot_DayUpdatePatch), nameof(CrabPot_DayUpdatePatch.Prefix)),
                postfix: new HarmonyMethod(typeof(CrabPot_DayUpdatePatch), nameof(CrabPot_DayUpdatePatch.Postfix))
            );

            if (ModEntry.UserConfig.AllowCombiningScarecrows)
            {
                //  Patch StardewValley.Object.GetRadiusForScareCrow to account for the added multiplier given by combined scarecrows
                //  See also: ScarecrowPatchesV2.Postfix
                Harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.GetRadiusForScarecrow)),
                    postfix: new HarmonyMethod(typeof(ScarecrowPatchesV2), nameof(ScarecrowPatchesV2.Postfix))
                );
            }
        }
    }
}
