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

namespace StardewDruid.Monster
{
    public class DarkShooter : DarkRogue
    {

        public DarkShooter()
        {


        }

        public DarkShooter(Vector2 vector, int CombatModifier, string name = "DarkShooter")
          : base(vector, CombatModifier, name)
        {

        }

        public override void LoadOut()
        {

            baseMode = 2;

            baseJuice = 4;

            basePulp = 20;

            cooldownInterval = 180;

            DarkWalk();

            DarkFlight();

            DarkBlast();

            DarkBrawl();

            weaponRender = new();

            weaponRender.melee = false;

            weaponRender.LoadWeapon(WeaponRender.weapons.bazooka);

            overHead = new(16, -144);

            loadedOut = true;

        }

        public override void SetMode(int mode)
        {
            
            base.SetMode(mode);

            tempermentActive = temperment.ranged;

        }

        public override bool PerformSpecial(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval * 2;

            netSpecialActive.Set(true);

            SetCooldown(1);

            SpellHandle fireball = new(currentLocation, target, GetBoundingBox().Center.ToVector2(), 192, GetThreat());

            fireball.type = SpellHandle.spells.missile;

            fireball.projectile = 2;

            fireball.missile = IconData.missiles.cannonball;

            fireball.display = IconData.impacts.impact;

            fireball.boss = this;

            fireball.indicator = IconData.cursors.scope;

            fireball.scheme = IconData.schemes.ember;

            fireball.added = new() { SpellHandle.effects.embers, };

            Mod.instance.spellRegister.Add(fireball);

            return true;

        }

        public override bool PerformChannel(Vector2 target)
        {

            specialTimer = (specialCeiling + 1) * specialInterval * 2;

            netChannelActive.Set(true);

            SetCooldown(2);

            SpellHandle fireball = new(currentLocation, target, GetBoundingBox().Center.ToVector2(), 256, GetThreat());

            fireball.type = SpellHandle.spells.ballistic;

            fireball.projectile = 3;

            fireball.missile = IconData.missiles.cannonball;

            fireball.display = IconData.impacts.impact;

            fireball.boss = this;

            fireball.indicator = IconData.cursors.scope;

            fireball.scheme = IconData.schemes.ember;

            fireball.added = new() { SpellHandle.effects.embers, };

            Mod.instance.spellRegister.Add(fireball);

            return true;

        }

    }

}

