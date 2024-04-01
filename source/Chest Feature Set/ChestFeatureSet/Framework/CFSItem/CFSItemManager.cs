/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zack20136/ChestFeatureSet
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace ChestFeatureSet.Framework.CFSItem
{
    /// <summary>
    /// SaveCFSItem Manager
    /// </summary>
    public class CFSItemManager
    {
        private readonly CFSItemController CFSItemController;

        private readonly ModEntry modEntry;

        private readonly string savePath;

        private bool IsFileLoaded { get; set; } = false;

        public CFSItemManager(CFSItemController CFSItemController, ModEntry modEntry, string saveFileName)
        {
            this.CFSItemController = CFSItemController;

            this.modEntry = modEntry;

            this.savePath = $"saveData\\" + Constants.SaveFolderName + "\\" + saveFileName;
        }

        /// <summary>
        /// Method that will be executed once the save is loaded.
        /// Will load the data and update the <see cref="CFSItemController"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (this.IsFileLoaded)
                return;

            var SaveCFSItem = this.modEntry.Helper.Data.ReadJsonFile<SaveCFSItem>(this.savePath) ?? new SaveCFSItem(new List<CFSItem>());
            try
            {
                this.CFSItemController.SetItems(CFSItemToItem(SaveCFSItem.Items));
                this.IsFileLoaded = true;
            }
            catch (InvalidOperationException exception)
            {
                this.modEntry.Monitor.Log("Something went wrong with loading the save file", LogLevel.Info);
                this.modEntry.Monitor.Log(exception.ToString(), LogLevel.Error);
            }
        }

        /// <summary>
        /// Method that will be called before saving.
        /// Will load the data from <see cref="CFSItemController"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSaving(object sender, SavingEventArgs e)
        {
            var items = this.CFSItemController.GetItems();
            var CFSItems = ItemToCFSItem(items);

            this.modEntry.Helper.Data.WriteJsonFile(savePath, new SaveCFSItem(CFSItems));
        }

        /// <summary>
        /// Parse a list of item into a ItemId and Quality.
        /// </summary>
        /// <param name="items">The items from the ItemController</param>
        /// <returns></returns>
        private static IEnumerable<CFSItem> ItemToCFSItem(IEnumerable<Item> items)
        {
            var itemList = items.ToList();

            foreach (var item in Game1.player.Items)
            {
                if (itemList.Contains(item))
                    yield return new CFSItem(item.ItemId, item.Quality);
            }
        }

        /// <summary>
        /// Will translate the CFSItem data to Item data.
        /// </summary>
        /// <param name="CFSItem"></param>
        /// <returns></returns>
        private static IEnumerable<Item> CFSItemToItem(IEnumerable<CFSItem> CFSItem)
        {
            var CFSItemList = CFSItem.ToList();

            foreach (var item in Game1.player.Items)
            {
                if (item == null)
                    continue;

                if (CFSItemList.Any(i => i.ItemId.Equals(item.ItemId) && i.Quality.Equals(item.Quality)))
                    yield return item;
            }
        }
    }
}
