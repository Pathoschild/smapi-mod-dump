/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FashionSense
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;

namespace FashionSense.Framework.Patches.Objects
{
    internal class ColoredObjectPatch : PatchTemplate
    {
        private readonly System.Type _entity = typeof(ColoredObject);

        internal ColoredObjectPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_entity, nameof(ColoredObject.drawWhenHeld), new[] { typeof(SpriteBatch), typeof(Vector2), typeof(Farmer) }), transpiler: new HarmonyMethod(GetType(), nameof(DrawWhenHeldTranspiler)));

            // Handle BAGI objects, as they prefix (and skip) the vanilla Object.drawWhenHeld method for their items
            if (PatchTemplate.IsBAGIUsed())
            {
                try
                {
                    if (System.Type.GetType("BetterArtisanGoodIcons.Patches.SObjectPatches.DrawWhenHeldPatch, BetterArtisanGoodIcons") is System.Type bagiDrawWhenHeldPatch && bagiDrawWhenHeldPatch != null)
                    {
                        harmony.Patch(AccessTools.Method("BetterArtisanGoodIcons.Patches.SObjectPatches.DrawWhenHeldPatch:Prefix"), transpiler: new HarmonyMethod(GetType(), nameof(DrawWhenHeldTranspiler)));
                    }
                    else
                    {
                        throw new System.Exception("BetterArtisanGoodIcons.Patches.SObjectPatches.DrawWhenHeldPatch:Prefix not found");
                    }

                    _monitor.Log($"Patched BAGI.DrawWhenHeldPatch successfully via {this.GetType().Name}", LogLevel.Trace);
                }
                catch (System.Exception ex)
                {
                    _monitor.Log($"Failed to patch BAGI.DrawWhenHeldPatch in {this.GetType().Name}: This may cause certain BAGI objects to be drawn incorrectly when held", LogLevel.Warn);
                    _monitor.Log($"Patch for BAGI.DrawWhenHeldPatch failed in {this.GetType().Name}: {ex}", LogLevel.Trace);
                }
            }
        }

        private static IEnumerable<CodeInstruction> DrawWhenHeldTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                var list = instructions.ToList();

                // Get the indices to insert at
                List<int> indices = new List<int>();
                List<int> replaceIndices = new List<int>();
                for (int i = 0; i < list.Count; i++)
                {
                    if (list[i].opcode == OpCodes.Call && list[i].operand is not null && list[i].operand.ToString().Contains("Max", System.StringComparison.OrdinalIgnoreCase))
                    {
                        indices.Add(i);
                    }
                    else if (list[i].opcode == OpCodes.Callvirt && list[i].operand is not null && list[i].operand.ToString().Contains("getDrawLayer", System.StringComparison.OrdinalIgnoreCase))
                    {
                        indices.Add(i);
                    }
                }

                // Insert the changes at the specified indices
                foreach (var index in indices.OrderByDescending(i => i))
                {
                    list.Insert(index + 1, new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ObjectPatch), nameof(ObjectPatch.AdjustLayerDepthForHeldObjects))));
                }
                return list;
            }
            catch (System.Exception e)
            {
                _monitor.Log($"There was an issue modifying the instructions for StardewValley.Object.drawPlayerHeldObject: {e}", LogLevel.Error);
                return instructions;
            }
        }
    }
}
