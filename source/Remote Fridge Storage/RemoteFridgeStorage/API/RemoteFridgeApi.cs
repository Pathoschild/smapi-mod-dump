using System.Collections.Generic;
using StardewValley;
using StardewValley.Objects;

namespace RemoteFridgeStorage.API
{
    public class RemoteFridgeApi : IRemoteFridgeApi
    {
        private readonly FridgeHandler _handler;

        public RemoteFridgeApi(FridgeHandler handler)
        {
            _handler = handler;
        }

        public IList<Item> Fridge()
        {
            return _handler.FridgeList;
        }

        public void UseCustomCraftingMenu(bool enabled = false)
        {
            _handler.MenuEnabled = enabled;
        }

        public void UpdateFridgeContents()
        {
            _handler.UpdateFridgeContents();
        }

        public bool IsFridgeChest(Chest chest)
        {
            return _handler.Chests.Contains(chest);
        }

        public bool AddFridgeChest(Chest chest)
        {
            return _handler.Chests.Add(chest);
        }

        public bool RemoveFridgeChest(Chest chest)
        {
            return _handler.Chests.Remove(chest);
        }
    }
}