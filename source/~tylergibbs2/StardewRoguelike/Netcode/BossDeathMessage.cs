/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

namespace StardewRoguelike.Netcode
{
    internal class BossDeathMessage
    {
        public string BossName { get; set; } = "";

        public int KillSeconds { get; set; } = 0;

        public BossDeathMessage() { }

        public BossDeathMessage(string bossName, int killSeconds)
        {
            BossName = bossName;
            KillSeconds = killSeconds;
        }
    }
}
