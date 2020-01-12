using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerFrameworkMod.ContentPack
{
    public class ProducerConfig
    {
        public string ProducerName;
        public bool AlternateFrameProducing;
        public bool AlternateFrameWhenReady;

        public ProducerConfig(string producerName, bool alternateFrameProducing = false, bool alternateFrameWhenReady = false)
        {
            ProducerName = producerName;
            AlternateFrameProducing = alternateFrameProducing;
            AlternateFrameWhenReady = alternateFrameWhenReady;
        }
    }
}
