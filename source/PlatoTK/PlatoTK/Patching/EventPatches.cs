/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/PlatoTK
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace PlatoTK.Patching
{
    internal class EventPatches
    {
        const string PageKey = "@PlatoGotoPage_";

        internal static bool _patched = false;
        internal static bool _isTV = false;
        internal static int _responsesPerPage => (Constants.TargetPlatform == GamePlatform.Android) ? 3 : 8;

        internal static DialogueCall LastPaginatedDialogue;

        internal static IPlatoHelper Plato { get; set; }

        internal static void InitializePatch(IPlatoHelper helper)
        {
            Plato = helper;
            if (_patched)
                return;

            _patched = true;
            var questionRaised = AccessTools.DeclaredConstructor(typeof(DialogueBox),new Type[] { typeof(string), typeof(List<Response>), typeof(int) });
            
            List<Type> questionLocationTypes = new List<Type>() { 
                typeof(GameLocation), 
                typeof(BusStop), 
                typeof(Desert), 
                typeof(JojaMart)};


            var channelSelected = AccessTools.DeclaredMethod(typeof(TV), "selectChannel");
            var tvAction = AccessTools.DeclaredMethod(typeof(TV), "checkForAction");

            List<MethodInfo> questionAsked = new List<MethodInfo>(questionLocationTypes.Select(t => AccessTools.Method(t, "answerDialogue")));

            var performTouchAction = new[] { 
                AccessTools.DeclaredMethod(typeof(GameLocation), "performTouchAction"), 
                AccessTools.DeclaredMethod(typeof(MovieTheater), "performTouchAction"), 
                AccessTools.DeclaredMethod(typeof(Desert), "performTouchAction") };

            var performAction = new[] {
                AccessTools.DeclaredMethod(typeof(GameLocation), "performAction"),
                AccessTools.DeclaredMethod(typeof(MovieTheater), "performAction"),
                AccessTools.DeclaredMethod(typeof(CommunityCenter), "performAction"),
                AccessTools.DeclaredMethod(typeof(FarmHouse), "performAction"),
                AccessTools.DeclaredMethod(typeof(ManorHouse), "performAction"),
                AccessTools.DeclaredMethod(typeof(LibraryMuseum), "performAction"),
                AccessTools.DeclaredMethod(typeof(Town), "performAction"),
            };

            var harmony = HarmonyInstance.Create($"Plato.QuestionPatches");
            harmony.Patch(questionRaised, prefix: new HarmonyMethod(AccessTools.Method(typeof(EventPatches), nameof(DialogueBox))));
            
            foreach(var method in questionAsked)
                harmony.Patch(method, prefix: new HarmonyMethod(
                    AccessTools.DeclaredMethod(typeof(EventPatches), 
                    nameof(QuestionAsked),
                    null, new Type[] { method.DeclaringType })));

            harmony.Patch(channelSelected, prefix: new HarmonyMethod(AccessTools.Method(typeof(EventPatches), nameof(SelectChannel))));
            harmony.Patch(tvAction, prefix: new HarmonyMethod(AccessTools.Method(typeof(EventPatches), nameof(SetIsTv))));
            harmony.Patch(tvAction, postfix: new HarmonyMethod(AccessTools.Method(typeof(EventPatches), nameof(UnsetIsTV))));
        
            harmony.Patch(AccessTools.Method(typeof(Event), nameof(Event.tryEventCommand)),
                new HarmonyMethod(typeof(EventPatches), nameof(TryEventCommandPre)));

            harmony.Patch(AccessTools.Method(typeof(Event), nameof(Event.tryEventCommand)), null,
            new HarmonyMethod(typeof(EventPatches), nameof(TryEventCommandPost)));

            foreach (var ta in performTouchAction)
                harmony.Patch(ta, prefix: new HarmonyMethod(AccessTools.Method(typeof(EventPatches), nameof(PerformTouchAction))));
            foreach (var a in performAction)
                harmony.Patch(a, prefix: new HarmonyMethod(AccessTools.Method(typeof(EventPatches), nameof(PerformAction))));

            var checkEventPreconditions = AccessTools.Method(typeof(GameLocation), "checkEventPrecondition");
            harmony.Patch(checkEventPreconditions, prefix: new HarmonyMethod(typeof(EventPatches), nameof(CheckEventConditions)));
        }

        internal static bool CheckEventConditions(ref string precondition, ref int __result)
        {
            if (precondition.StartsWith("9999999/"))
                return true;

            string[] conditions = precondition.Split('/').ToArray();
            string t = "l PlatoTK.NoFlag";
            if (Game1.MasterPlayer.mailReceived.Contains("PlatoTK.NoFlag"))
                Game1.MasterPlayer.mailReceived.Remove("PlatoTK.NoFlag");
            
            for (int i = 1; i < conditions.Length; i++)
                if (PlatoHelper.TryCheckConditions(conditions[i], Game1.currentLocation, out bool result,false))
                    if(result)
                        conditions[i] = t;
                    else
                    {
                        __result = -1;
                        return false;
                    }

            precondition = string.Join("/", conditions);
            return true;
        }

        internal static bool PerformTouchAction(GameLocation __instance, string fullActionString, Vector2 playerStandingPosition)
        {
            bool result = true;

            if (fullActionString.Contains(';'))
            {
                foreach (var a in fullActionString.Split(';'))
                    __instance.performTouchAction(a.Trim(), playerStandingPosition);

                return false;
            }

            Plato.Events.HandleTileAction(fullActionString.Split(' '), Game1.player, __instance, "Back", new Point((int)Game1.player.getTileX(), (int)Game1.player.getTileY()), (b) => result = !b);
            return result;
        }

        internal static bool PerformAction(GameLocation __instance, string action, Farmer who, xTile.Dimensions.Location tileLocation, ref bool __result)
        {
            bool result = true;
            bool returnValue = false;

            if (action.Contains(';'))
            {
                foreach (var a in action.Split(';'))
                    returnValue = returnValue || __instance.performAction(a, who, tileLocation);

                return false;
            }

            Plato.Events.HandleTileAction(action.Split(' '), who, __instance, "Buildings", new Point(tileLocation.X, tileLocation.Y), (b) =>
            {
                result = !b;
                returnValue = true;
            });

            if(returnValue)
                __result = returnValue;

            return result;
        }

        internal static bool TryEventCommandPre(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            if (split.Length == 0)
                return true;

            bool result = true;
            Plato.Events.HandleEventCommand(split, __instance, time, location, () => result = false, false);
            return result;
        }

        internal static void TryEventCommandPost(Event __instance, GameLocation location, GameTime time, string[] split)
        {
            if (split.Length == 0)
                return;

            Plato.Events.HandleEventCommand(split, __instance, time, location, null, true);
        }

        internal static void SetIsTv()
        {
            _isTV = true;
        }

        internal static void UnsetIsTV()
        {
            _isTV = false;
        }

        internal static bool SelectChannel(TV __instance, string answer)
        {
            bool result = true;
            Plato.Events.HandleChannelSelection(answer, __instance, () => result = false);
            return result;
        }

        internal static void DialogueBox(ref string dialogue, ref List<Response> responses, int width)
        {
            LastPaginatedDialogue = null;

            int currentPage = 0;
            bool paginate = false;
            string question = dialogue;
            List<Response> choices = new List<Response>();
            choices.AddRange(responses);
            Response leave = choices.FirstOrDefault(c => c.responseKey == "(Leave)");
            if (choices.FirstOrDefault(c => c.responseKey.StartsWith(PageKey)) is Response next) {
                currentPage = int.Parse(next.responseKey.Split('_')[1]);
                choices.Remove(next);
                paginate = true;
            }

            if (leave != null)
                choices.Remove(leave);

            if(!paginate)
                Plato.Events.HandleQuestion(question, choices, (q) => question = q, (r) =>
             {
                 if (!choices.Contains(r))
                     choices.Add(r);
             }, (r) =>
             {
                 choices.Remove(r);
             }, _isTV, () => paginate = true);

            if (leave != null)
                choices.Add(leave);

            if (paginate)
            {
                int startIndex = currentPage * _responsesPerPage;
                int nextPage = currentPage + 1;
                if ((nextPage * _responsesPerPage) >= choices.Count)
                    nextPage = 0;

                List<Response> allChoices = new List<Response>();
                allChoices.AddRange(choices);

                choices = allChoices.GetRange(startIndex, Math.Min(_responsesPerPage, allChoices.Count - startIndex));

                if (currentPage != nextPage)
                {
                    var nextChoice = new Response(PageKey + nextPage, "...");
                    choices.Add(nextChoice);
                    allChoices.Add(nextChoice);
                    if (choices.Contains(leave))
                    {
                        choices.Remove(leave);
                        choices.Add(leave);
                    }
                }

                LastPaginatedDialogue = new DialogueCall(dialogue, allChoices, width);
            }

            dialogue = question;
            responses = choices;
        }

        internal static bool QuestionAsked<T>(T __instance, Response answer)
        {
            bool result = true;

            if (answer.responseKey.StartsWith(PageKey))
            {
                LastPaginatedDialogue.OpenDialogue();
                return false;
            }

            if (__instance is GameLocation location)
            {
                Plato.Events.HandleAnswer(answer, () => result = false, location.lastQuestionKey);

                if (!result)
                {
                    location.lastQuestionKey = null;
                    location.afterQuestion = null;
                }
            }

            return result;
        }

    }

    internal class DialogueCall
    {
        internal readonly string Dialogue;
        internal readonly List<Response> Responses;
        internal readonly int Width;

        public DialogueCall(string dialogue, List<Response> responses, int width)
        {
            Dialogue = dialogue;
            Responses = responses;
            Width = width;
        }

        public void OpenDialogue()
        {
            Game1.activeClickableMenu = new DialogueBox(Dialogue, Responses, Width);
        }
    }

}
