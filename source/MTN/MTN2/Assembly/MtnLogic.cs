using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN2 {
    public abstract class MtnLogic {
        public abstract void HourUpdate();
        public abstract void DayUpdate();
        public abstract void SeasonUpdate();
        public abstract void YearUpdate();
    }
}
