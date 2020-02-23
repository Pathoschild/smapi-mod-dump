using StardewModdingAPI;
using System.Collections.Generic;

class ModConfig
{
    public static string DEFAULT_FOLDER = "default";
    private const float DEFAULT_ZOOM = 0.25f;

    public int TimeScreenshotGetsTakenAfter { get; set; }
    public float TakeScreenshotZoomLevel { get; set; }
    public SButton TakeScreenshotKey { get; set; }
    public float TakeScreenshotKeyZoomLevel { get; set; }
    public string FolderDestinationForDailyScreenshots { get; set; }
    public string FolderDestinationForKeypressScreenshots { get; set; }
    public Dictionary<string, bool> HowOftenToTakeScreenshot { get; set; }
    public bool TakeScreenshotOnRainyDays { get; set; }

    public ModConfig()
    {
        TimeScreenshotGetsTakenAfter = 600; // 6:00 AM
        TakeScreenshotZoomLevel = DEFAULT_ZOOM; // zoomed out to view entire map
        TakeScreenshotKey = SButton.None;
        TakeScreenshotKeyZoomLevel = DEFAULT_ZOOM; // zoomed out to view entire map
        FolderDestinationForDailyScreenshots = DEFAULT_FOLDER;
        FolderDestinationForKeypressScreenshots = DEFAULT_FOLDER;
        TakeScreenshotOnRainyDays = true;

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