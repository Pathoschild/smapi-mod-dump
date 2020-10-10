/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using Revitalize.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Revitalize.Resources.DataNodes
{
    public class SpellFunctionDataNode
    {
     public Spell.spellFunction spellToCast;
     public int timesToCast;


        public SpellFunctionDataNode(Spell.spellFunction SpellToCast, int numberOfCasts)
        {
            spellToCast = SpellToCast;
            timesToCast = numberOfCasts;

        }
    }
}
