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
using StardewValley.Network;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{
    public class BigBat : StardewValley.Monsters.Bat
    {

        public NetBool posturing = new NetBool(false);

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public int hatIndex;

        public Dictionary<int, Vector2> hatOffsets;

        public bool loadedout;

        public BigBat() { }

        public BigBat(Vector2 vector, int combatModifier)
            : base(vector * 64, 200)
        {

            moveTowardPlayerThreshold.Value = 99;

            Health = combatModifier * 100;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(15, Math.Max(30, combatModifier * 2));

            objectsToDrop.Clear();

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddFields(new INetSerializable[1]
            {
                 posturing,
            });
        }

        public void LoadOut()
        {

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            hatIndex = 8;

            hatSourceRect = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex, 20, 20);

            hatOffsets = new()
            {
                [0] = new Vector2(24f, 32f),
                [1] = new Vector2(24f, 28f),
                [2] = new Vector2(24f, 24f),
                [3] = new Vector2(24f, 28f),
            };

            loadedout = true;

        }

        public override List<Item> getExtraDropItems()
        {
            List<Item> list = new List<Item>();

            return list;

        }

        public override void behaviorAtGameTick(GameTime time)
        {

            if (posturing.Value) { return; }

            base.behaviorAtGameTick(time);

        }

        public override void defaultMovementBehavior(GameTime time)
        {

            if (posturing.Value) { return; }

            base.defaultMovementBehavior(time);

        }

        protected override void localDeathAnimation()
        {
            ModUtility.AnimateDeathSpray(currentLocation, Position, Color.MediumPurple);
        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (posturing) { return 0; }

            int ouchIndex = Game1.random.Next(15);

            List<string> ouchList = new()
            {
                "flap flap",
                "flippity",
                "cheeep"
            };

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 3000);

            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void drawAboveAllLayers(SpriteBatch b)
        {
            if (!Utility.isOnScreen(Position, 128))
            {
                return;
            }

            b.Draw(
                Sprite.Texture,
                getLocalPosition(Game1.viewport) + new Vector2(32f, 32f),
                Sprite.SourceRect,
                shakeTimer > 0 ? Color.Red : Color.White,
                0f,
                new Vector2(8f, 16f),
                Math.Max(0.2f, scale) * 6f,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                Math.Max(0f, drawOnTop ? 0.991f : getStandingY() / 10000f)
            );

            b.Draw(
                Game1.shadowTexture,
                getLocalPosition(Game1.viewport) + new Vector2(32f, 64f),
                Game1.shadowTexture.Bounds,
                Color.White,
                0f,
                new Vector2(Game1.shadowTexture.Bounds.Center.X, Game1.shadowTexture.Bounds.Center.Y),
                6f,
                SpriteEffects.None,
                wildernessFarmMonster ? 0.0001f : (getStandingY() - 1) / 10000f
             );

            int frameOffset = Sprite.currentFrame % 4;

            hatOffset = hatOffsets[frameOffset];

            b.Draw(
                hatsTexture,
                getLocalPosition(Game1.viewport) + hatOffset,
                hatSourceRect,
                Color.White,
                0f,
                new Vector2(8f, 16f),
                4f,
                SpriteEffects.None,
                Math.Max(0f, drawOnTop ? 0.992f : getStandingY() / 10000f + 0.00005f)
             );

        }

        public override void update(GameTime time, GameLocation location)
        {
            if (!loadedout) { LoadOut(); }
            base.update(time, location);
        }

    }

}
