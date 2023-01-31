/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/greyivy/OrnithologistsGuild
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrnithologistsGuild.Content;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.TerrainFeatures;
using StateMachine;

namespace OrnithologistsGuild.Game.Critters
{
    public partial class BetterBirdie : StardewValley.BellsAndWhistles.Critter
    {
        public BirdieDef BirdieDef;
        private GameLocation Environment;

        public Fsm<BetterBirdieState, BetterBirdieTrigger> StateMachine;

        public bool IsFlying { get {
            return StateMachine.Current.Identifier == BetterBirdieState.Relocating || StateMachine.Current.Identifier == BetterBirdieState.FlyingAway;
        } }

        public Vector3 Position3 { get
            {
                return new Vector3(position.X, position.Y, yOffset);
            } set
            {
                position = Utilities.XY(value);
                yOffset = value.Z;
            }
        }
        public Vector2 TileLocation
        {
            get
            {
                return Utilities.XY(Position3) / Game1.tileSize;
            }
        }

        public float ZIndex { get
            {
                return (position.Y / (10000f - 1f)) + (position.X / 1000000f);
            }
        }

        // Perch
        public Perch Perch;
        public bool IsPerched
        {
            get { return Perch != null; }
        }
        public bool IsRoosting
        {
            get { return IsPerched && (Perch.Type == PerchType.MapTile || Perch.Type == PerchType.Tree); }
        }
        public bool IsBathing
        {
            get { return IsPerched && Perch.Type == PerchType.Bath; }
        }

        // Timers
        private int CharacterCheckTimer = 200;

        public bool IsSpotted; // Whether player has spotted bird with binoculars

        private float FlySpeedOffset; // Individual birds fly at slightly different speeds

        public BetterBirdie(BirdieDef birdieDef, Vector2 tileLocation, Perch perch = null) : base(0, Vector2.Zero)
        {
            BirdieDef = birdieDef;
            Perch = perch;

            if (birdieDef.AssetPath != null)
            {
                var internalAssetName = birdieDef.ContentPackDef.ContentPack.ModContent.GetInternalAssetName(birdieDef.AssetPath).BaseName;

                baseFrame = birdieDef.BaseFrame;
                sprite = new AnimatedSprite(internalAssetName, baseFrame, 32, 32);
            } else
            {
                baseFrame = birdieDef.BaseFrame;
                sprite = new AnimatedSprite(critterTexture, baseFrame, 32, 32);
            }

            flip = Game1.random.NextDouble() < 0.5;

            // Position
            if (Perch == null)
            {
                position = tileLocation * Game1.tileSize;

                // Center on tile
                position.X += Game1.tileSize / 2;
                position.Y += Game1.tileSize / 2;

                // Scatter by up to half a tile in any direction
                position = Utility.getTranslatedVector2(position, Game1.random.Next(4), Game1.random.Next(Game1.tileSize / 2));
            } else
            {
                Position3 = Perch.Position;
            }
            startingPosition = position;

            FlySpeedOffset = (float)Game1.random.NextDouble() - 0.5f;

            InitializeStateMachine();
        }

        public override bool update(GameTime time, GameLocation environment)
        {
            Environment = environment;

            if (!IsFlying)
            {
                if (IsRoosting && Perch.Type == PerchType.Tree)
                {
                    // Fly away when tree is chopped
                    if (Perch.Tree.health.Value < Tree.startingHealth)
                    {
                        Frighten();
                    }

                    // Fly away when tree is shaken (see TreePatches)
                }
                else
                {
                    CheckCharacterProximity(time, environment);
                }
            }

            StateMachine.Update(time.ElapsedGameTime);

            updateEmote(time);

            return base.update(time, environment);
        }

        public void PlayCall()
        {
            Game1.playSound(BirdieDef.SoundID == null ? "SpringBirds" : BirdieDef.SoundID);
        }

        public void Flip()
        {
            flip = !flip;
        }

        public bool CheckRelocationDistance(Vector2 relocateTo)
        {
            var currentTile = base.position / Game1.tileSize;

            var distance = Vector2.Distance(currentTile, relocateTo);
            if (distance < 10) return false; // Too close

            var distanceX = MathF.Abs(currentTile.X - relocateTo.X);
            var distanceY = MathF.Abs(currentTile.Y - relocateTo.Y);
            if (distanceX < 6 || distanceY < 4 ) return false; // Too straight (lol)

            return true;
        }

        public static bool CanSpawnAtOrRelocateTo(GameLocation location, Vector2 tileLocation, BirdieDef birdieDef, BetterBirdie birdie = null, bool shouldBathe = false)
        {
            if (!location.isTileOnMap(tileLocation)) return false; // Tile not on map

            var isOpenWater = location.isOpenWater((int)tileLocation.X, (int)tileLocation.Y);
            if (isOpenWater && (!birdieDef.CanBathe || !shouldBathe)) return false; // Bird should not bathe

            if (birdie != null && !birdie.CheckRelocationDistance(tileLocation)) return false; // Too close/straight

            if (Utility.isThereAFarmerOrCharacterWithinDistance(tileLocation, birdieDef.GetContextualCautiousness(), location) != null) return false; // Character nearby

            if (!isOpenWater)
            {
                // Get a 3x3 patch around the random tile
                var randomRect = new Microsoft.Xna.Framework.Rectangle((int)tileLocation.X - 1, (int)tileLocation.Y - 1, 3, 3);
                if (!location.isAreaClear(randomRect)) return false; // Area not clear
            }

            return true;
        }

        public Tuple<Vector3, Perch> GetRandomRelocationTileOrPerch()
        {
            if (ModEntry.debug_PerchType.HasValue || Game1.random.NextDouble() < 0.25)
            {
                // Try to find an available perch to relocate to
                var perch = Perch.GetRandomAvailablePerch(Game1.player.currentLocation, BirdieDef, this);
                if (perch != null)
                {
                    return new Tuple<Vector3, Perch>(perch.Position, perch);
                }
            }

            var shouldBathe = Game1.random.NextDouble() < 0.25; // 25% chance to allow bathing

            // Try to find clear tile to relocate to
            for (int trial = 0; trial < 50; trial++)
            {
                var randomTile = Environment.getRandomTile();
                if (!CanSpawnAtOrRelocateTo(Environment, randomTile, BirdieDef, this, shouldBathe)) continue;

                var relocateTo = randomTile * Game1.tileSize;

                // Center on tile
                relocateTo.X += 32f;
                relocateTo.Y += 32f;

                return new Tuple<Vector3, Perch>(new Vector3(relocateTo.X, relocateTo.Y, 0), null);
            }

            return null;
        }

        private void CheckCharacterProximity(GameTime time, GameLocation environment)
        {
            CharacterCheckTimer -= time.ElapsedGameTime.Milliseconds;
            if (CharacterCheckTimer < 0)
            {
                CharacterCheckTimer = 200;

                if (!IsFlying && Utility.isThereAFarmerOrCharacterWithinDistance(position / Game1.tileSize, BirdieDef.GetContextualCautiousness(), environment) != null)
                {
                    Frighten();
                }
            }
        }

        public void Frighten() {
            if (ModEntry.debug_PerchType.HasValue || ModEntry.debug_BirdWhisperer.HasValue)
            {
                // Force relocate
                StateMachine.Trigger(BetterBirdieTrigger.Relocate);
                return;
            }

            if (Game1.random.NextDouble() < 0.8) StateMachine.Trigger(BetterBirdieTrigger.FlyAway);
            else StateMachine.Trigger(BetterBirdieTrigger.Relocate);
        }
    
        #region Rendering
        public void drawBirdie(SpriteBatch b)
        {
            if (sprite != null)
            {
                // Override `base.draw()` for extra control of clipping when bathing
                if (sprite.Texture != null)
                {
                    var clipBottom = StateMachine.Current.Identifier == BetterBirdieState.Bathing ? BirdieDef.BathingClipBottom : 0;

                    var screenPosition = Game1.GlobalToLocal(Game1.viewport, position + new Vector2(-64f, -128f + yJumpOffset + yOffset + (clipBottom * 3)));
                    var sourceRectangle = new Rectangle(sprite.sourceRect.X, sprite.sourceRect.Y + clipBottom, sprite.sourceRect.Width, sprite.sourceRect.Height - (clipBottom * 2));

                    b.Draw(sprite.Texture, screenPosition, sourceRectangle, Color.White, 0, Vector2.Zero, 4f, (flip || (sprite.CurrentAnimation != null && sprite.CurrentAnimation[sprite.currentAnimationIndex].flip)) ? SpriteEffects.FlipHorizontally : SpriteEffects.None, ZIndex);
                }

                if (StateMachine.Current.Identifier != BetterBirdieState.Bathing)
                {
                    // Draw shadow
                    b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, position + new Vector2(0f, -4f)), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f + Math.Max(-3f, (yJumpOffset + yOffset) / 64f), SpriteEffects.None, ZIndex - (1f / 10000f));
                }
            }
        }
        private void drawEmote(SpriteBatch b)
        {
            if (isEmoting)
            {
                Vector2 localPosition = getLocalPosition(Game1.viewport);
                localPosition.Y -= 118f - yOffset;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Microsoft.Xna.Framework.Rectangle(currentEmoteFrame * 16 % Game1.emoteSpriteSheet.Width, currentEmoteFrame * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16), Color.White /** alpha*/, 0f, Vector2.Zero, 4f, SpriteEffects.None, ZIndex);
            }
        }

        public override void drawAboveFrontLayer(SpriteBatch b)
        {
            if (IsFlying || IsRoosting)
            {
                drawBirdie(b);
                drawEmote(b);
            }
        }

        public override void draw(SpriteBatch b)
        {
            if (!(IsFlying || IsRoosting))
            {
                drawBirdie(b);
                drawEmote(b);
            }
        }

        public Vector2 getLocalPosition(xTile.Dimensions.Rectangle viewport)
        {
            Vector2 vector = position;
            return new Vector2(vector.X - (float)viewport.X - (0.5f * Game1.tileSize), vector.Y - (float)viewport.Y + (float)yJumpOffset);
        }

        private List<FarmerSprite.AnimationFrame> GetFlyingAnimation()
        {
            return new List<FarmerSprite.AnimationFrame> {
                    new FarmerSprite.AnimationFrame (baseFrame + 6, (int)MathF.Round(0.27f * BirdieDef.FlapDuration)),
                    new FarmerSprite.AnimationFrame (baseFrame + 7, (int)MathF.Round(0.23f * BirdieDef.FlapDuration), secondaryArm: false, flip, frameBehavior: (Farmer who) =>
                    {
                        // Make bird shoot up a bit while flapping for more realistic flight
                        // e.g. flapDuration = 500, gravityAffectedDY = 4
                        // e.g. flapDuration = 250, gravityAffectedDY = 2
                        gravityAffectedDY = -(BirdieDef.FlapDuration * (4f/500f));

                        // Play flapping noise
                        if (Utility.isOnScreen(position, Game1.tileSize)) Game1.playSound("batFlap");
                    }),
                    new FarmerSprite.AnimationFrame (baseFrame + 8, (int)MathF.Round(0.27f * BirdieDef.FlapDuration)),
                    new FarmerSprite.AnimationFrame (baseFrame + 7, (int)MathF.Round(0.23f * BirdieDef.FlapDuration))
                };
        }

        private List<FarmerSprite.AnimationFrame> GetBathingAnimation()
        {
            return new List<FarmerSprite.AnimationFrame> {
                    new FarmerSprite.AnimationFrame (baseFrame + 6, (int)MathF.Round(0.27f * (BirdieDef.FlapDuration / 2))),
                    new FarmerSprite.AnimationFrame (baseFrame + 7, (int)MathF.Round(0.23f * (BirdieDef.FlapDuration / 2)), secondaryArm: false, flip, frameBehavior: (Farmer who) =>
                    {
                        // Play bathing noise whether onscreen or not
                        Game1.playSound("waterSlosh");

                        if (Utility.isOnScreen(position, Game1.tileSize)) {
                            // Draw splashes
                            var splashPosition = new Vector2(position.X - (Game1.tileSize * 0.75f), position.Y - (Game1.tileSize * 1.5f));
                            splashPosition.X += (int)((-0.5 + Game1.random.NextDouble()) * (Game1.tileSize / 1.5));
                            splashPosition.Y += (int)((-0.5 + Game1.random.NextDouble()) * (Game1.tileSize / 1.5));

                            var splash = new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(0, 832, 64, 64), (BirdieDef.FlapDuration / 2) / 10, 10, 1, splashPosition, false, Game1.random.Next(0, 2) == 0);
                            splash.layerDepth = ZIndex + 2f;
                            splash.scale = 1 + Utility.RandomFloat(0, 0.75f);
                            Environment.temporarySprites.Add(splash);
                        }
                    }),
                    new FarmerSprite.AnimationFrame (baseFrame + 8, (int)MathF.Round(0.27f * (BirdieDef.FlapDuration / 2))),
                    new FarmerSprite.AnimationFrame (baseFrame + 7, (int)MathF.Round(0.23f * (BirdieDef.FlapDuration / 2)))
                };
        }
        #endregion
    }
}
