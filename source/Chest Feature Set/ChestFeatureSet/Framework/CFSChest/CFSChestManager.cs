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
using StardewValley.Objects;

namespace ChestFeatureSet.Framework.CFSChest
{
    /// <summary>
    /// SaveCFSChest Manager
    /// </summary>
    public class CFSChestManager
    {
        private readonly CFSChestController CFSChestController;

        private readonly ModEntry modEntry;

        private readonly string savePath;

        private bool IsFileLoaded { get; set; } = false;

        public CFSChestManager(CFSChestController CFSChestController, ModEntry modEntry, string saveFileName)
        {
            this.CFSChestController = CFSChestController;

            this.modEntry = modEntry;

            this.savePath = $"saveData\\" + Constants.SaveFolderName + "\\" + saveFileName;
        }

        /// <summary>
        /// Method that will be executed once the save is loaded.
        /// Will load the data and update the <see cref="CFSChestController"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            if (this.IsFileLoaded)
                return;

            var SaveCFSChest = this.modEntry.Helper.Data.ReadJsonFile<SaveCFSChest>(this.savePath) ?? new SaveCFSChest(new List<CFSChest>());
            try
            {
                this.CFSChestController.SetChests(CFSChestToChest(SaveCFSChest.Chests));
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
        /// Will load the data from <see cref="CFSChestController"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnSaving(object sender, SavingEventArgs e)
        {
            var chests = this.CFSChestController.GetChests();
            var CFSChests = ChestToCFSChest(chests);

            this.modEntry.Helper.Data.WriteJsonFile(savePath, new SaveCFSChest(CFSChests));
        }

        /// <summary>
        /// Parse a list of chest into a tile position and location name.
        /// </summary>
        /// <param name="chests">The chests from the ChestHandler</param>
        /// <returns></returns>
        private static IEnumerable<CFSChest> ChestToCFSChest(IEnumerable<Chest> chests)
        {
            var chestList = chests.ToList();

            foreach (var chestLocationPair in ChestExtension.GetAllChests())
            {
                var chest = chestLocationPair.Chest;
                var location = chestLocationPair.Location;
                if (chestList.Contains(chest))
                    yield return new CFSChest(chest.TileLocation, location.uniqueName.Value ?? location.Name);
            }
        }

        /// <summary>
        /// Will translate the data from the loaded file by comparing the locations to existing chests and returning the existing chests.
        /// </summary>
        /// <param name="CFSChest"></param>
        /// <returns></returns>
        private static IEnumerable<Chest> CFSChestToChest(IEnumerable<CFSChest> CFSChest)
        {
            var CFSChestList = CFSChest.ToList();

            foreach (var chestLocationPair in ChestExtension.GetAllChests())
            {
                var chest = chestLocationPair.Chest;
                var location = chestLocationPair.Location;
                if (CFSChestList.Any(data =>
                    data.ChestTileLocation.Equals(chest.TileLocation) &&
                    data.LocationName.Equals(location.uniqueName.Value ?? location.Name)))
                {
                    yield return chest;
                }
            }
        }
    }
}
