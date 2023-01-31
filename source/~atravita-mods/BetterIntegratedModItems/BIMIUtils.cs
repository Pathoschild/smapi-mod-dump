/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

using StardewValley.Locations;

namespace BetterIntegratedModItems;
internal static class BIMIUtils
{
    /// <summary>
    /// Whether or not it's a location the game saves.
    /// </summary>
    /// <param name="loc">Game location.</param>
    /// <returns>true if it's not persisted.</returns>
    /// <remarks>Carries the no-inlining flag so other mods can patch this if necessary.</remarks>
    [MethodImpl(MethodImplOptions.NoInlining)]
    internal static bool IsUnsavedLocation(this GameLocation loc)
        => loc is MineShaft or VolcanoDungeon || loc.NameOrUniqueName.StartsWith("DeepWoods_");
}
