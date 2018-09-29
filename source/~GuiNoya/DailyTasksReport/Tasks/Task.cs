using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System.Collections.Generic;

namespace DailyTasksReport.Tasks
{
    public abstract class Task
    {
        internal static readonly Dictionary<int, string> ObjectsNames = new Dictionary<int, string>();
        protected bool Enabled = true;

        protected abstract void FirstScan();

        public abstract void Clear();

        public abstract string GeneralInfo(out int usedLines);

        public abstract string DetailedInfo(out int usedLines, out bool skipNextPage);

        public virtual void FinishedReport()
        {
        }

        public virtual void OnDayStarted()
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
            var r = new Rectangle((int)destinationPosition.X, (int)destinationPosition.Y, Game1.tileSize * 3 / 4,
                Game1.tileSize * 3 / 4);
            b.Draw(Game1.mouseCursors, r, new Rectangle(141, 465, 20, 24), Color.White * 0.75f);
            r.Offset(r.Width / 4, r.Height / 6);
            r.Height /= 2;
            r.Width /= 2;
            b.Draw(texture, r, sourceRectangle, Color.White);
        }

        protected static void DrawBubble2Icons(SpriteBatch b, Texture2D texture1, Rectangle sourceRectangle1,
            Texture2D texture2, Rectangle sourceRectangle2, Vector2 destinationPosition)
        {
            var r = new Rectangle((int)destinationPosition.X - Game1.tileSize / 4,
                (int)destinationPosition.Y,
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

        internal static void PopulateObjectsNames()
        {
            foreach (var pair in Game1.objectInformation)
                ObjectsNames[pair.Key] = pair.Value.Split("/".ToCharArray())[0];
        }
    }
}