using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using StackSplitX.MenuHandlers;

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
            MenuEvents.MenuChanged += OnMenuChanged;
            MenuEvents.MenuClosed += OnMenuClosed;
            GraphicsEvents.Resize += OnResize;
            GameEvents.UpdateTick += OnUpdate;

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
                ControlEvents.MouseChanged += OnMouseStateChanged;
                ControlEvents.KeyPressed += OnKeyPressed;
                GraphicsEvents.OnPostRenderEvent += OnDraw;

                this.IsSubscribed = true;
            }
        }

        /// <summary>Unsubscribes from events when the handler is no longer active.</summary>
        private void UnsubscribeEvents()
        {
            if (this.IsSubscribed)
            {
                ControlEvents.MouseChanged -= OnMouseStateChanged;
                ControlEvents.KeyPressed -= OnKeyPressed;
                GraphicsEvents.OnPostRenderEvent -= OnDraw;

                this.IsSubscribed = false;
            }
        }

        /// <summary>Callback to the resize event. Sets flags to notify handler to resize next tick as the menu isn't always recreated.</summary>
        private void OnResize(object sender, EventArgs e)
        {
            this.WasResizeEvent = true;
            this.TickResizedOn = this.CurrentUpdateTick;
        }

        /// <summary>Callback for the menu closed event; closes the current handler and unsubscribes from the events.</summary>
        private void OnMenuClosed(object sender, EventArgsClickableMenuClosed e)
        {
            if (this.CurrentMenuHandler != null)
            {
                //this.Monitor.Log("[OnMenuClosed] Closing current menu handler", LogLevel.Trace);
                this.CurrentMenuHandler.Close();
                this.CurrentMenuHandler = null;

                UnsubscribeEvents();
            }
        }

        /// <summary>Callback for the menu changed event; switches the currently handler to the one for the new menu type.</summary>
        private void OnMenuChanged(object sender, EventArgsClickableMenuChanged e)
        {
            this.Monitor.DebugLog($"Menu changed from {e?.PriorMenu} to {e?.NewMenu}");

            // Resize event; ignore
            if (e.PriorMenu?.GetType() == e.NewMenu?.GetType() && this.WasResizeEvent)
            {
                this.WasResizeEvent = false;
                return;
            }
            this.WasResizeEvent = false; // Reset

            var newMenuType = e.NewMenu.GetType();
            if (this.MenuHandlers.ContainsKey(newMenuType))
            {
                // Close the current one of it's valid and not the same as the current one
                if (this.CurrentMenuHandler != null && this.CurrentMenuHandler != this.MenuHandlers[newMenuType])
                {
                    this.CurrentMenuHandler.Close();
                }

                this.CurrentMenuHandler = this.MenuHandlers[newMenuType];
                this.CurrentMenuHandler.Open(e.NewMenu);

                SubscribeEvents();
            }
        }

        /// <summary>Callback for the mouse changed event; forwards the input to the current handler and consumes it if the handler requests so.</summary>
        private void OnMouseStateChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (this.CurrentMenuHandler != null && this.CurrentMenuHandler.IsOpen())
            {
                switch (this.CurrentMenuHandler.HandleMouseInput(e.PriorState, e.NewState))
                {
                    case EInputHandled.Handled:
                        break;
                    case EInputHandled.Consumed:
                        Game1.oldMouseState = e.NewState;
                        break;
                    case EInputHandled.NotHandled:
                        // The click wasn't handled meaning the split menu no longer has focus and should be closed.
                        this.CurrentMenuHandler.CloseSplitMenu();
                        break;
                }
            }
        }

        /// <summary>Callback for the keypressed event. Forwards it to the handler and consumes it while the tooltip is active.</summary>
        private void OnKeyPressed(object sender, EventArgsKeyPressed e)
        {
            // Intercept keyboard input while the tooltip is active so numbers don't change the actively equipped item etc.
            // TODO: remove null checks if these events are only called subscribed when it's valid
            if (this.CurrentMenuHandler?.HandleKeyboardInput(e.KeyPressed) == EInputHandled.Handled)
            {
                // Obey unless we're hitting 'cancel' keys.
                if (e.KeyPressed != Keys.Escape)
                {
                    Game1.oldKBState = Keyboard.GetState();
                }
                else
                {
                    this.CurrentMenuHandler.CloseSplitMenu();
                }
            }
        }

        /// <summary>Callback for the UpdateTick event. Updates the current handler.</summary>
        private void OnUpdate(object sender, EventArgs e)
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

        /// <summary>Callback for the Draw event. Tells the current handler to draw the split menu if it's active.</summary>
        private void OnDraw(object sender, EventArgs e)
        {
            this.CurrentMenuHandler?.Draw(Game1.spriteBatch);
        }
    }
}
