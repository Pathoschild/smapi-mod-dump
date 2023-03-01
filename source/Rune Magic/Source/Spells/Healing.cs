/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using SpaceCore;
using StardewValley;

namespace RuneMagic.Source.Spells
{
    public class Healing : Spell
    {
        public Healing() : base(School.Abjuration)
        {
            Description += "Restores the caster's health.";
            Level = 1;
        }

        public override bool Cast()
        {
            if (Game1.player.health >= Game1.player.maxHealth)
                return false;
            var heal = Skill.Level * 10;
            if (heal > Game1.player.maxHealth - Game1.player.health)
                heal = Game1.player.maxHealth - Game1.player.health;
            Game1.player.health += heal;
            return base.Cast();
        }
    }
}