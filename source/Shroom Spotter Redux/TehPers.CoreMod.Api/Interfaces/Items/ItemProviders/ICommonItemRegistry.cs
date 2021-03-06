/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

namespace TehPers.CoreMod.Api.Items.ItemProviders {
    public interface ICommonItemRegistry {
        /// <summary>Item provider for simple objects, like the ones that can be found in "Maps/springobjects".</summary>
        IItemRegistry<IModObject> Objects { get; }

        /// <summary>Item provider for weaopns, like the ones that can be found in "TileSheets/weapons".</summary>
        IItemRegistry<IModWeapon> Weapons { get; }
    }
}