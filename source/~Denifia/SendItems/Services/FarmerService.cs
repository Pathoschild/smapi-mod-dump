using System;
using System.Collections.Generic;
using System.Linq;
using Denifia.Stardew.SendItems.Domain;
using StardewModdingAPI.Events;
using StardewValley;

namespace Denifia.Stardew.SendItems.Services
{
    public class FarmerService : IFarmerService
    {
        private readonly IConfigurationService _configService;

        private Domain.Farmer _currentFarmer;
        public Domain.Farmer CurrentFarmer
        {
            get
            {
                if (_currentFarmer == null)
                {
                    LoadCurrentFarmer();
                }
                return _currentFarmer;
            }
        }

        public FarmerService(IConfigurationService configService, IModEvents events)
        {
            _configService = configService;
            events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            _currentFarmer = null;
        }

        public void LoadCurrentFarmer()
        {
            var saves = _configService.GetSavedGames();
            var save = saves.FirstOrDefault(x => x.Name == Game1.player.Name && x.FarmName == Game1.player.farmName?.Value);
            if (save == null)
            {
                // Happens during a new game creation
                return;
                //throw new System.Exception("error loading current farmer");
            }


            var newFarmer = new Domain.Farmer()
            {
                // TODO: Set the id to the save folder name
                Id = save.Id,
                Name = save.Name,
                FarmName = save.FarmName
            };

            var existingFarmer = GetFarmerById(newFarmer.Id);

            if (existingFarmer != null)
            {
                _currentFarmer = existingFarmer;
                return;
            }

            SaveFarmer(newFarmer);
            _currentFarmer = newFarmer;
        }

        public List<Domain.Farmer> GetFarmers()
        {
            return Repository.Instance.Fetch<Domain.Farmer>();
        }

        public bool AddFriendToCurrentPlayer(Friend friend)
        {
            if (CurrentFarmer == null) throw new System.Exception("current farmer is unknown");
            if (!_currentFarmer.Friends.Contains(friend))
            {
                _currentFarmer.Friends.Add(friend);
                return Repository.Instance.Update(_currentFarmer);
            }
            return false;
        }

        public bool RemoveFriendFromCurrentPlayer(string id)
        {
            if (CurrentFarmer == null) throw new System.Exception("current farmer is unknown");

            var index = _currentFarmer.Friends.FindIndex(x => x.Id == id);
            if (index < 0) return false;

            _currentFarmer.Friends.RemoveAt(index);
            return Repository.Instance.Update(_currentFarmer);
        }

        public bool RemoveAllFriendFromCurrentPlayer()
        {
            if (CurrentFarmer == null) throw new System.Exception("current farmer is unknown");
            _currentFarmer.Friends.Clear();
            return Repository.Instance.Update(_currentFarmer);
        }

        private void SaveFarmer(Domain.Farmer farmer)
        {
            Repository.Instance.Upsert(farmer);
        }

        private Domain.Farmer GetFarmerById(string id)
        {
            return Repository.Instance.SingleOrDefault<Domain.Farmer>(x => x.Id == id);
        }
    }
}
