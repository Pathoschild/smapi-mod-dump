using System;
using System.Collections.Generic;
using System.Text;

namespace Heartbeat
{
    class ModConfig
    {
        public bool HeartBeatEnabled { get; set; } = true;
        public float HeartBeatAlertPercent { get; set; } = 45.0F;
        public float HeartBeatHeartRate { get; set; } = 80.0F;
    }
}
