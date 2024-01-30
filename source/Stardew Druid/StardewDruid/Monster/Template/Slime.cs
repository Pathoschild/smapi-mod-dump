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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Threading;

namespace StardewDruid.Monster.Template
{
    public class Slime : GreenSlime
    {

        public bool loadedout;

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public int hatIndex;

        public Dictionary<int, Vector2> hatOffsets;

        public bool partyHats;

        public Slime() { }

        public Slime(Vector2 position, int combatModifier)
            : base(position * 64, combatModifier * 10)
        {

            focusedOnFarmers = true;

            Health = combatModifier * 25;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(10, Math.Max(20, combatModifier));

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


        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {
            
            int ouchIndex = Game1.random.Next(10);

            List<string> ouchList = new()
            {
                "blup blup",
                "bloop",
            };

            if (ouchIndex < ouchList.Count)
            {
                showTextAboveHead(ouchList[ouchIndex], duration: 2000);
            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public void LoadOut()
        {

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            int random = new Random().Next(300);

            int mineLevel = random;

            if (random > 160)
            {

                mineLevel = Health;

            }

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

            partyHats = Mod.instance.PartyHats();

            loadedout = true;

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            // ----------------- hats

            if (!IsInvisible && Utility.isOnScreen(Position, 128) && partyHats)
            {

                if (Sprite.currentFrame >= 16)
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
                    vector = new Vector2((float)Math.Sin(randomStackOffset + Game1.currentGameTime.TotalGameTime.TotalSeconds * Math.PI * 2.0 + (stackedSlimes.Value - 1) * 30) * 8f, -30 * (stackedSlimes.Value - 1));
                }

                Vector2 localPosition = getLocalPosition(Game1.viewport) + new Vector2(32f, GetBoundingBox().Height / 2 + yOffset) + vector;

                float depth = Math.Max(0f, drawOnTop ? 0.992f : getStandingY() * 2 / 10000f + 0.00005f);

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
            if (!loadedout) { LoadOut(); }
            base.update(time, location);
        }
    }

}
