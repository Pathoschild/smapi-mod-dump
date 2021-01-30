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
        private readonly ModConfig Config;

        public ModUtils(ModConfig config)
        {
            this.Config = config;
        }

        public void SendHUDMessage(string message, int type = HUDMessage.newQuest_type)
        {
            if (this.Config.EnableHUDMessages == true)
                Game1.addHUDMessage(new HUDMessage(message, type));
        }
    }
}