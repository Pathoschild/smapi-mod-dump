using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace FollowerNPC
{
    public class ModEntry : Mod
    {

        #region Members and Entry Function
        public static ModConfig config;
        public static IMonitor monitor;
        public static IModHelper modHelper;

        public CompanionsManager companionsManager;

        public override void Entry(IModHelper helper)
        {
            // Initialize variables //
            config = Helper.ReadConfig<ModConfig>();
            monitor = Monitor;
            modHelper = helper;
            companionsManager = new CompanionsManager();
            //**********************//

            // Patch methods //
            HarmonyInstance harmony = HarmonyInstance.Create("Redwood.FollowerNPC");

            Type[] isCollidingPositionTypes0 = new Type[] { typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) };
            Type[] isCollidingPositionTypes1 = new Type[] { typeof(GameLocation), typeof(Rectangle), typeof(xTile.Dimensions.Rectangle), typeof(bool), typeof(int), typeof(bool), typeof(Character), typeof(bool), typeof(bool), typeof(bool) };
            MethodInfo isCollidingPositionOriginal = typeof(GameLocation).GetMethod("isCollidingPosition", isCollidingPositionTypes0);
            MethodInfo isCollidingPositionprefix = typeof(Patches).GetMethod("Prefix", isCollidingPositionTypes1);
            MethodInfo isCollidingPositionpostfix = typeof(Patches).GetMethod("Postfix", isCollidingPositionTypes1);
            harmony.Patch(isCollidingPositionOriginal, new HarmonyMethod(isCollidingPositionprefix), new HarmonyMethod(isCollidingPositionpostfix));

            Type[] updateMovementTypes0 = new Type[] {typeof(GameLocation), typeof(GameTime)};
            Type[] updateMovementTypes1 = new Type[] {typeof(NPC), typeof(GameLocation), typeof(GameTime)};
            MethodInfo updateMovementOriginal = typeof(NPC).GetMethod("updateMovement", updateMovementTypes0);
            MethodInfo updateMovementPrefix = typeof(Patches).GetMethod("Prefix", updateMovementTypes1);
            harmony.Patch(updateMovementOriginal, new HarmonyMethod(updateMovementPrefix), null);
            //**********************//

            // Subscribe to events //
            Helper.Events.Input.ButtonReleased += Input_ButtonReleased;
            //**********************//
        }


        #endregion

        #region Event Functions

        // Just used for debug commands
        private void Input_ButtonReleased(object sender, ButtonReleasedEventArgs e)
        {
            if (!Context.IsWorldReady)
                return;

            if (e.Button == Microsoft.Xna.Framework.Input.Keys.P.ToSButton())
            {
                Netcode.NetCollection<NPC> c = companionsManager.companionAStar.gameLocation.characters;
                foreach (NPC n in c)
                {
                    monitor.Log(n.Name);
                }
            }

            //if (e.Button == Microsoft.Xna.Framework.Input.Keys.L.ToSButton())
            //{
            //    Game1.getCharacterFromName("Penny");
            //}

            //else if (e.Button == Microsoft.Xna.Framework.Input.Keys.K.ToSButton())
            //{
            //    GameLocation gl = companionsManager.farmer.currentLocation;
            //    foreach (Character c in gl.characters)
            //    {
            //        monitor.Log(c.Name);
            //    }
            //}

            //else if (e.KeyPressed == Keys.U && spawned)
            //{
            //    monitor.Log(whiteBox?.currentLocation.Name + " : " + whiteBox?.getTileLocation());
            //}

            //else if (e.KeyPressed == Keys.I)
            //{
            //    monitor.Log(farmer?.currentLocation.Name + " : " + farmer?.getTileLocation());
            //}
        }

        //private void MenuEvents_MenuClosed(object sender, EventArgsClickableMenuClosed e)
        //{
        //    if (e.PriorMenu.GetType() == typeof(DialogueBox))
        //    {
        //        DialogueBox db = (e.PriorMenu as DialogueBox);

        //        Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
        //        if (d != null && d.speaker != null)
        //        {
        //            NPC n = d.speaker;

        //            // Push Companion Ask Dialogue
        //            if (n.CurrentDialogue.Count == 0 && npcsThatCanHangOut.TryGetValue(n.Name, out bool canHangOut) && canHangOut && !(whiteBox != null))
        //            {
        //                responseDialogue = new Dialogue(dialogueScripts[n.Name]["Companion"], n);
        //                yesResponseID = GetYesResponseID(responseDialogue);
        //                n.CurrentDialogue.Push(responseDialogue);
        //                npcsThatCanHangOut[n.Name] = false;
        //            }

        //            // Check Companion Ask Dialogue for answer
        //            else if (d.Equals(responseDialogue))
        //            {
        //                responseDialogue.speaker.CurrentDialogue.Push(new Dialogue(dialogueScripts[n.Name]["Companion"], n));
        //                if (farmer.DialogueQuestionsAnswered.Contains(yesResponseID))
        //                {
        //                    farmer.DialogueQuestionsAnswered.Remove(yesResponseID);

        //                    whiteBox = n;
        //                    whiteBoxAStar = new aStar(farmer.currentLocation, whiteBox.Name);
        //                    companionBuff = CompanionBuff.InitializeBuffFromCompanionName(whiteBox.Name, farmer);
        //                    companionVisitedLocations = new Dictionary<string, bool>();
        //                    Patches.companion = whiteBox;
        //                    whiteBoxAnimationSpeed = 10f;
        //                    whiteBoxFollow = true;
        //                    spawned = true;
        //                    whiteBox.faceTowardFarmerTimer = 0;
        //                }
        //                else if (farmer.dialogueQuestionsAnswered.Contains(yesResponseID + 1))
        //                {
        //                    farmer.dialogueQuestionsAnswered.Remove(yesResponseID + 1);
        //                }
        //                responseDialogue = null;
        //            }
                    
        //            // Check Companion Actions Dialogue
        //            else if (d.Equals(actionDialogue))
        //            {
        //                if (farmer.DialogueQuestionsAnswered.Contains(yesResponseID))
        //                {
        //                    whiteBoxFollow = false;
        //                    spawned = false;
        //                    farmer.DialogueQuestionsAnswered.Remove(yesResponseID);
        //                    whiteBoxAStar = null;
        //                    companionBuff.RemoveAndDisposeCompanionBuff();
        //                    companionBuff = null;

        //                    whiteBox.Schedule = GetWhiteBoxSchedule(Game1.dayOfMonth);
        //                    Game1.fadeScreenToBlack();
        //                    whiteBox.faceTowardFarmerTimer = 0;
        //                    DelayedWarp(whiteBoxScheduleCurrentDestinationLocation,
        //                        whiteBoxScheduleCurrentDestinationPoint, 500, new Action(CompanionEndCleanup));

        //                    foreach (KeyValuePair<string, bool> npcKvP in npcsThatCanHangOut)
        //                    {
        //                        if (npcKvP.Value)
        //                        {
        //                            NPC npc = Game1.getCharacterFromName(npcKvP.Key);
        //                            if (npc.CurrentDialogue.Count == 0)
        //                            {
        //                                Dialogue cRD = new Dialogue(dialogueScripts[npcKvP.Key]["Companion"], npc);
        //                                companionRecruitDialogues[npcKvP.Key] = new RecruitDialogueInfo()
        //                                {
        //                                    recruitDialogue = cRD,
        //                                    yesResponseID = GetYesResponseID(cRD)
        //                                };
        //                                npc.CurrentDialogue.Push(cRD);
        //                            }
        //                            npcsThatCanHangOut[n.Name] = false;
        //                        }
        //                    }
        //                }
        //                else if (farmer.dialogueQuestionsAnswered.Contains(yesResponseID + 1))
        //                {
        //                    farmer.dialogueQuestionsAnswered.Remove(yesResponseID + 1);
        //                    whiteBox.faceTowardFarmerTimer = 0;
        //                }
        //            }

        //            if (whiteBox != null && n.Equals(whiteBox))
        //            {
        //                whiteBox.faceTowardFarmerTimer = 0;
        //                whiteBox.movementPause = 0;
        //            }
        //        }
        //    }
        //}

        #endregion
    }

    /// <summary>
    /// A collection of Harmony patches that allow me to modify, add, or remove certain
    /// functionalities of CA's code to make it work nicely with mine. 
    /// </summary>
    class Patches
    {
        static public NPC companion;

        /// <summary>
        /// A weird, roundabout way of allowing Companions to pass through invisible
        /// barriers that normally block NPC's. Might want to consider making this a
        /// little less, strange(?), in the future.
        /// </summary>
        #region isCollidingPosition
        static public bool flag;

        static public void Prefix(GameLocation __instance, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (companion != null
                && character != null
                && character.Name != null
                && character.Name.Equals(companion.Name)
                && !character.eventActor)
                character.eventActor = flag = true;
        }

        static public void Postfix(GameLocation __instance, Rectangle position, xTile.Dimensions.Rectangle viewport, bool isFarmer, int damagesFarmer, bool glider, Character character, bool pathfinding, bool projectile = false, bool ignoreCharacterRequirement = false)
        {
            if (flag)
                character.eventActor = flag = false;
        }
        #endregion

        /// <summary>
        /// The Companion's daily schedule is remade from scratch once they are dismissed.
        /// This pops off any routes from their schedule that are in the past until it reaches
        /// the most recent route they would have traveled. It then pops off that route and stores
        /// it as the location they will be warped to after they are finished being dismissed.
        /// </summary>
        #region checkSchedule
        static public Point scheduleCurrentDestination;

        static public void Postfix(NPC __instance, int timeOfDay)
        {
            if (companion != null && companion == __instance)
            {
                SchedulePathDescription spd;
                if (__instance.Schedule.TryGetValue(timeOfDay, out spd) && spd != null)
                {
                    while (spd.route.Count != 0)
                        scheduleCurrentDestination = spd.route.Pop();
                }
            }
        }
        #endregion

        /// <summary>
        /// Prevents the Companion from updating their movement via CA's movementUpdate function
        /// while they are the farmer's companion.
        /// </summary>
        #region updateMovement
        static public bool Prefix(NPC __instance, GameLocation location, GameTime time)
        {
            return !(companion != null) && __instance.Equals(companion);
        }
        #endregion
    }

    /// <summary>
    /// Harmony patche(s) that I use(d) for debug purposes. These are not (or should not) be
    /// used in any release build of this mod.
    /// </summary>
    class DebugPatches
    {
        static public NPC testPC;
        static public bool debugging = false;

        static public bool Prefix(NPC __instance, int timeOfDay)
        {
            if (debugging && __instance.Name.Equals("Penny"))
            {
                debugCheckSchedule(Game1.timeOfDay);
                return false;
            }
            return true;
        }

        public static void debug(NPC n)
        {
            if (n != null)
            {
                testPC = n;
                debugging = true;
            }
            else
            {
                testPC = null;
                debugging = false;
            }
        }

        public static void debugCheckSchedule(int timeOfDay)
        {
            testPC.updatedDialogueYet = false;
            typeof(NPC).GetField("extraDialogueMessageToAddThisMorning", BindingFlags.NonPublic | BindingFlags.Instance)
                .SetValue(testPC, null);
            //testPC.extraDialogueMessageToAddThisMorning = null;
            if (testPC.ignoreScheduleToday)
            {
                return;
            }
            if (testPC.Schedule != null)
            {
                SchedulePathDescription possibleNewDirections;
                int time2Try = (int)typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(testPC);
                testPC.Schedule.TryGetValue((time2Try == 9999999) ? timeOfDay : time2Try, out possibleNewDirections);
                if (possibleNewDirections != null)
                {
                    bool walkingInSquare = (bool)typeof(NPC).GetField("isWalkingInSquare", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(testPC);
                    Rectangle lastCrossroad = (Rectangle)typeof(NPC).GetField("lastCrossroad", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(testPC);
                    Point prevEndPoint = (Point)typeof(NPC).GetField("previousEndPoint", BindingFlags.NonPublic | BindingFlags.Instance)
                        .GetValue(testPC);
                    if (!testPC.isMarried() && (!walkingInSquare || (lastCrossroad.Center.X / 64 != prevEndPoint.X && lastCrossroad.Y / 64 != prevEndPoint.Y)))
                    {
                        Point arg_A0_0 = prevEndPoint;
                        if (!prevEndPoint.Equals(Point.Zero) && !prevEndPoint.Equals(testPC.getTileLocationPoint()))
                        {
                            if (time2Try == 9999999)
                            {
                                typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                                    .SetValue(testPC, timeOfDay);
                                return;
                            }
                            return;
                        }
                    }

                    FieldInfo d2nL =
                        typeof(NPC).GetField("directionsToNewLocation",
                            BindingFlags.NonPublic | BindingFlags.Instance);
                    d2nL.SetValue(testPC, possibleNewDirections);
                    typeof(NPC).GetMethod("prepareToDisembarkOnNewSchedulePath",
                        BindingFlags.NonPublic | BindingFlags.Instance).Invoke(testPC, null);
                    if (testPC.Schedule == null)
                    {
                        return;
                    }

                    SchedulePathDescription d2nLValue = (SchedulePathDescription)d2nL.GetValue(testPC);
                    if (d2nLValue != null && d2nLValue.route != null && d2nLValue.route.Count > 0 && (Math.Abs(testPC.getTileLocationPoint().X - d2nLValue.route.Peek().X) > 1 || Math.Abs(testPC.getTileLocationPoint().Y - d2nLValue.route.Peek().Y) > 1) && testPC.temporaryController == null)
                    {
                        typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(testPC, 9999999);
                        return;
                    }
                    object[] parameters = new object[] { d2nLValue.endOfRouteBehavior, d2nLValue.endOfRouteMessage };
                    testPC.controller = new PathFindController(d2nLValue.route, testPC, Utility.getGameLocationOfCharacter(testPC))
                    {
                        finalFacingDirection = d2nLValue.facingDirection,
                        endBehaviorFunction = (PathFindController.endBehavior)typeof(NPC).GetMethod("getRouteEndBehaviorFunction", BindingFlags.NonPublic | BindingFlags.Instance).Invoke(testPC, parameters)
                    };
                    typeof(NPC).GetField("scheduleTimeToTry", BindingFlags.NonPublic | BindingFlags.Instance)
                        .SetValue(testPC, 9999999);
                    if (d2nLValue != null && d2nLValue.route != null)
                    {
                        typeof(NPC).GetField("previousEndPoint", BindingFlags.NonPublic | BindingFlags.Instance)
                            .SetValue(testPC, ((d2nLValue.route.Count > 0) ? d2nLValue.route.Last() : Point.Zero));
                    }
                }
            }
        }
    }

}

