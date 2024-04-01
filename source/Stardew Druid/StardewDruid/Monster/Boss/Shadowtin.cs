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
using StardewDruid.Event;
using StardewDruid.Map;
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
        
        public Queue<Vector2> blastZone;
        
        public int blastRadius;

        public Shadowtin()
        {
        }

        public Shadowtin(Vector2 vector, int CombatModifier, string useName = "Shadowtin")
          : base(vector, CombatModifier, useName)
        {
            
        }

        public override void BaseMode()
        {

            if(realName.Value == "Shadowtin")
            {
                MaxHealth = Math.Max(10000, combatModifier * 500);

                Health = MaxHealth;

                DamageToFarmer = Math.Max(10, Math.Min(60, combatModifier * 3));

                return;

            }

            base.BaseMode();

        }

        public override void LoadOut()
        {

            Health *= 2;

            MaxHealth *= 2;

            ShadowWalk();

            ShadowFlight();
            
            ShadowSpecial();

            overHead = new(16, -144);

            loadedOut = true;

        }

        public virtual void ShadowWalk()
        {
            
            characterTexture = MonsterData.MonsterTexture(realName.Value);

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

            blastZone = new();

            blastRadius = 1;

            abilities = 2;

            cooldownInterval = 60;

            specialCeiling = 1;

            specialFloor = 0;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            specialInterval = 30;

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

            sweepInterval = 9;

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

        }

        public override void HardMode()
        {

            Health *= 3;

            Health /= 2;

            MaxHealth = Health;

            blastRadius = 2;

            cooldownInterval = 40;

            tempermentActive = temperment.aggressive;

        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 position = Position;
            return new Rectangle((int)position.X - 16, (int)position.Y - flightHeight - 32, 96, 96);
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

            if (netSweepActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 64f), new Rectangle?(sweepFrames[netDirection.Value][sweepFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

                shadowOffset = 64;

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 64f), new Rectangle?(specialFrames[netDirection.Value][specialFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);
                
                shadowOffset = 64;

            }
            else if (netFlightActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f - flightHeight), new Rectangle?(flightFrames[netDirection.Value][flightFrame]), Color.White, 0, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netHaltActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f), new Rectangle?(idleFrames[netDirection.Value][0]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f), new Rectangle?(walkFrames[netDirection.Value][walkFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X - shadowOffset, localPosition.Y + 32f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

        }

    }

}
