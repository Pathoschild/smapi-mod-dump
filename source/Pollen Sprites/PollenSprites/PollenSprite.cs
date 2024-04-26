/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/PollenSprites
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Monsters;
using System;

namespace PollenSprites
{
    /// <summary>A custom monster based on Stardew's Ghost class.</summary>
    /// <remarks> This subclass copies all of its contents from Ghost, except those added/overridden inside this class.</remarks>
    public class PollenSprite : Ghost
    {
        /// <summary>If true, this monster will apply a debuff on contact with players.</summary>
        public bool EnableDebuff
        {
            get
            {
                return ModEntry.ModConfig.EnableSlowDebuff; //use the local player's config.json setting
            }
        }

        /// <summary>The monster will reduce a player's energy by this amount on contact.</summary>
        public int EnergyDamage
        {
            get
            {
                if (ModEntry.ModConfig.EnableEnergyDrain) //if the local player's config.json setting is enabled
                    return 2; //use the preset damage amount
                else
                    return 0; //don't damage energy
            }
        }

        private Color? effectColor = null;
        /// <summary>The color used by some of this monster's visual effects.</summary>
        public Color EffectColor
        {
            get
            {
                if (effectColor == null) //if a color hasn't been selected yet
                {
                    GameLocation loc = currentLocation ?? Game1.currentLocation; //try to get this monster's current location, or at least the local player's location
                    if (loc?.GetSeason() == Season.Fall) //if a location was found and it's fall there
                    {
                        effectColor = new Color(246, 97, 76); //red-orange, based on the fall sprite
                    }
                    else
                    {
                        effectColor = new Color(255, 183, 255); //pink, based on the spring sprite
                    }
                }

                return effectColor.Value; //use the cached color
            }

            set
            {
                effectColor = value;
            }
        }

        /// <summary>The internal name for this monster. Needs to match the name used by its spritesheet, as in "Characters/Monsters/Esca.PollenSprites".</summary>
        protected static string customName = "Esca.PollenSprites";

        /// <summary>The total game time (in milliseconds) when this monster last used the "sprinkles" visual effect.</summary>
        protected double timeOfLastSprinkle = 0;

        /// <summary>The approximate amount of time (in milliseconds) between this monster's "sprinkle" visual effects.</summary>
        protected double sprinkleCooldown = 2000;

        /// <summary>The total game time (in milliseconds) when this monster last harmed a player.</summary>
        protected double timeOfLastPlayerHarm = 0;

        /// <summary>The approximate amount of time (in milliseconds) between this monster's attempts to harm the player.</summary>
        protected double playerHarmCooldown = 1000;

        /// <summary>The spritesheet coordinantes to use in <see cref="shedChunks(int, float)"/>.</summary>
        protected Rectangle chunkSpriteRect = new Rectangle(0, 96, 16, 16);

        /// <summary>True if this monster is currently turning right.</summary>
        /// <remarks>This is a more accessible duplicate of Ghost.turningRight for use in the updateAnimation override method.</remarks>
        protected bool turningRight = false;

        /// <summary>Creates an non-functional instance of this monster for use by certain Stardew methods.</summary>
        /// <remarks>This should not be used by mods to create new monsters, as it avoids some necessary settings and methods.</remarks>
        public PollenSprite()
            : base() //call the default Ghost constructor
        {
        }

        /// <summary>Creates a new instance of this monster class.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        /// <remarks>
        /// Farm Type Manager calls this constructor (i.e. the constructor with a Vector2 parameter and no others) when it creates a custom monster from another mod.
        /// </remarks>
        public PollenSprite(Vector2 position)
            : base(position, customName) //call the Ghost class's "position, name" constructor
        {
            HideShadow = true; //hide this monster's shadow, preventing a "double shadow" bug in most game locations
            Scale = (float)Game1.random.Next(70, 91) / 100; //randomly choose a size from 70-90%

            if (Game1.random.NextSingle() < ModEntry.ModConfig.SeedDropChances.MixedSeeds) //if mixed seeds should be dropped
            {
                objectsToDrop.Add(SeedManager.MixedSeeds); //add mixed seeds to this monster's drop list
            }

            if (Game1.random.NextSingle() < ModEntry.ModConfig.SeedDropChances.FlowerSeeds) //if flower seeds should be dropped
            {
                objectsToDrop.Add(SeedManager.FlowerSeeds); //add mixed flower seeds to this monster's drop list
            }

            if (Game1.random.NextSingle() < ModEntry.ModConfig.SeedDropChances.AllSeeds) //if entirely random seeds should be dropped
            {
                string randomSeed = SeedManager.AllSeeds[Game1.random.Next(0, SeedManager.AllSeeds.Count)]; //get a random seed ID
                objectsToDrop.Add(randomSeed); //add the random seed to this monster's drop list
            }
        }

        /// <summary>This override forces instances of GameLocation to call drawAboveAllLayers, fixing a bug where flying monsters are invisible on some maps.</summary>
        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            base.drawAboveAlwaysFrontLayer(b); //call the base version of this method
            drawAboveAllLayers(b); //call the method that draws this monster above everything else
        }

        /// <summary>This override is a modified copy of the DustSpirit method.</summary>
        /// <remarks>
        /// * A sound effect was added.
        /// * The source rectangle was modified to use this monster's spritesheet.
        /// * The number of chunks was multiplied.
        /// * The scale parameter was used instead of a local health-based value.
        /// </remarks>
        public override void shedChunks(int number, float scale)
        {
            this.currentLocation.localSound("leafrustle"); //play the leaf rustling sound effect
            Game1.createRadialDebris(this.currentLocation, this.Sprite.textureName.Value, chunkSpriteRect, 8, StandingPixel.X, StandingPixel.Y, number * 2, base.TilePoint.Y, Color.White, scale * 2);
        }

        /// <summary>This override is a modified copy of the DustSpirit version.</summary>
        /// <remarks>
        /// * A leaf rustle sound was added.
        /// * The temporary sprites were modified to use this monster's preset effect color.
        /// * The temporary sprites' texture row numbers were changed to use a leaf animation.
        /// </remarks>
        protected override void localDeathAnimation()
        {
            this.currentLocation.localSound("leafrustle"); //play the leaf rustling sound effect
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position, EffectColor, 10, false, 100f, 0, -1, -1f, -1, 0)); //use animation row 50 (leaf animation used by weeds) for each sprite
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position + new Vector2((float)Game1.random.Next(-32, 32), (float)Game1.random.Next(-32, 32)), EffectColor, 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 150,
                scale = 0.5f
            });
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position + new Vector2((float)Game1.random.Next(-32, 32), (float)Game1.random.Next(-32, 32)), EffectColor, 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 300,
                scale = 0.5f
            });
            this.currentLocation.temporarySprites.Add(new TemporaryAnimatedSprite(50, this.Position + new Vector2((float)Game1.random.Next(-32, 32), (float)Game1.random.Next(-32, 32)), EffectColor, 10, false, 100f, 0, -1, -1f, -1, 0)
            {
                delayBeforeAnimationStart = 450,
                scale = 0.5f
            });
        }

        /// <summary>This override makes this monster's shadow transparent and scales it to the monster's size.</summary>
        public override void drawAboveAllLayers(SpriteBatch b)
        {
            Color shadowColor = new Color(255, 255, 255, 63); //75% transparency
            float shadowScale = (3f + (float)this.yOffset / 20f) * Scale; //multiply by the monster's scale

            int standingY = base.StandingPixel.Y;
            b.Draw(this.Sprite.Texture, base.getLocalPosition(Game1.viewport) + new Vector2(32f, 21 + this.yOffset), this.Sprite.SourceRect, Color.White, 0f, new Vector2(8f, 16f), Math.Max(0.2f, base.scale.Value) * 4f, base.flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, base.drawOnTop ? 0.991f : ((float)standingY / 10000f)));
            b.Draw(Game1.shadowTexture, base.getLocalPosition(Game1.viewport) + new Vector2(32f, 64f), Game1.shadowTexture.Bounds, shadowColor, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 3f + (float)this.yOffset / 20f, SpriteEffects.None, (float)(standingY - 1) / 10000f);
        }

        /// <summary>This override reworks several aspects of the Ghost version.</summary>
        /// <remarks>
        /// * A timed "sprinkle" visual effect was added.
        /// * yOffset now uses the public NPC.yOffset field, rather than Ghost's private field of the same name.
        /// * Lightsource generation was disabled. Note that lightsource removal is handled separately in the Ghost.takeDamage method.
        /// * Other Ghost private fields were removed or refactored due to being unused or redundant.
        /// * Redundant calls to bounding box methods were improved slightly.
        /// </remarks>
        protected override void updateAnimation(GameTime time)
        {
            if (time.TotalGameTime.TotalMilliseconds - timeOfLastSprinkle > sprinkleCooldown) //if this monster should use another sprinkle effect
            {
                timeOfLastSprinkle = time.TotalGameTime.TotalMilliseconds + Game1.random.Next(-500, 501); //record the current time, randomly offset by up to 0.5 seconds
                Color color = (Game1.random.NextDouble() < 0.5 ? EffectColor : Color.White); //randomly choose between the monster's effect color or white
                Utility.addSprinklesToLocation(this.currentLocation, (int)this.Tile.X, (int)this.Tile.Y, 2, 2, 101, 50, color, (string)null, false); //display a new sprinkle effect
            }

            this.yOffset = (int)(Math.Sin((double)((float)time.TotalGameTime.Milliseconds / 1000f) * (Math.PI * 2.0)) * 20.0); //the unused yOffsetExtra field
            //remove lightsource generation process
            Point monsterPixel = base.StandingPixel;
            Point standingPixel = base.Player.StandingPixel;
            float xSlope = -(standingPixel.X - monsterPixel.X);
            float ySlope = standingPixel.Y - monsterPixel.Y;
            float t = 400f;
            xSlope /= t;
            ySlope /= t;
            //remove "if Ghost.wasHitCounter <= 0" condition, which was always true
            float targetRotation = (float)Math.Atan2(0f - ySlope, xSlope) - (float)Math.PI / 2f; //declare targetRotation locally, which is only used in this method anyway
            if ((double)(Math.Abs(targetRotation) - Math.Abs(base.rotation)) > Math.PI * 7.0 / 8.0 && Game1.random.NextBool())
            {
                this.turningRight = true;
            }
            else if ((double)(Math.Abs(targetRotation) - Math.Abs(base.rotation)) < Math.PI / 8.0)
            {
                this.turningRight = false;
            }
            if (this.turningRight)
            {
                base.rotation -= (float)Math.Sign(targetRotation - base.rotation) * ((float)Math.PI / 64f);
            }
            else
            {
                base.rotation += (float)Math.Sign(targetRotation - base.rotation) * ((float)Math.PI / 64f);
            }
            base.rotation %= (float)Math.PI * 2f;
            //remove wasHitCounter value change
            float maxAccel = Math.Min(4f, Math.Max(1f, 5f - t / 64f / 2f));
            xSlope = (float)Math.Cos((double)base.rotation + Math.PI / 2.0);
            ySlope = 0f - (float)Math.Sin((double)base.rotation + Math.PI / 2.0);
            base.xVelocity += (0f - xSlope) * maxAccel / 6f + (float)Game1.random.Next(-10, 10) / 100f;
            base.yVelocity += (0f - ySlope) * maxAccel / 6f + (float)Game1.random.Next(-10, 10) / 100f;
            if (Math.Abs(base.xVelocity) > Math.Abs((0f - xSlope) * 5f))
            {
                base.xVelocity -= (0f - xSlope) * maxAccel / 6f;
            }
            if (Math.Abs(base.yVelocity) > Math.Abs((0f - ySlope) * 5f))
            {
                base.yVelocity -= (0f - ySlope) * maxAccel / 6f;
            }
            base.faceGeneralDirection(base.Player.getStandingPosition(), 0, opposite: false, useTileCalculations: false);
            base.resetAnimationSpeed();
        }

        /// <summary>This override reworks miscellaneous behaviors within the Ghost version.</summary>
        /// <remarks>
        /// * The "sprinkles" visual effect's color was changed.
        /// * Lightsource removal code was removed because this monster no longer generates lightsources.
        /// </remarks>
        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            int actualDamage = Math.Max(1, damage - this.resilience.Value);
            this.Slipperiness = 8;
            Color color = (Game1.random.NextDouble() < 0.5 ? EffectColor : Color.White); //randomly choose between the monster's effect color or white
            Utility.addSprinklesToLocation(this.currentLocation, this.TilePoint.X, this.TilePoint.Y, 2, 2, 101, 50, color); //use the selected color
            if (Game1.random.NextDouble() < this.missChance.Value - this.missChance.Value * addedPrecision)
            {
                actualDamage = -1;
            }
            else
            {
                this.Health -= actualDamage;
                if (this.Health <= 0)
                    this.deathAnimation();
                this.setTrajectory(xTrajectory, yTrajectory);
            }
            this.addedSpeed = -1;
            //remove lightsource removal code
            return actualDamage;
        }

        /// <summary>This override adds a "damage" check for debuffs and stamina drain when this monster touches the player.</summary>
        /// <remarks>In multiplayer, this method seems to be executed by the local player, i.e. whichever player was touched.
        /// The stamina drain and debuff effects are only effective on the local player, so this particular method was necessary; some others are only executed by the host.</remarks>
        public override void collisionWithFarmerBehavior()
        {
            if (!Context.IsMultiplayer || Game1.player.UniqueMultiplayerID == Player.UniqueMultiplayerID) //if this is single-player OR the monster is targeting the local player
            {
                GameTime time = Game1.currentGameTime; //get the local player's current time

                if (time?.TotalGameTime.TotalMilliseconds - timeOfLastPlayerHarm > playerHarmCooldown) //if this monster should try to harm the player again
                {
                    if (GetBoundingBox().Intersects(Game1.player.GetBoundingBox())) //if the monster is actually touching the player
                    {
                        timeOfLastPlayerHarm = time.TotalGameTime.TotalMilliseconds; //update the "last harm attempt" time

                        //if the player isn't invincible, isn't wearing the Slime Charmer ring, and doesn't resist the effect (mimicking the immunity check from GreenSlime)
                        if (!Game1.player.temporarilyInvincible && !Game1.player.isWearingRing("520") && Game1.random.Next(10) > Player.Immunity && !Player.hasBuff("28") && !Player.hasTrinketWithID("BasiliskPaw"))
                        {
                            Game1.player.Stamina = Math.Max(10, Game1.player.Stamina - EnergyDamage); //reduce the player's energy by this monster's energy damage (but not below 10)

                            if (EnableDebuff) //if this monster's debuff is enabled
                            {
                                Buff debuff = new Buff("13"); //create a new buff effect based on the "Slimed" debuff
                                debuff.glow = EffectColor; //use this monster's effect color
                                debuff.effects.Speed.Value = -2f; //drain less speed (original value: -4)
                                Player.applyBuff(debuff);
                            }
                        }
                    }
                }
            }

            base.collisionWithFarmerBehavior(); //call the base method, if any (this method is usually empty)
        }
    }
}
