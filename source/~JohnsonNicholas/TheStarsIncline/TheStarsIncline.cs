using StardewModdingAPI;

namespace TwilightShards.TheStarsIncline
{
    public class TheStarsIncline : Mod
    {
        private readonly int UniqueBaseID = 49134570;

        private AstrologicalSigns ZodiacData;
        //It's the age of aquarius. The age of aquarius~~~

        public override void Entry(IModHelper helper)
        {
            //woo!
            Helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
            var ZodiacData = new AstrologicalSigns();
            
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}
