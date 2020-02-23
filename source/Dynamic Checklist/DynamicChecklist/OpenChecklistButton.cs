namespace DynamicChecklist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DynamicChecklist.ObjectLists;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewModdingAPI.Events;
    using StardewValley;
    using StardewValley.Menus;

    public class OpenChecklistButton : IClickableMenu
    {
        private readonly Texture2D texture;
        private Action openChecklist;
        private string hoverText = string.Empty;
        private Func<int> countRemainingTasks;
        private ModConfig config;

        public OpenChecklistButton(Action openChecklist, Func<int> countRemainingTasks, ModConfig config, IModEvents events)
            : base(0, 0, OverlayTextures.Sign.Width * Game1.pixelZoom, OverlayTextures.Sign.Height * Game1.pixelZoom, false)
        {
            this.config = config;
            this.countRemainingTasks = countRemainingTasks;
            this.texture = OverlayTextures.Sign;
            this.openChecklist = openChecklist;
            events.Display.MenuChanged += this.OnMenuClosed;
            this.UpdateButtonPosition(config.OpenChecklistButtonLocation);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.openChecklist();
            base.receiveLeftClick(x, y, playSound);
        }

        public override void performHoverAction(int x, int y)
        {
            this.hoverText = "Checklist";
        }

        public override void draw(SpriteBatch b)
        {
            this.UpdateButtonPosition(this.config.OpenChecklistButtonLocation);
            int tasks = this.countRemainingTasks();
            SpriteFont font = (tasks > 9) ? Game1.smallFont : Game1.dialogueFont;
            string s = tasks.ToString();
            Vector2 sSize = font.MeasureString(s);
            Vector2 sPos = new Vector2(this.xPositionOnScreen + this.width / 2 - sSize.X / 2, this.yPositionOnScreen + this.height / 2 - sSize.Y / 2 + 10);
            b.Draw(this.texture, new Rectangle(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height), Color.White);
            b.DrawString(font, tasks.ToString(), sPos, Color.Black);
            if (this.isWithinBounds(Game1.getOldMouseX(), Game1.getOldMouseY()))
            {
                drawHoverText(Game1.spriteBatch, this.hoverText, Game1.dialogueFont);
            }

            base.draw(b);
        }

        private void OnMenuClosed(object sender, MenuChangedEventArgs e)
        {
            if (e.OldMenu is ChecklistMenu)
            {
                this.UpdateButtonPosition(this.config.OpenChecklistButtonLocation);
            }
        }

        private void UpdateButtonPosition(ModConfig.ButtonLocation buttonLocation)
        {
            switch (buttonLocation)
            {
                case ModConfig.ButtonLocation.BelowJournal:
                    this.xPositionOnScreen = Game1.viewport.Width - 300 + 180 + Game1.tileSize / 2;
                    this.yPositionOnScreen = Game1.tileSize / 8 + 296;
                    break;
                case ModConfig.ButtonLocation.LeftOfJournal:
                    this.xPositionOnScreen = Game1.viewport.Width - 300 + 125 + Game1.tileSize / 2;
                    this.yPositionOnScreen = Game1.tileSize / 8 + 240;
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}
