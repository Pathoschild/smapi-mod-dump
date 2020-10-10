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
    internal class Property : TileInfo
    {
        /*********
        ** Accessors
        *********/
        public string Key;
        public string LayerId;
        public string Value;


        /*********
        ** Public methods
        *********/
        public override string ToString()
        {
            return $"Property({this.MapName}@[{this.TileX}{','}{this.TileY}]:{this.LayerId} => `{this.Key}` = {this.Value}{')'}";
        }
    }
}
