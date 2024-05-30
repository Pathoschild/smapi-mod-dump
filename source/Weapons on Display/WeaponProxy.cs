/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/WeaponsOnDisplay
**
*************************************************/

using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;

namespace WeaponsOnDisplay
{
	public class WeaponProxy : Object
	{
		public MeleeWeapon Weapon { get; set; } = null;

		public new int ParentSheetIndex
		{
			get { return Weapon?.ParentSheetIndex ?? 0; }
			set
			{
				if (Weapon != null)
				{
					Weapon.ParentSheetIndex = value;
				}
			}
		}

		public WeaponProxy(MeleeWeapon weapon)
		{
			Weapon = weapon;
		}

		public WeaponProxy() {}

		protected override Item GetOneNew()
		{
			return Weapon.getOne();
		}

		public override bool performDropDownAction(Farmer who)
		{
			return false;
		}
	}
}