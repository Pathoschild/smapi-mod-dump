using System;
using System.Collections.Generic;
using System.Linq;
using Denifia.Stardew.SendItems.Domain;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace Denifia.Stardew.SendItems.Services
{
    public class CommandService : ICommandService
    {
        private bool _savedGameLoaded = false;

        private readonly IMod _mod;
        private readonly IConfigurationService _configService;
        private readonly IFarmerService _farmerService;

        private const string _meCommand = "si_me";
        private const string _localFarmersCommand = "si_localfarmers";
        private const string _addLocalFarmersCommand = "si_addlocalfarmers";
        private const string _addFriendCommand = "si_addfriend";
        private const string _removeFriendCommand = "si_removefriend";
        private const string _removeAllFriendsCommand = "si_removeallfriends";
        private const string _myFriendsCommand = "si_myfriends";

        public CommandService(
            IMod mod,
            IConfigurationService configService,
            IFarmerService farmerService)
        {
            _mod = mod;
            _configService = configService;
            _farmerService = farmerService;

            RegisterCommands();

            var events = mod.Helper.Events;
            events.GameLoop.SaveLoaded += OnSaveLoaded;
            events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            _savedGameLoaded = false;
        }

        private void RegisterCommands()
        {
            _mod.Helper.ConsoleCommands
                .Add(_meCommand, $"Shows you the command that your friends need to type to add the current farmer as a friend. \n\nUsage: {_meCommand}", HandleCommand)
                .Add(_localFarmersCommand, $"Lists all the local farmers (saved games). \n\nUsage: {_localFarmersCommand}", HandleCommand)
                .Add(_addLocalFarmersCommand, $"Adds all the local farmers (saved games) as friends to the current farmer. \n\nUsage: {_addLocalFarmersCommand}", HandleCommand)
                .Add(_addFriendCommand, $"Adds a friend to the current farmer. \n\nUsage: {_addFriendCommand} <id> <name> <farmName> \n- id: the id of your friend.\n- name: the name of your friend.\n- farmName: the name of your friends farm.", HandleCommand)
                .Add(_removeFriendCommand, $"Removes a friend of the current farmer. \n\nUsage: {_removeFriendCommand} <id> \n- id: id of your friend.", HandleCommand)
                .Add(_removeAllFriendsCommand, $"Removes all the friends of the current farmer. \n\nUsage: {_removeAllFriendsCommand}", HandleCommand)
                .Add(_myFriendsCommand, $"Lists all the friends of the current farmer. \n\nUsage: {_myFriendsCommand}", HandleCommand);
        }

        private void HandleCommand(string command, string[] args)
        {
            var quote = "\"";
            var newArgs = new List<string>();
            var temp = string.Empty;
            foreach (var arg in args)
            {
                if (arg.StartsWith(quote) && arg.EndsWith(quote))
                {
                    newArgs.Add(arg.Trim(quote.ToArray()));
                    continue;
                }

                if (arg.StartsWith(quote))
                {
                    temp = arg.Trim(quote.ToArray());
                    continue;
                }

                if (arg.EndsWith(quote))
                {
                    temp += " " + arg.Trim(quote.ToArray());
                    newArgs.Add(temp);
                    temp = string.Empty;
                    continue;
                }

                if (temp.Equals(string.Empty))
                {
                    newArgs.Add(arg);
                    continue;
                }

                temp += " " + arg;
            }
            args = newArgs.ToArray();

            if (!_savedGameLoaded)
            {
                _mod.Monitor.Log("Please load up a saved game first, then try again.", LogLevel.Warn);
                return;
            }

            switch (command)
            {
                case _meCommand:
                    ShowMyDetails(args);
                    break;
                case _localFarmersCommand:
                    ListLocalFarmers(args);
                    break;
                case _addLocalFarmersCommand:
                    AddLocalFarmers(args);
                    break;
                case _addFriendCommand:
                    AddFriend(args);
                    break;
                case _removeFriendCommand:
                    RemoveFriend(args);
                    break;
                case _removeAllFriendsCommand:
                    RemoveAllFriends(args);
                    break;
                case _myFriendsCommand:
                    ListMyFriends(args);
                    break;
                default:
                    throw new NotImplementedException($"Send Items received unknown command '{command}'.");
            }
        }

        private void ShowMyDetails(string[] args)
        {
            _mod.Monitor.Log("This is your \"friend command\". Get your friends to run this command in the SMAPI console to add you as a friend. Each farmer (saved game) has it's own list of friends.", LogLevel.Info);
            _mod.Monitor.Log($"{_addFriendCommand} {_farmerService.CurrentFarmer.Id} {_farmerService.CurrentFarmer.ConsoleSafeName} {_farmerService.CurrentFarmer.ConsoleSaveFarmName}", LogLevel.Info);
        }

        private void ListLocalFarmers(string[] args)
        {
            var farmers = Repository.Instance.Fetch<Domain.Farmer>();
            if (farmers.Any())
            {
                _mod.Monitor.Log("Here are all the local farmers (saved games)...", LogLevel.Info);
                _mod.Monitor.Log("<id> <name> <farm name>", LogLevel.Info);
                foreach (var farmer in farmers)
                {
                    _mod.Monitor.Log($"{farmer.Id} {farmer.ConsoleSafeName} {farmer.ConsoleSaveFarmName}", LogLevel.Info);
                }
            }
            else
            {
                _mod.Monitor.Log("No farmers (saved games) found.", LogLevel.Warn);
            }
        }

        private void AddLocalFarmers(string[] args)
        {
            var farmers = Repository.Instance.Fetch<Domain.Farmer>();
            var count = 0;
            foreach (var farmer in farmers)
            {
                if (!_farmerService.CurrentFarmer.Friends.Any(x => x.Id == farmer.Id))
                {
                    AddFriend(new Friend
                    {
                        Id = farmer.Id,
                        Name = farmer.Name,
                        FarmName = farmer.FarmName
                    });
                    count++;
                }
            }
            if (count == 0)
            {
                _mod.Monitor.Log($"No local farmers were added. They may already be added.", LogLevel.Info);
            }
        }

        private void AddFriend(string[] args)
        {
            if (args.Length == 3)
            {
                AddFriend(new Friend
                {
                    Id = args[0],
                    Name = args[1],
                    FarmName = args[2]
                });
            }
            else
            {
                LogArgumentsInvalid(_addFriendCommand);
            }
        }

        private void RemoveFriend(string[] args)
        {
            if (args.Length == 1)
            {
                var friend = _farmerService.CurrentFarmer.Friends.FirstOrDefault(x => x.Id == args[0]);
                if (friend != null)
                {
                    var success = _farmerService.RemoveFriendFromCurrentPlayer(friend.Id);
                    if (success)
                    {
                        _mod.Monitor.Log($"{friend.Name} ({friend.FarmName} Farm) [id:{friend.Id}] was removed!", LogLevel.Info);
                    }
                    else
                    {
                        _mod.Monitor.Log($"There was an issues removing {friend.Name} ({friend.FarmName} Farm) [id:{friend.Id}].", LogLevel.Warn);
                    }
                }
                else
                {
                    _mod.Monitor.Log($"Couldn'd find a friend with id {args[0]}.", LogLevel.Info);
                }
            }
            else
            {
                LogArgumentsInvalid(_removeFriendCommand);
            }
        }

        private void RemoveAllFriends(string[] args)
        {
            var success = _farmerService.RemoveAllFriendFromCurrentPlayer();
            if (success)
            {
                _mod.Monitor.Log($"Friends list successfully cleared!", LogLevel.Info);
            }
            else
            {
                _mod.Monitor.Log($"Friends list was not cleared. It may have already been cleared.", LogLevel.Info);
            }
        }

        private void ListMyFriends(string[] args)
        {
            var friends = _farmerService.CurrentFarmer.Friends;
            if (friends.Any())
            {
                _mod.Monitor.Log("Your friends for the current farmer (saved game) are...", LogLevel.Info);
                foreach (var friend in friends)
                {
                    _mod.Monitor.Log($"{friend.Name} ({friend.FarmName} Farm) [ID: {friend.Id}]", LogLevel.Info);
                }
            }
            else
            {
                _mod.Monitor.Log("The current farmer (saved game) has no friends. How sad :(", LogLevel.Info);
                _mod.Monitor.Log($"You can add friends with the {_addFriendCommand} command.", LogLevel.Info);
            }
        }

        private void AddFriend(Friend friend)
        {
            var success = _farmerService.AddFriendToCurrentPlayer(friend);
            if (success)
            {
                _mod.Monitor.Log($"{friend.Name} ({friend.FarmName} Farm) [id:{friend.Id}] was added.", LogLevel.Info);
            }
            else
            {
                _mod.Monitor.Log($"These was an issue adding {friend.Name} ({friend.FarmName} Farm) [id:{friend.Id}]. Maybe they already be added.", LogLevel.Warn);
            }
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _savedGameLoaded = true;
            _mod.Monitor.Log($"This is your \"friend command\". Get your friends to run this command in the SMAPI console to add you as a friend...", LogLevel.Info);
            _mod.Monitor.Log($"{_addFriendCommand} {_farmerService.CurrentFarmer.Id} {_farmerService.CurrentFarmer.ConsoleSafeName} {_farmerService.CurrentFarmer.ConsoleSaveFarmName}", LogLevel.Alert);
            _mod.Monitor.Log($"They need to be using the Send Items mod too :)", LogLevel.Info);
        }

        private void LogUsageError(string error, string command)
        {
            _mod.Monitor.Log($"{error} Type 'help {command}' for usage.", LogLevel.Error);
        }

        private void LogArgumentsInvalid(string command)
        {
            LogUsageError("The arguments are invalid.", command);
        }
    }
}
