using Denifia.Stardew.SendItems.Events;
using StardewModdingAPI.Events;
using StardewValley;
using System;
using System.Linq;
using xTile.Dimensions;

namespace Denifia.Stardew.SendItems.Services
{
    public interface IPostboxInteractionDetector
    {

    }

    /// <summary>
    /// Detects when the player is interacting with the postbox
    /// </summary>
    public class PostboxInteractionDetector : IPostboxInteractionDetector
    {
        private const string locationOfPostbox = "Farm";

        public PostboxInteractionDetector()
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
            if (e.NewLocation.Name == locationOfPostbox)
            {
                // Only watch for mouse events while at the location of the postbox, for performance
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
                    if (CanUsePostbox())
                    {
                        ModEvents.RaisePlayerUsingPostbox(this, EventArgs.Empty);
                    }
                }
            }
        }

        private bool CanUsePostbox()
        {
            return Game1.mailbox != null && !Game1.mailbox.Any();
        }
    }
}
