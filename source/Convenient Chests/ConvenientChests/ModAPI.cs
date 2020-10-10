/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

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
