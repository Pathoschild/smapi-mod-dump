/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System;
using Magic.Framework.Game;
using StardewValley;

namespace Magic.Framework.Spells
{
    internal class ProjectileSpell : Spell
    {
        public int ManaBase { get; }
        public int DamageBase { get; }
        public int DamageIncr { get; }
        public string Sound { get; }
        public string SoundHit { get; }
        public bool Seeking { get; }

        public ProjectileSpell(string school, string id, int manaBase, int dmgBase, int dmgIncr)
            : this(school, id, manaBase, dmgBase, dmgIncr, null, null) { }

        public ProjectileSpell(string school, string id, int manaBase, int dmgBase, int dmgIncr, string snd, string sndHit, bool seeking = false)
            : base(school, id)
        {
            this.ManaBase = manaBase;
            this.DamageBase = dmgBase;
            this.DamageIncr = dmgIncr;
            this.Sound = snd;
            this.SoundHit = sndHit;
            this.Seeking = seeking;
        }

        public override int GetManaCost(Farmer player, int level)
        {
            return this.ManaBase;
        }

        public override IActiveEffect OnCast(Farmer player, int level, int targetX, int targetY)
        {
            int dmg = (this.DamageBase + this.DamageIncr * level) * (player.CombatLevel + 1);
            float dir = (float)-Math.Atan2(player.getStandingY() - targetY, targetX - player.getStandingX());
            player.currentLocation.projectiles.Add(new SpellProjectile(player, this, dmg, dir, 4f + 3 * level, this.Seeking));
            if (this.Sound != null)
                player.LocalSound(this.Sound);

            return null;
        }
    }
}
