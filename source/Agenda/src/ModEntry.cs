/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ofts-cqm/StardewValley-Agenda
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using GenericModConfigMenu;
using System;
using StardewValley.GameData.Machines;

namespace MyAgenda
{
    internal sealed class ModEntry : Mod
    {
        ModConfig Config;
        //NamingMenu n;
        //DialogueBox b;
        //Billboard b;

        public override void Entry(IModHelper helper)
        {
            // 加载config
            Config = this.Helper.ReadConfig<ModConfig>();
            if (Config == null)
            {
                Config = new ModConfig();
                Monitor.Log("Config is missing, generating new one");
            }

            // 监听事件
            Helper.Events.Input.ButtonPressed += this.OnButtonPressed;
            Helper.Events.Display.MenuChanged += this.OnUIUpdated;
            Helper.Events.Display.WindowResized += this.onWindowResized;
            Helper.Events.GameLoop.Saving += this.onSaveSaved;
            Helper.Events.GameLoop.SaveLoaded += this.onSaveLoaded;
            Helper.Events.GameLoop.DayStarted += this.dailyCheck;
            Helper.Events.GameLoop.DayEnding += this.dayEnd;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.Content.LocaleChanged += Trigger.reloadTriggerOptions;
            Helper.ConsoleCommands.Add("agenda", "check the items on agenda at the specified date\nUsage: agenda [season(0-3)] [date(0-27)]", query);

            //初始化monitor和helper
            Agenda.monitor = Monitor;
            AgendaPage.monitor = Monitor;
            Trigger.helper = helper;
            Trigger.monitor = Monitor;
        }
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            // 让我康康玩家是不是要打开Agenda
            if (this.Config.AgendaKey.JustPressed() && Context.IsWorldReady && Game1.activeClickableMenu == null)
            {
                Game1.activeClickableMenu =Agenda.Instance;
            }
        }

        private void OnUIUpdated(object sender, MenuChangedEventArgs e) 
        {
            // 为了防止任何意外发生，保存环节在UI关闭时强制触发
            if(e.OldMenu != null && e.OldMenu is AgendaPage page)
            {
                 Agenda.save(page.season, page.day - 1);
            }

            // 如果当前菜单是日历并且已经说了把日历替换成Agenda
            // （为什么dailyQuestBoard不是public的啊！！！！！！！！！）
            if(e.NewMenu is Billboard board && !Helper.Reflection.GetField<bool>(board, "dailyQuestBoard").GetValue() && Config.Replace_Calender_With_Agenda)
            {
                // 你干什么谁让你出来了！！！滚回去！归！
                board.exitThisMenu();
                Game1.activeClickableMenu = Agenda.Instance;
            }
        }

        private void onWindowResized(object sender, WindowResizedEventArgs e)
        {
            // 如果游戏载入了并且窗口大小变了，更新一下
            if (!Context.IsWorldReady) return;
            Agenda.Instance.gameWindowSizeChanged(new Rectangle(0, 0, e.OldSize.X, e.OldSize.Y), new Rectangle(0, 0, e.NewSize.X, e.NewSize.Y));
            Agenda.agendaPage.resize();
            Trigger.Instance.resize();
        }
        
        private void onSaveSaved(object sender, SavingEventArgs e)
        {
            // 让Agenda保存一下数据，再手动保存一下昨天的运气
            Agenda.write(Helper);
            Helper.Data.WriteSaveData("previous_luck", $"{Util.previousLuckLevel}");
            Helper.Data.WriteSaveData("islandRained", $"{Util.IslandRained}");
            Helper.Data.WriteSaveData("mainlandRained", $"{Util.MainlandRained}");
        }

        private void onSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            // 加载游戏后，初始化所有Instance
            Agenda.Instance = new Agenda(Helper);
            Agenda.agendaPage = new AgendaPage(Helper);
            Trigger.Instance = new Trigger();

            // 读昨天的运气和天气
            string tmp = Helper.Data.ReadSaveData<string>("previous_luck");
            if (tmp != null) double.TryParse(tmp, out Util.previousLuckLevel);

            tmp = Helper.Data.ReadSaveData<string>("islandRained");
            if (tmp != null) bool.TryParse(tmp, out Util.IslandRained);

            tmp = Helper.Data.ReadSaveData<string>("mainlandRained");
            if (tmp != null) bool.TryParse(tmp, out Util.MainlandRained);
        }

        private void dailyCheck(object sender, DayStartedEventArgs e)
        {
            // 检查当天的Agenda
            int season = Utility.getSeasonNumber(Game1.currentSeason);
            int day = Game1.dayOfMonth - 1;
            if (Agenda.hasSomethingToDo(season, day))
            {
                Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("pop_up"), 2));
                Game1.addHUDMessage(new HUDMessage(getDisplayTitle(season, day), 2));
            }
            // 遍历所有Trigger
            for(int i = 0; i < 14; i++)
            {
                // 检查一下
                byte result = Util.examinDate(Agenda.triggerValue[i]);
                // 有东西！
                if ((result & 0xF0) != 0)
                {
                    // 骚扰一下玩家
                    Game1.addHUDMessage(new HUDMessage(Helper.Translation.Get("triggered"), 2));
                    Game1.addHUDMessage(new HUDMessage(Agenda.triggerTitle[i], 2));
                }
            }
        }

        private void dayEnd(object sender, DayEndingEventArgs e)
        {
            // 如果自动删除，删除一下
            if (Config.Auto_Delete_After_Complete)
            {
                int season = Utility.getSeasonNumber(Game1.currentSeason);
                int day = Game1.dayOfMonth - 1;
                if (season > 3 || season < 0 || day > 27 || day < 0) return;
                Agenda.pageNote[season, day] = "";
                Agenda.pageTitle[season, day] = "";

                // 再遍历所有Trigger
                for (int i = 0; i < 14; i++)
                {
                    // 检查一下
                    byte result = Util.examinDate(Agenda.triggerValue[i]);
                    // 有东西！
                    if (result != 0)
                    {
                        // 该删除了，全部删除
                        if ((result & 0x0F) == 0x0F)
                        {
                            Agenda.triggerValue[i] = new int[3];
                            Agenda.triggerTitle[i] = "";
                            Agenda.triggerNote[i] = "";
                        }

                        if ((result & 0xF0) == 0xF0 && Agenda.triggerValue[i][1] > 0 && Agenda.triggerValue[i][1] < 11)
                        {
                            Agenda.triggerValue[i][1]--;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i < 14; i++)
                {
                    byte result = Util.examinDate(Agenda.triggerValue[i]);

                    if ((result & 0xF0) == 0xF0 && Agenda.triggerValue[i][1] > 0 && Agenda.triggerValue[i][1] < 11)
                    {
                        Agenda.triggerValue[i][1]--;
                    }
                }
            }

            // 一天结束了，把昨天的东西设置成今天的
            Util.previousLuckLevel = Game1.player.DailyLuck;
            Util.IslandRained = Util.isRainHere(LocationContexts.IslandId);
            Util.MainlandRained = Util.isRainHere(LocationContexts.DefaultId);
        }

        // 星露谷，启动！
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // 加载一下mod config menu
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is null)
                return;

            configMenu.Register(
                mod: this.ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => this.Helper.WriteConfig(this.Config)
            );

            configMenu.AddKeybindList(
                mod : this.ModManifest,
                name: () => Helper.Translation.Get("keyBind"),
                tooltip : () => Helper.Translation.Get("keyBind_tip"),
                getValue: () => this.Config.AgendaKey,
                setValue: value => this.Config.AgendaKey = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("replace"),
                tooltip: () => Helper.Translation.Get("replace_tip"),
                getValue: () => this.Config.Replace_Calender_With_Agenda,
                setValue: value => this.Config.Replace_Calender_With_Agenda = value
            );

            configMenu.AddBoolOption(
                mod: this.ModManifest,
                name: () => Helper.Translation.Get("autoDelete"),
                tooltip: () => Helper.Translation.Get("autoDelete_tip"),
                getValue: () => this.Config.Auto_Delete_After_Complete,
                setValue: value => this.Config.Auto_Delete_After_Complete = value
            );
        }

        // 想起了我学sql的恐惧
        private void query(string commend, string[] args)
        {   
            // 还没加载呢急个jb
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Save not Loaded Yet!", LogLevel.Error);
            }

            // 虽然最开始是用来查询到但是后面就有debug的用处了因为我懒得搞一个新的指令
            if (args.Length == 1 && args[0] == "open")
            {
                Trigger.title = "";
                Trigger.note = "";
                Game1.activeClickableMenu = Trigger.Instance;
                return;
            }

            if(args.Length == 1 && args[0] == "yesterday")
            {
                Monitor.Log($"yesterday's luck: {Util.previousLuckLevel}, yesterday rained: {Util.MainlandRained} in mainland, {Util.IslandRained} in island", LogLevel.Info);
                return;
            }

            // 检查当前trigger，debug用
            if(args.Length == 2 && args[0] == "parse" && args[1] == "cur")
            {
                int[] trigger = Trigger.selectedTrigger;
                Monitor.Log($"parsing trigger time = {Trigger.choices[0][trigger[0]]}, frequency = {Trigger.choices[1][trigger[1]]}, condition = {Trigger.choices[2][trigger[2]]}", LogLevel.Info);
                byte result = Util.examinDate(trigger);
                Monitor.Log($"result is {result}: trigger valid = {result >> 7}, should_delete = {(result & 0x40) >> 6}, today = {(result & 0x20) >> 5}", LogLevel.Info);
                return;
            }

            if (args.Length == 2 && args[0] == "parse" && args[1] == "index")
            {
                int[] trigger = Agenda.triggerValue[int.Parse(args[1])];
                Monitor.Log($"parsing trigger time = {Trigger.choices[0][trigger[0]]}, frequency = {Trigger.choices[1][trigger[1]]}, condition = {Trigger.choices[2][trigger[2]]}", LogLevel.Info);
                byte result = Util.examinDate(trigger);
                Monitor.Log($"result is {result}", LogLevel.Info);
                return;
            }

            if (args.Length == 2 && args[0] == "trigger")
            {
                int[] trigger = Agenda.triggerValue[int.Parse(args[1])];
                Monitor.Log($"trigger is {trigger[0]}, {trigger[1]}, {trigger[2]}", LogLevel.Info);
                return;
            }

            //尝试获取当天Agenda
            int season, day;
            try
            {
                season = int.Parse(args[0]);
                day = int.Parse(args[1]);
                Monitor.Log($"retrieving item on season {Utility.getSeasonNameFromNumber(season)}, day {day + 1}\ntitle: \n{Agenda.pageTitle[season, day]}\nBirthday: {Agenda.pageBirthday[season, day]}, Festival: {Agenda.pageFestival[season, day]}\nNotes: \n{Agenda.pageNote[season, day]}", LogLevel.Info);
            }catch (System.Exception)
            {
                Monitor.Log("INCOMPLETE COMMEND!", LogLevel.Error);
            }
        }

        public static string getDisplayTitle(int season, int day)
        {
            if (Agenda.pageTitle[season, day] != "")
            {
                return Agenda.pageTitle[season, day];
            }

            if (Agenda.titleSubsitute[season, day] != "")
            {
                return Agenda.titleSubsitute[season, day];
            }

            return Agenda.pageNote[season, day].Substring(0, Math.Min(Agenda.pageNote[season, day].Length, 20));
        }
    }
    
    public sealed class ModConfig
    {
        public KeybindList AgendaKey { get; set; } = KeybindList.Parse("G");
        public bool Replace_Calender_With_Agenda { get; set; } = false;
        public bool Auto_Delete_After_Complete { get; set; } = true;
    }
}