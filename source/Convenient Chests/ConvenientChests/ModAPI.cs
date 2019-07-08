using System;
using ConvenientChests.CategorizeChests.Framework;
using StardewModdingAPI;
using StardewValley.Objects;

namespace ConvenientChests {
    public class ModAPI {
        public void CopyChestData(Chest source, Chest target) {
            IChestDataManager chestDataManager = ModEntry.CategorizeChests.ChestDataManager;
            ChestData sourceData = chestDataManager.GetChestData(source);
            ChestData targetData = chestDataManager.GetChestData(target);
            targetData.AcceptedItemKinds.Clear();
            foreach (var itemKey in sourceData.AcceptedItemKinds) {
                targetData.AddAccepted(itemKey);
            }
        }
    }
}
