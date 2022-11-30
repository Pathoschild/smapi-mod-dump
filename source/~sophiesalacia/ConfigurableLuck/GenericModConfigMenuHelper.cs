/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sophiesalacia/StardewMods
**
*************************************************/

using StardewModdingAPI;

namespace ConfigurableLuck;

internal class GenericModConfigMenuHelper
{

    internal static void BuildConfigMenu()
    {
        // register mod
        Globals.GmcmApi.Register(
            Globals.Manifest,
            () => Globals.Config = new ModConfig(),
            () =>
                {
                    Log.Trace($"Luck val: {Globals.Config.LuckValue}");
                    Globals.Helper.WriteConfig(Globals.Config);

                    // if in-game and mod is enabled, adjust luck value immediately
                    if (!Context.IsWorldReady || !Globals.Config.Enabled)
                        return;

                    Globals.Config.ApplyConfigChangesToGame();
                }
            );

        Globals.GmcmApi.AddBoolOption(
            Globals.Manifest,
            name: () => "Enabled",
            tooltip: () => "If this option is selected, your daily luck will be overridden with the selected value.",
            getValue: () => Globals.Config.Enabled,
            setValue: val => Globals.Config.Enabled = val
        );

        Globals.GmcmApi.AddParagraph(
            Globals.Manifest,
            () => "Note: Vanilla luck goes from Very Bad to Very Good. This mod lets you set your luck outside those values (everything labeled Worst or Best). Unintended behavior may occur.");

        Globals.GmcmApi.AddNumberOption(
            Globals.Manifest,
            name: () => "Luck Value",
            tooltip: () => "The value to override your luck with.",
            getValue: () => (float)Globals.Config.LuckValue,
            setValue: val => Globals.Config.LuckValue = val,
            min: (float)LuckManager.MIN_LUCK_VALUE,
            max: (float)LuckManager.MAX_LUCK_VALUE,
            interval: 0.01f,
            formatValue: val =>
            {
                string formattedValue = val switch
                {
                    <= -0.12f => "Worst",
                    > -0.12f and < -0.07f => "Very Bad",
                    >= -0.07f and < -0.02f => "Bad",
                    >= -0.02f and <= 0.02f => "Neutral",
                    > 0.02f and <= 0.07f => "Good",
                    > 0.07f and < 0.12f => "Very Good",
                    >= 0.12f => "Best",
                    _ => "Unknown"
                };
                formattedValue += $"\n({val})";
                return formattedValue;
            }
        );
    }
}
