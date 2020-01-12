using StardewModdingAPI;
using System.Collections.Generic;

class ModConfig
{
    public int TimeScreenshotGetsTakenAfter { get; set; }
    public SButton TakeScreenshotKey { get; set; }
    public float TakeScreenshotKeyZoomLevel { get; set; }
    public string FolderDestinationForDailyScreenshots { get; set; }
    public string FolderDestinationForKeypressScreenshots { get; set; }
    
    public Dictionary<string, bool> HowOftenToTakeScreenshot { get; set; }

    public ModConfig()
    {
        TimeScreenshotGetsTakenAfter = 600; // 6:00 AM
        TakeScreenshotKey = SButton.None;
        TakeScreenshotKeyZoomLevel = 0.25f; // zoomed out to view entire map
        FolderDestinationForDailyScreenshots = "default";
        FolderDestinationForKeypressScreenshots = "default";

        HowOftenToTakeScreenshot = new Dictionary<string, bool>
        {
            {"Daily", true},
            {"Mondays", true},
            {"Tuesdays", true},
            {"Wednesdays", true},
            {"Thursdays", true},
            {"Fridays", true},
            {"Saturdays", true},
            {"Sundays", true},
            {"First Day of Month", true},
            {"Last Day of Month", true}
        };
    }
}