/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using StackSplitRedux.MenuHandlers;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;

namespace StackSplitRedux
    {
    public class StackSplit {
        private static readonly int TICKS_DELAY_OPEN = StaticConfig.SplitMenuOpenDelayTicks;

        /// <summary>Are we subscribed to the events listened to while a handler is active.</summary>
        private bool IsSubscribed = false;

        /// <summary>The handler for the current menu.</summary>
        private IMenuHandler CurrentMenuHandler;

        /// <summary>Used to avoid resize events sent to menu changed.</summary>
        private bool WasResizeEvent = false;

        /// <summary>An index incremented on every tick and reset every 60th tick (0–59).</summary>
        private int CurrentUpdateTick = 0;

        /// <summary>Tracks what tick a resize event occurs on so we can resize the current handler next frame. -1 means no resize event.</summary>
        private int TickResizedOn = -1;

        private IClickableMenu MenuToHandle;
        private int WaitOpenTicks = 0;

        private ModConfigMenu ConfigMenu;

        public StackSplit() {
            PrepareMapping();
            RegisterEvents();
            }

        ~StackSplit() {
            Log.Error($"[{nameof(StackSplit)}] We got finalized! How come??");
            }

        public void PrepareModConfigMenu() {
            this.ConfigMenu = new ModConfigMenu();
            }

        public void PrepareMapping() {
            HandlerMapping.Add(typeof(GameMenu), typeof(GameMenuHandler));
            HandlerMapping.Add(typeof(ShopMenu), typeof(ShopMenuHandler));
            HandlerMapping.Add(typeof(ItemGrabMenu), typeof(ItemGrabMenuHandler));
            HandlerMapping.Add(typeof(CraftingPage), typeof(CraftingMenuHandler));
            HandlerMapping.Add(typeof(JunimoNoteMenu), typeof(JunimoNoteMenuHandler));
            }

        public void RegisterEvents() {
            Mod.Events.Display.MenuChanged += OnMenuChanged;
            Mod.Events.Display.WindowResized += OnWindowResized;
            Mod.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            Mod.Events.GameLoop.GameLaunched += OnGameLaunched;
            }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            // menu closed
            if (e.NewMenu == null) {
                // close the current handler and unsubscribe from the events
                if (this.CurrentMenuHandler != null) {
                    //Log.Trace("[OnMenuClosed] Closing current menu handler");
                    this.CurrentMenuHandler.Close();
                    this.CurrentMenuHandler = null;

                    UnsubscribeHandlerEvents();
                    }
                return;
                }

            // ignore resize event
            if (e.OldMenu?.GetType() == e.NewMenu?.GetType() && this.WasResizeEvent) {
                this.WasResizeEvent = false;
                return;
                }
            this.WasResizeEvent = false; // Reset

            // switch the currently handler to the one for the new menu type
            var nuMenu = e.NewMenu;
            Log.TraceIfD($"Menu changed from {e.OldMenu} to {nuMenu}");
            if (
                HandlerMapping.TryGetHandler(nuMenu.GetType(), out IMenuHandler handler)
                || HandlerMapping.TryGetHandler(nuMenu.ToString(), out handler)
                ) {
                Log.TraceIfD($"{nuMenu} intercepted");
                // Close the current one if still open, it's likely invalid
                if (this.CurrentMenuHandler != null) {
                    DequeueMenuHandlerOpener();
                    this.CurrentMenuHandler.Close();
                    }
                EnqueueMenuHandlerOpener(nuMenu, handler);
                }
            else {
                Log.TraceIfD($"{nuMenu} not intercepted, don't know how");
                }
            }

        /// <summary>Raised after the game window is resized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWindowResized(object sender, WindowResizedEventArgs e) {
            // set flags to notify handler to resize next tick as the menu isn't always recreated
            this.WasResizeEvent = true;
            this.TickResizedOn = this.CurrentUpdateTick;
            }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e) {
            if (++this.CurrentUpdateTick >= 60)
                this.CurrentUpdateTick = 0;

            // If TickResizedOn isn't -1 then there was a resize event, so do the resize next tick.
            // We need to do it this way rather than where we ignore resize in menu changed since not all menus are recreated on resize,
            // and during the actual resize event the new menu will not have been created yet so we need to wait.
            if (this.TickResizedOn > -1 && this.TickResizedOn != this.CurrentUpdateTick) {
                this.TickResizedOn = -1;
                this.CurrentMenuHandler?.Close();
                // Checking the menu type since actions like returning to title will cause a resize event (idk why the window is maximized)
                // and the activeClickableMenu will not be what it was before.
                if (this.CurrentMenuHandler?.IsCorrectMenuType(Game1.activeClickableMenu) == true) {
                    this.CurrentMenuHandler?.Open(Game1.activeClickableMenu);
                    }
                else {
                    this.CurrentMenuHandler = null;
                    }
                }

            this.CurrentMenuHandler?.Update();
            }

        /// <summary>
        /// Prepare to open MenuHandler by attaching to UpateTicked event. This is done to allow other mods to finish manipulating
        /// the inventory + extended inventory
        /// </summary>
        /// <param name="newMenu">The new menu being opened</param>
        /// <param name="handler">The handler for the new menu</param>
        private void EnqueueMenuHandlerOpener(IClickableMenu newMenu, IMenuHandler handler) {
            if (this.WaitOpenTicks > 0) return;
            Log.TraceIfD($"MenuHandlerOpener enregistered & enqueued");
            this.MenuToHandle = newMenu;
            this.CurrentMenuHandler = handler;
            Mod.Events.GameLoop.UpdateTicked += MenuHandlerOpener;
            }

        /// <summary>
        /// <para>Detach MenuHandlerOpener from UpdateTicked and reset the timer.</para>
        /// <para>Called to cancel a not-yet triggered opening of MenuHandler</para>
        /// </summary>
        private void DequeueMenuHandlerOpener() {
            Mod.Events.GameLoop.UpdateTicked -= MenuHandlerOpener;
            this.WaitOpenTicks = 0;
            }

        /// <summary>
        /// Opens a MenuHandler after several ticks have passed. This is to allow other mods to finish manipulating the
        /// inventory + extended inventory
        /// </summary>
        /// <param name="sender">Event's sender -- not used</param>
        /// <param name="e">Event's args -- not used</param>
        private void MenuHandlerOpener(object sender, UpdateTickedEventArgs e) {
            if (this.WaitOpenTicks++ >= TICKS_DELAY_OPEN) {
                DequeueMenuHandlerOpener();
                // Guards
                if (this.CurrentMenuHandler is null) return;
                if (this.MenuToHandle is null) return;
                // Final check; it's possible game has invoked emergencyShutdown and we're left with a dangling ref
                if (Game1.activeClickableMenu is null) {
                    Log.Trace($"Menu {this.MenuToHandle} ran away while we're prepping!");
                    return;
                    }
                this.CurrentMenuHandler.Open(this.MenuToHandle);
                SubscribeHandlerEvents();
                }
            }

        private void OnGameLaunched(object semder, GameLaunchedEventArgs e) {
            PrepareModConfigMenu();
            InterceptOtherMods();
            }

        /// <summary>Subscribes to the events we care about when a handler is active.</summary>
        private void SubscribeHandlerEvents() {
            if (this.IsSubscribed) return;
            Mod.Events.Input.ButtonPressed += OnButtonPressed;
            Mod.Events.Display.Rendered += OnRendered;
            this.IsSubscribed = true;
            }

        /// <summary>Unsubscribes from events when the handler is no longer active.</summary>
        private void UnsubscribeHandlerEvents() {
            if (!this.IsSubscribed) return;
            Mod.Events.Input.ButtonPressed -= OnButtonPressed;
            Mod.Events.Display.Rendered -= OnRendered;
            this.IsSubscribed = false;
            }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e) {
            // Forward input to the handler and consumes it while the tooltip is active.
            // Intercept keyboard input while the tooltip is active so numbers don't change the actively equipped item etc.
            // TODO: remove null checks if these events are only called subscribed when it's valid
            switch (this.CurrentMenuHandler?.HandleInput(e.Button)) {
                case EInputHandled.Handled:
                    // Obey unless we're hitting 'cancel' keys.
                    if (e.Button != SButton.Escape)
                        Mod.Input.Suppress(e.Button);
                    else
                        this.CurrentMenuHandler.CloseSplitMenu();
                    break;

                case EInputHandled.Consumed:
                    Mod.Input.Suppress(e.Button);
                    break;

                case EInputHandled.NotHandled:
                    if (e.Button == SButton.MouseLeft || e.Button == SButton.MouseRight)
                        this.CurrentMenuHandler.CloseSplitMenu(); // click wasn't handled meaning the split menu no longer has focus and should be closed.
                    break;
                }
            }

        /// <summary><para>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch
        /// is rendered to the screen.</para>
        /// <para>Since the game may open/close the sprite batch multiple times in a draw tick, 
        /// the sprite batch may not contain everything being drawn and some things may already be rendered to the screen. 
        /// Content drawn to the sprite batch at this point will be drawn over all vanilla content (including menus, HUD, 
        /// and cursor).</para></summary>
        private void OnRendered(object sender, RenderedEventArgs e) {
            // tell the current handler to draw the split menu if it's active
            this.CurrentMenuHandler?.Draw(Game1.spriteBatch);
            }

        private void InterceptOtherMods() {
            foreach (var kvp in OtherMods.AsEnumerable()) {
                string modID = kvp.Key;
                if (!Mod.Registry.IsLoaded(modID)) continue;
                Log.Debug($"{modID} detected, registering its menus:");
                foreach (var t in kvp.Value) {
                    HandlerMapping.Add(t.Item1, t.Item2);
                    Log.Debug($"  Registered {t.Item1} to be handled by {t.Item2.Name}");
                    }
                }
            }
        }
    }
