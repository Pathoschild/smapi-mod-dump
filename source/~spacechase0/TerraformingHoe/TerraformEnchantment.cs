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

namespace TerraformingHoe
{
    [XmlType("Mods_spacechase0_TerraformingHoe_TerraformEnchantment")]
    public class TerraformEnchantment : HoeEnchantment
    {
        public TerraformEnchantment()
        {
        }

        public override string GetName()
        {
            return "Terraform";
        }
    }
}
