/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/delixx/stardew-valley/unlockable-bundles
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unlockable_Bundles.Lib.AdvancedPricing
{
    public class AdvancedPricing
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<string> ContextTags { get; set; }
        public string ItemCopy { get; set; }
        public string ItemSprite { get; set; }
        public int ItemSpriteSize { get; set; }
        public string ItemSpriteAnimation { get; set; }
        public string SubItemCopy { get; set; }
        public string SubItemSprite { get; set; }
        public int SubItemSpriteSize { get; set; }
        public string SubItemSpriteAnimation { get; set; }
        public List<string> ItemTypes { get; set; }
    }
}
