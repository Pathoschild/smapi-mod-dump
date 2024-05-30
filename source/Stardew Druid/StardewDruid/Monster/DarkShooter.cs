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
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewDruid.Monster
{
    public class DarkShooter : Boss
    {

        public Texture2D weaponTexture;

        public Dictionary<int, List<Rectangle>> weaponFrames = new();
        public Dictionary<int, List<Rectangle>> channelWeaponFrames = new();

        public NetInt netChannel = new(0);

        public DarkShooter()
        {


        }

        public DarkShooter(Vector2 vector, int CombatModifier, string name = "DarkShooter")
          : base(vector, CombatModifier, name)
        {

            objectsToDrop.Clear();

            objectsToDrop.Add("769");

            if (Game1.random.Next(3) == 0)
            {
                objectsToDrop.Add("768");
            }
            else if (Game1.random.Next(4) == 0 && combatModifier >= 120)
            {
                List<string> shadowGems = new()
                {
                    "62","66","68","70",
                };

                objectsToDrop.Add(shadowGems[Game1.random.Next(shadowGems.Count)]);

            }
            else if (Game1.random.Next(5) == 0 && combatModifier >= 240)
            {
                List<string> shadowGems = new()
                {
                    "60","64","72",
                };

                objectsToDrop.Add(shadowGems[Game1.random.Next(shadowGems.Count)]);
            }

        }

        public override void LoadOut()
        {

            baseMode = 2;

            baseJuice = 4;

            basePulp = 20;

            ShooterWalk();

            ShooterFlight();

            ShooterSpecial();

            overHead = new(16, -144);

            loadedOut = true;

        }

        public override void SetMode(int mode)
        {
            base.SetMode(mode);

            abilities = 2;

            tempermentActive = temperment.ranged;

        }

        public virtual void ShooterWalk()
        {

            characterTexture = MonsterHandle.MonsterTexture(realName.Value);

            walkInterval = 12;

            gait = 2;

            overHead = new(0, -128);

            idleFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32, 0, 128, 6, idleFrames);

            walkSwitch = true;

        }

        public virtual void ShooterFlight()
        {

            flightInterval = 9;

            flightSpeed = 9;

            flightHeight = 2;

            flightTexture = characterTexture;

            flightFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new()
                {
                    new(0, 192, 32, 32),
                },
                [1] = new()
                {
                    new(0, 160, 32, 32),
                },
                [2] = new()
                {
                    new(0, 128, 32, 32),
                },
                [3] = new()
                {
                    new(0, 224, 32, 32),
                },
                [4] = new()
                {
                    new(32, 64, 32, 32),
                },
                [5] = new()
                {
                    new(32, 32, 32, 32),
                },
                [6] = new()
                {
                    new(32, 0, 32, 32),
                },
                [7] = new()
                {
                    new(32, 96, 32, 32),
                },
                [8] = new()
                {
                    new(96,192,32,32),
                    new(128,192,32,32),
                    new(160,192,32,32),
                },
                [9] = new()
                {
                    new(96,160,32,32),
                    new(128,160,32,32),
                    new(160,160,32,32),
                },
                [10] = new()
                {
                    new(96,128,32,32),
                    new(128,128,32,32),
                    new(160,128,32,32),
                },
                [11] = new()
                {
                    new(96,224,32,32),
                    new(128,224,32,32),
                    new(160,224,32,32),
                },
            };

            smashSet = true;

            smashFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new()
                {
                    new(0, 192, 32, 32),
                    new(32, 64, 32, 32),
                },
                [1] = new()
                {
                    new(0, 160, 32, 32),
                    new(32, 32, 32, 32),
                },
                [2] = new()
                {
                    new(0, 128, 32, 32),
                    new(32, 0, 32, 32),
                },
                [3] = new()
                {
                    new(0, 160, 32, 32),
                    new(32, 32, 32, 32),
                },
                [4] = new()
                {
                    new(64, 320, 32, 32),
                },
                [5] = new()
                {
                    new(64, 288, 32, 32),
                },
                [6] = new()
                {
                    new(64, 256, 32, 32),
                },
                [7] = new()
                {
                    new(64, 288, 32, 32),
                },
                [8] = new()
                {
                    new(96, 320, 32, 32),
                },
                [9] = new()
                {
                    new(96, 288, 32, 32),
                },
                [10] = new()
                {
                    new(96, 256, 32, 32),
                },
                [11] = new()
                {
                    new(96, 288, 32, 32),
                },
            };

        }

        public void ShooterSpecial()
        {

            abilities = 2;

            cooldownInterval = 180;

            specialScheme = IconData.schemes.fire;

            specialCeiling = 1;

            specialFloor = 0;

            specialInterval = 30;

            specialTexture = characterTexture;

            specialFrames = new Dictionary<int, List<Rectangle>>()
            {
                [0] = new()
                {

                    new(128, 64, 32, 32),
                    new(160, 64, 32, 32),

                },
                [1] = new()
                {

                    new(128, 32, 32, 32),
                    new(160, 32, 32, 32),

                },
                [2] = new()
                {

                    new(128, 0, 32, 32),
                    new(160, 0, 32, 32),

                },
                [3] = new()
                {

                    new(128, 32, 32, 32),
                    new(160, 32, 32, 32),

                },

            };

            channelFrames = new Dictionary<int, List<Rectangle>>()
            {
                
                [0] = new()
                {
                    new(160, 64, 32, 32),
                    new(160, 64, 32, 32),
                    new(160, 64, 32, 32),
                    new(128, 64, 32, 32),

                },
                [1] = new()
                {
                    new(160, 32, 32, 32),
                    new(160, 32, 32, 32),
                    new(160, 32, 32, 32),
                    new(128, 32, 32, 32),

                },
                [2] = new()
                {
                    new(160, 0, 32, 32),
                    new(160, 0, 32, 32),
                    new(160, 0, 32, 32),
                    new(128, 0, 32, 32),

                },
                [3] = new()
                {
                    new(160, 32, 32, 32),
                    new(160, 32, 32, 32),
                    new(160, 32, 32, 32),
                    new(128, 32, 32, 32),
 
                },

            };

            sweepSet = true;

            sweepInterval = 12;

            sweepTexture = characterTexture;

            sweepFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(96, 192, 32, 32),
                    new Rectangle(192, 192, 32, 32),
                    new Rectangle(224, 192, 32, 32),
                },
                [1] = new()
                {
                    new Rectangle(96, 160, 32, 32),
                    new Rectangle(192, 160, 32, 32),
                    new Rectangle(224, 160, 32, 32),
                },
                [2] = new()
                {
                    new Rectangle(96, 128, 32, 32),
                    new Rectangle(192, 128, 32, 32),
                    new Rectangle(224, 128, 32, 32),
                },
                [3] = new()
                {
                    new Rectangle(96, 224, 32, 32),
                    new Rectangle(192, 224, 32, 32),
                    new Rectangle(224, 224, 32, 32),
                },
            };

            weaponTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "FirearmBazooka.png"));

            weaponFrames = new()
            {
                [0] = new()
                {

                    new(0, 128, 64, 64),
                    new(64, 128, 64, 64),

                },
                [1] = new()
                {

                    new(0, 64, 64, 64),
                    new(64, 64, 64, 64),

                },
                [2] = new()
                {

                    new(0, 0, 64, 64),
                    new(64, 0, 64, 64),

                },
                [3] = new()
                {

                    new(0, 64, 64, 64),
                    new(64, 64, 64, 64),

                },
            };

            channelWeaponFrames = new()
            {
                [0] = new()
                {
                    new(64, 128, 64, 64),
                    new(64, 128, 64, 64),
                    new(64, 128, 64, 64),
                    new(0, 128, 64, 64),

                },
                [1] = new()
                {
                    new(64, 64, 64, 64),
                    new(64, 64, 64, 64),
                    new(64, 64, 64, 64),
                    new(0, 64, 64, 64),

                },
                [2] = new()
                {
                    new(64, 0, 64, 64),
                    new(64, 0, 64, 64),
                    new(64, 0, 64, 64),
                    new(0, 0, 64, 64),

                },
                [3] = new()
                {
                    new(64, 64, 64, 64),
                    new(64, 64, 64, 64),
                    new(64, 64, 64, 64),
                    new(0, 64, 64, 64),

                },
            };

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

            Vector2 spritePosition = localPosition - new Vector2(20 + (netScale * 4), 40f + (netScale * 8) + flightHeight);

            float spriteScale = 3.25f + (0.25f * netScale);

            if (netSweepActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(sweepFrames[netDirection.Value][sweepFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

                shadowOffset = 56;

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(specialFrames[netDirection.Value][specialFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(
                     weaponTexture,
                     spritePosition - new Vector2(64, 64f),
                     new Rectangle?(weaponFrames[netDirection.Value][specialFrame]),
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer + 0.0001f
                 );

            }
            else if (netChannelActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(channelFrames[netDirection.Value][specialFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

                b.Draw(
                     weaponTexture,
                     spritePosition - new Vector2(64, 64f),
                     new Rectangle?(channelWeaponFrames[netDirection.Value][specialFrame]),
                     Color.White,
                     0f,
                     Vector2.Zero,
                     4f,
                     (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer + 0.0001f
                 );

            }
            else if (netFlightActive.Value)
            {

                int setFlightSeries = netDirection.Value + (netFlightProgress.Value * 4);

                int setFlightFrame = Math.Min(flightFrame, (flightFrames[setFlightSeries].Count - 1));

                b.Draw(characterTexture, spritePosition, new Rectangle?(flightFrames[setFlightSeries][setFlightFrame]), Color.White, 0, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netSmashActive.Value)
            {

                int setFlightSeries = netDirection.Value + (netFlightProgress.Value * 4);

                int setFlightFrame = Math.Min(flightFrame, (smashFrames[setFlightSeries].Count - 1));

                b.Draw(characterTexture, spritePosition, new Rectangle?(smashFrames[setFlightSeries][setFlightFrame]), Color.White, 0, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) || netDirection.Value == 3 ? (SpriteEffects)1 : 0, drawLayer);

            }
            else if (netHaltActive.Value)
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(idleFrames[netDirection.Value][0]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }
            else
            {

                b.Draw(characterTexture, spritePosition, new Rectangle?(walkFrames[netDirection.Value][walkFrame]), Color.White, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, (netDirection.Value % 2 == 0 && netAlternative.Value == 3) ? (SpriteEffects)1 : 0, drawLayer);

            }

            b.Draw(Game1.shadowTexture, new(localPosition.X - shadowOffset, localPosition.Y + 40f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

        }

        public override void PerformBarrage(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval * 2;

            netChannelActive.Set(true);

            SetCooldown(2);

            SpellHandle fireball = new(currentLocation, target, GetBoundingBox().Center.ToVector2(), 256, DamageToFarmer);

            fireball.type = SpellHandle.spells.ballistic;

            fireball.projectile = 3;

            fireball.scheme = specialScheme;

            fireball.display = IconData.impacts.impact;

            fireball.boss = this;

            fireball.added = new() { SpellHandle.effects.burn, };

            Mod.instance.spellRegister.Add(fireball);

        }

    }

}

