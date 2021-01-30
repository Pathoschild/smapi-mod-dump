/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Wellbott/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Objects;
using StardewValley.Buildings;

namespace FishWellSpace
{
    public class FishWell : FishPond
    {
        public float border_width = 0.8f;
        public List<WellFishSilhouette> _wellSilhouettes;
        public FishWellConfig Config;

        /// <summary>
        /// Consttructors
        /// </summary>
        /// <param name="b"></param>
        /// <param name="tileLocation"></param>
        public FishWell(BluePrint b, Vector2 tileLocation)
            : base(b, tileLocation)
        {
            UpdateMaximumOccupancy();
            fadeWhenPlayerIsBehind.Value = false;
            Reseed();
            _fishSilhouettes = new List<PondFishSilhouette>();
            _wellSilhouettes = new List<WellFishSilhouette>();
            _jumpingFish = new List<JumpingFish>();
        }
        public FishWell()
        {
            _fishSilhouettes = new List<PondFishSilhouette>();
            _wellSilhouettes = new List<WellFishSilhouette>();
            _jumpingFish = new List<JumpingFish>();
        }

        /// <summary>
        /// The base functions are Private so these get used spottily. 
        /// I don't know enough to really understand when or why
        /// </summary>
        /// <returns></returns>
        public virtual new Vector2 GetItemBucketTile()
        {
            return new Vector2((int)tileX + 2, (int)tileY + 2);
        }

        public virtual new Vector2 GetRequestTile()
        {
            return new Vector2((int)tileX + 1, (int)tileY + 1);
        }

        public virtual new Vector2 GetCenterTile()
        {
            return new Vector2((int)tileX + 1, (int)tileY + 1);
        }

        public override Rectangle getSourceRectForMenu()
        {
            return new Rectangle(0, 0, 48, 48);
        }

        /// <summary>
        /// Custom fish shadows
        /// </summary>
        /// <returns></returns>
        public List<WellFishSilhouette> GetWellSilhouettes()
        {
            return _wellSilhouettes;
        }

        /// <summary>
        /// Slows the fish production and spawning, based on Config settings
        /// I like the randomness, but it's also easier - not sure how to gracefully handle slower fish spawning in a more consistent way
        /// </summary>
        /// <param name="dayOfMonth"></param>
        public override void dayUpdate(int dayOfMonth)
        {
            if (Config.FishWellsWorkSlower)
            {
                Item cachedOutput = output.Value;
                base.dayUpdate(dayOfMonth);
                if (Game1.random.NextDouble() > Config.SlowSpawnSpeed)
                {
                    daysSinceSpawn.Value--;
                }
                if (Game1.random.NextDouble() > Config.SlowProduceSpeed)
                {
                    output.Value = cachedOutput;
                }
            }
            else
            {
                base.dayUpdate(dayOfMonth);
            }
        }

        /// <summary>
        /// Prevent maxOccupants updating to overcap
        /// </summary>
        /// <param name="tileLocation"></param>
        /// <param name="who"></param>
        /// <returns></returns>
        public override bool doAction(Vector2 tileLocation, Farmer who)
        {
            bool baseReturn = base.doAction(tileLocation, who);
            ApplyPopulationCap();
            return baseReturn;
        }

        /// <summary>
        /// Remove excess fish and prevent futile quests
        /// </summary>
        public virtual void ApplyPopulationCap()
        {
            UpdateMaximumOccupancy();
            if (currentOccupants.Value > Config.WellPopulationCap)
            {
                currentOccupants.Value = Config.WellPopulationCap;
            }
            if (HasUnresolvedNeeds() && currentOccupants.Value == Config.WellPopulationCap)
            {
                neededItem.Value = null;
                neededItemCount.Set(-1);
                hasCompletedRequest.Value = false;
            }
        }

        /// <summary>
        /// Various bits to implement the custom fish shadows
        /// </summary>
        /// <param name="time"></param>
        public override void Update(GameTime time)
        {
            //also need to preempt the JumpFish call in the base context
            if (_numberOfFishToJump > 0 && _timeUntilFishHop > 0f && _timeUntilFishHop <= (float)time.ElapsedGameTime.TotalSeconds && JumpFish())
            {
                _numberOfFishToJump--;
                _timeUntilFishHop = Utility.RandomFloat(0.15f, 0.25f) + (float)time.ElapsedGameTime.TotalSeconds;
            }
            base.Update(time);
            SyncSilhouettes();
            for (int i = 0; i < _wellSilhouettes.Count; i++)
            {
                _wellSilhouettes[i].Update((float)time.ElapsedGameTime.TotalSeconds);
            }
        }

        public override void resetLocalState()
        {
            base.resetLocalState();
            SyncSilhouettes();
        }

        public virtual void SyncSilhouettes()
        {
            if (_fishSilhouettes.Count > _wellSilhouettes.Count)
            {
                _wellSilhouettes.Add(new WellFishSilhouette(this, border_width));
            }
            else if (_fishSilhouettes.Count < _wellSilhouettes.Count)
            {
                _wellSilhouettes.RemoveAt(0);
            }
        }

        public virtual void ReplacePondFish()
        {
            List<PondFishSilhouette> newFishSilhouettes = new List<PondFishSilhouette>();
            newFishSilhouettes = _fishSilhouettes.ConvertAll(new Converter<PondFishSilhouette, PondFishSilhouette>(PondFishToWellFish));
            _fishSilhouettes = newFishSilhouettes;
        }

        public virtual WellFishSilhouette PondFishToWellFish(PondFishSilhouette pFish)
        {
            if (pFish.GetType().Equals(typeof(WellFishSilhouette)))
            {
                return (WellFishSilhouette)pFish;
            }
            else
            return new WellFishSilhouette(this, border_width);
        }

        public new void UpdateMaximumOccupancy()
        {
            GetFishPondData();
            if (_fishPondData == null)
            {
                return;
            }
            for (int i = 1; i <= 10; i++)
            {
                if (i <= lastUnlockedPopulationGate.Value)
                {
                    maxOccupants.Set(i);
                    continue;
                }
                if (_fishPondData.PopulationGates == null || !_fishPondData.PopulationGates.ContainsKey(i))
                {
                    maxOccupants.Set(i);
                    continue;
                }
                break;
            }
            if (maxOccupants > Config.WellPopulationCap)
                maxOccupants.Set(Config.WellPopulationCap);
        }

        /// <summary>
        /// Adjust fish jumping
        /// </summary>
        /// <returns></returns>
        public new bool JumpFish()
        {
            WellFishSilhouette fish_silhouette = null;
            if (_fishSilhouettes.Count == 0)
            {
                return false;
            }
            fish_silhouette = Utility.GetRandom(_wellSilhouettes);
            _wellSilhouettes.Remove(fish_silhouette);
            _fishSilhouettes.RemoveAt(0);
            _jumpingFish.Add(new JumpingFish(this, 
                fish_silhouette.position,
                new Vector2(
                    tileX.Value + Utility.Lerp(border_width + 0.2f, (float)tilesWide - border_width - 0.2f, (float)Game1.random.NextDouble()),
                    tileY.Value + Utility.Lerp(border_width + 0.2f, (float)tilesHigh - border_width - 0.2f, (float)Game1.random.NextDouble())) *64f
                ));
            return true;
        }

        /// <summary>
        /// Have to repace some values, move some pieces for the new textures to work
        /// Also makes some of the private overrides work
        /// </summary>
        /// <param name="b"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void drawInMenu(SpriteBatch b, int x, int y)
        {
            y += 32;
            drawShadow(b, x, y);
            b.Draw(texture.Value, new Vector2(x, y), new Rectangle(0, 48, 48, 48), new Color(60, 126, 150) * alpha, 0f, new Vector2(0f, 0f), 4f, SpriteEffects.None, 1f);
            for (int yWater = tileY; yWater < (int)tileY + 3; yWater++)
            {
                for (int xWater = tileX; xWater < (int)tileX + 2; xWater++)
                {
                    bool num = yWater == (int)tileY + 2;
                    bool topY = yWater == (int)tileY;
                    if (num)
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(x + xWater * 64 + 32, y + (yWater + 1) * 64 - (int)Game1.currentLocation.waterPosition - 32), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((xWater + yWater) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)), 64, 32 + (int)Game1.currentLocation.waterPosition - 5), Game1.currentLocation.waterColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    }
                    else
                    {
                        b.Draw(Game1.mouseCursors, new Vector2(x + xWater * 64 + 32, y + yWater * 64 + 32 - (int)((!topY) ? Game1.currentLocation.waterPosition : 0f)), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((xWater + yWater) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)) + (topY ? ((int)Game1.currentLocation.waterPosition) : 0), 64, 64 + (topY ? ((int)(0f - Game1.currentLocation.waterPosition)) : 0)), Game1.currentLocation.waterColor, 0f, Vector2.Zero, 1f, SpriteEffects.None, 1f);
                    }
                }
            }
            b.Draw(texture.Value, new Vector2(x, y), new Rectangle(0, 0, 48, 48), color.Value * alpha, 0f, new Vector2(0f, 0f), 4f, SpriteEffects.None, 1f);
            b.Draw(texture.Value, new Vector2(x + 64, y + 44 + ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2500.0 < 1250.0) ? 4 : 0)), new Rectangle(16, 160, 48, 7), color.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, 1f);
            //no netting for now
            //b.Draw(texture.Value, new Vector2(x, y - 128), new Rectangle(48, 0, 48, 48), color.Value * alpha, 0f, new Vector2(0f, 0f), 4f, SpriteEffects.None, 1f);
        }

        public override void draw(SpriteBatch b)
        {
            if (base.isMoving)
            {
                return;
            }
            if ((int)daysOfConstructionLeft > 0)
            {
                drawInConstruction(b);
                return;
            }
            for (int l = animations.Count - 1; l >= 0; l--)
            {
                animations[l].draw(b);
            }
            drawShadow(b);
            //pond bed
            b.Draw(texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64, (int)tileY * 64 + (int)tilesHigh * 64)), new Rectangle(0, 48, 48, 48), (overrideWaterColor.Equals(Color.White) ? new Color(60, 126, 150) : ((Color)overrideWaterColor)) * alpha, 0f, new Vector2(0f, 48f), 4f, SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f - 3f) / 10000f);
            for (int y = tileY; y < (int)tileY + 3; y++)
            {
                for (int x = tileX; x < (int)tileX + 2; x++)
                {
                    //water textures
                    bool num = y == (int)tileY + 2;
                    bool topY = y == (int)tileY;
                    if (num)
                    {
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32, (y + 1) * 64 - (int)Game1.currentLocation.waterPosition - 32)), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((x + y) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)), 64, 32 + (int)Game1.currentLocation.waterPosition - 5), overrideWaterColor.Equals(Color.White) ? ((Color)Game1.currentLocation.waterColor) : (overrideWaterColor.Value * 0.5f), 0f, Vector2.Zero, 1f, SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f - 2f) / 10000f);
                    }
                    else
                    {
                        b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32, y * 64 + 32 - (int)((!topY) ? Game1.currentLocation.waterPosition : 0f))), new Rectangle(Game1.currentLocation.waterAnimationIndex * 64, 2064 + (((x + y) % 2 != 0) ? ((!Game1.currentLocation.waterTileFlip) ? 128 : 0) : (Game1.currentLocation.waterTileFlip ? 128 : 0)) + (topY ? ((int)Game1.currentLocation.waterPosition) : 0), 64, 64 + (topY ? ((int)(0f - Game1.currentLocation.waterPosition)) : 0)), overrideWaterColor.Equals(Color.White) ? ((Color)Game1.currentLocation.waterColor) : (overrideWaterColor.Value * 0.5f), 0f, Vector2.Zero, 1f, SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f - 2f) / 10000f);
                    }
                }
            }
            if (overrideWaterColor.Value.Equals(Color.White))
            {
                //water recolor?
                b.Draw(texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64 + 64, (int)tileY * 64 + 44 + ((Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 2500.0 < 1250.0) ? 4 : 0))), new Rectangle(16, 160, 48, 7), color.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f + 1f) / 10000f);
            }
            //main structure
            b.Draw(texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64, (int)tileY * 64 + (int)tilesHigh * 64)), new Rectangle(0, 0, 48, 48), color.Value * alpha, 0f, new Vector2(0f, 48f), 4f, SpriteEffects.None, ((float)(int)tileY + 0.5f) * 64f / 10000f);
            if (nettingStyle.Value < 3)
            {
                //netting styles, disabled for now
                //b.Draw(texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64, (int)tileY * 64 + (int)tilesHigh * 64 - 128)), new Rectangle(80, (int)nettingStyle * 48, 80, 48), color.Value * alpha, 0f, new Vector2(0f, 80f), 4f, SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f + 2f) / 10000f);
            }
            if (sign.Value != null)
            {
                //sign sprite
                //layers moved up to cover grass n' trees n' stuff
                b.Draw(Game1.bigCraftableSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64 + 8 - 20, (int)tileY * 64 + (int)tilesHigh * 64 - 128 - 32 + 16)), Game1.getSourceRectForStandardTileSheet(Game1.bigCraftableSpriteSheet, sign.Value.parentSheetIndex, 16, 32), color.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (((float)(int)tileY + 2.5f) * 64f + 2f) / 10000f); // previous tileY offset was 0.5f
                if (fishType.Value != -1)
                {
                    //fish sprites
                    b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64 + 8 + 8 - 4 - 20, (int)tileY * 64 + (int)tilesHigh * 64 - 128 - 8 + 4 + 16)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, fishType.Value, 16, 16), Color.Black * 0.4f * alpha, 0f, Vector2.Zero, 3f, SpriteEffects.None, (((float)(int)tileY + 2.5f) * 64f + 3f) / 10000f);
                    b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64 + 8 + 8 - 1 - 20, (int)tileY * 64 + (int)tilesHigh * 64 - 128 - 8 + 1 + 16)), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, fishType.Value, 16, 16), color.Value * alpha, 0f, Vector2.Zero, 3f, SpriteEffects.None, (((float)(int)tileY + 2.5f) * 64f + 4f) / 10000f);
                    //number
                    Utility.drawTinyDigits(currentOccupants.Value, b, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64 + 32 + 8 - 20 + ((currentOccupants.Value < 10) ? 8 : 0), (int)tileY * 64 + (int)tilesHigh * 64 - 96 + 16)), 3f, (((float)(int)tileY + 2.5f) * 64f + 5f) / 10000f, Color.LightYellow * alpha);
                }
            }
            if (_fishObject != null && ((int)_fishObject.parentSheetIndex == 393 || (int)_fishObject.parentSheetIndex == 397))
            {
                //resize and redistribute the coral, sea urchins
                for (int k = 0; k < (int)currentOccupants; k++)
                {
                    Vector2 drawOffset = Vector2.Zero;
                    float coralResize = 0.7f;
                    int drawI = (k + seedOffset.Value) % 10;
                    switch (drawI)
                    {
                        case 8: //0
                            drawOffset = new Vector2(0f, 0f);
                            break;
                        case 0: //1
                            drawOffset = new Vector2(48f, 32f);
                            break;
                        case 4: //2
                            drawOffset = new Vector2(80f, 72f);
                            break;
                        case 5: //3
                            drawOffset = new Vector2(140f, 28f);
                            break;
                        case 7: //4
                            drawOffset = new Vector2(96f, 0f);
                            break;
                        case 3: //5
                            drawOffset = new Vector2(0f, 96f);
                            break;
                        case 2: //6
                            drawOffset = new Vector2(140f, 80f);
                            break;
                        case 1: //7
                            drawOffset = new Vector2(64f, 120f);
                            break;
                        case 6: //8
                            drawOffset = new Vector2(140f, 140f);
                            break;
                        case 9: //9
                            drawOffset = new Vector2(0f, 150f);
                            break;
                    }
                    drawOffset = drawOffset * 0.6f + new Vector2(-28f, -28f);
                    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64 + 64 + (int)(7 * coralResize), (int)tileY * 64 + 64 + (int)(32 * coralResize)) + drawOffset), Game1.shadowTexture.Bounds, color.Value * alpha, 0f, Vector2.Zero, 3f * coralResize, SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f - 2f) / 10000f - 1.1E-05f);
                    b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64 + 64, (int)tileY * 64 + 64) + drawOffset), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, fishType.Value, 16, 16), color.Value * alpha * 0.75f, 0f, Vector2.Zero, 3f * coralResize, (drawI % 3 == 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f - 2f) / 10000f - 1E-05f);
                }
            }
            else
            {
                //draw custom shadows instead
                for (int j = 0; j < _wellSilhouettes.Count; j++)
                {
                    _wellSilhouettes[j].Draw(b);
                }
            }
            for (int i = 0; i < _jumpingFish.Count; i++)
            {
                _jumpingFish[i].Draw(b);
            }
            if (HasUnresolvedNeeds())
            {
                //quest marker
                Vector2 drawn_position = GetRequestTile() * 64f;
                drawn_position += 64f * new Vector2(0.5f, 0.5f);
                float y_offset2 = 3f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                float bubble_layer_depth2 = (drawn_position.Y + 160f) / 10000f + 1E-06f;
                drawn_position.Y += y_offset2 - 32f;
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, drawn_position), new Rectangle(403, 496, 5, 14), Color.White * 0.75f, 0f, new Vector2(2f, 14f), 4f, SpriteEffects.None, bubble_layer_depth2);
            }
            if (output.Value != null)
            {
                //full bucket and output bubble
                b.Draw(texture.Value, Game1.GlobalToLocal(Game1.viewport, new Vector2((int)tileX * 64, (int)tileY * 64) + new Vector2(124f, 128f)), new Rectangle(0, 96, 17, 16), color.Value * alpha, 0f, Vector2.Zero, 4f, SpriteEffects.None, (((float)(int)tileY + 0.5f) * 64f + 1f) / 10000f);
                Vector2 value = GetItemBucketTile() * 64f;
                float y_offset = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
                Vector2 bubble_draw_position = value + new Vector2(0.15f, -1.4f) * 64f + new Vector2(0f, y_offset);
                Vector2 item_relative_to_bubble = new Vector2(40f, 36f);
                float bubble_layer_depth = (value.Y + 64f) / 10000f + 1E-06f;
                float item_layer_depth = (value.Y + 64f) / 10000f + 1E-05f;
                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, bubble_draw_position), new Rectangle(141, 465, 20, 24), Color.White * 0.75f, 0f, Vector2.Zero, 4f, SpriteEffects.None, bubble_layer_depth);
                b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, bubble_draw_position + item_relative_to_bubble), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, output.Value.parentSheetIndex, 16, 16), Color.White * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, item_layer_depth);
                if (output.Value is ColoredObject)
                {
                    b.Draw(Game1.objectSpriteSheet, Game1.GlobalToLocal(Game1.viewport, bubble_draw_position + item_relative_to_bubble), Game1.getSourceRectForStandardTileSheet(Game1.objectSpriteSheet, (int)output.Value.parentSheetIndex + 1, 16, 16), (output.Value as ColoredObject).color.Value * 0.75f, 0f, new Vector2(8f, 8f), 4f, SpriteEffects.None, item_layer_depth + 1E-05f);
                }
            }
        }
    }
}
