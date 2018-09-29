﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace WarpAnimals {
    public class ModEntry : Mod {
        private ModConfig config;

        public override void Entry(IModHelper helper) {
            this.config = helper.ReadConfig<ModConfig>();

            if (config.WarpKey != null) {
                ControlEvents.KeyPressed += this.ControlEvents_KeyPress;
            }
            if (config.WarpButton != null) {
                ControlEvents.ControllerButtonPressed += (sender, evt) => {
                    this.GamepadButtonPress(evt.ButtonPressed);
                };
                ControlEvents.ControllerTriggerPressed += (sender, evt) => {
                    this.GamepadButtonPress(evt.ButtonPressed);
                };
            }

            helper.ConsoleCommands.Add(
                "warp_animals",
                "Warp wandering animals into their respective home.",
                this.ConsoleCommand_WarpAnimals
            );
        }

        private void GamepadButtonPress(Buttons button) {
            this.Monitor.Log($"Button press: {button}");
            if (Context.IsWorldReady) { // save is loaded
                if (button == config.WarpButton) {
                    WarpAllAnimalsHome();
                }
            }
        }

        private void ControlEvents_KeyPress(object sender, EventArgsKeyPressed e) {
            if (Context.IsWorldReady) { // save is loaded
                if (e.KeyPressed == config.WarpKey) {
                    WarpAllAnimalsHome();
                }
            }
        }

        private void ConsoleCommand_WarpAnimals(string command, string[] args) {
            this.WarpAllAnimalsHome();
        }

        private void WarpAllAnimalsHome() {
            Farm farm = Game1.getFarm();
            var animalsOutside = new List<FarmAnimal>(farm.animals.Values);
            this.Monitor.Log($"Warp {animalsOutside.Count} animals home.", LogLevel.Info);
            animalsOutside.ForEach(animal => {
                animal.warpHome(farm, animal);
            });
        }
    }
}
