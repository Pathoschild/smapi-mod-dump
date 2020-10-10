/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MouseyPounds/stardew-mods
**
*************************************************/

using System.Collections.Generic;

namespace AnythingPonds
{
    class AnythingPondsSaveData
    {
        public IDictionary<string,int> EmptyPonds { get; set; }

        public AnythingPondsSaveData()
        {
            this.EmptyPonds = new Dictionary<string, int>();
        }
    }
}
