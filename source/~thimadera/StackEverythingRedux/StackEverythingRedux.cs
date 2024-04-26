/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/thimadera/StardewMods
**
*************************************************/

using HarmonyLib;
using StackEverythingRedux.MenuHandlers;
using StackEverythingRedux.MenuHandlers.GameMenuHandlers;
using StackEverythingRedux.MenuHandlers.ShopMenuHandlers;
using StackEverythingRedux.Network;
using StackEverythingRedux.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.Tools;
using System.Reflection;
using SObject = StardewValley.Object;

namespace StackEverythingRedux
{
    internal class StackEverythingRedux : Mod
    {
        #region Internal Properties
        internal static Mod Instance;
        internal static IManifest Manifest => Instance.ModManifest;
        internal static Harmony Harmony => new(Manifest.UniqueID);
        internal static IModHelper ModHelper => Instance.Helper;
        internal static ModConfig Config => ModHelper.ReadConfig<ModConfig>();
        internal static ITranslationHelper I18n => ModHelper.Translation;
        internal static IReflectionHelper Reflection => ModHelper.Reflection;
        internal static IInputHelper Input => ModHelper.Input;
        internal static IModEvents Events => ModHelper.Events;
        internal static IModRegistry Registry => ModHelper.ModRegistry;
        #endregion

        private static readonly int TICKS_DELAY_OPEN = StaticConfig.SplitMenuOpenDelayTicks;
        private bool IsSubscribed = false;
        private IMenuHandler CurrentMenuHandler;
        private bool WasResizeEvent = false;
        private int CurrentUpdateTick = 0;
        private int TickResizedOn = -1;
        private IClickableMenu MenuToHandle;
        private int WaitOpenTicks = 0;

        public override void Entry(IModHelper helper)
        {
            Instance = this;

            PatchHarmony();
            PrepareMapping();
            RegisterEvents();
        }

        private static void PatchHarmony()
        {
            Patch(nameof(SObject.maximumStackSize), typeof(Furniture), typeof(MaximumStackSizePatches));
            Patch(nameof(SObject.maximumStackSize), typeof(Wallpaper), typeof(MaximumStackSizePatches));
            Patch(nameof(SObject.maximumStackSize), typeof(SObject), typeof(MaximumStackSizePatches));

            Patch(nameof(Utility.tryToPlaceItem), typeof(Utility), typeof(TryToPlaceItemPatches));
            Patch(nameof(Item.canStackWith), typeof(Item), typeof(CanStackWithPatches));
            Patch(nameof(Tool.attach), typeof(Tool), typeof(AttachPatches));

            Patch("removeQueuedFurniture", typeof(GameLocation), typeof(RemoveQueuedFurniturePatches));
            Patch("doDoneFishing", typeof(FishingRod), typeof(DoDoneFishingPatches));
        }

        public static void PrepareMapping()
        {
            HandlerMapping.Add(typeof(GameMenu), typeof(GameMenuHandler));
            HandlerMapping.Add(typeof(ShopMenu), typeof(ShopMenuHandler));
            HandlerMapping.Add(typeof(ItemGrabMenu), typeof(ItemGrabMenuHandler));
            HandlerMapping.Add(typeof(CraftingPage), typeof(CraftingMenuHandler));
            HandlerMapping.Add(typeof(JunimoNoteMenu), typeof(JunimoNoteMenuHandler));
        }

        public void RegisterEvents()
        {
            if (Events is not null)
            {
                Events.Display.MenuChanged += OnMenuChanged;
                Events.Display.WindowResized += OnWindowResized;
                Events.GameLoop.UpdateTicked += OnUpdateTicked;
                Events.GameLoop.GameLaunched += OnGameLaunched;
            }
        }

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Config?.EnableStackSplitRedux is not true)
            {
                return;
            }

            // menu closed
            if (e.NewMenu == null)
            {
                // close the current handler and unsubscribe from the events
                if (CurrentMenuHandler != null)
                {
                    //Log.Trace("[OnMenuClosed] Closing current menu handler");
                    CurrentMenuHandler.Close();
                    CurrentMenuHandler = null;

                    UnsubscribeHandlerEvents();
                }
                return;
            }

            // ignore resize event
            if (e.OldMenu?.GetType() == e.NewMenu?.GetType() && WasResizeEvent)
            {
                WasResizeEvent = false;
                return;
            }
            WasResizeEvent = false; // Reset

            // switch the currently handler to the one for the new menu type
            Log.TraceIfD($"Menu changed from {e.OldMenu} to {e.NewMenu}");
            if (
                HandlerMapping.TryGetHandler(e.NewMenu.GetType(), out IMenuHandler handler)
                || HandlerMapping.TryGetHandler(e.NewMenu.ToString(), out handler)
                )
            {
                Log.TraceIfD($"{e.NewMenu} intercepted");
                // Close the current one if still open, it's likely invalid
                if (CurrentMenuHandler != null)
                {
                    DequeueMenuHandlerOpener();
                    CurrentMenuHandler.Close();
                }
                EnqueueMenuHandlerOpener(e.NewMenu, handler);
            }
            else
            {
                Log.TraceIfD($"{e.NewMenu} not intercepted");
            }
        }

        /// <summary>Raised after the game window is resized.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnWindowResized(object sender, WindowResizedEventArgs e)
        {
            if (Config?.EnableStackSplitRedux is not true)
            {
                return;
            }

            WasResizeEvent = true;
            TickResizedOn = CurrentUpdateTick;
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            if (Config?.EnableStackSplitRedux is not true)
            {
                return;
            }

            if (++CurrentUpdateTick >= 60)
            {
                CurrentUpdateTick = 0;
            }

            // If TickResizedOn isn't -1 then there was a resize event, so do the resize next tick.
            // We need to do it this way rather than where we ignore resize in menu changed since not all menus are recreated on resize,
            // and during the actual resize event the new menu will not have been created yet so we need to wait.
            if (TickResizedOn > -1 && TickResizedOn != CurrentUpdateTick)
            {
                TickResizedOn = -1;
                CurrentMenuHandler?.Close();
                // Checking the menu type since actions like returning to title will cause a resize event (idk why the window is maximized)
                // and the activeClickableMenu will not be what it was before.
                if (CurrentMenuHandler?.IsCorrectMenuType(Game1.activeClickableMenu) == true)
                {
                    CurrentMenuHandler?.Open(Game1.activeClickableMenu);
                }
                else
                {
                    CurrentMenuHandler = null;
                }
            }

            CurrentMenuHandler?.Update();
        }

        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            GenericModConfigMenuIntegration.AddConfig();
            InterceptOtherMods();
        }

        /// <summary>
        /// Prepare to open MenuHandler by attaching to UpateTicked event. This is done to allow other mods to finish manipulating
        /// the inventory + extended inventory
        /// </summary>
        /// <param name="newMenu">The new menu being opened</param>
        /// <param name="handler">The handler for the new menu</param>
        private void EnqueueMenuHandlerOpener(IClickableMenu newMenu, IMenuHandler handler)
        {
            if (WaitOpenTicks > 0)
            {
                return;
            }

            Log.TraceIfD($"MenuHandlerOpener enregistered & enqueued");
            MenuToHandle = newMenu;
            CurrentMenuHandler = handler;
            Events.GameLoop.UpdateTicked += MenuHandlerOpener;
        }

        /// <summary>
        /// <para>Detach MenuHandlerOpener from UpdateTicked and reset the timer.</para>
        /// <para>Called to cancel a not-yet triggered opening of MenuHandler</para>
        /// </summary>
        private void DequeueMenuHandlerOpener()
        {
            Events.GameLoop.UpdateTicked -= MenuHandlerOpener;
            WaitOpenTicks = 0;
        }

        /// <summary>
        /// Opens a MenuHandler after several ticks have passed. This is to allow other mods to finish manipulating the
        /// inventory + extended inventory
        /// </summary>
        /// <param name="sender">Event's sender -- not used</param>
        /// <param name="e">Event's args -- not used</param>
        private void MenuHandlerOpener(object sender, UpdateTickedEventArgs e)
        {
            if (WaitOpenTicks++ >= TICKS_DELAY_OPEN)
            {
                DequeueMenuHandlerOpener();
                // Guards
                if (CurrentMenuHandler is null)
                {
                    return;
                }

                if (MenuToHandle is null)
                {
                    return;
                }
                // Final check; it's possible game has invoked emergencyShutdown and we're left with a dangling ref
                if (Game1.activeClickableMenu is null)
                {
                    Log.Trace($"Menu {MenuToHandle} ran away while we're prepping!");
                    return;
                }
                CurrentMenuHandler.Open(MenuToHandle);
                SubscribeHandlerEvents();
            }
        }

        /// <summary>Subscribes to the events we care about when a handler is active.</summary>
        private void SubscribeHandlerEvents()
        {
            if (IsSubscribed)
            {
                return;
            }

            Events.Input.ButtonPressed += OnButtonPressed;
            Events.Display.Rendered += OnRendered;
            Events.Input.CursorMoved += OnCursorMoved;
            IsSubscribed = true;
        }

        /// <summary>Unsubscribes from events when the handler is no longer active.</summary>
        private void UnsubscribeHandlerEvents()
        {
            if (!IsSubscribed)
            {
                return;
            }

            Events.Input.ButtonPressed -= OnButtonPressed;
            Events.Display.Rendered -= OnRendered;
            Events.Input.CursorMoved -= OnCursorMoved;
            IsSubscribed = false;
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (Config?.EnableStackSplitRedux is not true)
            {
                return;
            }
            // Forward input to the handler and consumes it while the tooltip is active.
            // Intercept keyboard input while the tooltip is active so numbers don't change the actively equipped item etc.
            // TODO: remove null checks if these events are only called subscribed when it's valid
            switch (CurrentMenuHandler?.HandleInput(e.Button))
            {
                case EInputHandled.Handled:
                    // Obey unless we're hitting 'cancel' keys.
                    if (e.Button != SButton.Escape)
                    {
                        Input.Suppress(e.Button);
                    }
                    else
                    {
                        CurrentMenuHandler.CloseSplitMenu();
                    }

                    break;

                case EInputHandled.Consumed:
                    Input.Suppress(e.Button);
                    break;

                case EInputHandled.NotHandled:
                    if (e.Button is SButton.MouseLeft or SButton.MouseRight)
                    {
                        CurrentMenuHandler.CloseSplitMenu(); // click wasn't handled meaning the split menu no longer has focus and should be closed.
                    }

                    break;
            }
        }

        /// <summary><para>Raised after the game draws to the sprite patch in a draw tick, just before the final sprite batch
        /// is rendered to the screen.</para>
        /// <para>Since the game may open/close the sprite batch multiple times in a draw tick, 
        /// the sprite batch may not contain everything being drawn and some things may already be rendered to the screen. 
        /// Content drawn to the sprite batch at this point will be drawn over all vanilla content (including menus, HUD, 
        /// and cursor).</para></summary>
        private void OnRendered(object sender, RenderedEventArgs e)
        {
            if (Config?.EnableStackSplitRedux is not true)
            {
                return;
            }
            // tell the current handler to draw the split menu if it's active
            CurrentMenuHandler?.Draw(Game1.spriteBatch);
        }

        private void OnCursorMoved(object sender, CursorMovedEventArgs e)
        {
            if (Config?.EnableStackSplitRedux is not true)
            {
                return;
            }
            CurrentMenuHandler?.PerformHoverAction(Game1.getMouseX(true), Game1.getMouseY(true));
        }

        private static void Patch(string originalName, Type originalType, Type patchType)
        {
            BindingFlags originalSearch = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;
            MethodInfo original = originalType.GetMethods(originalSearch).FirstOrDefault(m => m.Name == originalName);

            if (original == null)
            {
                Log.Error($"Failed to patch {originalType.Name}::{originalName}: could not find original method.");
                return;
            }

            MethodInfo[] patchMethods = patchType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            MethodInfo prefix = patchMethods.FirstOrDefault(m => m.Name == "Prefix");
            MethodInfo postfix = patchMethods.FirstOrDefault(m => m.Name == "Postfix");

            if (prefix != null || postfix != null)
            {
                try
                {
                    _ = Harmony.Patch(original, prefix == null ? null : new HarmonyMethod(prefix), postfix == null ? null : new HarmonyMethod(postfix));
                    Log.Trace($"Patched {originalType}::{originalName} with{(prefix == null ? "" : $" {patchType.Name}::{prefix.Name}")}{(postfix == null ? "" : $" {patchType.Name}::{postfix.Name}")}");
                }
                catch (Exception e)
                {
                    Log.Error($"Failed to patch {originalType.Name}::{originalName}: {e.Message}");
                }
            }
            else
            {
                Log.Error($"Failed to patch {originalType.Name}::{originalName}: both prefix and postfix are null.");
            }
        }

        private static void InterceptOtherMods()
        {
            foreach (KeyValuePair<string, List<Tuple<string, Type>>> kvp in OtherMods.AsEnumerable())
            {
                string modID = kvp.Key;
                if (!Registry.IsLoaded(modID))
                {
                    continue;
                }

                Log.Debug($"{modID} detected, registering its menus:");
                foreach (Tuple<string, Type> t in kvp.Value)
                {
                    HandlerMapping.Add(t.Item1, t.Item2);
                    Log.Debug($"Registered {t.Item1} to be handled by {t.Item2.Name}");
                }
            }
        }

    }
}
