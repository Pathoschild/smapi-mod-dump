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

using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace StardewDruid.Monster.Boss
{
    public class Dragon : StardewDruid.Monster.Boss.Boss
    {
        // ============================= Dragon Specific

        public Texture2D shadowTexture;

        public List<Rectangle> shadowFrames;

        public NetBool netBreathActive = new NetBool(false);
        public Dictionary<int, List<Rectangle>> breathFrames;
        public Dictionary<int, Vector2> breathVectors;
        public Dictionary<int, Vector2> breathVectorsFlip;
        public int breathFrame;
        public Texture2D breathTexture;
        public string breathColour;

        public Dragon()
        {
        }

        public Dragon(Vector2 vector, int CombatModifier, string name = "PurpleDragon", string template = "Pepper Rex")
          : base(vector, CombatModifier, name, template)
        {

        }

        public override void SetMode(int mode)
        {

            base.SetMode(mode);

            MaxHealth = (int)(MaxHealth * 1.35);

            Health = MaxHealth;

            DamageToFarmer = (int)(DamageToFarmer * 1.25f);

        }

        public override void LoadOut()
        {

            DragonWalk();

            DragonFlight();

            DragonSpecial();

            loadedOut = true;

        }

        protected override void initNetFields()
        {
            base.initNetFields();
            NetFields.AddField(netBreathActive,"netBreathActive");

        }

        public override Rectangle GetBoundingBox()
        {
            
            Vector2 position = Position;

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            return new Rectangle((int)position.X - 24 - (netScale * 12), (int)position.Y - 64 - flightHeight - (netScale * 32), 112 + (netScale * 24), 128 + (netScale * 32));

        }

        public virtual void DragonWalk()
        {

            characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", realName.Value + ".png"));

            walkCeiling = 5;

            walkFloor = 1;

            walkInterval = 9;

            gait = 3;

            shadowTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "DragonShadow.png"));

            shadowFrames = new List<Rectangle>()
              {
                new Rectangle(0, 0, 64, 32),
                new Rectangle(0, 32, 64, 32),
                new Rectangle(0, 64, 64, 32),
                new Rectangle(0, 32, 64, 32),
                new Rectangle(64, 0, 64, 32),
                new Rectangle(64, 32, 64, 32),
                new Rectangle(64, 64, 64, 32),
                new Rectangle(64, 32, 64, 32)
              };

            idleFrames = FrameSeries(64, 64, 0, 0, 1);

            walkFrames = FrameSeries(64, 64, 64);

            overHead = new(0, -224);

        }

        public virtual void DragonFlight()
        {

            flightInterval = 9;

            flightSpeed = 12;

            flightAscend = 8;

            flightCeiling = 4;

            flightFloor = 1;

            flightLast = 5;

            flightTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", realName.Value + "Flight.png"));

            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 64, 128, 64),
                    new Rectangle(128, 64, 128, 64),
                    new Rectangle(256, 64, 128, 64),
                    new Rectangle(384, 64, 128, 64),
                    new Rectangle(256, 64, 128, 64),
                    new Rectangle(0, 64, 128, 64)
                },
                [1] = new List<Rectangle>()
                {
                    new Rectangle(0, 0, 128, 64),
                    new Rectangle(128, 0, 128, 64),
                    new Rectangle(256, 0, 128, 64),
                    new Rectangle(384, 0, 128, 64),
                    new Rectangle(256, 0, 128, 64),
                    new Rectangle(0, 0, 128, 64)
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 128, 64),
                    new Rectangle(128, 128, 128, 64),
                    new Rectangle(256, 128, 128, 64),
                    new Rectangle(384, 128, 128, 64),
                    new Rectangle(256, 128, 128, 64),
                    new Rectangle(0, 128, 128, 64)
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(0, 0, 128, 64),
                    new Rectangle(128, 0, 128, 64),
                    new Rectangle(256, 0, 128, 64),
                    new Rectangle(384, 0, 128, 64),
                    new Rectangle(256, 0, 128, 64),
                    new Rectangle(0, 0, 128, 64)
                }
            };



        }

        public virtual void DragonSpecial()
        {

            abilities = 3;

            cooldownInterval = 90;

            specialCeiling = 2;

            specialFloor = 0;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            specialInterval = 12;

            barrageThreshold = 544;

            specialFrames = walkFrames;

            specialTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", realName.Value + "Special.png"));

            sweepSet = true;

            sweepInterval = 9;

            sweepTexture = flightTexture;

            sweepFrames = walkFrames;

            sweepFrames = new()
            {
                [0] = new() {
                    new Rectangle(0, 64, 128, 64),
                    new Rectangle(0, 0, 128, 64),
                    new Rectangle(0, 128, 128, 64),
                    new Rectangle(0, 128, 128, 64),
                    new Rectangle(0, 0, 128, 64),
                    new Rectangle(0, 64, 128, 64),
                },
            };

            breathFrames = new()
            {
                [0] = new List<Rectangle>()
                {
                new Rectangle(0, 256, 128, 128),
                new Rectangle(128, 256, 128, 128),
                new Rectangle(256, 256, 128, 128),
                },
                [1] = new List<Rectangle>()
                {
                new Rectangle(0, 128, 128, 128),
                new Rectangle(128, 128, 128, 128),
                new Rectangle(256, 128, 128, 128),
                },
                [2] = new List<Rectangle>()
                {
                new Rectangle(0, 0, 128, 128),
                new Rectangle(128, 0, 128, 128),
                new Rectangle(256, 0, 128, 128),
                },
                [3] = new List<Rectangle>()
                {
                new Rectangle(0, 128, 128, 128),
                new Rectangle(128, 128, 128, 128),
                new Rectangle(256, 128, 128, 128),
                }

            };

            breathVectors = new()
            {
                [0] = new(0, -64),
                [1] = new(32, 0),
                [2] = new(0, 0),
                [3] = new(0, 0),
                [4] = new(48, -64),
                [5] = new(88, 8),
                [6] = new(48, 24),
                [7] = new(64, 0),
            };

            breathVectorsFlip = new()
            {
                [0] = new(-64, -64),
                [1] = new(-160, 0),
                [2] = new(-64, 0),
                [3] = new(-96, 0),
                [4] = new(-48, -64),
                [5] = new(-160, 0),
                [6] = new(-48, 24),
                [7] = new(-88, 8),
            };

            breathColour = "Red";

            specialScheme = SpellHandle.schemes.fire;

            if (realName.Value == "BlackDragon" || realName.Value == "BlueDragon")
            {

                breathColour = "Blue";

                specialScheme = SpellHandle.schemes.ether;

            }

            breathTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", breathColour + "DragonBreath.png"));


        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            int adjustDirection = netDirection.Value == 3 ? 1 : netDirection.Value;

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            netScale++;

            Vector2 flightPosition = new Vector2(localPosition.X - 96 - (32 * netScale), localPosition.Y - 128f - flightHeight - (16 * netScale));
            
            Vector2 flightShadow = new Vector2(localPosition.X - 64 - (24 * netScale), localPosition.Y - (12 * netScale));
            
            Vector2 spritePosition = new Vector2(localPosition.X - 32 - (16 * netScale), localPosition.Y - 128f - flightHeight - (16 * netScale));
           
            Vector2 spriteShadow = new Vector2(localPosition.X - 32 - (16 * netScale), localPosition.Y - (8 * netScale));

            float spriteScale = 2f + (0.5f * netScale);

            DrawEmote(b, localPosition, drawLayer);

            if (netFlightActive.Value)
            {

                b.Draw(flightTexture, flightPosition, new Rectangle?(flightFrames[adjustDirection][flightFrame]), Color.White * 0.75f, 0, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(shadowTexture, flightShadow, new Rectangle?(shadowFrames[netDirection.Value + 4]), Color.White * 0.15f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale*1.5f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

                return;

            }

            if (netSweepActive.Value)
            {

                int sweepAdjust = 0;
                int shadowAdjust = 1;

                switch (netDirection.Value)
                {
                    case 0:
                        if (netAlternative.Value == 3)
                        {
                            sweepAdjust = 3;
                        }
                        shadowAdjust = 2;

                        break;
                    case 2:
                        if (netAlternative.Value == 3)
                        {
                            sweepAdjust = 1;
                        }
                        shadowAdjust = 0;
                        break;
                    case 3:
                        sweepAdjust = 2;
                        break;
                }

                int sweepingFrame = (netDirection.Value + sweepFrame + sweepAdjust + 1) % 6;

                switch (sweepingFrame)
                {
                    case 0:
                    case 5:
                        shadowAdjust = 2;
                        break;
                    case 2:
                    case 3:
                        shadowAdjust = 0;
                        break;

                }

                bool sweepFlip = sweepingFrame > 2;

                b.Draw(sweepTexture, flightPosition, sweepFrames[0][sweepFrame], Color.White * 0.75f, 0, new Vector2(0.0f, 0.0f), spriteScale, sweepFlip ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(shadowTexture, flightShadow, shadowFrames[shadowAdjust], Color.White * 0.15f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale*1.5f, sweepFlip ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

                return;

            }

            if (netSpecialActive.Value)
            {

                b.Draw(specialTexture, spritePosition, new Rectangle?(walkFrames[adjustDirection][walkFrame]), Color.White * 0.75f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                if (netBreathActive.Value)
                {

                    Vector2 breathVector = (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? breathVectorsFlip[netDirection.Value] : breathVectors[netDirection.Value];

                    b.Draw(breathTexture, spritePosition + (breathVector * spriteScale), breathFrames[netDirection.Value][specialFrame], Color.White * 0.65f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer + (netDirection.Value == 2 ? 0.005f : -0.005f));

                }

            }
            else if (netHaltActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(idleFrames[adjustDirection][0]), Color.White * 0.75f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(walkFrames[adjustDirection][walkFrame]), Color.White * 0.75f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(shadowTexture, spriteShadow, new Rectangle?(shadowFrames[adjustDirection]), Color.White * 0.15f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer - 1E-05f);

        }

        public override void PerformSpecial(Vector2 farmerPosition)
        {

            specialTimer = 60;

            netSpecialActive.Set(true);

            netBreathActive.Set(true);

            currentLocation.playSound("furnace");

            List<Vector2> zero = BlastZero();

            /*SpellHandle burn = new(currentLocation, zero[0] * 64, GetBoundingBox().Center.ToVector2(), 2, 0, DamageToFarmer * 0.2f);

            burn.type = SpellHandle.spells.burn;

            burn.scheme = specialScheme;

            burn.boss = this;

            Mod.instance.spellRegister.Add(burn);*/

            SpellHandle burn = new(Game1.player, zero[0] * 64, 128, DamageToFarmer);

            burn.scheme = specialScheme;

            burn.boss = this;

            burn.added = new() { SpellHandle.effects.burn, };

            Mod.instance.spellRegister.Add(burn);

        }

        public override void ClearSpecial()
        {

            base.ClearSpecial();

            netBreathActive.Set(false);

        }

    }

}