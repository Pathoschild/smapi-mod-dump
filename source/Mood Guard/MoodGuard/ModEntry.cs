using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Input;
using System;
using System.Linq;
using System.Collections.Generic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Menus;

namespace MoodGuard
{
    class ModEntry : Mod
    {
        public static ModConfig Config;

        public Dictionary<long, byte> happinessMap;

        enum NightFixMode: int
        {
            Standard = 0,
            Increased = 1,
            Maximized = 2
        }

        NightFixMode nightFixMode;

        public override void Entry(IModHelper helper)
        {
            Config = helper.ReadConfig<ModConfig>();

            // Initialize happiness dictionary
            if (Config.NightFix.Enabled)
            {
                switch (Config.NightFix.Mode)
                {
                    case "Increased":
                        nightFixMode = NightFixMode.Increased;
                        break;
                    case "Maximized":
                        nightFixMode = NightFixMode.Maximized;
                        break;
                    case "Standard":
                    default:
                        nightFixMode = NightFixMode.Standard;
                        break;
                }
                Monitor.Log($"Mode is [{nightFixMode.ToString()}]", LogLevel.Info);
                happinessMap = new Dictionary<long, byte>();
                TimeEvents.TimeOfDayChanged += this.TimeEvents_TimeOfDayChanged;
            }
            if (Config.ProfessionFix.Enabled)
            {
                InputEvents.ButtonPressed += this.InputEvents_ButtonPressed;
            }
        }

        private void InputEvents_ButtonPressed(object sender, EventArgsInput e)
        {
            // The goal is to make this script minimally invasive, so it's only called when a fix needs to be made
            // Activation Requirements:
            // * Is Action Button
            // * Is within player's reach
            // * Player has one of the vulnerable professions
            // * Player is on the farm or in a farm building
            // * grabTile has collision with an animal
            // * Animal has not been pet
            // * The particular profession and animal combination will cause overflow
            if (e.IsActionButton)
            {
                ICursorPosition cursorPosition = e.Cursor;
                Microsoft.Xna.Framework.Vector2 grabTile = cursorPosition.GrabTile;
                if (Utility.tileWithinRadiusOfPlayer((int)grabTile.X, (int)grabTile.Y, 1, Game1.player))
                {
                    StardewValley.Farmer farmer = Game1.player;
                    if (farmer.FarmerSprite.pauseForSingleAnimation)
                        return;
                    if (farmer.professions.Contains(2) || farmer.professions.Contains(3))
                    {
                        Microsoft.Xna.Framework.Rectangle rectangle = new Microsoft.Xna.Framework.Rectangle((int)grabTile.X * Game1.tileSize, (int)grabTile.Y * Game1.tileSize, Game1.tileSize, Game1.tileSize);
                        if (Game1.currentLocation is Farm farm)
                        {
                            foreach (KeyValuePair<long, FarmAnimal> animal in (Dictionary<long, FarmAnimal>)farm.animals)
                            {
                                if (Game1.timeOfDay >= 1900 && !animal.Value.isMoving())
                                    return;
                                if (animal.Value.GetBoundingBox().Intersects(rectangle))
                                {
                                    if (!animal.Value.wasPet
                                        && (
                                            (farmer.professions.Contains(3) && !animal.Value.isCoopDweller())
                                            || (farmer.professions.Contains(2) && animal.Value.isCoopDweller())
                                           )
                                       )
                                    {
                                        Monitor.Log($"Profession overflow prevented", LogLevel.Info);
                                        this.pet(animal.Value, farmer);
                                        this.SuppressButton(e.Button);
                                    }
                                }
                            }
                        } else if (Game1.currentLocation is AnimalHouse animalHouse)
                        {
                            foreach (KeyValuePair<long, FarmAnimal> animal in (Dictionary<long, FarmAnimal>)animalHouse.animals)
                            {
                                if (Game1.timeOfDay >= 1900 && !animal.Value.isMoving())
                                    return;
                                if (!animal.Value.wasPet
                                    && (
                                        (farmer.professions.Contains(3) && !animal.Value.isCoopDweller())
                                        || (farmer.professions.Contains(2) && animal.Value.isCoopDweller())
                                       )
                                   )
                                {
                                    if (animal.Value.GetBoundingBox().Intersects(rectangle))
                                    {
                                        Monitor.Log($"Profession overflow prevented", LogLevel.Info);
                                        this.pet(animal.Value, farmer);
                                        this.SuppressButton(e.Button);
                                    }
                                }
                            }

                        }
                    }
                }
            }
        }

        public void pet(FarmAnimal animal, StardewValley.Farmer farmer)
        {
            farmer.Halt();
            farmer.faceGeneralDirection(animal.position, 0);
            animal.Halt();
            animal.sprite.StopAnimation();
            animal.uniqueFrameAccumulator = -1;
            switch (Game1.player.FacingDirection)
            {
                case 0:
                    animal.sprite.currentFrame = 0;
                    break;
                case 1:
                    animal.sprite.currentFrame = 12;
                    break;
                case 2:
                    animal.sprite.currentFrame = 8;
                    break;
                case 3:
                    animal.sprite.currentFrame = 4;
                    break;
            }
            animal.wasPet = true;
            animal.friendshipTowardFarmer = Math.Min(1000, animal.friendshipTowardFarmer + 15);
            if (farmer.professions.Contains(3) && !animal.isCoopDweller())
            {
                animal.friendshipTowardFarmer = Math.Min(1000, animal.friendshipTowardFarmer + 15);
                animal.happiness = (byte) Math.Min((int)byte.MaxValue, ((uint)animal.happiness + (uint)Math.Max(5, 40 - (int)animal.happinessDrain)));
            }
            else if (farmer.professions.Contains(2) && animal.isCoopDweller())
            {
                animal.friendshipTowardFarmer = Math.Min(1000, animal.friendshipTowardFarmer + 15);
                animal.happiness = (byte) Math.Min((int)byte.MaxValue, ((uint)animal.happiness + (uint)Math.Max(5, 40 - (int)animal.happinessDrain)));
            }
            animal.doEmote((int)animal.moodMessage == 4 ? 12 : 20, true);
            animal.happiness = (byte)Math.Min((int)byte.MaxValue, (int)animal.happiness + Math.Max(5, 40 - (int)animal.happinessDrain));
            if (animal.sound != null && Game1.soundBank != null)
            {
                Cue cue = Game1.soundBank.GetCue(animal.sound);
                string name = "Pitch";
                double num = (double)(1200 + Game1.random.Next(-200, 201));
                cue.SetVariable(name, (float)num);
                cue.Play();
            }
            farmer.gainExperience(0, 5);
            if (!animal.type.Equals("Sheep") || animal.friendshipTowardFarmer < 900)
                return;
            animal.daysToLay = (byte)2;
        }

        public void SuppressButton(SButton button)
        {
            // SuppressButton copyright Pathoschild (https://github.com/Pathoschild)
            // Used and modified with permission
            // keyboard
            if (button.TryGetKeyboard(out Keys key))
                Game1.oldKBState = new KeyboardState(Game1.oldKBState.GetPressedKeys().Union(new[] { key }).ToArray());

            // controller
            else if (button.TryGetController(out Buttons controllerButton))
            {
                var newState = GamePad.GetState(PlayerIndex.One);
                var thumbsticks = Game1.oldPadState.ThumbSticks;
                var triggers = Game1.oldPadState.Triggers;
                var buttons = Game1.oldPadState.Buttons;
                var dpad = Game1.oldPadState.DPad;

                switch (controllerButton)
                {
                    // d-pad
                    case Buttons.DPadDown:
                        dpad = new GamePadDPad(dpad.Up, newState.DPad.Down, dpad.Left, dpad.Right);
                        break;
                    case Buttons.DPadLeft:
                        dpad = new GamePadDPad(dpad.Up, dpad.Down, newState.DPad.Left, dpad.Right);
                        break;
                    case Buttons.DPadRight:
                        dpad = new GamePadDPad(dpad.Up, dpad.Down, dpad.Left, newState.DPad.Right);
                        break;
                    case Buttons.DPadUp:
                        dpad = new GamePadDPad(newState.DPad.Up, dpad.Down, dpad.Left, dpad.Right);
                        break;

                    // trigger
                    case Buttons.LeftTrigger:
                        triggers = new GamePadTriggers(newState.Triggers.Left, triggers.Right);
                        break;
                    case Buttons.RightTrigger:
                        triggers = new GamePadTriggers(triggers.Left, newState.Triggers.Right);
                        break;

                    // thumbstick
                    case Buttons.LeftThumbstickDown:
                    case Buttons.LeftThumbstickLeft:
                    case Buttons.LeftThumbstickRight:
                    case Buttons.LeftThumbstickUp:
                        thumbsticks = new GamePadThumbSticks(newState.ThumbSticks.Left, thumbsticks.Right);
                        break;
                    case Buttons.RightThumbstickDown:
                    case Buttons.RightThumbstickLeft:
                    case Buttons.RightThumbstickRight:
                    case Buttons.RightThumbstickUp:
                        thumbsticks = new GamePadThumbSticks(newState.ThumbSticks.Right, thumbsticks.Left);
                        break;

                    // buttons
                    default:
                        var mask =
                            (buttons.A == ButtonState.Pressed ? Buttons.A : 0)
                            | (buttons.B == ButtonState.Pressed ? Buttons.B : 0)
                            | (buttons.Back == ButtonState.Pressed ? Buttons.Back : 0)
                            | (buttons.BigButton == ButtonState.Pressed ? Buttons.BigButton : 0)
                            | (buttons.LeftShoulder == ButtonState.Pressed ? Buttons.LeftShoulder : 0)
                            | (buttons.LeftStick == ButtonState.Pressed ? Buttons.LeftStick : 0)
                            | (buttons.RightShoulder == ButtonState.Pressed ? Buttons.RightShoulder : 0)
                            | (buttons.RightStick == ButtonState.Pressed ? Buttons.RightStick : 0)
                            | (buttons.Start == ButtonState.Pressed ? Buttons.Start : 0)
                            | (buttons.X == ButtonState.Pressed ? Buttons.X : 0)
                            | (buttons.Y == ButtonState.Pressed ? Buttons.Y : 0);
                        mask = mask ^ controllerButton;
                        buttons = new GamePadButtons(mask);
                        break;
                }

                Game1.oldPadState = new GamePadState(thumbsticks, triggers, buttons, dpad);
            }
            // mouse
            else if (button.TryGetStardewInput(out InputButton inputButton))
            {
                if (inputButton.mouseLeft)
                {
                    Game1.oldMouseState = new MouseState(
                        Game1.oldMouseState.X,
                        Game1.oldMouseState.Y,
                        Game1.oldMouseState.ScrollWheelValue,
                        ButtonState.Pressed,
                        Game1.oldMouseState.MiddleButton,
                        Game1.oldMouseState.RightButton,
                        Game1.oldMouseState.XButton1,
                        Game1.oldMouseState.XButton2
                    );
                }
                else if (inputButton.mouseRight)
                {
                    Game1.oldMouseState = new MouseState(
                        Game1.oldMouseState.X,
                        Game1.oldMouseState.Y,
                        Game1.oldMouseState.ScrollWheelValue,
                        Game1.oldMouseState.LeftButton,
                        Game1.oldMouseState.MiddleButton,
                        ButtonState.Pressed,
                        Game1.oldMouseState.XButton1,
                        Game1.oldMouseState.XButton2
                    );
                }
            }
        }

        private void TimeEvents_TimeOfDayChanged(object sender, EventArgsIntChanged e)
        {
            if (nightFixMode == NightFixMode.Maximized)
            {
                foreach (FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
                {
                    animal.happiness = (byte)255;
                }
            }
            else
            {
                // Happiness is calculated correctly in the winter, so only fix it if it's not winter
                if (!Game1.currentSeason.Equals("winter"))
                {
                    // At 5:50pm, record animals' happiness
                    if (e.NewInt == 1750)
                    {
                        happinessMap = new Dictionary<long, byte>();
                        foreach (FarmAnimal animal in Game1.getFarm().getAllFarmAnimals())
                        {
                            happinessMap[animal.myID] = animal.happiness;
                        }
                    }
                    if (e.NewInt >= 1800)
                    {
                        // Each time change after that, if the animal is inside, reset the happiness to the last known good value
                        foreach (Building building in Game1.getFarm().buildings)
                        {
                            if (building.indoors != null && building.indoors.GetType() == typeof(AnimalHouse))
                            {
                                foreach (KeyValuePair<long, FarmAnimal> animal in (Dictionary<long, FarmAnimal>)((AnimalHouse)building.indoors).animals)
                                {
                                    if (!happinessMap.ContainsKey(animal.Key))
                                    {
                                        // This should only happen if the user cheats to get a new animal after 6pm
                                        happinessMap[animal.Key] = animal.Value.happiness;
                                        continue;
                                    }
                                    var happiness = (int)happinessMap[animal.Key];
                                    int newHappiness = (int)animal.Value.happiness;
                                    if (newHappiness >= happiness)
                                    {
                                        // Not sure why this would happen, but just in case
                                        happiness = newHappiness;
                                    }
                                    if (nightFixMode == NightFixMode.Increased)
                                    {
                                        // If the user config mode is Increased, add happiness for being safe in the stable after 6pm
                                        happiness = Math.Min(byte.MaxValue, (happiness + animal.Value.happinessDrain));
                                    }
                                    animal.Value.happiness = (byte)happiness;
                                    happinessMap[animal.Key] = (byte)happiness;
                                }
                            }
                        }
                        // If the animal is outside, record a new known good value
                        foreach (KeyValuePair<long, FarmAnimal> animal in (Dictionary<long, FarmAnimal>)Game1.getFarm().animals)
                        {
                            happinessMap[animal.Key] = animal.Value.happiness;
                        }

                    }
                }
            }
        }
    }
}
