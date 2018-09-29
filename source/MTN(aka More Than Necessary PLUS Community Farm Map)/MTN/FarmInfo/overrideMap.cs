using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.FarmInfo
{
    public class overrideMap
    {
        public fileType type { get; set; } = fileType.xnb;

        public string Location { get; set; }
        public string FileName { get; set; }

        public overrideMap() { }

        public overrideMap(string Location, string FileName, fileType type)
        {
            this.Location = Location;
            this.FileName = FileName;
            this.type = type;
        }
    }
}
