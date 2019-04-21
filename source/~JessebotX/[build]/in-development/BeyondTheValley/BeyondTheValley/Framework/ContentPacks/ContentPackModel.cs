using BeyondTheValley.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondTheValley.Framework.ContentPacks
{
    class ContentPackModel
    {
        public List<BVEEditModel> ReplaceFiles { get; set; } = new List<BVEEditModel>();
        public List<BVEEditModel> EditFiles { get; set; } = new List<BVEEditModel>();
        public List<BVEEditModel> MapWarps { get; set; } = new List<BVEEditModel>();
    }
}
