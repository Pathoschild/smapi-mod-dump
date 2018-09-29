using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using StardewModdingAPI;

namespace Kisekae.Menu {
    abstract class TabMenu : AutoMenu {
        protected int m_tabYOffset = 0;

        public TabMenu(IMod env, int x, int y, int width, int height, bool showUpperRightCloseButton = false) : base(env,x,y,width,height,showUpperRightCloseButton) {
            m_drawBlackFade = false;
            m_drawMenuFrame = false;
        }
        public virtual void onSwitchBack() {
            updateLayout();
        }
    }
}
