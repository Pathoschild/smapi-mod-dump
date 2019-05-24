using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mine_Changes.MineChanges.Config
{
    public class MineConfig
    {
        public MapLayoutChanges mapLayout = new MapLayoutChanges();
        public OreStoneChances stoneChances = new OreStoneChances();
        public List<BreakStone> breakStones = new List<BreakStone>();
        public List<StoneReplace> stoneReplacements = new List<StoneReplace>();
        public List<ItemReplace> itemReplacements = new List<ItemReplace>();

    }
}
