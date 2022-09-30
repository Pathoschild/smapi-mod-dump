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
        public ShopSaveData shopSaveData;

        /// <summary>
        /// Save data persistent to the actual player.
        /// </summary>
        public PlayerSaveData playerSaveData;

        public SaveDataManager()
        {

        }

        public virtual void loadOrCreateSaveData()
        {
            this.shopSaveData = new ShopSaveData();
            this.playerSaveData = PlayerSaveData.LoadOrCreate();
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

        public virtual string getRelativeSaveDataPath()
        {
            string friendlyName = SaveGame.FilterFileName(Game1.GetSaveGameName());
            string filenameNoTmpString = friendlyName + "_" + Game1.uniqueIDForThisGame;
            filenameNoTmpString += Game1.player.name + "_" + Game1.player.uniqueMultiplayerID.Value;

            return Path.Combine("SaveData", filenameNoTmpString + Path.DirectorySeparatorChar);
        }

        /// <summary>
        /// Saves all of the necessary save data for the mod.
        /// </summary>
        public virtual void save()
        {
                this.shopSaveData.save();
            this.playerSaveData.save();

        }
    }
}
