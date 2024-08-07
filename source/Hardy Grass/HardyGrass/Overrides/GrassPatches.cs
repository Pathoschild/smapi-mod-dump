/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DiscipleOfEris/HardyGrass
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Tools;
using StardewValley.TerrainFeatures;
using HarmonyLib;

namespace HardyGrass
{
    public class GrassPatches
    {
        public static void ApplyPatches(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.dayUpdate)),
                prefix: new HarmonyMethod(typeof(GrassPatches), nameof(GrassPatches.dayUpdate_Prefix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.reduceBy)),
                postfix: new HarmonyMethod(typeof(GrassPatches), nameof(GrassPatches.reduceBy_Postfix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.doCollisionAction)),
                prefix: new HarmonyMethod(typeof(GrassPatches), nameof(GrassPatches.doCollisionAction_Prefix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.performToolAction)),
                prefix: new HarmonyMethod(typeof(GrassPatches), nameof(GrassPatches.performToolAction_Prefix)),
                postfix: new HarmonyMethod(typeof(GrassPatches), nameof(GrassPatches.performToolAction_Postfix)));
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.TerrainFeatures.Grass), nameof(StardewValley.TerrainFeatures.Grass.draw)),
                prefix: new HarmonyMethod(typeof(GrassPatches), nameof(GrassPatches.draw_Prefix)));
        }

        public static bool dayUpdate_Prefix(Grass __instance, GameLocation environment, Vector2 tileLocation)
        {
            if (__instance.grassType.Value == 1 && !environment.GetSeasonForLocation().Equals("winter") && __instance.numberOfWeeds.Value < 4)
            {
                __instance.numberOfWeeds.Value = Utility.Clamp(__instance.numberOfWeeds.Value + ModEntry.CalculateTuftsToAdd(ModEntry.GrassIsQuick(__instance), __instance.numberOfWeeds.Value == 0 ? ModEntry.GrowthType.Cut : ModEntry.GrowthType.Standard), 0, 4);
            }
            __instance.setUpRandom(tileLocation);

            return false;
        }

        public static void reduceBy_Postfix(Grass __instance, ref bool __result)
        {
            __result = false;

            __instance.numberOfWeeds.Value = Math.Max(0, __instance.numberOfWeeds.Value);

            if (__instance.numberOfWeeds.Value <= 0)
            {
                __result = Game1.random.NextDouble() >= (ModEntry.GrassIsQuick(__instance) ? ModEntry.config.quickHardiness : ModEntry.config.vanillaHardiness);
            }
        }

        public static bool doCollisionAction_Prefix(Grass __instance, float ___maxShake, float ___shakeRotation, Rectangle positionOfCollider, int speedOfCollision, Vector2 tileLocation, Character who, GameLocation location)
        {
            if (location != Game1.currentLocation)
            {
                return false;
            }
            if (speedOfCollision > 0 && ___maxShake == 0f && positionOfCollider.Intersects(__instance.getBoundingBox(tileLocation)))
            {
                if ((who == null || !(who is FarmAnimal)) && Grass.grassSound != null && !Grass.grassSound.IsPlaying && Utility.isOnScreen(new Point((int)tileLocation.X, (int)tileLocation.Y), 2, location) && Game1.soundBank != null)
                {
                    Grass.grassSound = Game1.soundBank.GetCue("grassyStep");
                    Grass.grassSound.Play();
                }

                ModEntry.GrassShakeMethodInfo.Invoke(__instance, new object[] { (float)Math.PI / 8f / (float)((5 + Game1.player.addedSpeed) / speedOfCollision), (float)Math.PI / 80f / (float)((5 + Game1.player.addedSpeed) / speedOfCollision), (float)positionOfCollider.Center.X > tileLocation.X * 64f + 32f });
                //this.shake((float)Math.PI / 8f / (float)((5 + Game1.player.addedSpeed) / speedOfCollision), (float)Math.PI / 80f / (float)((5 + Game1.player.addedSpeed) / speedOfCollision), (float)positionOfCollider.Center.X > tileLocation.X * 64f + 32f);
            }
            if (who is Farmer && Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon && ((MeleeWeapon)Game1.player.CurrentTool).isOnSpecial && ((MeleeWeapon)Game1.player.CurrentTool).type.Value == 0 && Math.Abs(___shakeRotation) < 0.001f && __instance.performToolAction(Game1.player.CurrentTool, -1, tileLocation, location))
            {
                Game1.currentLocation.terrainFeatures.Remove(tileLocation);
            }
            if (__instance.numberOfWeeds.Value > 0 && who is Farmer)
            {
                (who as Farmer).temporarySpeedBuff = -1f;
                if (__instance.grassType.Value == 6)
                {
                    (who as Farmer).temporarySpeedBuff = -3f;
                }
            }

            return false;
        }

        public static bool performToolAction_Prefix(Grass __instance, ref bool __result, Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            // If the grass is already fully cut, skip the default action entirely.
            __result = false;
            return __instance.numberOfWeeds.Value > 0;
        }

        public static void performToolAction_Postfix(Grass __instance, ref bool __result, Tool t, int explosion, Vector2 tileLocation, GameLocation location)
        {
            __instance.numberOfWeeds.Value = Math.Max(0, __instance.numberOfWeeds.Value);

            if (location == null)
            {
                location = Game1.currentLocation;
            }

            if (__instance.numberOfWeeds.Value > 0)
            {
                return;
            }

            if (t is Hoe || t is Axe || t is Pickaxe)
            {
                DelayedAction.playSoundAfterDelay("daggerswipe", 50);
                location.playSound("swordswipe");

                Color c = Color.Green;
                switch (__instance.grassType.Value)
                {
                    case 1:
                        switch (location.GetSeasonForLocation())
                        {
                            case "spring":
                                c = new Color(60, 180, 58);
                                break;
                            case "summer":
                                c = new Color(110, 190, 24);
                                break;
                            case "fall":
                                c = new Color(219, 102, 58);
                                break;
                        }
                        break;
                    case 2:
                        c = new Color(148, 146, 71);
                        break;
                    case 3:
                        c = new Color(216, 240, 255);
                        break;
                    case 4:
                        c = new Color(165, 93, 58);
                        break;
                    case 6:
                        c = Color.White * 0.6f;
                        break;
                }

                ModEntry.Game1Multiplayer.broadcastSprites(location, new TemporaryAnimatedSprite(28, tileLocation * 64f + new Vector2(Game1.random.Next(-16, 16), Game1.random.Next(-16, 16)), c, 8, Game1.random.NextDouble() < 0.5, Game1.random.Next(60, 100)));

                __result = true;
                return;
            }
            else if (__result)
            {
                __result = Game1.random.NextDouble() >= (ModEntry.GrassIsQuick(__instance) ? ModEntry.config.quickHardiness : ModEntry.config.vanillaHardiness);
            }
        }

        public static bool draw_Prefix(Grass __instance, SpriteBatch spriteBatch, Vector2 tileLocation, int[] ___whichWeed, int[] ___offset1, int[] ___offset2, int[] ___offset3, int[] ___offset4, bool[] ___flip, double[] ___shakeRandom, float ___shakeRotation)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 pos = ((i != 4) ? (tileLocation * 64f + new Vector2((float)(i % 2 * 64 / 2 + ___offset3[i] * 4 - 4) + 30f, i / 2 * 64 / 2 + ___offset4[i] * 4 + 40)) : (tileLocation * 64f + new Vector2((float)(16 + ___offset1[i] * 4 - 4) + 30f, 16 + ___offset2[i] * 4 + 40)));
                spriteBatch.Draw(i >= __instance.numberOfWeeds.Value ? ModEntry.texture.Value : __instance.texture.Value, Game1.GlobalToLocal(Game1.viewport, pos), new Rectangle(___whichWeed[i] * 15, __instance.grassSourceOffset.Value, 15, 20), Color.White, ___shakeRotation / (float)(___shakeRandom[i] + 1.0), new Vector2(7.5f, 17.5f), 4f, ___flip[i] ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (pos.Y - 24f) / 10000f + pos.X / 1E+07f);
            }

            return false;
        }
    }
}