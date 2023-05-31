/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using FishingTrawler.Framework.Utilities;
using FishingTrawler.Objects;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Linq;
using xTile.Tiles;

namespace FishingTrawler.Patches.Locations
{
    internal class BeachPatch : PatchTemplate
    {
        private readonly Type _beach = typeof(Beach);

        internal BeachPatch(IMonitor modMonitor, IModHelper modHelper) : base(modMonitor, modHelper)
        {

        }

        internal override void Apply(Harmony harmony)
        {
            harmony.Patch(AccessTools.Method(_beach, nameof(Beach.checkAction), new[] { typeof(xTile.Dimensions.Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(CheckActionPatch)));
            harmony.Patch(AccessTools.Method(_beach, nameof(Beach.cleanupBeforePlayerExit), null), postfix: new HarmonyMethod(GetType(), nameof(CleanupBeforePlayerExitPatch)));
            harmony.Patch(AccessTools.Method(_beach, nameof(Beach.draw), new[] { typeof(SpriteBatch) }), postfix: new HarmonyMethod(GetType(), nameof(DrawPatch)));
            harmony.Patch(AccessTools.Method(_beach, nameof(Beach.UpdateWhenCurrentLocation), new[] { typeof(GameTime) }), postfix: new HarmonyMethod(GetType(), nameof(UpdateWhenCurrentLocationPatch)));
        }

        internal static void CheckActionPatch(Beach __instance, ref bool __result, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            if (__result)
            {
                return;
            }

            if (FishingTrawler.murphyNPC != null && FishingTrawler.murphyNPC.getTileX() == tileLocation.X && Enumerable.Range(FishingTrawler.murphyNPC.getTileY() - 1, 3).Contains(tileLocation.Y))
            {
                __result = FishingTrawler.murphyNPC.checkAction(who, __instance);
                return;
            }

            // Check to see if player is trying to access Trawler's reward chest
            Tile tile = __instance.map.GetLayer("Buildings").PickTile(new xTile.Dimensions.Location(tileLocation.X * 64, tileLocation.Y * 64), viewport.Size);
            if (tile is null || !tile.Properties.ContainsKey("CustomAction"))
            {
                return;
            }

            switch (tile.Properties["CustomAction"].ToString())
            {
                case "TrawlerRewardStorage":
                    __result = true;

                    if (FishingTrawler.rewardChest.items.Count() == 0)
                    {
                        Game1.drawObjectDialogue(FishingTrawler.i18n.Get("game_message.empty_crate"));
                        break;
                    }

                    __instance.playSound("fishSlap");
                    FishingTrawler.rewardChest.ShowMenu();
                    break;
                case "TrawlerNote":
                    if (!who.mailReceived.Contains(ModDataKeys.MAIL_FLAG_MURPHY_WAS_INTRODUCED))
                    {
                        Game1.drawObjectDialogue(FishingTrawler.i18n.Get("game_message.messy_note"));
                        break;
                    }

                    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
                    {
                        Game1.drawObjectDialogue(string.Format("There's a note here...#It is from Murphy! It says he will be docked every {0} and to speak with him before nightfall if you wish to go on a fishing trip.", Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR]));
                        break;
                    }

                    Game1.drawObjectDialogue(string.Format(FishingTrawler.i18n.Get("game_message.readable_note"), Game1.MasterPlayer.modData[ModDataKeys.MURPHY_DAY_TO_APPEAR]));
                    break;
                default:
                    break;
            }
        }

        internal static void CleanupBeforePlayerExitPatch(Beach __instance)
        {
            FishingTrawler.trawlerObject.Reset();
            FishingTrawler.murphyNPC = null;
        }


        internal static void DrawPatch(Beach __instance, SpriteBatch b)
        {
            if (!FishingTrawler.ShouldMurphyAppear(__instance) && __instance.currentEvent is null)
            {
                // Skip this draw patch if Murphy isn't here today
                return;
            }

            Texture2D boatTexture = FishingTrawler.assetManager.boatTexture;
            if (FishingTrawler.config.useOldTrawlerSprite)
            {
                boatTexture = FishingTrawler.assetManager.oldBoatTexture;
                if (boatTexture != null)
                {
                    b.Draw(boatTexture, Game1.GlobalToLocal(FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(0, 16, 224, 160), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                    if (FishingTrawler.trawlerObject._closeGate)
                    {
                        b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(107f, 16f) * 4f + FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(251, 32, 18, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07f);
                    }
                    else
                    {
                        b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(106f, 7f) * 4f + FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(282, 23, 4, 24), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07f);
                    }
                }
            }
            else if (boatTexture != null)
            {
                b.Draw(boatTexture, Game1.GlobalToLocal(FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(0, 16, 288, 160), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                if (FishingTrawler.trawlerObject._closeGate)
                {
                    b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(113f, 24f) * 4f + FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(321, 40, 14, 8), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1.1f);
                    b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(112f, 96f) * 4f + FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(320, 112, 16, 48), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1.11f);
                }
                else
                {
                    b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(110f, 14f) * 4f + FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(302, 29, 5, 19), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1.1f);
                    b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(112f, 96f) * 4f + FishingTrawler.trawlerObject.GetTrawlerPosition()), new Rectangle(304, 112, 16, 64), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1.11f);
                }
            }

            // Draw the Murphy NPC
            if (FishingTrawler.murphyNPC != null)
            {
                FishingTrawler.murphyNPC.draw(b);
            }
        }

        internal static void UpdateWhenCurrentLocationPatch(Beach __instance, GameTime time)
        {
            // Update the Murphy NPC
            if (FishingTrawler.ShouldMurphyAppear(__instance) && FishingTrawler.murphyNPC == null)
            {
                FishingTrawler.SpawnMurphy(__instance);
            }

            if (FishingTrawler.murphyNPC != null)
            {
                FishingTrawler.murphyNPC.update(time, __instance);

                if (__instance.modData.ContainsKey(ModDataKeys.MURPHY_ON_TRIP) && __instance.modData[ModDataKeys.MURPHY_ON_TRIP] == "true")
                {
                    FishingTrawler.murphyNPC = null;
                }
            }

            // Update the appearance of the reward chest
            if (FishingTrawler.rewardChest.items.Count() == 0 && __instance.getTileIndexAt(new Point(82, 37), "Buildings") != 10)
            {
                SwapRewardChestTiles(__instance, 10);
            }
            else if (FishingTrawler.rewardChest.items.Count() > 0 && __instance.getTileIndexAt(new Point(82, 37), "Buildings") != 0)
            {
                SwapRewardChestTiles(__instance, 0);
            }

            Trawler trawler = FishingTrawler.trawlerObject;
            if (trawler is null)
            {
                return;
            }

            if (trawler._boatDirection != 0)
            {
                trawler._boatOffset += trawler._boatDirection;
                if (__instance.currentEvent != null)
                {
                    foreach (NPC actor in __instance.currentEvent.actors)
                    {
                        actor.shouldShadowBeOffset = true;
                        actor.drawOffset.X = trawler._boatOffset;
                    }
                    foreach (Farmer farmerActor in __instance.currentEvent.farmerActors)
                    {
                        farmerActor.shouldShadowBeOffset = true;
                        farmerActor.drawOffset.X = trawler._boatOffset;
                    }
                }
            }

            Rectangle back_rectangle = new Rectangle(24, 188, 16, 220);
            back_rectangle.X += (int)trawler.GetTrawlerPosition().X;
            back_rectangle.Y += (int)trawler.GetTrawlerPosition().Y;
            if (trawler._boatDirection != 0f)
            {
                if (trawler._nextBubble > 0f)
                {
                    trawler._nextBubble -= (float)time.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    Vector2 position2 = Utility.getRandomPositionInThisRectangle(back_rectangle, Game1.random);
                    TemporaryAnimatedSprite sprite2 = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 0, 64, 64), 50f, 9, 1, position2, flicker: false, flipped: false, 0f, 0.025f, Color.White, 1f, 0f, 0f, 0f);
                    sprite2.acceleration = new Vector2(-0.25f * Math.Sign(trawler._boatDirection), 0f);
                    if (Context.IsSplitScreen)
                    {
                        FishingTrawler.multiplayer.broadcastSprites(__instance, sprite2);
                    }
                    else
                    {
                        __instance.temporarySprites.Add(sprite2);

                    }
                    trawler._nextBubble = 0.01f;
                }
                if (trawler._nextSlosh > 0f)
                {
                    trawler._nextSlosh -= (float)time.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    Game1.playSound("waterSlosh");
                    trawler._nextSlosh = 0.5f;
                }
            }
        }

        private static void SwapRewardChestTiles(GameLocation location, int startingOffset)
        {
            for (int x = 0; x < 3; x++)
            {
                location.setMapTileIndex(82 + x, 37, startingOffset + x, "Buildings");
                location.setMapTileIndex(82 + x, 38, startingOffset + x + 5, "Buildings"); // Offsetting by 5 for second row from tilesheet
            }
        }
    }
}
