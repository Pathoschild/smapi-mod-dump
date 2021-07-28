/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using System.Xml.Serialization;
using StardewValley;
using StardewValley.Tools;

namespace MoreEnchantments.Enchantments
{
    [XmlType("Mods_MoreLuresEnchantment")]
    public class MoreLuresEnchantment : FishingRodEnchantment
    {
        public override string GetName()
        {
            return "A-lure-ing";
        }

        protected override void _ApplyTo(Item item)
        {
            base._ApplyTo(item);
            (item as FishingRod).numAttachmentSlots.Value = 3;
            (item as FishingRod).attachments.SetCount(3);
        }

        protected override void _UnapplyTo(Item item)
        {
            base._UnapplyTo(item);
            (item as FishingRod).numAttachmentSlots.Value = 2;
            (item as FishingRod).attachments.SetCount(2);
        }
    }
}
