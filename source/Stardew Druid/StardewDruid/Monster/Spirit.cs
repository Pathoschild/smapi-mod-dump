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
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster
{
    public class Spirit : DustSpirit
    {

        public List<string> ouchList;

        public List<string> dialogueList;

        public int tickCount;

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public int hatIndex;

        public bool hatFlip;

        public bool partyHats;

        //public Dictionary<int, Rectangle> hatSourceRects;

        //public Dictionary<int, Vector2> hatOffsets;

        //public Dictionary<int, bool> hatFlips;

        public bool spawnBuff;

        public int spawnDamage;

        public double spawnTimeout;

        public Spirit(Vector2 position, int combatModifier, bool hats)
            : base(position * 64, true)
        {

            focusedOnFarmers = true;

            Health = (int)Math.Max(2, combatModifier * 0.25);

            MaxHealth = Health;

            DamageToFarmer = 0;

            spawnDamage = (int)Math.Max(2, combatModifier * 0.05);

            spawnBuff = true;

            spawnTimeout = Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 600;

            // ---------------------------------

            Slipperiness = 3;

            HideShadow = false;

            jitteriness.Value = 0.0;

            objectsToDrop.Clear();

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add(382); // coal

            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                objectsToDrop.Add(395); // coffee (edible)

            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                objectsToDrop.Add(251); // tea sapling
            }

            ouchList = new()
            {
                "ow ow",
                "ouchies",
            };

            dialogueList = new()
            {
                "meep meep?",
                "meep",
                "MEEEP",
            };

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            List<int> hatList = new()
            {
                103,
                104,
                //201,
                //202,
                //203,
            };

            hatIndex = hatList[Game1.random.Next(hatList.Count)];

            hatSourceRect = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex, 20, 20);

            partyHats = hats;

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            if (spawnBuff)
            {
                return 0;
            }

            int ouchIndex = Game1.random.Next(10);
            if (ouchList.Count - 1 >= ouchIndex)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 2000);
            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void update(GameTime time, GameLocation location)
        {

            if (spawnBuff)
            {
                if (Game1.currentGameTime.TotalGameTime.TotalMilliseconds > spawnTimeout)
                {
                    spawnBuff = false;

                    DamageToFarmer = spawnDamage;
                }
            }

            tickCount++;

            if (tickCount >= 200)
            {
                int dialogueIndex = Game1.random.Next(12);
                if (dialogueList.Count - 1 >= dialogueIndex)
                {
                    showTextAboveHead(dialogueList[dialogueIndex], duration: 2000);
                }
                tickCount = 0;
            }

            base.update(time, location);

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            // ----------------- hats

            if (!IsInvisible && Utility.isOnScreen(Position, 128) && partyHats)
            {

                Vector2 localPosition = getLocalPosition(Game1.viewport) + new Vector2(32 + ((shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), 64 + yJumpOffset);

                float depth = Math.Max(0f, drawOnTop ? 0.992f : (getStandingY() * 2 / 10000f) + 0.00005f);

                b.Draw(
                    hatsTexture,
                    localPosition,
                    hatSourceRect,
                    //Color.White * 0.65f,
                    Color.Blue * 0.75f,
                    0f,
                    //new Vector2(9f, 13f),
                    new Vector2(9f, 11f),
                    3f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    depth
                 );

            }

        }

    }

}
