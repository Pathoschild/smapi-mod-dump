/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewNametags
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using System;
using System.Reflection;

namespace StardewNametags
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        public static bool DisplayNames = true;

        public static bool AllowToggle = true;

        private static ModConfig Config;

        public ModEntry()
        {
            Harmony harmony = new("tylergibbs2.stardewnametags");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.SaveLoaded += (o, e) =>
            {
                if (Config.MultiplayerOnly && !Context.IsMultiplayer)
                {
                    DisplayNames = false;
                    AllowToggle = false;
                }
                else
                {
                    DisplayNames = true;
                    AllowToggle = true;
                }
            };

            helper.Events.Input.ButtonPressed += (o, e) =>
            {
                if (Config.ToggleKey.JustPressed() && AllowToggle)
                    DisplayNames = !DisplayNames;
            };

            helper.Events.GameLoop.GameLaunched += SetupGMCM;
        }

        private void SetupGMCM(object sender, GameLaunchedEventArgs e)
        {
            IGenericModConfigMenuApi configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            configMenu.OnFieldChanged(ModManifest, (o, e) =>
            {
                Config = Helper.ReadConfig<ModConfig>();
                if (Config.MultiplayerOnly && !Context.IsMultiplayer)
                {
                    DisplayNames = false;
                    AllowToggle = false;
                }
                else
                    AllowToggle = true;
            });

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Multiplayer Only",
                tooltip: () => "Whether or not nametags are only visible in multiplayer",
                getValue: () => Config.MultiplayerOnly,
                setValue: value => Config.MultiplayerOnly = value
            );

            configMenu.AddKeybindList(
                mod: ModManifest,
                name: () => "Toggle Key",
                tooltip: () => "The key used to toggle nametags",
                getValue: () => Config.ToggleKey,
                setValue: value => Config.ToggleKey = value
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Text Color",
                tooltip: () => "The hexadecimal color of the foreground text.",
                getValue: () => Config.TextColor.ToUpper(),
                setValue: value => Config.TextColor = value.ToUpper()
            );

            configMenu.AddTextOption(
                mod: ModManifest,
                name: () => "Background Color",
                tooltip: () => "The hexadecimal color of the background box.",
                getValue: () => Config.BackgroundColor.ToUpper(),
                setValue: value => Config.BackgroundColor = value.ToUpper()
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                name: () => "Background Opacity",
                tooltip: () => "The level of 'see-through' you want the backgrond box.",
                getValue: () => Config.BackgroundOpacity,
                setValue: value => Config.BackgroundOpacity = value
            );

            configMenu.AddBoolOption(
                mod: ModManifest,
                name: () => "Opacity on Text",
                tooltip: () => "Whether or not to apply the background opacity to the text",
                getValue: () => Config.AlsoApplyOpacityToText,
                setValue: value => Config.AlsoApplyOpacityToText = value
            );
        }

        private static Color ConvertFromHex(string s)
        {
            if (s.Length != 7)
                return Color.Gray;

            int r = Convert.ToInt32(s.Substring(1, 2), 16);
            int g = Convert.ToInt32(s.Substring(3, 2), 16);
            int b = Convert.ToInt32(s.Substring(5, 2), 16);
            return new Color(r, g, b);
        }

        public static Color GetTextColor()
        {
            return ConvertFromHex(Config.TextColor);
        }

        public static bool GetShouldApplyOpacityToText()
        {
            return Config.AlsoApplyOpacityToText;
        }

        public static Color GetBackgroundColor()
        {
            return ConvertFromHex(Config.BackgroundColor);
        }

        public static float GetBackgroundOpacity()
        {
            return Config.BackgroundOpacity;
        }
    }
}
