/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Pong.Framework.Menus.Elements;
using IDrawable = Pong.Framework.Common.IDrawable;
using IUpdateable = Pong.Framework.Common.IUpdateable;

namespace Pong.Framework.Game
{
    internal interface IDrawableCollideable : IUpdateable, IDrawable, IBoundable
    {
    }
}
