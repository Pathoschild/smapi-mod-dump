/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Microsoft.Xna.Framework.Content;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace Igorious.StardewValley.DynamicAPI.Services
{
    public sealed class InformationService
    {
        #region Singleton Access

        private InformationService()
        {
            GameEvents.LoadContent += (s, e) => LoadMainContentToGame();
            LocationEvents.CurrentLocationChanged += (s, e) => LoadAdditionalContentToGame();
        }

        private static InformationService _instance;

        public static InformationService Instance => _instance ?? (_instance = new InformationService());

        #endregion

        #region Private Data

        private readonly List<CraftableInformation> _craftableInformations = new List<CraftableInformation>();
        private readonly List<IItemInformation> _itemsInformations = new List<IItemInformation>();
        private readonly List<ITreeInformation> _treesInformations = new List<ITreeInformation>();
        private readonly List<ICropInformation> _cropInformations = new List<ICropInformation>();
        private readonly List<ItemInformation> _overridableInformations = new List<ItemInformation>();

        #endregion

        #region	Public Methods

        /// <summary>
        /// Register information about craftable item.
        /// </summary>
        public void Register(CraftableInformation information)
        {
            _craftableInformations.Add(information);
        }

        /// <summary>
        /// Register information about item.
        /// </summary>
        public void Register(IItemInformation information)
        {
            _itemsInformations.Add(information);
        }

        /// <summary>
        /// Register information about item.
        /// </summary>
        public void Register(ITreeInformation information)
        {
            _treesInformations.Add(information);
        }

        /// <summary>
        /// Register information about crop.
        /// </summary>
        public void Register(ICropInformation information)
        {
            _cropInformations.Add(information);
        }

        /// <summary>
        /// Override information about item.
        /// </summary>
        public void Override(ItemInformation information)
        {
            _overridableInformations.Add(information);
        }

        #endregion

        #region	Auxiliary Methods

        private void LoadMainContentToGame()
        {
            LoadToGame(Game1.bigCraftablesInformation, _craftableInformations);
            LoadToGame(Game1.objectInformation, _itemsInformations);
            LoadToGame(GetDataCache(@"Data\Crops", false), _cropInformations);
            LoadToGame(GetDataCache(@"Data\fruitTrees", false), _treesInformations);

            var objectInformation = Game1.objectInformation;
            foreach (var overridableInformation in _overridableInformations)
            {
                var id = overridableInformation.ID;
                string actualInfo;
                if (!objectInformation.TryGetValue(id, out actualInfo)) continue;

                objectInformation.Remove(id);
                objectInformation.Add(id, OverrideInformation(actualInfo, overridableInformation));
            }
        }

        private void LoadAdditionalContentToGame()
        {
            LoadToGame(GetDataCache(@"Data\Crops", true), _cropInformations, false);
            LoadToGame(GetDataCache(@"Data\fruitTrees", true), _treesInformations, false);
        }

        private static void LoadToGame(IDictionary<int, string> gameInformation, IReadOnlyList<IInformation> customInformation, bool showWarnings = true)
        {
            foreach (var information in customInformation)
            {
                var key = information.ID;
                var newValue = information.ToString();
                string oldValue;
                if (!gameInformation.TryGetValue(key, out oldValue))
                {
                    gameInformation.Add(key, newValue);
                }
                else if (newValue != oldValue && showWarnings)
                {
                    Log.SyncColour($"Information for ID={key} already has another mapping {key}->{oldValue} (current:{newValue})", ConsoleColor.DarkRed);
                }
            }
        }

        private static Dictionary<int, string> GetDataCache(string assetPath, bool useTemporary)
        {
            ContentManager contentManager;
            if (useTemporary)
            {
                if (Game1.temporaryContent == null)
                {
                    Game1.temporaryContent = new ContentManager(Game1.content.ServiceProvider, Game1.content.RootDirectory);
                }
                contentManager = Game1.temporaryContent;
            }
            else
            {
                contentManager = Game1.content;
            }
            contentManager.Load<Dictionary<int, string>>(assetPath);
            var loadedAssets = contentManager.GetField<Dictionary<string, object>>("loadedAssets");
            return (Dictionary<int, string>)loadedAssets[assetPath];
        }

        private static string OverrideInformation(string actualInfo, ItemInformation overridableInformation)
        {
            var parts = actualInfo.Split('/');
            if (overridableInformation.Name != null) parts[0] = overridableInformation.Name;
            if (overridableInformation.Price != 0) parts[1] = overridableInformation.Price.ToString();
            if (overridableInformation.Description != null) parts[4] = overridableInformation.Description;
            return string.Join("/", parts);
        }

        #endregion
    }
}