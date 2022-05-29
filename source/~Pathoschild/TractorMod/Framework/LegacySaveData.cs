/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pathoschild/StardewMods
**
*************************************************/

namespace Pathoschild.Stardew.TractorMod.Framework
{
    /// <summary>Contains legacy data that's stored in the save file.</summary>
    /// <param name="Buildings">The custom buildings to save.</param>
    internal record LegacySaveData(LegacySaveDataBuilding[] Buildings);
}
