/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/TMThong/Stardew-Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.Menus;
using ThongUICore.Framework.Manager;
using ThongUICore.Framework.Renderer;

namespace ThongUICore.Framework.Element
{
    [DebuggerDisplay("{" + nameof(GetDebuggerDisplay) + "(),nq}")]
    public abstract class BaseElement : OptionsElement
    {

        public virtual int Height { get; protected set; }
        public virtual int Width { get; protected set; }

        public int WhichOptionId { get; }

        public bool IsGameOptionId { get; protected set; } = false;

        public BaseRenderer Background;

        public BaseElement() : base(string.Empty)
        {
        }

        public BaseElement(int whichOptionId, string menuid, bool isGameOptionId = false) : this()
        {
            WhichOptionId = whichOptionId;
            IsGameOptionId = isGameOptionId;
            if (!IsGameOptionId)
            {
                ThongUIManager.MenuManager.CreateWhichOptionId(menuid);
            }
        }



        public override void draw(SpriteBatch b, int slotX, int slotY, IClickableMenu context = null)
        {
            base.draw(b, slotX, slotY, context);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void leftClickHeld(int x, int y)
        {
            base.leftClickHeld(x, y);
        }

        public override void leftClickReleased(int x, int y)
        {
            base.leftClickReleased(x, y);
        }

        public override void receiveKeyPress(Keys key)
        {
            base.receiveKeyPress(key);
        }

        public override void receiveLeftClick(int x, int y)
        {
            base.receiveLeftClick(x, y);
        }

        public override string ToString()
        {
            return base.ToString();
        }

        private string GetDebuggerDisplay()
        {
            return ToString();
        }
    }
}
