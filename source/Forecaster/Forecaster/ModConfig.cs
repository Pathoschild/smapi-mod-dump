using StardewModdingAPI;

class ModConfig {
    public bool showLuckForecastOnWakeUp { get; set; } = true;
    public bool showWeatherOnWakeUp { get; set; } = true;
    public int initialDelay { get; set; } = 1;
    public int offsetDelay { get; set; } = 2;
    public bool enableShortcutKeys { get; set; } = false;
    public SButton tipKey { get; set; } = SButton.OemOpenBrackets;
    public SButton weatherKey { get; set; } = SButton.OemCloseBrackets;
}