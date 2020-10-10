/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/JPANv2/Stardew-Valley-Mine-Changes
**
*************************************************/

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
