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
        private string _weaponId;

        public MeleeWeaponToRecover() : base()
        {
        }

        public MeleeWeaponToRecover(string weaponId) : base(weaponId)
        {
            _weaponId = weaponId;
        }

        public override int salePrice(bool ignoreProfitMargins = false)
        {
            return base.salePrice(ignoreProfitMargins) * 4;
        }

        public override bool actionWhenPurchased(string shopId)
        {
            var realWeapon = new MeleeWeapon(_weaponId);
            // Game1.player.itemsLostLastDeath.Clear(); No need to clear it
            isLostItem = false;
            Game1.player.recoveredItem = realWeapon;
            Game1.player.mailReceived.Remove("MarlonRecovery");
            Game1.addMailForTomorrow("MarlonRecovery");
            Game1.playSound("newArtifact");
            Game1.exitActiveMenu();
            var isStack = Stack > 1;
            var dialogueKey = isStack ? "Strings\\StringsFromCSFiles:ItemRecovery_Engaged_Stack" : "Strings\\StringsFromCSFiles:ItemRecovery_Engaged";
            var marlon = Game1.getCharacterFromName("Marlon");
            Game1.DrawDialogue(marlon, dialogueKey, Lexicon.makePlural(realWeapon.DisplayName, !isStack));
            return true;
        }
    }
}
