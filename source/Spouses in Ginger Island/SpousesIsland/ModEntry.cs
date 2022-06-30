/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/SpousesIsland
**
*************************************************/

using System;
using System.Collections.Generic;
using GenericModConfigMenu;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Characters;
using SpousesIsland.Framework;

namespace SpousesIsland
{
    class ModConfig
    {
        public int CustomChance { get; set; } = 10;
        public bool ScheduleRandom { get; set; } = false;
        public bool CustomRoom { get; set; } = false;
        public string Childbedcolor { get; set; } = "1";
        public string ChildbedType { get; set; } = "mod";
        public bool NPCDevan { get; set; } = false;
        public bool Allow_Children { get; set; } = true;
        public bool Allow_Abigail { get; set; } = true;
        public bool Allow_Alex { get; set; } = true;
        public bool Allow_Elliott { get; set; } = true;
        public bool Allow_Emily { get; set; } = true;
        public bool Allow_Haley { get; set; } = true;
        public bool Allow_Harvey { get; set; } = true;
        public bool Allow_Krobus { get; set; } = true;
        public bool Allow_Leah { get; set; } = true;
        public bool Allow_Maru { get; set; } = true;
        public bool Allow_Penny { get; set; } = true;
        public bool Allow_Sam { get; set; } = true;
        public bool Allow_Sebastian { get; set; } = true;
        public bool Allow_Shane { get; set; } = true;
        public bool Allow_Claire { get; set; } = true;
        public bool Allow_Lance { get; set; } = true;
        public bool Allow_Magnus { get; set; } = true;
        public bool Allow_Olivia { get; set; } = true;
        public bool Allow_Sophia { get; set; } = true;
        public bool Allow_Victor { get; set; } = true;
        //debug
        public bool Verbose { get; set; } = false;
        public bool Debug { get; set; } = false;
        public bool CheckDaily { get; set; } = false;
    }
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.SaveLoaded += this.OnSaveLoaded;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.ReturnedToTitle += this.TitleReturn;

            this.Config = this.Helper.ReadConfig<ModConfig>();
            ModMonitor = this.Monitor;
            ModHelper = this.Helper;

            //??
            int RandomizedInt = this.RandomizedInt;
            //commands
            if (Config.Debug is true)
            {
                helper.ConsoleCommands.Add("sgi_help", helper.Translation.Get("CLI.help"), this.SGI_Help);
                helper.ConsoleCommands.Add("sgi_chance", helper.Translation.Get("CLI.chance"), this.SGI_Chance);
                helper.ConsoleCommands.Add("sgi_reset", helper.Translation.Get("CLI.reset"), this.SGI_Reset);
                helper.ConsoleCommands.Add("sgi_list", helper.Translation.Get("CLI.list"), this.SGI_List);
                helper.ConsoleCommands.Add("sgi_about", helper.Translation.Get("CLI.about"), this.SGI_About);
            }
        }
        /* Private
         * Can only be accessed by this mod.
         * Contents: Events called by Entry, player config, static Random.
         */
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            HasSVE = Commands.HasMod("FlashShifter.StardewValleyExpandedCP", this.Helper);
            HasC2N = Commands.HasMod("Loe2run.ChildToNPC", this.Helper);
            HasExGIM = Commands.HasMod("mistyspring.extraGImaps", this.Helper);
            HasSeasonal = Commands.HasMod("Poltergeister.SeasonalCuteCharacters", this.Helper);

            if (Config.Verbose == true)
            {
                this.Monitor.Log($"\n   HasSVE = {HasSVE}\n   HasC2N = {HasC2N}\n   HasExGIM = {HasExGIM}\n   HasSeasonal = {HasSeasonal}");
            }

            RandomizedInt = Random.Next(1, 101);

            ClearValues(this);

            // get Generic Mod Config Menu's API (if it's installed)
            var configMenu = this.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (configMenu is not null)
            {
                // register mod
                configMenu.Register(
                    mod: this.ModManifest,
                    reset: () => this.Config = new ModConfig(),
                    save: () => this.Helper.WriteConfig(this.Config)
                );

                // basic config options
                configMenu.AddNumberOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.CustomChance.name"),
                    tooltip: () => this.Helper.Translation.Get("config.CustomChance.description"),
                    getValue: () => this.Config.CustomChance,
                    setValue: value => this.Config.CustomChance = value,
                    min: 0,
                    max: 100,
                    interval: 1
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.CustomRoom.name"),
                    tooltip: () => this.Helper.Translation.Get("config.CustomRoom.description"),
                    getValue: () => this.Config.CustomRoom,
                    setValue: value => this.Config.CustomRoom = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.Devan_Nosit.name"),
                    tooltip: () => this.Helper.Translation.Get("config.Devan_Nosit.description"),
                    getValue: () => this.Config.NPCDevan,
                    setValue: value => this.Config.NPCDevan = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.ScheduleRandom.name"),
                    tooltip: () => this.Helper.Translation.Get("config.ScheduleRandom.description"),
                    getValue: () => this.Config.ScheduleRandom,
                    setValue: value => this.Config.ScheduleRandom = value
                );
                //links to config pages
                configMenu.AddPageLink(
                    mod: this.ModManifest,
                    pageId: "advancedConfig",
                    text: () => this.Helper.Translation.Get("config.advancedConfig.name"),
                    tooltip: () => this.Helper.Translation.Get("config.advancedConfig.description")
                );
                if (HasC2N is true)
                {
                    configMenu.AddPageLink(
                        mod: this.ModManifest,
                        pageId: "C2Nconfig",
                        text: () => "Child2NPC...",
                        tooltip: () => this.Helper.Translation.Get("config.Child2NPC.description")
                    );
                    configMenu.AddPage(
                        mod: this.ModManifest,
                        pageId: "C2Nconfig",
                        pageTitle: () => "Child2NPC..."
                    );
                    configMenu.AddBoolOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.ChildVisitIsland.name"),
                        tooltip: () => this.Helper.Translation.Get("config.ChildVisitIsland.description"),
                        getValue: () => this.Config.Allow_Children,
                        setValue: value => this.Config.Allow_Children = value
                    );
                    configMenu.AddTextOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.Childbedtype.name"),
                        tooltip: () => this.Helper.Translation.Get("config.Childbedtype.description"),
                        getValue: () => this.Config.ChildbedType,
                        setValue: value => this.Config.ChildbedType = value,
                        allowedValues: new string[] { "sdv", "mod" }
                        );
                    if (Config.ChildbedType is "mod")
                    {
                        configMenu.AddTextOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.Childbedcolor.name"),
                        tooltip: () => this.Helper.Translation.Get("config.Childbedcolor.description"),
                        getValue: () => this.Config.Childbedcolor,
                        setValue: value => this.Config.Childbedcolor = value,
                        allowedValues: new string[] { "1", "2", "3", "4", "5", "6" }
                    );
                        configMenu.AddImage(
                        mod: this.ModManifest,
                        texture: KbcSamples,
                        texturePixelArea: null,
                        scale: 1
                    );
                    }
                }
                //adv. config
                configMenu.AddPage(
                    mod: this.ModManifest,
                    pageId: "advancedConfig",
                    pageTitle: () => this.Helper.Translation.Get("config.advancedConfig.name")
                );
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: Titles.SpouseT,
                    tooltip: SpouseD
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Abigail",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Abigail,
                    setValue: value => this.Config.Allow_Abigail = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Alex",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Alex,
                    setValue: value => this.Config.Allow_Alex = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Elliott",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Elliott,
                    setValue: value => this.Config.Allow_Elliott = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Emily",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Emily,
                    setValue: value => this.Config.Allow_Emily = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Haley",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Haley,
                    setValue: value => this.Config.Allow_Haley = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Harvey",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Harvey,
                    setValue: value => this.Config.Allow_Harvey = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Krobus",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Krobus,
                    setValue: value => this.Config.Allow_Krobus = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Leah",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Leah,
                    setValue: value => this.Config.Allow_Leah = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Maru",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Maru,
                    setValue: value => this.Config.Allow_Maru = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Penny",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Penny,
                    setValue: value => this.Config.Allow_Penny = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Sam",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Sam,
                    setValue: value => this.Config.Allow_Sam = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Sebastian",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Sebastian,
                    setValue: value => this.Config.Allow_Sebastian = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Shane",
                    tooltip: () => null,
                    getValue: () => this.Config.Allow_Shane,
                    setValue: value => this.Config.Allow_Shane = value
                );
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: Titles.SVET,
                    tooltip: null
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Claire",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Claire,
                    setValue: value => this.Config.Allow_Claire = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Lance",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Lance,
                    setValue: value => this.Config.Allow_Lance = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Magnus",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Magnus,
                    setValue: value => this.Config.Allow_Magnus = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Olivia",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Olivia,
                    setValue: value => this.Config.Allow_Olivia = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Sophia",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Sophia,
                    setValue: value => this.Config.Allow_Sophia = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => "Victor",
                    tooltip: () => this.Helper.Translation.Get("config.RequiresSVE"),
                    getValue: () => this.Config.Allow_Victor,
                    setValue: value => this.Config.Allow_Victor = value
                );
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: Titles.Debug,
                    tooltip: null
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.DebugComm.name"),
                    tooltip: () => this.Helper.Translation.Get("config.DebugComm.description"),
                    getValue: () => this.Config.Debug,
                    setValue: value => this.Config.Debug = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.Verbose.name"),
                    tooltip: () => this.Helper.Translation.Get("config.Verbose.description"),
                    getValue: () => this.Config.Verbose,
                    setValue: value => this.Config.Verbose = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.CheckDaily.name"),
                    tooltip: () => this.Helper.Translation.Get("config.CheckDaily.description"),
                    getValue: () => this.Config.CheckDaily,
                    setValue: value => this.Config.CheckDaily = value
                );
            }

            ContentPackData data = new ContentPackData();
            //this.Helper.Data.WriteJsonFile("ContentTemplate.json", data);
            foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Reading content pack: {contentPack.Manifest.Name} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Debug);
                if (!contentPack.HasFile("content.json"))
                {
                    // show 'required file missing' error
                    this.Monitor.Log(Helper.Translation.Get($"FW.contentpack.error"), LogLevel.Warn);
                }
                ContentPackObject obj = contentPack.ReadJsonFile<ContentPackObject>("content.json");
                foreach (var cpd in obj.data)
                {
                    if (SGIValues.CheckSpouseName(cpd.Spousename))
                    {
                        this.Monitor.Log($"{contentPack.Manifest.Name} is trying to add a schedule for {cpd.Spousename}. To avoid any conflicts, please untick '{cpd.Spousename}' from Advanced config.", LogLevel.Warn);
                    }
                    //error logs
                    if (Commands.ParseContentPack(cpd, this.Monitor))
                    {
                        this.Monitor.Log(string.Format(Helper.Translation.Get($"FW.contentpack.oneOrMoreErrors"), $"{contentPack.Manifest.Name}"), LogLevel.Error);
                    }
                    else
                    {
                        CustomSchedule.Add(cpd.Spousename, cpd);
                        this.Monitor.Log($"Added {cpd.Spousename} schedule", LogLevel.Debug);
                    }
                }
            }

        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            /*Format:
             * ("Word", if partial OK (e.g Word1), if subfolder OK (e.g Word/Sub/file)
             * 
             * NPCDevan is outside bc their stuff doesnt all load otherwise
             */
            if (e.Name.StartsWith("Maps/", false, true))
            {
                AssetRequest.Maps(e);
                AssetRequest.IslandMaps(this, e, Config);
            }
            
            if (Config.NPCDevan == true)
            {
                this.Monitor.LogOnce("Adding Devan", LogLevel.Trace);

                if (e.Name.IsEquivalentTo("Portraits/Devan"))
                {
                    if(HasSeasonal is true) //maybe change isworldready by checking season string?
                    {
                        e.LoadFromModFile<Texture2D>($"assets/Devan/Portrait_{Game1.currentSeason}.png", AssetLoadPriority.Medium);
                    }
                    else
                    {
                        e.LoadFromModFile<Texture2D>("assets/Devan/Portrait.png", AssetLoadPriority.Medium);
                    }
                    
                }
                if (e.Name.IsEquivalentTo("Maps/Saloon"))
                {
                    if (HasSVE is false)
                        e.Edit(asset => Devan.VanillaSaloon(asset));
                    else
                        e.Edit(asset => Devan.SVESaloon(asset));
                    if (SawDevan4H == true)
                        e.Edit(asset => Devan.PictureInRoom(asset));
                }
                if (e.Name.StartsWith("Characters", false, true))
                {
                    if (e.NameWithoutLocale.IsEquivalentTo("Characters/Devan"))
                    {
                        if(!Context.IsWorldReady || HasSeasonal is false)
                        {
                            e.LoadFromModFile<Texture2D>("assets/Devan/Character.png", AssetLoadPriority.Medium);
                        }
                        else
                        {
                            e.LoadFromModFile<Texture2D>($"assets/Devan/Character_{Game1.currentSeason}.png", AssetLoadPriority.Medium);
                        }
                    };
                    if (e.Name.IsEquivalentTo("Characters/schedules/Devan"))
                    {
                        if (Children is not null)
                        {
                            if (Config.CustomChance >= RandomizedInt)
                            {
                                e.LoadFromModFile<Dictionary<string, string>>("assets/Devan/Schedule_Babysit.json", AssetLoadPriority.Medium);
                            }
                            else
                            {
                                e.LoadFromModFile<Dictionary<string, string>>("assets/Devan/Schedule_Normal.json", AssetLoadPriority.Medium);
                            }
                        }
                        else
                        {
                            e.LoadFromModFile<Dictionary<string, string>>("assets/Devan/Schedule_Normal.json", AssetLoadPriority.Medium);
                        }
                        if (CCC == true)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Wed"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 CommunityCenter 26 17 0 \"Characters\\Dialogue\\Devan:CommunityCenter1\"/1600 CommunityCenter 16 20 0/1630 CommunityCenter 11 27 2 Devan_sit \"Characters\\Dialogue\\Devan:CommunityCenter2\"/1700 Town 26 21 2/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 Saloon 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                        if (IsLeahMarried is true && IsElliottMarried is false)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Fri"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 Woods 8 9 0 \"Characters\\Dialogue\\Devan:statue\"/1600 ElliottHouse 5 8 1/1800 ElliottHouse 8 4 Devan_sit/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                        else if (IsLeahMarried is false && IsElliottMarried is true)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Fri"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 LeahHouse 6 7 0 \"Characters\\Dialogue\\Devan:leahHouse\"/1500 LeahHouse 13 4 2 Devan_sit \"Characters\\Dialogue\\Devan:leahHouse_2\"/1600 Woods 12 6 2 Devan_sit \"Characters\\Dialogue\\Devan:secretforest\"/1800 Woods 10 17 2/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                        else if (IsLeahMarried is true && IsElliottMarried is true)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Fri"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 Woods 8 9 0 \"Characters\\Dialogue\\Devan:statue\"/1800 Woods 12 6 2 Devan_sit \"Characters\\Dialogue\\Devan:secretforest\"/1900 Woods 10 17 2/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                        if (HasSVE is true)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Sat"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 Forest 43 10 2 \"Characters\\Dialogue\\Devan:forest\"/1530 Forest 45 41 0/a1840 Forest 102 64 0 \"Characters\\Dialogue\\Devan:forest_2\"/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                    }
                    //if spanish load spanish file, if not it'll continue
                    if (e.Name.IsEquivalentTo("Characters/Dialogue/Devan.es-ES"))
                    {
                        e.LoadFromModFile<Dictionary<string, string>>("assets/Devan/Dialogue.es-ES.json", AssetLoadPriority.Medium);
                    }
                    if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Devan"))
                    {
                        e.LoadFromModFile<Dictionary<string, string>>("assets/Devan/Dialogue.json", AssetLoadPriority.Medium);
                    }
                }
                if (e.Name.StartsWith("Data/", false, true))
                {
                    if (e.Name.StartsWith("Data/Festivals/", false, false))
                    {
                        Devan.AppendFestivalData(e);
                        Devan.FesInternational(e);
                    }
                    if (e.Name.StartsWith("Data/", false, false))
                    {
                        Devan.MainData(e);
                    }
                    if (e.Name.StartsWith("Data/Events/", false, false))
                    {
                        Devan.EventsInternational(e);
                    }
                }
            }

            //if hasnt unlocked island = dont do visit
            if(!Game1.MasterPlayer.mailReceived.Contains("willyBoatFixed"))
            {
                return;
            }

            if (Config.CustomChance >= RandomizedInt)
            {
                if (e.Name.StartsWith("Characters/schedules/", false, true))
                {
                    if (HasC2N is true && Config.Allow_Children == true)
                    {
                        this.Monitor.LogOnce("Child To NPC is in the mod folder. Adding compatibility...", LogLevel.Trace);
                        if (Children is not null && Config.ChildbedType is "mod")
                        {
                            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/" + Children?[0].Name))
                            {
                                e.Edit(asset => ChildrenData.ChildMOD(InfoChild1, asset));
                            }
                            if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/" + Children?[1].Name))
                            {
                                e.Edit(asset => ChildrenData.ChildMOD(InfoChild2, asset));
                            }

                        }
                        if (Children is not null && Config.ChildbedType is "sdv")
                        {
                            if (SGIValues.HasAnyKidBeds() == true)
                            {
                                if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/" + Children?[0].Name))
                                {
                                    e.Edit(asset => ChildrenData.ChildSDV(InfoChild1, asset));
                                }
                                if (e.NameWithoutLocale.IsEquivalentTo("Characters/schedules/" + Children?[1].Name))
                                {
                                    e.Edit(asset => ChildrenData.ChildSDV(InfoChild2, asset));
                                }
                            }
                            else
                            {
                                this.Monitor.Log("There's no children beds in farmhouse! The kids won't visit.", LogLevel.Warn);
                            }
                        }

                    }
                    AssetRequest.ChangeSchedulesIntegrated(this, e, Config);
                }
                foreach (ContentPackData cpd in CustomSchedule.Values)
                {
                    AssetRequest.ContentPackSchedule(this, e, cpd);
                }
            }

            if (e.Name.StartsWith("Characters/", false, true))
            {
                AssetRequest.CharacterSheetsByConfig(this, e, Config);

                if (e.Name.StartsWith("Characters/Dialogue/", false, true))
                {
                    if (e.Name.StartsWith("Characters/Dialogue/Marr", true, false))
                    {
                        if (e.Name.IsEquivalentTo("Characters/Dialogue/MarriageDialogueKrobus.es-ES"))
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data.Add("funLeave_Krobus", "Intentaré ir fuera hoy...si voy temprano, tu gente no se dará cuenta$0.#$b#Pasar tiempo contigo me ha hecho ganar interés por las actividades de tu gente.$1");
                            });
                        if (e.Name.IsEquivalentTo("Characters/Dialogue/MarriageDialogueKrobus"))
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data.Add("funLeave_Krobus", "I'll go outside today...if i'm quick, your people won't notice.$0#$b#Thanks to you, i've become curious of humans' \"Entertainment activities\".$1");
                            });
                    }

                    SGIData.DialoguesInternational(e, Config);
                }

                foreach (ContentPackData cpd in CustomSchedule.Values)
                {
                    if (e.NameWithoutLocale.IsEquivalentTo($"Characters/Dialogue/{cpd.Spousename}"))
                    {
                        AssetRequest.ContentPackDialogue(this, e, cpd);
                    }
                }
            }
            if (e.Name.StartsWith("Portraits/", false, true))
            {
                if (e.Name.IsEquivalentTo("Portraits/Krobus") && Config.Allow_Krobus == true && Config.CustomChance >= RandomizedInt)
                {
                    e.LoadFromModFile<Texture2D>("assets/Spouses/Krobus_Outside_Portrait.png", AssetLoadPriority.Medium);
                }
            }
        }
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            //get values used by mod
            NPC kr = Game1.getCharacterFromName("Krobus", false, false);
            NPC lh = Game1.getCharacterFromName("Leah", false, false);
            NPC el = Game1.getCharacterFromName("Elliott", false, false);

            if(kr.isMarriedOrEngaged() || kr.isRoommate())
            {
                IsKrobusRoommate = true;
            }
            else
            {
                IsKrobusRoommate = false;
            }
            IsLeahMarried = lh.isMarriedOrEngaged();
            IsElliottMarried = el.isMarriedOrEngaged();

            Children = Game1.MasterPlayer.getChildren();
            CCC = Game1.MasterPlayer.hasCompletedCommunityCenter();
            SawDevan4H = Game1.MasterPlayer.eventsSeen.Contains(110371000);

            if (Config.Verbose == true)
            {
                this.Monitor.Log($"\nChildren (count) = {Children};\nIsKrobusRoommate = {IsKrobusRoommate};\nIsLeahMarried = {IsLeahMarried};\nIsElliottMarried = {IsElliottMarried};\nCCC = {CCC};\nSawDevan4H = {SawDevan4H};");
            }
        }
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            //get new chance
            PreviousDayRandom = RandomizedInt;
            RandomizedInt = Random.Next(1, 101);

            //re-check values
            Children = Game1.MasterPlayer.getChildren();
            CCC = Game1.MasterPlayer.hasCompletedCommunityCenter();
            SawDevan4H = Game1.MasterPlayer.eventsSeen.Contains(110371000);
        }
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (e.NewTime >= 2200 && Config.CustomChance >= RandomizedInt)
            {
                var sgv = new SGIValues();
                var ifh = Game1.getLocationFromName("IslandFarmHouse");
                foreach (NPC c in ifh.characters)
                {
                    if(c.isMarried())
                    {
                        if (Config.Verbose == true)
                        {
                            this.Monitor.Log($"Pathing {c.Name} to bed in {ifh.Name}...");
                        }
                        try
                        {
                            sgv.MakeSpouseGoToBed(c, ifh);
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                        }
                    }
                    
                    else if(!c.isMarried() && HasC2N == true)
                    {
                        if (Config.Verbose == true)
                        {
                            this.Monitor.Log($"Pathing {c.Name} to kid bed in {ifh.Name}...");
                        }
                        try
                        {
                            sgv.MakeKidGoToBed(c, ifh);
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                        }
                    }
                }
            }
        }
        private void TitleReturn(object sender, ReturnedToTitleEventArgs e)
        {
            ClearValues(this);
        }

        private void ClearValues(ModEntry e)
        {
            //empty lists preemptively
            if (e.SchedulesEdited is not null)
            {
                e.Monitor.Log("Resetting schedule list...", LogLevel.Trace);
                e.SchedulesEdited.Clear();
            }
            if (e.DialoguesEdited is not null)
            {
                e.Monitor.Log("Resetting dialogue list...", LogLevel.Trace);
                e.DialoguesEdited.Clear();
            }
            if (e.IsLeahMarried is not false)
            {
                e.IsLeahMarried = false;
            }
            if (e.IsElliottMarried is not false)
            {
                e.IsElliottMarried = false;
            }
            if (e.IsKrobusRoommate is not false)
            {
                e.IsKrobusRoommate = false;
            }
            if (e.SawDevan4H is not false)
            {
                e.SawDevan4H = false;
            }
            if (e.CCC is not false)
            {
                e.CCC = false;
            }
            if (e.RandomizedInt is not 0)
            {
                e.RandomizedInt = 0;
            }
        }

        private ModConfig Config;
        private static Random random;

        /*   Internal (can only be accessed by current .cs) */
        internal void SGI_About(string command, string[] args)
        {
            if (LocalizedContentManager.CurrentLanguageCode.ToString() is "es")
            {
                this.Monitor.Log("Este mod permite que tu pareja vaya a la isla (compatible con ChildToNPC, SVE y otros). También permite crear paquetes de contenido / agregar rutinas personalizadas.\nMod creado por mistyspring (nexusmods)", LogLevel.Info);
            }
            else
            {
                this.Monitor.Log("This mod allows your spouse to visit the Island (compatible with ChildToNPC, SVE, Free Love and a few others). It's also a framework, so you can add custom schedules.\nMod created by mistyspring (nexusmods)", LogLevel.Info);
            }
        }
        internal void SGI_List(string command, string[] args)
        {
            Debugging.List(this, args, Config);
        }
        internal void SGI_Chance(string command, string[] args)
        {
            Debugging.Chance(this, args, Config);
        }
        internal void SGI_Reset(string command, string[] args)
        {
            Debugging.Reset(this, args, Config);
        }
        internal void SGI_Help(string command, string[] args)
        {
            this.Monitor.Log(this.Helper.Translation.Get("CLI.helpdescription"), LogLevel.Info);
        }

        internal Texture2D KbcSamples() => Helper.ModContent.Load<Texture2D>("assets/kbcSamples.png");
        internal string SpouseD()
        {
            var SpousesDesc = this.Helper.Translation.Get("config.Vanillas.description");
            return SpousesDesc;
        }
        internal List<string> SchedulesEdited = new();
        internal List<string> DialoguesEdited = new();
        internal List<string> TranslationsAdded = new();
        internal List<Child> Children = new();

        internal int RandomizedInt { get; private set; }
        internal static Dictionary<string, ContentPackData> CustomSchedule { get; private set; } = new();
        internal static IModHelper ModHelper { get; private set; }
        internal static IMonitor ModMonitor { get; private set; }
        internal static Random Random
        {
            get
            {
                random ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed * 36));
                return random;
            }
        }
        //internal bool CanBeSpecific;
        internal bool IsLeahMarried { get; private set; }
        internal bool IsElliottMarried { get; private set; }
        internal bool IsKrobusRoommate { get; private set; }
        internal bool SawDevan4H { get; private set; }  = false;
        internal bool CCC { get; private set; }
        internal bool HasSVE;
        internal bool HasC2N;
        internal bool HasExGIM;
        internal bool HasSeasonal;
        internal int PreviousDayRandom { get; private set; }
        internal ChildSchedule InfoChild1 { get; private set; }
        internal ChildSchedule InfoChild2 { get; private set; }
    }
}