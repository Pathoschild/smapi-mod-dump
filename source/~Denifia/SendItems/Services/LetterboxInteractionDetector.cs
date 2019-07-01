using System;
using System.Linq;
using Denifia.Stardew.SendItems.Events;
using Denifia.Stardew.SendItems.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using xTile.Dimensions;

namespace Denifia.Stardew.SendItems.Services
{
    /// <summary>
    /// Detects when the player is interacting with the letterbox
    /// </summary>
    public class LetterboxInteractionDetector : ILetterboxInteractionDetector
    {
        private const string _locationOfLetterbox = "Farm";
        private readonly IModEvents _events;

        public LetterboxInteractionDetector(IModEvents events)
        {
            _events = events;

            events.GameLoop.SaveLoaded += OnSaveLoaded;
            events.GameLoop.ReturnedToTitle += OnReturnedToTitle;
        }

        /// <summary>Raised after the game returns to the title screen.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnReturnedToTitle(object sender, ReturnedToTitleEventArgs e)
        {
            _events.Input.ButtonPressed -= OnButtonPressed;
            _events.Player.Warped -= OnWarped;
        }

        /// <summary>Raised after the player loads a save slot and the world is initialised.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            _events.Player.Warped += OnWarped;
        }

        /// <summary>Raised after a player warps to a new location. NOTE: this event is currently only raised for the current player.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.NewLocation.Name == _locationOfLetterbox)
            {
                // Only watch for mouse events while at the location of the letterbox, for performance
                _events.Input.ButtonPressed += OnButtonPressed;
            }
            else
            {
                _events.Input.ButtonPressed -= OnButtonPressed;
            }
        }

        /// <summary>Raised after the player presses a button on the keyboard, controller, or mouse.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void OnButtonPressed(object sender, ButtonPressedEventArgs e)
        {
            if (e.Button == SButton.MouseRight)
            {
                // Check if the click is on the letterbox tile or the one above it
                Location tileLocation = new Location((int)Game1.currentCursorTile.X, (int)Game1.currentCursorTile.Y);

                if (tileLocation.X == 68 && (tileLocation.Y >= 15 && tileLocation.Y <= 16))
                {
                    if (CanUseLetterbox())
                    {
                        ModEvents.RaisePlayerUsingLetterbox(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool CanUseLetterbox()
        {
            return Game1.mailbox != null && Game1.mailbox.Any() && Game1.mailbox.First() == ModConstants.PlayerMailKey;
        }
    }
}
