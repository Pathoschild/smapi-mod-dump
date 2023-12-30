/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Map;
using StardewModdingAPI;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster
{

    public class Solaris : StardewValley.Monsters.Monster
    {

        public Texture2D monsterTexture;

        public bool loadedOut;

        public List<string> ouchList;

        public bool spawnBuff;

        public int spawnDamage;

        public double spawnTimeout;

        public const float rotationIncrement = MathF.PI / 64f;

        private int wasHitCounter;

        private float targetRotation;

        private bool turningRight;

        public Color SolardColor;

        public int spawnType;

        public string spawnName;

        public float spawnVelocity;

        public Solaris() { }

        public Solaris(Vector2 vector, int combatModifier, int type = 1)
            : base("Fly", vector * 64)
        {

            lastCrossroad = new Microsoft.Xna.Framework.Rectangle((int)position.X, (int)position.Y + 64, 64, 64);

            focusedOnFarmers = false;

            faceDirection(0);

            Position = vector * 64;

            DefaultPosition = vector * 64;

            defaultFacingDirection = 0;

            DamageToFarmer = 0;

            spawnType = type;

            spawnBuff = true;

            spawnTimeout = Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 600;

            objectsToDrop.Clear();

            ouchList = new()
            {
                "seep", "SEEP"
            };

            wasHitCounter = 500;

            Slipperiness = 24 + Game1.random.Next(-10, 10);

            IsWalkingTowardPlayer = true;

            HideShadow = true;

            spawnName = "Solaris";

            // ==============================================================
            // Type Differences
            // ==============================================================

            if (type % 2 == 0)
            {

                spawnName = "Voidle";

                objectsToDrop.Add(769);

                if (Game1.random.Next(3) == 0)
                {
                    objectsToDrop.Add(769);
                }

            }
            else
            {

                objectsToDrop.Add(768);

                if (Game1.random.Next(3) == 0)
                {
                    objectsToDrop.Add(768);
                }

            }

            switch (type)
            {

                case 5:
                case 6:

                    spawnName += "Prime";

                    moveTowardPlayerThreshold.Value = 12;

                    Health = combatModifier * 5;

                    Scale = 1.25f;

                    spawnDamage = (int)Math.Max(2, combatModifier * 0.1);

                    spawnVelocity = 0f;

                    break;

                case 3:
                case 4:

                    moveTowardPlayerThreshold.Value = 12;

                    Health = combatModifier;

                    Scale = 0.75f;

                    spawnDamage = (int)Math.Max(2, combatModifier * 0.05);

                    spawnVelocity = 1f;

                    break;

                default:

                    spawnName += "Zero";

                    moveTowardPlayerThreshold.Value = 6;

                    Health = (int)(combatModifier * 0.125);

                    Scale = 1f;

                    spawnDamage = (int)Math.Max(2, combatModifier * 0.01);

                    spawnVelocity = 2f;

                    break;

            }

            MaxHealth = Health;

            Sprite = MonsterData.MonsterSprite(spawnName);

            LoadOut();

        }

        public void LoadOut()
        {

            monsterTexture = MonsterData.MonsterTexture(spawnName);

            loadedOut = true;

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (spawnBuff)
            {
                return 0;
            }

            int ouchIndex = Game1.random.Next(10);

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 2000);
            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void update(GameTime time, GameLocation location)
        {

            if (!Context.IsMainPlayer)
            {

                if (Sprite.loadedTexture == null || Sprite.loadedTexture.Length == 0)
                {

                    Sprite.spriteTexture = MonsterData.MonsterTexture(Name);

                    Sprite.loadedTexture = Sprite.textureName.Value;

                    LoadOut();

                }

            }

            if (spawnBuff)
            {
                if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds > spawnTimeout)
                {
                    spawnBuff = false;

                    DamageToFarmer = spawnDamage;
                }
            }

            base.update(time, location);

        }

        public override void reloadSprite()
        {
            Sprite = MonsterData.MonsterSprite(spawnName);

            HideShadow = true;
        }

        public override Microsoft.Xna.Framework.Rectangle GetBoundingBox()
        {

            float height = 16 * Scale;

            float width = 16 * Scale;

            Microsoft.Xna.Framework.Rectangle box = new((int)(Position.X + 32 - (width / 2)), (int)(Position.Y + 32 - (height / 2)), (int)width, (int)height);

            return box;

        }

        public override void drawAboveAllLayers(SpriteBatch b)
        {
            if (!Utility.isOnScreen(base.Position, 128))
            {

                return;

            }

            if (!loadedOut)
            {

                LoadOut();

            }

            //b.Draw(Sprite.Texture, getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2 - 32), Sprite.SourceRect, Color.White, 0f, new Vector2(8f, 16f), Math.Max(0.2f, scale) * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((getStandingY() + 8) / 10000f)));

            b.Draw(monsterTexture, getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2 - 32), Sprite.SourceRect, Color.White, 0f, new Vector2(8f, 16f), Math.Max(0.2f, scale) * 4f, flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None, Math.Max(0f, drawOnTop ? 0.991f : ((getStandingY() + 8) / 10000f)));

            b.Draw(Game1.shadowTexture, getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2), Game1.shadowTexture.Bounds, Color.White, 0f, new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y), 4f, SpriteEffects.None, (getStandingY() - 1) / 10000f);

        }

        public override void drawAboveAlwaysFrontLayer(SpriteBatch b)
        {
            if (base.currentLocation != null && currentLocation.treatAsOutdoors.Value)
            {
                drawAboveAllLayers(b);
            }
        }

        public override void shedChunks(int number)
        {

        }

        protected override void sharedDeathAnimation()
        {

        }

        protected override void localDeathAnimation()
        {

        }

        protected override void updateAnimation(GameTime time)
        {

            if (wasHitCounter >= 0)
            {
                wasHitCounter -= time.ElapsedGameTime.Milliseconds;
            }

            bool thresholdCheck = withinPlayerThreshold();

            int idleDirection = (xVelocity < 0) ? 2 : 0;

            Sprite.Animate(time, idleDirection, 4, 75f);

            Vector2 playerVector = Player.getTileLocation();

            Vector2 monsterVector = getTileLocation();

            moveTowardPlayerThreshold.Value = Math.Max(7, (int)(Vector2.Distance(playerVector, monsterVector) * 1.5));

            if ((thresholdCheck))
            {

                if (playerVector.X < monsterVector.X)
                {
                    faceDirection(3);

                }
                else
                {

                    faceDirection(1);

                }

                float num2 = -(base.Player.GetBoundingBox().Center.X - GetBoundingBox().Center.X);

                float num3 = base.Player.GetBoundingBox().Center.Y - GetBoundingBox().Center.Y;

                float num4 = Math.Max(1f, Math.Abs(num2) + Math.Abs(num3));

                if (num4 < 64f)
                {

                    xVelocity = Math.Max(-7f, Math.Min(7f, xVelocity * 1.1f));

                    yVelocity = Math.Max(-7f, Math.Min(7f, yVelocity * 1.1f));

                }

                num2 /= num4;

                num3 /= num4;

                if (wasHitCounter <= 0)
                {

                    targetRotation = (float)Math.Atan2(0f - num3, num2) - MathF.PI / 2f;

                    if ((double)(Math.Abs(targetRotation) - Math.Abs(rotation)) > Math.PI * 7.0 / 8.0 && Game1.random.NextDouble() < 0.5)
                    {
                        turningRight = true;
                    }
                    else if ((double)(Math.Abs(targetRotation) - Math.Abs(rotation)) < Math.PI / 8.0)
                    {
                        turningRight = false;
                    }

                    if (turningRight)
                    {
                        rotation -= Math.Sign(targetRotation - rotation) * (MathF.PI / 64f);
                    }
                    else
                    {
                        rotation += Math.Sign(targetRotation - rotation) * (MathF.PI / 64f);
                    }

                    rotation %= MathF.PI * 2f;

                    wasHitCounter = 5 + Game1.random.Next(-1, 2);

                }

                float num5 = Math.Min(4f + spawnVelocity, Math.Max(2f, 4f + spawnVelocity - num4 / 64f / 2f));

                num2 = (float)Math.Cos(rotation + Math.PI / 2.0);

                num3 = 0f - (float)Math.Sin(rotation + Math.PI / 2.0);

                xVelocity += (0f - num2) * num5 / 6f + Game1.random.Next(-10, 10) / 100f;

                yVelocity += (0f - num3) * num5 / 6f + Game1.random.Next(-10, 10) / 100f;

                if (Math.Abs(xVelocity) > Math.Abs((0f - num2) * 7f))
                {

                    xVelocity -= (0f - num2) * num5 / 6f;

                }

                if (Math.Abs(yVelocity) > Math.Abs((0f - num3) * 7f))
                {

                    yVelocity -= (0f - num3) * num5 / 6f;

                }

            }

            resetAnimationSpeed();

        }

        public override void behaviorAtGameTick(GameTime time)
        {
            base.behaviorAtGameTick(time);

            if (double.IsNaN(xVelocity) || double.IsNaN(yVelocity))
            {
                base.Health = -500;
            }

            if (base.Position.X <= -640f || base.Position.Y <= -640f || base.Position.X >= base.currentLocation.Map.Layers[0].LayerWidth * 64 + 640 || base.Position.Y >= base.currentLocation.Map.Layers[0].LayerHeight * 64 + 640)
            {
                base.Health = -500;
            }

        }

    }

}
