/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/danvolchek/StardewMods
**
*************************************************/

using System;

namespace Pong.Framework.Menus
{
    internal class SwitchMenuEventArgs : EventArgs
    {
        public IMenu NewMenu { get; }

        public SwitchMenuEventArgs(IMenu newMenu)
        {
            this.NewMenu = newMenu;
        }
    }
}
