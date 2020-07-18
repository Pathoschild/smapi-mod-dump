using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Loader.ContentPacks.Data
{
    class LegacyChanges
    {
        public string Action { get; set; }
        public string Target { get; set; }
        public string FromFile { get; set; }
        public string Locale { get; set; }
        public string LogName { get; set; }
        public bool Disabled { get; internal set; }
    }
}
