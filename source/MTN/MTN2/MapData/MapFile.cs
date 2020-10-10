/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Dawilly/MTN2
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.MapData
{
    public class MapFile {
        //The Type of file the map is stored (.xnb or .tbin (raw))
        public FileType FileType { get; set; } = FileType.xnb;
        //The file name containing the map data.
        public string FileName { get; set; }
        //The type of class said map should be based on. Default is GameLocation (the absolute basis)
        public string MapType { get; set; } = "GameLocation";
        //The name of the map.
        public string Name { get; set; } = "Untitled";

        public MapFile() { }

        public MapFile(string FileName) {
            this.FileName = FileName;
            MapType = "GameLocation";
            Name = "Untitled";
        }

        public MapFile(string FileName, string Name, FileType fileType) {
            this.FileName = FileName;
            this.Name = Name;
            this.FileType = fileType;
        }

        public MapFile(string FileName, string MapType, string Name) {
            this.FileName = FileName;
            this.MapType = MapType;
            this.Name = Name;
        }

        public MapFile(string FileName, string MapType, string Name, FileType fileType) {
            this.FileName = FileName;
            this.MapType = MapType;
            this.Name = Name;
            this.FileType = fileType;
        }
    }
}
