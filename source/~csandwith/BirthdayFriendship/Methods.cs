/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using StardewValley;

namespace BirthdayFriendship
{
    public partial class ModEntry
    {
        private static bool CheckBirthday(NPC npc)
        {
            if (!Config.ModEnabled)
                return npc.isVillager();
            return npc.isVillager() && Game1.player.friendshipData.TryGetValue(npc.Name, out Friendship f) && f.Points >= Config.Hearts * 250;
        }
    }
}