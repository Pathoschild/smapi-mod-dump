/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/cheesysteak/stardew-steak
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Menus;

namespace MoreMultiplayerInfo.Models
{
    public class PlayerIcon
    {
        public long PlayerId { get; set; }

        public ClickableTextureComponent WaitingIcon { get; set; }

        public ClickableTextureComponent OfflineIcon { get; set; }

        public Rectangle HeadshotPosition { get; set; }

        
    }
}