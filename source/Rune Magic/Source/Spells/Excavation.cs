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
using StardewValley.Locations;
using System.Linq;

namespace RuneMagic.Source.Spells
{
    public class Excavation : Spell
    {
        public Excavation() : base(School.Conjuration)
        {
            Description += "The caster digs a hole at a target location.";
            Level = 5;
        }

        //public override bool Cast()
        //{
        //    if (!RuneMagic.PlayerStats.ActiveEffects.OfType<Hastened>().Any())
        //    {
        //        Effect = new ObjectSummoned(this, new Object(Game1.currentCursorTile, RuneMagic.JsonAssetsApi.GetBigCraftableId("Spell Excavation")));
        //        return base.Cast();
        //    }
        //    else
        //        return false;
        //}
    }
}