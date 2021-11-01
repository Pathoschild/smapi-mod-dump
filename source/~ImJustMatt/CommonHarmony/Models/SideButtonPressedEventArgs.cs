/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ImJustMatt/StardewMods
**
*************************************************/

namespace CommonHarmony.Models
{
    using Enums;
    using StardewValley.Menus;

    internal record SideButtonPressedEventArgs
    {
        public SideButtonPressedEventArgs(ClickableTextureComponent cc, ButtonType type)
        {
            this.Button = cc;
            this.Type = type;
        }

        public ClickableTextureComponent Button { get; }

        public ButtonType Type { get; }
    }
}