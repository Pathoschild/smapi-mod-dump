/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/atravita-mods/StardewMods
**
*************************************************/

namespace CameraPan.Framework;

/// <summary>
/// Manages console commands for this mod.
/// </summary>
internal static class ConsoleCommands
{
    /// <summary>
    /// Gets a value indicating whether or not the debug target circle should be drawn.
    /// </summary>
    internal static bool DrawMarker { get; private set; } =
#if DEBUG
    true;
#else
    false;
#endif

    /// <summary>
    /// Registers the console commands for this mod.
    /// </summary>
    /// <param name="commandHelper">console command helper.</param>
    internal static void Register(ICommandHelper commandHelper)
    {
        commandHelper.Add("av.camera.debug", "Sets whether or not to draw a circle on the target point for the camera.", ToggleDebug);
    }

    private static void ToggleDebug(string command, string[] args)
    {
        if (args.Length > 1)
        {
            ModEntry.ModMonitor.Log($"Expected at most one argument");
        }

        if (args.Length == 0)
        {
            DrawMarker = !DrawMarker;
        }
        else
        {
            switch (args[0])
            {
                case "enable":
                    DrawMarker = true;
                    break;
                case "disable":
                    DrawMarker = false;
                    break;
            }
        }

        if (!DrawMarker)
        {
            AssetManager.Reset();
        }

        ModEntry.ModMonitor.Log("Okay, the debug marker is " + (DrawMarker ? "enabled" : "disabled"), LogLevel.Info);
    }
}
