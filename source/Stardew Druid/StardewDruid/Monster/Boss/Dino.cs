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
using StardewDruid.Event;
using StardewDruid.Map;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Monsters;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;


namespace StardewDruid.Monster.Boss
{
    public class Dino : Boss
    {


        public Texture2D hatsTexture;
        public Rectangle hatSourceRect;
        public Vector2 hatOffset;
        public int hatIndex;
        public Dictionary<int, Rectangle> hatSourceRects;
        public Dictionary<int, Vector2> hatOffsets;
        public Dictionary<int, float> hatRotates;
        public Dictionary<int, Vector2> hatRotateOffsets;

        public Dino()
        {
        }

        public Dino(Vector2 vector, int CombatModifier)
          : base(vector, CombatModifier, "Dinosaur")
        {

        }
        public override void BaseMode()
        {

            MaxHealth = Math.Max(4000, combatModifier * 300);

            Health = MaxHealth;

            DamageToFarmer = Math.Max(15, Math.Min(50, combatModifier * 2));

        }

        public override void LoadOut()
        {

            BaseWalk();
            DinoFlight();
            DinoSpecial();

            overHead = new(16, -144);

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            hatIndex = 345;

            hatSourceRects = new Dictionary<int, Rectangle>()
            {
                [2] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex, 20, 20),
                [1] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 12, 20, 20),
                [3] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 24, 20, 20),
                [0] = Game1.getSourceRectForStandardTileSheet(hatsTexture, hatIndex + 36, 20, 20)
            };

            hatOffsets = new Dictionary<int, Vector2>()
            {
                [2] = new Vector2(-16f, 0.0f),
                [1] = new Vector2(36f, 2f),
                [3] = new Vector2(-68f, 4f),
                [0] = new Vector2(-16f, -32f)
            };

            hatRotates = new Dictionary<int, float>()
            {
                [1] = 6f,
                [3] = 0.4f
            };

            hatRotateOffsets = new Dictionary<int, Vector2>()
            {
                [1] = new Vector2(-4f, -2f),
                [3] = new Vector2(8f, -12f)
            };

            loadedOut = true;

        }

        public void DinoFlight()
        {

            flightSpeed = 12;

            flightHeight = 2;

            flightCeiling = 1;

            flightFloor = 1;

            flightLast = 1;

            flightInterval = 9;

            flightTexture = characterTexture;

            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 192, 32, 32),
                    new Rectangle(32, 192, 32, 32),
                },

                [1] = new List<Rectangle>()
                {
                    new Rectangle(0, 160, 32, 32),
                    new Rectangle(32, 160, 32, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 32, 32),
                    new Rectangle(32, 128, 32, 32),

                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(0, 224, 32, 32),
                    new Rectangle(32, 224, 32, 32),
                }
            };

        }

        public void DinoSpecial()
        {

            abilities = 2;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            specialCeiling = 3;

            specialFloor = 1;

            specialInterval = 15;

            cooldownInterval = 60;

            cooldownTimer = cooldownInterval;

            specialTexture = characterTexture;

            specialFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 192, 32, 32),
                    new Rectangle(32, 192, 32, 32),
                },

                [1] = new List<Rectangle>()
                {
                    new Rectangle(0, 160, 32, 32),
                    new Rectangle(32, 160, 32, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 32, 32),
                    new Rectangle(32, 128, 32, 32),

                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(0, 224, 32, 32),
                    new Rectangle(32, 224, 32, 32),
                }
            };

            sweepSet = false;

            sweepInterval = 12;

            sweepTexture = characterTexture;

            sweepFrames = walkFrames;
        }


        public override void HardMode()
        {

            Health *= 3;

            Health /= 2;

            MaxHealth = Health;

            tempermentActive = temperment.aggressive;

        }

        public override Rectangle GetBoundingBox()
        {

            Vector2 position = Position;
            return new Rectangle((int)position.X - 48, (int)position.Y - 32, 160, 128);
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            if (IsEmoting && !Game1.eventUp)
            {
                Vector2 emotePosition = localPosition;
                emotePosition.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);
            }

            hatSourceRect = hatSourceRects[FacingDirection];

            hatOffset = hatOffsets[FacingDirection];

            float num1 = 0.0f;

            if (netFlightActive.Value)
            {

                b.Draw(characterTexture, localPosition + new Vector2(56, 16 - flightHeight), new Rectangle?(flightFrames[netDirection.Value][flightFrame]), Color.White * 0.65f, 0, new Vector2(16f, 16f), 7f, SpriteEffects.None, drawLayer);

            }
            else if (netSpecialActive.Value)
            {

                int specialUseFrame = specialFrame;

                if (specialFrame > 1)
                {
                    specialUseFrame = 1;
                }

                b.Draw(characterTexture, localPosition + new Vector2(56, 16), new Rectangle?(specialFrames[netDirection.Value][specialUseFrame]), Color.White * 0.65f, 0.0f, new Vector2(16f, 16f), 7f, SpriteEffects.None, drawLayer);

                if (hatRotates.ContainsKey(FacingDirection))
                {
                    num1 = hatRotates[FacingDirection];
                    hatOffset = hatOffset + hatRotateOffsets[FacingDirection];
                }
                else
                {
                    hatOffset = hatOffset - new Vector2(0.0f, 4f);
                }

            }
            else
            {

                b.Draw(characterTexture, localPosition + new Vector2(56, 16), new Rectangle?(walkFrames[netDirection.Value][walkFrame]), Color.White * 0.65f, 0.0f, new Vector2(16f, 16f), 7f, SpriteEffects.None, drawLayer);

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X, localPosition.Y + 32f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * 0.65f, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);


            if (FacingDirection % 2 == 0)
            {
                switch (Sprite.currentFrame % 4)
                {
                    case 1:
                        hatOffset = hatOffset + new Vector2(4f, 0.0f);
                        break;
                    case 3:
                        hatOffset = hatOffset - new Vector2(4f, 0.0f);
                        break;
                }
            }
            else
            {
                switch (Sprite.currentFrame % 4)
                {
                    case 1:
                        hatOffset = hatOffset + new Vector2(0.0f, 4f);
                        break;
                    case 3:
                        hatOffset = hatOffset + new Vector2(0.0f, 4f);
                        break;
                }
            }

            float num2 = 0.991f;

            if (FacingDirection == 0)
            {
                num2 = 0.989f;
            }

            Vector2 vector2 = getLocalPosition(Game1.viewport) + new Vector2(56f, (float)(16 + yJumpOffset) - flightHeight) + hatOffset;

            b.Draw(hatsTexture, vector2, new Rectangle?(hatSourceRect), Color.White * 0.6f, num1, new Vector2(8f, 12f), 8f, flip ? (SpriteEffects)1 : 0, num2);

        }

        public override void PerformSpecial(Vector2 farmerPosition)
        {

            specialTimer = (specialCeiling + 1) * specialInterval;

            netSpecialActive.Set(true);

            SpellHandle beam = new(currentLocation, farmerPosition, GetBoundingBox().Center.ToVector2(), 2, 0, DamageToFarmer * 0.4f);

            beam.type = SpellHandle.barrages.beam;

            beam.monster = this;

            Mod.instance.spellRegister.Add(beam);

        }

    }

}
