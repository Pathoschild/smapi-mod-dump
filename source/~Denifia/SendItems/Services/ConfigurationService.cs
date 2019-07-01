using System;
using System.Collections.Generic;
using System.IO;
using Denifia.Stardew.SendItems.Domain;
using Denifia.Stardew.SendItems.Framework;
using StardewModdingAPI;

namespace Denifia.Stardew.SendItems.Services
{
    // TODO: Add private members for each of the public properties for performance
    public class ConfigurationService : IConfigurationService
    {
        private IModHelper _modHelper;
        private ModConfig _modConfig;
        private const string _databaseName = ModConstants.DatabaseName;
        private string _connectionString = string.Empty;

        public string ConnectionString
        {
            get
            {
                if (_connectionString.Equals(string.Empty))
                {
                    _connectionString = Path.Combine(GetLocalPath(), _databaseName);
                }
                return _connectionString;
            }
        }

        public ConfigurationService(IModHelper modHelper)
        {
            _modHelper = modHelper;
            _modConfig = _modHelper.ReadConfig<ModConfig>();
        }

        public Uri GetApiUri()
        {
            return _modConfig.ApiUrl;
        }

        public string GetLocalPath()
        {
            return _modHelper.DirectoryPath;
        }

        public bool InDebugMode()
        {
            return _modConfig.Debug;
        }

        public bool InLocalOnlyMode()
        {
            return !(_modConfig.ApiUrl != null && _modConfig.ApiUrl.OriginalString.Length > 0 && _modConfig.ApiUrl.IsAbsoluteUri);
        }

        public List<SavedGame> GetSavedGames()
        {
            var savedGames = new List<SavedGame>();
            string saveDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "StardewValley", "Saves");
            if (Directory.Exists(saveDirectory))
            {
                string[] directories = Directory.GetDirectories(saveDirectory);
                foreach (string savedGameDirectory in directories)
                {
                    try
                    {
                        FileInfo file = new FileInfo(Path.Combine(savedGameDirectory, "SaveGameInfo"));
                        if (file.Exists)
                        {
                            var saveGameFolder = file.Directory.Name;
                            var fileContents = File.ReadAllText(file.FullName);

                            var farmerNodeStart = fileContents.IndexOf("<Farmer");
                            var farmerNodeEnd = fileContents.IndexOf("</Farmer>");
                            var farmerNode = fileContents.Substring(farmerNodeStart, farmerNodeEnd - farmerNodeStart);
                            var playerNameNodeStart = farmerNode.IndexOf("<name>") + 6;
                            var playerNameNodeEnd = farmerNode.IndexOf("</name>");
                            var playerName = farmerNode.Substring(playerNameNodeStart, playerNameNodeEnd - playerNameNodeStart);

                            var farmNameNodeStart = fileContents.IndexOf("<farmName>") + 10;
                            var farmNameNodeEnd = fileContents.IndexOf("</farmName>");
                            var farmName = fileContents.Substring(farmNameNodeStart, farmNameNodeEnd - farmNameNodeStart);

                            var savedGame = new SavedGame
                            {
                                Id = saveGameFolder,
                                Name = playerName,
                                FarmName = farmName
                            };

                            savedGames.Add(savedGame);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception($"error searching for saved games", ex);
                    }
                }
            }
            return savedGames;
        }
    }
}
