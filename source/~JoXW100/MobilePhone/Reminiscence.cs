/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JoXW100/StardewValleyMods
**
*************************************************/

using StardewValley;
using System.Collections.Generic;

namespace MobilePhone
{
    public class Reminiscence
    {
        public List<Reminisce> events = new List<Reminisce>();

        public void WeedOutUnseen()
        {
            if (events.Count == 0)
                return;
            for (int i = events.Count - 1; i >= 0; i--)
            {
                string ids = events[i].eventId;

                if (!Game1.player.eventsSeen.Contains(ids))
                    events.RemoveAt(i);
            }
        }
    }

    public class Reminisce
    {
        public string name;
        public string location;
        public string eventId;
        public bool night;
        public string mail;
    }
}