using StardewModdingAPI;
using StardewValley;

namespace NetworkOptimizer
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            LoadConfig();

            helper.ConsoleCommands.Add("networkoptimizer_reloadconfig", "Reload the network config", (a,b) => LoadConfig());
        }

        private void LoadConfig()
        {
            ModConfig config = Helper.ReadConfig<ModConfig>();
            if (config == null)
            {
                config = new ModConfig();
                Helper.WriteConfig(config);
            }

            Multiplayer multiplayer = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue();
            
            Helper.Reflection.GetField<int>(multiplayer, "defaultInterpolationTicks").SetValue(config.DefaultInterpolationTicks);
            Helper.Reflection.GetField<int>(multiplayer, "farmerDeltaBroadcastPeriod").SetValue(config.FarmerDeltaBroadcastPeriod);
            Helper.Reflection.GetField<int>(multiplayer, "locationDeltaBroadcastPeriod").SetValue(config.LocationDeltaBroadcastPeriod);
            Helper.Reflection.GetField<int>(multiplayer, "worldStateDeltaBroadcastPeriod").SetValue(config.WorldStateDeltaBroadcastPeriod);
        }
    }
}
