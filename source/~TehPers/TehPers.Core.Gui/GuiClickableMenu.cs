using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using TehPers.Core.Enums;
using TehPers.Core.Gui.Base.Components;
using TehPers.Core.Gui.Base.Units;
using TehPers.Core.Gui.SDV.Units;
using TehPers.Core.Helpers.Static;

namespace TehPers.Core.Gui {
    public class GuiClickableMenu : IClickableMenu {
        private readonly IGuiComponent _component;
        private readonly ITehCoreApi _coreApi;
        public IMod Owner => this._coreApi.Owner;

        internal GuiClickableMenu(IGuiComponent component, ITehCoreApi coreApi) : base(0, 0, 400, 200, false) {
            this._component = component;
            this._coreApi = coreApi;
        }

        public override void draw(SpriteBatch b) {
            ResolvedVector2 viewportSize = new ResolvedVector2(Game1.viewport.Width, Game1.viewport.Height);
            ResolvedVector2 componentSize = this._component.Size.Resolve(new GuiInfo(viewportSize.X, viewportSize.X), new GuiInfo(viewportSize.Y, viewportSize.Y));
            ResolvedVector2 componentLocation = this._component.Location.Resolve(new GuiInfo(0, viewportSize.X, componentSize.X), new GuiInfo(0, viewportSize.Y, componentSize.Y));

            // Draw the menu
            b.WithScissorRect(new Rectangle(0, 0, Game1.viewport.Width, Game1.viewport.Height), menuBatch => {
                this._component.Draw(menuBatch, componentLocation, componentSize);
            });

            // Draw the cursor
            if (!Game1.options.hardwareCursor) {
                b.Draw(Game1.mouseCursors, new Vector2(Game1.getMouseX(), Game1.getMouseY()), Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, Game1.options.SnappyMenus ? 44 : 0, 16, 16), Color.White * Game1.mouseCursorTransparency, 0.0f, Vector2.Zero, Game1.pixelZoom + Game1.dialogueButtonScale / 150f, SpriteEffects.None, 0.1f);
            }
        }

        public override void performHoverAction(int x, int y) { }

        public override void receiveLeftClick(int x, int y, bool playSound = true) {
            ResolvedVector2 viewportSize = new ResolvedVector2(Game1.viewport.Width, Game1.viewport.Height);
            ResolvedVector2 componentSize = this._component.Size.Resolve(new GuiInfo(viewportSize.X, viewportSize.X), new GuiInfo(viewportSize.Y, viewportSize.Y));
            ResolvedVector2 componentLocation = this._component.Location.Resolve(new GuiInfo(0, viewportSize.X, componentSize.X), new GuiInfo(0, viewportSize.Y, componentSize.Y));
            ResolvedVector2 clickRelativeLocation = new ResolvedVector2(x - componentLocation.X, y - componentLocation.Y);

            // Pass click to the component
            if (this._component.Click(componentLocation, componentSize, clickRelativeLocation, MouseButtons.LEFT))
                base.receiveLeftClick(x, y, playSound);
        }
    }
}