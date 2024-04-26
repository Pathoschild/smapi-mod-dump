/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Armitxes/StardewValley_UnlimitedPlayers
**
*************************************************/

using StardewModdingAPI.Events;

namespace UnlimitedPlayers.Events.GameLoop
{
  public class TickEvents
  {
    public void FirstUpdateTick(object sender, GameLaunchedEventArgs e)
    {
      // Overwrite the player limit in Stardew Valley source code
      LazyHelper.UpdateHost();
      LazyHelper.UpdateClient();
    }
  }
}
