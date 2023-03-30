/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SoapStuff/Remote-Fridge-Storage
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Objects;

namespace RemoteFridgeStorage.controller.saving
{
    /// <summary>
    /// Handling of saveData
    /// </summary>
    public class SaveManager
    {
        private readonly ChestController _chestController;

        public SaveManager(ChestController chestController)
        {
            _chestController = chestController;
        }

        /// <summary>
        /// Method that will be executed once the save is loaded.
        /// Will load the data and update the <see cref="ChestController"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            var savePath = $"save_data/{Constants.SaveFolderName}.json";
            var helperData = ModEntry.Instance.Helper.Data;
            var chestData = helperData.ReadJsonFile<SaveData>(savePath) ?? new SaveData(new List<ChestData>());

            try
            {
                _chestController.SetChests(FromChestData(chestData.Chests));
            }
            catch (InvalidOperationException exception)
            {
                ModEntry.Instance.Monitor.Log("Something went wrong with loading the save file", LogLevel.Info);
                ModEntry.Instance.Monitor.Log(exception.ToString(), LogLevel.Error);
            }
        }

        /// <summary>
        /// Method that will be called before saving.
        /// Will load the data from <see cref="ChestController"/>
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Saving(object sender, SavingEventArgs e)
        {
            var chests = _chestController.GetChests();
            var chestData = ToChestData(chests);
            ModEntry.Instance.Helper.Data.WriteJsonFile($"save_data/{Constants.SaveFolderName}.json",
                new SaveData(chestData));
        }

        /// <summary>
        /// Parse a list of chest into a tile position and location name.
        /// </summary>
        /// <param name="chests">The chests from the ChestHandler</param>
        /// <returns></returns>
        private static IEnumerable<ChestData> ToChestData(IEnumerable<Chest> chests)
        {
            var chestList = chests.ToList();

            foreach (var chestLocationPair in Util.GetAllChests())
            {
                var chest = chestLocationPair.Chest;
                var location = chestLocationPair.Location;
                if (chestList.Contains(chest))
                {
                    yield return new ChestData(chest.TileLocation, location.uniqueName.Value ?? location.Name);
                }
            }
        }

        /// <summary>
        /// Will translate the data from the loaded file by comparing the locations to existing chests and returning the existing chests.
        /// </summary>
        /// <param name="chestData"></param>
        /// <returns></returns>
        private static IEnumerable<Chest> FromChestData(IEnumerable<ChestData> chestData)
        {
            var chestDataList = chestData.ToList();
            foreach (var chestLocationPair in Util.GetAllChests())
            {
                var chest = chestLocationPair.Chest;
                var location = chestLocationPair.Location;
                if (chestDataList.Any(data =>
                    data.ChestTileLocation.Equals(chest.TileLocation) &&
                    data.LocationName.Equals(location.uniqueName.Value ?? location.Name)))
                {
                    yield return chest;
                }
            }
        }
    }
}