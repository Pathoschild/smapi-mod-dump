using Denifia.Stardew.SendItems.Events;
using Denifia.Stardew.SendItems.Framework;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using xTile.Dimensions;

namespace Denifia.Stardew.SendItems.Services
{
    public interface ILetterboxInteractionDetector
    {
    }

    /// <summary>
    /// Detects when the player is interacting with the letterbox
    /// </summary>
    public class LetterboxInteractionDetector : ILetterboxInteractionDetector
    {
        private const string _locationOfLetterbox = "Farm";

        public LetterboxInteractionDetector()
        {
            SaveEvents.AfterLoad += AfterSavedGameLoad;
            SaveEvents.AfterReturnToTitle += AfterReturnToTitle;
        }

        private void AfterReturnToTitle(object sender, EventArgs e)
        {
            try
            {
                ControlEvents.MouseChanged -= MouseChanged;
                PlayerEvents.Warped -= PlayerWarped;
            }
            catch (Exception)
            {
            }
        }

        private void AfterSavedGameLoad(object sender, EventArgs e)
        {
            PlayerEvents.Warped += PlayerWarped;
        }

        private void PlayerWarped(object sender, EventArgsPlayerWarped e)
        {
            if (e.NewLocation.Name == _locationOfLetterbox)
            {
                // Only watch for mouse events while at the location of the letterbox, for performance
                ControlEvents.MouseChanged += MouseChanged;
            }
            else
            {
                ControlEvents.MouseChanged -= MouseChanged;
            }
        }

        private void MouseChanged(object sender, EventArgsMouseStateChanged e)
        {
            if (e.NewState.RightButton == Microsoft.Xna.Framework.Input.ButtonState.Pressed)
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
