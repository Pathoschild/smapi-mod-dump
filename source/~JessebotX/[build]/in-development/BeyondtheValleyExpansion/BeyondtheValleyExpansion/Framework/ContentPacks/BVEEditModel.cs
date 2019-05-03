using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondTheValleyExpansion.Framework.ContentPacks
{
    class BVEEditModel
    {
        public string ReplaceFile { get; set; } = "none";
        public string FromFile { get; set; } = "none";
        public string[] Warps { get; set; } = new string[] { };
        public string Conditions { get; set; } = "none";
    }
}
