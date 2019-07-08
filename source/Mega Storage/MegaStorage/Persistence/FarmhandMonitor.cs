using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace MegaStorage.Persistence
{
    public class FarmhandMonitor
    {
        public event Action OnPlayerAdded;
        public event Action OnPlayerRemoved;

        private readonly IModHelper _modHelper;
        private readonly IMonitor _monitor;

        private int _prevLength;

        public FarmhandMonitor(IModHelper modHelper, IMonitor monitor)
        {
            _modHelper = modHelper;
            _monitor = monitor;
            _prevLength = 0;
        }

        public void Start()
        {
            _modHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            var currentLength = Game1.otherFarmers.Count;
            if (currentLength > _prevLength)
            {
                _monitor.VerboseLog("Player added");
                OnPlayerAdded?.Invoke();
                _prevLength = currentLength;
            }
            else if (currentLength < _prevLength)
            {
                _monitor.VerboseLog("Player removed");
                OnPlayerRemoved?.Invoke();
                _prevLength = currentLength;
            }
        }
    }
}