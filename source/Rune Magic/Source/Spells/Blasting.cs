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
using static SpaceCore.Skills;

namespace RuneMagic.Source.Spells
{
    public class Blasting : Spell
    {
        public Blasting() : base(School.Evocation)

        {
            Description += "Creates an explosion at a target location.";
            Level = 3;
        }

        public override bool Cast()
        {
            Target = Game1.currentCursorTile;
            var radius = 1 + (Skill.Level - 4) / 6;
            Game1.currentLocation.explode(Target, radius, Game1.player);
            return base.Cast();
        }
    }
}