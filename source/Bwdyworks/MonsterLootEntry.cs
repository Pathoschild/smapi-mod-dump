using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks
{
    public class MonsterLootEntry
    {
        public string Module;
        public string MonsterID;
        public string ItemID;
        public double Weight;

        public MonsterLootEntry(string module, string monster_id, string id, double weight)
        {
            Module = module;
            MonsterID = monster_id;
            ItemID = id;
            Weight = weight;
        }
    }
}
