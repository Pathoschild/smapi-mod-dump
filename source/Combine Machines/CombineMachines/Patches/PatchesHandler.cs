/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Videogamers0/SDV-CombineMachines
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
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
            HarmonyInstance Harmony = HarmonyInstance.Create(ModEntry.ModInstance.ModManifest.UniqueID);

            //  Patch StardewValley.Object.maximumStackSize to always return 1 if a machine has been combined with other machines
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.maximumStackSize)),
                prefix: new HarmonyMethod(typeof(MaximumStackSizePatch), nameof(MaximumStackSizePatch.Prefix))
            );

            //  Patch StardewValley.Object.canStackWith to always return false if either one has already been combined with other machines
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.canStackWith)),
                prefix: new HarmonyMethod(typeof(CanStackWithPatch), nameof(CanStackWithPatch.Prefix))
            );

            //  Patch StardewValley.Object.performRemoveAction to detect when a combined machine is removed from a game tile (such as by hitting a furnace with a pickaxe)
            //  to then refund the other copies of the machine that were combined with the one that just got removed
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performRemoveAction)),
                prefix: new HarmonyMethod(typeof(PerformRemoveActionPatch), nameof(PerformRemoveActionPatch.Prefix))
            );

            //  Patch StardewValley.Object.performObjectDropInAction to detect when the player inserts items into a machine, and make the game take additional inputs to process
            //  if they inserted the inputs into a combined machine
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.performObjectDropInAction)),
                prefix: new HarmonyMethod(typeof(PerformObjectDropInActionPatch), nameof(PerformObjectDropInActionPatch.Prefix)),
                postfix: new HarmonyMethod(typeof(PerformObjectDropInActionPatch), nameof(PerformObjectDropInActionPatch.Postfix))
            );

            //  Patch StardewValley.Object.minutesElapsed to detect when a machines output is ready for collecting, and when that happens, increase the output quantity
            //  to account for how many machines were combined
            Harmony.Patch(
                original: AccessTools.Method(typeof(SObject), nameof(SObject.minutesElapsed)),
                prefix: new HarmonyMethod(typeof(MinutesElapsedPatch), nameof(MinutesElapsedPatch.Prefix)),
                postfix: new HarmonyMethod(typeof(MinutesElapsedPatch), nameof(MinutesElapsedPatch.Postfix))
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

            //  Patch StardewValley.Menus.InventoryMenu.draw to also draw a yellow border around other machines that the current CursorSlotItem can be combined with.
            Harmony.Patch(
                original: AccessTools.Method(typeof(InventoryMenu), nameof(InventoryMenu.draw), new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(typeof(InventoryMenuDrawPatch), nameof(InventoryMenuDrawPatch.Postfix))
            );
        }
    }
}
