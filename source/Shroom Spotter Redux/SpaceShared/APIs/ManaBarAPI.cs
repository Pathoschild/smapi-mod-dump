/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/strobel1ght/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace SpaceShared.APIs
{
    public interface ManaBarAPI
    {
        int GetMana( Farmer farmer );
        void AddMana( Farmer farmer, int amt );

        int GetMaxMana( Farmer farmer );
        void SetMaxMana( Farmer farmer, int newMaxMana );
    }
}
