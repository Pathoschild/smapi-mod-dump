using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using StardewValley.Network;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Menus
{
    class MTNCoopMenu : CoopMenu
    {

        protected abstract class MTNCoopMenuSlot : CoopMenuSlot
        {
            protected new MTNCoopMenu menu;

            public MTNCoopMenuSlot(MTNCoopMenu menu) : base(menu)
            {
                this.menu = menu;
            }
        }

        protected abstract class MTNLabeledSlot : LabeledSlot
        {
            protected new MTNCoopMenu menu;
            public MTNLabeledSlot(MTNCoopMenu menu, string message) :base(menu, message)
            {
                this.menu = menu;
            }
            public abstract override void Activate();
        }

        protected class MTNLanSlot : MTNLabeledSlot
        {
            protected new MTNCoopMenu menu;
            
            public MTNLanSlot(MTNCoopMenu menu) : base(menu, Game1.content.LoadString("Strings\\UI:CoopMenu_JoinLANGame"))
            {
                this.menu = menu;
            }

            public override void Activate()
            {
                this.menu.MTNenterIPPressed();
            }
        }

        public void MTNenterIPPressed()
        {
            string title = Game1.content.LoadString("Strings\\UI:CoopMenu_EnterIP");
            setMenu(new MTNTitleTextInputMenu(title, delegate (string address)
            {
                if (address == "")
                {
                    address = "localhost";
                }
                setMenu(new FarmhandMenu(Memory.multiplayer.InitClient(new LidgrenClient(address))));
            }));
        }
    }
}
