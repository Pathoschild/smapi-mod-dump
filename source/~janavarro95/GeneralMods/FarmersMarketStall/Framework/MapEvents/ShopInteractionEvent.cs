using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EventSystem;
using EventSystem.Framework.FunctionEvents;
using Microsoft.Xna.Framework;
using StardewValley;

namespace FarmersMarketStall.Framework.MapEvents
{
    public class ShopInteractionEvent :EventSystem.Framework.MapEvent
    {
        public ShopInteractionEvent(string Name, GameLocation Location, Vector2 Position, MouseButtonEvents MouseEvents, MouseEntryLeaveEvent EntryLeave) : base(Name, Location, Position)
        {
            this.name = Name;
            this.location = Location;
            this.tilePosition = Position;
            this.mouseButtonEvents = MouseEvents;

            this.doesInteractionNeedToRun = true;

            this.mouseEntryLeaveEvents = EntryLeave;
        }


        public override bool OnLeftClick()
        {
            if (base.OnLeftClick() == false) return false;
            if (this.location.isObjectAt((int)this.tilePosition.X * Game1.tileSize, (int)this.tilePosition.Y * Game1.tileSize)) return false;
            Game1.activeClickableMenu = Menus.MarketStallMenu.openMenu(Class1.marketStall);
            return true;
        }

        /// <summary>
        /// Used to update the event and check for interaction.
        /// </summary>
        public override void update()
        {
            this.clickEvent();
            //Needed for updating.
            this.OnMouseEnter();
            this.OnMouseLeave();
        }

    }
}
