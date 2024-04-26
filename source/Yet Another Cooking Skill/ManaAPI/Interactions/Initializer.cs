/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Pet-Slime/StardewValley
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;

namespace ManaBar.Interactions
{
    internal static class Initializer
    {
        public static void InitializeModMenu(IModHelper helper)
        {
            // Get Generic Mod Config Menu's API (if it's installed).
            var configMenu = helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (configMenu is null)
                return;

            // Register mod.
            configMenu.Register(
                mod: ModEntry.Instance.ModManifest,
                reset: () => ModEntry.Config = new ModConfig(),
                save: () => helper.WriteConfig(ModEntry.Config)
            );

            #region Main Setting.

            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: () => helper.Translation.Get("main-setting")
            );

            // Render ManaBar?
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: () => helper.Translation.Get("render-setting"),
                tooltip: () => helper.Translation.Get("render-setting-des"),
                getValue: () => ModEntry.Config.RenderManaBar,
                setValue: value => ModEntry.Config.RenderManaBar = value
            );
            #endregion

            #region Positions Settings.

            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: () => helper.Translation.Get("position-settings")
            );

            // Bar Moving
            configMenu.AddBoolOption(
                mod: ModEntry.Instance.ModManifest,
                name: () => helper.Translation.Get("bar-moving"),
                tooltip: () => helper.Translation.Get("bar-moving-des"),
                getValue: () => ModEntry.Config.BarsPosition,
                setValue: value => ModEntry.Config.BarsPosition = value
            );

            // X Position Offset.
            configMenu.AddTextOption(
                mod: ModEntry.Instance.ModManifest,
                name: () => helper.Translation.Get("bar-x-position"),
                tooltip: () => helper.Translation.Get("bar-x-position-des"),
                getValue: () => ModEntry.Config.XManaBarOffset.ToString(),
                setValue: value =>
                {
                    if (int.TryParse(value, out int result))
                        ModEntry.Config.XManaBarOffset = result;
                }
            );

            // Y Position Offset.
            configMenu.AddTextOption(
                mod: ModEntry.Instance.ModManifest,
                name: () => helper.Translation.Get("bar-y-position"),
                tooltip: () => helper.Translation.Get("bar-y-position-des"),
                getValue: () => ModEntry.Config.YManaBarOffset.ToString(),
                setValue: value =>
                {
                    if (int.TryParse(value, out int result))
                        ModEntry.Config.YManaBarOffset = result;
                }
            );
            #endregion

            #region Additional Settings.

            configMenu.AddSectionTitle(
                mod: ModEntry.Instance.ModManifest,
                text: () => helper.Translation.Get("additional-settings")
            );

            // Bar Size Multiplier.
            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: () => helper.Translation.Get("size-multiplier"),
                tooltip: () => helper.Translation.Get("size-multiplier-des"),
                getValue: () => ModEntry.Config.SizeMultiplier,
                setValue: value => ModEntry.Config.SizeMultiplier = value,
                interval: 0.5f,
                min: 5f,
                max: 35f
            );

            // Overcharge Max Value.
            configMenu.AddNumberOption(
                mod: ModEntry.Instance.ModManifest,
                name: () => helper.Translation.Get("overcharge-max-value"),
                tooltip: () => helper.Translation.Get("overcharge-max-value-des"),
                getValue: () => ModEntry.Config.MaxOverchargeValue,
                setValue: value => ModEntry.Config.MaxOverchargeValue = value,
                interval: 0.1f,
                min: 1f,
                max: 20f
            );
            #endregion
        }
    }
}
