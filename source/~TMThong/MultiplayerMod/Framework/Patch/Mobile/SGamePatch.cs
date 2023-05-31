/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley.BellsAndWhistles;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Tools;
using StardewValley;
using xTile.Dimensions;
using xTile.Layers;
using xTile.Tiles;
using System.Reflection;

namespace MultiplayerMod.Framework.Patch.Mobile
{
    /// <summary>
    /// Debug class
    /// </summary>
    internal class SGamePatch : Game1, IPatch
    {
        public Type PATCH_TYPE { get; }
        public SGamePatch()
        {
            PATCH_TYPE = typeof(IModHelper).Assembly.GetType("StardewModdingAPI.Framework.SGame");
        }

        public void Apply(Harmony harmony)
        {
           // harmony.Patch(AccessTools.Method(PATCH_TYPE, "DrawImpl"), prefix: new HarmonyMethod(GetType(), nameof(prefix_DrawImpl)));
        }

        private static PropertyInfo IsInDrawProperty;
        private static MethodInfo DrawImplMethod;
        private static IReflectedField<StringBuilder> DebugStringBuilderField;

        // Token: 0x040001E5 RID: 485
        private static IReflectedField<Task> NewDayTaskField;

        // Token: 0x040001E6 RID: 486
        private static IReflectedMethod DrawTapToMoveTargetMethod;

        // Token: 0x040001E7 RID: 487
        private static IReflectedMethod DrawGreenPlacementBoundsMethod;

        private static IReflectedMethod drawOverlaysMethod;

        private static IReflectedField<List<Farmer>> _farmerShadowsField;

        private static IReflectedMethod drawHUDMethod;

        private static IReflectedMethod drawDialogueBoxMethod;

        private static bool prefix_DrawImpl(GameTime gameTime, RenderTarget2D target_screen, Game1 __instance)
        {
            if (IsInDrawProperty == null)
            {
                IsInDrawProperty = typeof(Context).GetProperty("IsInDrawLoop");
                DrawImplMethod = __instance.GetType().GetMethod("DrawImpl", BindingFlags.Instance | BindingFlags.NonPublic);
                DebugStringBuilderField = ModUtilities.Helper.Reflection.GetField<StringBuilder>(typeof(Game1), "_debugStringBuilder", true);
                NewDayTaskField = ModUtilities.Helper.Reflection.GetField<Task>(typeof(Game1), "_newDayTask", true);
                DrawTapToMoveTargetMethod = ModUtilities.Helper.Reflection.GetMethod(__instance, "DrawTapToMoveTarget", true);
                DrawGreenPlacementBoundsMethod = ModUtilities.Helper.Reflection.GetMethod(__instance, "DrawGreenPlacementBounds", true);
                drawOverlaysMethod = ModUtilities.Helper.Reflection.GetMethod(__instance, "drawOverlays", true);
                _farmerShadowsField = ModUtilities.Helper.Reflection.GetField<List<Farmer>>(__instance, "_farmerShadows", true);
                drawHUDMethod = ModUtilities.Helper.Reflection.GetMethod(__instance, "drawHUD", true);
                drawDialogueBoxMethod = ModUtilities.Helper.Reflection.GetMethod(__instance, "drawDialogueBox", true);
            }
            if (!(Game1.gameMode == 3 && Game1.client != null && Game1.currentLocation != null))
            {
                return true;
            }
            try
            {
                __instance.GraphicsDevice.Clear(Game1.bgColor);
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);

                if (Game1.background != null)
                {
                    Game1.background.draw(Game1.spriteBatch);
                }
                Game1.currentLocation.drawBackground(Game1.spriteBatch);
                Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
                Game1.currentLocation.Map.GetLayer("Back").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
                Game1.currentLocation.drawWater(Game1.spriteBatch);
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
                Game1.currentLocation.drawFloorDecorations(Game1.spriteBatch);
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                _farmerShadowsField.GetValue().Clear();
                if (Game1.currentLocation.currentEvent != null && !Game1.currentLocation.currentEvent.isFestival && Game1.currentLocation.currentEvent.farmerActors.Count > 0)
                {
                    foreach (Farmer f in Game1.currentLocation.currentEvent.farmerActors)
                    {
                        if ((f.IsLocalPlayer && Game1.displayFarmer) || !f.hidden)
                        {
                            _farmerShadowsField.GetValue().Add(f);
                        }
                    }
                }
                else
                {
                    foreach (Farmer f2 in Game1.currentLocation.farmers)
                    {
                        if ((f2.IsLocalPlayer && Game1.displayFarmer) || !f2.hidden)
                        {
                            _farmerShadowsField.GetValue().Add(f2);
                        }
                    }
                }
                if (!Game1.currentLocation.shouldHideCharacters())
                {
                    if (Game1.CurrentEvent == null)
                    {
                        foreach (NPC k in Game1.currentLocation.characters)
                        {
                            if (!k.swimming && !k.HideShadow && !k.IsInvisible && !__instance.checkCharacterTilesForShadowDrawFlag(k))
                            {
                                Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, k.GetShadowOffset() + k.Position + new Vector2((float)(k.GetSpriteWidthForPositioning() * 4) / 2f, k.GetBoundingBox().Height + ((!k.IsMonster) ? 12 : 0))), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), Math.Max(0f, (4f + (float)k.yJumpOffset / 40f) * (float)k.scale), SpriteEffects.None, Math.Max(0f, (float)k.getStandingY() / 10000f) - 1E-06f);
                            }
                        }
                    }
                    else
                    {
                        foreach (NPC l in Game1.CurrentEvent.actors)
                        {
                            if ((Game1.CurrentEvent == null || !Game1.CurrentEvent.ShouldHideCharacter(l)) && !l.swimming && !l.HideShadow && !__instance.checkCharacterTilesForShadowDrawFlag(l))
                            {
                                Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, l.GetShadowOffset() + l.Position + new Vector2((float)(l.GetSpriteWidthForPositioning() * 4) / 2f, l.GetBoundingBox().Height + ((!l.IsMonster) ? ((l.Sprite.SpriteHeight <= 16) ? (-4) : 12) : 0))), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), Math.Max(0f, 4f + (float)l.yJumpOffset / 40f) * (float)l.scale, SpriteEffects.None, Math.Max(0f, (float)l.getStandingY() / 10000f) - 1E-06f);
                            }
                        }
                    }
                    foreach (Farmer f3 in _farmerShadowsField.GetValue())
                    {
                        if (!Game1.multiplayer.isDisconnecting(f3.UniqueMultiplayerID) && !f3.swimming && !f3.isRidingHorse() && !f3.IsSitting() && (Game1.currentLocation == null || !__instance.checkCharacterTilesForShadowDrawFlag(f3)))
                        {
                            Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(f3.GetShadowOffset() + f3.Position + new Vector2(32f, 24f)), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f - (((f3.running || f3.UsingTool) && f3.FarmerSprite.currentAnimationIndex > 1) ? ((float)Math.Abs(FarmerRenderer.featureYOffsetPerFrame[f3.FarmerSprite.CurrentFrame]) * 0.5f) : 0f), SpriteEffects.None, 0f);
                        }
                    }
                }
                Layer building_layer = Game1.currentLocation.Map.GetLayer("Buildings");
                building_layer.Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
                Game1.mapDisplayDevice.EndScene();
                if ((Game1.currentLocation != null))
                {
                    object tapToMove = Game1.currentLocation.GetType().GetProperty("tapToMove").GetValue(Game1.currentLocation);
                    if (tapToMove == null)
                    {
                        object TapToMove = typeof(IClickableMenu).Assembly.GetType("StardewValley.Mobile.TapToMove").CreateInstance<object>(new object[] { Game1.currentLocation });
                        tapToMove = TapToMove;
                        Game1.currentLocation.GetType().GetProperty("tapToMove").SetValue(Game1.currentLocation, TapToMove);
                    }
                    NPC targetNPC = ModUtilities.Helper.Reflection.GetField<NPC>(tapToMove, "_targetNPC").GetValue();
                    if (targetNPC != null)
                    {
                        Game1.spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, targetNPC.Position + new Vector2((float)(targetNPC.Sprite.SpriteWidth * 4) / 2f - 32f, (float)(targetNPC.GetBoundingBox().Height + (targetNPC.IsMonster ? 0 : 12) - 32))), new Microsoft.Xna.Framework.Rectangle?(new Microsoft.Xna.Framework.Rectangle(194, 388, 16, 16)), Color.White, 0f, Vector2.Zero, 4f, 0, 0.58f);
                    }
                }
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
                if (!Game1.currentLocation.shouldHideCharacters())
                {
                    if (Game1.CurrentEvent == null)
                    {
                        foreach (NPC m in Game1.currentLocation.characters)
                        {
                            if (!m.swimming && !m.HideShadow && !m.isInvisible && __instance.checkCharacterTilesForShadowDrawFlag(m))
                            {
                                Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, m.GetShadowOffset() + m.Position + new Vector2((float)(m.GetSpriteWidthForPositioning() * 4) / 2f, m.GetBoundingBox().Height + ((!m.IsMonster) ? 12 : 0))), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), Math.Max(0f, (4f + (float)m.yJumpOffset / 40f) * (float)m.scale), SpriteEffects.None, Math.Max(0f, (float)m.getStandingY() / 10000f) - 1E-06f);
                            }
                        }
                    }
                    else
                    {
                        foreach (NPC n in Game1.CurrentEvent.actors)
                        {
                            if ((Game1.CurrentEvent == null || !Game1.CurrentEvent.ShouldHideCharacter(n)) && !n.swimming && !n.HideShadow && __instance.checkCharacterTilesForShadowDrawFlag(n))
                            {
                                Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, n.GetShadowOffset() + n.Position + new Vector2((float)(n.GetSpriteWidthForPositioning() * 4) / 2f, n.GetBoundingBox().Height + ((!n.IsMonster) ? 12 : 0))), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), Math.Max(0f, (4f + (float)n.yJumpOffset / 40f) * (float)n.scale), SpriteEffects.None, Math.Max(0f, (float)n.getStandingY() / 10000f) - 1E-06f);
                            }
                        }
                    }
                    foreach (Farmer f4 in _farmerShadowsField.GetValue())
                    {
                        float draw_layer = Math.Max(0.0001f, f4.getDrawLayer() + 0.00011f) - 0.0001f;
                        if (!f4.swimming && !f4.isRidingHorse() && !f4.IsSitting() && Game1.currentLocation != null && __instance.checkCharacterTilesForShadowDrawFlag(f4))
                        {
                            Game1.spriteBatch.Draw(Game1.shadowTexture, Game1.GlobalToLocal(f4.GetShadowOffset() + f4.Position + new Vector2(32f, 24f)), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f - (((f4.running || f4.UsingTool) && f4.FarmerSprite.currentAnimationIndex > 1) ? ((float)Math.Abs(FarmerRenderer.featureYOffsetPerFrame[f4.FarmerSprite.CurrentFrame]) * 0.5f) : 0f), SpriteEffects.None, draw_layer);
                        }
                    }
                }
                if ((Game1.eventUp || Game1.killScreen) && !Game1.killScreen && Game1.currentLocation.currentEvent != null)
                {
                    Game1.currentLocation.currentEvent.draw(Game1.spriteBatch);
                }
                if (Game1.player.currentUpgrade != null && Game1.player.currentUpgrade.daysLeftTillUpgradeDone <= 3 && Game1.currentLocation.Name.Equals("Farm"))
                {
                    Game1.spriteBatch.Draw(Game1.player.currentUpgrade.workerTexture, Game1.GlobalToLocal(Game1.viewport, Game1.player.currentUpgrade.positionOfCarpenter), Game1.player.currentUpgrade.getSourceRectangle(), Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, (Game1.player.currentUpgrade.positionOfCarpenter.Y + 48f) / 10000f);
                }
                Game1.currentLocation.draw(Game1.spriteBatch);
                foreach (Vector2 tile_position in Game1.crabPotOverlayTiles.Keys)
                {
                    Tile tile = building_layer.Tiles[(int)tile_position.X, (int)tile_position.Y];
                    if (tile != null)
                    {
                        Vector2 vector_draw_position = Game1.GlobalToLocal(Game1.viewport, tile_position * 64f);
                        Location draw_location = new((int)vector_draw_position.X, (int)vector_draw_position.Y);
                        Game1.mapDisplayDevice.DrawTile(tile, draw_location, (tile_position.Y * 64f - 1f) / 10000f);
                    }
                }
                if (Game1.eventUp && Game1.currentLocation.currentEvent != null)
                {
                    _ = Game1.currentLocation.currentEvent.messageToScreen;
                }
                if (Game1.player.ActiveObject == null && (Game1.player.UsingTool || Game1.pickingTool) && Game1.player.CurrentTool != null && (!Game1.player.CurrentTool.Name.Equals("Seeds") || Game1.pickingTool))
                {
                    Game1.drawTool(Game1.player);
                }
                if (Game1.currentLocation.Name.Equals("Farm"))
                {
                    //__instance.drawFarmBuildings();
                }
                if (Game1.tvStation >= 0)
                {
                    Game1.spriteBatch.Draw(Game1.tvStationTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2(400f, 160f)), new Microsoft.Xna.Framework.Rectangle(Game1.tvStation * 24, 0, 24, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1E-08f);
                }
                if (Game1.panMode)
                {
                    Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle((int)Math.Floor((double)(Game1.getOldMouseX() + Game1.viewport.X) / 64.0) * 64 - Game1.viewport.X, (int)Math.Floor((double)(Game1.getOldMouseY() + Game1.viewport.Y) / 64.0) * 64 - Game1.viewport.Y, 64, 64), Color.Lime * 0.75f);
                    foreach (Warp w in Game1.currentLocation.warps)
                    {
                        Game1.spriteBatch.Draw(Game1.fadeToBlackRect, new Microsoft.Xna.Framework.Rectangle(w.X * 64 - Game1.viewport.X, w.Y * 64 - Game1.viewport.Y, 64, 64), Color.Red * 0.75f);
                    }
                }
                Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
                Game1.currentLocation.Map.GetLayer("Front").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
                Game1.mapDisplayDevice.EndScene();
                Game1.currentLocation.drawAboveFrontLayer(Game1.spriteBatch);
                object tapToMove_ = Game1.currentLocation.GetType().GetProperty("tapToMove").GetValue(Game1.currentLocation);
                NPC targetNPC_ = ModUtilities.Helper.Reflection.GetField<NPC>(tapToMove_, "_targetNPC").GetValue();
                if ((((targetNPC_ == null) && (Game1.displayHUD || Game1.eventUp)) && (((Game1.currentBillboard == 0) && (Game1.gameMode == 3)) && (!Game1.freezeControls && !Game1.panMode))) && !Game1.HostPaused)
                {
                    DrawTapToMoveTargetMethod.Invoke();
                }
                Game1.spriteBatch.End();
                Game1.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
                if (Game1.currentLocation.Map.GetLayer("AlwaysFront") != null)
                {
                    Game1.mapDisplayDevice.BeginScene(Game1.spriteBatch);
                    Game1.currentLocation.Map.GetLayer("AlwaysFront").Draw(Game1.mapDisplayDevice, Game1.viewport, Location.Origin, wrapAround: false, 4);
                    Game1.mapDisplayDevice.EndScene();
                }
                Game1.spriteBatch.End();
            }
            catch (Exception ex)
            {
                ModUtilities.ModMonitor.Log(ex.GetBaseException().ToString(), LogLevel.Error);
            }
            return false;
        }
    }
}
