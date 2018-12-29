using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Buildings;

namespace HuggableScarecrows
{
    public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.DayStarted += GameLoop_DayStarted;
        }

        private void GameLoop_DayStarted(object sender, StardewModdingAPI.Events.DayStartedEventArgs e)
        {
            foreach (Building building in Game1.getFarm().buildings)
            {
                this.Monitor.VerboseLog($"Checking Building {building.buildingType.Value}");
                if (building is Coop || building is Barn)
                {
                    if (building.indoors.Value is AnimalHouse indoors && indoors.Objects.Values.Any(p => p.Name.Contains("arecrow")))
                    {
                        foreach (FarmAnimal animal in indoors.animals.Values)
                        {
                            animal.pet(Game1.MasterPlayer);
                            this.Monitor.VerboseLog($"   Petting {animal.Name}");
                        }
                    }
                }
            }
        }
    }
}
