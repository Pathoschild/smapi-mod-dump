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
