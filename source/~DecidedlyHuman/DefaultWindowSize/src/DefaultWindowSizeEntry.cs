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
        private static readonly string _commandName = "set_resolution";

        private static readonly string _commandDescription =
            "\tSet the resolution of the Stardew Valley window. (Minimum 1280x720)";

        private static readonly string _commandUsage = $"\nUsage: \t\t\t{_commandName} <Resolution>\n";
        private ModConfig _config;
        private IModHelper _helper;
        private Logger _logger;

        public override void Entry(IModHelper helper)
        {
            this._logger = new Logger(this.Monitor);
            this._helper = helper;
            this._config = this._helper.ReadConfig<ModConfig>();

            helper.ConsoleCommands.Add(_commandName,
                $"{_commandDescription}\n\n{_commandUsage}", this.SetResolutionCommand);
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;

            if (this._config.SetOnStart)
            {
                var newRes = new Resolution(this._config.StartResolution);

                this.SetResolution(newRes);
            }
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                this.RegisterWithGmcm();
            }
            catch (Exception ex)
            {
                this._logger.Log("User doesn't appear to have GMCM installed. This is not a bug.");
            }
        }

        private void SetResolutionCommand(string command, string[] args)
        {
            // We expect, and can have only one argument.
            if (args.Length != 1)
            {
                this.PrintUsage("Incorrect amount of arguments.");
                return;
            }

            // We pass in our one and only argument to have it sanitised, and parsed.
            var newRes = new Resolution(args[0]);

            // The game doesn't seem to let you set a resolution lower than 720p, so we want to warn if the user tries to do that.
            if (newRes.X < 1280 || newRes.Y < 720) this.PrintUsage("Minimum resolution is 1280x720. Setting to that.");

            this.SetResolution(newRes);
        }

        private void SetResolution(Resolution res)
        {
            Game1.game1.SetWindowSize(res.X, res.Y);
        }

        private void PrintUsage(string error)
        {
            this._logger.Log(error);
            this._logger.Log(_commandUsage);
        }

        private void RegisterWithGmcm()
        {
            var configMenuApi =
                this.Helper.ModRegistry.GetApi<GenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            configMenuApi.Register(this.ModManifest,
                () => this._config = new ModConfig(),
                () => this.Helper.WriteConfig(this._config));

            configMenuApi.AddBoolOption(
                this.ModManifest,
                name: () => "Set on Start",
                getValue: () => this._config.SetOnStart,
                setValue: value => this._config.SetOnStart = value);

            configMenuApi.AddParagraph(
                this.ModManifest,
                () =>
                    "This setting will determine whether or not the resolution you specify here will be set on game launch.");

            configMenuApi.AddTextOption(
                this.ModManifest,
                name: () => "Start Resolution",
                getValue: () => this._config.StartResolution,
                setValue: value => this._config.StartResolution = value);

            configMenuApi.AddParagraph(
                this.ModManifest,
                () =>
                    "This is the resolution that the game will start at if the above box is ticked.");
        }
    }
}
