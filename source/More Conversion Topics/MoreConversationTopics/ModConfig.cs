/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MoreConversationTopics
**
*************************************************/

using System;
namespace MoreConversationTopics
{
    public class ModConfig
    {
        //public bool ExampleBoolean { get; set; }
        public int WeddingDuration { get; set; }
        public int LuauDuration { get; set; }
        public int BirthDuration { get; set; }

        public ModConfig()
        {
            // this.ExampleBoolean = true;
            this.WeddingDuration = 7;
            this.LuauDuration = 7;
            this.BirthDuration = 7;
        }
    }
}
