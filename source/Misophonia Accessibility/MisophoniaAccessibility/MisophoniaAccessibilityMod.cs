/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jamespfluger/Stardew-MisophoniaAccessibility
**
*************************************************/


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using MisophoniaAccessibility.Config;
using HarmonyLib;
using MisophoniaAccessibility.UserInterface;
using StardewModdingAPI.Events;

namespace MisophoniaAccessibility
{
    public class MisophoniaAccessibilityMod : Mod
    {
        public static Dictionary<string, bool> DisabledCodeSounds { get; set; } = new Dictionary<string, bool>();
        public static Dictionary<string, bool> DisabledNamedSounds { get; set; } = new Dictionary<string, bool>();
        public ModConfig Config { get; set; }
        public Sounds SoundsConfig { get; set; }

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();
            SoundsConfig = Config.SoundsToDisable;

            if (!Config.Enabled)
            {
                return;
            }

            SetDisabledSounds(SoundsConfig);
            PerformHarmonyPatches();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        }

        /// <summary>
        /// Patches the Game1.playSound method to not play the eat sound
        /// </summary>
        private void PerformHarmonyPatches()
        {
            try
            {
                Monitor.Log("Doing harmony patches...", LogLevel.Trace);

                Harmony harmony = new Harmony(ModManifest.UniqueID);

                MethodInfo originalMethod = AccessTools.Method(typeof(Game1), nameof(Game1.playSound));
                MethodInfo prefixMethod = AccessTools.Method(typeof(SoundPatch), nameof(SoundPatch.PatchSound));

                // Have Harmony take the original property get method, and replace it with the new one we defined in SoundPatch
                harmony.Patch(original: originalMethod, prefix: new HarmonyMethod(prefixMethod));

                Monitor.Log("Patched the 'playSound' with a Harmony prefix. Note that this may cause unexpected behavior if other mods modiy this method.", LogLevel.Info);
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed to patch 'playSound' with Harmony: {ex}", LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Using the config, we can simply add the names of sounds that Stardew Valley plays without needing additional code
        /// </summary>
        /// <param name="soundConfig">The configuration of sounds not to play</param>
        private void SetDisabledSounds(Sounds soundConfig)
        {
            PropertyInfo[] configProperties = typeof(Sounds).GetProperties();

            foreach (PropertyInfo property in configProperties)
            {
                GameSound gameSound = (GameSound)property.GetCustomAttribute(typeof(GameSound));

                string soundName = gameSound.CodeName;
                bool isDisabled = (bool)property.GetValue(soundConfig);

                MisophoniaAccessibilityMod.DisabledCodeSounds.TryAdd(key: gameSound.CodeName, value: isDisabled);
                MisophoniaAccessibilityMod.DisabledNamedSounds.TryAdd(key: gameSound.DisplayName, value: isDisabled);
            }
        }

        /// <summary>
        /// When the game is first launched, register the mod configuration page so it can be used in the UI too
        /// </summary>
        private void OnGameLaunched(object sender, GameLaunchedEventArgs args)
        {
            IGenericModConfigMenuApi configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
            {
                return;
            }

            // register mod
            configMenu.Register(
                mod: this.ModManifest,
                reset: () => this.SoundsConfig = new ModConfig().SoundsToDisable,
                save: () => this.Helper.WriteConfig(this.SoundsConfig)
            );

            PropertyInfo[] configProperties = typeof(Sounds).GetProperties();

            foreach (PropertyInfo property in configProperties)
            {
                GameSound gameSound = (GameSound)property.GetCustomAttribute(typeof(GameSound));

                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => gameSound.DisplayName,
                    tooltip: () => "Check if this sound should be disabled.",
                    getValue: () => (bool)property.GetValue(this.SoundsConfig),
                    setValue: isDisabled =>
                    {
                        if (isDisabled)
                        {
                            DisabledCodeSounds.Remove(gameSound.CodeName);
                        }
                        else
                        {
                            DisabledCodeSounds.TryAdd(gameSound.CodeName, isDisabled);
                        }
                        property.SetValue(this.SoundsConfig, isDisabled);
                    }
                );
            }
        }
    }
}
