/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EnhancedSlingshots.Framework.Enchantments
{
    //auto reload
    [XmlType("Mods_ytsc_ExpertEnchantment")]
    public class ExpertEnchantment : BaseEnchantment
    {
        public override bool CanApplyTo(Item item)
        {
            return item is Slingshot;
        }

        public override string GetName()
        {
            return ModEntry.Instance.i18n.Get("ExpertEnchantment");
        }
    }
}
