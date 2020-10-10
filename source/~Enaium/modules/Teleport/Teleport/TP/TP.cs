/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium/Stardew_Valley_Mods
**
*************************************************/

namespace Teleport
{
    public class TPData
    {
        public string name { get; }

        public string locationName { get; }

        public int tileX { get; }
        
        public int tileY { get; }

        public TPData(string name, string locationName,int tileX,int tileY)
        {
            this.name = name;
            this.locationName = locationName;
            this.tileX = tileX;
            this.tileY = tileY;
        }
        
    }
}