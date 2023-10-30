/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/chudders1231/SDV-FishingProgression
**
*************************************************/

using StardewModdingAPI;
using StardewValley.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FishingProgression.framework
{
    internal static class Globals
    {

        public static ModConfig Config { get; set; }
        public static IManifest Manifest { get; set; }
        public static IModHelper Helper { get; set; }

        internal static void InitializeConfig()
        {
            Config = Helper.ReadConfig<ModConfig>();
        }

        internal static void InitializeGlobals(ModEntry modEntry)
        {
            Log.Monitor = modEntry.Monitor;
            Manifest = modEntry.ModManifest;
            Helper = modEntry.Helper;
        }

        internal static void RegisterConfigMenu()
        {
            var configMenu = Globals.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            // register mod
            configMenu.Register(
                mod: Globals.Manifest,
                reset: () => Globals.Config = new ModConfig(),
                save: () => Globals.Helper.WriteConfig(Globals.Config)
            );

            configMenu.AddSectionTitle
            (
                mod: Globals.Manifest,
                text: () => "Fishing Progression"
            );

            configMenu.AddParagraph
            (
                mod: Globals.Manifest,
                text: () => "This mod focuses on passive and small, configurable features that you can use to tailor your fishing experience." +
                "\n\nThis mod will grow and expand with additional features driven by the community, with the aim of being as modular as possible."
            );
            configMenu.AddBoolOption(
            mod: Globals.Manifest,
            name: () => "Enable Difficulty Modifier",
                tooltip: () => "This will enable or disable the difficulty modifier when fishing.",
                getValue: () => Globals.Config.EnableDifficultyModifier,
                setValue: value => Globals.Config.EnableDifficultyModifier = value
            );

            configMenu.AddNumberOption(
                mod: Globals.Manifest,
                name: () => "Difficulty Modifier",
                tooltip: () => "This is the chance per level that the difficulty will be reduced by.",
                getValue: () => Globals.Config.DifficultyModifier,
                setValue: value => Globals.Config.DifficultyModifier = value,
                min: 0.0f,
                max: 7.5f,
                interval: 0.5f,
                formatValue: value => String.Format("{0:0.00}%", value)
            );

            configMenu.AddBoolOption(
                mod: Globals.Manifest,
                name: () => "Enable Tackle Restoration",
                tooltip: () => "This will enable or disable the chance to restore tackle durability.",
                getValue: () => Globals.Config.EnableTackleRestoration,
                setValue: value => Globals.Config.EnableTackleRestoration = value
            );

            configMenu.AddNumberOption(
                mod: Globals.Manifest,
                name: () => "Tackle Restoration",
                tooltip: () => "This is the chance per level that the tackle's durability will be restored.",
                getValue: () => Globals.Config.TackleRestorationChance,
                setValue: value => Globals.Config.TackleRestorationChance = value,
                min: 0,
                max: 10,
                interval: 1,
                formatValue: value => String.Format("{0:0.0}%", value)
            );

            configMenu.AddBoolOption(
                 mod: Globals.Manifest,
                 name: () => "Enable Double Hook",
                 tooltip: () => "This will enable or disable the chance to double hook (applies to both fish and trash).",
                 getValue: () => Globals.Config.EnableDoubleHook,
                 setValue: value => Globals.Config.EnableDoubleHook = value
             );

            configMenu.AddNumberOption(
                mod: Globals.Manifest,
                name: () => "Double Hook",
                tooltip: () => "This is the chance per level that you will double hook.",
                getValue: () => Globals.Config.DoubleHookChance,
                setValue: value => Globals.Config.DoubleHookChance = value,
                min: 0.0f,
                max: 7.5f,
                interval: 0.5f,
                formatValue: value => String.Format("{0:0.0}%", value)
            );
        }
    }
}
