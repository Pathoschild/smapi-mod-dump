using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sdv_helper.Detectors
{
    interface IDetector
    {
        IDetector SetLocation(GameLocation loc);
        EntityList Detect();
    }
}
