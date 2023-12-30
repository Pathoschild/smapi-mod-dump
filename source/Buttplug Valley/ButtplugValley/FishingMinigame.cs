/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/DryIcedTea/Buttplug-Valley
**
*************************************************/

using System;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace ButtplugValley
{
    internal class FishingMinigame
    {
        private IModHelper helper;
        private IReflectionHelper reflectionHelper;
        public float previousCaptureLevel;
        private BPManager _bpManager;
        private IMonitor monitor;
        public bool isActive = true;
        
        
        public float maxVibration = 100f; // Adjust as desired

        public FishingMinigame(IModHelper modHelper, IMonitor MeMonitor, BPManager MEbpManager)
        {
            helper = modHelper;
            monitor = MeMonitor;
            _bpManager = MEbpManager;
            reflectionHelper = helper.Reflection;
            previousCaptureLevel = 0f;
            helper.Events.Input.ButtonPressed += OnButtonPressed;
            helper.Events.GameLoop.UpdateTicked += OnUpdateTicked;
            helper.Events.GameLoop.DayStarted += OnDayStarted;
        }

        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            
        }

        private void OnUpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            FishingCheck();
        }

        private void FishingCheck()
        {
            if (!Context.IsWorldReady)
                return;

            if (Game1.activeClickableMenu is StardewValley.Menus.BobberBar menu)
            {
                monitor.Log("FishingMinigameIsActive", LogLevel.Debug);

                // Get the distanceFromCatching field using reflection
                IReflectedField<float> distanceFromCatchingField = this.reflectionHelper.GetField<float>(menu, "distanceFromCatching");

                if (distanceFromCatchingField == null)
                {
                    monitor.Log("distanceFromCatching field not found", LogLevel.Debug);
                    return;
                }

                float captureLevel = distanceFromCatchingField.GetValue();
                monitor.Log($"distancefrom {captureLevel}", LogLevel.Debug);

                // Scale the capture level based on the maximum vibration value
                float scaledCaptureLevel = captureLevel * maxVibration;

                // Ensure the scaled capture level does not exceed the maximum vibration value
                float capturePercentage = Math.Min(scaledCaptureLevel, maxVibration);

                // Vibrate the device based on the capture percentage if it has changed
                if (capturePercentage != previousCaptureLevel)
                {
                    monitor.Log($"FISHINGMINIGAME {capturePercentage}", LogLevel.Debug);
                    _bpManager.VibrateDevice(capturePercentage);
                    previousCaptureLevel = capturePercentage;
                }
            }
            else
            {
                // The bobber bar menu is no longer active, stop vibrating the device
                if (previousCaptureLevel > 0)
                {
                    monitor.Log("Stopping device vibration", LogLevel.Debug);
                    _bpManager.VibrateDevice(0);
                    previousCaptureLevel = 0;
                }
            }
        }



        private void OnDayStarted(object sender, DayStartedEventArgs e)
        {
            // Reset previous capture level when a new day starts
            previousCaptureLevel = 0f;
        }
        
    }
}