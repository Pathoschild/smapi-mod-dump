/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mekadrom/IFramesInMines
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace IFramesInMines
{
    internal sealed class IFramesInMinesMod : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.OnWarped;
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation != e.OldLocation && e.NewLocation.NameOrUniqueName.ToLower().StartsWith("undergroundmine"))
            {
                e.Player.currentTemporaryInvincibilityDuration = 1500;
                e.Player.temporarilyInvincible = true;
            }
        }
    }
}
