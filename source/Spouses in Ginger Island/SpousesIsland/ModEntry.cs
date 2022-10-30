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
using Microsoft.Xna.Framework.Graphics;
using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Enums;
using StardewValley;
using StardewValley.Characters;
using SpousesIsland.Framework;
using Microsoft.Xna.Framework;
using System.Linq;
using HarmonyLib;
using JsonAssets;

namespace SpousesIsland
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
            helper.Events.Content.AssetRequested += this.OnAssetRequested;
            helper.Events.GameLoop.DayEnding += this.OnDayEnding;
            helper.Events.GameLoop.TimeChanged += this.OnTimeChanged;
            helper.Events.GameLoop.ReturnedToTitle += this.TitleReturn;

            helper.Events.Multiplayer.PeerContextReceived += PeerContextReceived;
            helper.Events.Specialized.LoadStageChanged += LoadStageChanged;

            this.Config = this.Helper.ReadConfig<ModConfig>();

            Mon = this.Monitor;
            Help = this.Helper;
            TL = this.Helper.Translation;
            IsDebug = Config.Debug;

            //commands
            if (Config.Debug is true)
            {
                helper.ConsoleCommands.Add("islandchance", helper.Translation.Get("CLI.chance"), Debugging.Chance);
            }

            this.Monitor.Log($"Applying Harmony patch \"{nameof(Patches)}\": prefixing SDV method \"NPC.tryToReceiveActiveObject(Farmer who)\".");
            var harmony = new Harmony(this.ModManifest.UniqueID);
            harmony.Patch(
                original: AccessTools.Method(typeof(StardewValley.NPC), nameof(StardewValley.NPC.tryToReceiveActiveObject)),
                prefix: new HarmonyMethod(typeof(Patches), nameof(Patches.tryToReceiveTicket))
                );
        }

        //these add and/or depend on config
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ClearValues();

            //check for compatibility, log result
            HasSVE = Information.HasMod("FlashShifter.StardewValleyExpandedCP");
            HasC2N = Information.HasMod("Loe2run.ChildToNPC");
            HasExGIM = Information.HasMod("mistyspring.extraGImaps");
            notfurniture = Config.UseFurnitureBed == false;

            this.Monitor.Log($"\n   HasSVE = {HasSVE}\n   HasC2N = {HasC2N}\n   HasExGIM = {HasExGIM}");

            //choose random
            RandomizedInt = Random.Next(1, 101);
            IslandToday = Config.CustomChance >= RandomizedInt;

            /* get all content packs installed - deprecated
            GetContentPacks(); */

            // get CP's api and register token
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            api.RegisterToken(this.ModManifest, "CanVisitIsland", () =>
            {
                // is island day
                if (IslandToday)
                    return new string[] { "true" };

                else
                    return new string[] { "false" };
            });

            InfoChildren = ChildrenData.GetInformation(Config.ChildSchedules);

            jsonAssets = Helper.ModRegistry.GetApi<IApi>("spacechase0.JsonAssets");

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

                //devan
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.Devan_Nosit.name"),
                    tooltip: () => this.Helper.Translation.Get("config.Devan_Nosit.description"),
                    getValue: () => this.Config.NPCDevan,
                    setValue: value => this.Config.NPCDevan = value
                );
                configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.SeasonalDevan.name"),
                    tooltip: () => this.Helper.Translation.Get("config.SeasonalDevan.description"),
                    getValue: () => this.Config.SeasonalDevan,
                    setValue: value => this.Config.SeasonalDevan = value
                );

                //random place
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
                    configMenu.AddBoolOption(
                        mod: this.ModManifest,
                        name: () => this.Helper.Translation.Get("config.UseFurnitureBed.name"),
                        tooltip: () => this.Helper.Translation.Get("config.UseFurnitureBed.description"),
                        getValue: () => this.Config.UseFurnitureBed,
                        setValue: value => this.Config.UseFurnitureBed = value 
                    );
                    if (Config.UseFurnitureBed == false) //if it's not bed furniture: lets you decide the "mod bed" color.
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
                        texture: Integrated.KbcSamples,
                        texturePixelArea: null,
                        scale: 1
                    );
                    }
                    configMenu.AddBoolOption(
                    mod: this.ModManifest,
                    name: () => this.Helper.Translation.Get("config.UseModSchedule.name"),
                    tooltip: () => this.Helper.Translation.Get("config.UseModSchedule.description"),
                    getValue: () => this.Config.UseModSchedule,
                    setValue: value => this.Config.UseModSchedule = value
                );
                }
                
                //adv. config page
                configMenu.AddPage(
                    mod: this.ModManifest,
                    pageId: "advancedConfig",
                    pageTitle: () => this.Helper.Translation.Get("config.advancedConfig.name")
                );
                configMenu.AddSectionTitle(
                    mod: this.ModManifest,
                    text: Titles.SpouseT,
                    tooltip: Integrated.SpouseD
                );
                //all spouse bools below
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
                //debug options
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
            }
        }

        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            /* Format:
             * 1. Word
             * 2. if partial OK (e.g Word1)
             * 3. if subfolder OK (e.g Word/Sub/file)
             */
            if (e.Name.StartsWith("Maps/", false, true))
            {
                Integrated.Maps(e);
                Integrated.IslandMaps(e, Config);
            }
            
            if (Config.NPCDevan == true)
            {
                this.Monitor.LogOnce("Adding Devan", LogLevel.Trace);

                if (e.Name.IsEquivalentTo("Portraits/Devan"))
                {
                    if(Config.SeasonalDevan == true)
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
                        if(Config.SeasonalDevan == false)
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
                        var IsLeahMarried = MarriedAndAllowed.Contains("Leah");
                        var IsElliottMarried = MarriedAndAllowed.Contains("Elliott");
                        
                        if (Children is not null && IslandToday)
                        {
                            e.LoadFromModFile<Dictionary<string, string>>("assets/Devan/Schedule_Babysit.json", AssetLoadPriority.Medium);
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

                        if (IsLeahMarried == true && IsElliottMarried == false)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Fri"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 Woods 8 9 0 \"Characters\\Dialogue\\Devan:statue\"/1600 ElliottHouse 5 8 1/1800 ElliottHouse 8 4 Devan_sit/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                        else if (IsLeahMarried == false && IsElliottMarried == true)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Fri"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 LeahHouse 6 7 0 \"Characters\\Dialogue\\Devan:leahHouse\"/1500 LeahHouse 13 4 2 Devan_sit \"Characters\\Dialogue\\Devan:leahHouse_2\"/1600 Woods 12 6 2 Devan_sit \"Characters\\Dialogue\\Devan:secretforest\"/1800 Woods 10 17 2/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                        else if (IsLeahMarried == true && IsElliottMarried == true)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Fri"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 Woods 8 9 0 \"Characters\\Dialogue\\Devan:statue\"/1800 Woods 12 6 2 Devan_sit \"Characters\\Dialogue\\Devan:secretforest\"/1900 Woods 10 17 2/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }

                        if (HasSVE == true)
                        {
                            e.Edit(asset =>
                            {
                                IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                                data["Sat"] = "640 Saloon 39 7 2/650 Saloon 35 8 2/700 Saloon 14 17 2 Devan_broom/810 Saloon 13 22 2 Devan_broom/920 Saloon 32 19 2 Devan_broom/1020 Saloon 42 19 2 Devan_broom/1100 Saloon 33 8 2 Devan_broom/1200 Saloon 24 19 Devan_broom/1300 Forest 43 10 2 \"Characters\\Dialogue\\Devan:forest\"/1530 Forest 45 41 0/a1840 Forest 102 64 0 \"Characters\\Dialogue\\Devan:forest_2\"/a2140 Saloon 31 19 1/2150 Saloon 31 8 2/2200 39 7 0/2210 Saloon 44 5 3 devan_sleep";
                            });
                        }
                    }
                    
                    //will load a file using the locale code.
                    if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/Devan"))
                    {
                        e.LoadFromModFile<Dictionary<string, string>>($"assets/Devan/Dialogue{e.Name.LocaleCode}.json", AssetLoadPriority.Medium);
                    }
                }
                if (e.Name.StartsWith("Data/", false, true))
                {
                    if (e.Name.StartsWith("Data/Festivals/", false, false))
                    {
                        Devan.AppendFestivalData(e);
                        Devan.FesInternational(e);
                    }
                    else if (e.Name.StartsWith("Data/Events/", false, false))
                    {
                        Devan.EventsInternational(e);
                    }
                    else
                    {
                        Devan.MainData(e);
                    }
                }
            }

            /* if hasnt unlocked island:
             * returns / doesnt apply these patches
             */
            if (!BoatFixed || !IslandHouse)
            {
                return;
            }


            if (e.Name.Equals("Data/mail") && IslandHouse)
            {
                e.Edit(asset => {
                    IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                    data.Add("Islandvisit_Qi", this.Helper.Translation.Get("Islandvisit_Qi"));
                });
            }

            if (IslandToday && e.Name.StartsWith("Characters",false,true))
            {
                //harvey edit
                if (e.NameWithoutLocale.IsEquivalentTo("Characters/Harvey") && MarriedAndAllowed.Contains("Harvey"))
                {
                    e.Edit(asset =>
                    {
                        var editor = asset.AsImage();
                        Texture2D Harvey = ModEntry.Help.ModContent.Load<Texture2D>("assets/Spouses/Harvey_anim.png");
                        editor.PatchImage(Harvey, new Rectangle(0, 192, 64, 32), new Rectangle(0, 192, 64, 32), PatchMode.Replace);
                    });
                }

                //krobus
                if(e.Name.Name.Contains("Krobus") && MarriedAndAllowed.Contains("Krobus"))
                {
                    this.Monitor.Log("Krobus sprites will be edited.");

                    if (e.NameWithoutLocale.IsEquivalentTo("Portraits/Krobus"))
                    {
                        e.LoadFromModFile<Texture2D>("assets/Spouses/Krobus_Outside_Portrait.png", AssetLoadPriority.Medium);
                    }
                    if (e.NameWithoutLocale.IsEquivalentTo("Characters/Krobus"))
                    {
                        e.LoadFromModFile<Texture2D>("assets/Spouses/Krobus_Outside_Character.png", AssetLoadPriority.Medium);
                    }
                }

                if (e.Name.StartsWith("Characters/schedules/", false, true))
                {
                    
                    if (HasC2N && Config.Allow_Children && Children is not null && Config.UseModSchedule)
                    {
                        this.Monitor.LogOnce("Child To NPC is in the mod folder. Adding compatibility...", LogLevel.Trace);
                        ChildrenData.EditAllKidSchedules(Config.UseFurnitureBed, e);
                    }
                    Integrated.Schedules(e);
                }
            }

            if (e.Name.StartsWith("Characters/Dialogue/", false, true))
            {
                if (e.NameWithoutLocale.IsEquivalentTo("Characters/Dialogue/MarriageDialogueKrobus"))
                    e.Edit(asset =>
                    {
                        IDictionary<string, string> data = asset.AsDictionary<string, string>().Data;
                        data.Add("funLeave_Krobus", this.Helper.Translation.Get("Krobus.GoOutside"));
                    });
                    
                Integrated.Dialogues(e);

            }
        }

        //if SP, loadstagechanged will obtain required data. if MP, peercontext will.
        private void LoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == LoadStage.SaveLoadedBasicInfo)
            {
                GetRequiredData(Game1.MasterPlayer);
            }
        }
        private void PeerContextReceived(object sender, PeerContextReceivedEventArgs e)
        {
            var newFarmer = Game1.getFarmer(e.Peer.PlayerID);

            if (!newFarmer.IsLocalPlayer)
            {
                return;
            }

            GetRequiredData(newFarmer);
        }

        //these happen regardless of SP/MP
        private void OnDayEnding(object sender, DayEndingEventArgs e)
        {
            //get new %
            PreviousDayRandom = RandomizedInt;
            RandomizedInt = Random.Next(1, 101);
            IslandToday = Config.CustomChance >= RandomizedInt;
            IsFromTicket = false;

            var ticketday = Game1.player.mailReceived.Contains("VisitTicket_day");
            var ticketweek = Game1.player.mailReceived.Contains("VisitTicket_week");

            //if player used a visit ticket
            if (ticketday || ticketweek)
            {
                RandomizedInt = 0;
                IslandToday = true;
                IsFromTicket = true;

                //remove flags
                Game1.player.RemoveMail("VisitTicket_day");

                //set them in mod file. ticketday set to true for future loading
                status[Game1.player.userID.Value].DayVisit = ticketday;
                status[Game1.player.userID.Value].WeekVisit = (ticketweek, status[Game1.player.userID.Value]?.WeekVisit.Item2 ?? 0);

                //if true, check int value. if 7, reset. else, add 1
                var week = status[Game1.player.userID.Value].WeekVisit;
                if (week.Item1)
                {
                    if(week.Item2 == 7)
                    {
                        Game1.player.RemoveMail("VisitTicket_week");
                        status[Game1.player.userID.Value].WeekVisit = (false, 0);
                    }
                    else
                    {
                        status[Game1.player.userID.Value].WeekVisit = (true, week.Item2 + 1);
                    }
                }

                this.Helper.Data.WriteJsonFile("moddata.json", status);
            }

            //re-check values
            Children = Game1.player.getChildren();

            if(CCC == false)
            {
                CCC = Game1.player.hasCompletedCommunityCenter();
            }
            if(SawDevan4H == false)
            {
                SawDevan4H = Game1.player.eventsSeen.Contains(110371000);
            }
        }
        private void OnTimeChanged(object sender, TimeChangedEventArgs e)
        {
            if(IslandToday == false)
            {
                return;
            }

            if (e.NewTime >= 2200)
            {
                var bc = new BedCode();
                var ifh = Game1.getLocationFromName("IslandFarmHouse");

                foreach (NPC c in ifh.characters)
                {
                    if(c.isMarried())
                    {
                        this.Monitor.Log($"Pathing {c.Name} to bed in {ifh.Name}...");
                        try
                        {
                            bc.MakeSpouseGoToBed(c, ifh);
                        }
                        catch (Exception ex)
                        {
                            this.Monitor.Log($"An error ocurred while pathing {c.Name} to bed: {ex}");
                        }
                    }
                    
                    else if(!c.isMarried() && HasC2N == true)
                    {
                        this.Monitor.Log($"Pathing {c.Name} to kid bed in {ifh.Name}...");
                        try
                        {
                            bc.MakeKidGoToBed(c, ifh);
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
            ClearValues();

            //get new %
            PreviousDayRandom = 0;
            RandomizedInt = Random.Next(1, 101);
        }

        private void ClearValues()
        {
            status = null;

            this.Monitor.Log("Clearing Children...");
            Children?.Clear();

            //empty bools and int
            SawDevan4H = false;
            CCC = false;
            PreviousDayRandom = 0;
            RandomizedInt = 0;

            this.Monitor.Log("SawDevan4H = false; CCC = false; RandomizedInt = 0;");
        }
        private void GetRequiredData(Farmer player)
        {
            //try to read file from moddata. if empty, check mail
            //read here. 
            ReadModData(player);

            //now get user data
            var boatFix = player?.mailReceived?.Contains("willyBoatFixed");
            BoatFixed = boatFix ?? false;
            this.Monitor.Log($"BoatFixed = {BoatFixed};");

            IslandHouse = player?.mailReceived?.Contains("Island_UpgradeHouse") ?? false;
            bool QiMail = player?.mailReceived?.Contains("Islandvisit_Qi") ?? false;
            if(!QiMail && IslandHouse)
            {
                player.mailbox.Add("Islandvisit_Qi");
            }

            var married = Values.GetAllSpouses(player);
            foreach (var name in married)
            {
                if (IsDebug)
                {
                    this.Monitor.Log($"Checking NPC {name}...");
                }

                if (Values.IntegratedAndEnabled(name, Config))
                {
                    MarriedAndAllowed.Add(name);
                    this.Monitor.Log($"{name} is married to player.", LogLevel.Debug);
                }
            }

            Children = player.getChildren();
            CCC = player.hasCompletedCommunityCenter();
            SawDevan4H = player.eventsSeen.Contains(110371000);

            this.Monitor.Log($"\nChildren (count) = {Children};\nCCC = {CCC};\nSawDevan4H = {SawDevan4H};", LogLevel.Debug);
        }
        private void ReadModData(Farmer player)
        {
            var userID = player.userID.Value;
            var file = Helper.Data.ReadJsonFile<Dictionary<string, ModStatus>>("moddata.json");
            if(file == null)
            {
                status.Add(userID, new ModStatus(player));
            }
            else if (file.Keys.Any(id => id == userID))
            {
                if (file[userID].DayVisit)
                {
                    //set to true n remove
                    IsFromTicket = true;
                    IslandToday = true;
                    RandomizedInt = 0;
                    file[userID].DayVisit = false;
                }
                if (file[userID].WeekVisit.Item1)
                {
                    var wv = file[userID].WeekVisit;
                    //check value
                    if (wv.Item2 == 7)
                    {
                        file[userID].WeekVisit = (false, 0);
                    }
                    else
                    {
                        IsFromTicket = true;
                        IslandToday = true;
                        RandomizedInt = 0;
                        file[userID].WeekVisit = (true, wv.Item2 + 1);
                    }
                }
                else
                {
                    status = file;
                }
            }
            else
            {
                file.Add(userID, new ModStatus(player));
            }

            status = file;
        }

        /* Helpers + things the mod uses */

        private ModConfig Config;
        public static IApi jsonAssets;
        private static Random random;
        internal static IModHelper Help { get; private set; }
        internal static ITranslationHelper TL { get; private set; }
        internal static IMonitor Mon { get; private set; }
        internal static Random Random
        {
            get
            {
                random ??= new Random(((int)Game1.uniqueIDForThisGame * 26) + (int)(Game1.stats.DaysPlayed * 36));
                return random;
            }
        }

        /* User-related starts here */

        internal static bool IsDebug = false;
        internal static bool IslandToday { get; private set; }
        internal static bool IsFromTicket { get; private set; }
        internal static int RandomizedInt { get; private set; }
        internal static int PreviousDayRandom { get; private set; }

        /* children related */
        internal static List<Child> Children { get; private set; } = new();
        internal static Dictionary<string,string> InfoChildren = new(); //this refers to info in relation to the mod (ie, schedule data for island visit). not actual info

        /* player data */
        public static List<string> MarriedAndAllowed { get; private set; } = new();
        internal static bool SawDevan4H  = false;
        internal static bool CCC = false;
        internal static bool BoatFixed;
        internal static bool IslandHouse = false;
        internal static bool HasSVE;
        internal static bool HasC2N;
        internal static bool HasExGIM;
        internal static bool notfurniture;
        internal static Dictionary<string, ModStatus> status { get; private set; } = new();
    }
}