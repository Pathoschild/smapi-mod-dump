/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ApryllForever/PolyamorySweetLove
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using SpaceCore.Events;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Audio;
using StardewValley.Characters;
using StardewValley.Events;
using StardewValley.Extensions;
using StardewValley.GameData.Shops;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Pathfinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using xTile.Dimensions;
using StardewValley.Util;
using StardewValley.GameData.Characters;
using System.Reflection;

namespace PolyamorySweetLove
{
    /// <summary>The mod entry point.</summary>
    public partial class ModEntry : Mod
    {

        public static Mod instance;

        public static IMonitor SMonitor;
        public static IModHelper SHelper;
        public static ModConfig Config;
        public static ModEntry context;
        public static Multiplayer mp;
        public static Random myRand;
        public static string farmHelperSpouse = null;
        internal static NPC tempOfficialSpouse;
        public static int bedSleepOffset = 76;

        public static string spouseToDivorce = null;
        public static int divorceHeartsLost;

        public static Dictionary<long, Dictionary<string, NPC>> currentSpouses = new Dictionary<long, Dictionary<string, NPC>>();
        public static Dictionary<long, Dictionary<string, NPC>> currentUnofficialSpouses = new Dictionary<long, Dictionary<string, NPC>>();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;

            Config = Helper.ReadConfig<ModConfig>();
            context = this;
            if (!Config.EnableMod)
                return;

            SMonitor = Monitor;
            SHelper = helper;

            mp = helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            myRand = new Random();

            helper.Events.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle; ;
            helper.Events.GameLoop.GameLaunched += GameLoop_GameLaunched;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            helper.Events.GameLoop.OneSecondUpdateTicked += GameLoop_OneSecondUpdateTicked;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.Input.ButtonReleased += OnButtonReleased;

            helper.Events.Content.AssetRequested += Content_AssetRequested;

            SpaceEvents.BeforeGiftGiven += AforeGiftGiven;

            Helper.ConsoleCommands.Add("Proposal_Sweet", "Attempt or Force Proposal. Usage:\n\nProposal_Sweet // shows info about current proposal.\nProposal_Sweet Attempt X // Attempt to propose to a character.\nProposal_Sweet Force X // Force Proposal to a character.", new Action<string, string[]>(ProposalCommand));



            PathFindControllerPatches.Initialize(Monitor, Config, helper);
            Divorce.Initialize(Monitor, Config, helper);
            NPCPatches.Initialize(Monitor, Config, helper);
            Game1Patches.Initialize(Monitor);
            LocationPatches.Initialize(Monitor, Config, helper);
            FarmerPatches.Initialize(Monitor, Config, helper);
            UIPatches.Initialize(Monitor, Config, helper);
            EventPatches.Initialize(Monitor, Config, helper);

            var harmony = new Harmony(ModManifest.UniqueID);


            // npc patches

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.marriageDuties)),
               postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_marriageDuties_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.getSpouse)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_getSpouse_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.isRoommate)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_isRoommate_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.isMarried)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_isMarried_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.isMarriedOrEngaged)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_isMarriedOrEngaged_Prefix))
            );


            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_tryToReceiveActiveObject_Prefix)),
               transpiler: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_tryToReceiveActiveObject_Transpiler))
            );


            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), "engagementResponse"),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_engagementResponse_Prefix)),
               postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_engagementResponse_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.spouseObstacleCheck)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_spouseObstacleCheck_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.setUpForOutdoorPatioActivity)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_setUpForOutdoorPatioActivity_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.playSleepingAnimation)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_playSleepingAnimation_Prefix)),
               postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_playSleepingAnimation_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.GetDispositionModifiedString)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_GetDispositionModifiedString_Prefix)),
               postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_GetDispositionModifiedString_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), "loadCurrentDialogue"),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_loadCurrentDialogue_Prefix)),
               postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_loadCurrentDialogue_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToRetrieveDialogue)),
               prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_tryToRetrieveDialogue_Prefix))
            );

            // harmony.Patch(
            //    original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
            //    prefix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.NPC_checkAction_Prefix))
            // );


            // Child patches

            harmony.Patch(
               original: typeof(Character).GetProperty("displayName").GetMethod,
               postfix: new HarmonyMethod(typeof(NPCPatches), nameof(NPCPatches.Character_displayName_Getter_Postfix))
            );

            // Path patches

            harmony.Patch(
               original: AccessTools.Constructor(typeof(PathFindController), new Type[] { typeof(Character), typeof(GameLocation), typeof(Point), typeof(int), typeof(bool) }),
               prefix: new HarmonyMethod(typeof(PathFindControllerPatches), nameof(PathFindControllerPatches.PathFindController_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Constructor(typeof(PathFindController), new Type[] { typeof(Character), typeof(GameLocation), typeof(Point), typeof(int), typeof(PathFindController.endBehavior) }),
               prefix: new HarmonyMethod(typeof(PathFindControllerPatches), nameof(PathFindControllerPatches.PathFindController_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Constructor(typeof(PathFindController), new Type[] { typeof(Character), typeof(GameLocation), typeof(Point), typeof(int), typeof(PathFindController.endBehavior), typeof(int) }),
               prefix: new HarmonyMethod(typeof(PathFindControllerPatches), nameof(PathFindControllerPatches.PathFindController_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Constructor(typeof(PathFindController), new Type[] { typeof(Character), typeof(GameLocation), typeof(Point), typeof(int) }),
               prefix: new HarmonyMethod(typeof(PathFindControllerPatches), nameof(PathFindControllerPatches.PathFindController_Prefix))
            );


            // Location patches

            harmony.Patch(
               original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.GetSpouseBed)),
               postfix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.FarmHouse_GetSpouseBed_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(FarmHouse), nameof(FarmHouse.getSpouseBedSpot)),
               prefix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.FarmHouse_getSpouseBedSpot_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Beach), nameof(Beach.checkAction)),
               prefix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.Beach_checkAction_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Beach), "resetLocalState"),
               postfix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.Beach_resetLocalState_Postfix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), "checkEventPrecondition", new Type[] { typeof(string), typeof(bool) }),
               prefix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.GameLocation_checkEventPrecondition_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(ManorHouse), nameof(ManorHouse.performAction), new Type[] { typeof(string[]), typeof(Farmer), typeof(Location) }),
               prefix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.ManorHouse_performAction_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(ManorHouse), nameof(ManorHouse.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.ManorHouse_answerDialogueAction_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.answerDialogueAction)),
               prefix: new HarmonyMethod(typeof(LocationPatches), nameof(LocationPatches.GameLocation_answerDialogueAction_Prefix))
            );


            // pregnancy patches

            harmony.Patch(
               original: AccessTools.Method(typeof(Utility), nameof(Utility.pickPersonalFarmEvent)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.Utility_pickPersonalFarmEvent_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(QuestionEvent), nameof(QuestionEvent.setUp)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.QuestionEvent_setUp_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(BirthingEvent), nameof(BirthingEvent.setUp)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.BirthingEvent_setUp_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(BirthingEvent), nameof(BirthingEvent.tickUpdate)),
               prefix: new HarmonyMethod(typeof(ModEntry), nameof(ModEntry.BirthingEvent_tickUpdate_Prefix))
            );


            // Farmer patches

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.doDivorce)),
               prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.Farmer_doDivorce_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.isMarriedOrRoommates)),
               prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.Farmer_isMarried_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getSpouse)),
               postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.Farmer_getSpouse_Postfix))
            );
            harmony.Patch(
               original: AccessTools.PropertyGetter(typeof(Farmer), nameof(Farmer.spouse)),
               postfix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.Farmer_spouse_Postfix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.GetSpouseFriendship)),
               prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.Farmer_GetSpouseFriendship_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.checkAction)),
               prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.Farmer_checkAction_Prefix))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(Farmer), nameof(Farmer.getChildren)),
               prefix: new HarmonyMethod(typeof(FarmerPatches), nameof(FarmerPatches.Farmer_getChildren_Prefix))
            );


            // UI patches

            harmony.Patch(
               original: AccessTools.Method(typeof(SocialPage), "drawNPCSlot"),
               prefix: new HarmonyMethod(typeof(UIPatches), nameof(UIPatches.SocialPage_drawNPCSlot_prefix)),
               transpiler: new HarmonyMethod(typeof(UIPatches), nameof(UIPatches.SocialPage_drawSlot_transpiler))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(SocialPage), "drawFarmerSlot"),
               transpiler: new HarmonyMethod(typeof(UIPatches), nameof(UIPatches.SocialPage_drawSlot_transpiler))
            );

            harmony.Patch(
               original: AccessTools.Method(typeof(SocialPage.SocialEntry), nameof(SocialPage.SocialEntry.IsMarriedToAnyone)),
               prefix: new HarmonyMethod(typeof(UIPatches), nameof(UIPatches.SocialPage_isMarriedToAnyone_Prefix))
            );

            harmony.Patch(
               original: typeof(DialogueBox).GetConstructor(new Type[] { typeof(List<string>) }),
               prefix: new HarmonyMethod(typeof(UIPatches), nameof(UIPatches.DialogueBox_Prefix))
            );


            // Event patches

            harmony.Patch(
               original: AccessTools.Method(typeof(Event), nameof(Event.answerDialogueQuestion)),
               prefix: new HarmonyMethod(typeof(EventPatches), nameof(EventPatches.Event_answerDialogueQuestion_Prefix))
            );
            harmony.Patch(
               original: AccessTools.Method(typeof(Event.DefaultCommands), nameof(Event.DefaultCommands.LoadActors)),
               prefix: new HarmonyMethod(typeof(EventPatches), nameof(EventPatches.Event_command_loadActors_Prefix)),
               postfix: new HarmonyMethod(typeof(EventPatches), nameof(EventPatches.Event_command_loadActors_Postfix))
            );


            // Game1 patches

            harmony.Patch(
               original: AccessTools.GetDeclaredMethods(typeof(Game1)).Where(m => m.Name == "getCharacterFromName" && m.ReturnType == typeof(NPC)).First(),
               prefix: new HarmonyMethod(typeof(Game1Patches), nameof(Game1Patches.getCharacterFromName_Prefix))
            );

        }

        public override object GetApi()
        {
            return new PolyamorySweetLoveAPI();
        }

        private void Content_AssetRequested(object sender, StardewModdingAPI.Events.AssetRequestedEventArgs e)
        {
            if (!Config.EnableMod)
                return;
            if (e.NameWithoutLocale.IsEquivalentTo("Data/Shops"))
            {
                e.Edit(delegate (IAssetData data)
                {
                    var dict = data.AsDictionary<string, ShopData>();
                    try
                    {
                        for (int i = 0; i < dict.Data["DesertTrade"].Items.Count; i++)
                        {
                            if (dict.Data["DesertTrade"].Items[i].ItemId == "(O)808")
                                dict.Data["DesertTrade"].Items[i].Condition = "PLAYER_FARMHOUSE_UPGRADE Current 1, !PLAYER_HAS_ITEM Current 808";
                        }
                    }
                    catch
                    {

                    }
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/HaleyHouse"))
            {
                e.Edit(delegate (IAssetData idata)
                {
                    IDictionary<string, string> data = idata.AsDictionary<string, string>().Data;

                    string key = "195012/f Haley 2500/f Emily 2500/f Penny 2500/f Abigail 2500/f Leah 2500/f Maru 2500/o Abigail/o Penny/o Leah/o Emily/o Maru/o Haley/o Shane/o Harvey/o Sebastian/o Sam/o Elliott/o Alex/e 38/e 2123343/e 10/e 901756/e 54/e 15/k 195019";
                    if (data.TryGetValue(key, out string value))
                    {
                        data[key] = Regex.Replace(value, "(pause 1000/speak Maru \\\")[^$]+.a\\\"", $"$1{SHelper.Translation.Get("confrontation-female")}$h\"/emote Haley 21 true/emote Emily 21 true/emote Penny 21 true/emote Maru 21 true/emote Leah 21 true/emote Abigail 21").Replace("/dump girls 3", "");
                        //data["91740942"] = data["195012/f Haley 2500/f Emily 2500/f Penny 2500/f Abigail 2500/f Leah 2500/f Maru 2500/o Abigail/o Penny/o Leah/o Emily/o Maru/o Haley/o Shane/o Harvey/o Sebastian/o Sam/o Elliott/o Alex/e 38/e 2123343/e 10/e 901756/e 54/e 15/k 195019"];
                    }
                    key = "195012/f Olivia 2500/f Sophia 2500/f Claire 2500/f Haley 2500/f Emily 2500/f Penny 2500/f Abigail 2500/f Leah 2500/f Maru 2500/o Abigail/o Penny/o Leah/o Emily/o Maru/o Haley/o Shane/o Harvey/o Sebastian/o Sam/o Elliott/o Alex/e 38/e 2123343/e 10/e 901756/e 54/e 15/k 195019";
                    if (data.TryGetValue(key, out value))
                    {
                        data[key] = Regex.Replace(value, "(pause 1000/speak Maru \\\")[^$]+.a\\\"", $"$1{SHelper.Translation.Get("confrontation-female")}$h\"/emote Haley 21 true/emote Emily 21 true/emote Penny 21 true/emote Maru 21 true/emote Leah 21 true/emote Abigail 21").Replace("/dump girls 3", "");
                        //data["91740942"] = data["195012/f Haley 2500/f Emily 2500/f Penny 2500/f Abigail 2500/f Leah 2500/f Maru 2500/o Abigail/o Penny/o Leah/o Emily/o Maru/o Haley/o Shane/o Harvey/o Sebastian/o Sam/o Elliott/o Alex/e 38/e 2123343/e 10/e 901756/e 54/e 15/k 195019"];
                    }

                    if (data.TryGetValue("choseToExplain", out value))
                    {
                        data["choseToExplain"] = Regex.Replace(value, "(pause 1000/speak Maru \\\")[^$]+.a\\\"", $"$1{SHelper.Translation.Get("confrontation-female")}$h\"/emote Haley 21 true/emote Emily 21 true/emote Penny 21 true/emote Maru 21 true/emote Leah 21 true/emote Abigail 21").Replace("/dump girls 4", "");
                    }
                    if (data.TryGetValue("lifestyleChoice", out value))
                    {
                        data["lifestyleChoice"] = Regex.Replace(value, "(pause 1000/speak Maru \\\")[^$]+.a\\\"", $"$1{SHelper.Translation.Get("confrontation-female")}$h\"/emote Haley 21 true/emote Emily 21 true/emote Penny 21 true/emote Maru 21 true/emote Leah 21 true/emote Abigail 21").Replace("/dump girls 4", "");
                    }

                });

            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/Events/Saloon"))
            {
                e.Edit(delegate (IAssetData idata)
                {
                    IDictionary<string, string> data = idata.AsDictionary<string, string>().Data;
                    string key = "195013/f Shane 2500/f Sebastian 2500/f Sam 2500/f Harvey 2500/f Alex 2500/f Elliott 2500/o Abigail/o Penny/o Leah/o Emily/o Maru/o Haley/o Shane/o Harvey/o Sebastian/o Sam/o Elliott/o Alex/e 911526/e 528052/e 9581348/e 43/e 384882/e 233104/k 195099";
                    if (!data.TryGetValue(key, out string value))
                    {
                        Monitor.Log("Missing event key for Saloon!");
                        return;
                    }
                    data[key] = Regex.Replace(value, "(pause 1000/speak Sam \\\")[^$]+.a\\\"", $"$1{SHelper.Translation.Get("confrontation-male")}$h\"/emote Shane 21 true/emote Sebastian 21 true/emote Sam 21 true/emote Harvey 21 true/emote Alex 21 true/emote Elliott 21").Replace("/dump guys 3", "");
                    //data["19501342"] = Regex.Replace(aData, "(pause 1000/speak Sam \\\")[^$]+.a\\\"",$"$1{SHelper.Translation.Get("confrontation-male")}$h\"/emote Shane 21 true/emote Sebastian 21 true/emote Sam 21 true/emote Harvey 21 true/emote Alex 21 true/emote Elliott 21").Replace("/dump guys 3", "");
                    if (data.TryGetValue("choseToExplain", out value))
                    {
                        data["choseToExplain"] = Regex.Replace(value, "(pause 1000/speak Sam \\\")[^$]+.a\\\"", $"$1{SHelper.Translation.Get("confrontation-male")}$h\"/emote Shane 21 true/emote Sebastian 21 true/emote Sam 21 true/emote Harvey 21 true/emote Alex 21 true/emote Elliott 21").Replace("/dump guys 4", "");
                    }
                    if (data.TryGetValue("crying", out value))
                    {
                        data["crying"] = Regex.Replace(value, "(pause 1000/speak Sam \\\")[^$]+.a\\\"", $"$1{SHelper.Translation.Get("confrontation-male")}$h\"/emote Shane 21 true/emote Sebastian 21 true/emote Sam 21 true/emote Harvey 21 true/emote Alex 21 true/emote Elliott 21").Replace("/dump guys 4", "");
                    }
                });
            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Strings/StringsFromCSFiles"))
            {
                e.Edit(delegate (IAssetData idata)
                {
                    IDictionary<string, string> data = idata.AsDictionary<string, string>().Data;
                    data["NPC.cs.3985"] = Regex.Replace(data["NPC.cs.3985"], @"\.\.\.\$s.+", $"$n#$b#$c 0.5#{data["ResourceCollectionQuest.cs.13681"]}#{data["ResourceCollectionQuest.cs.13683"]}");
                    Monitor.Log($"NPC.cs.3985 is set to \"{data["NPC.cs.3985"]}\"");
                });

            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/animationDescriptions"))
            {
                e.Edit(delegate (IAssetData idata)
                {
                    IDictionary<string, string> data = idata.AsDictionary<string, string>().Data;
                    List<string> sleepKeys = data.Keys.ToList().FindAll((s) => s.EndsWith("_Sleep"));
                    foreach (string key in sleepKeys)
                    {
                        if (!data.ContainsKey(key.ToLower()))
                        {
                            Monitor.Log($"adding {key.ToLower()} to animationDescriptions");
                            data.Add(key.ToLower(), data[key]);
                        }
                    }
                });

            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Data/EngagementDialogue"))
            {
                if (!Config.RomanceAllVillagers)
                    return;
                e.Edit(delegate (IAssetData idata)
                {
                    IDictionary<string, string> data = idata.AsDictionary<string, string>().Data;
                    Farmer f = Game1.player;
                    if (f == null)
                    {
                        return;
                    }
                    foreach (string friend in f.friendshipData.Keys)
                    {
                        if (!data.ContainsKey(friend + "0"))
                        {
                            data[friend + "0"] = "";
                        }
                        if (!data.ContainsKey(friend + "1"))
                        {
                            data[friend + "1"] = "";
                        }
                    }
                });
            }
            else if (Config.RomanceAllVillagers && (e.NameWithoutLocale.BaseName.StartsWith("Characters/schedules/") || e.NameWithoutLocale.BaseName.StartsWith("Characters\\schedules\\")))
            {
                try
                {
                    string name = e.NameWithoutLocale.BaseName.Replace("Characters/schedules/", "").Replace("Characters\\schedules\\", "");
                    NPC npc = Game1.getCharacterFromName(name);
                    if (npc != null && npc.Age < 2 && !(npc is Child))
                    {

                        if (Game1.characterData[npc.Name].CanBeRomanced)
                        {
                            Monitor.Log($"can edit schedule for {name}");
                            e.Edit(delegate (IAssetData idata)
                            {
                                IDictionary<string, string> data = idata.AsDictionary<string, string>().Data;
                                List<string> keys = new List<string>(data.Keys);
                                foreach (string key in keys)
                                {
                                    if (!data.ContainsKey($"marriage_{key}"))
                                        data[$"marriage_{key}"] = data[key];
                                }
                            });
                        }
                    }
                }
                catch
                {
                }


            }
            else if (e.NameWithoutLocale.IsEquivalentTo("Strings/Locations"))
            {
                e.Edit(delegate (IAssetData idata)
                {
                    IDictionary<string, string> data = idata.AsDictionary<string, string>().Data;
                    data["Beach_Mariner_PlayerBuyItem_AnswerYes"] = data["Beach_Mariner_PlayerBuyItem_AnswerYes"].Replace("5000", Config.PendantPrice + "");
                });
            }
            else if (e.Name.IsEquivalentTo("Strings/StringsFromApryllFiles"))
            {
                e.LoadFrom(
                    () =>
                    {
                        return new Dictionary<string, string>
                        {
                            ["FlowerDanceFemaleAccept"] = "You want to be my partner for the flower dance?#$b#Okay! I'd love to dance with you! <$h",
                            ["FlowerDanceMaleAccept"] = "You want to be my partner for the flower dance?#$b#Okay! I'd love to dance with you! <$h"
                        };
                    },
                    AssetLoadPriority.Medium
                );
            }
        }

        public static void AforeGiftGiven(object sender, EventArgsBeforeReceiveObject e)
        {

            if (sender != Game1.player)
                return;
            ModEntry.ResetSpouses(Game1.player);
            var roomie = Game1.player.isRoommate("");
            NPC c = e.Npc;
            Friendship friendship;
            Game1.player.friendshipData.TryGetValue(c.Name, out friendship);


            if (e.Gift.Name.Equals("Áine Flower"))

            {
                //if (c.Equals(Game1.player.spouse) || c.Equals(roomie))
                if (ModEntry.GetSpouses(Game1.player, true).ContainsKey(c.Name))
                {
                    //if (ModEntry.GetSpouses(Game1.player, true).ContainsKey(c.Name))
                    {
                        Game1.player.spouse = c.Name;
                        ModEntry.ResetSpouses(Game1.player);
                        Game1.currentLocation.playSound("dwop", null, null, SoundContext.NPC);

                        {
                            FarmHouse fh = Utility.getHomeOfFarmer(Game1.player);
                            fh.showSpouseRoom();
                            if (Game1.player.currentLocation == fh)
                            {
                                SHelper.Reflection.GetMethod(fh, "resetLocalState").Invoke();
                            }
                            else
                            {
                                Game1.addHUDMessage(new HUDMessage("The room and patio will change when you enter the farmhouse."));
                            }
                        }
                    }
                }


                else if (friendship.Points < 2000)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:AineFlower_reject", c.displayName));
                }
                else
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:AineFlower_accept", c.displayName));
                    Game1.player.changeFriendship(5, c);
                }
            }



            /*
            else if (e.Gift.ParentSheetIndex == 458)

            {
                if (c.datable.Value)
                {
                    if (Game1.random.NextBool())
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3955", c.displayName));
                    }
                    else
                    {
                        c.CurrentDialogue.Push(Game1.random.NextBool() ? new Dialogue(c, "Strings\\StringsFromCSFiles:NPC.cs.3956", false) : new Dialogue(c, "Strings\\StringsFromCSFiles:NPC.cs.3957", true));
                    }
                    Game1.drawDialogue(c);

                }
                else
                {
                    if (friendship?.IsDating() == true)
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\UI:AlreadyDatingBouquet", c.displayName));

                    }
                    if (friendship?.IsDivorced() == true)
                    {
                        c.CurrentDialogue.Push(new Dialogue(c, "Strings\\Characters:Divorced_bouquet", true));
                        Game1.drawDialogue(c);

                    }
                    if (friendship?.Points < Config.MinPointsToDate / 2f)
                    {
                        c.CurrentDialogue.Push(Game1.random.NextBool() ? new Dialogue(c, "Strings\\StringsFromCSFiles:NPC.cs.3958", false) : new Dialogue(c, "Strings\\StringsFromCSFiles:NPC.cs.3959", true));
                        Game1.drawDialogue(c);

                    }
                    if (friendship?.Points < Config.MinPointsToDate)
                    {
                        c.CurrentDialogue.Push(new Dialogue(c, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose("3960", "3961"), false));
                        Game1.drawDialogue(c);

                    }
                    if (friendship?.IsDating() == false)
                    {
                        friendship.Status = FriendshipStatus.Dating;
                        Multiplayer mp = ModEntry.SHelper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
                        mp.globalChatInfoMessage("Dating", new string[]
                        {
                                   Game1.player.Name,
                                    c.displayName
                        });
                    }
                    c.CurrentDialogue.Push(new Dialogue(c, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose("3962", "3963"), true));
                    Game1.player.changeFriendship(25, c);
                    Game1.player.reduceActiveItemByOne();
                    Game1.player.completelyStopAnimatingOrDoingAction();
                    c.doEmote(20, true);
                    Game1.drawDialogue(c);



                }
            }*/
        }

        public static bool Button = false;
        public static bool Proposal_Sweet = false;

        public void OnButtonPressed(object sender, EventArgs e)
        {
            bool actionButton = this.Helper.Input.IsDown(SButton.MouseLeft) || this.Helper.Input.IsDown(SButton.MouseMiddle) || this.Helper.Input.IsDown(SButton.MouseRight) || this.Helper.Input.IsDown(SButton.C) || this.Helper.Input.IsDown(SButton.LeftTrigger) || this.Helper.Input.IsDown(SButton.RightTrigger) || this.Helper.Input.IsDown(SButton.LeftShoulder) || this.Helper.Input.IsDown(SButton.RightShoulder) || this.Helper.Input.IsDown(SButton.ControllerA) || this.Helper.Input.IsDown(SButton.ControllerB) || this.Helper.Input.IsDown(SButton.ControllerX) || this.Helper.Input.IsDown(SButton.ControllerY) || this.Helper.Input.IsDown(SButton.ControllerBack) || this.Helper.Input.IsDown(SButton.ControllerStart) || this.Helper.Input.IsDown(SButton.DPadDown) || this.Helper.Input.IsDown(SButton.DPadLeft) || this.Helper.Input.IsDown(SButton.DPadUp) || this.Helper.Input.IsDown(SButton.DPadRight) || this.Helper.Input.IsDown(SButton.LeftStick) || this.Helper.Input.IsDown(SButton.RightStick) || this.Helper.Input.IsDown(SButton.BigButton) || this.Helper.Input.IsDown(SButton.X);

            if (actionButton)
            {
                Button = true;
            }
        }
        public void OnButtonReleased(object sender, EventArgs e)
        {
            // bool actionButton = this.Helper.Input.IsDown(SButton.MouseLeft) || this.Helper.Input.IsDown(SButton.MouseMiddle) || this.Helper.Input.IsDown(SButton.MouseRight) || this.Helper.Input.IsDown(SButton.C) || this.Helper.Input.IsDown(SButton.LeftTrigger);

            //if (actionButton)

            Button = false;

        }


        private void ProposalCommand(string arg1, string[] arg2)
        {
            if (!Context.IsWorldReady)
            {
                Monitor.Log("Game not loaded.", LogLevel.Error);
                return;
            }

            if (!(Game1.player.friendshipData[Game1.player.spouse].IsEngaged()))
            {
                Monitor.Log("No current engagement.", LogLevel.Alert);
                return;
            }
            string fiancee = Game1.player.spouse;
            if (arg2.Length == 0)
            {
                Monitor.Log($"{Game1.player.Name} is engaged to {fiancee}. The wedding is in {Game1.player.friendshipData[fiancee].CountdownToWedding} days, on {Utility.getDateStringFor(Game1.player.friendshipData[fiancee].WeddingDate.DayOfMonth, Game1.player.friendshipData[fiancee].WeddingDate.SeasonIndex, Game1.player.friendshipData[fiancee].WeddingDate.Year)}.", LogLevel.Info);
            }
            else if (arg2.Length == 2 && arg2[0] == "Attempt")
            {
                NPC monica = Game1.getCharacterFromName(arg2[1]);

                if (monica != null)
                {
                    //Button = true;

                    AttemptEngagement(monica, Game1.player);

                    // typeof(NPC).GetMethod("engagementResponse", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(monica, new object[] { Game1.player, false });

                }

                Monitor.Log($"{Game1.player.Name} attemping to propose to {monica}.", LogLevel.Info);
            }
            else if (arg2.Length == 2 && arg2[0] == "Force")
            {

                NPC JaneGrey = Game1.getCharacterFromName(arg2[1]);

                TryForce(JaneGrey, Game1.player);

           
                Monitor.Log($"{Game1.player.Name} is now engaged to {JaneGrey}.", LogLevel.Info);
            }
        }

        public static void AttemptEngagement(NPC __instance, Farmer who) //(NPC __instance, ref Farmer who,
        {
            Friendship friendship;
            who.friendshipData.TryGetValue(__instance.Name, out friendship);

            string acceptpendant = $"Strings\\StringsFromCSFiles\\{__instance.Name}:{__instance.Name}_Engaged";
            string rejectDivorced = $"Characters\\Dialogue\\{__instance.Name}:RejectMermaidPendant_Divorced";
            string rejectNotDatable = $"Characters\\Dialogue\\{__instance.Name}:RejectMermaidPendant_NotDatable";
            string rejectNpcAlreadyMarried = $"Characters\\Dialogue\\{__instance.Name}:RejectMermaidPendant_NpcWithSomeoneElse";
            string rejectPlayerAlreadyMarried = $"Characters\\Dialogue\\{__instance.Name}:RejectMermaidPendant_PlayerWithSomeoneElse";
            string rejectUnder8Hearts = $"Characters\\Dialogue\\{__instance.Name}:RejectMermaidPendant_Under8Hearts";
            string rejectUnderTenHearts = $"Characters\\Dialogue\\{__instance.Name}:RejectMermaidPendant_Under10Hearts";
            string rejectUnderTenHeartsAskedAgain = $"Characters\\Dialogue\\{__instance.Name}:RejectMermaidPendant_Under10Hearts_AskedAgain";

            SMonitor.Log($"Try give pendant to {__instance.Name}");
            if (who.isEngaged())
            {
                SMonitor.Log($"Tried to give pendant while player already currently engaged");

                if ((__instance.Dialogue.ContainsKey("RejectMermaidPendant_PlayerWithSomeoneElse")))
                {
                    __instance.setNewDialogue(rejectPlayerAlreadyMarried, false, false);
                }

                else
                {
                    __instance.CurrentDialogue.Push(new Dialogue(__instance, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose("3965", "3966"), true));
                    Game1.drawDialogue(__instance);
                }
            }

            else if (!__instance.datable.Value)
            {
                SMonitor.Log($"Tried to give pendant to someone not datable");

                if ((__instance.Dialogue.ContainsKey("RejectMermaidPendant_NotDatable")))
                {
                    __instance.setNewDialogue(rejectNotDatable, false, false);
                }

                else
                {
                    if (ModEntry.myRand.NextDouble() < 0.5)
                    {
                        Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3969", __instance.displayName));

                    }
                }
                __instance.CurrentDialogue.Push(new Dialogue(__instance, "Strings\\StringsFromCSFiles:NPC.cs." + ((__instance.Gender == Gender.Female) ? "3970" : "3971"), false));
                Game1.drawDialogue(__instance);

            }
            else if (__instance.datable.Value && who.friendshipData.ContainsKey(__instance.Name) && who.friendshipData[__instance.Name].Points < 10) //Math.Min(10, Config.MinPointsToMarry)
            {
                SMonitor.Log($"Tried to give pendant to someone with fewer hearts than 10.");

                if (!who.friendshipData[__instance.Name].ProposalRejected)
                {
                    if ((__instance.Dialogue.ContainsKey("RejectMermaidPendant_Under10Hearts")))
                    {
                        __instance.setNewDialogue(rejectUnderTenHearts, false, false);
                    }
                    else
                    {
                        __instance.CurrentDialogue.Push(new Dialogue(__instance, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose("3972", "3973"), false));
                    }
                    Game1.drawDialogue(__instance);
                    who.changeFriendship(-50, __instance);
                    who.friendshipData[__instance.Name].ProposalRejected = true;

                }
                if ((__instance.Dialogue.ContainsKey("RejectMermaidPendant_Under10Hearts_AskedAgain")))
                {
                    __instance.setNewDialogue(rejectUnderTenHeartsAskedAgain, false, false);
                }
                else
                {
                    __instance.CurrentDialogue.Push(new Dialogue(__instance, "Strings\\StringsFromCSFiles:NPC.cs." + Game1.random.Choose("3974", "3975"), true));
                }
                Game1.drawDialogue(__instance);
                who.changeFriendship(-100, __instance);

            }
            else
            {

                //WORK HERE APRYLL. PUT MEGAN UP.
                SMonitor.Log($"Tried to give pendant to someone marriable");
                if (who.HouseUpgradeLevel >= 1)
                {


                    Game1.changeMusicTrack("silence");
                    who.spouse = __instance.Name;

                    {
                        Game1.Multiplayer.globalChatInfoMessage("Engaged", Game1.player.Name, __instance.GetTokenizedDisplayName());
                    }

                    //Friendship friendship = who.friendshipData[base.Name];
                    friendship.Status = FriendshipStatus.Engaged;

                    WorldDate worldDate = new WorldDate(Game1.Date);
                    worldDate.TotalDays += 3;
                    while (!Game1.canHaveWeddingOnDay(worldDate.DayOfMonth, worldDate.Season))
                    {
                        worldDate.TotalDays++;
                    }

                    friendship.WeddingDate = worldDate;
                    __instance.CurrentDialogue.Clear();

                    {
                        Dialogue dialogue2 = StardewValley.Dialogue.TryGetDialogue(__instance, "Data\\EngagementDialogue:" + __instance.Name + "0");
                        if (dialogue2 != null)
                        {
                            __instance.CurrentDialogue.Push(dialogue2);
                        }

                        dialogue2 = StardewValley.Dialogue.TryGetDialogue(__instance, "Strings\\StringsFromCSFiles:" + __instance.Name + "_Engaged");
                        if (dialogue2 != null)
                        {
                            __instance.CurrentDialogue.Push(dialogue2);
                        }
                        else
                        {
                            __instance.CurrentDialogue.Push(new Dialogue(__instance, "Strings\\StringsFromCSFiles:NPC.cs.3980"));
                        }
                    }

                    Dialogue obj = __instance.CurrentDialogue.Peek();
                    obj.onFinish = (Action)Delegate.Combine(obj.onFinish, (Action)delegate
                    {
                        Game1.changeMusicTrack("none", track_interruptable: true);
                        GameLocation.HandleMusicChange(null, Game1.player.currentLocation);
                    });
                    who.changeFriendship(1, __instance);
                    who.reduceActiveItemByOne();
                    who.completelyStopAnimatingOrDoingAction();
                    Game1.drawDialogue(__instance);

                    //typeof(NPC).GetMethod("engagementResponse", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(__instance, new object[] { who, false });

                }
                SMonitor.Log($"Can't marry");
                if (ModEntry.myRand.NextDouble() < 0.5)
                {
                    Game1.drawObjectDialogue(Game1.content.LoadString("Strings\\StringsFromCSFiles:NPC.cs.3969", __instance.displayName));

                }
                __instance.CurrentDialogue.Push(new Dialogue(__instance, "Strings\\StringsFromCSFiles:NPC.cs.3972", false));
                Game1.drawDialogue(__instance);

            }

        }


        public static void TryForce(NPC __instance, Farmer who)
        {
            if (__instance.IsVillager)
            {
                if (who.HouseUpgradeLevel == 0)
                {
                    SMonitor.Log($"You must upgrade your house in order to do this sorts of thing!");
                }

                else if (who.HouseUpgradeLevel >= 1)
                {
                    Friendship friendship;
                    who.friendshipData.TryGetValue(__instance.Name, out friendship);

                    Game1.changeMusicTrack("silence");
                    who.spouse = __instance.Name;

                    {
                        Game1.Multiplayer.globalChatInfoMessage("Engaged", Game1.player.Name, __instance.GetTokenizedDisplayName());
                    }

                    //Friendship friendship = who.friendshipData[base.Name];
                    friendship.Status = FriendshipStatus.Engaged;

                    WorldDate worldDate = new WorldDate(Game1.Date);
                    worldDate.TotalDays += 3;
                    while (!Game1.canHaveWeddingOnDay(worldDate.DayOfMonth, worldDate.Season))
                    {
                        worldDate.TotalDays++;
                    }

                    friendship.WeddingDate = worldDate;
                    __instance.CurrentDialogue.Clear();

                    {
                        Dialogue dialogue2 = StardewValley.Dialogue.TryGetDialogue(__instance, "Data\\EngagementDialogue:" + __instance.Name + "0");
                        if (dialogue2 != null)
                        {
                            __instance.CurrentDialogue.Push(dialogue2);
                        }

                        dialogue2 = StardewValley.Dialogue.TryGetDialogue(__instance, "Strings\\StringsFromCSFiles:" + __instance.Name + "_Engaged");
                        if (dialogue2 != null)
                        {
                            __instance.CurrentDialogue.Push(dialogue2);
                        }
                        else
                        {
                            __instance.CurrentDialogue.Push(new Dialogue(__instance, "Strings\\StringsFromCSFiles:NPC.cs.3980"));
                        }
                    }

                    Dialogue obj = __instance.CurrentDialogue.Peek();
                    obj.onFinish = (Action)Delegate.Combine(obj.onFinish, (Action)delegate
                    {
                        Game1.changeMusicTrack("none", track_interruptable: true);
                        GameLocation.HandleMusicChange(null, Game1.player.currentLocation);
                    });
                    who.changeFriendship(1, __instance);
                    who.reduceActiveItemByOne();
                    who.completelyStopAnimatingOrDoingAction();
                    Game1.drawDialogue(__instance);

                }
            }
            else
            {
                SMonitor.Log($"The entity you are to force is not marriagable. You cannot do this!!!");

            }

        }
    }
}