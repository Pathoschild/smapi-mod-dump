/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/mus-candidus/GoToBed
**
*************************************************/

using System.Linq;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Pathfinding;

using GoToBed.Framework;


namespace GoToBed {
    public class ModEntry : Mod {
        private ModConfig config_;

        public override void Entry(IModHelper helper) {
            config_ = helper.ReadConfig<ModConfig>();

            if (config_.Stardew13SpouseSleep) {
                // Provide StardewValley13 spouse sleeping behavior.
                Stardew13SpouseSleepPatch.Create(this.ModManifest.UniqueID, this.Monitor);
            }

            SpouseBedTimeVerifier spouseBedTime = new SpouseBedTimeVerifier(config_, this.Monitor);
            if (!spouseBedTime.IsDefault) {
                // Set time when your spouse gets up and goes to bed.
                SpouseBedTimePatch.Create(this.ModManifest.UniqueID, this.Monitor, spouseBedTime);
            }

            // Put hat on.
            this.Helper.Events.GameLoop.DayStarted += OnDayStarted;
            // Hook into MenuChanged event to intercept dialogues.
            this.Helper.Events.Display.MenuChanged += OnMenuChanged;
            // Enable controls at the end of day.
            this.Helper.Events.GameLoop.DayEnding += OnDayEndingEnableInput;
        }

        private void OnDayStarted(object sender, DayStartedEventArgs e) {
            // We changed into swimsuit during the night.
            Game1.player.changeOutOfSwimSuit();
        }

        private void OnMenuChanged(object sender, MenuChangedEventArgs e) {
            // Intercept sleep dialogue as suggested by Pathos.
            if (e.NewMenu is DialogueBox dialogue) {
                string text = this.Helper.Reflection.GetField<List<string>>(dialogue, "dialogues").GetValue().FirstOrDefault();
                string sleepText = Game1.content.LoadString("Strings\\Locations:FarmHouse_Bed_GoToSleep");
                if (text == sleepText) {
                    // handle "Go to sleep for the night?" dialogue
                    this.Monitor.Log("Go to bed?", LogLevel.Debug);
                    Game1.player.currentLocation.afterQuestion = GoToBed;
                }
            }
        }

        private void GoToBed(Farmer who, string whichAnswer) {
            if (whichAnswer.Equals("Yes")) {
                this.Monitor.Log($"Farmer {Game1.player.Name} goes to bed", LogLevel.Debug);

                // Only move farmer under the blanket if we are in farm house because
                // the appropriate methods are part of class FarmHouse .
                // Note that iterating over furniture would be possible but can't be done
                // in a backward compatible way (NetCollection requires recompilation in 1.5).
                FarmHouse farmHouse = null;
                if (Game1.player.currentLocation is FarmHouse) {
                    farmHouse = Game1.player.currentLocation as FarmHouse;

                    Game1.player.position.Y = farmHouse.GetPlayerBedSpot().Y * 64f + 24f;
                }

                // Take hat off. We could check for a hat and store it in the inventory
                // overnight but this requires a free inventory slot and causes its own
                // problems (you always have to put your hat on if you load a game...)
                // so we move the farmer under the blanket and change into swimsuit.
                // Simple and reliable.
                Game1.player.changeIntoSwimsuit();

                // Player is not married or spouse already went to bed or current location is not farm house.
                if (!Game1.player.isMarriedOrRoommates() || Game1.timeOfDay > config_.SpouseGoToBedTime || farmHouse == null) {
                    FarmerSleep();

                    return;
                }

                // If spouse isn't in the farm house player has to sleep alone.
                NPC spouse = Game1.player.getSpouse();
                if (spouse.currentLocation != farmHouse) {
                    this.Monitor.Log($"Spouse {spouse.Name} isn't in the farm house", LogLevel.Info);

                    FarmerSleep();

                    return;
                }

                // Disable player movement so spouse can finish his/her path to bed.
                this.Helper.Events.Input.ButtonPressed += OnButtonPressedDisableInput;

                // Spouse goes to bed.
                this.Monitor.Log($"Spouse {spouse.Name} goes to bed", LogLevel.Debug);

                spouse.controller = null;
                spouse.temporaryController =
                    new PathFindController(
                        spouse,
                        farmHouse,
                        farmHouse.getSpouseBedSpot(spouse.Name),
                        0,
                        (c, location) => {
                            c.doEmote(Character.sleepEmote);
                            FarmHouse.spouseSleepEndFunction(c, location);

                            // Player can rest assured.
                            FarmerSleep();
                        });

                if (spouse.temporaryController.pathToEndPoint == null) {
                    this.Monitor.Log($"Spouse {spouse.Name} can't reach bed", LogLevel.Warn);

                    FarmerSleep();
                }
            }
        }

        private void OnButtonPressedDisableInput(object sender, ButtonPressedEventArgs e) {
            // The button has not processed by the game yet so we can suppress it now.
            this.Helper.Input.Suppress(e.Button);
            // Menu buttons immediately send player to sleep without waiting for spouse.
            // This should allow us to recover from errors.
            // Note that keyboard buttons are configurable but controller buttons are hardcoded.
            if (Game1.options.menuButton.Any(button => button.ToSButton() == e.Button)
             || e.Button == SButton.ControllerStart
             || e.Button == SButton.ControllerB
             || e.Button == SButton.ControllerY) {
                FarmerSleep();
            }
            else {
                this.Monitor.Log("Press menu button or <ESC> to fall asleep immediately", LogLevel.Info);
            }
        }

        private void OnDayEndingEnableInput(object sender, DayEndingEventArgs e) {
            // If the handler wasn't attached nothing will happen here.
            this.Helper.Events.Input.ButtonPressed -= OnButtonPressedDisableInput;
            // Enable all buttons.
            this.Helper.Input.Suppress(SButton.None);
        }

        private void FarmerSleep() {
            // Call the appropriate private method.
            this.Helper.Reflection.GetMethod(Game1.player.currentLocation, "startSleep").Invoke();
        }
    }
}
