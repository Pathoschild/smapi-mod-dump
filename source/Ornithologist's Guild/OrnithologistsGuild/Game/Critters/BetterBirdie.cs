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
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using OrnithologistsGuild.Content;
using StardewValley;
using System.Collections.Generic;
using StateMachine;
using StardewValley.TerrainFeatures;

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

        public float ZIndex { get
            {
                return (position.Y / 10000f) + (position.X / 1000000f);
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
            get { return IsPerched && (Perch.MapTile.HasValue || Perch.Tree != null); }
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
                if (IsRoosting && Perch.Tree != null)
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

        public bool CheckRelocationDistance(Vector2 relocateTo)
        {
            var currentTile = base.position / Game1.tileSize;

            var distance = Vector2.Distance(currentTile, relocateTo);
            if (distance < 10) return false; // Too close

            var distanceX = MathF.Abs(currentTile.X - relocateTo.X);
            var distanceY = MathF.Abs(currentTile.Y - relocateTo.Y);
            if (distanceX < 5 || distanceY < 5 ) return false; // Too straight (lol)

            return true;
        }

        public Tuple<Vector3, Perch> GetRandomRelocationTileOrPerch()
        {
            if (Game1.random.NextDouble() < 0.8)
            {
                // Try to find clear tile to relocate to
                for (int trial = 0; trial < 50; trial++)
                {
                    var randomTile = Environment.getRandomTile();
                    if (Environment.isWaterTile((int)randomTile.X, (int)randomTile.Y)) continue; // On water (this may not be enough)

                    // Get a 3x3 patch around the random tile
                    var randomRect = new Microsoft.Xna.Framework.Rectangle((int)randomTile.X - 1, (int)randomTile.Y - 1, 3, 3);
                    if (!CheckRelocationDistance(randomTile)) continue; // Too close/straight

                    if (Environment.isAreaClear(randomRect) && Utility.isThereAFarmerOrCharacterWithinDistance(randomTile, BirdieDef.GetContextualCautiousness(), Environment) != null) continue; // Character nearby

                    var relocateTo = randomTile * Game1.tileSize;

                    // Center on tile
                    relocateTo.X += 32f;
                    relocateTo.Y += 32f;

                    return new Tuple<Vector3, Perch>(new Vector3(relocateTo.X, relocateTo.Y, 0), null);
                }
            }
            else
            {
                // Try to find an available perch to relocate to
                var perch = Perch.GetRandomAvailablePerch(Game1.player.currentLocation, BirdieDef, this); 
                if (perch != null)
                {
                    return new Tuple<Vector3, Perch>(perch.Position, perch);
                }
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
            StateMachine.Trigger(Game1.random.NextDouble() < 0.8 ? BetterBirdieTrigger.FlyAway : BetterBirdieTrigger.Relocate);
        }
    
        #region Rendering
        public void drawBirdie(SpriteBatch b)
        {
            if (sprite != null)
            {
                // Experimental z-index
                sprite.draw(b, Game1.GlobalToLocal(Game1.viewport, position + new Vector2(-64f, -128f + yJumpOffset + yOffset)), ZIndex, 0, 0, Color.White, flip, 4f);
                b.Draw(Game1.shadowTexture, Game1.GlobalToLocal(Game1.viewport, position + new Vector2(0f, -4f)), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f + Math.Max(-3f, (yJumpOffset + yOffset) / 64f), SpriteEffects.None, ZIndex - (1f / 10000f));
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
        #endregion
    }
}
