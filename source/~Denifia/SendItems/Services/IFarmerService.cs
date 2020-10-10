/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Denifia/StardewMods
**
*************************************************/

using System.Collections.Generic;
using Denifia.Stardew.SendItems.Domain;

namespace Denifia.Stardew.SendItems.Services
{
    public interface IFarmerService
    {
        Domain.Farmer CurrentFarmer { get; }
        void LoadCurrentFarmer();
        List<Domain.Farmer> GetFarmers();
        bool AddFriendToCurrentPlayer(Friend friend);
        bool RemoveFriendFromCurrentPlayer(string id);
        bool RemoveAllFriendFromCurrentPlayer();
    }
}
