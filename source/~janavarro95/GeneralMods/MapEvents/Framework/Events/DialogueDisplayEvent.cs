using EventSystem.Framework.FunctionEvents;
using EventSystem.Framework.Information;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Framework.Events
{
    public class DialogueDisplayEvent :MapEvent
    {
        string dialogue;
        public DialogueDisplayEvent(string Name, GameLocation Location, Vector2 Position, MouseButtonEvents MouseEvents,MouseEntryLeaveEvent EntryLeave, string Dialogue) : base(Name, Location, Position)
        {
            this.name = Name;
            this.location = Location;
            this.tilePosition = Position;
            this.mouseButtonEvents = MouseEvents;

            this.doesInteractionNeedToRun = true;
            this.dialogue = Dialogue;

            this.mouseEntryLeaveEvents = EntryLeave;
        }


        public override bool OnLeftClick()
        {
            if (base.OnLeftClick() == false) return false;
            if (this.location.isObjectAt((int)this.tilePosition.X*Game1.tileSize, (int)this.tilePosition.Y*Game1.tileSize)) return false;
            Game1.activeClickableMenu = new StardewValley.Menus.DialogueBox(this.dialogue);
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
