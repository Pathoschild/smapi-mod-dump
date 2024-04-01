/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using HarmonyLib;
using NeverToxic.StardewMods.SelfServe.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Patches = NeverToxic.StardewMods.SelfServe.Framework.Patches;

namespace NeverToxic.StardewMods.SelfServe
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig Config { get; set; }

        private ModConfigKeys Keys => this.Config.Keys;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            this.Config = helper.ReadConfig<ModConfig>();
            Harmony harmony = new(this.ModManifest.UniqueID);
            Patches.Initialise(harmony, this.Monitor, () => this.Config, this.Helper.Reflection);

            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            new GenericModConfigMenu(this.Helper.ModRegistry, this.ModManifest, this.Monitor, () => this.Config, () => this.Config = new ModConfig(), () => this.Helper.WriteConfig(this.Config)).Register();
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (this.Keys.ReloadConfig.JustPressed())
                this.ReloadConfig();
        }

        private void ReloadConfig()
        {
            this.Config = this.Helper.ReadConfig<ModConfig>();
            this.Monitor.Log(I18n.Message_ConfigReloaded(), LogLevel.Info);
        }
    }
}
