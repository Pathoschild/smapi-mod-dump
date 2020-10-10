/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using Harmony;
using PurrplingCore.Patching;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NpcAdventure.Patches
{
    class GameLocationPerformActionPatch : Patch<GameLocationPerformActionPatch>
    {
        public override string Name => nameof(GameLocationPerformActionPatch);

        private CompanionManager Manager { get; set; }
        private bool OverriedDoorLock { get; }

        public GameLocationPerformActionPatch(CompanionManager manager, bool overrideDoorLock)
        {
            this.Manager = manager;
            this.OverriedDoorLock = overrideDoorLock;
            Instance = this;
        }

        private static bool Before_performAction(GameLocation __instance, string action, Farmer who)
        {
            try
            {
                if (action != null && who.IsLocalPlayer && action.StartsWith("Message "))
                {
                    var recruitedCsm = Instance.Manager
                        .PossibleCompanions
                        .Values
                        .FirstOrDefault(csm => csm.CurrentStateFlag == StateMachine.CompanionStateMachine.StateFlag.RECRUITED);
                    string[] strArray = action.Split(' ');
                    string s = strArray[0];

                    if (recruitedCsm != null && strArray.Length >= 2 && s == "Message")
                    {
                        var key = strArray[1].Replace("\"", "");

                        if (key != null && recruitedCsm.Dialogues.GetRawDialogue($"companionReaction_{key}", out var dialogue))
                        {
                            // If there are exist any reaction dialogue for this tile message action,
                            // override the explore message with companion's dialogue
                            recruitedCsm.Companion.setNewDialogue(dialogue.Value);
                            Game1.drawDialogue(recruitedCsm.Companion);

                            return false;
                        }
                    }
                }

                return true;

            } catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Before_performAction));
                return true;
            }
        }

        private static bool Before_lockedDoorWarp(GameLocation __instance, string[] actionParams)
        {
            try
            {
                var recruitedCsm = Instance.Manager
                    .PossibleCompanions
                    .Values
                    .FirstOrDefault(csm => csm.CurrentStateFlag == StateMachine.CompanionStateMachine.StateFlag.RECRUITED);

                if (recruitedCsm != null)
                {
                    NPC npc = recruitedCsm.Companion;
                    Dictionary<string, string> dictionary = Game1.content.Load<Dictionary<string, string>>("Data\\NPCDispositions");
                    
                    if (!dictionary.ContainsKey(npc.Name))
                        return true;

                    string pureDefaultMap = dictionary[npc.Name].Split('/')[10].Split(' ')[0];
                    if (actionParams[3] == pureDefaultMap)
                    {
                        Rumble.rumble(0.15f, 200f);
                        Game1.player.completelyStopAnimatingOrDoingAction();
                        __instance.playSoundAt("doorClose", Game1.player.getTileLocation());
                        Game1.warpFarmer(actionParams[3], Convert.ToInt32(actionParams[1]), Convert.ToInt32(actionParams[2]), false);

                        return false;
                    }
                }

                return true;

            }
            catch (Exception e)
            {
                Instance.LogFailure(e, nameof(Before_lockedDoorWarp));
                return true;
            }
        }

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(GameLocationPerformActionPatch), nameof(GameLocationPerformActionPatch.Before_performAction))
            );

            if (this.OverriedDoorLock)
            {
                harmony.Patch(
                    original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.lockedDoorWarp)),
                    prefix: new HarmonyMethod(typeof(GameLocationPerformActionPatch), nameof(GameLocationPerformActionPatch.Before_lockedDoorWarp))
                );
            }
        }
    }
}
