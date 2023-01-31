/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace AtraShared.Utils;

public static class FarmerHelpers
{
    public static IEnumerable<Farmer> GetFarmers()
    => Game1.getAllFarmers().Where(f => f is not null);

    public static bool HasAnyFarmerRecievedFlag(string flag)
    {
        foreach (Farmer farmer in GetFarmers())
        {
            if (farmer.hasOrWillReceiveMail(flag))
            {
                return true;
            }
        }

        return false;
    }
}