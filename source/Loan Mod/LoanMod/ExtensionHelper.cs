/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/legovader09/SDVLoanMod
**
*************************************************/

using StardewValley;

namespace LoanMod
{
    public class ExtensionHelper
    {
        public static void AddMessage(string message, int messageType)
        {
            Game1.addHUDMessage(new HUDMessage(message, messageType));
        }
    }
}
