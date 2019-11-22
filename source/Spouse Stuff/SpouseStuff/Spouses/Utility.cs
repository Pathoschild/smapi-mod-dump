using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpouseStuff.Spouses
{
    class Utility
    {
        public static bool WithinRange(int target, int min, int max)
        {
            return target > min && target < max;
        }
    }
}
