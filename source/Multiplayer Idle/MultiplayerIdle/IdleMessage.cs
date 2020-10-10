/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/SourceZh/MultiplayerIdle
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiplayerIdle {
    class IdleMessage {
        public bool Idle;
        public IdleMessage(bool isIdle = true) {
            Idle = isIdle;
        }
    }
    class ShowIdleMessage {
        public bool Idle;
        public ShowIdleMessage(bool isIdle = true) {
            Idle = isIdle;
        }
    }
}
