/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.TerrainFeatures;
using System;

namespace WindEffects.Framework.Shakers
{
    internal class HoeDirtShaker : IShaker
    {
        private readonly HoeDirt dirt;
        private readonly bool left;
        public HoeDirtShaker(HoeDirt dirt, bool left)
        {
            this.dirt = dirt;
            this.left = left;

        }
        public void Shake(IReflectionHelper helper, Vector2 tile)
        {
            if (dirt.crop == null)
                return;

            helper.GetMethod(this.dirt, "shake").Invoke((float)(0.392699092626572 / 2), (float)(Math.PI / 80f), this.left);
        }
    }
}
