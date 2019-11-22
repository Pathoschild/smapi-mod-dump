using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomKissingMod
{
    public class NpcConfig
    {
        public string Name { get; set; }
        public int Frame { get; set; }
        public bool FrameDirectionRight { get; set; }
        public int? RequiredEvent { get; set; }
    }
}
