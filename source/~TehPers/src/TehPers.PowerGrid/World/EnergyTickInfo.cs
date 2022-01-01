/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TehPers/StardewValleyMods
**
*************************************************/

using TehPers.PowerGrid.Units;

namespace TehPers.PowerGrid.World
{
    /// <summary>
    /// Information about the energy state of a conductor.
    /// </summary>
    /// <param name="Satisfaction">The power satisfaction available to the conductor.</param>
    public record EnergyTickInfo(Power Satisfaction);
}