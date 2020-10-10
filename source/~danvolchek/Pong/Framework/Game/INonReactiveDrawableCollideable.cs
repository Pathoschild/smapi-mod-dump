/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Pong.Game;

namespace Pong.Framework.Game
{
    internal interface INonReactiveDrawableCollideable : IDrawableCollideable
    {
        CollideInfo GetCollideInfo(IReactiveDrawableCollideable other);

        void Resize();
    }
}
