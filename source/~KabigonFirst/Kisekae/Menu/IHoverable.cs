using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kisekae.Menu {
    interface IHoverable {
        float m_hoverScale { get; set; }
        void tryHover(int x, int y, float scale);
    }
}
