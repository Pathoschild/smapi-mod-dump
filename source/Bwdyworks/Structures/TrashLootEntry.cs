using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.Structures
{
    public class TrashLootEntry
    {
        public string Module;
        public string ItemID;
        public int Filter;

        public TrashLootEntry(string module, string id, int filter = -1)
        {
            Module = module;
            ItemID = id;
            Filter = filter;
        }
    }
}
