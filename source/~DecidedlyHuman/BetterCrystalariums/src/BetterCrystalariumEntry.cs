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
using BetterCrystalariums.Helpers;
using BetterCrystalariums.Utilities;
using HarmonyLib;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BetterCrystalariums
{
    public class BetterCrystalariumEntry : Mod
    {
        private ModConfig _config;
        private IModHelper _helper;
        private Logger _logger;
        private IMonitor _monitor;
        private Patches _patches;

        public override void Entry(IModHelper helper)
        {
            this._helper = helper;
            this._monitor = this.Monitor;
            this._logger = new Logger(this._monitor);
            this._config = this._helper.ReadConfig<ModConfig>();
            this._patches = new Patches(this._monitor, this._helper, this._logger, this._config);

            Harmony harmony = new(this.ModManifest.UniqueID);

            harmony.Patch(
                AccessTools.Method(typeof(SObject),
                    nameof(SObject.performObjectDropInAction),
                    new[] { typeof(Item), typeof(bool), typeof(Farmer), typeof(bool) }),
                new HarmonyMethod(typeof(Patches),
                    nameof(Patches.ObjectDropIn_Prefix))
            );

            this._helper.Events.GameLoop.GameLaunched += this.GameLaunched;
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            try
            {
                this.RegisterWithGmcm();
            }
            catch (Exception ex)
            {
                this._logger.Log(this._helper.Translation.Get("bettercrystalariums.no-gmcm"));
            }
        }

        private void RegisterWithGmcm()
        {
            var configMenuApi =
                this.Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            configMenuApi.RegisterModConfig(this.ModManifest,
                () => this._config = new ModConfig(),
                () => this.Helper.WriteConfig(this._config));

            configMenuApi.RegisterSimpleOption(this.ModManifest,
                this._helper.Translation.Get("bettercrystalariums.debug-setting-title"),
                this._helper.Translation.Get("bettercrystalariums.debug-setting-description"),
                () => this._config.DebugMode,
                value => this._config.DebugMode = value);
        }
    }
}
