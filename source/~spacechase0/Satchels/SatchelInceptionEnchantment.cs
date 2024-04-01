/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using StardewValley;
using StardewValley.Enchantments;

namespace Satchels
{
    [XmlType("Mods_spacechase0_Satchels_SatchelInceptionEnchantment")]
    public class SatchelInceptionEnchantment : BaseEnchantment
    {
        public override bool CanApplyTo(Item item)
        {
            return (item is Satchel);
        }

        public override string GetName()
        {
            return "Satchel Inception";
        }
    }
}
