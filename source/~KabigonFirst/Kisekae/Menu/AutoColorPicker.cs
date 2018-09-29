using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.Menus;

namespace Kisekae.Menu {
    class AutoColorPicker : ColorPicker, IAutoComponent {
        public string m_name { get; set; }
        public bool m_visible { get; set; } = true;
        public AutoColorPicker(string name, int x, int y) : base(x,y) {
            m_name = name;
        }
    }
}
