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
    public class Scavenger : Boss
    {

        public Scavenger()
        {
        }

        public Scavenger(Vector2 vector, int CombatModifier, string useName = "Scavenger")
          : base(vector, CombatModifier / 2, useName)
        {
            
            DamageToFarmer /= 2;

        }

        public override void LoadOut()
        {

            CatWalk();

            CatFlight();

            CatSpecial();

            overHead = new(16, -128);

            loadedOut = true;

        }

        public void CatWalk()
        {

            characterTexture = MonsterData.MonsterTexture(realName.Value);

            walkCeiling = 5;

            walkFloor = 0;

            walkInterval = 9;

            gait = 2;

            idleFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32, 32);

        }

        public void CatFlight()
        {
            flightSpeed = 12;

            flightHeight = 2;

            flightFloor = 1;

            flightCeiling = 4;

            flightLast = 5;

            flightInterval = 9;

            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 192, 32, 32),
                    new Rectangle(32, 192, 32, 32),
                    new Rectangle(64, 192, 32, 32),
                    new Rectangle(96, 192, 32, 32),
                    new Rectangle(128, 192, 32, 32),
                    new Rectangle(160, 192, 32, 32),
                },

                [1] = new List<Rectangle>()
                {
                    new Rectangle(0, 160, 32, 32),
                    new Rectangle(32, 160, 32, 32),
                    new Rectangle(64, 160, 32, 32),
                    new Rectangle(96, 160, 32, 32),
                    new Rectangle(128, 160, 32, 32),
                    new Rectangle(160, 160, 32, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 32, 32),
                    new Rectangle(32, 128, 32, 32),
                    new Rectangle(64, 128, 32, 32),
                    new Rectangle(96, 128, 32, 32),
                    new Rectangle(128, 128, 32, 32),
                    new Rectangle(160, 128, 32, 32),
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(0, 224, 32, 32),
                    new Rectangle(32, 224, 32, 32),
                    new Rectangle(64, 224, 32, 32),
                    new Rectangle(96, 224, 32, 32),
                    new Rectangle(128, 224, 32, 32),
                    new Rectangle(160, 224, 32, 32),
                }
            };

        }

        public void CatSpecial()
        {

            abilities = 2;

            specialCeiling = 0;

            specialFloor = 0;

            cooldownInterval = 60;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            specialFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(128, 192, 32, 32),
                },
                [1] = new List<Rectangle>()
                {
                    new Rectangle(128, 160, 32, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(128, 128, 32, 32),
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(128, 224, 32, 32),
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

            cooldownInterval = 60;

            tempermentActive = temperment.aggressive;

        }

        public override Rectangle GetBoundingBox()
        {
            Vector2 position = Position;
            return new Rectangle((int)position.X - 32, (int)position.Y - flightHeight - 32, 128, 128);
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

            if (netFlightActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f - flightHeight), new Rectangle?(flightFrames[netDirection.Value][flightFrame]), Color.White, 0, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f), new Rectangle?(specialFrames[netDirection.Value][0]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if(netHaltActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f), new Rectangle?(idleFrames[netDirection.Value][0]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f), new Rectangle?(walkFrames[netDirection.Value][walkFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X, localPosition.Y + 32f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

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
