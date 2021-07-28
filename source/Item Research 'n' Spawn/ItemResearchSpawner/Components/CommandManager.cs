/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TSlex/StardewValley
**
*************************************************/

using System;
using ItemResearchSpawner.Models;
using ItemResearchSpawner.Models.Enums;
using ItemResearchSpawner.Utils;
using StardewModdingAPI;
using StardewValley;

namespace ItemResearchSpawner.Components
{
    internal class CommandManager
    {
        private readonly IModHelper _helper;
        private readonly IMonitor _monitor;
        private ProgressionManager _progressionManager;
        private ModManager _modManager;

        public CommandManager(IModHelper helper, IMonitor monitor, ProgressionManager progressionManager, ModManager modManager)
        {
            _helper = helper;
            _monitor = monitor;
            
            _progressionManager = progressionManager;
            _modManager = modManager;

            helper.ConsoleCommands.Add("research_unlock_all", "unlock all items research progression",
                UnlockAllProgression);

            helper.ConsoleCommands.Add("research_unlock_active", "unlock hotbar active item",
                UnlockActiveProgression);

            helper.ConsoleCommands.Add("research_set_mode", "change mode to \n 0 - Spawn Mode \n 1 - Buy/Sell Mode",
                SetMode);

            helper.ConsoleCommands.Add("research_set_price",
                "set hotbar active item price (globally, for mod menu only) \n 0+ values only",
                SetPrice);

            helper.ConsoleCommands.Add("research_reset_price",
                "reset hotbar active item price (globally, for mod menu only)",
                ResetPrice);

            helper.ConsoleCommands.Add("research_get_key", "get hotbar active item unique key",
                GetUniqueKey);
            
            helper.ConsoleCommands.Add("research_dump_progression", "dump player(s) progression to file",
                DumpProgression
                );
            
            helper.ConsoleCommands.Add("research_load_progression", "load player(s) progression from file",
                LoadProgression
            );
            
            helper.ConsoleCommands.Add("research_dump_pricelist", "dump pricelist to file",
                DumpPricelist
            );
            
            helper.ConsoleCommands.Add("research_load_pricelist", "load pricelist from file",
                LoadPricelist
            );
            
            helper.ConsoleCommands.Add("research_dump_categories", "dump categories to file",
                DumpCategories
            );
            
            helper.ConsoleCommands.Add("research_load_categories", "load categories from file",
                LoadCategories
            );
        }

        private void UnlockAllProgression(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            _progressionManager.UnlockAllProgression();
            
            _monitor.Log($"All researches were completed! :D", LogLevel.Info);
        }

        private void UnlockActiveProgression(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                _monitor.Log($"Select an item first", LogLevel.Info);
            }
            else
            {
                _progressionManager.UnlockProgression(activeItem);
                _monitor.Log($"Item - {activeItem.DisplayName}, was unlocked! ;)", LogLevel.Info);
            }
        }

        private void SetMode(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;

            try
            {
                _modManager.ModMode = (ModMode) int.Parse(args[0]);
                _monitor.Log($"Mode was changed to: {_modManager.ModMode.GetString()}", LogLevel.Info);
            }
            catch (Exception)
            {
                _monitor.Log($"Available modes: \n 0 - Spawn Mode \n 1 - Buy/Sell Mode", LogLevel.Info);
            }
        }

        private void SetPrice(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                _monitor.Log($"Select an item first", LogLevel.Info);
            }
            else
            {
                try
                {
                    var price = int.Parse(args[0]);

                    if (price < 0)
                    {
                        _monitor.Log($"Price must be a non-negative number", LogLevel.Info);
                    }

                    _modManager.SetItemPrice(activeItem, price);
                    _monitor.Log($"Price for {activeItem.DisplayName}, was changed to: {price}! ;)", LogLevel.Info);
                }
                catch (Exception)
                {
                    _monitor.Log($"Price must be a correct non-negative number", LogLevel.Info);
                }
            }
        }

        private void ResetPrice(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                _monitor.Log($"Select an item first", LogLevel.Info);
            }
            else
            {
                _modManager.SetItemPrice(activeItem, -1);
                _monitor.Log($"Price for {activeItem.DisplayName}, was resetted! ;)", LogLevel.Info);
            }
        }

        private void GetUniqueKey(string command, string[] args)
        {
            if (!CheckCommandInGame()) return;

            var activeItem = Game1.player.CurrentItem;

            if (activeItem == null)
            {
                _monitor.Log($"Select an item first", LogLevel.Info);
            }
            else
            {
                _monitor.Log($"{Helpers.GetItemUniqueKey(activeItem)}", LogLevel.Info);
            }
        }
        
        private void DumpProgression(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;
            
            ProgressionManager.Instance.DumpPlayersProgression();
            
            _monitor.Log($"Progressions were dumped", LogLevel.Info);
        }

        private void LoadProgression(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;
            
            ProgressionManager.Instance.LoadPlayersProgression();
            
            _monitor.Log($"Player(s) progression was loaded", LogLevel.Info);
            _monitor.Log($"Note: all changes will be applied next day", LogLevel.Info);
            _monitor.Log($"All changes made in game will be ignored", LogLevel.Info);
        }

        private void DumpPricelist(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;
            
            ModManager.Instance.DumpPricelist();
            
            _monitor.Log($"Pricelist was dumped to {SaveHelper.PricelistDumpPath}", LogLevel.Info);
        }

        private void LoadPricelist(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;
            
            ModManager.Instance.LoadPricelist();
            
            _monitor.Log($"Pricelist was loaded", LogLevel.Info);
        }
        
        private void DumpCategories(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;
            
            ModManager.Instance.DumpCategories();
            
            _monitor.Log($"Categories was dumped to {SaveHelper.CategoriesDumpPath}", LogLevel.Info);
        }

        private void LoadCategories(string command, string[] args)
        {
            if (!CheckIsHostPlayer()) return;
            
            ModManager.Instance.LoadCategories();
            
            _monitor.Log($"Categories was loaded", LogLevel.Info);
        }

        private bool CheckCommandInGame()
        {
            if (!Game1.hasLoadedGame)
            {
                _monitor.Log($"Use this command in-game", LogLevel.Info);
                return false;
            }

            return true;
        }
        
        private bool CheckIsHostPlayer()
        {
            if (!CheckCommandInGame() && Context.IsMainPlayer)
            {
                _monitor.Log($"This command is for host player only ", LogLevel.Info);
                return false;
            }

            return true;
        }
    }
}