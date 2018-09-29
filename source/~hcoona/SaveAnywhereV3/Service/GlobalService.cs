using SaveAnywhereV3.DataContract;
using StardewValley;

namespace SaveAnywhereV3.Service
{
    public class GlobalService : SaveLoadServiceBase<GlobalInfo>
    {
        public GlobalService()
            : base(model => model.GlobalInfo)
        { }

        protected override void DoLoad(GlobalInfo model)
        {
            Game1.timeOfDay = model.TimeOfDay;

            if (Game1.questOfTheDay != null && model.DailyQuestAcceptance.HasValue)
            {
                Game1.questOfTheDay.accepted = model.DailyQuestAcceptance.Value;
            }
        }

        protected override GlobalInfo DumpSaveModel()
        {
            return new GlobalInfo
            {
                TimeOfDay = Game1.timeOfDay,
                DailyQuestAcceptance = Game1.questOfTheDay?.accepted
            };
        }
    }
}
