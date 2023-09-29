/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewValley.Tools;

namespace StardewArchipelago.Stardew
{
    public class MeleeWeaponToRecover : MeleeWeapon
    {
        public MeleeWeaponToRecover() : base()
        {

        }

        public MeleeWeaponToRecover(int weaponId) : base(weaponId)
        {

        }

        public override int salePrice()
        {
            return base.salePrice() * 4;
        }
    }
}
