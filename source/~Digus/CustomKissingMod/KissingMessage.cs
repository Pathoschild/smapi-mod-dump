using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomKissingMod
{
    public class KissingMessage
    {
        public long Kisser;
        public long Kissed;

        public KissingMessage(long kisser, long kissed)
        {
            Kisser = kisser;
            Kissed = kissed;
        }
    }
}
