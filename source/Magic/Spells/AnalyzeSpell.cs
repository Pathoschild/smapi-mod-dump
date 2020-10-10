/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/Magic
**
*************************************************/

using Magic.Schools;
using SpaceCore;
using SpaceShared;
using StardewValley;

namespace Magic.Spells
{
    public class AnalyzeSpell : Spell
    {
        public AnalyzeSpell() : base(SchoolId.Arcane, "analyze")
        {
        }

        public override int getManaCost(Farmer player, int level)
        {
            return 0;
        }

        public override int getMaxCastingLevel()
        {
            return 1;
        }

        public override IActiveEffect onCast(Farmer player, int level, int targetX, int targetY)
        {
            if (Magic.OnAnalyzeCast != null)
                Util.invokeEvent< AnalyzeEventArgs >("OnAnalyzeCast", Magic.OnAnalyzeCast.GetInvocationList(), player, new AnalyzeEventArgs(targetX, targetY));

            return null;
        }
    }
}
