/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dunc4nNT/StardewMods
**
*************************************************/

using NeverToxic.StardewMods.YetAnotherTimeMod.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace NeverToxic.StardewMods.YetAnotherTimeMod
{
    internal class ModEntry : Mod
    {
        private ModConfig _config;

        private ModConfigKeys Keys => this._config.Keys;

        private TimeHelper _timeHelper;

        public override void Entry(IModHelper helper)
        {
            I18n.Init(helper.Translation);

            this._config = helper.ReadConfig<ModConfig>();
            this._timeHelper = new(this._config, this.Monitor);

            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.UpdateTicked += this.OnUpdateTicked;
            helper.Events.Input.ButtonsChanged += this.OnButtonsChanged;
        }

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            if (!Context.IsMainPlayer)
                this.Monitor.Log(I18n.Message_NotMainPlayerWarning(), LogLevel.Warn);
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;

            this._timeHelper.Update();
        }

        private void OnButtonsChanged(object sender, ButtonsChangedEventArgs e)
        {
            if (!Context.IsWorldReady || !Context.IsMainPlayer)
                return;

            if (this.Keys.ReloadConfig.JustPressed())
                this.ReloadConfig();
            else if (this.Keys.IncreaseSpeed.JustPressed())
                this._timeHelper.IncreaseSpeed();
            else if (this.Keys.DecreaseSpeed.JustPressed())
                this._timeHelper.DecreaseSpeed();
            else if (this.Keys.ResetSpeed.JustPressed())
                this._timeHelper.ResetSpeed();
            else if (this.Keys.DoubleSpeed.JustPressed())
                this._timeHelper.SetDoubleSpeed();
            else if (this.Keys.HalfSpeed.JustPressed())
                this._timeHelper.SetHalfSpeed();
            else if (this.Keys.ToggleFreeze.JustPressed())
                this._timeHelper.ToggleFreeze();
        }

        private void ReloadConfig()
        {
            this._config = this.Helper.ReadConfig<ModConfig>();
            this.Monitor.Log(I18n.Message_ConfigReloaded(), LogLevel.Info);
        }
    }
}
