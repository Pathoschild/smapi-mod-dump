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
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;
using AeroCore.Utils;

namespace AeroCore.Models
{
    public class UseObjectEventArgs : IUseObjectEventArgs
    {
        public SObject Object { get; }
        public Vector2 Tile { get; }
        public Farmer Who { get; }
        public string ObjectStringID { get; }
        public bool IsChecking { get; }
        public bool IsHandled
        {
            get => isHandled;
            set => isHandled = isHandled || value;
        }
        private bool isHandled;

        internal UseObjectEventArgs(Farmer who, bool checking, SObject obj, bool handled)
        {
            Object = obj;
            Who = who;
            Tile = obj.TileLocation;
            IsChecking = checking;
            isHandled = handled;
            ObjectStringID = obj.GetStringID();
        }
    }
}
