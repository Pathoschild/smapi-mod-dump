/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Fishnets.Data
{
    public class Fishnet : Object
    {
        private float yBob;
        [XmlElement("directionOffset")]
        public readonly NetVector2 directionOffset = new();
        [XmlElement("bait")]
        public readonly NetRef<Object> bait = new();

        public static Texture2D Texture => ModEntry.IHelper.GameContent.Load<Texture2D>("Fishnets/Fishnet");

        public static Rectangle SourceRect => new(0, 0, 16, 16);

        private readonly int[] Qualities = new[] { lowQuality, medQuality, highQuality, bestQuality };

        public Fishnet() : base(Vector2.Zero, ModEntry.ObjectInfo.Id, "Fish Net", true, false, false, false) { }

        public Fishnet(Vector2 tileLocation, int stack = 1) : this()
        {
            TileLocation = tileLocation;
            Type = "interactive";
            Stack = stack;
        }

        private void drawDefault(SpriteBatch spriteBatch, int x, int y, float alpha = 1) => spriteBatch.Draw(Texture, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f, y * 64f + yBob)), SourceRect, Color.White * alpha, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((y * 64f) + directionOffset.Y + (x % 4)) / 10000.0f);

        private Rectangle getSourceRectForQuality(int quality)
        {
            return quality switch
            {
                medQuality => new(338, 400, 8, 8),
                highQuality => new(346, 400, 8, 8),
                bestQuality => new(346, 392, 8, 8),
                _ => new(338, 392, 8, 8)
            };
        }

        protected void addOverlayTilesIfNecessary(GameLocation location, int x, int y, List<Vector2> tiles)
        {
            if (location != Game1.currentLocation || location.getTileIndexAt(x, y, "Buildings") < 0 || location.doesTileHaveProperty(x, y + 1, "Back", "Water") != null)
                return;
            tiles.Add(new(x, y));
        }

        protected bool checkLocation(GameLocation location, float x, float y) => location.doesTileHaveProperty((int)x, (int)y, "Water", "Back") == null || location.doesTileHaveProperty((int)x, (int)y, "Passable", "Buildings") != null;

        public bool CanCatchFish() => (Game1.getFarmer(owner) != null && Game1.getFarmer(owner).professions.Contains(11)) || bait.Value is not null;

        public List<Vector2> getOverlayTiles(GameLocation location)
        {
            List<Vector2> tiles = new();
            if (directionOffset.Y < 0f)
                addOverlayTilesIfNecessary(location, (int)TileLocation.X, (int)TileLocation.Y, tiles);
            addOverlayTilesIfNecessary(location, (int)TileLocation.X, (int)TileLocation.Y + 1, tiles);
            if (directionOffset.X < 0f)
                addOverlayTilesIfNecessary(location, (int)TileLocation.X - 1, (int)TileLocation.Y + 1, tiles);
            if (directionOffset.X > 0f)
                addOverlayTilesIfNecessary(location, (int)TileLocation.X + 1, (int)TileLocation.Y + 1, tiles);
            return tiles;
        }

        public void addOverlayTiles(GameLocation location)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in getOverlayTiles(location))
            {
                if (!Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                    Game1.crabPotOverlayTiles[overlayTile] = 0;
                Game1.crabPotOverlayTiles[overlayTile]++;
            }
        }

        public void removeOverlayTiles(GameLocation location)
        {
            if (location != Game1.currentLocation)
                return;
            foreach (Vector2 overlayTile in getOverlayTiles(location))
            {
                if (Game1.crabPotOverlayTiles.ContainsKey(overlayTile))
                {
                    Game1.crabPotOverlayTiles[overlayTile]--;
                    if (Game1.crabPotOverlayTiles[overlayTile] <= 0)
                        Game1.crabPotOverlayTiles.Remove(overlayTile);
                }
            }
        }

        public void updateOffset(GameLocation location)
        {
            Vector2 zero = Vector2.Zero;
            if (checkLocation(location, TileLocation.X - 1f, TileLocation.Y))
                zero += new Vector2(32f, 0f);
            if (checkLocation(location, TileLocation.X + 1f, TileLocation.Y))
                zero += new Vector2(-32f, 0f);
            if (zero.X != 0.0f && checkLocation(location, TileLocation.X + Math.Sign(zero.X), TileLocation.Y + 1f))
                zero += new Vector2(0.0f, -42f);
            if (checkLocation(location, TileLocation.X, TileLocation.Y - 1f))
                zero += new Vector2(0.0f, 32f);
            if (checkLocation(location, TileLocation.X, TileLocation.Y + 1f))
                zero += new Vector2(0.0f, -42f);
            directionOffset.Value = zero;
        }

        public static bool IsValidPlacementLocation(GameLocation location, int x, int y)
        {
            Vector2 tile = new(x, y);
            bool flag = location.doesTileHaveProperty(x + 1, y, "Water", "Back") != null && location.doesTileHaveProperty(x - 1, y, "Water", "Back") != null || location.doesTileHaveProperty(x, y + 1, "Water", "Back") != null && location.doesTileHaveProperty(x, y - 1, "Water", "Back") != null;
            return !location.Objects.ContainsKey(tile) && !location.Objects.ContainsKey(new Vector2(tile.X + .5f, tile.Y + .5f)) && flag && (location.doesTileHaveProperty(x, y, "Water", "Back") != null && location.doesTileHaveProperty(x, y, "Passable", "Buildings") == null);
        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(directionOffset, bait);
        }

        public override Item getOne()
        {
            Object o = new(ModEntry.ObjectInfo.Id, 1);
            o._GetOneFrom(this);
            return o;
        }

        public override bool isPlaceable() => true;

        public override bool canBePlacedInWater() => true;

        public override bool canBePlacedHere(GameLocation l, Vector2 tile) => IsValidPlacementLocation(l, (int)tile.X, (int)tile.Y);

        public override bool canBeTrashed() => true;

        public override bool canBeDropped() => true;

        public override bool canBeGivenAsGift() => false;

        public override bool canBeShipped() => false;

        public override void actionOnPlayerEntry()
        {
            updateOffset(Game1.currentLocation);
            addOverlayTiles(Game1.currentLocation);
            base.actionOnPlayerEntry();
        }

        public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
        {
            if (who != null)
                owner.Value = who.UniqueMultiplayerID;
            if (!IsValidPlacementLocation(location, (int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f)))
                return false;
            TileLocation = new Vector2((int)Math.Floor(x / 64f), (int)Math.Floor(y / 64f));
            location.Objects.Add(TileLocation, this);
            location.playSound("waterSlosh");
            DelayedAction.playSoundAfterDelay("slosh", 150);
            updateOffset(location);
            addOverlayTiles(location);
            return true;
        }

        public override bool performObjectDropInAction(Item dropInItem, bool probe, Farmer who)
        {
            if (dropInItem is not Object o || o.Category != -21 || bait.Value != null || who.professions.Contains(11))
                return false;
            if (!probe)
            {
                bait.Value = o.getOne() as Object;
                who.currentLocation.playSound("Ship");
            }
            return true;
        }

        public override void performRemoveAction(Vector2 tileLocation, GameLocation environment)
        {
            removeOverlayTiles(environment);
            bait.Value = null;
            base.performRemoveAction(tileLocation, environment);
        }

        public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
        {
            if (heldObject.Value is not null)
            {
                if (justCheckingForActivity)
                    return true;
                Object o = heldObject.Value;
                if (who.IsLocalPlayer && !who.addItemToInventoryBool(o))
                {
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                    return false;
                }
                heldObject.Value = null;
                Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
                if (fishData.ContainsKey(o.ParentSheetIndex))
                    who.caughtFish(o.ParentSheetIndex, -1);
                readyForHarvest.Value = false;
                bait.Value = null;
                who.animateOnce(279 + who.FacingDirection);
                who.currentLocation.playSound("fishingRodBend");
                DelayedAction.playSoundAfterDelay("coin", 500);
                who.gainExperience(1, 5);
                return true;
            }
            if (bait.Value is null)
            {
                if (justCheckingForActivity)
                    return true;
                if (Game1.didPlayerJustClickAtAll(true))
                {
                    if (who.addItemToInventoryBool(getOne()))
                    {
                        if (who.isMoving())
                            Game1.haltAfterCheck = false;
                        Game1.playSound("coin");
                        who.currentLocation.Objects.Remove(TileLocation);
                        return true;
                    }
                    else
                        Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
                }
            }

            return false;
        }

        public override void dropItem(GameLocation location, Vector2 origin, Vector2 destination)
        {
            if (fragility == 2)
                return;
            location.debris.Add(new(new Object(ModEntry.ObjectInfo.Id, 1), origin, destination));
        }

        public override void DayUpdate(GameLocation location)
        {
            bool flag1 = Game1.getFarmer(owner) != null && Game1.getFarmer(owner).professions.Contains(11);
            bool flag2 = Game1.getFarmer(owner) != null && Game1.getFarmer(owner).professions.Contains(10);
            if (owner.Value == 0L && Game1.player.professions.Contains(11))
                flag2 = true;
            if ((bait.Value == null && !flag1) || heldObject.Value != null)
                return;
            readyForHarvest.Value = true;
            Random r = new();
            Dictionary<int, string> fishData = Game1.content.Load<Dictionary<int, string>>("Data\\Fish");
            List<int> nums = new();
            double chance = flag2 ? 0.0 : 0.2;
            if (!flag2)
                chance += location.getExtraTrashChanceForCrabPot((int)TileLocation.X, (int)TileLocation.Y);
            if (r.NextDouble() > chance)
            {
                foreach (KeyValuePair<int, string> kvp in fishData)
                {
                    if (kvp.Value.Contains("trap")) continue;
                    if (flag2 && Statics.CanCatchThisFish(kvp.Key, location.Name))
                        nums.Add(kvp.Key);
                    else
                    {
                        if (r.NextDouble() <= .15)
                        {
                            heldObject.Value = Statics.GetRandomFishForLocation(bait.Value?.ParentSheetIndex ?? -1, Game1.player, location.Name);
                            if (ModEntry.HasQualityBait)
                                heldObject.Value.Quality = ModEntry.IQualityBaitApi.GetQuality(heldObject.Value.Quality, bait.Value?.Quality ?? (flag1 ? Qualities[Game1.random.Next(4)] : lowQuality));
                            break;
                        }
                    }
                }
            }
            if (heldObject.Value != null)
                return;
            if (flag2 && nums.Count > 0)
            {
                heldObject.Value = new(nums[r.Next(nums.Count)], bait.Value?.ParentSheetIndex == 774 && r.NextDouble() <= .15 ? 2 : 1);
                if (ModEntry.HasQualityBait)
                    heldObject.Value.Quality = ModEntry.IQualityBaitApi.GetQuality(heldObject.Value.Quality, bait.Value?.Quality ?? (flag1 ? Qualities[Game1.random.Next(4)] : lowQuality));
            }
            else
                heldObject.Value = new(r.Next(168, 173), 1);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
        {
            bool shouldDrawStackNumber = false;
            if (isRecipe)
            {
                transparency = 0.5f;
                scaleSize *= 0.75f;
            }
            else
                shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && maximumStackSize() > 1 && Stack > 1) || drawStackNumber == StackDrawType.Draw_OneInclusive) && (double)scaleSize > 0.3 && Stack != int.MaxValue;

            if (drawShadow)
                spriteBatch.Draw(Game1.shadowTexture, location + new Vector2(32f, 48f), new Rectangle?(Game1.shadowTexture.Bounds), color * 0.5f, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f, SpriteEffects.None, layerDepth - 0.0001f);
            spriteBatch.Draw(Texture, location + new Vector2(32f * scaleSize, 32f * scaleSize), SourceRect, color * transparency, 0f, new Vector2(8f, 8f) * scaleSize, 4f * scaleSize, SpriteEffects.None, layerDepth);
            
            if (shouldDrawStackNumber)
                Utility.drawTinyDigits(Stack, spriteBatch, location + new Vector2(64 - Utility.getWidthOfTinyDigitString(Stack, 3f * scaleSize) + 3f * scaleSize, 64f - 18f * scaleSize + 1f), 3f * scaleSize, 1f, color);

            if (drawStackNumber != StackDrawType.Hide && Quality > 0)
            {
                Rectangle quality_rect = (Quality < 4) ? new(338 + (Quality - 1) * 8, 400, 8, 8) : new(346, 392, 8, 8);
                Texture2D quality_sheet = Game1.mouseCursors;
                float yOffset = (Quality < 4) ? 0f : (((float)Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * 3.141592653589793 / 512.0) + 1f) * 0.05f);
                spriteBatch.Draw(quality_sheet, location + new Vector2(12f, 52f + yOffset), quality_rect, color * transparency, 0f, new Vector2(4f, 4f), 3f * scaleSize * (1f + yOffset), SpriteEffects.None, layerDepth);
            }

            if (isRecipe)
                spriteBatch.Draw(Game1.objectSpriteSheet, location + new Vector2(16f, 16f), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, 451, 16, 16), color, 0f, Vector2.Zero, 3f, SpriteEffects.None, layerDepth + 0.0001f);
        }

        public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
        {
            spriteBatch.Draw(Texture, objectPosition, SourceRect, Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, Math.Max(0f, (f.getStandingY() + 3) / 10000f));
        }

        public override void draw(SpriteBatch spriteBatch, int x, int y, float alpha = 1)
        {
            yBob = (float)Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 500.0f + (x * 64f)) * 8.0f + 8.0f;
            if (ModEntry.HasAlternativeTextures)
            {
                Rectangle sourceRect = Rectangle.Empty;
                Texture2D? texture = ModEntry.IAlternativeTexturesApi?.GetTextureForObject(this, out sourceRect);
                if (texture is not null && sourceRect != Rectangle.Empty)
                    spriteBatch.Draw(texture, Game1.GlobalToLocal(Game1.viewport, directionOffset + new Vector2(x * 64f, y * 64f + yBob)), sourceRect, Color.White, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, ((y * 64f) + directionOffset.Y + (x % 4)) / 10000.0f);
                else
                    drawDefault(spriteBatch, x, y, alpha);
            }
            else
                drawDefault(spriteBatch, x, y, alpha);
            if (!readyForHarvest.Value || heldObject.Value is null) return;
            float num = 4.0f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
            spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64f - 8, y * 64f - 112 + num)), new Rectangle(141, 465, 20, 24), Color.White * .75f, 0.0f, Vector2.Zero, 4f, SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999997475243E-07 + TileLocation.X / 10000.0f));
            spriteBatch.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64f + 32, y * 64f - 72 + num)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, heldObject.Value.ParentSheetIndex, 16, 16), Color.White * .75f, 0.0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999974737875E-06 + TileLocation.X / 10000.0f));
            if (heldObject.Value.Stack > 1)
                NumberSprite.draw(heldObject.Value.Stack, spriteBatch, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64f + 32 + 20, y * 64f - 72 + num + 20)), Color.White, .5f, (float)((y + 1) * 64f / 10000.0f + 9.99999974737875E-06 + TileLocation.X / 10000.0f) + 0.001f, 1f, 0);
            if (heldObject.Value.Quality > 0)
            {
                float num2 = quality < 4 ? 0.0f : (float)((Math.Cos(Game1.currentGameTime.TotalGameTime.Milliseconds * Math.PI / 512.0) + 1.0) * 0.0500000007450581);
                spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, directionOffset.Value + new Vector2(x * 64f + 32 - 20, y * 64f - 72 + num + 20)), getSourceRectForQuality(heldObject.Value.Quality), Color.White, 0.0f, new(4f), (float)(2.0 * 1.0 * (1.0 + num2)), SpriteEffects.None, (float)((y + 1) * 64f / 10000.0f + 9.99999974737875E-06 + TileLocation.X / 10000.0f) + 0.001f);
            }
        }
    }
}
