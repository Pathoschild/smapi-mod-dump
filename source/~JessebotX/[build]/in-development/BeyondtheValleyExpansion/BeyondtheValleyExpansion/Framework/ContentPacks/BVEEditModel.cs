using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondTheValleyExpansion.Framework.ContentPacks
{
    class BVEEditModel
    {
        /// <summary> the file to replace that is relative to BeyondtheValleyExpansion's root folder (BeyondtheValleyExpansion/) </summary>
        public string ReplaceFile { get; set; } = "none";
        /// <summary> the content pack file to load instead that is relative to the content.json</summary>
        public string FromFile { get; set; } = "none";
        /// <summary> the specified map warps to change </summary>
        public string[] Warps { get; set; } = new string[] { };
        /// <summary> the specified if/else conditions for applying an asset </summary>
        public string Conditions { get; set; } = "none";
    }
}
