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