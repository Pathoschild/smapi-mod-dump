/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MolsonCAD/DeluxeJournal
**
*************************************************/

namespace DeluxeJournal.Task
{
    /// <summary>Flags for enabling smart icons in the task menu.</summary>
    [Flags]
    public enum SmartIconFlags
    {
        /// <summary>Do not show any icons.</summary>
        None = 0,

        /// <summary>Show the item icon.</summary>
        Item = 1 << 0,

        /// <summary>Show building icon.</summary>
        Building = 1 << 2,

        /// <summary>Show the farm animal icon.</summary>
        Animal = 1 << 3,

        /// <summary>Show the NPC icon.</summary>
        Npc = 1 << 4,

        /// <summary>Show all icons.</summary>
        All = ~(-1 << 5)
    }
}
