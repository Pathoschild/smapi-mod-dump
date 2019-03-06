using System;
using System.Collections.Generic;
using StackSplitX.MenuHandlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace StackSplitX
{
    public class StackSplitX : Mod
    {
        /// <summary>Are we subscribed to the events listened to while a handler is active.</summary>
        private bool IsSubscribed = false;

        /// <summary>Handlers mapped to the type of menu they handle.</summary>
        private Dictionary<Type, IMenuHandler> MenuHandlers;

        /// <summary>The handler for the current menu.</summary>
        private IMenuHandler CurrentMenuHandler;

        /// <summary>Used to avoid resize events sent to menu changed.</summary>
        private bool WasResizeEvent = false;

        /// <summary>An index incremented on every tick and reset every 60th tick (0–59).</summary>
        private int CurrentUpdateTick = 0;

        /// <summary>Tracks what tick a resize event occurs on so we can resize the current handler next frame. -1 means no resize event.</summary>
        private int TickResizedOn = -1;

        /// <summary>Mod entry point.</summary>
        /// <param name="helper">Mod helper.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.Display.MenuChanged += OnMenuChanged;
            helper.Events.Display.WindowResized += OnWindowResized;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;

            this.MenuHandlers = new Dictionary<Type, IMenuHandler>()
            {
                { typeof(GameMenu), new GameMenuHandler(helper, this.Monitor) },
                { typeof(ShopMenu), new ShopMenuHandler(helper, this.Monitor) },
                { typeof(ItemGrabMenu), new ItemGrabMenuHandler(helper, this.Monitor) },
                { typeof(CraftingPage), new CraftingMenuHandler(helper, this.Monitor) },
                { typeof(JunimoNoteMenu), new JunimoNoteMenuHandler(helper, this.Monitor) }
            };
        }

        /// <summary>Subscribes to the events we care about when a handler is active.</summary>
        private void SubscribeEvents()
        {
            if (!this.IsSubscribed)
            {
                Helper.Events.Input.ButtonPressed += OnButtonPressed;
                Helper.Events.Display.Rendered += OnRendered;

                this.IsSubscribed = true;
            }
        }

        /// <summary>Unsubscribes from events when the handler is no longer active.</summary>
        private void UnsubscribeEvents()
        {
            if (this.IsSubscribed)
            {
                Helper.Events.Input.ButtonPressed -= OnButtonPressed;
                Helper.Events.Display.Rendered -= OnRendered;

                this.IsSubscribed = false;
            }
        }

        /// <summary>Raised after the game window is resized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            // set flags to notify handler to resize next tick as the menu isn't always recreated
            this.WasResizeEvent = true;
            this.TickResizedOn = this.CurrentUpdateTick;
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            // menu closed
            if (e.NewMenu == null)
            {
                // close the current handler and unsubscribe from the events
                if (this.CurrentMenuHandler != null)
                {
                    //this.Monitor.Log("[OnMenuClosed] Closing current menu handler", LogLevel.Trace);
                    this.CurrentMenuHandler.Close();
                    this.CurrentMenuHandler = null;

                    UnsubscribeEvents();
                }
                return;
            }

            // ignore resize event
            if (e.OldMenu?.GetType() == e.NewMenu?.GetType() && this.WasResizeEvent)
            {
                this.WasResizeEvent = false;
                return;
            }
            this.WasResizeEvent = false; // Reset


            // switch the currently handler to the one for the new menu type
            this.Monitor.DebugLog($"Menu changed from {e.OldMenu} to {e.NewMenu}");
            var newMenuType = e.NewMenu.GetType();
            if (this.MenuHandlers.ContainsKey(newMenuType))
            {
                // Close the current one of it's valid
                if (this.CurrentMenuHandler != null)
                {
                    this.CurrentMenuHandler.Close();
                }

                this.CurrentMenuHandler = this.MenuHandlers[newMenuType];
                this.CurrentMenuHandler.Open(e.NewMenu);

                SubscribeEvents();
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // Forward input to the handler and consumes it while the tooltip is active.
            // Intercept keyboard input while the tooltip is active so numbers don't change the actively equipped item etc.
            // TODO: remove null checks if these events are only called subscribed when it's valid
            switch (this.CurrentMenuHandler?.HandleInput(e.Button))
            {
                case EInputHandled.Handled:
                    // Obey unless we're hitting 'cancel' keys.
                    if (e.Button != SButton.Escape)
                        this.Helper.Input.Suppress(e.Button);
                    else
                        this.CurrentMenuHandler.CloseSplitMenu();
                    break;

                case EInputHandled.Consumed:
                    this.Helper.Input.Suppress(e.Button);
                    break;

                case EInputHandled.NotHandled:
                    if (e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight)
                        this.CurrentMenuHandler.CloseSplitMenu(); // click wasn't handled meaning the split menu no longer has focus and should be closed.
                    break;
            }
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            this.CurrentUpdateTick += 1;
            if (this.CurrentUpdateTick >= 60)
                this.CurrentUpdateTick = 0;

            // If TickResizedOn isn't -1 then there was a resize event, so do the resize next tick.
            // We need to do it this way rather than where we ignore resize in menu changed since not all menus are recreated on resize,
            // and during the actual resize event the new menu will not have been created yet so we need to wait.
            if (this.TickResizedOn > -1 && this.TickResizedOn != this.CurrentUpdateTick)
            {
                this.TickResizedOn = -1;
                this.CurrentMenuHandler?.Close();
                // Checking the menu type since actions like returning to title will cause a resize event (idk why the window is maximized)
                // and the activeClickableMenu will not be what it was before.
                if (this.CurrentMenuHandler?.IsCorrectMenuType(Game1.activeClickableMenu) == true)
                {
                    this.CurrentMenuHandler?.Open(Game1.activeClickableMenu);
                }
                else
                {
                    this.CurrentMenuHandler = null;
                }
            }

            this.CurrentMenuHandler?.Update();
        }

        /// <summary>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch is rendered to the screen. Since the game may open/close the sprite batch multiple times in a draw tick, the sprite batch may not contain everything being drawn and some things may already be rendered to the screen. Content drawn to the sprite batch at this point will be drawn over all vanilla content (including menus, HUD, and cursor).</summary>
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            // tell the current handler to draw the split menu if it's active
            this.CurrentMenuHandler?.Draw(Game1.spriteBatch);
        }
    }
}
