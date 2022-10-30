/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MissCoriel/Event-Repeater
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace EventRepeater
{
    class EventRepeaterWindow : IClickableMenu
    {
        public ClickableTextureComponent? forgetEvent;
        public ClickableTextureComponent? forgetMail;
        public ClickableTextureComponent? forgetResponse;
        public ClickableTextureComponent? toggleInfo;
        public EventRepeaterWindow(IDataHelper helper, string modBaseDirectory) : base((int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).X, (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).Y, 980, 470, true)
        {
            int menuX = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).X;
            int menuY = (int)Utility.getTopLeftPositionForCenteringOnScreen(980, 470).Y;

        }
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            this.xPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720, 0, 0).X;
            this.yPositionOnScreen = (int)Utility.getTopLeftPositionForCenteringOnScreen(1280, 720, 0, 0).Y;
        }
    }
}
