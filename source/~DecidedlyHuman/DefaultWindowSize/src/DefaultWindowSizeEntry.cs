/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DecidedlyHuman/StardewValleyMods
**
*************************************************/

using System;
using DefaultWindowSize.Helpers;
using DefaultWindowSize.Utilities;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace DefaultWindowSize
{
	public class DefaultWindowSizeEntry : Mod
	{
		private Logger _logger;
		private ModConfig _config;
		private IModHelper _helper;

		private static string _commandName = "set_resolution";
		private static string _commandDescription = "\tSet the resolution of the Stardew Valley window. (Minimum 1280x720)";
		private static string _commandUsage = $"\nUsage: \t\t\t{_commandName} <Resolution>\n";

		public override void Entry(IModHelper helper)
		{
			_logger = new Logger(Monitor);
			_helper = helper;
			_config = _helper.ReadConfig<ModConfig>();

			helper.ConsoleCommands.Add(_commandName,
			   $"{_commandDescription}\n\n{_commandUsage}", SetResolutionCommand);
			helper.Events.GameLoop.GameLaunched += GameLaunched;

			if (_config.SetOnStart)
			{
				Resolution newRes = new Resolution(_config.StartResolution);

				SetResolution(newRes);
			}
		}

		private void GameLaunched(object sender, GameLaunchedEventArgs e)
		{
			try
			{
				RegisterWithGmcm();
			}
			catch (Exception ex)
			{
				_logger.Log("User doesn't appear to have GMCM installed. This is not a bug.");
			}
		}

		private void SetResolutionCommand(string command, string[] args)
		{
			// We expect, and can have only one argument.
			if (args.Length != 1)
			{
				PrintUsage("Incorrect amount of arguments.");
				return;
			}
			else
			{
				// We pass in our one and only argument to have it sanitised, and parsed.
				Resolution newRes = new Resolution(args[0]);

				// The game doesn't seem to let you set a resolution lower than 720p, so we want to warn if the user tries to do that.
				if (newRes.X < 1280 || newRes.Y < 720)
				{
					PrintUsage("Minimum resolution is 1280x720. Setting to that.");
				}

				SetResolution(newRes);
			}
		}

		private void SetResolution(Resolution res)
		{
			Game1.game1.SetWindowSize(res.X, res.Y);
		}

		private void PrintUsage(string error)
		{
			_logger.Log(error);
			_logger.Log(_commandUsage);
		}

		private void RegisterWithGmcm()
		{
			GenericModConfigMenuApi configMenuApi =
				Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

			configMenuApi.Register(ModManifest,
				() => _config = new ModConfig(),
				() => Helper.WriteConfig(_config));

			configMenuApi.AddBoolOption(
				mod: ModManifest,
				name: () => "Set on Start",
				getValue: () => _config.SetOnStart,
				setValue: value => _config.SetOnStart = value);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () =>
					"This setting will determine whether or not the resolution you specify here will be set on game launch.");

			configMenuApi.AddTextOption(
				mod: ModManifest,
				name: () => "Start Resolution",
				getValue: () => _config.StartResolution,
				setValue: value => _config.StartResolution = value);

			configMenuApi.AddParagraph(
				mod: ModManifest,
				text: () =>
					"This is the resolution that the game will start at if the above box is ticked.");
		}
	}
}
