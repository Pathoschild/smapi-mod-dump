using Harmony;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NpcAdventure.Patches
{
    class GameLocationPerformActionPatch : Patch<GameLocationPerformActionPatch>
    {
        public override string Name => nameof(GameLocationPerformActionPatch);

        private CompanionManager Manager { get; set; }

        public GameLocationPerformActionPatch(CompanionManager manager)
        {
            this.Manager = manager;
            Instance = this;
        }

        private static bool Before_performAction(GameLocation __instance, string action, Farmer who)
        {
            try
            {
                if (action != null && who.IsLocalPlayer)
                {
                    var recruitedCsm = Instance.Manager
                        .PossibleCompanions
                        .Values
                        .FirstOrDefault(csm => csm.CurrentStateFlag == StateMachine.CompanionStateMachine.StateFlag.RECRUITED);
                    string[] strArray = action.Split(' ');
                    string s = strArray[0];

                    if (recruitedCsm != null && s == "Message")
                    {
                        var key = action.Substring(action.IndexOf('"'))?.Replace("\"", "");

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

        protected override void Apply(HarmonyInstance harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(GameLocation), nameof(GameLocation.performAction)),
                prefix: new HarmonyMethod(typeof(GameLocationPerformActionPatch), nameof(GameLocationPerformActionPatch.Before_performAction))
            );
        }
    }
}
