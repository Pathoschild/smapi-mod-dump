/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using System.Collections.Generic;

namespace StardewDruid.Journal
{
    public class Page
    {
        public string title;
        public string description;
        public List<string> objectives;
        public List<string> transcript;
        public string icon;
        public bool active;

        public Page()
        {
            objectives = new();
            transcript = new();
        }
    }
}