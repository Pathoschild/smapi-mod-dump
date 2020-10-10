/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/remybach/stardew-valley-forecaster
**
*************************************************/

using StardewModdingAPI;

internal class ModConfig {
    public bool ShowLuckForecastOnWakeUp { get; set; } = true;
    public bool ShowWeatherOnWakeUp { get; set; } = true;
    public int InitialDelay { get; set; } = 1;
    public int OffsetDelay { get; set; } = 2;
    public bool EnableShortcutKeys { get; set; } = false;
    public SButton TipKey { get; set; } = SButton.OemOpenBrackets;
    public SButton WeatherKey { get; set; } = SButton.OemCloseBrackets;
}
