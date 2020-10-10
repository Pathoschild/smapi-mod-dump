/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KabigonFirst/StardewValleyMods
**
*************************************************/

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
