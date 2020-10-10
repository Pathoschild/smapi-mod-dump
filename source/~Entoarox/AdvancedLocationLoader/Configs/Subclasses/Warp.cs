/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Entoarox/StardewMods
**
*************************************************/

namespace Entoarox.AdvancedLocationLoader.Configs
{
    internal class Warp : TileInfo
    {
        /*********
        ** Accessors
        *********/
        public string TargetName;
        public int TargetX;
        public int TargetY;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Warp({this.MapName}@[{this.TileX}{','}{this.TileY}] => {this.TargetName}@[{this.TargetX}{','}{this.TargetY}])";
        }
    }
}
