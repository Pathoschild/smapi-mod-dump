using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;

namespace WarpAnimals
{
    public class ModEntry : Mod {
        private ModConfig config;

        public override void Entry(IModHelper helper) {
            config = helper.ReadConfig<ModConfig>();
            Helper.Events.Input.ButtonPressed += ControlEvents_KeyPress;

            helper.ConsoleCommands.Add(
                "warp_animals",
                "Warp wandering animals into their respective home.",
                ConsoleCommand_WarpAnimals
            );
        }

        private void ControlEvents_KeyPress(object sender, ButtonPressedEventArgs e) {
            if (Context.IsWorldReady) { // save is loaded
                if (e.Button == config.WarpKey) {
                    WarpAllAnimalsHome();
                }
            }
        }

        private void ConsoleCommand_WarpAnimals(string command, string[] args) {
            WarpAllAnimalsHome();
        }

        private void WarpAllAnimalsHome() {
            Farm farm = Game1.getFarm();
            var animalsOutside = new List<FarmAnimal>(farm.animals.Values);
            Monitor.Log($"Warp {animalsOutside.Count} animals home.", LogLevel.Info);
            animalsOutside.ForEach(animal => {
                animal.warpHome(farm, animal);
            });
        }
    }
}
