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
    using global::HorseOverhaul.Patches;
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Characters;
    using StardewValley.Locations;
    using StardewValley.TerrainFeatures;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    public class Patcher
    {
        private static HorseOverhaul mod;

        public static void PatchAll(HorseOverhaul horseOverhaul)
        {
            mod = horseOverhaul;

            var harmony = new Harmony(mod.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                   original: AccessTools.Method(typeof(Desert), "playerReachedBusDoor", new Type[] { typeof(Character), typeof(GameLocation) }),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(PreventSoftlock)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getMovementSpeed)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(ChangeHorseMovementSpeed)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Horse), nameof(Horse.checkAction)),
                   prefix: new HarmonyMethod(typeof(Patcher), nameof(CheckForPetting)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(Horse), nameof(Horse.PerformDefaultHorseFootstep)),
                   postfix: new HarmonyMethod(typeof(Patcher), nameof(PerformDefaultHorseFootstep)));

                harmony.Patch(
                   original: AccessTools.Method(typeof(FarmerSprite), "checkForFootstep"),
                   transpiler: new HarmonyMethod(typeof(Patcher), nameof(FixMultiplayerFootstepDisplay)));

                StableAndSaddleBagPatches.ApplyPatches(horseOverhaul, harmony);

                ThinHorsePatches.ApplyPatches(horseOverhaul, harmony);

                InteractPatches.ApplyPatches(horseOverhaul, harmony);

                HorseDrawPatches.ApplyPatches(horseOverhaul, harmony);
            }
            catch (Exception e)
            {
                mod.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        // transpiler checked for 1.6
        public static IEnumerable<CodeInstruction> FixMultiplayerFootstepDisplay(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            try
            {
                var instructionList = instructions.ToList();

                // use nops instead of removeal in case there are labels (unlikely but more safe)
                // remove the initial 4 instructions, which correspond to 'if (Game1.player.isRidingHorse()) return;'
                for (int i = 0; i < 4; i++)
                {
                    instructionList[i].opcode = OpCodes.Nop;
                    instructionList[i].operand = null;
                }
                // the null check is now at the top

                // get the return instruction address
                // index 6 points to the first Brfalse_S
                Label branchDestination = (Label)instructionList[6].operand;

                // insert 'if (this.owner.isRidingHorse()) return;' after the null check
                instructionList.InsertRange(3, new List<CodeInstruction>()
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Patcher), nameof(Patcher.CheckSpriteOwnerIsRidingHorse))),
                    new CodeInstruction(OpCodes.Brtrue_S, branchDestination)
                });

                return instructionList.AsEnumerable();
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a transpiler patch", e);
                return instructions;
            }
        }

        public static bool CheckSpriteOwnerIsRidingHorse(FarmerSprite sprite)
        {
            if (mod.Config.HorseHoofstepEffects)
            {
                // multiplayer/split screen compatibility
                return sprite.Owner is Farmer farmer && farmer.isRidingHorse();
            }
            else
            {
                return Game1.player.isRidingHorse();
            }
        }

        public static void PerformDefaultHorseFootstep(Horse __instance, string step_type)
        {
            if (!mod.Config.HorseHoofstepEffects || __instance.IsTractor())
            {
                return;
            }

            var rider = __instance.rider;

            if (rider?.currentLocation == null || rider.currentLocation != Game1.currentLocation)
            {
                return;
            }

            if (step_type != null)
            {
                switch (step_type)
                {
                    case "Dirt":
                        step_type = "sandyStep";
                        break;

                    case "Stone":
                        step_type = "stoneStep";
                        break;

                    case "Grass":
                        step_type = Game1.GetSeasonForLocation(rider.currentLocation) == Season.Winter ? "snowyStep" : "grassyStep";
                        break;

                    case "Wood":
                        step_type = "woodyStep";
                        break;
                }
            }

            Vector2 riderTileLocation = rider.Tile;

            if (rider.currentLocation.terrainFeatures.TryGetValue(riderTileLocation, out var terrainFeature)
                && terrainFeature is Flooring flooring)
            {
                step_type = flooring.getFootstepSound();
            }

            bool isLastFrame = false;
            float width = 32f; // horse.Sprite.SpriteWidth
            float snowXOffset = 0f;
            float dustXOffset = 0f;

            switch (__instance.FacingDirection)
            {
                case Game1.up:
                    isLastFrame = __instance?.Sprite.CurrentFrame == 17;
                    snowXOffset = mod.Config.ThinHorse ? 8f : width;
                    dustXOffset = mod.Config.ThinHorse ? -8 : width - 8;
                    break;

                case Game1.down:
                    isLastFrame = __instance?.Sprite.CurrentFrame == 3;
                    snowXOffset = mod.Config.ThinHorse ? 8f : width;
                    dustXOffset = mod.Config.ThinHorse ? -8 : width - 8;
                    break;

                case Game1.left:
                    isLastFrame = __instance?.Sprite.CurrentFrame == 10;
                    snowXOffset = mod.Config.ThinHorse ? 16f : width + 8f;
                    dustXOffset = mod.Config.ThinHorse ? 8 : width + 8;
                    break;

                case Game1.right:
                    isLastFrame = __instance?.Sprite.CurrentFrame == 10;
                    snowXOffset = mod.Config.ThinHorse ? 16f : 2 * width - 8f;
                    dustXOffset = mod.Config.ThinHorse ? -8 : width - 8;
                    break;
            }

            bool isFacingLeftOrRight = rider.facingDirection.Value is Game1.right or Game1.left;

            Vector2 position = __instance.Position;

            switch (step_type)
            {
                case "sandyStep":
                    rider.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(128, 2948, 64, 64), 80f, 8, 0, new Vector2(position.X + dustXOffset + (float)Game1.random.Next(-8, 8), position.Y + (float)(Game1.random.Next(-3, -1) * 4)), false, Game1.random.NextDouble() < 0.5, position.Y / 10000f, 0.03f, Color.Khaki * 0.45f, 0.75f + (float)Game1.random.Next(-3, 4) * 0.05f, 0f, 0f, 0f, false));
                    rider.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(128, 2948, 64, 64), 80f, 8, 0, new Vector2(position.X + dustXOffset + (float)Game1.random.Next(-4, 4), position.Y + (float)(Game1.random.Next(-3, -1) * 4)), false, Game1.random.NextDouble() < 0.5, position.Y / 10000f, 0.03f, Color.Khaki * 0.45f, 0.55f + (float)Game1.random.Next(-3, 4) * 0.05f, 0f, 0f, 0f, false)
                    {
                        delayBeforeAnimationStart = 20
                    });
                    break;

                case "snowyStep":
                    rider.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(position.X + snowXOffset + (float)(Game1.random.Next(-4, 4) * 4), position.Y + 8f + (float)(Game1.random.Next(-4, 4) * 4)), false, false, position.Y / 10000000f, 0.01f, Color.White, 3f + (float)Game1.random.NextDouble(), 0f, isFacingLeftOrRight ? -0.7853982f : 0f, 0f, false));

                    // do two footprints so we have a total of 4 (footstep event gets raised on 3 frames)
                    if (isLastFrame)
                    {
                        rider.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite("LooseSprites\\Cursors", new Microsoft.Xna.Framework.Rectangle(247, 407, 6, 6), 2000f, 1, 10000, new Vector2(position.X + snowXOffset + (float)(Game1.random.Next(-4, 4) * 4), position.Y + 8f + (float)(Game1.random.Next(-4, 4) * 4)), false, false, position.Y / 10000000f, 0.01f, Color.White, 3f + (float)Game1.random.NextDouble(), 0f, isFacingLeftOrRight ? -0.7853982f : 0f, 0f, false)
                        {
                            delayBeforeAnimationStart = 20
                        });
                    }
                    break;

                default:
                    return;
            }
        }

        public static bool PreventSoftlock(Character c)
        {
            if (c != null && c is Farmer player && player.isRidingHorse())
            {
                Game1.drawObjectDialogue(mod.Helper.Translation.Get("HorseWarning"));
                return false;
            }
            else
            {
                return true;
            }
        }

        public static void ChangeHorseMovementSpeed(Farmer __instance, ref float __result)
        {
            if (mod.Config.MovementSpeed && !Game1.eventUp && (Game1.CurrentEvent == null || Game1.CurrentEvent.playerControlSequence) && !(__instance.hasBuff("19") && Game1.CurrentEvent == null))
            {
                Horse horse = __instance.mount;

                if (horse != null && !horse.IsTractor() && mod?.Horses != null)
                {
                    var horseW = mod.Horses.Where(h => h?.Horse?.HorseId == horse.HorseId).FirstOrDefault();

                    if (horseW == null)
                    {
                        return;
                    }

                    float addedMovementSpeed = horseW.GetMovementSpeedBonus();

                    if (__instance.movementDirections.Count > 1)
                    {
                        addedMovementSpeed *= 0.7f;
                    }

                    __result += addedMovementSpeed;
                }
            }
        }

        public static bool CheckForPetting(Horse __instance, ref bool __result)
        {
            if (!mod.Config.Petting || __instance.IsTractor())
            {
                return true;
            }

            var horseW = mod.Horses.Where(h => h?.Horse?.HorseId == __instance.HorseId).FirstOrDefault();

            if (horseW != null && !horseW.WasPet)
            {
                horseW.JustGotPetted();

                if (mod.Config.ThinHorse)
                {
                    __instance.doEmote(Character.heartEmote);
                }

                __result = true;
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}