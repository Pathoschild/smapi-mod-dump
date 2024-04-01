/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/DailyTaskReportPlus
**
*************************************************/

using DailyTasksReport.TaskEngines;
using Microsoft.Xna.Framework.Graphics;

namespace DailyTasksReport.Tasks
{
    public abstract class DailyReportTask
    {

        internal TaskEngine _Engine;
        internal static TaskReportConfig _config;

        //public bool Enabled;
        internal static readonly Dictionary<int, string> ObjectsNames = new Dictionary<int, string>();
        protected bool Enabled = true;
        public string TaskClass { get { return _Engine.TaskClass; } }
        public string TaskSubClass { get { return _Engine.TaskSubClass; } }
        protected void FirstScan() { _Engine.FirstScan(); }

        public virtual void Clear() { _Engine.Clear(); }

        public abstract string GeneralInfo(out int usedLines);
        public abstract string DetailedInfo(out int usedLines, out bool skipNextPage);

        public void FinishedReport() { _Engine.FinishedReport(); }
        public void OnDayStarted()
        {
            Clear();
            FirstScan();
        }
        
        public virtual void Draw(SpriteBatch b)
        {
        }
        protected static void DrawBubble(SpriteBatch b, Texture2D texture, Rectangle sourceRectangle,
        Vector2 destinationPosition)
        {
            float bubbleBounce = 0;
            if (_config.BounceBubbles)
            {
                bubbleBounce = 4f * (float)Math.Round(Math.Sin(Game1.currentGameTime.TotalGameTime.TotalMilliseconds / 250.0), 2);
            }
            var r = new Rectangle((int)(destinationPosition.X * Game1.options.zoomLevel/ Game1.options.desiredUIScale), (int)(destinationPosition.Y  * Game1.options.zoomLevel/ Game1.options.desiredUIScale + bubbleBounce),(int)(( Game1.tileSize * 3 / 4) * Game1.options.zoomLevel/ Game1.options.desiredUIScale),
             ( int)(  (Game1.tileSize * 3 / 4)*Game1.options.zoomLevel/ Game1.options.desiredUIScale));
             b.Draw(Game1.mouseCursors, r, new Rectangle(141, 465, 20, 24), Color.White * 0.75f);
            r.Offset(r.Width / 4, r.Height / 6);
            r.Height /= 2;
            r.Width /= 2;
            b.Draw(texture, r, sourceRectangle, Color.White);
        }
        protected static void DrawBubble2Icons(SpriteBatch b, Texture2D texture1, Rectangle sourceRectangle1,
        Texture2D texture2, Rectangle sourceRectangle2, Vector2 destinationPosition)
        {
            var r = new Rectangle((int)(destinationPosition.X * Game1.options.zoomLevel) - Game1.tileSize / 4,
                (int)(destinationPosition.Y * Game1.options.zoomLevel),
                Game1.tileSize,
                Game1.tileSize * 3 / 4);
            b.Draw(Game1.mouseCursors, r, new Rectangle(141, 465, 20, 24), Color.White * 0.75f);
            r.Offset(r.Width / 6, r.Height / 6);
            r.Width = (r.Width - Game1.tileSize / 4) / 2;
            r.Height /= 2;
            b.Draw(texture1, r, sourceRectangle1, Color.White);
            r.Offset(r.Width, 0);
            b.Draw(texture2, r, sourceRectangle2, Color.White);
        }
    }
}