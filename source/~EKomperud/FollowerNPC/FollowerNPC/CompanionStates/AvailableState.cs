using System;
using System.Reflection;
using StardewValley;
using StardewValley.Menus;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace FollowerNPC.CompanionStates
{
    class AvailableState: CompanionState
    {
        public AvailableState(CompanionStateMachine sm) : base(sm)
        {

        }

        public override void EnterState()
        {
            base.EnterState();
            ModEntry.modHelper.Events.Display.MenuChanged += Display_MenuChanged;
            ModEntry.modHelper.Events.GameLoop.TimeChanged += GameLoop_TimeChanged; ;
            ModEntry.modHelper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
        }

        public override void ExitState()
        {
            base.ExitState();
            IModHelper mh = ModEntry.modHelper;
            ModEntry.modHelper.Events.Display.MenuChanged -= Display_MenuChanged;
            ModEntry.modHelper.Events.GameLoop.TimeChanged -= GameLoop_TimeChanged;
            ModEntry.modHelper.Events.GameLoop.DayEnding -= GameLoop_DayEnding;
        }

        private void Display_MenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu != null)
            {
                DialogueBox db = (e.OldMenu as DialogueBox);
                if (db != null)
                {
                    Dialogue d = (Dialogue)typeof(DialogueBox).GetField("characterDialogue", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(db);
                    if (d != null && d.speaker != null && d.speaker.Equals(stateMachine.companion) &&
                        d.speaker.CurrentDialogue.Count == 0 &&
                        Game1.timeOfDay < 2200 &&
                        stateMachine.manager.currentCompanion == null &&
                        stateMachine.manager.farmer.getFriendshipHeartLevelForNPC(d.speaker.Name) >= ModEntry.config.heartThreshold)
                    {
                        stateMachine.MakeRecruitable();
                    }

                }
            }
        }

        private void GameLoop_TimeChanged(object sender, TimeChangedEventArgs e)
        {
            if (stateMachine.companion.CurrentDialogue.Count == 0 &&
                stateMachine.manager.currentCompanion == null && 
                Game1.timeOfDay < 2200 &
                stateMachine.manager.farmer.getFriendshipHeartLevelForNPC(stateMachine.companion.Name) >= ModEntry.config.heartThreshold)
            {
                stateMachine.MakeRecruitable();
            }
        }

        private void GameLoop_DayEnding(object sender, DayEndingEventArgs e)
        {
            stateMachine.NewDaySetup();
        }

    }
}
