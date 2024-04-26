/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using StardewModdingAPI;
using StardewValley;
using static SpousesIsland.Additions.Dialogues;

namespace SpousesIsland.Patches;

internal static class NpcPatches
{
    private static string Translate(string msg) => ModEntry.Translate(msg);
    private static void Log(string msg, LogLevel lv = LogLevel.Trace) => ModEntry.Mon.Log(msg, lv);

    internal static void Apply(Harmony harmony)
    {
        Log($"Applying Harmony patch \"{nameof(Patches)}\": prefixing SDV method \"NPC.tryToReceiveActiveObject(Farmer who)\".");
        harmony.Patch(
            original: AccessTools.Method(typeof(NPC), nameof(NPC.tryToReceiveActiveObject)),
            prefix: new HarmonyMethod(typeof(NpcPatches), nameof(Pre_tryToReceiveActiveObject))
            );
    }

    /// <summary>
    /// If the item received is ours, runs custom actions
    /// </summary>
    /// <param name="__instance">NPC receiving.</param>
    /// <param name="who">Player.</param>
    /// <param name="__result">OG result.</param>
    /// <param name="probe">If just checking for an action.</param>
    /// <returns>Whether the OG method should be run.</returns>
    private static bool Pre_tryToReceiveActiveObject(NPC __instance, Farmer who, ref bool __result, bool probe = false)
    {
        var obj = who.ActiveObject;
        if (obj == null)
        {
            return true;
        }

        if (!TryGetCustomFields(obj, out var days))
            return true;

        //if just checking for an action
        if (probe)
        {
            __result = true;
            return true;
        }

        who.Halt();
        who.faceGeneralDirection(__instance.getStandingPosition(), 0, opposite: false, useTileCalculations: false);

        if (ModEntry.IslandToday && __instance.Name != "Willy")
        {
            //tell player
            string alreadyongoing = Translate("AlreadyOngoing.Visit");
            Game1.addHUDMessage(new HUDMessage(alreadyongoing, HUDMessage.newQuest_type));
        }
        //if festival tomorrow
        else if (Utility.isFestivalDay(Game1.dayOfMonth + 1, Game1.season))
        {
            //tell player there's a festival tomorrow
            var notice = Game1.parseText(Translate("FestivalTomorrow"));
            Game1.drawDialogueBox(notice);
        }
        //if not, call method that handles NPC's reaction (+etc.)
        else
        {
            TriggerTicket(__instance, who, days);
        }

        __result = true;
        return false;
    }

    /// <summary>
    /// Depending on the NPC, causes actions.
    /// </summary>
    /// <param name="receiver"></param>
    /// <param name="giver"></param>
    /// <param name="isDayTicket"></param>
    private static void TriggerTicket(NPC receiver, Farmer giver, int days)
    {
        var npcdata = giver.friendshipData[receiver.Name];

        if (npcdata.IsMarried() || npcdata.IsRoommate())
        {
            //var scheduledWeek = isDayTicket && giver.mailReceived.Contains("VisitTicket_week");
            //var scheduledDay = !isDayTicket && giver.mailReceived.Contains("VisitTicket_day");

            //if already invited
            if (ModEntry.Status is not null && ModEntry.Status.Any() && ModEntry.Status.ContainsKey(receiver.Name))
            {
                //tell player about it
                var alreadyinvited = string.Format(Translate("AlreadyInvited"), receiver.displayName);
                Game1.addHUDMessage(new HUDMessage(alreadyinvited, HUDMessage.error_type));
            }
            else
            {
                Draw(receiver, GetInviteDialogue(receiver));

                //user will always have data in Status (created during SaveLoadedBasicInfo).
                //so there's no worry about possible nulls
                ModEntry.Status ??= new Dictionary<string, (int, bool)>();
                ModEntry.Status?.Add(receiver.Name, (days, days == 1));
                giver.mailReceived.Add(days == 1 ? "VisitTicket_day" : "VisitTicket_week");
                giver.reduceActiveItemByOne();
            }
        }
        else if (receiver.Name == "Willy" && (giver.currentLocation.Name == "Beach" || giver.currentLocation.Name == "FishShop"))
        {
            var willytext = Translate("Willy.IslandTicket");
            Draw(receiver, willytext);

            var yn = giver.currentLocation.createYesNoResponses();

            giver.currentLocation.createQuestionDialogue(
                question: Translate("IslandVisit.Question"),
                answerChoices: yn,
                afterDialogueBehavior: HandleIslandEvent);
        }
        else
        {
            //send rejection dialogue like the pendant one
            Game1.drawObjectDialogue(Game1.parseText(Game1.content.LoadString("Strings\\Characters:MovieInvite_NoTheater", receiver.displayName)));
        }
    }

    /// <summary>
    /// Tries to get our specific mod data.
    /// </summary>
    /// <param name="obj">The object.</param>
    /// <param name="days">How many days the visit lasts.</param>
    /// <returns>Whether the item is an island ticket.</returns>
    private static bool TryGetCustomFields(Object obj, out int days)
    {
        if (obj == null)
        {
            days = -1;
            return false;
        }

        if (Game1.objectData.TryGetValue(obj.ItemId, out var data) == false || data.CustomFields is null ||
            data.CustomFields.Any() == false)
        {
            days = -1;
            return false;
        }

        if(data.CustomFields.TryGetValue($"{ModEntry.Id}_Days", out var howMany) == false)
        {
            days = -1;
            return false;
        }
        
        return int.TryParse(howMany, out days);
    }

    private static void HandleIslandEvent(Farmer who, string whichAnswer)
    {
        if (whichAnswer == "No")
        {
            string rejected = Translate("IslandVisit.Rejected");
            Game1.drawObjectDialogue(rejected);
            return;
        }

        var willy = Utility.fuzzyCharacterSearch("Willy");
        willy.jump();
        who.reduceActiveItemByOne();

        if (Game1.random.NextDouble() < 0.2)
        {
            var aboveHead = "Strings\\Locations:BoatTunnel_willyText_random" + Game1.random.Next(2);
            willy.showTextAboveHead(aboveHead);

            Game1.pauseThenDoFunction(1500, TravelToIsland);
        }
        else
            TravelToIsland();

    }

    private static void TravelToIsland()
    {
        Game1.stats.Increment("boatRidesToIsland");
        Game1.fadeScreenToBlack();
        Game1.warpFarmer("IslandSouth", 21, 43, 0);
    }

    /// <summary>
    /// Get the dialogue for a NPC, depending on name and personality.
    /// </summary>
    /// <param name="who"></param>
    /// <returns>The NPC's reply to being invited (to the island).</returns>
    private static string GetInviteDialogue(NPC who)
    {
        var vanilla = who.Name switch
        {

            "Abigail" => true,
            "Alex" => true,
            "Elliott" => true,
            "Emily" => true,
            "Haley" => true,
            "Harvey" => true,
            "Krobus" => true,
            "Leah" => true,
            "Maru" => true,
            "Penny" => true,
            "Sam" => true,
            "Sebastian" => true,
            "Shane" => true,
            "Claire" => true,
            "Lance" => true,
            "Olivia" => true,
            "Sophia" => true,
            "Victor" => true,
            "Wizard" => true,
            _ => false,
        };

        if (vanilla)
        {
            return Translate($"Invite_{who.Name}");
        }
        else
        {
            var r = Game1.random.Next(1, 4);
            return Translate($"Invite_generic_{who.Optimism}_{r}"); //1 polite, 2 rude, 0 normal?
        }
    }
}