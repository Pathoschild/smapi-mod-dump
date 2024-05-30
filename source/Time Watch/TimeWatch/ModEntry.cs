/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KyuubiRan/TimeWatch
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using TimeWatch.Data;
using TimeWatch.External;
using TimeWatch.Options;
using TimeWatch.Utils;
using TimeWatch.Widgets;

namespace TimeWatch;

public class ModEntry : Mod
{
    private TimeWatchButton? _timeWatchButton;
    
    public override void Entry(IModHelper helper)
    {
        ModHelpers.Config = Helper.ReadConfig<ModConfig>();
        ModHelpers.Monitor = Monitor;
        ModHelpers.Helper = Helper;

        helper.Events.GameLoop.GameLaunched += OnGameLaunched;
        helper.Events.GameLoop.ReturnedToTitle += OnReturnToTitle;
        helper.Events.GameLoop.Saving += OnGameSaving;
        helper.Events.GameLoop.DayStarted += OnDayStared;
        helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
        helper.Events.Input.ButtonPressed += OnKeyDown;
        I18n.Init(helper.Translation);
    }

    private void OnDayStared(object? sender, DayStartedEventArgs e)
    {
        MagicTimeWatch.TodayWorldSeekedTime = new GameTimeSpan();
    }

    private void OnReturnToTitle(object? sender, ReturnedToTitleEventArgs e)
    {
        _timeWatchButton?.Dispose();
        _timeWatchButton = null;
    }
    
    private void OnKeyDown(object? sender, ButtonPressedEventArgs e)
    {
        if (!Context.IsPlayerFree)
            return;

        if (Game1.IsMultiplayer && ModHelpers.Config.MultiPlayHostOnly && !Game1.IsMasterGame)
            return;

        if (e.Button != ModHelpers.Config.IncreaseTimeKeyBind && e.Button != ModHelpers.Config.DecreaseTimeKeyBind)
            return;

        var cnt = ModHelpers.Config.DefaultSeekTimeValue;
        if (Helper.Input.IsDown(SButton.LeftShift))
            cnt = ModHelpers.Config.HoldShiftSeekTimeValue;
        else if (Helper.Input.IsDown(SButton.LeftControl))
            cnt = ModHelpers.Config.HoldCtrlSeekTimeValue;

        cnt = cnt.CoerceIn(ModConstants.MinSeekTime, ModConstants.MaxSeekTime);

        var tw = TimeWatchManager.CurrentPlayerTimeWatch;

        var isPlus = true;
        if (e.Button == ModHelpers.Config.IncreaseTimeKeyBind)
            isPlus = true;
        else if (e.Button == ModHelpers.Config.DecreaseTimeKeyBind)
            isPlus = false;

        var cntSeeked = tw.Seek(cnt * (isPlus ? 1 : -1), ModHelpers.Config.UpdateGameObjects,
            ModHelpers.Config.ShowTimeChangedNotify);
#if DEBUG
        Monitor.Log($"{(isPlus ? "Increase" : "Decrease")} Time Seeked: {cntSeeked}, Stored: {tw.StoredTime}",
            LogLevel.Debug);
#endif
    }

    private void OnGameSaving(object? sender, SavingEventArgs e)
    {
        TimeWatchManager.OnSave();
        // MagicTimeWatch.OnSave();
    }

    private void OnSaveLoaded(object? sender, SaveLoadedEventArgs e)
    {
        _timeWatchButton ??= new TimeWatchButton();
        TimeWatchManager.OnLoad();
        // MagicTimeWatch.OnLoad();
    }

    private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
    {
        var configMenuVersion = Helper.ModRegistry.Get("spacechase0.GenericModConfigMenu")?.Manifest.Version;
        if (configMenuVersion is null)
            return;

        const string minConfigMenuVersionVersion = "1.6.0";
        if (configMenuVersion.IsOlderThan(minConfigMenuVersionVersion))
        {
            Monitor.Log(
                $"Detected Generic Mod Config Menu {configMenuVersion} but expected {minConfigMenuVersionVersion} or newer. Disabling integration with that mod.",
                LogLevel.Warn
            );
            return;
        }

        var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (configMenu is null)
            return;

        configMenu.Register(ModManifest, () => ModHelpers.Config = new ModConfig(),
            () => Helper.WriteConfig(ModHelpers.Config));

        configMenu.AddKeybind(
            ModManifest,
            () => ModHelpers.Config.IncreaseTimeKeyBind,
            keybind => ModHelpers.Config.IncreaseTimeKeyBind = keybind,
            I18n.Config_IncreaseTimeKeyBinding,
            I18n.Config_IncreaseTimeKeyBindingTooltip
        );

        configMenu.AddKeybind(
            ModManifest,
            () => ModHelpers.Config.DecreaseTimeKeyBind,
            keybind => ModHelpers.Config.DecreaseTimeKeyBind = keybind,
            I18n.Config_DecreaseTimeKeyBinding,
            I18n.Config_DecreaseTimeKeyBindingTooltip
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => ModHelpers.Config.DefaultSeekTimeValue,
            value => ModHelpers.Config.DefaultSeekTimeValue =
                value.CoerceIn(ModConstants.MinSeekTime, ModConstants.MaxSeekTime),
            I18n.Config_SeekTimeValue,
            I18n.Config_SeekTimeValueTooltip,
            min: ModConstants.MinSeekTime,
            max: ModConstants.MaxSeekTime,
            interval: ModConstants.MinSeekTime
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => ModHelpers.Config.HoldShiftSeekTimeValue,
            value => ModHelpers.Config.HoldShiftSeekTimeValue =
                value.CoerceIn(ModConstants.MinSeekTime, ModConstants.MaxSeekTime),
            I18n.Config_HoldShiftSeekTimeValue,
            I18n.Config_HoldShiftSeekTimeValueTooltip,
            min: ModConstants.MinSeekTime,
            max: ModConstants.MaxSeekTime,
            interval: ModConstants.MinSeekTime
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => ModHelpers.Config.HoldCtrlSeekTimeValue,
            value => ModHelpers.Config.HoldCtrlSeekTimeValue =
                value.CoerceIn(ModConstants.MinSeekTime, ModConstants.MaxSeekTime),
            I18n.Config_HoldCtrlSeekTimeValue,
            I18n.Config_HoldCtrlSeekTimeValueTooltip,
            min: ModConstants.MinSeekTime,
            max: ModConstants.MaxSeekTime,
            interval: ModConstants.MinSeekTime
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => ModHelpers.Config.DailyMaximumStorableTime,
            value => ModHelpers.Config.DailyMaximumStorableTime =
                value.CoerceIn(ModConstants.MinDailyStorableTime, ModConstants.MaxDailyStorableTime),
            I18n.Config_DailyMaximumStorableTime,
            I18n.Config_DailyMaximumStorableTimeTooltip,
            min: ModConstants.MinDailyStorableTime,
            max: ModConstants.MaxDailyStorableTime,
            interval: 1
        );

        configMenu.AddNumberOption(
            ModManifest,
            () => ModHelpers.Config.MaximumStorableTime,
            value => ModHelpers.Config.MaximumStorableTime =
                value.CoerceIn(ModConstants.MinStorableTime, ModConstants.MaxStorableTime),
            I18n.Config_MaximumStorableTime,
            I18n.Config_MaximumStorableTimeTooltip
        );

        configMenu.AddBoolOption(
            ModManifest,
            () => ModHelpers.Config.ShowTimeChangedNotify,
            value => ModHelpers.Config.ShowTimeChangedNotify = value,
            I18n.Config_ShowTimeChangedNotify,
            I18n.Config_ShowTimeChangedNotifyTooltip);

        configMenu.AddBoolOption(
            ModManifest,
            () => ModHelpers.Config.MultiPlayHostOnly,
            value => ModHelpers.Config.MultiPlayHostOnly = value,
            I18n.Config_MultiPlayHostOnly,
            I18n.Config_MultiPlayHostOnlyTooltip);

        configMenu.AddBoolOption(
            ModManifest,
            () => ModHelpers.Config.UpdateGameObjects,
            value => ModHelpers.Config.UpdateGameObjects = value,
            I18n.Config_UpdateObjects,
            I18n.Config_UpdateObjectsTooltip);
    }
}