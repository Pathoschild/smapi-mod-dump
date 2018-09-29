using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;

namespace DailyTasksReport.UI
{
    public class ReportButton : IClickableMenu
    {
        private readonly ModEntry _parent;
        private readonly Action<bool> _openReport;

        private Rectangle _buttonRect;
        private Rectangle _insideRect;

        private int _questCount;
        private string _hoverText = string.Empty;

        public ReportButton(ModEntry parent, Action<bool> openReport)
        {
            _parent = parent;
            _openReport = openReport;

            _questCount = Game1.player.questLog.Count;
            UpdatePosition();

            GameEvents.UpdateTick += GameEvents_UpdateTick;
            SaveEvents.AfterReturnToTitle += SaveEvents_AfterReturnToTitle;
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (_questCount == Game1.player.questLog.Count) return;

            _questCount = Game1.player.questLog.Count;
            UpdatePosition();
        }

        private void SaveEvents_AfterReturnToTitle(object sender, EventArgs e)
        {
            GameEvents.UpdateTick -= GameEvents_UpdateTick;
            SaveEvents.AfterReturnToTitle -= SaveEvents_AfterReturnToTitle;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            UpdatePosition();
        }

        private void UpdatePosition()
        {
            xPositionOnScreen = Game1.viewport.Width - 300 + 212;
            yPositionOnScreen = Game1.tileSize / 8 + 240;
            if (_questCount > 0)
                yPositionOnScreen += 14 * Game1.pixelZoom;
            width = 11 * Game1.pixelZoom;
            height = 14 * Game1.pixelZoom;

            _buttonRect = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
            _insideRect = new Rectangle(xPositionOnScreen + 2 * Game1.pixelZoom,
                yPositionOnScreen + 4 * Game1.pixelZoom,
                width - 3 * Game1.pixelZoom, height - 6 * Game1.pixelZoom);
        }

        public override void draw(SpriteBatch b)
        {
            if (!_parent.Config.DisplayReportButton) return;

            b.Draw(Game1.mouseCursors, _buttonRect, new Rectangle(383, 493, 11, 14), Color.White);

            b.Draw(Game1.mouseCursors, _insideRect, new Rectangle(5, 594, 10, 13), Color.NavajoWhite);

            if (_hoverText.Length > 0)
            {
                drawHoverText(b, _hoverText, Game1.dialogueFont);
                _hoverText = "";
            }
        }

        public override void performHoverAction(int x, int y)
        {
            var keybinding = _parent.Config.OpenReportKey != SButton.None ? $"({_parent.Config.OpenReportKey})" : "";
            _hoverText = $"Report {keybinding}";
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            _openReport(false);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }
    }
}