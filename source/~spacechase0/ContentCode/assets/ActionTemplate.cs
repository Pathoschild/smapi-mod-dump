/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using xTile;

namespace #REPLACE_packname
{
    public class Runner : global::ContentCode.BaseRunner
    {
        public override void Run( object[] args )
        {
            var arg = args[ 0 ] as SpaceCore.Events.EventArgsAction;
            ActualRun( arg.Position, arg.ActionString );
        }

        public void ActualRun(xTile.Dimensions.Location tilePosition, string actionString)
        {
            #REPLACE_code
        }
    }
}
