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
    public class Shadowtin : Dragon
    {
        
        public Queue<Vector2> blastZone;
        
        public int blastRadius;

        public Shadowtin()
        {
        }

        public Shadowtin(Vector2 vector, int CombatModifier)
          : base(vector, CombatModifier, "Shadowtin")
        {
            
        }

        public override void LoadOut()
        {

            Health *= 2;

            MaxHealth *= 2;

            BaseWalk();
            ShadowFlight();
            ShadowSpecial();

            overHead = new(16, -144);

            loadedOut = true;

        }


        public virtual void ShadowFlight()
        {

            flightIncrement = 9;

            flightSpeed = 12;

            flightHeight = 2;

            flightCeiling = 4;

            flightFloor = 1;

            flightLast = 4;

            flightTexture = characterTexture;

            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 160, 64, 32),
                    new Rectangle(64, 128, 64, 32),
                    new Rectangle(0, 128, 64, 32),
                    new Rectangle(64, 160, 64, 32),
                    new Rectangle(0, 160, 64, 32),
                },

                [1] = new List<Rectangle>()
                {
                    new Rectangle(64, 128, 64, 32),
                    new Rectangle(0, 128, 64, 32),
                    new Rectangle(64, 160, 64, 32),
                    new Rectangle(0, 160, 64, 32),
                    new Rectangle(64, 128, 64, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 64, 32),
                    new Rectangle(64, 160, 64, 32),
                    new Rectangle(0, 160, 64, 32),
                    new Rectangle(64, 128, 64, 32),
                    new Rectangle(0, 128, 64, 32),
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(64, 160, 64, 32),
                    new Rectangle(0, 160, 64, 32),
                    new Rectangle(64, 128, 64, 32),
                    new Rectangle(0, 128, 64, 32),
                    new Rectangle(64, 160, 64, 32),
                }
            };

        }

        public void ShadowSpecial()
        {

            blastZone = new();

            blastRadius = 1;

            abilities = 2;

            cooldownInterval = 48;

            specialCeiling = 1;

            specialFloor = 0;

            reachThreshold = 64;

            safeThreshold = 544;

            specialThreshold = 320;

            barrageThreshold = 544;

            barrageColor = "Red";

            barrages = new();

            specialTexture = characterTexture;

            specialFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new List<Rectangle>()
                {
                    new Rectangle(0, 160, 64, 32),
                    new Rectangle(0, 160, 64, 32),
                },
                [1] = new List<Rectangle>()
                {
                    new Rectangle(64, 128, 64, 32),
                    new Rectangle(64, 128, 64, 32),
                },
                [2] = new List<Rectangle>()
                {
                    new Rectangle(0, 128, 64, 32),
                    new Rectangle(0, 128, 64, 32),
                },
                [3] = new List<Rectangle>()
                {
                    new Rectangle(64, 160, 64, 32),
                    new Rectangle(64, 160, 64, 32),
                }
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
            return new Rectangle((int)position.X - 16, (int)position.Y - netFlightHeight.Value - 32, 96, 96);
        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {
            if (IsInvisible || !Utility.isOnScreen(Position, 128))
            {
                return;
            }

            Vector2 localPosition = getLocalPosition(Game1.viewport);

            float drawLayer = Game1.player.getDrawLayer();

            if (IsEmoting && !Game1.eventUp)
            {
                Vector2 emotePosition = localPosition;
                emotePosition.Y -= 32 + Sprite.SpriteHeight * 4;
                b.Draw(Game1.emoteSpriteSheet, localPosition, new Rectangle?(new Rectangle(CurrentEmoteIndex * 16 % Game1.emoteSpriteSheet.Width, CurrentEmoteIndex * 16 / Game1.emoteSpriteSheet.Width * 16, 16, 16)), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer);
            }

            if (netFlightActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 64f - netFlightHeight.Value), new Rectangle?(flightFrames[netDirection.Value][netFlightFrame.Value]), Color.White, 0, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 96f, localPosition.Y - 64f), new Rectangle?(specialFrames[netDirection.Value][0]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, new Vector2(localPosition.X - 32f, localPosition.Y - 64f), new Rectangle?(walkFrames[netDirection.Value][netWalkFrame.Value]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), 4f, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X, localPosition.Y + 32f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

        }

        public override void PerformSpecial()
        {
            behaviourActive = behaviour.special;

            behaviourTimer = 72;

            netSpecialActive.Set(true);

            List<Vector2> zero = BlastTarget();

            BarrageHandle fireball = new(currentLocation, zero[0], zero[1], 3, 1, "Red", DamageToFarmer);

            fireball.type = BarrageHandle.barrageType.fireball;

            fireball.monster = this;

            barrages.Add(fireball);

        }
    }

}
