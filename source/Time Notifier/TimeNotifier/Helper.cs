using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeNotifier
{
    public static class Helper
    {
        public static short RoundUp(this short i)
        {
            return (short)(Math.Ceiling(i / 10.0) * 10);
        }

        // test if valid time 0600 to 2600
    }
}
