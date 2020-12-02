/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Loader.ContentPacks.Data
{
    class Contents
    {
        public string Format { get; set; }
        public Dictionary<string, string> Companions { get; set; }
        public List<Dialogues> Dialogues { get; set; }

        // Legacy field (formats 1.1 - 1.3)
        public List<ContentChange> Changes { get; set; }
    }
}
