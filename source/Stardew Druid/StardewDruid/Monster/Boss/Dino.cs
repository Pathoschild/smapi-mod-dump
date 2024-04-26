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
        public int hatIndex;
        public Dictionary<int, Rectangle> hatSourceRects;
        public Dictionary<int, List<Vector2>> hatOffsets;
        public Dictionary<int, List<float>> hatRotates;

        public Dino()
        {
        }

        public Dino(Vector2 vector, int CombatModifier)
          : base(vector, CombatModifier, "Dinosaur")
        {

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

            /*hatOffsets = new Dictionary<int, List<Vector2>>()
            {
                [2] = new() { new Vector2(-16f, 0.0f), new Vector2(-12f, 0.0f), new Vector2(-12f, 0.0f), },
                [1] = new() { new Vector2(36f, 2f), new Vector2(36f, 6f), new Vector2(32, 4), },
                [3] = new() { new Vector2(-68f, 4f), new Vector2(-68f, 8f), new Vector2(60, 20), },
                [0] = new() { new Vector2(-16f, -32f), new Vector2(-12f, -32f), new Vector2(-12f, -32f), },
            };*/

            hatRotates = new Dictionary<int, List<float>>()
            {
                [2] = new() { 0f,0f,0f, },
                [1] = new() { 0f, 0f, 6f, },
                [3] = new() { 0f, 0f, 0.4f, },
                [0] = new() { 0f, 0f, 0f, },
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

            sweepThreshold = 192;

            specialThreshold = 512;

            barrageThreshold = 640;

            specialCeiling = 3;

            specialFloor = 1;

            specialInterval = 15;

            cooldownInterval = 180;

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

            specialScheme = SpellHandle.schemes.ether;

        }

        public override Rectangle GetBoundingBox()
        {

            int netScale = netMode.Value > 5 ? netMode.Value = 1 : netMode.Value;

            return new Rectangle((int)Position.X - 13 - (14 * netScale), (int)Position.Y - 32 - flightHeight - (32 * netScale), 90 + (netScale * 28), 96 + (netScale * 32));

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }


            hatOffsets = new Dictionary<int, List<Vector2>>()
            {
                [2] = new() {
                    new Vector2(18,24),
                    new Vector2(24,32),
                    new Vector2(30,40),
                    new Vector2(36,48),
                    new Vector2(42,56),
                },
                [1] = new(){
                    new Vector2(36, 3f),
                    new Vector2(48, 4f),
                    new Vector2(60, 5f),
                    new Vector2(72, 6f),
                    new Vector2(84, 7f),
                },
                [0] = new(){
                    new Vector2(18,-9),
                    new Vector2(24,-12),
                    new Vector2(30,-15),
                    new Vector2(36,-18),
                    new Vector2(42,-21),
                },
                [3] = new(){
                    new Vector2(-6, 3f),
                    new Vector2(-8, 4f),
                    new Vector2(-10, 5f),
                    new Vector2(-12, 6f),
                    new Vector2(-14, 7f),
                },

            };

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            DrawEmote(b, localPosition, drawLayer);

            int netScale = netMode.Value > 5 ? netMode.Value = 1 : netMode.Value;

            Vector2 dinoPosition = new Vector2(localPosition.X - 16f - (16 * netScale), localPosition.Y - 32f - flightHeight - (32 * netScale));

            float dinoScale = 3f + netScale;

            int hatFrame = walkFrame % 2 == 0 ? 0 : 1;

            if (netFlightActive.Value)
            {

                b.Draw(
                    characterTexture,
                    dinoPosition, 
                    flightFrames[netDirection.Value][flightFrame], 
                    Color.White * 0.75f, 
                    0,
                    Vector2.Zero,
                    dinoScale,
                    SpriteEffects.None,
                    drawLayer
                );

                hatFrame = flightFrame % 2 == 0 ? 0 : 1;

            }
            else if (netSpecialActive.Value)
            {

                int specialUseFrame = specialFrame;

                if (specialFrame > 1)
                {
                    specialUseFrame = 1;
                    hatFrame = 2;
                }

                b.Draw(
                    characterTexture,
                    dinoPosition,
                    specialFrames[netDirection.Value][specialUseFrame],
                    Color.White * 0.75f,
                    0.0f,
                    Vector2.Zero,
                    dinoScale,
                    SpriteEffects.None,
                    drawLayer
                );

            }
            else
            {

                b.Draw(
                    characterTexture,
                    dinoPosition,
                    walkFrames[netDirection.Value][walkFrame],
                    Color.White * 0.75f,
                    0.0f,
                    Vector2.Zero,
                    dinoScale,
                    SpriteEffects.None,
                    drawLayer
                   );

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X, localPosition.Y + 32f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White * 0.65f, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

            b.Draw(
                hatsTexture,
                dinoPosition + hatOffsets[netDirection.Value][netScale], //+ (hatOffsets[netDirection.Value][hatFrame] / 8 * dinoScale),
                hatSourceRects[netDirection.Value],
                Color.White * 0.6f,
                hatRotates[netDirection.Value][hatFrame],
                Vector2.Zero,
                dinoScale,
                flip ? (SpriteEffects)1 : 0,
                netDirection.Value == 0 ? 0.989f : 0.991f);

        }

        public override void PerformSpecial(Vector2 farmerPosition)
        {

            specialTimer = 90;

            netSpecialActive.Set(true);

            SpellHandle beam = new(currentLocation, farmerPosition, GetBoundingBox().Center.ToVector2(), 128, DamageToFarmer * 0.4f);

            beam.type = SpellHandle.spells.beam;

            beam.scheme = specialScheme;

            beam.added = new() { SpellHandle.effects.burn };

            beam.boss = this;

            Mod.instance.spellRegister.Add(beam);

        }

    }

}
