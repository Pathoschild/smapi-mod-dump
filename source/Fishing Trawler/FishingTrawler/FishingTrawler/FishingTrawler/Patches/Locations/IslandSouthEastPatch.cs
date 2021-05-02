/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/FishingTrawler
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using FishingTrawler.Objects;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xTile.Tiles;
using System.Reflection;
using FishingTrawler.Objects.Rewards;

namespace FishingTrawler.Patches.Locations
{
    public class IslandSouthEastPatch : Patch
    {
        private readonly Type _islandSouthEast = typeof(IslandSouthEast);

        internal IslandSouthEastPatch(IMonitor monitor) : base(monitor)
        {

        }

        internal override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(AccessTools.Method(_islandSouthEast, nameof(IslandSouthEast.checkAction), new[] { typeof(xTile.Dimensions.Location), typeof(xTile.Dimensions.Rectangle), typeof(Farmer) }), postfix: new HarmonyMethod(GetType(), nameof(CheckActionPatch)));
            harmony.Patch(AccessTools.Method(_islandSouthEast, nameof(IslandSouthEast.cleanupBeforePlayerExit), null), postfix: new HarmonyMethod(GetType(), nameof(CleanupBeforePlayerExitPatch)));
            harmony.Patch(AccessTools.Method(_islandSouthEast, nameof(IslandSouthEast.draw), new[] { typeof(SpriteBatch) }), postfix: new HarmonyMethod(GetType(), nameof(DrawPatch)));
            harmony.Patch(AccessTools.Method(_islandSouthEast, nameof(IslandSouthEast.UpdateWhenCurrentLocation), new[] { typeof(GameTime) }), postfix: new HarmonyMethod(GetType(), nameof(UpdateWhenCurrentLocationPatch)));
        }

        internal static void CheckActionPatch(IslandSouthEast __instance, ref bool __result, xTile.Dimensions.Location tileLocation, xTile.Dimensions.Rectangle viewport, Farmer who)
        {
            if (__result)
            {
                return;
            }

            if (ModEntry.murphyNPC != null && ModEntry.murphyNPC.getTileX() == tileLocation.X && Enumerable.Range(ModEntry.murphyNPC.getTileY() - 1, 3).Contains(tileLocation.Y))
            {
                __result = ModEntry.murphyNPC.checkAction(who, __instance);
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

                    if (ModEntry.rewardChest.items.Count() == 0)
                    {
                        Game1.drawObjectDialogue(ModEntry.i18n.Get("game_message.empty_crate"));
                        break;
                    }

                    __instance.playSound("fishSlap");
                    ModEntry.rewardChest.ShowMenu();
                    break;
                case "TrawlerNote":
                    if (!who.mailReceived.Contains("PeacefulEnd.FishingTrawler_WillyIntroducesMurphy"))
                    {
                        Game1.drawObjectDialogue(ModEntry.i18n.Get("game_message.messy_note"));
                        break;
                    }

                    if (LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.en)
                    {
                        Game1.drawObjectDialogue(String.Format("There's a note here...#It is from Murphy! It says he will be docked every {0} and to speak with him before nightfall if you wish to go on a fishing trip.", Game1.MasterPlayer.modData[ModEntry.MURPHY_DAY_TO_APPEAR_ISLAND]));
                        break;
                    }

                    Game1.drawObjectDialogue(String.Format(ModEntry.i18n.Get("game_message.readable_note"), Game1.MasterPlayer.modData[ModEntry.MURPHY_DAY_TO_APPEAR]));
                    break;
                default:
                    break;
            }
        }

        internal static void CleanupBeforePlayerExitPatch(IslandSouthEast __instance)
        {
            ModEntry.trawlerObject.Reset();
            ModEntry.murphyNPC = null;
        }


        internal static void DrawPatch(IslandSouthEast __instance, SpriteBatch b)
        {
            if (!ModEntry.ShouldMurphyAppear(__instance) && __instance.currentEvent is null)
            {
                // Skip this draw patch if Murphy isn't here today
                return;
            }

            Texture2D boatTexture = ModResources.boatTexture;
            if (boatTexture != null)
            {
                b.Draw(boatTexture, Game1.GlobalToLocal(ModEntry.trawlerObject.GetTrawlerPosition()), new Rectangle(0, 16, 224, 160), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
                if (ModEntry.trawlerObject._closeGate)
                {
                    b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(107f, 16f) * 4f + ModEntry.trawlerObject.GetTrawlerPosition()), new Rectangle(251, 32, 18, 15), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07f);
                }
                else
                {
                    b.Draw(boatTexture, Game1.GlobalToLocal(new Vector2(106f, 7f) * 4f + ModEntry.trawlerObject.GetTrawlerPosition()), new Rectangle(282, 23, 4, 24), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.07f);
                }
            }

            // Draw the Murphy NPC
            if (ModEntry.murphyNPC != null)
            {
                ModEntry.murphyNPC.draw(b);
            }
        }

        internal static void UpdateWhenCurrentLocationPatch(IslandSouthEast __instance, GameTime time)
        {
            // Update the Murphy NPC
            if (ModEntry.ShouldMurphyAppear(__instance) && ModEntry.murphyNPC == null)
            {
                ModEntry.SpawnMurphy(__instance);
            }

            if (ModEntry.murphyNPC != null)
            {
                ModEntry.murphyNPC.update(time, __instance);

                if (__instance.modData.ContainsKey(ModEntry.MURPHY_ON_TRIP) && __instance.modData[ModEntry.MURPHY_ON_TRIP] == "true")
                {
                    ModEntry.murphyNPC = null;
                }
            }

            // Update the appearance of the reward chest
            if (ModEntry.rewardChest.items.Count() == 0 && __instance.getTileIndexAt(new Point(5, 39), "Buildings") != 10)
            {
                SwapRewardChestTiles(__instance, 10);
            }
            else if (ModEntry.rewardChest.items.Count() > 0 && __instance.getTileIndexAt(new Point(5, 39), "Buildings") != 0)
            {
                SwapRewardChestTiles(__instance, 0);
            }

            Trawler trawler = ModEntry.trawlerObject;
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

            Microsoft.Xna.Framework.Rectangle back_rectangle = new Microsoft.Xna.Framework.Rectangle(24, 188, 16, 220);
            back_rectangle.X += (int)trawler.GetTrawlerPosition().X;
            back_rectangle.Y += (int)trawler.GetTrawlerPosition().Y;
            if ((float)trawler._boatDirection != 0f)
            {
                if (trawler._nextBubble > 0f)
                {
                    trawler._nextBubble -= (float)time.ElapsedGameTime.TotalSeconds;
                }
                else
                {
                    Vector2 position2 = Utility.getRandomPositionInThisRectangle(back_rectangle, Game1.random);
                    TemporaryAnimatedSprite sprite2 = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 0, 64, 64), 50f, 9, 1, position2, flicker: false, flipped: false, 0f, 0.025f, Color.White, 1f, 0f, 0f, 0f);
                    sprite2.acceleration = new Vector2(-0.25f * (float)Math.Sign(trawler._boatDirection), 0f);
                    if (Context.IsSplitScreen)
                    {
                        ModEntry.multiplayer.broadcastSprites(__instance, sprite2);
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
            if (trawler._boatAnimating)
            {
                if (trawler._nextSmoke > 0f)
                {
                    trawler._nextSmoke -= (float)time.ElapsedGameTime.TotalSeconds;
                    return;
                }
                Vector2 position = new Vector2(158f, -32f) * 4f + trawler.GetTrawlerPosition();
                TemporaryAnimatedSprite sprite = new TemporaryAnimatedSprite("TileSheets\\animations", new Microsoft.Xna.Framework.Rectangle(0, 1600, 64, 128), 200f, 9, 1, position, flicker: false, flipped: false, 1f, 0.025f, Color.Gray, 1f, 0.025f, 0f, 0f);
                sprite.acceleration = new Vector2(-0.25f, -0.15f);
                __instance.temporarySprites.Add(sprite);
                trawler._nextSmoke = 0.2f;
            }
        }

        private static void SwapRewardChestTiles(GameLocation location, int startingOffset)
        {
            for (int x = 0; x < 3; x++)
            {
                location.setMapTileIndex(5 + x, 38, startingOffset + x, "Buildings");
                location.setMapTileIndex(5 + x, 39, startingOffset + x + 5, "Buildings"); // Offsetting by 5 for second row from tilesheet
            }
        }
    }
}
