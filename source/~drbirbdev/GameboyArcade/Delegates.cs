/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using Microsoft.Xna.Framework;
using StardewValley;

namespace GameboyArcade;

[SDelegate]
internal class Delegates
{

    [SDelegate.TileAction]
    public static bool Play(GameLocation gameLocation, string[] args, Farmer farmer, Point point)
    {
        if (args is null || args.Length < 2)
        {
            Log.Error("drbirbdev.GameboyArcade_Play tile action requires GameID parameter");
            return true;
        }
        Content content = ModEntry.GetGame(args[1]);
        Utilities.ShowArcadeMenu(content.UniqueID, content.Name);
        return true;
    }

    [SDelegate.EventCommand]
    public static void StartGame(Event @event, string[] args, EventContext context)
    {
        if (args is null || args.Length < 2)
        {
            Log.Error("drbirbdev.GameboyArcade_StartGame event command requires GameID parameter");
            return;
        }

        if (Game1.currentMinigame != null)
        {
            return;
        }

        Content content = ModEntry.GetGame(args[1]);
        if (content is null)
        {
            Log.Error($"Minigame [{args?[1]}] does not exist.");
            return;
        }

        if (!content.EnableEvents)
        {
            Log.Error($"Event is attempting to use minigame {content.UniqueID} in a cutscene, but that content pack has disallowed this.");
            return;
        }

        GameboyMinigame.LoadGame(content, true);
        @event.CurrentCommand++;

    }

}
