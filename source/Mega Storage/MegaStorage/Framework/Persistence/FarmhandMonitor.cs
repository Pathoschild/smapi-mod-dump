using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace MegaStorage.Framework.Persistence
{
    public class FarmhandMonitor
    {
        public event Action OnPlayerAdded;
        public event Action OnPlayerRemoved;

        private int _prevLength;

        public FarmhandMonitor()
        {
            _prevLength = 0;
        }

        public void Start()
        {
            MegaStorageMod.ModHelper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            var currentLength = Game1.otherFarmers.Count;
            if (currentLength > _prevLength)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Player added");
                OnPlayerAdded?.Invoke();
                _prevLength = currentLength;
            }
            else if (currentLength < _prevLength)
            {
                MegaStorageMod.ModMonitor.VerboseLog("Player removed");
                OnPlayerRemoved?.Invoke();
                _prevLength = currentLength;
            }
        }
    }
}