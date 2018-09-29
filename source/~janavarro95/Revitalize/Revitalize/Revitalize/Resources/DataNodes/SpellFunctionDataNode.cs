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
