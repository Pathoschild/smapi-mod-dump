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
