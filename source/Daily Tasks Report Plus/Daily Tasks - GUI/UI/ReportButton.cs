/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley.Menus;


namespace DailyTasksReport.UI
{
    public class ReportButton : IClickableMenu
    {
        private readonly ModEntry _parent;
        private readonly Action<bool> _openReport;

        private Rectangle _buttonRect;
        private Rectangle _insideRect;
        private float _uiscale;
        private int _questCount;
        private string _hoverText = string.Empty;

        public ReportButton(ModEntry parent, Action<bool> openReport)
        {
            _parent = parent;
            _openReport = openReport;
            _uiscale = Game1.options.uiScale;
            _questCount = Game1.player.questLog.Count;
            UpdatePosition();

            ModEntry.EventsHelper.GameLoop.UpdateTicked += GameLoop_UpdateTicked;
            ModEntry.EventsHelper.GameLoop.ReturnedToTitle += GameLoop_ReturnedToTitle;
        }

        private void GameLoop_UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (Game1.options.uiScale != _uiscale)
            {
                _uiscale = Game1.options.uiScale;
                UpdatePosition();
            }

            if (_questCount == Game1.player.questLog.Count) return;

            _questCount = Game1.player.questLog.Count;
            UpdatePosition();
        }

        private void GameLoop_ReturnedToTitle(object? sender, ReturnedToTitleEventArgs e)
        {
            ModEntry.EventsHelper.GameLoop.UpdateTicked -= GameLoop_UpdateTicked;
            ModEntry.EventsHelper.GameLoop.ReturnedToTitle -= GameLoop_ReturnedToTitle;
        }

        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            UpdatePosition();
        }
        
        private void UpdatePosition()
        {
            xPositionOnScreen=(int)( Game1.viewport.Width * Game1.options.zoomLevel/Game1.options.uiScale) - 140;// Game1.viewport.Width - 300 + 212;
            yPositionOnScreen = Game1.tileSize / 8 + 225;
            if (_questCount > 0)
                yPositionOnScreen +=(int)( 14);
            width = (int)(45);
            height = (int)(55);

            _buttonRect = new Rectangle(xPositionOnScreen, yPositionOnScreen, width, height);
            _insideRect = new Rectangle((int)( xPositionOnScreen + 2 ),
               (int)(yPositionOnScreen + 4 ),
               (int)(width - 3), (int)(height - 6));
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