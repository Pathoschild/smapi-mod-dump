namespace DynamicChecklist.ObjectLists
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Graph;
    using Graph.Graphs;
    using Microsoft.Xna.Framework;
    using Microsoft.Xna.Framework.Graphics;
    using StardewValley;

    public abstract class ObjectList
    {
        private ShortestPath path;
        private ModConfig config;
        private bool overlayActive;
        private bool taskDone;

        public ObjectList(ModConfig config)
        {
            this.config = config;
        }

        public event EventHandler TaskFinished;

        public event EventHandler OverlayActivated;

        public event EventHandler OverlayActiveChanged;

        public static CompleteGraph Graph { get; set; }

        public abstract string OptionMenuLabel { get; protected set; }

        public bool TaskExistsNow { get; private set; }

        public bool IgnoreTask { get; private set; }

        public List<StardewObjectInfo> ObjectInfoList { get; set; }

        public abstract string TaskDoneMessage { get; protected set; }

        public bool ShowInMenu
        {
            get
            {
                return (this.TaskExistsNow || this.config.ShowAllTasks) && this.config.IncludeTask[this.Name];
            }
        }

        public bool TaskLeft
        {
            get
            {
                return !this.taskDone && this.config.IncludeTask[this.Name];
            }
        }

        public bool OverlayActive
        {
            get
            {
                return this.overlayActive;
            }

            set
            {
                if (!this.overlayActive && value)
                {
                    this.OnOverlayActivated(new EventArgs());
                }

                if (this.overlayActive != value)
                {
                    this.OnOverlayActivateChanged(new EventArgs());
                }

                this.overlayActive = value;
            }
        }

        public bool TaskDone
        {
            get
            {
                return this.taskDone;
            }

            protected set
            {
                var oldTaskDone = this.taskDone;
                this.taskDone = value;
                if (this.taskDone && !oldTaskDone && Game1.timeOfDay > 600)
                {
                    this.OnTaskFinished(new EventArgs());
                }
            }
        }

        protected bool TaskExistedAtStartOfDay { get; private set; }

        protected TaskName Name { get; set; }

        protected int Count { get; set; }

        protected abstract Texture2D ImageTexture { get; set; }

        protected int CountNeedAction
        {
            get
            {
                var count = 0;
                foreach (StardewObjectInfo soi in this.ObjectInfoList)
                {
                    if (soi.NeedAction)
                    {
                        count++;
                    }
                }

                return count;
            }
        }

        public abstract void OnMenuOpen();

        public abstract void BeforeDraw();

        public void OnNewDay()
        {
            this.UpdateObjectInfoList();
            this.TaskExistedAtStartOfDay = this.ObjectInfoList.Count > 0;
            this.TaskExistedAtStartOfDay = this.CountNeedAction > 0;
            this.TaskExistsNow = this.TaskExistedAtStartOfDay;
        }

        public void Draw(SpriteBatch b)
        {
            if (!this.TaskExistedAtStartOfDay && !this.TaskExistsNow && this.CountNeedAction > 0)
            {
                this.TaskExistsNow = true;
            }

            if (this.OverlayActive)
            {
                var currentPlayerLocation = Game1.currentLocation;
                var viewport = Game1.viewport;
                var smallestDistanceFromPlayer = float.PositiveInfinity;
                StardewObjectInfo closestSOI = null;
                bool anyOnScreen = false;
                foreach (StardewObjectInfo objectInfo in this.ObjectInfoList)
                {
                    if (objectInfo.NeedAction)
                    {
                        if (objectInfo.IsOnScreen())
                        {
                            anyOnScreen = true;
                        }

                        if (objectInfo.Location == currentPlayerLocation)
                        {
                            var drawLoc = new Vector2(objectInfo.Coordinate.X - viewport.X, objectInfo.Coordinate.Y - viewport.Y - Game1.tileSize / 2);
                            var spriteBox = new Rectangle((int)drawLoc.X - this.ImageTexture.Width / 4 * Game1.pixelZoom, (int)drawLoc.Y - this.ImageTexture.Height / 4 * Game1.pixelZoom - Game1.tileSize / 2, this.ImageTexture.Width * Game1.pixelZoom / 2, this.ImageTexture.Height * Game1.pixelZoom / 2);
                            var spriteBoxSpeechBubble = new Rectangle((int)drawLoc.X - OverlayTextures.SpeechBubble.Width / 4 * Game1.pixelZoom, (int)drawLoc.Y - OverlayTextures.SpeechBubble.Height / 4 * Game1.pixelZoom - Game1.tileSize / 2, OverlayTextures.SpeechBubble.Width * Game1.pixelZoom / 2, OverlayTextures.SpeechBubble.Height * Game1.pixelZoom / 2);
                            spriteBoxSpeechBubble.Offset(0, Game1.pixelZoom / 2);
                            if (this.config.ShowOverlay)
                            {
                                Game1.spriteBatch.Draw(OverlayTextures.SpeechBubble, spriteBoxSpeechBubble, Color.White);
                                Game1.spriteBatch.Draw(this.ImageTexture, spriteBox, Color.White);
                            }

                            var distanceFromPlayer = objectInfo.GetDistance(Game1.player);
                            if (distanceFromPlayer < smallestDistanceFromPlayer)
                            {
                                smallestDistanceFromPlayer = distanceFromPlayer;
                                closestSOI = objectInfo;
                            }
                        }
                    }
                }

                if (this.config.ShowArrow && smallestDistanceFromPlayer == float.PositiveInfinity)
                {
                    if (this.path != null)
                    {
                        Step nextStep = this.path.GetNextStep(Game1.currentLocation);
                        var warpSOI = new StardewObjectInfo();
                        warpSOI.Coordinate = nextStep.Position * Game1.tileSize;
                        DrawArrow(warpSOI.GetDirection(Game1.player), 3 * Game1.tileSize);
                    }
                }

                if (this.config.ShowArrow && !(closestSOI == null) && !anyOnScreen)
                {
                    DrawArrow(closestSOI.GetDirection(Game1.player), 3 * Game1.tileSize);
                }
            }
        }

        public void UpdatePath()
        {
            var targetLocation = this.ObjectInfoList.FirstOrDefault(x => x.NeedAction)?.Location;
            if (targetLocation != null)
            {
                this.path = Graph.GetPathToTarget(Game1.currentLocation, targetLocation);
            }
            else
            {
                this.path = null;
            }
        }

        public void ClearPath()
        {
            this.path = null;
        }

        protected void OnTaskFinished(EventArgs e)
        {
            this.TaskFinished?.Invoke(this, e);
        }

        protected void OnOverlayActivated(EventArgs e)
        {
            this.OverlayActivated?.Invoke(this, e);
        }

        protected void OnOverlayActivateChanged(EventArgs e)
        {
            this.OverlayActiveChanged?.Invoke(this, e);
        }

        protected abstract void UpdateObjectInfoList();

        private static void DrawArrow(float rotation, float distanceFromCenter)
        {
            var tex = OverlayTextures.ArrowRight;
            Point center = new Point(Game1.viewport.Width / 2, Game1.viewport.Height / 2);

            var destinationRectangle = new Rectangle(center.X - tex.Width / 2, center.Y - tex.Height / 2, tex.Width, tex.Height);
            destinationRectangle.X += (int)(Math.Cos(rotation) * distanceFromCenter);
            destinationRectangle.Y += (int)(Math.Sin(rotation) * distanceFromCenter);

            destinationRectangle.X += destinationRectangle.Width / 2;
            destinationRectangle.Y += destinationRectangle.Height / 2;
            Game1.spriteBatch.Draw(tex, destinationRectangle, null, Color.White, rotation, new Vector2(tex.Width / 2, tex.Height / 2), SpriteEffects.None, 0);
        }

        public class StardewObjectInfo
        {
            public GameLocation Location { get; set; }

            public Vector2 Coordinate { get; set; }

            public bool NeedAction { get; set; }

            public float GetDistance(Character c)
            {
                var charPos = c.getStandingPosition();
                return Vector2.Distance(charPos, this.Coordinate);
            }

            public bool IsOnScreen()
            {
                var v = Game1.viewport;
                bool leftOrRight = this.Coordinate.X < v.X || this.Coordinate.X > v.X + v.Width;
                bool belowOrAbove = this.Coordinate.Y < v.Y || this.Coordinate.Y > v.Y + v.Height;
                return !leftOrRight && !belowOrAbove;
            }

            public float GetDirection(Character c)
            {
                var v = this.Coordinate - c.getStandingPosition();
                return (float)Math.Atan2(v.Y, v.X);
            }
        }
    }
}
