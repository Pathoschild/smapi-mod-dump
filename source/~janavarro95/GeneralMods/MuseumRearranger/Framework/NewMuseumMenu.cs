using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;

namespace Omegasis.MuseumRearranger.Framework
{
    /// <summary>A subclass of <see cref="MuseumMenu"/> which adds support for toggling the inventory box.</summary>
    internal class NewMuseumMenu : MuseumMenu
    {
        /*********
        ** Properties
        *********/
        /// <summary>Whether to show the inventory screen.</summary>
        private bool ShowInventory = true;

        /// <summary>A reference to a private <see cref="MuseumMenu"/> field for use in the overridden draw code.</summary>
        private readonly IReflectedField<bool> HoldingMuseumPiece;


        /*********
        ** Public methods
        *********/
        /// <summary>Construct an instance.</summary>
        /// <param name="reflection">Simplifies access to private game code.</param>
        public NewMuseumMenu(IReflectionHelper reflection)
        {
            this.HoldingMuseumPiece = reflection.GetField<bool>(this, "holdingMuseumPiece");
        }

        /// <summary>Toggle the inventory box.</summary>
        public void ToggleInventory()
        {
            this.ShowInventory = !this.ShowInventory;
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch being drawn.</param>
        public override void draw(SpriteBatch b)
        {
            if ((this.fadeTimer <= 0 || !this.fadeIntoBlack) && this.state != 3)
            {
                if (this.heldItem != null)
                {
                    for (int i = Game1.viewport.Y / Game1.tileSize - 1; i < (Game1.viewport.Y + Game1.viewport.Height) / Game1.tileSize + 2; i++)
                    {
                        for (int j = Game1.viewport.X / Game1.tileSize - 1; j < (Game1.viewport.X + Game1.viewport.Width) / Game1.tileSize + 1; j++)
                        {
                            if (((LibraryMuseum)Game1.currentLocation).isTileSuitableForMuseumPiece(j, i))
                                b.Draw(Game1.mouseCursors, Game1.GlobalToLocal(Game1.viewport, new Vector2(j, i) * Game1.tileSize), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 29), Color.LightGreen);
                        }
                    }
                }
                if (!this.HoldingMuseumPiece.GetValue() && this.ShowInventory)
                    base.draw(b, false, false);
                if (!this.hoverText.Equals(""))
                    IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
                this.heldItem?.drawInMenu(b, new Vector2(Game1.getOldMouseX() + 8, Game1.getOldMouseY() + 8), 1f);
                base.drawMouse(b);
                this.sparkleText?.draw(b, Game1.GlobalToLocal(Game1.viewport, this.globalLocationOfSparklingArtifact));
            }
            b.Draw(Game1.fadeToBlackRect, new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), Color.Black * this.blackFadeAlpha);
        }
    }
}
