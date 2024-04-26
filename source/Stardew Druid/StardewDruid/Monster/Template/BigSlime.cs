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
using StardewDruid.Data;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{
    public class BigSlime : StardewValley.Monsters.BigSlime
    {

        public NetBool posturing = new NetBool(false);

        public bool loadedout;

        public bool dropHat;

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public Dictionary<int, Vector2> hatOffsets;

        public Microsoft.Xna.Framework.Color slimeColor;

        public BigSlime()
        {

        }

        public BigSlime(Vector2 position, int combatModifier)
            : base(position*64, combatModifier * 10)
        {

            moveTowardPlayerThreshold.Value = 99;

            Health = combatModifier * 150;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(20, Math.Max(40, combatModifier * 2));

            IsWalkingTowardPlayer = true;

            speed = speed * 2;

        }

        protected override void initNetFields()
        {
            base.initNetFields();

            NetFields.AddField(posturing,"posturing");
        }

        public virtual void LoadOut()
        {

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            hatSourceRect = Game1.getSourceRectForStandardTileSheet(hatsTexture, 192, 20, 20);

            dropHat = true;

            loadedout = true;

            slimeColor = Color.Orange * 0.7f;

        }

        public override List<Item> getExtraDropItems()
        {
            List<Item> list = new List<Item>();

            return list;

        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 vector = Position;

            return new Rectangle((int)vector.X - 16, (int)vector.Y, 96, 64);
        }

        public override void defaultMovementBehavior(GameTime time)
        {

            if (posturing.Value)
            {
                return;
            }

            base.defaultMovementBehavior(time);

        }

        public override void behaviorAtGameTick(GameTime time)
        {
            if (posturing.Value)
            {
                return;
            }

            base.behaviorAtGameTick(time);

        }

        public override void updateMovement(GameLocation location, GameTime time)
        {

            if (posturing.Value)
            {
                return;
            }

            base.updateMovement(location, time);

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        { 

            if (posturing.Value)
            {
                return 0;
            }

            DialogueData.DisplayText(this, 3);

            Slipperiness = 3;

            Health -= damage;

            setTrajectory(xTrajectory, yTrajectory);

            currentLocation.playSound("hitEnemy");

            IsWalkingTowardPlayer = true;

            if (Health <= 0)
            {
                deathAnimation();

            }

            return damage;

        }

        public override void draw(SpriteBatch b)
        {
            //base.draw(b);

            // ----------------- hats

            if (!IsInvisible && Utility.isOnScreen(Position, 128))
            {

                b.Draw(
                    Sprite.Texture,
                    getLocalPosition(Game1.viewport) + new Vector2(56f, 16 + yJumpOffset),
                    Sprite.SourceRect,
                    slimeColor,
                    rotation,
                    new Vector2(16f, 16f),
                    6f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.991f : Tile.Y / 10000f)
                );

                hatOffsets = new()
                {
                    [0] = new Vector2(0, 6),
                    [1] = new Vector2(0, 0),
                    [2] = new Vector2(0, -6),
                    [3] = new Vector2(0, -12),
                    [4] = new Vector2(0, -18),
                    [5] = new Vector2(0, -12),
                    [6] = new Vector2(0, -6),
                    [7] = new Vector2(0, 0),
                };

                hatOffset = hatOffsets[Sprite.currentFrame];

                Vector2 localPosition = getLocalPosition(Game1.viewport) + new Vector2(56f, 16 + yJumpOffset);

                b.Draw(
                    hatsTexture,
                    localPosition + hatOffset,
                    hatSourceRect,
                    Color.White * 0.90f,
                    0f,
                    //new Vector2(10f, 16f),
                    new Vector2(10f, 9f),
                    6f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    Math.Max(0f, drawOnTop ? 0.990f : Tile.Y / 10000f - 0.00005f)
                );

            }

        }

        public override void update(GameTime time, GameLocation location)
        {
            if (!loadedout) { LoadOut(); }
            base.update(time, location);
        }

        public override void onDealContactDamage(Farmer who)
        {

            if ((who.health + who.buffs.Defense) - DamageToFarmer < 10)
            {

                who.health = (DamageToFarmer - who.buffs.Defense) + 10;

                Mod.instance.CriticalCondition();

            }

        }

    }

}
