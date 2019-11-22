using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI.Events;
using StardewValley;

namespace RelationshipTooltips.Relationships
{
    public interface IInputListener
    {
        Action<Character, Item, EventArgsInput> ButtonPressed { get; }
        Action<Character, Item, EventArgsInput> ButtonReleased { get; }
    }
}
