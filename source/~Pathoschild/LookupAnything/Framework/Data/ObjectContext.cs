/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

using System;

namespace Pathoschild.Stardew.LookupAnything.Framework.Data
{
    /// <summary>The context in which to override an object.</summary>
    [Flags]
    internal enum ObjectContext
    {
        /// <summary>Objects in the world.</summary>
        World = 1,

        /// <summary>Objects in an item inventory.</summary>
        Inventory = 2,

        /// <summary>Objects in any context.</summary>
        Any = ObjectContext.World | ObjectContext.Inventory
    }
}
