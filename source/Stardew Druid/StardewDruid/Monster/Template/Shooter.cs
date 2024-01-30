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
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Projectiles;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{
    public class Shooter : StardewValley.Monsters.Shooter
    {

        public NetBool posturing = new NetBool(false)  ;

        public bool loadedout;

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public int hatIndex;

        public Dictionary<int, Rectangle> hatSourceRects;

        public Dictionary<int, Vector2> hatOffsets;

        public bool aimPlayer;

        public bool shootPlayer;

        public bool shiftPosition;

        public Shooter() { }

        public Shooter(Vector2 position, int combatModifier)
            : base(position * 64, "Shadow Sniper")
        {
            focusedOnFarmers = true;

            Health = combatModifier * 100;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(15, Math.Max(30, combatModifier));

            objectsToDrop.Clear();


        }

        public void LoadOut()
        {

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            hatIndex = 243;

            hatSourceRects = new()
            {
                [2] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex, 20, 20),       // down
                [1] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 12, 20, 20),  // right
                [3] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 24, 20, 20),  // left
                [0] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 36, 20, 20),  // up
            };

            loadedout = true;

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[1]
            {
                 posturing,
            });
        }

        public override void behaviorAtGameTick(GameTime time)
        {

            if (aimPlayer)
            {
                Halt();
                faceGeneralDirection(Player.getStandingPosition());

                return;
            }
            else if (shootPlayer)
            {
                Vector2 vector = Vector2.Zero;
                float value = 0f;
                if ((int)facingDirection == 0)
                {
                    vector = new Vector2(0f, -1f);
                    value = 0f;
                }

                if ((int)facingDirection == 3)
                {
                    vector = new Vector2(-1f, 0f);
                    value = -MathF.PI / 2f;
                }

                if ((int)facingDirection == 1)
                {
                    vector = new Vector2(1f, 0f);
                    value = MathF.PI / 2f;
                }

                if ((int)facingDirection == 2)
                {
                    vector = new Vector2(0f, 1f);
                    value = MathF.PI;
                }

                vector *= projectileSpeed;

                Vector2 basePosition = Position - new Vector2(128f, 0);

                for (int i = 0; i < 3; i++)
                {

                    Vector2 projectilePosition = basePosition + new Vector2(128f * i, 0);

                    fireEvent.Fire();
                    currentLocation.playSound(fireSound);
                    BasicProjectile basicProjectile = new BasicProjectile(DamageToFarmer, firedProjectile, 0, 0, 0f, vector.X, vector.Y, projectilePosition, "", "", explode: false, damagesMonsters: false, currentLocation, this);
                    basicProjectile.startingRotation.Value = value;
                    basicProjectile.height.Value = 24f;
                    basicProjectile.debuff.Value = projectileDebuff;
                    basicProjectile.ignoreTravelGracePeriod.Value = true;
                    basicProjectile.IgnoreLocationCollision = true;
                    basicProjectile.maxTravelDistance.Value = 128 * projectileRange;
                    currentLocation.projectiles.Add(basicProjectile);

                }

                shootPlayer = false;

                return;
            }
            else if (posturing.Value)
            {
                return;
            }

            base.behaviorAtGameTick(time);

        }
        public override List<Item> getExtraDropItems()
        {
            List<Item> list = new List<Item>();

            return list;

        }
        public override void defaultMovementBehavior(GameTime time)
        {

            if (posturing.Value && !shiftPosition) { return; }

            base.defaultMovementBehavior(time);

        }

        public override void updateMovement(GameLocation location, GameTime time)
        {

            if (posturing.Value && !shiftPosition) { return; }

            if (shooting.Value)
            {
                MovePosition(time, Game1.viewport, location);
            }
            else
            {
                base.updateMovement(location, time);
            }

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (posturing.Value) { return 0; }

            List<string> ouchList = new()
            {
                "ooft",
                "a worthy opponent",
                "deep deep!"
            };

            int ouchIndex = Game1.random.Next(15);

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 3000);

            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            // ----------------- hats

            if (!IsInvisible && Utility.isOnScreen(Position, 128))
            {

                int spriteDirection;

                hatOffsets = new()
                {
                    [2] = new Vector2(-2, 2),
                    [1] = new Vector2(-6, 8),
                    [3] = new Vector2(-2, 2),
                    [0] = new Vector2(-2, 0),
                };

                if (shooting.Value)
                {
                    switch (Sprite.currentFrame % 4)
                    {

                        case 1:

                            spriteDirection = 1;
                            break;

                        case 2:

                            spriteDirection = 0;
                            break;

                        case 3:

                            spriteDirection = 3;
                            break;

                        default:

                            spriteDirection = 2;
                            break;

                    }

                    hatSourceRect = hatSourceRects[spriteDirection];

                    hatOffset = hatOffsets[spriteDirection];

                    hatOffset += new Vector2(0, 12);

                }
                else
                {

                    spriteDirection = getFacingDirection();

                    hatSourceRect = hatSourceRects[spriteDirection];

                    hatOffset = hatOffsets[spriteDirection];

                }

                Vector2 localPosition = getLocalPosition(Game1.viewport) + new Vector2(56f, 16 + yJumpOffset);

                float depth = GetBoundingBox().Center.Y / 10000f + 0.00005f;

                b.Draw(
                    hatsTexture,
                    localPosition + hatOffset,
                    hatSourceRect,
                    Color.White,
                    //hatRotate,
                    0f,
                    new Vector2(14f, 23f),
                    5f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    depth
                 );

            }

        }

        public override void update(GameTime time, GameLocation location)
        {
            if (!loadedout) { LoadOut(); }
            base.update(time, location);
        }

    }

}
