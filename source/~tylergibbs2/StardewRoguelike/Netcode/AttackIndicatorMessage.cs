/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/StardewValleyMods
**
*************************************************/

using StardewRoguelike.UI;
using StardewValley;

namespace StardewRoguelike.Netcode
{
    internal class AttackIndicatorMessage
    {
        public string LocationName { get; set; } = null!;

        public int TickDuration { get; set; }

        public void Trigger()
        {
            if (Game1.player.currentLocation.Name == LocationName)
            {
                Game1.playSound("shadowpeep");
                Game1.onScreenMenus.Add(new AttackIndicator(TickDuration));
            }
        }
    }
}
