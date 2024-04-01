/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace ObtainableCowboyBoots
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void AddSectionTitle(IManifest mod, Func<string> text, Func<string> tooltip = null);

        void AddParagraph(IManifest mod, Func<string> text);

        void AddBoolOption(IManifest mod, Func<bool> getValue, Action<bool> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<int> getValue, Action<int> setValue, Func<string> name, Func<string> tooltip = null, int? min = null, int? max = null, int? interval = null, Func<int, string> formatValue = null, string fieldId = null);

        void AddNumberOption(IManifest mod, Func<float> getValue, Action<float> setValue, Func<string> name, Func<string> tooltip = null, float? min = null, float? max = null, float? interval = null, Func<float, string> formatValue = null, string fieldId = null);

        void AddTextOption(IManifest mod, Func<string> getValue, Action<string> setValue, Func<string> name, Func<string> tooltip = null, string[] allowedValues = null, Func<string, string> formatAllowedValue = null, string fieldId = null);

        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
    }

    public class ObtainableCowboyBootsConfig
    {
        public bool MovementSpeedWhileOnSandOrDirt { get; set; } = true;

        public float BonusOnFootMovementSpeed { get; set; } = 2f;

        public float BonusHorseMovementSpeed { get; set; } = 0.5f;

        public bool SkipHatCountIfBeatenCowboyArcadeGame { get; set; } = true;

        public bool CanOnlyObtainOnce { get; set; } = true;

        public int RequiredCowboyHats { get; set; } = 4;

        public static void VerifyConfigValues(ObtainableCowboyBootsConfig config, ObtainableCowboyBoots mod)
        {
            bool invalidConfig = false;

            if (config.BonusOnFootMovementSpeed < 0f)
            {
                config.BonusOnFootMovementSpeed = 0f;
                invalidConfig = true;
            }

            if (config.BonusHorseMovementSpeed < 0f)
            {
                config.BonusHorseMovementSpeed = 0f;
                invalidConfig = true;
            }

            if (config.RequiredCowboyHats < 1)
            {
                config.RequiredCowboyHats = 1;
                invalidConfig = true;
            }

            if (invalidConfig)
            {
                mod.DebugLog("At least one config value was out of range and was reset.");
                mod.Helper.WriteConfig(config);
            }
        }

        public static void SetUpModConfigMenu(ObtainableCowboyBootsConfig config, ObtainableCowboyBoots mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: delegate
                {
                    config = new ObtainableCowboyBootsConfig();
                },
                save: delegate
                {
                    mod.Helper.WriteConfig(config);
                    VerifyConfigValues(config, mod);
                }
            );

            api.AddSectionTitle(manifest, () => "Movement Speed Buff");

            api.AddBoolOption(manifest, () => config.MovementSpeedWhileOnSandOrDirt, (bool val) => config.MovementSpeedWhileOnSandOrDirt = val, () => "Movement Speed While\nOn Sand Or Dirt");
            api.AddNumberOption(manifest, () => config.BonusOnFootMovementSpeed, (float val) => config.BonusOnFootMovementSpeed = val, () => "Bonus On Foot Movement Speed", null, 0);
            api.AddNumberOption(manifest, () => config.BonusHorseMovementSpeed, (float val) => config.BonusHorseMovementSpeed = val, () => "Bonus Horse Movement Speed", null, 0);

            api.AddSectionTitle(manifest, () => "Unlock Requirement");

            api.AddBoolOption(manifest, () => config.SkipHatCountIfBeatenCowboyArcadeGame, (bool val) => config.SkipHatCountIfBeatenCowboyArcadeGame = val, () => "Skip Hat Count If Beaten\nCowboy Arcade Game", null);
            api.AddNumberOption(manifest, () => config.RequiredCowboyHats, (int val) => config.RequiredCowboyHats = val, () => "Required Cowboy Hats", null, 1);
            api.AddBoolOption(manifest, () => config.CanOnlyObtainOnce, (bool val) => config.CanOnlyObtainOnce = val, () => "Can Only Obtain Once", null);
        }
    }
}