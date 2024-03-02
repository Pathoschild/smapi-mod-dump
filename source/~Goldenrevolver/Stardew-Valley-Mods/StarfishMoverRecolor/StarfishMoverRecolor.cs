/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace StarfishMoverRecolor
{
    using HarmonyLib;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI;
    using StardewValley;
    using StardewValley.Locations;
    using StardewValley.Projectiles;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using xTile.Dimensions;

    public class StarfishMoverRecolor : Mod, IAssetEditor
    {
        private bool checkedThisFrame = false;

        private Texture2D clamTexture;

        private StarfishMoverRecolorConfig config;

        private static StarfishMoverRecolor mod;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<StarfishMoverRecolorConfig>();

            Helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            Helper.Events.Player.Warped += Player_Warped;

            clamTexture = Helper.Content.Load<Texture2D>($"assets/starfish_texture.png");

            mod = this;

            var harmony = new Harmony(this.ModManifest.UniqueID);

            try
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(MermaidHouse), nameof(MermaidHouse.checkAction)),
                    prefix: new HarmonyMethod(typeof(StarfishMoverRecolor), nameof(CheckAction)));
                harmony.Patch(
                    original: AccessTools.Method(typeof(MermaidHouse), nameof(MermaidHouse.playClamTone), new[] { typeof(int), typeof(Farmer) }),
                    prefix: new HarmonyMethod(typeof(StarfishMoverRecolor), nameof(PlayClamTone)));
                harmony.Patch(
                    original: AccessTools.Method(typeof(MermaidHouse), nameof(MermaidHouse.drawAboveAlwaysFrontLayer)),
                    prefix: new HarmonyMethod(typeof(StarfishMoverRecolor), nameof(DrawAboveAlwaysFrontLayer)));
            }
            catch (Exception e)
            {
                this.ErrorLog("Error while trying to setup required patches:", e);
            }
        }

        private void Player_Warped(object sender, StardewModdingAPI.Events.WarpedEventArgs e)
        {
            if (e?.NewLocation is MermaidHouse)
            {
                Helper.Content.InvalidateCache("Maps/mermaid_house_tiles");
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return asset.AssetNameEquals("Maps/mermaid_house_tiles") && clamTexture != null;
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Maps/mermaid_house_tiles") && clamTexture != null)
            {
                var editor = asset.AsImage();

                for (int i = 0; i < config.ClamPositions.Length; i++)
                {
                    editor.PatchImage(clamTexture, null, new Microsoft.Xna.Framework.Rectangle((int)config.ClamPositions[i].X * 16, (int)config.ClamPositions[i].Y * 16, 16, 16), PatchMode.Overlay);
                }
            }
        }

        private void GameLoop_UpdateTicked(object sender, StardewModdingAPI.Events.UpdateTickedEventArgs e)
        {
            checkedThisFrame = false;
        }

        /// <summary>
        /// Small helper method to log to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        public void DebugLog(object o)
        {
            Monitor.Log(o == null ? "null" : o.ToString(), LogLevel.Debug);
        }

        /// <summary>
        /// Small helper method to log an error to the console because I keep forgetting the signature
        /// </summary>
        /// <param name="o">the object I want to log as a string</param>
        /// <param name="e">an optional error message to log additionally</param>
        public void ErrorLog(object o, Exception e = null)
        {
            string baseMessage = o == null ? "null" : o.ToString();

            string errorMessage = e == null ? string.Empty : $"\n{e.Message}\n{e.StackTrace}";

            Monitor.Log(baseMessage + errorMessage, LogLevel.Error);
        }

        public static bool CheckAction(MermaidHouse __instance, Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who, ref bool __result)
        {
            try
            {
                if (mod.checkedThisFrame && mod.config.ExactClamClickLocationCheck)
                {
                    return true;
                }

                mod.checkedThisFrame = true;

                for (int i = 0; i < mod.config.ClamPositions.Length; i++)
                {
                    if (tileLocation.X == mod.config.ClamPositions[i].X && tileLocation.Y == mod.config.ClamPositions[i].Y)
                    {
                        __instance.playClamTone(i, who);
                        __result = true;
                        return false;
                    }
                }

                // return base.checkAction(tileLocation, viewport, who);
                return true;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static bool PlayClamTone(MermaidHouse __instance, int which, Farmer who, ref Farmer ___pearlRecipient, ref List<int> ___lastFiveClamTones, ref float ___fairyTimer, ref Texture2D ___mermaidSprites, ref float ___oldStopWatchTime)
        {
            try
            {
                if (___oldStopWatchTime < 68000f && !mod.config.DebugAllowClamClickDuringSong)
                {
                    return false;
                }

                var clamPitch = mod.config.ClamPitches[which];
                ICue clamTone = Game1.soundBank.GetCue("clam_tone");
                clamTone.SetVariable("Pitch", clamPitch);
                clamTone.Play();

                var clamPosition = mod.config.ClamPositions[which] * 64;
                var clamColor = mod.config.ClamColors[which];

                __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                {
                    texture = mod.clamTexture,
                    color = clamColor,
                    sourceRect = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16),
                    scale = 4f,
                    position = clamPosition,
                    interval = 1000f,
                    animationLength = 1,
                    alphaFade = 0.03f,
                    layerDepth = 0.0001f
                });

                ___lastFiveClamTones.Add(which);

                while (___lastFiveClamTones.Count > 5)
                {
                    ___lastFiveClamTones.RemoveAt(0);
                }

                if (who != null && !who.mailReceived.Contains("gotPearl") && ___lastFiveClamTones.Count == 5 &&
                    ___lastFiveClamTones[0] == 0 && ___lastFiveClamTones[1] == 4 && ___lastFiveClamTones[2] == 3 && ___lastFiveClamTones[3] == 1 && ___lastFiveClamTones[4] == 2)
                {
                    who.freezePause = 4500;
                    ___fairyTimer = 3500f;
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        interval = 1f,
                        delayBeforeAnimationStart = 885,
                        texture = ___mermaidSprites,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(__instance.playClamTone),
                        extraInfoForEndBehavior = 0
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        interval = 1f,
                        delayBeforeAnimationStart = 1270,
                        texture = ___mermaidSprites,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(__instance.playClamTone),
                        extraInfoForEndBehavior = 4
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        interval = 1f,
                        delayBeforeAnimationStart = 1655,
                        texture = ___mermaidSprites,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(__instance.playClamTone),
                        extraInfoForEndBehavior = 3
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        interval = 1f,
                        delayBeforeAnimationStart = 2040,
                        texture = ___mermaidSprites,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(__instance.playClamTone),
                        extraInfoForEndBehavior = 1
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        interval = 1f,
                        delayBeforeAnimationStart = 2425,
                        texture = ___mermaidSprites,
                        endFunction = new TemporaryAnimatedSprite.endBehavior(__instance.playClamTone),
                        extraInfoForEndBehavior = 2
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        texture = ___mermaidSprites,
                        delayBeforeAnimationStart = 885,
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, 127, 19, 18),
                        sourceRectStartingPos = new Vector2(2f, 127f),
                        scale = 4f,
                        position = new Vector2(28f, 49f) * 4f,
                        interval = 96f,
                        animationLength = 4,
                        totalNumberOfLoops = 121
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        texture = ___mermaidSprites,
                        delayBeforeAnimationStart = 1270,
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, 127, 19, 18),
                        sourceRectStartingPos = new Vector2(2f, 127f),
                        scale = 4f,
                        position = new Vector2(108f, 49f) * 4f,
                        interval = 96f,
                        animationLength = 4,
                        totalNumberOfLoops = 117
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        texture = ___mermaidSprites,
                        delayBeforeAnimationStart = 1655,
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, 127, 19, 18),
                        sourceRectStartingPos = new Vector2(2f, 127f),
                        scale = 4f,
                        position = new Vector2(88f, 39f) * 4f,
                        interval = 96f,
                        animationLength = 4,
                        totalNumberOfLoops = 113
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        texture = ___mermaidSprites,
                        delayBeforeAnimationStart = 2040,
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, 127, 19, 18),
                        sourceRectStartingPos = new Vector2(2f, 127f),
                        scale = 4f,
                        position = new Vector2(48f, 39f) * 4f,
                        interval = 96f,
                        animationLength = 4,
                        totalNumberOfLoops = 19
                    });
                    __instance.temporarySprites.Add(new TemporaryAnimatedSprite
                    {
                        texture = ___mermaidSprites,
                        delayBeforeAnimationStart = 2425,
                        sourceRect = new Microsoft.Xna.Framework.Rectangle(2, 127, 19, 18),
                        sourceRectStartingPos = new Vector2(2f, 127f),
                        scale = 4f,
                        position = new Vector2(68f, 29f) * 4f,
                        interval = 96f,
                        animationLength = 4,
                        totalNumberOfLoops = 15
                    });
                    ___pearlRecipient = who;
                }

                return false;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }

        public static bool DrawAboveAlwaysFrontLayer(MermaidHouse __instance, SpriteBatch b, float ___blackBGAlpha, Stopwatch ___stopWatch, Texture2D ___mermaidSprites, float ___finalLeftMermaidAlpha, float ___finalRightMermaidAlpha, List<Vector2> ___bubbles, List<TemporaryAnimatedSprite> ___alwaysFrontTempSprites, float ___bigMermaidAlpha, float ___finalBigMermaidAlpha)
        {
            try
            {
                if (__instance.critters != null && Game1.farmEvent == null)
                {
                    for (int i = 0; i < __instance.critters.Count; i++)
                    {
                        __instance.critters[i].drawAboveFrontLayer(b);
                    }
                }

                foreach (NPC npc in __instance.characters)
                {
                    npc.drawAboveAlwaysFrontLayer(b);
                }

                foreach (TemporaryAnimatedSprite s in __instance.TemporarySprites)
                {
                    if (s.drawAboveAlwaysFront)
                    {
                        s.draw(b, false, 0, 0, 1f);
                    }
                }

                foreach (Projectile projectile in __instance.projectiles)
                {
                    projectile.draw(b);
                }

                //base.drawAboveAlwaysFrontLayer(b);

                b.Draw(Game1.staminaRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * ___blackBGAlpha);
                int spacing = Game1.graphics.GraphicsDevice.Viewport.Bounds.Height / 4;
                for (int i = -448; i < Game1.graphics.GraphicsDevice.Viewport.Width + 448; i += 448)
                {
                    b.Draw(___mermaidSprites, new Vector2((float)(i - (int)((float)___stopWatch.ElapsedMilliseconds / 6f % 448f)), (float)(spacing - spacing * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), mod.config.BackgroundSwirlColors[0] * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                    b.Draw(___mermaidSprites, new Vector2((float)(i + 112) - (float)___stopWatch.ElapsedMilliseconds / 6f % 448f, (float)spacing - (float)spacing / 4f + (float)Math.Sin((double)((float)___stopWatch.ElapsedMilliseconds / 1000f)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(177, 0, 16, 16)), Color.White * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                    b.Draw(___mermaidSprites, new Vector2((float)(i + (int)((float)___stopWatch.ElapsedMilliseconds / 6f % 448f)), (float)(spacing * 2 - spacing * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), mod.config.BackgroundSwirlColors[1] * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                    b.Draw(___mermaidSprites, new Vector2((float)(i + 112) + (float)___stopWatch.ElapsedMilliseconds / 6f % 448f, (float)(spacing * 2) - (float)spacing / 4f + (float)Math.Sin((double)((float)___stopWatch.ElapsedMilliseconds / 1000f + 4f)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(161, 0, 16, 16)), Color.White * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.001f);
                    b.Draw(___mermaidSprites, new Vector2((float)(i - (int)((float)___stopWatch.ElapsedMilliseconds / 6f % 448f)), (float)(spacing * 3 - spacing * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), mod.config.BackgroundSwirlColors[2] * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                    b.Draw(___mermaidSprites, new Vector2((float)(i + 112) - (float)___stopWatch.ElapsedMilliseconds / 6f % 448f, (float)(spacing * 3) - (float)spacing / 4f + (float)Math.Sin((double)((float)___stopWatch.ElapsedMilliseconds / 1000f + 3f)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(129, 0, 16, 16)), Color.White * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                    b.Draw(___mermaidSprites, new Vector2((float)(i + (int)((float)___stopWatch.ElapsedMilliseconds / 6f % 448f)), (float)(spacing * 4 - spacing * 3 / 4)), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(144, 32, 112, 48)), mod.config.BackgroundSwirlColors[3] * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                    b.Draw(___mermaidSprites, new Vector2((float)(i + 112) + (float)___stopWatch.ElapsedMilliseconds / 6f % 448f, (float)(spacing * 4) - (float)spacing / 4f + (float)Math.Sin((double)((float)___stopWatch.ElapsedMilliseconds / 1000f + 2f)) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(145, 0, 16, 16)), Color.White * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.001f);
                }
                b.Draw(___mermaidSprites, new Vector2((float)(Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.X - 112) + (float)Math.Sin((double)((float)___stopWatch.ElapsedMilliseconds / 1000f)) * 64f * 2f, (float)(Game1.graphics.GraphicsDevice.Viewport.Bounds.Center.Y - 140) + (float)Math.Cos((double)((float)___stopWatch.ElapsedMilliseconds / 1000f * 2f) + 1.5707963267948966) * 64f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int)(57L * (___stopWatch.ElapsedMilliseconds % 1538L / 769L)), 0, 57, 70)), Color.White * ___bigMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);

                foreach (TemporaryAnimatedSprite temporaryAnimatedSprite in ___alwaysFrontTempSprites)
                {
                    temporaryAnimatedSprite.draw(b, true, 0, 0, 1f);
                }

                foreach (Vector2 v in ___bubbles)
                {
                    b.Draw(___mermaidSprites, v + new Vector2((float)Math.Sin((double)((float)___stopWatch.ElapsedMilliseconds / 1000f * 4f + v.X)) * 4f * 6f, 0f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(132, 20, 8, 8)), Color.White * ___blackBGAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                }

                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(-20f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * ___finalLeftMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(-20f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Orange * ___finalLeftMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0011f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(-30f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * ___finalLeftMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(-30f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Cyan * ___finalLeftMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0011f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(-40f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * ___finalLeftMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(-40f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Lime * ___finalLeftMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0011f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(150f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * ___finalRightMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.001f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(150f, 50f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Orange * ___finalRightMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.0011f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(160f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * ___finalRightMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.001f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(160f, 90f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Cyan * ___finalRightMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.0011f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(170f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(192, 0, 16, 32)), Color.White * ___finalRightMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.001f);
                b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(170f, 130f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(208, 0, 16, 32)), Color.Lime * ___finalRightMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.FlipHorizontally, 0.0011f);

                if (!mod.config.HideLastBigMermaid)
                {
                    b.Draw(___mermaidSprites, Game1.GlobalToLocal(new Vector2(43f, 180f) * 4f), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle((int)(57L * (___stopWatch.ElapsedMilliseconds % 1538L / 769L)), 0, 57, 70)), Color.White * ___finalBigMermaidAlpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.001f);
                }

                return false;
            }
            catch (Exception e)
            {
                mod.ErrorLog("There was an exception in a patch", e);
                return true;
            }
        }
    }
}