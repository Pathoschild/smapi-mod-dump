using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.FarmInfo
{
    /// <summary>
    /// A simple class that retains custom tilesheets for custom maps.
    /// 
    /// Currently Unused
    /// </summary>
    public class additionalTileSheet
    {
        public string sheetID;
        public string source;
        public string fileName;
        public int width;
        public int height;

        public additionalTileSheet() { }

        public additionalTileSheet(string sheetID, string source, string fileName, int width, int height)
        {
            this.sheetID = sheetID;
            this.source = source;
            this.fileName = fileName;
            this.width = width;
            this.height = height;
        }
    }
}
