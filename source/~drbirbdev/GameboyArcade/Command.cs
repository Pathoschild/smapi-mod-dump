/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using System.IO;
using BirbShared;
using BirbShared.Command;
using StardewValley;

namespace GameboyArcade
{
    [CommandClass(Prefix = "arcade_")]
    class Command
    {
        [CommandMethod("Play a loaded rom")]
        public static void Play(string game)
        {
            if (!ModEntry.LoadedContentPacks.TryGetValue(game, out Content content))
            {
                foreach (Content search in ModEntry.LoadedContentPacks.Values)
                {
                    if (search.Name == game)
                    {
                        content = search;
                        break;
                    }
                }
            }
            if (content is null)
            {
                Log.Error($"Could not find game {game}");
                return;
            }
            GameboyMinigame.LoadGame(content);
        }

        [CommandMethod("List all loaded roms")]
        public static void List()
        {
            string list = "\n";
            foreach (string id in ModEntry.LoadedContentPacks.Keys)
            {
                list += $"{id}\n";
            }
            Log.Info(list);
        }
    }
}
