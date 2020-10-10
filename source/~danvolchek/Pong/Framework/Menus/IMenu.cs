/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using Pong.Framework.Common;
using System;

namespace Pong.Framework.Menus
{
    internal interface IMenu : IDrawable, IUpdateable, IInputable
    {
        void Resize();

        event EventHandler<SwitchMenuEventArgs> SwitchToNewMenu;
    }
}
