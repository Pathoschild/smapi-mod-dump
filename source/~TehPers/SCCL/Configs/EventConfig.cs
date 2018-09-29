using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.Configs {
    public class EventConfig {
        public bool Enabled { get; set; } = true;

        public ModEvent[] Events { get; set; } = new ModEvent[] {
            new ModEvent("Location1", "Condition1", "Data1"),
            new ModEvent("Location2", "Condition2", "Data2"),
            new ModEvent("Location3", "Condition3", "Data3")
        };
    }

    public class ModEvent {
        public string Location { get; set; }
        public string Condition { get; set; }
        public string Data { get; set; }
        public bool Repeatable { get; set; }

        public ModEvent(string location, string condition, string data, bool repeatable = true) {
            this.Location = location;
            this.Condition = condition;
            this.Data = data;
            this.Repeatable = repeatable;
        }

        public int? getID() {
            string[] d = this.Condition.Split('/');
            if (d.Length > 0)
                return Convert.ToInt32(d[0]);
            return null;
        }
    }
}
