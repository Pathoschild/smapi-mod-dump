/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using StardewArchipelago.Archipelago;
using StardewArchipelago.Constants.Modded;
using StardewValley;

namespace StardewArchipelago.GameModifications.Modded
{
    public class ModifiedVillagerEventChecker
    {
        private const string JUNA_MAIL_ONE = "JunaWizardletter2";
        private const string JUNA_MAIL_TWO = "JunaWizardletter4";

        public void CheckJunaHearts(ArchipelagoClient _archipelago)
        {
            if (_archipelago == null || !_archipelago.SlotData.Mods.HasMod(ModNames.JUNA))
            {
                return;
            }

            var junaHearts = Game1.player.getFriendshipHeartLevelForNPC("Juna");
            if (junaHearts >= 3 && !Game1.player.hasOrWillReceiveMail(JUNA_MAIL_ONE))
            {
                Game1.player.mailForTomorrow.Add(JUNA_MAIL_ONE);
            }
            if (junaHearts >= 6 && !Game1.player.hasOrWillReceiveMail(JUNA_MAIL_TWO))
            {
                Game1.player.mailForTomorrow.Add(JUNA_MAIL_TWO);
            }
        }
    }
}