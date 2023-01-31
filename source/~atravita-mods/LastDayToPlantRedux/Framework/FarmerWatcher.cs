/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

using Netcode;

namespace LastDayToPlantRedux.Framework;

/// <summary>
/// Used to watch events on farmer.
/// Make sure that the only references to it is on farmer.
/// </summary>
internal class FarmerWatcher
{
    private const int PRESTIGED = Farmer.agriculturist + 100;

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used to listen to an event.")]
    internal void Professions_OnArrayReplaced(NetList<int, NetInt> list, IList<int> before, IList<int> after)
    {
        if (before.Contains(Farmer.agriculturist) != after.Contains(Farmer.agriculturist)
                || before.Contains(PRESTIGED) != after.Contains(PRESTIGED))
        {
            CropAndFertilizerManager.RequestReset();
        }
    }

    [SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "Used to listen to an event.")]
    internal void Professions_OnElementChanged(NetList<int, NetInt> list, int index, int oldValue, int newValue)
    {
        if (oldValue == Farmer.agriculturist || newValue == Farmer.agriculturist || oldValue == PRESTIGED || newValue == PRESTIGED)
        {
            CropAndFertilizerManager.RequestReset();
        }
    }
}
