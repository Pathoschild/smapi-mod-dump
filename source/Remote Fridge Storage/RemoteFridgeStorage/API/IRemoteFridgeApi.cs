using System.Collections.Generic;
using StardewValley;
using StardewValley.Objects;

namespace RemoteFridgeStorage.API
{
    /// <summary>
    /// API For getting the list with items 
    /// </summary>
    public interface IRemoteFridgeApi
    {
        IList<Item> Fridge();
        
        void UseCustomCraftingMenu(bool enabled = false);

        void UpdateFridgeContents();

        bool IsFridgeChest(Chest chest);

        bool AddFridgeChest(Chest chest);

        bool RemoveFridgeChest(Chest chest);
    }
}