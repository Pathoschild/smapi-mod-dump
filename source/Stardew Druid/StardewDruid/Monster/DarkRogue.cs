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
using StardewDruid.Cast;
using StardewDruid.Data;
using StardewDruid.Render;
using StardewValley;
using System;
using System.Collections.Generic;

namespace StardewDruid.Monster
{
    public class DarkRogue : Boss
    {

        public WeaponRender weaponRender;

        public DarkRogue()
        {


        }

        public DarkRogue(Vector2 vector, int CombatModifier, string name = "DarkRogue")
          : base(vector, CombatModifier, name)
        {
            SpawnData.MonsterDrops(this, SpawnData.drops.shadow);

        }


        public override void LoadOut()
        {

            baseMode = 3;

            baseJuice = 4;
            
            basePulp = 50;

            cooldownInterval = 180;

            DarkWalk();

            DarkFlight();

            DarkCast();

            DarkSmash();

            DarkSword();

            weaponRender = new();

            weaponRender.LoadWeapon(WeaponRender.weapons.estoc);

            overHead = new(16, -144);

            loadedOut = true;

        }


        public virtual void DarkWalk()
        {
            
            characterTexture = MonsterHandle.MonsterTexture(realName.Value);

            walkInterval = 12;

            gait = 2;

            overHead = new(0, -128);

            idleFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32, 0, 128, 6, FrameSeries(32, 32, 0, 0, 1));

            walkSwitch = true;

            woundedFrames = new()
            {
                [0] = new()
                {

                    new(128, 64, 32, 32),

                },
                [1] = new()
                {

                    new(128, 32, 32, 32),

                },
                [2] = new()
                {

                    new(128, 0, 32, 32),

                },
                [3] = new()
                {

                    new(128, 32, 32, 32),

                },

            };

        }

        public virtual void DarkFlight()
        {

            flightInterval = 9;

            flightSpeed = 9;

            flightPeak = 128;

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

        }

        public virtual void DarkSmash()
        {

            smashSet = true;

            smashFrames = new()
            {
                [0] = new()
                {
                    new(0, 320, 32, 32),new(32, 320, 32, 32),
                },
                [1] = new()
                {
                    new(0, 288, 32, 32),new(32, 288, 32, 32),
                },
                [2] = new()
                {
                    new(0, 256, 32, 32),new(32, 256, 32, 32),
                },
                [3] = new()
                {
                    new(0, 288, 32, 32),new(32, 288, 32, 32),
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

        public virtual void DarkBrawl()
        {
            
            sweepSet = true;

            sweepInterval = 12;

            sweepFrames = new ()
            {
                [0] = new ()
                {
                    new Rectangle(96, 192, 32, 32),
                    new Rectangle(192, 192, 32, 32),
                    new Rectangle(224, 192, 32, 32),
                },
                [1] = new ()
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

        }

        public virtual void DarkSword()
        {
            sweepSet = true;

            sweepInterval = 8;

            sweepFrames = new()
            {
                [0] = new()
                {
                    new Rectangle(192, 288, 32, 32),
                    new Rectangle(224, 288, 32, 32),
                    new Rectangle(128, 288, 32, 32),
                    new Rectangle(160, 288, 32, 32),
                },
                [1] = new()
                {
                    new Rectangle(128, 256, 32, 32),
                    new Rectangle(160, 256, 32, 32),
                    new Rectangle(192, 256, 32, 32),
                    new Rectangle(224, 256, 32, 32),
                },
                [2] = new()
                {
                    new Rectangle(128, 288, 32, 32),
                    new Rectangle(160, 288, 32, 32),
                    new Rectangle(192, 288, 32, 32),
                    new Rectangle(224, 288, 32, 32),
                },
                [3] = new()
                {
                    new Rectangle(128, 256, 32, 32),
                    new Rectangle(160, 256, 32, 32),
                    new Rectangle(192, 256, 32, 32),
                    new Rectangle(224, 256, 32, 32),
                },
            };

        }

        public void DarkCast()
        {

            specialSet = true;

            specialCeiling = 1;

            specialFloor = 0;

            specialInterval = 30;

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

        }

        public virtual void DarkBlast()
        {

            specialSet = true;

            cooldownInterval = 180;

            specialCeiling = 1;

            specialFloor = 0;

            specialInterval = 30;

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

            channelSet = true;

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

            int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

            Vector2 spritePosition = localPosition - new Vector2(20 + (netScale * 4), 40f + (netScale * 8) + flightHeight);

            float spriteScale = 3.25f + (0.25f * netScale);

            bool flippant = (netDirection.Value % 2 == 0 && netAlternative.Value == 3);

            bool flippity = flippant || netDirection.Value == 3;

            if (netSweepActive.Value)
            {

                b.Draw(
                     characterTexture,
                     spritePosition,
                     sweepFrames[netDirection.Value][sweepFrame],
                     Color.White,
                     0f,
                     Vector2.Zero,
                     spriteScale,
                     flippity ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                     drawLayer
                );

                weaponRender.DrawWeapon(b, spritePosition, drawLayer, new() { source = sweepFrames[netDirection.Value][sweepFrame], flipped = flippity });

                weaponRender.DrawSwipe(b, spritePosition, drawLayer, new() { source = sweepFrames[netDirection.Value][sweepFrame], flipped = flippity });

            }
            else if (netSpecialActive.Value)
            {

                b.Draw(
                    characterTexture,
                    spritePosition,
                    specialFrames[netDirection.Value][specialFrame],
                    Color.White,
                    0.0f,
                    Vector2.Zero,
                    spriteScale,
                    flippant ? (SpriteEffects)1 : 0,
                    drawLayer
                );

                weaponRender.DrawFirearm(b, spritePosition, drawLayer, new() { source = specialFrames[netDirection.Value][specialFrame], flipped = flippant });

            }
            else if (netChannelActive.Value)
            {

                b.Draw(
                    characterTexture, 
                    spritePosition, 
                    new Rectangle?(channelFrames[netDirection.Value][specialFrame]), 
                    Color.White,
                    0.0f, 
                    new Vector2(0.0f, 0.0f), 
                    spriteScale,
                    flippity ? (SpriteEffects)1 : 0, 
                    drawLayer
                    );

                weaponRender.DrawFirearm(b, spritePosition, drawLayer, new() { source = channelFrames[netDirection.Value][specialFrame], flipped = flippity });

            }
            else if (netFlightActive.Value)
            {

                int setFlightSeries = netDirection.Value + (netFlightProgress.Value * 4);

                int setFlightFrame = Math.Min(flightFrame, (flightFrames[setFlightSeries].Count - 1));

                b.Draw(
                    characterTexture,
                    spritePosition,
                    new Rectangle?(flightFrames[setFlightSeries][setFlightFrame]),
                    Color.White,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    flippant ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netSmashActive.Value)
            {

                int setFlightSeries = netDirection.Value + (netFlightProgress.Value * 4);

                int setFlightFrame = Math.Min(flightFrame, (smashFrames[setFlightSeries].Count - 1));

                b.Draw(
                    characterTexture, 
                    spritePosition, 
                    smashFrames[setFlightSeries][setFlightFrame],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    flippity ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

                weaponRender.DrawWeapon(b, spritePosition, drawLayer, new() { source = smashFrames[setFlightSeries][setFlightFrame], flipped = flippity });

                if (netFlightProgress.Value >= 2)
                {

                    weaponRender.DrawSwipe(b, spritePosition, drawLayer, new() { source = smashFrames[setFlightSeries][setFlightFrame], flipped = flippity });

                }

            }
            else if (netWoundedActive.Value)
            {

                b.Draw(
                    characterTexture,
                    spritePosition,
                    woundedFrames[netDirection.Value][0],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    flippity ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else if (netHaltActive.Value)
            {

                b.Draw(
                    characterTexture,
                    spritePosition,
                    idleFrames[netDirection.Value][0],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    flippant ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

            }
            else
            {

                b.Draw(
                    characterTexture,
                    localPosition - new Vector2(32, 64f),
                    walkFrames[netDirection.Value][walkFrame],
                    Color.White,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    flippant ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                    drawLayer
                );

    
            }

            b.Draw(Game1.shadowTexture, localPosition + new Vector2(10, 44f), new Rectangle?(Game1.shadowTexture.Bounds), Color.White, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

        }

        public override bool PerformSpecial(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval;

            netSpecialActive.Set(true);

            SetCooldown(1);

            SpellHandle fireball = new(currentLocation, target, GetBoundingBox().Center.ToVector2(), 128, GetThreat()*2/3);

            fireball.type = SpellHandle.spells.missile;

            fireball.projectile = 1;

            fireball.missile = IconData.missiles.fireball;

            fireball.display = IconData.impacts.impact;

            fireball.boss = this;

            fireball.scheme = IconData.schemes.ember;

            Mod.instance.spellRegister.Add(fireball);

            return true;

        }


    }


}

