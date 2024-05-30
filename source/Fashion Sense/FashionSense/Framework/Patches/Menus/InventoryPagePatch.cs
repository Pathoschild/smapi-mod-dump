/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using FashionSense.Framework.Utilities;
using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace FashionSense.Framework.Patches.Menus
{
    internal class InventoryPagePatch : PatchTemplate
    {
        private readonly Type _menu = typeof(InventoryPage);

        internal InventoryPagePatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_menu, nameof(InventoryPage.draw), new[] { typeof(SpriteBatch) }), transpiler: new HarmonyMethod(GetType(), nameof(DrawTranspiler)));
        }

        private static IEnumerable<CodeInstruction> DrawTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var list = instructions.ToList();

                // Get the indices to insert at
                List<int> indices = new List<int>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Callvirt && list[i].operand is not null && list[i].operand.ToString().Contains("draw", System.StringComparison.OrdinalIgnoreCase) && list[i + 1].opcode == OpCodes.Ldsfld && list[i + 1].operand.ToString().Contains("timeOfDay", System.StringComparison.OrdinalIgnoreCase))
                    {
                        // Get the line for (Game1.timeOfDay >= 1900)
                        indices.Add(i + 2);
                    }
                }

                // Insert the changes at the specified indices
                foreach (var index in indices.OrderByDescending(i => i))
                {
                    list.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(InventoryPagePatch), nameof(HandleNightDrawOverlay))));
                }

                return list;
            }
            catch (System.Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.InventoryMenu.draw: {e}", LogLevel.Error);
                return instructions;
            }
        }

        private static int HandleNightDrawOverlay(int originalTimeOfDay)
        {
            if (AppearanceHelpers.GetCurrentlyEquippedModels(Game1.player, Game1.player.FacingDirection).Count == 0)
            {
                return originalTimeOfDay;
            }

            // Skip night overlay draw if any Fashion Sense appearances are used, until better handling is implemented (need to standardize DrawTool.OverrideColor usage)
            return int.MaxValue;
        }
    }
}
