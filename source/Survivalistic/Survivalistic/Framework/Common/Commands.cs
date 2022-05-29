/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Ophaneom/Survivalistic
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using Survivalistic.Framework.Bars;
using Survivalistic.Framework.Networking;

namespace Survivalistic.Framework.Common
{
    public class Commands
    {
        private static string _error_permission = "You aren't the host!";
        private static string _error_player_not_found = "Player not found!";
        private static string _error_command_wrong = "Command missing arguments!\nPlease check the command usage with:";
        private static string _error_multiplayer = "That command only works on multiplayer!";

        public static void Feed(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer)
            {
                if (Context.IsMainPlayer)
                {
                    if (args.Length > 0)
                    {
                        bool _player_check = false;
                        foreach (Farmer _farmer in Game1.getAllFarmers())
                        {
                            if (_farmer.displayName == args[0])
                            {
                                Data _data = ModEntry.instance.Helper.Data.ReadSaveData<Data>($"{_farmer.UniqueMultiplayerID}");
                                _data.actual_hunger = _data.max_hunger;
                                ModEntry.instance.Helper.Data.WriteSaveData($"{_farmer.UniqueMultiplayerID}", _data);

                                if (Context.IsMainPlayer)
                                {
                                    ModEntry.data.actual_hunger = _data.actual_hunger;
                                    BarsUpdate.CalculatePercentage();
                                }
                                else NetController.SyncSpecificPlayer(_farmer.UniqueMultiplayerID);

                                Debugger.Log($"Feeding player {_farmer.displayName}.", "Info");
                                _player_check = true;
                                break;
                            }
                        }
                        if (!_player_check) Debugger.Log(_error_player_not_found, "Error");
                    }
                    else Debugger.Log($"{_error_command_wrong} 'help {command}'", "Error");
                }
                else Debugger.Log(_error_permission, "Error");
            }
            else
            {
                ModEntry.data.actual_hunger = ModEntry.data.max_hunger;
                Debugger.Log("Feeding the player.", "Info");
            }
        }

        public static void Hydrate(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer)
            {
                if (Context.IsMainPlayer)
                {
                    if (args.Length > 0)
                    {
                        bool _player_check = false;
                        foreach (Farmer _farmer in Game1.getAllFarmers())
                        {
                            if (_farmer.displayName == args[0])
                            {
                                Data _data = ModEntry.instance.Helper.Data.ReadSaveData<Data>($"{_farmer.UniqueMultiplayerID}");
                                _data.actual_thirst = _data.max_thirst;
                                ModEntry.instance.Helper.Data.WriteSaveData($"{_farmer.UniqueMultiplayerID}", _data);

                                if (Context.IsMainPlayer)
                                {
                                    ModEntry.data.actual_thirst = _data.actual_thirst;
                                    BarsUpdate.CalculatePercentage();
                                }
                                else NetController.SyncSpecificPlayer(_farmer.UniqueMultiplayerID);

                                Debugger.Log($"Hydrating player {_farmer.displayName}.", "Info");
                                _player_check = true;
                                break;
                            }
                        }
                        if (!_player_check) Debugger.Log(_error_player_not_found, "Error");
                    }
                    else Debugger.Log($"{_error_command_wrong} 'help {command}'", "Error");
                }
                else Debugger.Log(_error_permission, "Error");
            }
            else
            {
                ModEntry.data.actual_thirst = ModEntry.data.max_thirst;
                Debugger.Log("Hydrating the player.", "Info");
            }
        }

        public static void Fullness(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer)
            {
                if (Context.IsMainPlayer)
                {
                    if (args.Length > 0)
                    {
                        bool _player_check = false;
                        foreach (Farmer _farmer in Game1.getAllFarmers())
                        {
                            if (_farmer.displayName == args[0])
                            {
                                Data _data = ModEntry.instance.Helper.Data.ReadSaveData<Data>($"{_farmer.UniqueMultiplayerID}");
                                _data.actual_hunger = _data.max_hunger;
                                _data.actual_thirst = _data.max_thirst;
                                ModEntry.instance.Helper.Data.WriteSaveData($"{_farmer.UniqueMultiplayerID}", _data);

                                if (Context.IsMainPlayer)
                                {
                                    ModEntry.data.actual_hunger = _data.max_hunger;
                                    ModEntry.data.actual_thirst = _data.actual_thirst;
                                    BarsUpdate.CalculatePercentage();
                                }
                                else NetController.SyncSpecificPlayer(_farmer.UniqueMultiplayerID);

                                Debugger.Log($"Setting full status to the player {_farmer.displayName}.", "Info");
                                _player_check = true;
                                break;
                            }
                        }
                        if (!_player_check) Debugger.Log(_error_player_not_found, "Error");
                    }
                    else Debugger.Log($"{_error_command_wrong} 'help {command}'", "Error");
                }
                else Debugger.Log(_error_permission, "Error");
            }
            else
            {
                ModEntry.data.actual_hunger = ModEntry.data.max_hunger;
                ModEntry.data.actual_thirst = ModEntry.data.max_thirst;
                Debugger.Log("Setting full status to the player.", "info");
            }
        }

        public static void ForceSync(string command, string[] args)
        {
            if (!Context.IsWorldReady) return;
            if (Context.IsMultiplayer)
            {
                if (Context.IsMainPlayer)
                {
                    NetController.SyncAllPlayers();
                    NetController.Sync();
                    Debugger.Log($"All players are now synchronized!", "Info");
                }
                else Debugger.Log(_error_permission, "Error");
            }
            else Debugger.Log(_error_multiplayer, "Info");
        }
    }
}
