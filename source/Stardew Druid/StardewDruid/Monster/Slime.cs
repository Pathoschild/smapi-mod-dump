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
using StardewValley;
using StardewValley.Monsters;
using System.Collections.Generic;
using System;
using Microsoft.Xna.Framework.Graphics;
using System.Reflection.Metadata.Ecma335;

namespace StardewDruid.Monster
{
    public class Slime : GreenSlime
    {
        
        public List<string> ouchList;

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public int hatIndex;

        public Dictionary<int, Vector2> hatOffsets;

        public bool partyHats;

        public bool spawnBuff;

        public int spawnDamage;

        public double spawnTimeout;

        public Slime(Vector2 position, int combatModifier,bool hats)
            : base(position * 64, combatModifier / 2)
        {

            focusedOnFarmers = true;

            Health = (int)(combatModifier * 0.5);

            MaxHealth = Health;

            DamageToFarmer = 0;

            spawnDamage = (int)Math.Max(2, combatModifier * 0.075);

            spawnBuff = true;

            spawnTimeout = Game1.currentGameTime.TotalGameTime.TotalMilliseconds + 600;

            objectsToDrop.Clear();

            objectsToDrop.Add(766);

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add(766);
            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                objectsToDrop.Add(766);

            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                List<int> slimeSyrups = new()
                {
                    724,725,726,247,184,419,
                };

                objectsToDrop.Add(slimeSyrups[Game1.random.Next(slimeSyrups.Count)]);
            }

            ouchList = new()
            {
                "blup blup",
                "bloop",
            };

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            int mineLevel = combatModifier / 2;

            if (mineLevel < 40)
            {

                hatIndex = 203;

            }
            else if (mineLevel < 80)
            {

                hatIndex = 202;

            }
            else if (mineLevel > 120)
            {
                //hatIndex = 203;
                hatIndex = 147;

            }
            else
            {

                hatIndex = 201;

            }

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

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 2000);
            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            // ----------------- hats

            if (!IsInvisible && Utility.isOnScreen(Position, 128) && partyHats)
            {

                if(Sprite.currentFrame >= 16)
                {
                    hatOffsets = new()
                    {
                        [0] = new Vector2(0, 0),
                        [1] = new Vector2(0, 3),
                        [2] = new Vector2(0, 6),
                        [3] = new Vector2(0, 3),
                    };

                }
                else
                {
                    hatOffsets = new()
                    {
                        [0] = new Vector2(0, 0),
                        [1] = new Vector2(0, -2),
                        [2] = new Vector2(0, -4),
                        [3] = new Vector2(0, -2),
                    };

                }
 
                hatOffset = hatOffsets[Sprite.currentFrame % 4];

                Vector2 vector = Vector2.Zero;

                if (stackedSlimes.Value > 0)
                {
                    vector = new Vector2((float)Math.Sin((double)randomStackOffset + Game1.currentGameTime.TotalGameTime.TotalSeconds * Math.PI * 2.0 + (double)((stackedSlimes.Value-1) * 30)) * 8f, -30 * (stackedSlimes.Value-1));
                }

                Vector2 localPosition = getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2 + yOffset) + vector;

                float depth = Math.Max(0f, drawOnTop ? 0.992f : ((float)(getStandingY() * 2) / 10000f)+0.00005f);

                b.Draw(
                    hatsTexture,
                    localPosition + hatOffset,
                    hatSourceRect,
                    Color.White * 0.90f,
                    0f,
                    //new Vector2(9f, 13f),
                    new Vector2(10f, 11f),
                    4f,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    depth
                );
                
            }

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

            base.update(time, location);

        }

    }

}
