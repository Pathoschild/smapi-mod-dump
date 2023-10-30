/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Tools;

namespace StardewArchipelago.Stardew
{
    public class MeleeWeaponToRecover : MeleeWeapon
    {
        private int _weaponId;

        public MeleeWeaponToRecover() : base()
        {

        }

        public MeleeWeaponToRecover(int weaponId) : base(weaponId)
        {
            _weaponId = weaponId;
        }

        public override int salePrice()
        {
            return base.salePrice() * 4;
        }

        public override bool actionWhenPurchased()
        {
            var realWeapon = new MeleeWeapon(_weaponId);
            // Game1.player.itemsLostLastDeath.Clear(); No need to clear it
            this.isLostItem = false;
            Game1.player.recoveredItem = realWeapon;
            Game1.player.mailReceived.Remove("MarlonRecovery");
            Game1.addMailForTomorrow("MarlonRecovery");
            Game1.playSound("newArtifact");
            Game1.exitActiveMenu();
            bool flag = this.Stack > 1;
            Game1.drawDialogue(Game1.getCharacterFromName("Marlon"), Game1.content.LoadString(flag ? "Strings\\StringsFromCSFiles:ItemRecovery_Engaged_Stack" : "Strings\\StringsFromCSFiles:ItemRecovery_Engaged", (object)Lexicon.makePlural(this.DisplayName, !flag)));
            return true;
        }
    }
}
