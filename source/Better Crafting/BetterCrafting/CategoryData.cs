/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RedstoneBoy/BetterCrafting
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BetterCrafting
{
    public class CategoryData
    {
        public Dictionary<string, string> categoryNames { get; set; } = new Dictionary<string, string>();
        public Dictionary<string, List<string>> categories { get; set; } = new Dictionary<string, List<string>>();
    }
}
