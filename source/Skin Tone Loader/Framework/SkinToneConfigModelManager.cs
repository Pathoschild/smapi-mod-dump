/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/HeyImAmethyst/SkinToneLoader
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace SkinToneLoader.Framework
{
    public static class SkinToneConfigModelManager
    {
        /// <summary>
        /// Save the character's skin tone index to a json file.
        /// </summary>
        /// <param name="skinIndex">The skin index</param>
        public static void SaveCharacterLayout(ModEntry entry)
        {
            // Save the current player skin tone index to the SkinColorConfigModel
            SkinToneConfigModel currentPlayerSkinColorConfig = new SkinToneConfigModel();
            currentPlayerSkinColorConfig.SkinIndex = Game1.player.skin.Value;

            // Write the config model to a json
            if (Constants.SaveFolderName != null)
            {
                currentPlayerSkinColorConfig.SaveFolderName = Constants.SaveFolderName;
                entry.Helper.Data.WriteJsonFile<SkinToneConfigModel>(Path.Combine("Saves", $"{Constants.SaveFolderName}_SkinToneConfig.json"), currentPlayerSkinColorConfig);
            }
        }

        /// <summary>
        /// Save the character's skin tone index to a json file.
        /// </summary>
        /// <param name="skinIndex">The skin index</param>
        public static void SaveCharacterLayout(ModEntry entry, int skinIndex)
        {
            // Save all the skin index to the ConfigModel
            SkinToneConfigModel currentPlayerSkinTone = new SkinToneConfigModel();
            currentPlayerSkinTone.SkinIndex = skinIndex;

            // Write the config model to a json
            if (Constants.SaveFolderName != null && Context.IsWorldReady)
            {
                currentPlayerSkinTone.SaveFolderName = Constants.SaveFolderName;
                entry.Helper.Data.WriteJsonFile<SkinToneConfigModel>(Path.Combine("Saves", $"{Constants.SaveFolderName}_SkinToneConfig.json"), currentPlayerSkinTone);
            }
        }

        /// <summary>
        /// Reads the character's skin tone index from a json file.
        /// </summary>
        /// <param name="skinIndex">The skin index</param>
        /// <returns>a new Config Model Object</returns>
        public static SkinToneConfigModel ReadCharacterLayout(ModEntry entry, Farmer farmer)
        {
            string localConfigPath = Path.Combine("Saves", $"{new DirectoryInfo(farmer.slotName)}_SkinToneConfig.json");

            SkinToneConfigModel model = entry.Helper.Data.ReadJsonFile<SkinToneConfigModel>(localConfigPath);

            if (model == null)
                model = CreateNewConfigForSave(entry, localConfigPath, new DirectoryInfo(farmer.slotName).Name);

            return model;
        }

        /// <summary>
        /// Creates a new SkinColorConfigModel json file.
        /// </summary>
        /// <param name="skinIndex">The skin index</param>
        ///<returns>a new Config Model Object</returns>
        private static SkinToneConfigModel CreateNewConfigForSave(ModEntry entry, string localConfigPath, string saveFolderName)
        {
            SkinToneConfigModel model = new SkinToneConfigModel();

            model.SaveFolderName = saveFolderName;

            entry.Monitor.Log("Creating a new SkinColorConfigModel json for " + saveFolderName, LogLevel.Debug);
            entry.Helper.Data.WriteJsonFile(localConfigPath, model);

            return model;
        }
    }
}
