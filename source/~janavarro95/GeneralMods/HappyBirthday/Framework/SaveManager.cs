/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Omegasis.HappyBirthday.Framework.Utilities;
using StardewValley;

namespace Omegasis.HappyBirthday.Framework
{
    /// <summary>
    /// Manages save data for happy birthday.
    /// </summary>
    public static class SaveManager
    {

        /// <summary>
        /// Forces a read when the day starts since for farmhands they don't seem to hook into the OnSave/OnLoad methods with SMAPI.
        /// </summary>
        public static void OnDayStarted(object Sender, StardewModdingAPI.Events.DayStartedEventArgs args)
        {
            if (HappyBirthdayModCore.Instance.birthdayManager.hasChosenBirthday() == false)
            {
                HappyBirthdayModCore.Instance.Monitor.Log("Loading player's birthday on new day started.");
                Load(Game1.player.uniqueMultiplayerID);
            }
        }

        /// <summary>
        /// Forces player's birthday to be saved at the end of the day.
        /// </summary>
        public static void OnDayEnded(object Sender, StardewModdingAPI.Events.DayEndingEventArgs args)
        {
            Save(Game1.player.uniqueMultiplayerID);
        }

        /// <summary>
        /// Called when the day has ended. Updates the mod's birthday info and villager queue if they have changed.
        /// </summary>
        /// <param name="UniqueMultiplayerId"></param>
        public static void Save(long UniqueMultiplayerId)
        {
            Farmer player = Game1.getFarmer(UniqueMultiplayerId);
            string uniqueSaveName = $"{player.Name}_{player.UniqueMultiplayerID}";
            string dataDirectory = Path.Combine("data", uniqueSaveName);
            string dataFilePath = Path.Combine(dataDirectory,uniqueSaveName+".json");
            string villagerQueuePath = Path.Combine(dataDirectory , uniqueSaveName + "_VillagerBirthdayGiftsQueue.json");

            if (HappyBirthdayModCore.Instance.birthdayManager.hasChosenBirthday())
            {
                Directory.CreateDirectory(dataDirectory);

                //Write birthday file to disk.
                HappyBirthdayModCore.Instance.Helper.Data.WriteJsonFile(dataFilePath, HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData);
                HappyBirthdayModCore.Instance.Helper.Data.WriteJsonFile(villagerQueuePath, HappyBirthdayModCore.Instance.birthdayManager.villagerQueue);
            }
        }

        /// <summary>
        /// Loads the player's birthday information from disk.
        /// </summary>
        /// <param name="UniqueMultiplayerId"></param>
        public static void Load(long UniqueMultiplayerId)
        {
            Farmer player=Game1.getFarmer(UniqueMultiplayerId);
            string uniqueSaveName = $"{player.Name}_{player.UniqueMultiplayerID}";
            string dataDirectory = Path.Combine("data", uniqueSaveName);
            string dataFilePath = Path.Combine(dataDirectory, uniqueSaveName + ".json");
            string villagerQueuePath = Path.Combine(dataDirectory, uniqueSaveName + "_VillagerBirthdayGiftsQueue.json");

            HappyBirthdayModCore.Instance.Monitor.Log("Loading player's birthday from: " + dataFilePath);
            // reset state
            HappyBirthdayModCore.Instance.birthdayManager.setCheckedForBirthday(false);

            //Loads the player's birthday from disk.
            HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData = HappyBirthdayModCore.Instance.Helper.Data.ReadJsonFile<PlayerData>(dataFilePath) ?? new PlayerData();
            HappyBirthdayModCore.Instance.birthdayManager.villagerQueue = HappyBirthdayModCore.Instance.Helper.Data.ReadJsonFile<Dictionary<string, VillagerInfo>>(villagerQueuePath) ?? new Dictionary<string, VillagerInfo>();

            if (HappyBirthdayModCore.Instance.birthdayManager.playerBirthdayData != null)
            {
                //This is still necessary to so that other players know when this player's birthday is.
                MultiplayerUtilities.SendBirthdayInfoToOtherPlayers();
            }
        }

    }
}
