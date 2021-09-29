/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using HarmonyLib;
using NpcAdventure.StateMachine;
using PurrplingCore.Patching;
using StardewValley;
using System;

namespace NpcAdventure.Patches
{
    class GameUseToolPatch : Patch<GameUseToolPatch>
    {
        public override string Name => nameof(GameUseToolPatch);

        private CompanionManager Manager { get; set; }

        public GameUseToolPatch(CompanionManager manager)
        {
            this.Manager = manager;
            Instance = this;
        }

        private static bool Before_pressUseToolButton()
        {
            try
            {
                if (CanUseToolOverCompanion)
                {
                    NPC npc = Game1.currentLocation.doesPositionCollideWithCharacter(
                        Utility.getRectangleCenteredAt(
                            Game1.player.GetToolLocation(false), 64),
                        ignoreMonsters: true);

                    if (npc != null
                        && Instance.Manager.PossibleCompanions.TryGetValue(npc.Name, out CompanionStateMachine csm)
                        && csm.CurrentStateFlag == CompanionStateMachine.StateFlag.RECRUITED)
                    {
                        // Force use tool when player performed use tool and has recruited a companion instead of dialogue
                        // To perform dialogue player must perform it by action button (right-click by default on PC)
                        Game1.player.BeginUsingTool();

                        return false;
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Instance.LogFailure(ex, nameof(Before_pressUseToolButton));
                return true;
            }
        }

        private static bool CanUseToolOverCompanion => !(Game1.player.CurrentTool == null 
            || Game1.player.UsingTool
            || Game1.player.canOnlyWalk
            || Game1.player.isRidingHorse() 
            || Game1.dialogueUp 
            || Game1.eventUp);

        protected override void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Method(typeof(Game1), nameof(Game1.pressUseToolButton)),
                prefix: new HarmonyMethod(typeof(GameUseToolPatch), nameof(GameUseToolPatch.Before_pressUseToolButton))
            );
        }
    }
}
