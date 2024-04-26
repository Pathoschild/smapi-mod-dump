/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace HorseOverhaul
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using xTile.Dimensions;

    internal class InteractPatches
    {
        private static HorseOverhaul mod;

        internal static void ApplyPatches(HorseOverhaul horseOverhaul, Harmony harmony)
        {
            mod = horseOverhaul;

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.checkAction)),
               transpiler: new HarmonyMethod(typeof(InteractPatches), nameof(AllowInteractWhileRiding)));

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.animateOnce)),
               prefix: new HarmonyMethod(typeof(InteractPatches), nameof(DontAnimateHorsePickUp)));
        }

        public static bool DontAnimateHorsePickUp(Farmer __instance, int whichAnimation)
        {
            if (!__instance.isRidingHorse())
            {
                return true;
            }

            switch (whichAnimation)
            {
                case 279 + 0:
                case 279 + 1:
                case 279 + 2:
                case 279 + 3:

                    __instance.noMovementPause = Math.Max(__instance.noMovementPause, 200);
                    return false;

                default:
                    return true;
            }
        }

        // transpiler checked for 1.6.4
        public static IEnumerable<CodeInstruction> AllowInteractWhileRiding(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                var instructionsList = instructions.ToList();

                int foundSpawnedObject = -1;
                OpCode foundSpawnedObjectLocalVariable = OpCodes.Ldloc_2;
                int foundTerrainFeature = -1;

                for (int i = 0; i < instructionsList.Count; i++)
                {
                    if (instructionsList[i].opcode == OpCodes.Callvirt
                        && instructionsList[i].operand != null
                        && instructionsList[i].ToString().Contains("isRidingHorse"))
                    {
                        if (foundSpawnedObject < 0)
                        {
                            if (i + 4 < instructionsList.Count
                                && instructionsList[i + 1].opcode == OpCodes.Brfalse_S
                                && instructionsList[i + 3].opcode == OpCodes.Isinst
                                && instructionsList[i + 3].operand != null
                                && instructionsList[i + 3].operand.ToString().Contains("StardewValley.Fence"))
                            {
                                foundSpawnedObjectLocalVariable = instructionsList[i + 2].opcode;
                                foundSpawnedObject = i;
                            }
                        }

                        if (foundTerrainFeature < 0)
                        {
                            if (i + 10 < instructionsList.Count
                                && instructionsList[i + 1].opcode == OpCodes.Brfalse_S
                                && instructionsList[i + 2].opcode == OpCodes.Ldarg_3
                                && instructionsList[i + 3].opcode == OpCodes.Callvirt
                                && instructionsList[i + 3].operand != null
                                && instructionsList[i + 3].operand.ToString().Contains("get_mount")
                                //&& instructionsList[i + 4].opcode == OpCodes.Ldarg_3
                                //&& instructionsList[i + 5].opcode == OpCodes.Ldarg_0
                                && instructionsList[i + 6].opcode == OpCodes.Callvirt
                                && instructionsList[i + 6].operand != null
                                && instructionsList[i + 6].operand.ToString().Contains("checkAction")
                                && instructionsList[i + 8].opcode == OpCodes.Ldc_I4_1
                                && instructionsList[i + 9].opcode == OpCodes.Ret)
                            {
                                foundTerrainFeature = i;
                            }
                        }
                    }
                }

                // checking foundTerrainFeature before foundSpawnedObject is important, because we insert into the list

                if (foundTerrainFeature > 0)
                {
                    Label returnTrueLabel = generator.DefineLabel();
                    instructionsList[foundTerrainFeature + 8].labels.Add(returnTrueLabel);

                    var foundTerrainFeatureInsert = new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(InteractPatches), nameof(InteractWithTerrainFeatureWhileRiding)));

                    var instructionsToInsert = new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_0),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        foundTerrainFeatureInsert,
                        new CodeInstruction(OpCodes.Brtrue, returnTrueLabel)
                    };

                    instructionsList.InsertRange(foundTerrainFeature + 2, instructionsToInsert);
                }

                if (foundSpawnedObject > 0)
                {
                    var foundSpawnObjectInsert = new CodeInstruction(OpCodes.Call,
                        AccessTools.Method(typeof(InteractPatches), nameof(IsAllowedHorsePickUp)));

                    var instructionsToInsert = new CodeInstruction[]
                    {
                        new CodeInstruction(OpCodes.Ldarg_3),
                        new CodeInstruction(OpCodes.Ldarg_1),
                        new CodeInstruction(foundSpawnedObjectLocalVariable),
                        foundSpawnObjectInsert,
                        new CodeInstruction(OpCodes.Or)
                    };

                    instructionsList.InsertRange(foundSpawnedObject + 4, instructionsToInsert);
                }

                return instructionsList.AsEnumerable();
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a transpiler patch", e);
                return instructions;
            }
        }

        // obj is Fence -> obj is Fence || IsAllowedHorsePickUp(tilePos, who, obj)
        public static bool IsAllowedHorsePickUp(Farmer who, Location tileLocation, StardewValley.Object obj)
        {
            if (!mod.Config.EnableLimitedInteractionWhileRiding)
            {
                return false;
            }

            if (obj == null)
            {
                return false;
            }

            var tilePos = new Vector2(tileLocation.X, tileLocation.Y);

            bool no = (tilePos == who.Tile && !obj.isPassable() && (obj is not Fence fence || !fence.isGate.Value));
            bool no2 = (obj.Type == "Crafting" || obj.Type == "interactive");

            bool yes = mod.Config.InteractWithForageWhileRiding && obj.IsSpawnedObject && obj.isForage();
            bool yes2 = mod.Config.InteractWithTappersWhileRiding && obj.IsTapper();
            // '|=' is 'x = x | y', so if yes2 is true it still calculates y
            yes2 = yes2 || (mod.Config.InteractWithMushroomLogsAndBoxesWhileRiding && IsMushroomLogOrBox(obj));

            bool allowForage = !no && !no2 && yes;
            bool allowMachine = !no && no2 && yes2;

            return allowForage || allowMachine;
        }

        private static bool IsMushroomLogOrBox(StardewValley.Object obj)
        {
            return obj.QualifiedItemId is "(BC)128" or "(BC)MushroomLog";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidImplicitNetFieldCast:Netcode types shouldn't be implicitly converted", Justification = "No other choice")]
        public static bool InteractWithTerrainFeatureWhileRiding(GameLocation location, Location tileLocation)
        {
            if (!mod.Config.EnableLimitedInteractionWhileRiding)
            {
                return false;
            }

            var tileRect = new Microsoft.Xna.Framework.Rectangle(tileLocation.X * 64, tileLocation.Y * 64, 64, 64);

            foreach (KeyValuePair<Vector2, TerrainFeature> terrainFeaturePair in location.terrainFeatures.Pairs)
            {
                TerrainFeature terrainFeature = terrainFeaturePair.Value;

                bool IsAllowed = false;

                IsAllowed |= terrainFeature is Tree && mod.Config.InteractWithTreesWhileRiding;
                IsAllowed |= terrainFeature is FruitTree && mod.Config.InteractWithFruitTreesWhileRiding;
                IsAllowed |= terrainFeature is Bush && mod.Config.InteractWithBushesWhileRiding;
                IsAllowed |= terrainFeature is HoeDirt dirt && mod.Config.InteractWithForageWhileRiding
                    && dirt.crop?.forageCrop.Value == true && dirt.crop.whichForageCrop.Value == Crop.forageCrop_springOnionID;

                if (IsAllowed)
                {
                    if (terrainFeature.getBoundingBox().Intersects(tileRect) && terrainFeature.performUseAction(terrainFeaturePair.Key))
                    {
                        Game1.haltAfterCheck = false;
                        return true;
                    }
                }
            }

            if (location.largeTerrainFeatures != null && mod.Config.InteractWithBushesWhileRiding)
            {
                foreach (LargeTerrainFeature largeTF in location.largeTerrainFeatures)
                {
                    if (largeTF is not Bush)
                    {
                        continue;
                    }

                    if (largeTF.getBoundingBox().Intersects(tileRect) && largeTF.performUseAction(largeTF.Tile))
                    {
                        Game1.haltAfterCheck = false;
                        return true;
                    }
                }
            }

            return false;
        }
    }
}