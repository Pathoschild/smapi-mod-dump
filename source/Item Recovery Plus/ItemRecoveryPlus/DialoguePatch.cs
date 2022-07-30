/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunarDiver/StardewValley.ItemRecoveryPlus
**
*************************************************/

using System.Linq;
using HarmonyLib;
using StardewValley;
using StardewValley.Menus;

namespace ItemRecoveryPlus
{
    /// <summary>
    /// This patch is meant to replace the in-game method to answer a dialogue question in order to add our own options.
    /// </summary>
    [HarmonyPatch]
    internal static class DialoguePatch
    {
        // ReSharper disable once InconsistentNaming
        public static void Postfix(ref bool __result, string questionAndAnswer)
        {
            if(questionAndAnswer is not "adventureGuild_RecoveryPlus")
                return;

            Game1.drawDialogue(Game1.getCharacterFromName("Marlon"), I18n.Marlon_RecoveryInfo());

            Game1.afterDialogues += () =>
            {
                Game1.activeClickableMenu = new ConfirmationDialog(I18n.Question_Confirm(), player =>
                {
                    Game1.activeClickableMenu = new ShopMenu(player.itemsLostLastDeath.Cast<ISalable>().ToList());

                    player.itemsLostLastDeath.Clear();
                });
            };

            __result = true;
        }
    }
}
