/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;

namespace Entoarox.DynamicDungeons
{
    internal class ActionInfo
    {
        /*********
        ** Accessors
        *********/
        public Farmer Player { get; }
        public string Action { get; }
        public string[] Arguments { get; }
        public Vector2 Position { get; }


        /*********
        ** Public methods
        *********/
        public ActionInfo(Farmer player, string action, string[] arguments, Vector2 position)
        {
            this.Player = player;
            this.Action = action;
            this.Arguments = arguments;
            this.Position = position;
        }
    }
}