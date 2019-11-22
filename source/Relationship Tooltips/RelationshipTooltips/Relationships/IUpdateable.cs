using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RelationshipTooltips.Relationships
{
    public interface IUpdateable
    {
        Action<Character, Item> OnTick { get; }
        Action<Character, Item> OnQuaterSecondTick { get; }
    }
}
