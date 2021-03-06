/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AlejandroAkbal/Stardew-Valley-Quick-Sell-Mod
**
*************************************************/

using StardewValley;

namespace Quick_Sell
{
    internal class ModUtils
    {
        public static void SendHUDMessage(string message, int type = HUDMessage.newQuest_type)
        {
            if (ModEntry.Config.EnableHUDMessages == false)
                return;

            Game1.addHUDMessage(new HUDMessage(message, type));
        }
    }
}