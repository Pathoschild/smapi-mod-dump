/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tylergibbs2/BattleRoyalley-Year2
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewValley;

namespace BattleRoyale.Patches
{
    class DrawPlayerNames : Patch
    {
        protected override PatchDescriptor GetPatchDescriptor() => new PatchDescriptor(typeof(Farmer), "draw");

        public static void Postfix(Farmer __instance, SpriteBatch b)
        {
            Round round = ModEntry.BRGame.GetActiveRound();
            if (round != null && !round.AlivePlayers.Contains(__instance))
                return;

            if (ModEntry.DisplayNames)
                PlayerNameBox.draw(b, __instance);
        }
    }
}
