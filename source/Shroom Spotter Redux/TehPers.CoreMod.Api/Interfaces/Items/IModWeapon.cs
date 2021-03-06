/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace TehPers.CoreMod.Api.Items {
    public interface IModWeapon : IModItem {
        /// <summary>Gets the raw information that should be added to "Data/weapons".</summary>
        /// <returns>The raw information string.</returns>
        string GetRawWeaponInformation();
    }
}