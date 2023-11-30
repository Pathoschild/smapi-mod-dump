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

namespace GameboyArcade;

[SCommand("gameboy")]
class Command
{
    [SCommand.Command("Play a loaded rom")]
    public static void Play(string game)
    {
        Content gameToPlay = ModEntry.SearchGames(game);

        if (gameToPlay is null)
        {
            Log.Error($"Could not find game {game}");
            return;
        }
        GameboyMinigame.LoadGame(gameToPlay);
    }

    [SCommand.Command("List all loaded roms")]
    public static void List()
    {
        foreach (Content content in ModEntry.AllGames())
        {
            Log.Info(content.Name);
        }
    }
}
