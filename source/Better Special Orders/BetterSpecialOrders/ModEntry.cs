/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/xeru98/StardewMods
**
*************************************************/

using System.Collections.ObjectModel;
using BetterSpecialOrders.Messages;
using BetterSpecialOrders.UI;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.GameData.SpecialOrders;
using StardewValley.Menus;
using StardewValley.Mods;
using StardewValley.SpecialOrders;

namespace BetterSpecialOrders;

public class ModEntry : Mod
{

    public static IModHelper? GHelper;
    public static IMonitor? GMonitor;

    public static string ModID = "";

    private ModConfig config
    {
        get { return RerollManager.Get().config; }
        set { RerollManager.Get().config = value;  }
    }
    
    
    public override void Entry(IModHelper helper)
    {
        GHelper = helper;
        GMonitor = Monitor;
        ModID = this.ModManifest.UniqueID;
        RerollManager.Get(); // init the game state since the monitor and helper now exist

        // hook up events
        helper.Events.GameLoop.GameLaunched += Lifecycle_OnGameLaunch;
        helper.Events.GameLoop.DayEnding += Lifecycle_OnDayEnd;
        helper.Events.GameLoop.DayStarted += Lifecycle_OnDayStart;
        helper.Events.Input.ButtonsChanged += Input_OnButtonsChanged;
        helper.Events.Display.MenuChanged += Display_OnMenuChanged;
        helper.Events.Multiplayer.ModMessageReceived += Multiplayer_OnMessageRecieved;
    }
    
    // Called when the game is launched and sets up the GMCM integration if found
    internal void Lifecycle_OnGameLaunch(object? sender, GameLaunchedEventArgs args)
    {
        SetupGMCM();
    }
    
    internal void Lifecycle_OnDayEnd(object? sendex, DayEndingEventArgs args)
    {
        if (!Context.IsMainPlayer)
        {
            return;
        }

        RerollManager.Get().CacheCurrentAvailableSpecialOrders();
    }

    internal void Lifecycle_OnDayStart(object? sender, DayStartedEventArgs args)
    {
        if (!Context.IsMainPlayer)
        {
            return;
        }
        
        // use this over Game1.Date.DayOfWeek because that returns an enum
        int dayOfTheWeek = Game1.Date.DayOfMonth % 7;
        
        // stardew valley board
        if (config.sv_refresh_schedule[dayOfTheWeek])
        {
            RerollManager.Get().Reroll(Constants.SVBoardContext);
        }
        else
        {
            RerollManager.Get().ReloadSpecialOrdersFromCache(Constants.SVBoardContext);
        }
        
        // qi board
        if (config.qi_refresh_schedule[dayOfTheWeek])
        {
            RerollManager.Get().Reroll(Constants.QiBoardContext);
        }
        else
        {
            RerollManager.Get().ReloadSpecialOrdersFromCache(Constants.QiBoardContext);
        }
        
        // Reset the daily rerolls
        RerollManager.Get().ResetRerolls(resetDayTotal: true);
    }

    

    // Triggered when the buttons are changed
    internal void Input_OnButtonsChanged(object? sender, ButtonsChangedEventArgs args)
    {
        // only the host can trigger a reset
        if (!Context.IsMainPlayer)
        {
            return;
        }
        
        if (config.resetRerollsKeybind.IsDown())
        {
            Monitor.Log("Host Resetting Reroll With Keybind", LogLevel.Debug);
            RerollManager.Get().ResetRerolls();
        }
    }

    internal void Display_OnMenuChanged(object? sender, MenuChangedEventArgs args)
    {
        if (!Context.IsWorldReady)
        {
            return;
        }

        if (args.NewMenu is BetterSpecialOrdersBoard)
        {
            return;
        }

        if (args.NewMenu is SpecialOrdersBoard)
        {
            Monitor.Log("New Menu is a special orders board... replacing with custom one", LogLevel.Debug);
            string orderType = (args.NewMenu as SpecialOrdersBoard).GetOrderType();
            Game1.activeClickableMenu = new BetterSpecialOrdersBoard(orderType);
        }
    }

    internal void Multiplayer_OnMessageRecieved(object? sender, ModMessageReceivedEventArgs args)
    {
        if (args.FromModID != ModID)
        {
            return;
        }
        
        if (args.Type == Constants.REQUEST_REROLL)
        {
            RequestReroll msg = args.ReadAs<RequestReroll>();
            RerollManager.Get().Reroll(msg.orderType);
        }
    }

    private void OnGMCMUpdated()
    {
        if (Context.IsMainPlayer)
        {
            RerollManager.Get().RebuildConfig();
        }
    }

    // sets up the GMCM
    private void SetupGMCM()
    {
        IGenericModConfigMenuApi? GMCM_API = Helper.ModRegistry.GetApi<GenericModConfigMenu.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
        if (GMCM_API == null)
        {
            Monitor.Log("Generic Mod Config Menu not found. Skipping mod menu setup", LogLevel.Info);
            return;
        }
        
        GMCM_API.Register(mod: ModManifest, reset: () => config = new ModConfig(), save: () => Helper.WriteConfig(config));
        
        
        // GENERAL
        GMCM_API.AddSectionTitle(
            mod: ModManifest,
            text: () => "General Settings"
        );
        
        GMCM_API.AddParagraph(
            mod: ModManifest,
            text: () => "All of these settings only need to be set by the host. All joining farmers will use the host's settings"
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Use unseeded random generator",
            tooltip: () => "When unchecked, randomizer will use seeded pseudorandom generator. When checked will use unseeded random generator.",
            getValue: () => config.useTrueRandom,
            setValue: value =>
            {
                config.useTrueRandom = value;
                OnGMCMUpdated();
            }
        );
        
        GMCM_API.AddKeybindList(
            mod: ModManifest,
            name: () => "Reroll Reset Keybind",
            tooltip: () => "Allows the host to reset the number of available rerolls back to their max amount",
            getValue: () => config.resetRerollsKeybind,
            setValue: value =>
            {
                config.resetRerollsKeybind = value;
                OnGMCMUpdated();
            }
        );
        
        
        // SV
        GMCM_API.AddSectionTitle(
            mod: ModManifest,
            text: () => "Stardew Valley Special Orders Board"
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Allow Rerolls",
            tooltip: () => "When checked, allows the Stardew Valley Special Orders Board to be rerolled",
            getValue: () => config.sv_allowReroll,
            setValue: value =>
            {
                config.sv_allowReroll = value;
                OnGMCMUpdated();
            }
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Infinite Rerolls",
            tooltip: () => "When checked, allows the Stardew Valley Special Orders Board to be rerolled infinitely",
            getValue: () => config.sv_infiniteReroll,
            setValue: value =>
            {
                config.sv_infiniteReroll = value;
                OnGMCMUpdated();
            }
        );

        GMCM_API.AddNumberOption(
            mod: ModManifest,
            name: () => "Max Daily Rerolls",
            tooltip: () => "The number of daily rerolls the team has shared across them per day for use on the Stardew Valley Special Orders Board",
            getValue: () => config.sv_maxRerollCount,
            setValue: value =>
            {
                config.sv_maxRerollCount = value;
                OnGMCMUpdated();
            },
            min: 1,
            max: 10
        );
        
        GMCM_API.AddParagraph(
            mod: ModManifest,
            text: () => "Refresh Schedule"
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Monday",
            getValue: () => config.sv_refresh_schedule[0],
            setValue: value =>
            {
                config.sv_refresh_schedule[0] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Tuesday",
            getValue: () => config.sv_refresh_schedule[1],
            setValue: value =>
            {
                config.sv_refresh_schedule[1] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Wednesday",
            getValue: () => config.sv_refresh_schedule[2],
            setValue: value =>
            {
                config.sv_refresh_schedule[2] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Thursday",
            getValue: () => config.sv_refresh_schedule[3],
            setValue: value =>
            {
                config.sv_refresh_schedule[3] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Friday",
            getValue: () => config.sv_refresh_schedule[4],
            setValue: value =>
            {
                config.sv_refresh_schedule[4] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Saturday",
            getValue: () => config.sv_refresh_schedule[5],
            setValue: value =>
            {
                config.sv_refresh_schedule[5] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Sunday",
            getValue: () => config.sv_refresh_schedule[6],
            setValue: value =>
            {
                config.sv_refresh_schedule[6] = value;
                OnGMCMUpdated();
            }
        );
        
        
        // Qi
        GMCM_API.AddSectionTitle(
            mod: ModManifest,
            text: () => "Mr. Qi Special Orders Board"
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Allow Rerolls",
            tooltip: () => "When checked, allows the Mr. Qi Special Orders Board to be rerolled",
            getValue: () => config.qi_allowReroll,
            setValue: value =>
            {
                config.qi_allowReroll = value;
                OnGMCMUpdated();
            }
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Infinite Rerolls",
            tooltip: () => "When checked, allows the Mr. Qi Special Orders Board to be rerolled infinitely",
            getValue: () => config.qi_infiniteReroll,
            setValue: value =>
            {
                config.qi_infiniteReroll = value;
                OnGMCMUpdated();
            }
        );

        GMCM_API.AddNumberOption(
            mod: ModManifest,
            name: () => "Max Daily Rerolls",
            tooltip: () => "The number of daily rerolls the team has shared across them per day for use on Mr. Qi's Special Order Board",
            getValue: () => config.qi_maxRerollCount,
            setValue: value =>
            {
                config.qi_maxRerollCount = value;
                OnGMCMUpdated();
            },
            min: 1,
            max: 10
        );
        
        GMCM_API.AddParagraph(
            mod: ModManifest,
            text: () => "Refresh Schedule"
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Monday",
            getValue: () => config.qi_refresh_schedule[0],
            setValue: value =>
            {
                config.qi_refresh_schedule[0] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Tuesday",
            getValue: () => config.qi_refresh_schedule[1],
            setValue: value =>
            {
                config.qi_refresh_schedule[1] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Wednesday",
            getValue: () => config.qi_refresh_schedule[2],
            setValue: value =>
            {
                config.qi_refresh_schedule[2] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Thursday",
            getValue: () => config.qi_refresh_schedule[3],
            setValue: value =>
            {
                config.qi_refresh_schedule[3] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Friday",
            getValue: () => config.qi_refresh_schedule[4],
            setValue: value =>
            {
                config.qi_refresh_schedule[4] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Saturday",
            getValue: () => config.qi_refresh_schedule[5],
            setValue: value =>
            {
                config.qi_refresh_schedule[5] = value;
                OnGMCMUpdated();
            }
        );
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Sunday",
            getValue: () => config.qi_refresh_schedule[6],
            setValue: value =>
            {
                config.qi_refresh_schedule[6] = value;
                OnGMCMUpdated();
            }
        );
        
        
        // Desert Event
        GMCM_API.AddSectionTitle(
            mod: ModManifest,
            text: () => "Desert Festival Special Orders Board"
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Allow Rerolls",
            tooltip: () => "When checked, allows the Desert Festival Special Orders Board to be rerolled",
            getValue: () => config.de_allowReroll,
            setValue: value =>
            {
                config.de_allowReroll = value;
                OnGMCMUpdated();
            }
        );
        
        GMCM_API.AddBoolOption(
            mod: ModManifest,
            name: () => "Infinite Rerolls",
            tooltip: () => "When checked, allows the Desert Festival Special Orders Board to be rerolled infinitely",
            getValue: () => config.de_infiniteReroll,
            setValue: value =>
            {
                config.de_infiniteReroll = value;
                OnGMCMUpdated();
            }
        );

        GMCM_API.AddNumberOption(
            mod: ModManifest,
            name: () => "Max Daily Rerolls",
            tooltip: () => "The number of daily rerolls the team has shared across them per day for use on Desert Festival Special Order Board",
            getValue: () => config.de_maxRerollCount,
            setValue: value =>
            {
                config.de_maxRerollCount = value;
                OnGMCMUpdated();
            },
            min: 1,
            max: 10
        );
    }
}