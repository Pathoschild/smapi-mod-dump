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
using StardewValley.Monsters;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EnhancedSlingshots.Framework.Enchantments
{
	//more damage to insect enemies
	[XmlType("Mods_ytsc_BugKillerEnchantment")]
	public class BugKillerEnchantment : BaseEnchantment
	{		
		public override bool CanApplyTo(Item item)
		{
			return item is Slingshot;
		}
		protected override void _OnDealDamage(Monster monster, GameLocation location, Farmer who, ref int amount)
		{
			if (monster is Grub || monster is Fly || monster is Bug || monster is Leaper || monster is LavaCrab || monster is RockCrab)
			{
				amount = (int)(amount * ModEntry.Instance.config.BugKillerEnchantment_Damage);
			}
		}

		public override string GetName()
		{
			return ModEntry.Instance.i18n.Get("BugKillerEnchantment");
		}
	}
}
