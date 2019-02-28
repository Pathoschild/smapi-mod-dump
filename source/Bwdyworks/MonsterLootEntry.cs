using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks
{
    public class MonsterLootEntry
    {
        public StardewModdingAPI.Mod Mod;
        public string MonsterID;
        public string ItemID;
        public float Weight;

        public MonsterLootEntry(StardewModdingAPI.Mod mod, string monster_id, string id, float weight)
        {
            Mod = mod;
            MonsterID = monster_id;
            ItemID = id;
            Weight = weight;
        }
    }
}
