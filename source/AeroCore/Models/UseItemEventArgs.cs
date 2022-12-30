/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Tlitookilakin/AeroCore
**
*************************************************/

using AeroCore.API;
using AeroCore.Utils;
using Microsoft.Xna.Framework;
using StardewValley;

namespace AeroCore.Models
{
    public class UseItemEventArgs : IUseItemEventArgs
    {
        public GameLocation Where { get; }
        public Point Tile { get; }
        public Farmer Who { get; }
        public bool NormalGameplay { get; }
        public bool IsTool { get; }
        public Item Item { get; }
        public string ItemStringID { get; }
        public int ToolPower { get; }
        public bool IsHandled
        {
            get => isHandled;
            set => isHandled = value || isHandled;
        }
        public bool ConsumeItem { get; set; } = true;

        private bool isHandled = false;

        internal UseItemEventArgs(Item what)
        {
            Where = Game1.currentLocation;
            Who = Game1.player;
            NormalGameplay = GetIsNormalGameplay();
            Item = what;
            IsTool = false;
            Tile = Who.getTileLocationPoint();
            ItemStringID = Item.GetStringID();
            ToolPower = 0;
        }
        internal UseItemEventArgs(Tool what, Point tile, Farmer who, GameLocation where, int power)
        {
            IsTool = true;
            Where = where;
            Who = who;
            Tile = tile;
            ToolPower = power;
            Item = what;
            NormalGameplay = GetIsNormalGameplay();
            ItemStringID = Item.GetStringID();
        }
        private static bool GetIsNormalGameplay()
        {
            return 
                !Game1.eventUp && 
                !Game1.isFestival() && 
                !Game1.fadeToBlack && 
                !Game1.player.swimming.Value && 
                !Game1.player.bathingClothes.Value && 
                !Game1.player.onBridge.Value;
        }
    }
}
