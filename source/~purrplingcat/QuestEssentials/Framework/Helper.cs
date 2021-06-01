/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/StardewMods
**
*************************************************/

using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuestEssentials.Framework
{
    internal static class Helper
    {
        public static bool CheckItemContextTags(Item item, string tags)
        {
            if (string.IsNullOrEmpty(tags))
                return true;

            bool fail = false;

            foreach (string tagArray in tags.Split(','))
            {
                bool foundMatch = false;

                foreach (string tag in tagArray.Split('/'))
                {
                    if (item.HasContextTag(tag.Trim()))
                    {
                        foundMatch = true;
                        break;
                    }
                }

                if (!foundMatch)
                {
                    fail = true;
                }
            }

            return !fail;
        }

        public static void StartEventFrom(this GameLocation location, string source)
        {
            string[] eventInfo = source.Split(' ');

            if (eventInfo.Length < 2)
                QuestEssentialsMod.ModMonitor.Log("Invalid event source format. It requires `<int:EventId> <string:pathToScript>` where path to script must be `<strin g:GameContentFile>:<string:Key>`", StardewModdingAPI.LogLevel.Error);

            int eventId = Convert.ToInt32(eventInfo[0]);
            string scriptPath = string.Join(" ", eventInfo.Skip(1));

            StartEventFrom(location, eventId, scriptPath);
        }

        public static void StartEventFrom(this GameLocation location, int eventId, string scriptPath)
        {
            Game1.player.Halt();
            Game1.exitActiveMenu();
            Game1.dialogueUp = false;
            Game1.currentSpeaker = null;
            Game1.globalFadeToBlack(delegate
            {
                location.startEvent(new Event(Game1.content.LoadString(scriptPath), eventId));
            });
        }
    }
}
