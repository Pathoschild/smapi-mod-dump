/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Pong.Framework.Enums;

namespace Pong.Game
{
    internal class CollideInfo
    {
        public Orientation Orientation { get; }
        public double CollidePercentage { get; }

        public CollideInfo(Orientation o, double c)
        {
            this.Orientation = o;
            this.CollidePercentage = c;
        }
    }
}
