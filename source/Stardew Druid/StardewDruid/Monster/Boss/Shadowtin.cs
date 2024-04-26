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
using StardewDruid.Data;
using StardewDruid.Event;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.IO;


namespace StardewDruid.Monster.Boss
{
    public class Shadowtin : Boss
    {
        
        public Texture2D swipeTexture;
        public Dictionary<int, List<Rectangle>> swipeFrames;

        public Shadowtin()
        {
        }

        public Shadowtin(Vector2 vector, int CombatModifier, string useName = "Shadowtin")
          : base(vector, CombatModifier, useName)
        {
            
        }

        public override void SetMode(int mode)
        {
            base.SetMode(mode);

            if (realName.Value == "Shadowtin")
            {
                MaxHealth += 2;

                Health = MaxHealth;

                return;

            }

        }

        public override void LoadOut()
        {

            ShadowWalk();

            ShadowFlight();
            
            ShadowSpecial();

            overHead = new(16, -144);

            loadedOut = true;

        }

        public virtual void ShadowWalk()
        {
            
            characterTexture = MonsterHandle.MonsterTexture(realName.Value);

            walkCeiling = 5;

            walkFloor = 0;

            walkInterval = 12;

            gait = 2;

            overHead = new(0, -128);

            idleFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32, 0, 128);

        }

        public virtual void ShadowFlight()
        {

            flightInterval = 9;

            flightSpeed = 12;

            flightHeight = 2;

            flightCeiling = 2;

            flightFloor = 1;

            flightLast = 3;

            flightTexture = characterTexture;

            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new()
                {
                    new(0, 192, 32, 32),
                    new(32, 64, 32, 32),
                    new(32, 64, 32, 32),
                    new(0, 192, 32, 32),
                },
                [1] = new()
                {
                    new(0, 160, 32, 32),
                    new(32, 32, 32, 32),
                    new(32, 32, 32, 32),
                    new(0, 160, 32, 32),
                },
                [2] = new()
                {
                    new(0, 128, 32, 32),
                    new(32, 0, 32, 32),
                    new(32, 0, 32, 32),
                    new(0, 128, 32, 32),
                },
                [3] = new()
                {
                    new(0, 224, 32, 32),
                    new(32, 96, 32, 32),
                    new(32, 96, 32, 32),
                    new(0, 224, 32, 32),
                },
            };

        }

        public void ShadowSpecial()
        {

            abilities = 2;

            cooldownInterval = 180;

            specialCeiling = 1;

            specialFloor = 0;

            specialInterval = 30;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            specialTexture = characterTexture;

            specialFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new()
                {

                    new(64, 64, 32, 32),
                    new(96, 64, 32, 32),

                },
                [1] = new()
                {

                    new(64, 32, 32, 32),
                    new(96, 32, 32, 32),

                },
                [2] = new()
                {

                    new(64, 0, 32, 32),
                    new(96, 0, 32, 32),

                },
                [3] = new()
                {

                    new(64, 96, 32, 32),
                    new(96, 96, 32, 32),

                },

            };

            specialScheme = SpellHandle.schemes.ether;

            sweepSet = true;

            sweepInterval = 8;

            sweepTexture = characterTexture;

            sweepFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                },
                [1] = new()
                {
                    new Rectangle(0, 384, 64, 64),
                    new Rectangle(64, 384, 64, 64),
                    new Rectangle(128, 384, 64, 64),
                    new Rectangle(192, 384, 64, 64),
                },
                [2] = new()
                {
                    new Rectangle(0, 256, 64, 64),
                    new Rectangle(64, 256, 64, 64),
                    new Rectangle(128, 256, 64, 64),
                    new Rectangle(192, 256, 64, 64),
                },
                [3] = new()
                {
                    new Rectangle(0, 320, 64, 64),
                    new Rectangle(64, 320, 64, 64),
                    new Rectangle(128, 320, 64, 64),
                    new Rectangle(192, 320, 64, 64),
                },
            };

            swipeTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Swipe.png"));

            swipeFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                },
                [1] = new()
                {
                    new Rectangle(0, 128, 64, 64),
                    new Rectangle(64, 128, 64, 64),
                    new Rectangle(128, 128, 64, 64),
                    new Rectangle(192, 128, 64, 64),
                },
                [2] = new()
                {
                    new Rectangle(0, 0, 64, 64),
                    new Rectangle(64, 0, 64, 64),
                    new Rectangle(128, 0, 64, 64),
                    new Rectangle(192, 0, 64, 64),
                },
                [3] = new()
                {
                    new Rectangle(0, 64, 64, 64),
                    new Rectangle(64, 64, 64, 64),
                    new Rectangle(128, 64, 64, 64),
                    new Rectangle(192, 64, 64, 64),
                },
            };

            specialScheme = SpellHandle.schemes.fire;

        }


        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = (float)StandingPixel.Y / 10000f;

            DrawEmote(b, localPosition, drawLayer);

            int shadowOffset = 0;

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            Vector2 spritePosition = localPosition - new Vector2(20 + (netScale * 4), 40f + (netScale * 8) - flightHeight);

            Vector2 sweepPosition = localPosition - new Vector2(72 + (netScale * 8), 96f + (netScale * 16) - flightHeight);

            float spriteScale = 3.25f + (0.25f * netScale);

            if (netSweepActive.Value)
            {

                b.Draw(characterTexture, sweepPosition, new Rectangle?(sweepFrames[netDirection.Value][sweepFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(swipeTexture, sweepPosition, new Rectangle?(swipeFrames[netDirection.Value][sweepFrame]), Color.White*0.5f, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);
                
                shadowOffset = 56;

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(specialFrames[netDirection.Value][specialFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netFlightActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(flightFrames[netDirection.Value][flightFrame]), Color.White, 0, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netHaltActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(idleFrames[netDirection.Value][0]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(walkFrames[netDirection.Value][walkFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X - shadowOffset, localPosition.Y + 32f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

        }

    }

}
