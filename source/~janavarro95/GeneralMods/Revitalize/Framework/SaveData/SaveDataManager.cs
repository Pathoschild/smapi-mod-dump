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
using Omegasis.Revitalize.Framework.SaveData.Player;
using Omegasis.Revitalize.Framework.SaveData.ShopConditionsSaveData;
using Omegasis.Revitalize.Framework.SaveData.World;
using StardewModdingAPI;
using StardewValley;

namespace Omegasis.Revitalize.Framework.SaveData
{
    /// <summary>
    /// Save data manager for managing custom save data for the Revitalize mod.
    /// </summary>
    public class SaveDataManager
    {
        /// <summary>
        /// Save data persisting to shop conditions.
        /// </summary>
        public ShopSaveDataManager shopSaveData;

        /// <summary>
        /// Save data persistent to the actual player.
        /// </summary>
        public PlayerSaveData playerSaveData;

        /// <summary>
        /// Keeps track of all items obtained by the player. Used to determine if certain recipes should be unlocked or not.
        /// </summary>
        public PlayerObtainedItems playerObtainedItems;

        /// <summary>
        /// Save data in regards to things involving the world.
        /// </summary>
        public WorldSaveDataManager worldSaveData;

        public SaveDataManager()
        {

        }

        public virtual void loadOrCreateSaveData()
        {
            this.playerSaveData = this.initializeSaveData<PlayerSaveData>(this.getRelativeSaveDataPath(), "PlayerSaveData.json");
            this.playerObtainedItems = this.initializeSaveData<PlayerObtainedItems>(this.getRelativeSaveDataPath(), "PlayerObtainedItems.json");

            //Save data managers work a bit differently.

            this.shopSaveData = new ShopSaveDataManager();
            this.shopSaveData.load();

            this.worldSaveData = new WorldSaveDataManager();
            this.worldSaveData.load();
        }

        /// <summary>
        /// Gets the save data path for the mod save data and creates the necessary directory if it doesn't exist.
        /// </summary>
        /// <returns></returns>
        public virtual string getFullSaveDataPath()
        {

            string save_directory = Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, this.getRelativeSaveDataPath());
            Directory.CreateDirectory(save_directory);
            return save_directory;
        }

        public virtual string getFullSaveDataPath(params string[] RelativePath)
        {
            string save_directory = Path.Combine(RevitalizeModCore.ModHelper.DirectoryPath, this.getRelativeSaveDataPath(),Path.Combine(RelativePath));
            return save_directory;
        }

        public virtual string getRelativeSaveDataPath()
        {
            string friendlyName = SaveGame.FilterFileName(Game1.GetSaveGameName());
            string filenameNoTmpString = friendlyName + "_" + Game1.uniqueIDForThisGame;
            filenameNoTmpString += Game1.player.Name + "_" + Game1.player.UniqueMultiplayerID;

            return Path.Combine("SaveData", filenameNoTmpString + Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Saves all of the necessary save data for the mod.
        /// </summary>
        public virtual void save()
        {
            this.shopSaveData.save();
            this.writeSaveFile(this.playerSaveData, RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(), "PlayerSaveData.json");
            this.writeSaveFile(this.playerObtainedItems, RevitalizeModCore.SaveDataManager.getRelativeSaveDataPath(), "PlayerObtainedItems.json");

            this.worldSaveData.save();
        }

        public virtual void writeSaveFile(object SaveData, params string[] RelativePathToSaveFile)
        {
            RevitalizeModCore.ModHelper.Data.WriteJsonFile(Path.Combine(RelativePathToSaveFile), this);
        }

        /// <summary>
        /// Initializes a save data file for Revitalize.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="RelativePathToConfig"></param>
        /// <returns></returns>
        public T initializeSaveData<T>(params string[] RelativePathToConfig) where T : SaveDataInfo
        {
            return this.initializeSaveData<T>(Revitalize.RevitalizeModCore.ModHelper, RelativePathToConfig);
        }

        /// <summary>
        /// Initializes the save data at the given relative path or creates it. 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="helper">The mod helper to use to get the full path for file existence checking.</param>
        /// <param name="RelativePathToConfig"></param>
        /// <returns></returns>
        public T initializeSaveData<T>(IModHelper helper, params string[] RelativePathToConfig) where T : SaveDataInfo
        {
            string relativePath = Path.Combine(RelativePathToConfig);
            if (string.IsNullOrEmpty(relativePath))
            {
                throw new Exception("A relative path to a config file MUST be supplied otherwise a file access error will be thrown.");
            }

            if (File.Exists(this.getFullSaveDataPath(RelativePathToConfig)))
            {
                T saveData = helper.Data.ReadJsonFile<T>(relativePath);
                saveData.load();
                return saveData;
            }
            else
            {
                T Config = (T)Activator.CreateInstance(typeof(T));
                helper.Data.WriteJsonFile(relativePath, Config);
                return Config;
            }
        }


    }
}
