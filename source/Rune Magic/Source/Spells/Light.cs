/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using RuneMagic.Source.Effects;
using StardewValley;

namespace RuneMagic.Source.Spells
{
    public class Light : Spell
    {
        public Light() : base(School.Conjuration)
        {
            School = School.Conjuration;
            Description += "Conjures a torch at a target location.";
            Level = 2;
        }

        public override bool Cast()
        {
            var target = Game1.currentCursorTile;
            if (Game1.currentLocation.objects.ContainsKey(target))
                return base.Cast();
            Effect = new Lighted(this, target);
            return true;
        }
    }
}