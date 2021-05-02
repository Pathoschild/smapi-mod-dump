/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LunaticShade/StardewValley.SafeReading
**
*************************************************/

using SkillfulClothes;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SafeReading
{
    /// <summary>
    /// Watches changes to the GameLocation (e.g. map)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    class LocationObserver
    {
        IModHelper helper;
        string currentLocation;
        HashSet<string> activeLocations = new HashSet<string>() { nameof(MineShaft), nameof(VolcanoDungeon), nameof(Woods) };

        bool isAllowed = false;
        bool isActive = false;

        bool originalInvincibility = false;
        int originalInvincibilityTimer = 0;

        public LocationObserver(IModHelper helper)
        {
            this.helper = helper;
        }

        public void Update()
        {
            string newLocation = Game1.currentLocation?.GetType()?.Name;
            if (currentLocation != newLocation)
            {
                LocationChanged(newLocation);
            }            
        }

        public void LocationChanged(string newLocation)
        {
            currentLocation = newLocation;            
            // Logger.Debug($"Location changed to {newLocation}");
            
            // avoid adding event handler multiple items
            helper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
            
            if (activeLocations.Contains(newLocation))
            {
                AllowSafeReading(true);
            } else
            {
                AllowSafeReading(false);                
            }
        }

        private void AllowSafeReading(bool allowed)
        {
            if (allowed != isAllowed)
            {
                isAllowed = allowed;

                if (allowed)
                {
                    helper.Events.Display.MenuChanged += Display_MenuChanged;                    
                } else
                {
                    helper.Events.Display.MenuChanged -= Display_MenuChanged;
                    ActivateSafeReading(false);
                }
            }
        }

        private void Display_MenuChanged(object sender, StardewModdingAPI.Events.MenuChangedEventArgs e)
        {
            ActivateSafeReading(e.NewMenu != null);            
        }

        private void ActivateSafeReading(bool active)
        {
            if (isActive != active)
            {
                isActive = active;

                if (active)
                {
                    Logger.Debug($"SafeReading is now active");

                    originalInvincibility = Game1.player.temporarilyInvincible;
                    originalInvincibilityTimer = Game1.player.temporaryInvincibilityTimer;

                    helper.Events.GameLoop.UpdateTicking += GameLoop_UpdateTicking;
                } else
                {
                    helper.Events.GameLoop.UpdateTicking -= GameLoop_UpdateTicking;
                    // restore the original values
                    Game1.player.temporarilyInvincible = originalInvincibility;
                    Game1.player.temporaryInvincibilityTimer = originalInvincibilityTimer;

                    Logger.Debug($"SafeReading is now disabled");
                }             
            }
        }

        private void GameLoop_UpdateTicking(object sender, StardewModdingAPI.Events.UpdateTickingEventArgs e)
        {
            if (isActive)
            {
                // if a menu is opened -> make player invincible             
                Game1.player.temporarilyInvincible = true;
                Game1.player.temporaryInvincibilityTimer = 1;
            }
        }

        public void Reset()
        {
            AllowSafeReading(false);
            currentLocation = null;            
        }
    }   
}
