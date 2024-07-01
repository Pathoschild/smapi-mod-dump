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
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static StardewDruid.Cast.SpellHandle;
using static StardewDruid.Data.IconData;

namespace StardewDruid.Monster
{
    public class Phantom : Boss
    {

        public WeaponRender weaponRender;

        public Texture2D hatsTexture;

        public float fadeFactor;

        public Phantom()
        {


        }

        public Phantom(Vector2 vector, int CombatModifier, string name = "Phantom")
          : base(vector, CombatModifier, name)
        {
            SpawnData.MonsterDrops(this, SpawnData.drops.phantom);

        }


        public override void LoadOut()
        {

            baseMode = 2;

            baseJuice = 3;
            
            basePulp = 35;

            hatsTexture = Game1.content.Load<Texture2D>("Characters\\Farmer\\hats");

            fadeFactor = 0.6f;

            PhantomWalk();

            PhantomFlight();

            PhantomSweep();

            PhantomSpecial();

            overHead = new(16, -144);

            loadedOut = true;

        }

        public override void SetMode(int mode)
        {

            base.SetMode(mode);

            if (netMode.Value < 2)
            {

                smashSet = false;

                flightSet = false;

                specialSet = false;

            }
            else
            {

                smashSet = true;

                flightSet = true;

                specialSet = true;

            }

        }
        public override void RandomTemperment()
        {

            Random random = new();

            int newScheme = random.Next(0, 6);

            netScheme.Set(newScheme);

            base.RandomTemperment();

        }

        public virtual void PhantomWalk()
        {
            
            characterTexture = MonsterHandle.MonsterTexture(realName.Value);

            walkInterval = 12;

            gait = 2;

            overHead = new(0, -128);

            idleFrames = FrameSeries(32, 32, 0, 0, 1);

            walkFrames = FrameSeries(32, 32, 0, 128, 6, idleFrames);

            walkSwitch = true;

        }

        public virtual void PhantomFlight()
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

        public virtual void PhantomSweep()
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

        public void PhantomSpecial()
        {

            cooldownInterval = 180;

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

        public virtual void PhantomWeapon()
        {
            weaponRender = new();

            weaponRender.LoadWeapon(WeaponRender.weapons.cutlass);

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

            Color fadeout = Color.White * fadeFactor;

            Color jacket = new Color(142,96,100) * fadeFactor;

            switch (netScheme.Value)
            {
                case 0:
                    jacket = new Color(151, 69, 33) * fadeFactor;
                    break;
                case 1:
                    jacket = new Color(92, 86, 109) * fadeFactor;
                    break;
                case 2:
                    jacket = new Color(97, 83, 100) * fadeFactor;
                    break;
                case 3:
                    jacket = new Color(56, 75, 144) * fadeFactor; 
                    break;
                case 4:
                    jacket = new Color(144, 75, 56) * fadeFactor;
                    break;
            }

            Rectangle main = walkFrames[netDirection.Value][walkFrame];

            bool mirrored = netDirection.Value % 2 == 0 && netAlternative.Value == 3;

            if (netSweepActive.Value)
            {

                main = sweepFrames[netDirection.Value][sweepFrame];

                if (netDirection.Value == 3) { mirrored = true; }

                weaponRender.DrawWeapon(b, spritePosition, drawLayer, new() { scale = (spriteScale / 4f), source = sweepFrames[netDirection.Value][sweepFrame] });

            }
            else if (netSpecialActive.Value)
            {

                main = specialFrames[netDirection.Value][specialFrame];

            }
            else if (netFlightActive.Value)
            {

                int setFlightSeries = netDirection.Value + (netFlightProgress.Value * 4);

                int setFlightFrame = Math.Min(flightFrame, (flightFrames[setFlightSeries].Count - 1));

                main = flightFrames[setFlightSeries][setFlightFrame];
            }
            else if (netSmashActive.Value)
            {

                int setFlightSeries = netDirection.Value + (netFlightProgress.Value * 4);

                int setFlightFrame = Math.Min(flightFrame, (smashFrames[setFlightSeries].Count - 1));

                main = smashFrames[setFlightSeries][setFlightFrame];

                if (netDirection.Value == 3) { mirrored = true; }

                weaponRender.DrawWeapon(b, spritePosition,  drawLayer, new() { scale = (spriteScale / 4f), source = smashFrames[setFlightSeries][setFlightFrame] });

            }
            else if (netHaltActive.Value)
            {

                main = idleFrames[netDirection.Value][0];
            }


            b.Draw(characterTexture, spritePosition, main, fadeout, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, mirrored ? (SpriteEffects)1 : 0, drawLayer);

            main.X += 256;

            b.Draw(characterTexture, spritePosition, main, jacket, 0.0f, new Vector2(0.0f, 0.0f), spriteScale, mirrored ? (SpriteEffects)1 : 0, drawLayer);

            b.Draw(Game1.shadowTexture, localPosition + new Vector2(10, 44f), new Rectangle?(Game1.shadowTexture.Bounds), fadeout, 0.0f, Vector2.Zero, 4f, 0, drawLayer - 1E-06f);

            DrawHat(b, spritePosition, (spriteScale / 4f), drawLayer);

        }

        public virtual void DrawHat(SpriteBatch b, Vector2 spritePosition, float scale, float drawLayer)
        {

            int hatTile = 337;

            switch (netScheme.Value)
            {
                case 0:
                    hatTile = 0;
                    break;
                case 1:
                    hatTile = 155;
                    break;
                case 2:
                    hatTile = 242;
                    break;
                case 3:
                    hatTile = 292;
                    break;
                case 4:
                    hatTile = 3;
                    break;
            }

            int hatOffset = 5;

            switch (netDirection.Value)
            {
                case 0:

                    hatTile += 36;

                    break;

                case 1:

                    hatTile += 12;

                    hatOffset = 0;

                    break;

                case 3:

                    hatTile += 24;

                    hatOffset = 0;

                    break;


            }

            b.Draw(
                hatsTexture,
                spritePosition + new Vector2(18*scale,(0 - (32 + hatOffset)) *scale),
                Game1.getSourceRectForStandardTileSheet(hatsTexture, hatTile, 20, 20),
                Color.White * fadeFactor,
                0f,
                Vector2.Zero,
                4.5f * scale,
                flip ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                drawLayer + 0.0001f
            );

        }


        public override bool PerformSpecial(Vector2 target)
        {

            base.PerformSpecial(target);

            SpellHandle missile = new(currentLocation, target, Position - new Vector2(64), 192, GetThreat(), Mod.instance.CombatDamage());

            missile.type = SpellHandle.spells.missile;

            missile.projectile = 3;

            missile.missile = missiles.cannonball;

            missile.display = IconData.impacts.bomb;

            missile.indicator = IconData.cursors.death;

            missile.sound = sounds.flameSpellHit;

            Mod.instance.spellRegister.Add(missile);

            return true;

        }

    }

}

