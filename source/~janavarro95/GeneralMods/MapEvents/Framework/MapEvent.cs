using EventSystem.Framework.FunctionEvents;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;

namespace EventSystem.Framework
{

    /// <summary>Base class used to handle map tile events.</summary>
    public class MapEvent
    {
        /// <summary>//MAKE NAME FOR EVENTS</summary>
        public string name;

        public Vector2 tilePosition;
        public GameLocation location;

        public PlayerEvents playerEvents;
        public bool playerOnTile;

        public MouseButtonEvents mouseButtonEvents;
        public MouseEntryLeaveEvent mouseEntryLeaveEvents;
        public bool mouseOnTile;

        public bool doesInteractionNeedToRun;
        public bool loopInteraction;

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Constructors 

        *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        */
        #region
        /// <summary>Empty Constructor.</summary>
        public MapEvent() { }

        /// <summary>A simple map event that doesn't do anything.</summary>
        public MapEvent(string name, GameLocation location, Vector2 position)
        {
            this.name = name;
            this.location = location;
            this.tilePosition = position;
        }

        /// <summary>A simple map function that runs when the player enters and leaves a tile. Set values to null for nothing to happen.</summary>
        /// <param name="location">The game location where the event is. I.E Farm, Town, Mine etc.</param>
        /// <param name="position">The x,y position on the map the event is.</param>
        /// <param name="playerEvents">Handles various events that runs when the player enters/leaves a tile, etc.</param>
        public MapEvent(string name, GameLocation location, Vector2 position, PlayerEvents playerEvents)
        {
            this.name = name;
            this.location = location;
            this.tilePosition = position;
            this.playerEvents = playerEvents;
        }

        /// <summary>A constructor that handles when the mouse leaves and enters a tile.</summary>
        /// <param name="location">The game location where the event is.</param>
        /// <param name="position">The x,y position of the tile at the game location.</param>
        /// <param name="mouseEvents">A class used to handle mouse entry/leave events.</param>
        public MapEvent(string name, GameLocation location, Vector2 position, MouseEntryLeaveEvent mouseEvents)
        {
            this.name = name;
            this.location = location;
            this.tilePosition = position;
            this.mouseEntryLeaveEvents = mouseEvents;
        }

        /// <summary>A constructor that handles when the mouse leaves and enters a tile.</summary>
        /// <param name="location">The game location where the event is.</param>
        /// <param name="position">The x,y position of the tile at the game location.</param>
        /// <param name="mouseEvents">A class used to handle mouse click/scroll events.</param>
        public MapEvent(string name, GameLocation location, Vector2 position, MouseButtonEvents mouseEvents)
        {
            this.name = name;
            this.location = location;
            this.tilePosition = position;
            this.mouseButtonEvents = mouseEvents;
        }

        /// <summary>A constructor encapsulating player, mouse button, and mouse entry events.</summary>
        /// <param name="location">The game location for which the event is located. I.E Town, Farm, etc.</param>
        /// <param name="position">The x,y cordinates for this event to be located at.</param>
        /// <param name="playerEvents">The events that occur associated with the player. I.E player entry, etc.</param>
        /// <param name="mouseButtonEvents">The events associated with clicking a mouse button while on this tile.</param>
        /// <param name="mouseEntryLeaveEvents">The events that occur when the mouse enters or leaves the same tile position as this event.</param>
        public MapEvent(string name, GameLocation location, Vector2 position, PlayerEvents playerEvents, MouseButtonEvents mouseButtonEvents, MouseEntryLeaveEvent mouseEntryLeaveEvents)
        {
            this.name = name;
            this.location = location;
            this.tilePosition = position;
            this.playerEvents = playerEvents;
            this.mouseButtonEvents = mouseButtonEvents;
            this.mouseEntryLeaveEvents = mouseEntryLeaveEvents;
        }
        #endregion


        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Player related functions 

        *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        */
        #region
        /// <summary>Occurs when the player enters the same tile as this event. The function associated with this event is then ran.</summary>
        public virtual bool OnPlayerEnter()
        {
            if (this.playerEvents == null) return false;
            if (this.isPlayerOnTile() && this.doesInteractionNeedToRun)
            {
                this.playerOnTile = true;
                this.doesInteractionNeedToRun = false;
                this.playerEvents.onPlayerEnter?.run();
                return true;
            }
            return false;
        }

        /// <summary>Occurs when the player leaves the same tile that this event is on. The function associated with thie event is then ran.</summary>
        public virtual bool OnPlayerLeave()
        {
            if (this.playerEvents == null) return false;
            if (!this.isPlayerOnTile() && this.playerOnTile)
            {
                this.playerOnTile = false;
                this.doesInteractionNeedToRun = true;
                this.playerEvents.onPlayerLeave?.run();
                return true;
            }
            return false;
        }

        /// <summary>Checks if the player is on the same tile as this event.</summary>
        public virtual bool isPlayerOnTile()
        {
            return
                Game1.player.getTileX() == this.tilePosition.X
                && Game1.player.getTileY() == this.tilePosition.Y;
        }
        #endregion

        /*~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
          Mouse related functions 

        *~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        */
        #region
        /// <summary>Occurs when the player left clicks the same tile that this event is on.</summary>
        public virtual bool OnLeftClick()
        {
            if (!this.mouseOnTile) return false;
            if (this.mouseButtonEvents == null) return false;
            this.mouseButtonEvents.onLeftClick?.run();
            return true;

        }

        /// <summary>Occurs when the player right clicks the same tile that this event is on.</summary>
        public virtual bool OnRightClick()
        {
            if (!this.mouseOnTile) return false;
            if (this.mouseButtonEvents == null) return false;
            this.mouseButtonEvents.onRightClick?.run();
            return true;
        }

        /// <summary>Occurs when the mouse tile position is the same as this event's x,y position.</summary>
        public virtual bool OnMouseEnter()
        {
            if (this.mouseEntryLeaveEvents == null) return false;
            if (this.isMouseOnTile())
            {
                this.mouseOnTile = true;
                this.mouseEntryLeaveEvents.onMouseEnter?.run();
                return true;
            }
            return false;
        }

        /// <summary>Occurs when the mouse tile position leaves the the same x,y position as this event.</summary>
        public virtual bool OnMouseLeave()
        {
            if (this.mouseEntryLeaveEvents == null) return false;
            if (!this.isMouseOnTile() && this.mouseOnTile)
            {
                this.mouseOnTile = false;
                this.mouseEntryLeaveEvents.onMouseLeave?.run();
                return true;
            }
            return false;
        }

        /// <summary>UNUSED!!!! Occurs when the mouse is on the same position as the tile AND the user scrolls the mouse wheel.</summary>
        public virtual bool OnMouseScroll()
        {
            if (!this.isMouseOnTile()) return false;
            this.mouseButtonEvents.onMouseScroll?.run();
            return true;
        }

        /// <summary>Checks if the mouse is on the tile.</summary>
        public virtual bool isMouseOnTile()
        {
            Vector2 mousePosition = Game1.currentCursorTile;
            if (mousePosition.X == this.tilePosition.X && mousePosition.Y == this.tilePosition.Y) return true;
            return false;
        }

        /// <summary>Occurs when the tile is clicked. Runs the appropriate event.</summary>
        public virtual void clickEvent()
        {
            if (!this.mouseOnTile) return;
            var mouseState = Mouse.GetState();
            if (mouseState.LeftButton == ButtonState.Pressed)
                this.OnLeftClick();
            if (mouseState.RightButton == ButtonState.Pressed)
                this.OnRightClick();
        }
        #endregion

        /// <summary>Used to check if any sort of events need to run on this tile right now.</summary>
        public virtual void update()
        {
            if (Game1.activeClickableMenu != null) return;
            this.clickEvent(); //click events
            this.OnPlayerEnter(); //player enter events
            this.OnPlayerLeave(); //player leave events
            this.OnMouseEnter(); //on mouse enter events
            this.OnMouseLeave(); //on mouse leave events.
        }
    }
}
