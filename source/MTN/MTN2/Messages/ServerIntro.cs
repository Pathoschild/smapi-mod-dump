using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2.Messages
{
    public class ServerIntro
    {
        public bool Canon { get; set; }
        public string FarmType { get; set; }
        public int WhichFarmId { get; set; }
    }
}
