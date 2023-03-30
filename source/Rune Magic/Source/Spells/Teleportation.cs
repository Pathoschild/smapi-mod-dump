/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/facufierro/RuneMagic
**
*************************************************/

using StardewValley;

namespace RuneMagic.Source.Spells
{
    public class Teleportation : Spell
    {
        public Teleportation() : base(School.Alteration)
        {
            Description += "Teleports the caster home.";
            Level = 5;
        }

        public override bool Cast()
        {
            Game1.warpFarmer("FarmHouse", 4, 3, false);
            return base.Cast();
        }
    }
}