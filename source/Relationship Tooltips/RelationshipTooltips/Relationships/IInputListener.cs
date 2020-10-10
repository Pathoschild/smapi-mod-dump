/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/M3ales/RelationshipTooltips
**
*************************************************/

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
