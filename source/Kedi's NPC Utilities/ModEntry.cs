/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KediDili/NPCUtilities
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using HarmonyLib;
using System;
using StardewValley.Events;
using StardewValley.Menus;
using Microsoft.Xna.Framework;

namespace KediNPCUtilities
{
    public class ModEntry : Mod
    {
        internal static new IModHelper Helper;
        internal static Dictionary<string, Dictionary<string, string>> UtilityData = new();

        internal static IManifest manifest;
        internal static IMonitor monitor;
        internal static ISpaceCore SpaceCoreAPI;
        public static bool isShakedAlready = false;
        public override void Entry(IModHelper helper)
        {
            Helper = helper;
            monitor = Monitor;

            Helper.Events.Content.AssetRequested += OnAssetRequested;
            Helper.Events.GameLoop.DayStarted += OnDayStarted;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            Helper.Events.Display.MenuChanged += OnMenuChanged;

            manifest = ModManifest;

            var harmony = new Harmony(ModManifest.UniqueID);

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
                prefix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.tryToReceiveActiveObject_Prefix))
            );
            Monitor.Log("Prefixed 'StardewValley.NPC.tryToReceiveActiveObject' method.");

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.canGetPregnant)),
                postfix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.canGetPregnant_Postfix))
            );
            Monitor.Log("Postfixed 'StardewValley.NPC.canGetPregnant' method.");

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.isGaySpouse)),
                postfix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.isGaySpouse_Postfix))
            );
            Monitor.Log("Postfixed 'StardewValley.NPC.isGaySpouse' method.");

            harmony.Patch(
                original: AccessTools.Method(typeof(NPC), nameof(NPC.checkAction)),
                prefix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.checkAction_Prefix))
            );
            Monitor.Log("Prefixed 'StardewValley.NPC.checkAction' method.");

            harmony.Patch(
                original: AccessTools.Method(typeof(BirthingEvent), nameof(BirthingEvent.setUp)),
                postfix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.setUp_Postfix))
            );
            Monitor.Log("Postfixed 'StardewValley.Events.BirthingEvent.setUp' method.");

            harmony.Patch(
                original: AccessTools.Method(typeof(DialogueBox), "shouldPortraitShake"),
                postfix: new HarmonyMethod(typeof(UtilityPatches), nameof(UtilityPatches.shouldPortraitShake_Postfix))
            );
            Monitor.Log("Postfixed 'StardewValley.Menus.DialogueBox.shouldPortraitShake' method.");
        }
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            SpaceCoreAPI = Helper.ModRegistry.GetApi<ISpaceCore>("spacechase0.SpaceCore");
            var methodInfo = AccessTools.Method(typeof(ModEntry), "changeRelationship");
            if (SpaceCoreAPI is not null)
            {
                SpaceCoreAPI.AddEventCommand("changeRelationship", methodInfo);
                Monitor.Log("Event command 'changeRelationship' has been added.");
            }
        }
        private void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is DialogueBox && e.NewMenu is null)
                isShakedAlready = false;
        }
        public void changeRelationship(Event @event, GameLocation gameLocation, GameTime gameTime, string[] args)
        {
            //args[0] is expected to be an actor's name.
            //args[1] is expected to be one of marriage, platonic, breakup, date, divorce
            if (Game1.player.friendshipData.ContainsKey(args[0]) && UtilityData.TryGetValue(args[0], out Dictionary<string, string> value))
            {
                if (value.ContainsKey(args[1] + "Proposal") && @event.getActorByName(args[0])?.Age != 2) //That's right, it's a child check <.<
                {
                    var data = UtilityData[args[0]][args[1] + "Proposal"];

                    if (data == @event.id.ToString())
                    {
                        switch (args[1].ToLower())
                        {
                            case "platonic":
                            case "marriage":
                                if (!Game1.player.isMarried())
                                {
                                    Game1.player.friendshipData[args[0]].Status = FriendshipStatus.Engaged;
                                    Game1.player.friendshipData[args[0]].RoommateMarriage = args[1] == "platonic";
                                    Game1.player.spouse = args[0];
                                    WorldDate weddingDate = new(Game1.Date);
                                    weddingDate.TotalDays += 3;
                                    while (!Game1.canHaveWeddingOnDay(weddingDate.DayOfMonth, weddingDate.Season))
                                        weddingDate.TotalDays++;

                                    Game1.player.friendshipData[args[0]].WeddingDate = weddingDate;
                                }
                                break;
                            case "breakup":
                                if (Game1.player.friendshipData[args[0]].Status == FriendshipStatus.Dating)
                                {
                                    Game1.player.friendshipData[args[0]].Status = FriendshipStatus.Friendly;
                                    Game1.player.friendshipData[args[0]].Points -= 150;

                                    Game1.player.eventsSeen.Remove(@event.id);
                                }
                                break;
                            case "date":
                                if (Game1.player.friendshipData[args[0]].Status != FriendshipStatus.Dating)
                                {
                                    Game1.player.friendshipData[args[0]].Status = FriendshipStatus.Dating;
                                }
                                break;
                            case "divorce":
                                Game1.player.divorceTonight.Value = true;
                                if (Game1.player.Money >= 50000)
                                    Game1.player.Money -= 50000;
                                else
                                {
                                    if (!Game1.player.modData.ContainsKey("KediDili.KNU.divorceDebt"))
                                        Game1.player.modData.Add("KediDili.KNU.divorceDebt", (50000 - Game1.player.Money).ToString());

                                    else if (Game1.player.modData["KediDili.KNU.divorceDebt"] == "0")
                                        Game1.player.modData["KediDili.KNU.divorceDebt"] = (50000 - Game1.player.Money).ToString();

                                    else if (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) > 0)
                                        Game1.player.modData["KediDili.KNU.divorceDebt"] = (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) + (50000 - Game1.player.Money)).ToString();

                                    Game1.player.Money = 0;
                                }
                                Game1.player.friendshipData[args[0]].Points = 0;
                                break;
                            default:
                                monitor.Log($"Warning: in the event with ID of {@event.id}, the NPC named {args[0]} has an entry in UtilityData, but '{args[1]}Proposal' key isn't a thing for NPCUtilities at all.", LogLevel.Warn);
                                break;
                        }
                    }
                }
                else
                    monitor.Log($"Warning: in the event with ID of {@event.id}, the NPC named {args[0]} has an entry in UtilityData, but it doesn't have the key for '{args[1]}Proposal'.", LogLevel.Warn);
            }
            else
                monitor.Log($"Warning: in the event with ID of {@event.id}, the NPC named {args[0]} is either haven't been met yet or they don't exist in UtilityData data asset.", LogLevel.Warn);
            @event.CurrentCommand++;
        }
        private void OnAssetRequested(object sender, AssetRequestedEventArgs e)
        {
            if (e.NameWithoutLocale.IsEquivalentTo(PathUtilities.NormalizePath("KediDili.KNU/UtilityData")))
                e.LoadFrom(() => new Dictionary<string, Dictionary<string, string>>(), AssetLoadPriority.Exclusive);
        }
        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            UtilityData = new();
            UtilityData = Helper.GameContent.Load<Dictionary<string, Dictionary<string, string>>>(PathUtilities.NormalizePath("KediDili.KNU/UtilityData"));

            if (Game1.Date.DayOfMonth == 1 && Game1.player.modData.ContainsKey("KediDili.KNU.divorceDebt"))
            {
                if (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) == Game1.player.Money)
                {
                    Game1.player.modData["KediDili.KNU.divorceDebt"] = "0";
                    Game1.player.Money = 0;
                }
                else if (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) > Game1.player.Money)
                {
                    Game1.player.modData["KediDili.KNU.divorceDebt"] = (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) - Game1.player.Money).ToString();
                    Game1.player.Money = 0;
                }
                else if (Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]) < Game1.player.Money)
                {
                    Game1.player.Money -= Convert.ToInt32(Game1.player.modData["KediDili.KNU.divorceDebt"]);
                    Game1.player.modData["KediDili.KNU.divorceDebt"] = "0";
                }
            }
            for (int i = 0; i < Game1.getLocationFromName("FarmHouse").characters.Count; i++)
                if (Game1.player.friendshipData.TryGetValue(Game1.getLocationFromName("FarmHouse").characters[i].Name, out Friendship friendship))
                    if (friendship.Status == FriendshipStatus.Divorced)
                        Game1.getLocationFromName("FarmHouse").characters[i].PerformDivorce();
        }
    }
}