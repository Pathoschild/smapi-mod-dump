using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.Events
{
    public class CancelableEventArgs : EventArgs
    {
        public bool Cancelled { get; set; }
    }
}
