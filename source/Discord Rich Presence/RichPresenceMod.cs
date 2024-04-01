/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/RuiNtD/SVRichPresence
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using DiscordRPC;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using Constants = StardewModdingAPI.Constants;
using LogLevel = StardewModdingAPI.LogLevel;
using Utility = StardewValley.Utility;

namespace SVRichPresence
{
  public class RichPresenceMod : Mod
  {
    private static readonly string clientId = "444517509148966923";
    private static readonly string steamId = "413150";
    private static readonly string ModURL = "https://www.nexusmods.com/stardewvalley/mods/2156";

    private ModConfig Config = new();
    private IRichPresenceAPI api;
    private DiscordRpcClient client;

    public override void Entry(IModHelper helper)
    {
      I18n.Init(Helper.Translation);
      api = new RichPresenceAPI(this);
      client = new DiscordRpcClient(
        clientId,
        autoEvents: false,
        logger: new MonitorLogger(Monitor)
      );
      client.SetSubscription(EventType.Join);
      client.RegisterUriScheme(steamId);
      client.OnReady += (sender, e) =>
        Monitor.Log(I18n.Console_DiscordConnected(e.User.ToString()), LogLevel.Info);
      client.Initialize();

      #region Console Commands
      Helper.ConsoleCommands.Add(
        "DiscordReload",
        I18n.Command_Reload_Desc(),
        (string command, string[] args) =>
        {
          LoadConfig();
        }
      );
      Helper.ConsoleCommands.Add(
        "DiscordFormat",
        I18n.Command_Format_Desc(),
        (string command, string[] args) =>
        {
          string text = this.api.FormatText(string.Join(" ", args));
          Monitor.Log(I18n.Command_Format_Result(text), LogLevel.Info);
        }
      );
      Helper.ConsoleCommands.Add(
        "DiscordTags",
        I18n.Command_Tags_Desc(),
        (string command, string[] args) =>
        {
          bool all = string.Join("", args).ToLower().StartsWith("all");
          string output = $"{I18n.TagList_Header()}\n";
          output += FormatTags(out int nulls, format: "  {{{0}}}: {1}", pad: true, all: all);
          if (nulls > 0)
            output += $"\n\n{I18n.Command_Tags_Nulls(nulls)}";
          Monitor.Log(output, LogLevel.Info);
        }
      );
      #endregion
      LoadConfig();

      Helper.Events.GameLoop.GameLaunched += RegisterConfigMenu;
      Helper.Events.Input.ButtonReleased += HandleButton;
      Helper.Events.GameLoop.UpdateTicked += DoUpdate;
      Helper.Events.GameLoop.SaveLoaded += SetTimestamp;
      Helper.Events.GameLoop.ReturnedToTitle += SetTimestamp;
      Helper.Events.GameLoop.SaveLoaded += (object sender, SaveLoadedEventArgs e) =>
        api.GamePresence = I18n.GamePresence_GettingStarted();
      Helper.Events.GameLoop.SaveCreated += (object sender, SaveCreatedEventArgs e) =>
        api.GamePresence = I18n.GamePresence_StartingNewGame();
      Helper.Events.GameLoop.GameLaunched += (object sender, GameLaunchedEventArgs e) =>
      {
        SetTimestamp();
        timestampSession = Timestamps.Now;
      };

      #region Default Tags
      var None = api.None;

      Tag("Activity", () => api.GamePresence);
      Tag("ModCount", () => Helper.ModRegistry.GetAll().Count().ToString());
      Tag("SMAPIVersion", () => Constants.ApiVersion.ToString());
      Tag("StardewVersion", () => Game1.version);
      Tag("RPCModVersion", () => ModManifest.Version.ToString());
      Tag("Song", () => Utility.getSongTitleFromCueName(Game1.currentSong?.Name ?? None));

      WTag("Name", () => Game1.player.Name);
      WTag(
        "Farm",
        () => Game1.content.LoadString("Strings\\UI:Inventory_FarmName", api.FormatTag("FarmName"))
      );
      WTag("FarmName", () => Game1.player.farmName.ToString());
      WTag("PetName", () => Game1.player.hasPet() ? Game1.player.getPetDisplayName() : None);
      WTag("Location", () => Game1.currentLocation.Name);
      WTag(
        "RomanticInterest",
        () => Utility.getTopRomanticInterest(Game1.player)?.getName() ?? None
      );
      WTag(
        "NonRomanticInterest",
        () => Utility.getTopNonRomanticInterest(Game1.player)?.getName() ?? None
      );

      WTag(
        "Money",
        () =>
        {
          // Copied from LoadGameMenu.drawSlotMoney
          string cashText = Game1.content.LoadString(
            "Strings\\StringsFromCSFiles:LoadGameMenu.cs.11020",
            Utility.getNumberWithCommas(Game1.player.Money)
          );
          if (
            Game1.player.Money == 1
            && LocalizedContentManager.CurrentLanguageCode
              == LocalizedContentManager.LanguageCode.pt
          )
            cashText = cashText[..^1];
          return cashText;
        }
      );
      WTag("MoneyCommas", () => Utility.getNumberWithCommas(Game1.player.Money));
      WTag("MoneyNumber", () => Game1.player.Money.ToString());
      WTag(
        "Level",
        () =>
          Game1.content.LoadString(
            "Strings\\UI:Inventory_PortraitHover_Level",
            api.FormatTag("LevelNumber")
          )
      );
      WTag("LevelNumber", () => Game1.player.Level.ToString());
      WTag("Title", () => Game1.player.getTitle().ToString());
      WTag(
        "TotalTime",
        () => Utility.getHoursMinutesStringFromMilliseconds(Game1.player.millisecondsPlayed)
      );

      WTag("Health", () => Game1.player.health.ToString());
      WTag("HealthMax", () => Game1.player.maxHealth.ToString());
      WTag(
        "HealthPercent",
        () => Math.Round((double)Game1.player.health / Game1.player.maxHealth * 100, 2).ToString()
      );
      WTag("Energy", () => Game1.player.Stamina.ToString());
      WTag("EnergyMax", () => Game1.player.MaxStamina.ToString());
      WTag(
        "EnergyPercent",
        () => Math.Round((double)Game1.player.Stamina / Game1.player.MaxStamina * 100, 2).ToString()
      );

      WTag("Time", () => Game1.getTimeOfDayString(Game1.timeOfDay));
      WTag("Date", () => Utility.getDateString());
      WTag("Season", () => Utility.getSeasonNameFromNumber(SDate.Now().SeasonIndex));
      WTag("DayOfWeek", () => Game1.shortDayDisplayNameFromDayOfSeason(SDate.Now().Day));

      WTag("Day", () => SDate.Now().Day.ToString());
      WTag("DayPad", () => $"{SDate.Now().Day:00}");
      WTag("DaySuffix", () => Utility.getNumberEnding(SDate.Now().Day));
      WTag("Year", () => SDate.Now().Year.ToString());
      WTag("YearSuffix", () => Utility.getNumberEnding(SDate.Now().Year));

      WTag(
        "GameInfo",
        () =>
          Context.IsMultiplayer
            ? Context.IsMainPlayer
              ? I18n.HostingCoop()
              : I18n.PlayingCoop()
            : I18n.PlayingSolo()
      );
      #endregion
    }

    private void Tag(string tag, Func<string> func) => api.SetTag(this.ModManifest, tag, func);

    private void WTag(string tag, Func<string> func) =>
      Tag(
        tag,
        () =>
        {
          if (!Context.IsWorldReady)
            return null;
          return func();
        }
      );

    private string FormatTags(
      out int nulls,
      string format = "{{{0}}}: {1}",
      bool pad = false,
      bool all = false
    )
    {
      var tags = api.ResolveAllTags();
      nulls = 0;

      Dictionary<string, Dictionary<string, string>> groups = new() { [""] = new() };
      foreach (var tag in tags)
      {
        string owner = api.GetTagOwner(tag.Key) ?? "";
        owner = Helper.ModRegistry.Get(owner)?.Manifest.Name ?? "";

        if (!groups.ContainsKey(owner))
          groups[owner] = new();

        var val = tag.Value.Value;
        if (!all && val is null)
          nulls++;
        else
        {
          val ??= "[NULL]";
          if (!tag.Value.Success)
            val = "[ERROR]";
          groups[owner][tag.Key] = val;
        }
      }

      List<string> output = new(tags.Count + groups.Count);
      void list(Dictionary<string, string> group)
      {
        int longest = 0;
        if (pad)
          foreach (var tag in group)
            longest = Math.Max(longest, tag.Key.Length);
        foreach (var tag in group)
          output.Add(String.Format(format, tag.Key.PadLeft(longest), tag.Value));
      }
      void section(Dictionary<string, string> group, string name)
      {
        var count = group.Count;
        if (count == 0)
          return;

        output.Add("");
        if (name != "")
          output.Add(I18n.TagList_ModHeader(count, name));
        else
          output.Add(I18n.TagList_UnknownModsHeader(count));

        list(group);
      }
      list(groups[ModManifest.Name]);

      foreach (var group in groups)
      {
        if (group.Key == ModManifest.Name)
          continue;
        if (group.Key == "")
          continue;
        section(group.Value, group.Key);
      }
      section(groups[""], "");

      return string.Join("\n", output);
    }

    public override object GetApi() => api;

    private void RegisterConfigMenu(object sender, GameLaunchedEventArgs e)
    {
      // get Generic Mod Config Menu's API (if it's installed)
      var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(
        "spacechase0.GenericModConfigMenu"
      );
      if (configMenu is null)
        return;
      var mod = ModManifest;

      configMenu.Register(mod, reset: () => Config = new ModConfig(), save: () => SaveConfig());

      configMenu.AddBoolOption(
        mod,
        name: () => I18n.Options_ShowGlobalPlayTime(),
        getValue: () => Config.ShowGlobalPlayTime,
        setValue: value => Config.ShowGlobalPlayTime = value
      );
      configMenu.AddBoolOption(
        mod,
        name: () => I18n.Options_AddGetModButton(),
        tooltip: () => I18n.Options_AddGetModButton_Desc(),
        getValue: () => Config.AddGetModButton,
        setValue: value => Config.AddGetModButton = value
      );

      configMenu.AddSectionTitle(mod, () => I18n.Options_Preview());
      configMenu.AddParagraph(
        mod,
        () =>
        {
          var text = $"{api.FormatText(Conf.State)}\n";
          text += $"{api.FormatText(Conf.Details)}\n";
          text += $"{api.FormatText(Conf.LargeImageText)}\n";
          text += $"{api.FormatText(Conf.SmallImageText)}\n";
          return text;
        }
      );

      configMenu.AddSectionTitle(mod, () => I18n.Options_MenuPresence());
      RPCModMenuSection(configMenu, Config.MenuPresence);

      configMenu.AddSectionTitle(mod, () => I18n.Options_GamePresence());
      RPCModMenuSection(configMenu, Config.GamePresence);
      configMenu.AddBoolOption(
        mod,
        name: () => I18n.Options_ShowSeason(),
        tooltip: () => I18n.Options_ShowSeason_Desc(),
        getValue: () => Config.GamePresence.ShowSeason,
        setValue: value => Config.GamePresence.ShowSeason = value
      );
      configMenu.AddBoolOption(
        mod,
        name: () => I18n.Options_ShowFarmType(),
        tooltip: () => I18n.Options_ShowFarmType_Desc(),
        getValue: () => Config.GamePresence.ShowFarmType,
        setValue: value => Config.GamePresence.ShowFarmType = value
      );
      configMenu.AddBoolOption(
        mod,
        name: () => I18n.Options_ShowWeather(),
        tooltip: () => I18n.Options_ShowWeather_Desc(),
        getValue: () => Config.GamePresence.ShowWeather,
        setValue: value => Config.GamePresence.ShowWeather = value
      );
      configMenu.AddBoolOption(
        mod,
        name: () => I18n.Options_ShowPlayTime(),
        tooltip: () => I18n.Options_ShowPlayTime_Desc(),
        getValue: () => Config.GamePresence.ShowPlayTime,
        setValue: value => Config.GamePresence.ShowPlayTime = value
      );

      // Tags Page
      configMenu.AddPage(mod, "tags", () => I18n.Options_Page_Tags());
      configMenu.AddParagraph(
        mod,
        () =>
        {
          string output = FormatTags(out int nulls, pad: false);
          output += $"\n\n{I18n.Options_UnavailableTags(nulls)}";
          return output;
        }
      );
      configMenu.AddPageLink(mod, "alltags", () => I18n.Options_ShowAllTags());

      // All Tags Page
      configMenu.AddPage(mod, "alltags", () => I18n.Options_Page_AllTags());
      configMenu.AddParagraph(mod, () => FormatTags(out _, "{{{0}}}: {1}", pad: false, all: true));
    }

    private void RPCModMenuSection(IGenericModConfigMenuApi api, MenuPresence conf)
    {
      var mod = ModManifest;
      api.AddPageLink(mod, "tags", () => I18n.Options_ShowAvailableTags());
      api.AddTextOption(
        mod,
        name: () => I18n.Options_State(),
        getValue: () => conf.State,
        setValue: value => conf.State = value
      );
      api.AddTextOption(
        mod,
        name: () => I18n.Options_Details(),
        getValue: () => conf.Details,
        setValue: value => conf.Details = value
      );
      api.AddTextOption(
        mod,
        name: () => I18n.Options_LargeImageText(),
        getValue: () => conf.LargeImageText,
        setValue: value => conf.LargeImageText = value
      );
      api.AddTextOption(
        mod,
        name: () => I18n.Options_SmallImageText(),
        getValue: () => conf.SmallImageText,
        setValue: value => conf.SmallImageText = value
      );
      api.AddBoolOption(
        mod,
        name: () => I18n.Options_ForceSmallImage(),
        tooltip: () => I18n.Options_ForceSmallImage_Desc(),
        getValue: () => conf.ForceSmallImage,
        setValue: value => conf.ForceSmallImage = value
      );
    }

    private void HandleButton(object sender, ButtonReleasedEventArgs e)
    {
      if (e.Button != Config.ReloadConfigButton)
        return;
      try
      {
        LoadConfig();
        Game1.addHUDMessage(new HUDMessage(I18n.Notify_ReloadConfig(), HUDMessage.newQuest_type));
      }
      catch (Exception err)
      {
        Game1.addHUDMessage(
          new HUDMessage(I18n.Notify_ReloadConfig_Failed(), HUDMessage.error_type)
        );
        Monitor.Log(err.ToString(), LogLevel.Error);
      }
    }

    private void LoadConfig() => Config = Helper.ReadConfig<ModConfig>();

    private void SaveConfig() => Helper.WriteConfig(Config);

    private Timestamps timestampSession;
    private Timestamps timestampFarm;

    private void SetTimestamp(object sender, EventArgs e) => SetTimestamp();

    private void SetTimestamp() => timestampFarm = Timestamps.Now;

    private void DoUpdate(object sender, UpdateTickedEventArgs e)
    {
      client.Invoke();
      if (e.IsMultipleOf(30))
        client.SetPresence(GetPresence());
    }

    private MenuPresence Conf => !Context.IsWorldReady ? Config.MenuPresence : Config.GamePresence;

    private RichPresence GetPresence()
    {
      var presence = new RichPresence
      {
        Details = api.FormatText(Conf.Details),
        State = api.FormatText(Conf.State)
      };
      var smallImageText = api.FormatText(Conf.SmallImageText);
      var assets = new Assets
      {
        LargeImageKey = "default_large",
        LargeImageText = api.FormatText(Conf.LargeImageText),
        SmallImageText = smallImageText,
      };
      if (Conf.ForceSmallImage || smallImageText.Length > 0)
        assets.SmallImageKey = "default_small";

      if (Context.IsWorldReady)
      {
        var conf = (GamePresence)Conf;
        if (conf.ShowSeason)
          assets.LargeImageKey = $"{Game1.currentSeason}_{FarmTypeKey()}";
        if (conf.ShowWeather)
          assets.SmallImageKey = "weather_" + WeatherKey();
        if (conf.ShowPlayTime)
          presence.Timestamps = timestampFarm;
        if (Context.IsMultiplayer)
          try
          {
            presence.Party = new Party
            {
              ID = Game1.MasterPlayer.UniqueMultiplayerID.ToString(),
              Size = Game1.numberOfPlayers(),
              Max = Game1.getFarm().getNumberBuildingsConstructed("Cabin") + 1
            };
          }
          catch { }
      }

      if (Config.ShowGlobalPlayTime)
        presence.Timestamps = timestampSession;
      if (Config.AddGetModButton)
        presence.Buttons = new Button[]
        {
          new() { Label = I18n.GetModButton(), Url = ModURL }
        };

      presence.Assets = assets;
      return presence;
    }

    private string FarmTypeKey()
    {
      if (!Config.GamePresence.ShowFarmType)
        return "default";
      return Game1.whichFarm switch
      {
        Farm.default_layout => "standard",
        Farm.riverlands_layout => "riverland",
        Farm.forest_layout => "forest",
        Farm.mountains_layout => "hilltop",
        Farm.combat_layout => "wilderness",
        _ => "default",
      };
    }

    private static string WeatherKey()
    {
      if (Game1.isRaining)
        return Game1.isLightning ? "stormy" : "rainy";
      if (Game1.isDebrisWeather)
        return "windy_" + Game1.currentSeason;
      if (Game1.isSnowing)
        return "snowy";
      if (Game1.weddingToday)
        return "wedding";
      if (Game1.isFestival())
        return "festival";
      return "sunny";
    }
  }
}
