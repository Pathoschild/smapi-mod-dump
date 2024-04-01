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
using CombatOverhaulMod.Combat.WeaponTypes;
using StardewValley.Tools;

namespace CombatOverhaulMod.Combat
{
    internal class WeaponTypeManager
    {
        private static Dictionary<int, WeaponType> weaponTypes = GetInitialWeaponTypes();

        public static WeaponType GetWeaponType(int type)
        {
            return weaponTypes[type];
        }

        private static Dictionary<int, WeaponType> GetInitialWeaponTypes()
        {
            Dictionary<int, WeaponType> ret = new();

            ret.Add(MeleeWeapon.defenseSword, new SwordWeaponType());
            ret.Add(MeleeWeapon.dagger, new DaggerWeaponType());
            ret.Add(MeleeWeapon.club, new ClubWeaponType());

            return ret;
        }
    }
}
