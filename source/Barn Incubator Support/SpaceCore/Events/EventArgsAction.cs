/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/StardewValleyMods
**
*************************************************/

using SpaceShared;
using xTile.Dimensions;

namespace SpaceCore.Events
{
    public class EventArgsAction : CancelableEventArgs
    {
        internal EventArgsAction(bool touch, string action, Location pos)
        {
            this.TouchAction = touch;
            this.Action = action.Split(' ')[0];
            this.ActionString = action;
            this.Position = pos;
        }

        public bool TouchAction { get; }
        public string Action { get; }
        public string ActionString { get; }
        public Location Position { get; } // Not valid for TouchActions
    }
}
