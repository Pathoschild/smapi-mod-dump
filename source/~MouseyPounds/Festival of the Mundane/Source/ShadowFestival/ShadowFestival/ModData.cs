using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace ShadowFestival
{
    class ModData
    {
        public List<string> CalmingHats { get; set; } = new List<string> {
            "Imposing Mask", "Shamanic Mask", "Shady Mask", "Shady Bowed Mask"
        };
        public List<string> OtherHats { get; set; } = new List<string> {
            "Strange Bun Hat"
        };
    }
}
