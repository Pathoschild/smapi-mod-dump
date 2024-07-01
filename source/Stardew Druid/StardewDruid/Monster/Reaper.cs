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
using StardewDruid.Render;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Reflection;
using xTile.Dimensions;

namespace StardewDruid.Monster
{
    public class Reaper : DarkRogue
    {

        public NetBool netShieldActive = new NetBool(false);
        public int shieldTimer;

        public Reaper()
        {


        }

        public Reaper(Vector2 vector, int CombatModifier, string name = "Reaper")
          : base(vector, CombatModifier, name)
        {

        }


        protected override void initNetFields()
        {
            
            base.initNetFields();

            NetFields.AddField(netShieldActive, "netShieldActive");

        }

        public override void LoadOut()
        {

            baseMode = 3;

            baseJuice = 4;
            
            basePulp = 50;

            cooldownInterval = 120;

            DarkWalk();

            DarkFlight();

            DarkCast();

            DarkSmash();

            DarkSword();

            weaponRender = new();

            weaponRender.LoadWeapon(WeaponRender.weapons.scythe);

            overHead = new(16, -144);

            loadedOut = true;

        }

        public override void draw(SpriteBatch b, float alpha = 1f)
        {

            base.draw(b, alpha);

            if (netShieldActive.Value)
            {

                if (IsInvisible || !Utility.isOnScreen(Position, 128))
                {
                    return;
                }

                Vector2 localPosition = getLocalPosition(Game1.viewport);

                float drawLayer = (float)StandingPixel.Y / 10000f;

                int netScale = netMode.Value > 5 ? netMode.Value - 4 : netMode.Value;

                float spriteScale = 3.25f + (0.25f * netScale);

                Vector2 spritePosition = localPosition - new Vector2(20 + (netScale * 4), 40f + (netScale * 8) + flightHeight) - (new Vector2(8) * spriteScale);

                b.Draw(
                    Mod.instance.iconData.shieldTexture,
                    spritePosition,
                    new Microsoft.Xna.Framework.Rectangle(0, 0, 48, 48),
                    Mod.instance.iconData.schemeColours[IconData.schemes.death] * 0.2f,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    0,
                    drawLayer+0.0004f
                );

                int sparkle= (int)(Game1.currentGameTime.TotalGameTime.TotalMilliseconds % 1000) / 200;

                b.Draw(
                    Mod.instance.iconData.shieldTexture,
                    spritePosition,
                    new Microsoft.Xna.Framework.Rectangle(48 + (48 * sparkle),0,48,48),
                    Color.White * 0.6f,
                    0f,
                    Vector2.Zero,
                    spriteScale,
                    0,
                    drawLayer + 0.0005f
                );

            }

        }


        public override int takeDamage(int damage, int xTrajectory, int yTrajectory, bool isBomb, double addedPrecision, Farmer who)
        {

            if (netShieldActive.Value)
            {

                return 0;

            }

            return base.takeDamage(damage, xTrajectory, yTrajectory, isBomb, addedPrecision, who);

        }

        public override void update(GameTime time, GameLocation location)
        {

            base.update(time, location);

            if (shieldTimer > 0)
            {
                shieldTimer--;
            }

            if(netShieldActive.Value)
            { 
                if(shieldTimer <= 300)
                {

                    netShieldActive.Set(false);

                }
            }

        }

        public override bool PerformSpecial(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval;

            netSpecialActive.Set(true);

            SetCooldown(1);

            if(Mod.instance.randomIndex.Next(2) == 0 && !netShieldActive.Value && shieldTimer <= 0)
            {

                netShieldActive.Set(true);

                shieldTimer = 600;

                return true;

            }

            SpellHandle fireball = new(currentLocation, target, GetBoundingBox().Center.ToVector2(), 256, GetThreat());

            fireball.type = SpellHandle.spells.missile;

            fireball.projectile = 4;

            fireball.missile = IconData.missiles.death;

            fireball.display = IconData.impacts.death;

            fireball.boss = this;

            fireball.added = new() { SpellHandle.effects.capture,};

            Mod.instance.spellRegister.Add(fireball);

            return true;

        }

        public override bool PerformChannel(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval;

            netChannelActive.Set(true);
            
            SetCooldown(2);

            int offset = Mod.instance.randomIndex.Next(2);

            for (int i = 0; i < 4; i++)
            {
                
                List<Vector2> castSelection = ModUtility.GetTilesWithinRadius(currentLocation, ModUtility.PositionToTile(Position), Mod.instance.randomIndex.Next(4,6), true, (i*2) + offset % 8);
                
                if (castSelection.Count > 0)
                {

                    Vector2 tryVector = castSelection[Mod.instance.randomIndex.Next(castSelection.Count)];

                    SpellHandle fireball = new(currentLocation, target, GetBoundingBox().Center.ToVector2(), 256, GetThreat());

                    fireball.type = SpellHandle.spells.ballistic;

                    fireball.projectile = 4;

                    fireball.missile = IconData.missiles.death;

                    fireball.display = IconData.impacts.death;

                    fireball.indicator = IconData.cursors.death;

                    fireball.boss = this;

                    fireball.added = new() { SpellHandle.effects.capture, };

                    Mod.instance.spellRegister.Add(fireball);

                }
            
            }

            return true;

        }

    }

}

