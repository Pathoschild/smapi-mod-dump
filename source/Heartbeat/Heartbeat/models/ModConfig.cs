/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/embolden/Heartbeat
**
*************************************************/

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
