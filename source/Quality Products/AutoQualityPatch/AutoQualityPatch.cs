using Pathoschild.Stardew.Automate;
using StardewModdingAPI;
using StardewModdingAPI.Events;

namespace AutoQualityPatch
{
    internal class AutoQualityPatch : Mod
    {
        public override void Entry(IModHelper helper)
        {
            Helper.Events.GameLoop.GameLaunched += OnModLoaded;
        }

        private void OnModLoaded(object sender, GameLaunchedEventArgs e)
        {
            IAutomateAPI automateAPI = Helper.ModRegistry.GetApi<IAutomateAPI>("Pathoschild.Automate");
            automateAPI.AddFactory(new ProcessorFactory());
        }
    }
}
