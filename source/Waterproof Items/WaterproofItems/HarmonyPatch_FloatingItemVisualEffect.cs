using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Network;
using Harmony;
using System.Reflection;

namespace WaterproofItems
{
    /// <summary>A Harmony patch that prevents item-containing debris sinking in water.</summary>
    public static class HarmonyPatch_FloatingItemVisualEffect
    {
        /// <summary>If true, this patch is currently applied.</summary>
        private static bool IsPatchApplied { get; set; } = false;
        /// <summary>The Harmony instance provided to this patch.</summary>
        public static HarmonyInstance Instance { get; set; } = null;

        /// <summary>Applies this Harmony patch to the game through the provided Harmony instance.</summary>
        public static void ApplyPatch()
        {
            if (IsPatchApplied || Instance == null) //if the patch is applied OR the static harmony instance is null
                return; //do nothing

            ModEntry.Instance.Monitor.Log($"Applying Harmony patch \"{nameof(HarmonyPatch_FloatingItemVisualEffect)}\": transpiling SDV method \"GameLocation.drawDebris(SpriteBatch)\".", LogLevel.Trace);

            Instance.Patch(
                original: AccessTools.Method(typeof(GameLocation), "drawDebris", new[] { typeof(SpriteBatch) }), //note: GameLocation.drawDebris access level is protected
                transpiler: new HarmonyMethod(typeof(HarmonyPatch_FloatingItemVisualEffect), nameof(drawDebris_Transpiler))
            );

            IsPatchApplied = true;
        }

        /// <summary>Removes this Harmony patch from the game through the provided Harmony instance.</summary>
        public static void RemovePatch()
        {
            if (!IsPatchApplied || Instance == null) //if the patch is NOT applied OR the static harmony instance is null
                return; //do nothing

            ModEntry.Instance.Monitor.Log($"Removing Harmony patch \"{nameof(HarmonyPatch_FloatingItemVisualEffect)}\": removing transpiler from SDV method \"GameLocation.drawDebris(SpriteBatch)\".", LogLevel.Trace);

            Instance.Unpatch(
                original: AccessTools.Method(typeof(GameLocation), "drawDebris", new[] { typeof(SpriteBatch) }), //note: GameLocation.drawDebris access level is protected
                patch: AccessTools.Method(typeof(HarmonyPatch_FloatingItemVisualEffect), nameof(drawDebris_Transpiler))
            );

            IsPatchApplied = false;
        }

        /// <summary>Replaces calls to <see cref="Chunk.position"/> with <see cref="GetFloatingPosition(Chunk)"/> and inserts calls to <see cref="UpdateRecentDebrisType(Debris)"/> before them.</summary>
        public static IEnumerable<CodeInstruction> drawDebris_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            try
            {
                MethodInfo updateTypeInfo = AccessTools.Method(typeof(HarmonyPatch_FloatingItemVisualEffect), nameof(UpdateRecentDebrisType), new Type[] { typeof(Debris) }); 
                CodeInstruction updateTypeCode = new CodeInstruction(OpCodes.Call, updateTypeInfo); //an instruction that calls the preceding method on a Debris

                FieldInfo chunkPositionInfo = AccessTools.Field(typeof(Chunk), nameof(Chunk.position));
                MethodInfo floatingPositionInfo = AccessTools.Method(typeof(HarmonyPatch_FloatingItemVisualEffect), nameof(GetFloatingPosition));

                List<CodeInstruction> patched = new List<CodeInstruction>(instructions); //make a copy of the instructions to modify

                for (int x = patched.Count - 1; x >= 0; x--) //for each instruction (looping backward)
                {
                    if //if this instruction is getting a Vector2 from Chunk.position
                    (
                        x > 0 //if this isn't the first instruction
                        && patched[x].opcode == OpCodes.Call //and this instruction is a call
                        && (patched[x].operand as MethodInfo)?.ReturnType == typeof(Vector2) //and this call returns a Vector2
                        && patched[x-1].opcode == OpCodes.Ldfld //and the previous instruction is a ldfld
                        && patched[x-1].operand?.Equals(chunkPositionInfo) == true //and the previous instruction loads Chunk.position
                    )
                    {
                        patched[x] = new CodeInstruction(OpCodes.Call, floatingPositionInfo); //replace the current instruction with a call to GetFloatingPosition
                    }
                    else if //if this instruction is getting a Debris
                    (
                        patched[x].operand is MethodInfo info //if this instruction calls a method
                        && info.ReturnType == typeof(Debris) //and the method returns a debris
                    )
                    {
                        patched.Insert(x + 1, updateTypeCode); //insert the "update type" method instruction after the current instruction
                    }
                }
                return patched; //return the patched instructions
            }
            catch (Exception ex)
            {
                ModEntry.Instance.Monitor.LogOnce($"Harmony patch \"{nameof(drawDebris_Transpiler)}\" has encountered an error and will not be applied:\n{ex.ToString()}", LogLevel.Error);
                return instructions; //return the original instructions
            }
        }

        /// <summary>The <see cref="Debris"/> most recently accessed by <see cref="GameLocation.drawDebris(SpriteBatch)"/>.</summary>
        public static Debris RecentDebris { get; set; } = null;

        /// <summary>Updates <see cref="RecentDebris"/> to refer to this <see cref="Debris"/>.</summary>
        /// <param name="debris">The <see cref="Debris"/> most recently accessed by <see cref="GameLocation.drawDebris(SpriteBatch)"/>.</param>
        /// <returns>The provided <see cref="Debris"/>.</returns>
        /// <remarks>Returning the provided debris allows it to remain on the stack when this method is inserted by a transpiler.</remarks>
        public static Debris UpdateRecentDebrisType(this Debris debris)
        {
            RecentDebris = debris; //update the reference to the latest debris
            return debris;
        }

        public static Vector2 GetFloatingPosition(NetPosition position)
        {
            if (RecentDebris?.IsAnItem() == true) //if this chunk's debris represents an item
            {
                Vector2 tilePosition = new Vector2((int)(position.X / 64.0) + 1, (int)(position.Y / 64.0) + 1); //get this chunk's tile position
                if (Game1.player.currentLocation.doesTileSinkDebris((int)tilePosition.X, (int)tilePosition.Y, RecentDebris.debrisType)) //if this chunk is floating (i.e. should sink on its current tile)
                {
                    int offset = (int)(Math.Sin((double)((Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2500.0) + (position.X * 5)) / 2500.0 * (2.0 * Math.PI)) * 6.0); //calculate this chunk's offset
                    return new Vector2((int)position.X, position.Y + offset); //return the chunk's position with the offset added to its Y value
                }
            }

            //if this chunk is NOT floating
            return position.Value; //return the chunk's actual position
        }
    }
}
