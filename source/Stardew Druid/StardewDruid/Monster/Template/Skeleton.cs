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
using StardewValley.Objects;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster.Template
{
    public class Skeleton : StardewValley.Monsters.Skeleton
    {

        public bool loadedout;

        public int tickCount;

        public Texture2D hatsTexture;

        public Rectangle hatSourceRect;

        public Vector2 hatOffset;

        public int hatIndex;

        public Dictionary<int, Rectangle> hatSourceRects;

        public Dictionary<int, Vector2> hatOffsets;

        public float hatSize;

        public Texture2D swordsTexture;

        public Rectangle swordSourceRect;

        public bool swordFlip;

        public Vector2 swordOffset;

        public int swordIndex;

        public float swordRotate;

        public Dictionary<int, Vector2> swordOffsets;

        public Dictionary<int, bool> swordFlips;

        public Dictionary<int, float> swordRotates;

        public Dictionary<int, float> swordDepths;

        public float swordDepth;

        public bool partyHats;

        public Skeleton() { }

        public Skeleton(Vector2 vector, int combatModifier)
            : base(vector * 64, false)
        {

            focusedOnFarmers = true;

            Health = combatModifier * 25;

            MaxHealth = Health;

            DamageToFarmer = Math.Min(10, Math.Max(20, combatModifier));

            // ---------------------------------

            IsWalkingTowardPlayer = true;

            moveTowardPlayerThreshold.Value = 16;

            Slipperiness = 3;

            HideShadow = false;

            jitteriness.Value = 0.0;

            objectsToDrop.Clear();

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add(378);
            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                objectsToDrop.Add(380);
            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                objectsToDrop.Add(384);
            }
            else if (Game1.random.Next(6) == 0 && combatModifier >= 360)
            {
                objectsToDrop.Add(386); // iridium
            }

        }

        public void LoadOut()
        {

            partyHats = Mod.instance.PartyHats();

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            List<int> hatList = new()
            {
                242,
                292,
                //3,
                //299,
            };

            if (partyHats)
            {

                hatList.Add(3);
                hatList.Add(149);

            }

            hatSize = 3.5f;

            hatIndex = hatList[Game1.random.Next(hatList.Count)];

            //if (hatIndex == 299) { hatSize = 4f; }

            hatSourceRects = new()
            {
                [2] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex, 20, 20),       // down
                [1] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 12, 20, 20),  // right
                [3] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 24, 20, 20),  // left
                [0] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 36, 20, 20),  // up
            };

            hatOffsets = new()
            {
                [2] = new Vector2(-2, 0),
                [1] = new Vector2(0, -4),
                [3] = new Vector2(-4, -4),
                [0] = new Vector2(2, 0),
            };

            swordsTexture = Game1.content.Load<Texture2D>("TileSheets\\weapons");

            List<int> swordList = new()
            {
                0,
                12,
                49,
            };

            swordIndex = swordList[Game1.random.Next(swordList.Count)];

            swordSourceRect = new(swordIndex % 8 * 16, (swordIndex - swordIndex % 8) / 8 * 16, 16, 16);

            swordOffsets = new()
            {
                [2] = new Vector2(20, 0), // down
                [1] = new Vector2(-52, 4), // right
                [3] = new Vector2(0, -20), // left
                [0] = new Vector2(-42, 24), // up
            };

            swordFlips = new()
            {
                [2] = false,
                [1] = true,
                [3] = false,
                [0] = true,
            };

            swordRotates = new()
            {
                [2] = 2.4f,//2.4f,//4.0f,
                [1] = 4.8f,//1.6f,//4.8f,
                [3] = 1.6f,//0f,
                [0] = 4.0f,//0.8f,
            };

            loadedout = true;

        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);

            if (!IsInvisible && Utility.isOnScreen(Position, 128))
            {
                int spriteDirection = getFacingDirection();

                if (Sprite.currentFrame >= 20)
                {
                    spriteDirection = 2;

                }

                // ----------------- hats

                hatSourceRect = hatSourceRects[spriteDirection];

                hatOffset = hatOffsets[spriteDirection];

                switch (Sprite.currentFrame % 4)
                {

                    case 1:

                        hatOffset += new Vector2(0, 3); // down

                        break;

                    case 3:

                        hatOffset += new Vector2(0, 3); // down

                        break;

                    default: break;

                }

                Vector2 localPosition = getLocalPosition(Game1.viewport) + new Vector2(56f, 16 + yJumpOffset);

                float depth = GetBoundingBox().Center.Y / 10000f + 0.00005f;

                b.Draw(
                    hatsTexture,
                    localPosition + hatOffset,
                    hatSourceRect,
                    Color.White,
                    0,
                    new Vector2(16f, 29f),
                    hatSize,
                    flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    depth
                 );

                // ------------------ swords

                swordOffset = swordOffsets[spriteDirection];

                swordFlip = swordFlips[spriteDirection];

                swordRotate = swordRotates[spriteDirection];

                swordDepths = new()
                {
                    [2] = depth - 0.0001f,
                    [1] = depth - 0.0001f,
                    [3] = depth,
                    [0] = depth,
                };

                swordDepth = swordDepths[spriteDirection];

                b.Draw(
                    swordsTexture,
                    localPosition + swordOffset,
                    swordSourceRect,
                    Color.White,
                    swordRotate,
                    new(0, 0),
                    2.25f,
                    swordFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    swordDepth
                 );

            }

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            DialogueData.DisplayText(this, 3);

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override List<Item> getExtraDropItems()
        {
            return new List<Item>();
        }

        public void triggerPanic()
        {

            DialogueData.DisplayText(this, 3, 2);

        }

        public override void update(GameTime time, GameLocation location)
        {
            if (!loadedout) { LoadOut(); }
            base.update(time, location);
        }
    }

}
