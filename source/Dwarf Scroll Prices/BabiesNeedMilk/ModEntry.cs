/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/noriteway/StardewMods
**
*************************************************/

using GenericModConfigMenu;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace BabiesNeedMilk
{
    internal sealed class ModEntry : Mod
    {
        private ModConfig? Config;

        public override void Entry(IModHelper helper)
        {
            Config = Helper.ReadConfig<ModConfig>();

            helper.Events.GameLoop.GameLaunched += OnGameLaunched;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnDayStarted(object? sender, DayStartedEventArgs e)
        {
            List<long> nursingParents = new List<long>();

            Farm curFarm = Game1.getFarm();
            foreach(FarmAnimal animal in curFarm.getAllFarmAnimals())
            {
                if(animal.type.Value == "White Cow" || animal.type.Value == "Brown Cow")
                {
                    //Monitor.Log($"Id = {animal.myID} | parent id = {animal.parentId}", LogLevel.Debug);
                    if(!animal.parentId.Equals(-1))
                    {
                        if(animal.GetDaysOwned() < Config.DaysUntilCalfWeaned) nursingParents.Add(animal.parentId.Value);
                    }
                }

                if(animal.type.Value == "Goat")
                {
                    Monitor.Log($"Id = {animal.myID} | parent id = {animal.parentId}", LogLevel.Debug);
                    if(!animal.parentId.Equals(-1))
                    {
                        if(animal.GetDaysOwned() < Config.DaysUntilKidWeaned) nursingParents.Add(animal.parentId.Value);
                    }
                }
            }

            if(nursingParents.Count == 0) return;

            foreach(FarmAnimal animal in curFarm.getAllFarmAnimals())
            {
                if(nursingParents.Contains(animal.myID.Value))
                {
                    Monitor.Log(animal.name.Value + " should be nursing...", LogLevel.Debug);
                    //animal.currentProduce.Set("340");
                    animal.currentProduce.Set(null);
                }
            }
        }

        private void OnGameLaunched(object? sender, GameLaunchedEventArgs e)
        {
            // Get MCM API(if installed)
            var configMenu = Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            // Exit if no config
            if(configMenu is null) return;
            if(Config == null) return;

            // Register mod for MCM
            configMenu.Register(
                mod: ModManifest,
                reset: () => Config = new ModConfig(),
                save: () => Helper.WriteConfig(Config)
            );

            // Add UI for MCM
            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.DaysUntilCalfWeaned,
                setValue: value => Config.DaysUntilCalfWeaned = (int)value,
                name: () => "Days until calf weaned",
                tooltip: () => "Determines when young cows no longer need milk from their parent.",
                min: 1
            );

            configMenu.AddNumberOption(
                mod: ModManifest,
                getValue: () => Config.DaysUntilKidWeaned,
                setValue: value => Config.DaysUntilKidWeaned = (int)value,
                name: () => "Days until kid weaned",
                tooltip: () => "Determines when young goats no longer need milk from their parent.",
                min: 1
            );
        }
    }
}
