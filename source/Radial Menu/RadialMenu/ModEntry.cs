/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/focustense/StardewRadialMenu
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using RadialMenu.Config;
using RadialMenu.Gmcm;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Reflection;

namespace RadialMenu;

public class ModEntry : Mod
{
    private const string GMCM_MOD_ID = "spacechase0.GenericModConfigMenu";
    private const string GMCM_OPTIONS_MOD_ID = "jltaylor-us.GMCMOptions";

    private readonly PerScreen<PlayerState> playerState;
    // Painter doesn't actually need to be per-screen in order to have correct output, but it does
    // some caching related to its current items/selection, so giving the same painter inputs from
    // different players would slow performance for both of them.
    private readonly PerScreen<Painter> painter;

    // Wrappers around PlayerState fields
    internal PlayerState PlayerState => playerState.Value;

    internal Cursor Cursor => PlayerState.Cursor;

    internal PreMenuState PreMenuState
    {
        get => PlayerState.PreMenuState;
        set => PlayerState.PreMenuState = value;
    }

    internal int MenuOffset
    {
        get => PlayerState.MenuOffset;
        set => PlayerState.MenuOffset = value;
    }

    internal IReadOnlyList<MenuItem> ActiveMenuItems
    {
        get => PlayerState.ActiveMenuItems;
        set => PlayerState.ActiveMenuItems = value;
    }

    internal Func<DelayedActions?, ItemActivationResult>? PendingActivation
    {
        get => PlayerState.PendingActivation;
        set => PlayerState.PendingActivation = value;
    }

    internal bool IsActivationDelayed
    {
        get => PlayerState.IsActivationDelayed;
        set => PlayerState.IsActivationDelayed = value;
    }

    internal double RemainingActivationDelayMs
    {
        get => PlayerState.RemainingActivationDelayMs;
        set => PlayerState.RemainingActivationDelayMs = value;
    }

    internal Painter Painter => painter.Value;

    // Global state
    private Configuration config = null!;
    private ConfigMenu? configMenu;
    private IGenericModMenuConfigApi? configMenuApi;
    private IGMCMOptionsAPI? gmcmOptionsApi;
    private GenericModConfigKeybindings? gmcmKeybindings;
    private GenericModConfigSync? gmcmSync;
    private MenuItemBuilder menuItemBuilder = null!;
    private TextureHelper textureHelper = null!;
    private KeybindActivator keybindActivator = null!;

    public ModEntry()
    {
        playerState = new(CreatePlayerState);
        painter = new(CreatePainter);
    }

    public override void Entry(IModHelper helper)
    {
        config = Helper.ReadConfig<Configuration>();
        textureHelper = new(Helper.GameContent, Monitor);
        menuItemBuilder = new(textureHelper, ActivateCustomMenuItem);
        keybindActivator = new(helper.Input);

        helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
        // For optimal latency: handle input before the Update loop, perform actions/rendering after.
        helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
        helper.Events.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
        helper.Events.Input.ButtonsChanged += Input_ButtonsChanged;
        helper.Events.Display.RenderedHud += Display_RenderedHud;
    }

    [EventPriority(EventPriority.Low)]
    private void Display_RenderedHud(object? sender, RenderedHudEventArgs e)
    {
        if (Cursor.ActiveMenu is null)
        {
            return;
        }
        var selectionBlend = GetSelectionBlend();
        Painter.Paint(
            e.SpriteBatch,
            Game1.uiViewport,
            Cursor.ActiveMenu == MenuKind.Inventory && MenuOffset == 0
                ? GetCurrentNonEmptyToolIndex()
                : -1,
            Cursor.CurrentTarget?.SelectedIndex ?? -1,
            Cursor.CurrentTarget?.Angle,
            selectionBlend);
    }

    [EventPriority(EventPriority.Low - 10)]
    private void GameLoop_GameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        configMenuApi = Helper.ModRegistry.GetApi<IGenericModMenuConfigApi>(GMCM_MOD_ID);
        gmcmOptionsApi = Helper.ModRegistry.GetApi<IGMCMOptionsAPI>(GMCM_OPTIONS_MOD_ID);
        LoadGmcmKeybindings();
        RegisterConfigMenu();
    }

    private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }
        if (Cursor.WasMenuChanged && Cursor.ActiveMenu != null)
        {
            Game1.playSound("shwip");
        }
        else if (Cursor.WasTargetChanged && Cursor.CurrentTarget != null)
        {
            Game1.playSound("smallSelect");
        }
        if (PendingActivation is not null)
        {
            Cursor.CheckSuppressionState(out _);
            // Delay filtering is slippery, because sometimes in order to know whether an item
            // _will_ be consumed (vs. switched to), we just have to attempt it, i.e. using the
            // performUseAction method.
            //
            // So what we can do is pass the delay parameter into the action itself, removing it
            // once the delay is up; the action will abort if it matches the delay setting but
            // proceed otherwise.
            if (RemainingActivationDelayMs > 0)
            {
                RemainingActivationDelayMs -= Game1.currentGameTime.ElapsedGameTime.TotalMilliseconds;
            }
            var activationResult = MaybeInvokePendingActivation();
            if (activationResult != ItemActivationResult.Ignored
                && activationResult != ItemActivationResult.Delayed)
            {
                PendingActivation = null;
                RemainingActivationDelayMs = 0;
                Cursor.Reset();
                RestorePreMenuState();
            }
        }
    }

    private void GameLoop_UpdateTicking(object? sender, UpdateTickingEventArgs e)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        Cursor.GamePadState = GetRawGamePadState();
        if (!Context.CanPlayerMove && Cursor.ActiveMenu is null)
        {
            Cursor.CheckSuppressionState(out _);
            return;
        }

        if (RemainingActivationDelayMs <= 0)
        {
            Cursor.UpdateActiveMenu();
            if (Cursor.WasMenuChanged)
            {
                if (Cursor.ActiveMenu is not null)
                {
                    PreMenuState = new(Game1.freezeControls);
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    Game1.freezeControls = true;
                    MenuOffset = 0;
                }
                else
                {
                    if (config.PrimaryActivation == ItemActivationMethod.TriggerRelease
                        && Cursor.CurrentTarget is not null)
                    {
                        Cursor.RevertActiveMenu();
                        ScheduleActivation(/* forceSelect= */ config.PrimaryAction);
                    }
                    else
                    {
                        RestorePreMenuState();
                    }
                }
                UpdateMenuItems(Cursor.ActiveMenu);
            }
            Cursor.UpdateCurrentTarget(ActiveMenuItems.Count);
        }

        // Here be dragons: because the triggers are analog values and SMAPI uses a deadzone, it
        // will race with Stardew (and usually lose), with the practical symptom being that if we
        // try to do the suppression in the "normal" spot (e.g. on input events), Stardew still
        // receives not only the initial trigger press, but another spurious "press" when the
        // trigger is released.
        //
        // The "solution" - preemptively suppress all trigger presses whenever the player is being
        // controlled, and punch through SMAPI's controller abstraction when reading the trigger
        // values ourselves. This will almost certainly cause incompatibilities with any other mods
        // that want to leverage the trigger buttons outside of menus... but then again, the overall
        // functionality is inherently incompatible regardless of hackery.
        Helper.Input.Suppress(SButton.LeftTrigger);
        Helper.Input.Suppress(SButton.RightTrigger);
    }

    private void Input_ButtonsChanged(object? sender, ButtonsChangedEventArgs e)
    {
        if (!Context.IsWorldReady || RemainingActivationDelayMs > 0 || Cursor.ActiveMenu is null)
        {
            return;
        }
        
        foreach (var button in e.Pressed)
        {
            if (Cursor.CurrentTarget is not null)
            {
                if (button == config.SecondaryActionButton)
                {
                    ScheduleActivation(/* forceSelect= */ config.SecondaryAction);
                    Helper.Input.Suppress(button);
                    return;
                }
                else if (IsActivationButton(button))
                {
                    ScheduleActivation(/* forceSelect= */ config.PrimaryAction);
                    Helper.Input.Suppress(button);
                    return;
                }
            }

            switch (button)
            {
                case SButton.LeftShoulder:
                    // We could also apply an offset equal to the Config.MaxInventoryItems value.
                    // But that could land us in a weird situation where the menu gets permanently
                    // out of sync with the toolbar, since the configured size does not have to be a
                    // multiple of the true page size.
                    //
                    // Also note that the reason not to immediately apply an offset here (i.e. via
                    // Farmer.shiftToolbar) is that if the player cancels out of the menu without
                    // selecting anything, or quick-activates a consumable item, we don't want to
                    // change the tool that's already equipped, which would happen automatically if
                    // using shiftToolbar.
                    ApplyMenuOffset(-GameConstants.BACKPACK_PAGE_SIZE);
                    UpdateMenuItems(Cursor.ActiveMenu);
                    break;
                case SButton.RightShoulder:
                    ApplyMenuOffset(GameConstants.BACKPACK_PAGE_SIZE);
                    UpdateMenuItems(Cursor.ActiveMenu);
                    break;
            }
        }
    }

    private Painter CreatePainter()
    {
        return new(Game1.graphics.GraphicsDevice, () => config.Styles);
    }

    private PlayerState CreatePlayerState()
    {
        var cursor = new Cursor(() => config);
        return new(cursor);
    }

    private static int GetCurrentNonEmptyToolIndex()
    {
        return Game1.player.CurrentItem is not null
            ? Game1.player.Items
                .Take(Game1.player.CurrentToolIndex + 1)
                .Where(item => item is not null)
                .Count() - 1
            : -1;
    }

    private static GamePadState GetRawGamePadState()
    {
        return Game1.playerOneIndex >= PlayerIndex.One
            ? GamePad.GetState(Game1.playerOneIndex)
            : new GamePadState();
    }

    private Func<DelayedActions?, ItemActivationResult>? GetSelectedItemActivation(
        ItemAction preferredAction)
    {
        var itemIndex = Cursor.CurrentTarget?.SelectedIndex;
        return itemIndex < ActiveMenuItems.Count
            ? (delayedActions) => ActiveMenuItems[itemIndex.Value]
                .Activate(delayedActions, preferredAction)
            : null;
    }

    private float GetSelectionBlend()
    {
        if (PendingActivation is null)
        {
            return 1.0f;
        }
        var elapsed = (float)(config.ActivationDelayMs - RemainingActivationDelayMs);
        return MathF.Abs(((elapsed / 80) % 2) - 1);
    }

    private bool IsActivationButton(SButton button)
    {
        return config.PrimaryActivation switch
        {
            ItemActivationMethod.ActionButtonPress => button.IsActionButton(),
            ItemActivationMethod.ThumbStickPress => Cursor.IsThumbStickForActiveMenu(button),
            _ => false,
        };
    }

    private ItemActivationResult MaybeInvokePendingActivation()
    {
        if (PendingActivation is null)
        {
            // We shouldn't actually hit this, since it's only called from conditional blocks that
            // have already confirmed there's a pending activation.
            // Nonetheless, for safety we assign a special result type to this, just in case the
            // assumption gets broken later.
            return ItemActivationResult.Ignored;
        }
        if (IsActivationDelayed && RemainingActivationDelayMs > 0)
        {
            return ItemActivationResult.Delayed;
        }
        var result = RemainingActivationDelayMs <= 0
            ? PendingActivation.Invoke(null)
            : PendingActivation.Invoke(config.DelayedActions);
        if (result == ItemActivationResult.Delayed)
        {
            Game1.playSound("select");
            IsActivationDelayed = true;
        }
        // Because we allow "internal" page changes within the radial menu that don't go through
        // Farmer.shiftToolbar (on purpose), the selected index can now be on a non-active page.
        // To avoid confusing the game's UI, check for this condition and switch to the backpack
        // page that actually does contain the index.
        if (result == ItemActivationResult.Selected
            && Game1.player.CurrentToolIndex >= GameConstants.BACKPACK_PAGE_SIZE)
        {
            var items = Game1.player.Items;
            var currentPage = Game1.player.CurrentToolIndex / GameConstants.BACKPACK_PAGE_SIZE;
            var indexOnPage = Game1.player.CurrentToolIndex % GameConstants.BACKPACK_PAGE_SIZE;
            var newFirstIndex = currentPage * GameConstants.BACKPACK_PAGE_SIZE;
            var itemsBefore = items.GetRange(0, newFirstIndex);
            var itemsAfter = items.GetRange(newFirstIndex, items.Count - newFirstIndex);
            items.Clear();
            items.AddRange(itemsAfter);
            items.AddRange(itemsBefore);
            Game1.player.CurrentToolIndex = indexOnPage;
            // Menu offset applies to the same Items array we just modified, so it has to be reset
            // in order for the radial menu to stay in sync.
            //
            // Offset is also reset when bringing up the menu, so in a certain sense, this is
            // superfluous. However, resetting on each menu open is a design choice that might seem
            // annoying to some users, or we might want to revisit in the future, whereas the offset
            // always needs to be reset after rebuilding the inventory, just to avoid bugging out.
            MenuOffset = 0;
        }
        return result;
    }

    private void LoadGmcmKeybindings()
    {
        if (configMenuApi is null)
        {
            Monitor.Log(
                $"Couldn't read global keybindings; mod {GMCM_MOD_ID} is not installed.",
                LogLevel.Warn);
            return;
        }
        Monitor.Log("Generic Mod Config Menu is loaded; reading keybindings.", LogLevel.Info);
        try
        {
            gmcmKeybindings = GenericModConfigKeybindings.Load();
            Monitor.Log("Finished reading keybindings from GMCM.", LogLevel.Info);
            if (config.DumpAvailableKeyBindingsOnStartup)
            {
                foreach (var option in gmcmKeybindings.AllOptions)
                {
                    Monitor.Log(
                        "Found keybind option: " +
                        $"[{option.ModManifest.UniqueID}] - {option.UniqueFieldName}",
                        LogLevel.Info);
                }
            }
            gmcmSync = new(() => config, gmcmKeybindings, Monitor);
            gmcmSync.SyncAll();
            Helper.WriteConfig(config);
        }
        catch (Exception ex)
        when (ex is InvalidOperationException || ex is TargetInvocationException)
        {
            Monitor.Log(
                $"Couldn't read global keybindings; the current version of {GMCM_MOD_ID} is " +
                $"not compatible.\n{ex.GetType().FullName}: {ex.Message}\n{ex.StackTrace}",
                LogLevel.Error);
        }
    }

    private void RegisterConfigMenu()
    {
        if (configMenuApi is null)
        {
            return;
        }
        configMenuApi.Register(
            mod: ModManifest,
            reset: ResetConfiguration,
            save: () => Helper.WriteConfig(config));
        configMenu = new(
            configMenuApi,
            gmcmOptionsApi,
            gmcmKeybindings,
            gmcmSync,
            ModManifest,
            Helper.Translation,
            Helper.ModContent,
            textureHelper,
            Helper.Events.GameLoop,
            () => config);
        configMenu.Setup();
    }

    private void ResetConfiguration()
    {
        config = new();
    }

    private void RestorePreMenuState()
    {
        Game1.freezeControls = PreMenuState.WasFrozen;
    }

    private void ScheduleActivation(ItemAction preferredAction)
    {
        IsActivationDelayed = false;
        PendingActivation = GetSelectedItemActivation(preferredAction);
        if (PendingActivation is null)
        {
            return;
        }
        RemainingActivationDelayMs = config.ActivationDelayMs;
        Cursor.SuppressUntilTriggerRelease();
    }

    private void ActivateCustomMenuItem(CustomMenuItemConfiguration item)
    {
        if (!item.Keybind.IsBound)
        {
            Game1.showRedMessage(Helper.Translation.Get("error.missingbinding"));
            return;
        }
        keybindActivator.Activate(item.Keybind);
    }

    private void ApplyMenuOffset(int offset)
    {
        var previousOffset = MenuOffset;
        var maxItems = Game1.player.Items.Count;
        MenuOffset = (MenuOffset + offset + maxItems) % maxItems;
        if (MenuOffset != previousOffset)
        {
            Game1.playSound("shwip");
        }
    }

    private IEnumerable<(Item item, int index)> GetVisibleInventoryItems()
    {
        var items = Game1.player.Items;
        var index = PlayerState.MenuOffset;
        for (int i = 0; i < config.MaxInventoryItems; i++)
        {
            yield return (items[index], index);
            if (++index >= items.Count)
            {
                index = 0;
            }
        }
    }

    private void UpdateMenuItems(MenuKind? menu)
    {
        ActiveMenuItems = menu switch
        {
            MenuKind.Inventory => GetVisibleInventoryItems()
                .Select(x => x.item != null ? menuItemBuilder.GameItem(x.item, x.index) : null)
                .Where(i => i is not null)
                .Cast<MenuItem>()
                .ToList(),
            MenuKind.Custom => config.CustomMenuItems
                .Select(item => menuItemBuilder.CustomItem(item))
                .ToList(),
            _ => [],
        };
        Painter.Items = ActiveMenuItems;
    }
}
