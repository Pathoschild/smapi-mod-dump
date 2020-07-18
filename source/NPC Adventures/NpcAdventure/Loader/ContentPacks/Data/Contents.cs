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
        public List<LegacyChanges> Changes { get; set; }
    }
}
