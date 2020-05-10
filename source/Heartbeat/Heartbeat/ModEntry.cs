using System;
using System.IO;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

namespace Heartbeat
{
    public class ModEntry : Mod
    {
        private ModConfig Config;

        private SoundEffect soundEffect;

        public override void Entry(IModHelper helper)
        {
            registerSoundEffect();

            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        }

        private void GameLoop_GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            Config = Helper.ReadConfig<ModConfig>();

            var api = Helper.ModRegistry.GetApi<GenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");

            if (api == null) { return; }

            api.RegisterModConfig(ModManifest, () => Config = new ModConfig(), () => Helper.WriteConfig(Config));
            
            api.RegisterSimpleOption(
                ModManifest,
                "Heartbeat Enabled",
                "Enable Heartbeat mod.",
                () => Config.HeartBeatEnabled,
                (bool val) => Config.HeartBeatEnabled = val
            );

            api.RegisterSimpleOption(
                ModManifest,
                "Hearbeat Alert Percentage",
                "What percent life total to beat at?",
                () => Config.HeartBeatAlertPercent,
                (float val) => Config.HeartBeatAlertPercent = val
            );

            api.RegisterSimpleOption(
                ModManifest,
                "Heart Tick Rate (bpm)",
                "Human average is between 60 and 100.",
                () => Config.HeartBeatHeartRate,
                (float val) => Config.HeartBeatHeartRate = val
            );
        }

        private void GameLoop_UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (!Context.IsWorldReady) { return; }

            if (!Config.HeartBeatEnabled) { return; }

            double currentHealthPercent = Math.Round(((double)Game1.player.health / Game1.player.maxHealth * 100), 0);
            
            if (currentHealthPercent > Config.HeartBeatAlertPercent) { return; }
            
            double damageMultiplier = 1.0;
            float  pitchModifier    = 0.0f;
            float  panModifier      = 0.0f;

            if (currentHealthPercent <= (Config.HeartBeatAlertPercent / 4))
            {
                damageMultiplier = 2.5;
                pitchModifier = 0.5f;
            }
            else if(currentHealthPercent <= (Config.HeartBeatAlertPercent / 2))
            {
                damageMultiplier = 2;
            }
            else if(currentHealthPercent <= Config.HeartBeatAlertPercent / 1.33)
            {
                damageMultiplier = 1.5;
            }

            uint tickRate = (uint)Math.Round(Config.HeartBeatHeartRate / damageMultiplier);

            if (! e.IsMultipleOf(tickRate)) { return; }

            soundEffect.Play(Game1.options.soundVolumeLevel, pitchModifier, panModifier);
        }
    
        private void registerSoundEffect()
        {
            string path = Path.Combine(Helper.DirectoryPath, "assets", "human-heartbeat-daniel_simon.wav");

            FileStream stream = new FileStream(path, FileMode.Open);

            try
            { 
                soundEffect = SoundEffect.FromStream(stream);
            }
            catch (Exception e)
            {
                Monitor.Log(e.Message, LogLevel.Error);
            }

            stream.Close();
            stream.Dispose();
        }
    }
}

public interface GenericModConfigMenuAPI
{
    void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);

    void RegisterLabel(IManifest mod, string labelName, string labelDesc);
    void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
    void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);
    void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);
    void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);
    void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);

    void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
    void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max);

    void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);

    void RegisterComplexOption(IManifest mod, string optionName, string optionDesc,
                               Func<Vector2, object, object> widgetUpdate,
                               Func<SpriteBatch, Vector2, object, object> widgetDraw,
                               Action<object> onSave);
}