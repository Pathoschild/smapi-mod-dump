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
using StardewValley.Characters;
using StardewValley.Monsters;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Monster
{
    public class Skeleton : StardewValley.Monsters.Skeleton
    {

        public List<string> ouchList;

        public List<string> dialogueList;

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

        public int spawnBuff;

        public int spawnDamage;

        public Skeleton(Vector2 vector, int combatModifier, bool hats)
            : base(vector*64, false)
        {

            focusedOnFarmers = true;

            Health = combatModifier;

            MaxHealth = Health;

            DamageToFarmer = 0;

            spawnDamage = (int)Math.Max(2, combatModifier * 0.075);

            spawnBuff = 60;

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

            ouchList = new()
            {
                "deep",
                "yeoww",
                "crikey!",
            };

            dialogueList = new()
            {
                "DEEP",
                "shivers",
                "timbers",
                "yarr",
            };

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            List<int> hatList = new()
            {
                242,
                292,
                //3,
                //299,
            };

            if (hats)
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

            partyHats = hats;

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

                float depth = (float)GetBoundingBox().Center.Y / 10000f + 0.00005f;

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
                    new(0,0),
                    2.25f,
                    swordFlip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    swordDepth
                 );

            }

        }

        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (spawnBuff > 0)
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

        public override List<Item> getExtraDropItems()
        {
            return new List<Item>();
        }

        public void triggerPanic()
        {

            List<string> panicList = new()
            {
                "cover!",
                "RUN",
                "oh no!",
            };

            showTextAboveHead(panicList[Game1.random.Next(3)], duration: 3000);

        }

        public override void behaviorAtGameTick(GameTime time)
        {

            if (spawnBuff > 0)
            {

                spawnBuff--;

                if (spawnBuff < 1)
                {

                    DamageToFarmer = spawnDamage;

                }

            }

            tickCount++;

            if (tickCount >= 200)
            {
                int dialogueIndex = Game1.random.Next(15);
                if (dialogueList.Count - 1 >= dialogueIndex)
                {
                    showTextAboveHead(dialogueList[dialogueIndex], duration: 2000);
                }
                tickCount = 0;
            }

            base.behaviorAtGameTick(time);

        }

    }

}
