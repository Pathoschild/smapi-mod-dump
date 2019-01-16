
namespace FollowerNPC.CompanionStates
{
    class UnavailableState: CompanionState
    {
        public UnavailableState(CompanionStateMachine sm) : base(sm)
        {

        }

        public override void EnterState()
        {
            base.EnterState();

            ModEntry.modHelper.Events.GameLoop.DayEnding += GameLoop_DayEnding;
        }

        public override void ExitState()
        {
            base.ExitState();

            ModEntry.modHelper.Events.GameLoop.DayEnding -= GameLoop_DayEnding;
        }

        private void GameLoop_DayEnding(object sender, StardewModdingAPI.Events.DayEndingEventArgs e)
        {
            stateMachine.NewDaySetup();
        }
    }
}
