/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System;

namespace SP_Regen
{
    class ModEntry : Mod
    {
        public GameTime time;
        public SPConfig Config;

        public override void Entry(IModHelper helper)//Load helpers and read config
        {
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
            ReadConfig();
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e) => registerForGMCM();

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady || (!Config.TimeFrozenRegen && !Game1.shouldTimePass()))//if world isn't loaded, do nothing
                return;

            time = Game1.currentGameTime; //assigns time variable to currentGameTime
            if (Game1.player.isInBed.Value || (Config.Sitting && Game1.player.isSitting.Value))//Check if the player is in bed
            {
                Game1.player.regenTimer -= time.ElapsedGameTime.Milliseconds;//Set the regenTimer
                if (Game1.player.regenTimer < 0)//if the regenTimer is below 0 Milliseconds, add stamina and health
                {
                    Game1.player.regenTimer = Config.RegenTime;
                    if (Game1.player.stamina < Game1.player.MaxStamina)
                        ++Game1.player.stamina;
                    if (Game1.player.health < Game1.player.maxHealth)
                        ++Game1.player.health;
                }
            }
        }

        private void ReadConfig()
        {
            Config = Helper.ReadConfig<SPConfig>();
            if (Config.RegenTime <= 0)
                Config.RegenTime = 1;
            Helper.WriteConfig(Config);
        }

        private void registerForGMCM()
        {
            var gmcm = Helper.ModRegistry.GetApi<IGMCMApi>("spacechase0.GenericModConfigMenu");
            if (gmcm is null)
                return;

            gmcm.Register(ModManifest, () => Config = new(), () => Helper.WriteConfig(Config));

            gmcm.AddNumberOption(ModManifest, () => Config.RegenTime, (x) => Config.RegenTime = x <= 0 ? 1 : x, () => "Interval", () => "The interval at which health and stamina regen (Lower value = faster)");

            gmcm.AddBoolOption(ModManifest, () => Config.Sitting, (x) => Config.Sitting = x, () => "Regen while sitting", () => "Whether or not health and stamina should regen while sitting");
            gmcm.AddBoolOption(ModManifest, () => Config.TimeFrozenRegen, (x) => Config.TimeFrozenRegen = x, () => "Regen while time is frozen", () => "Whether or not health and stamina should regen while time is frozen");
        }
    }

    public interface IGMCMApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);
    }
}
