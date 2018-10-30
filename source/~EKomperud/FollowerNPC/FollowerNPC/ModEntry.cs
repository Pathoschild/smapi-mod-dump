using System;
using System.Collections.Generic;
using Harmony;
using StardewModdingAPI;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;
using StardewModdingAPI.Events;

namespace FollowerNPC
{
    public class ModEntry : Mod
    {
        #region Members and Entry Function
        public static ModConfig config;
        public static IMonitor monitor;
        public bool spawned;
        public NPC whiteBox;
        public float whiteBoxSpeed;
        public float whiteBoxAnimationSpeed;
        public float whiteBoxFollowThreshold;
        private float whiteBoxFollowThresholdSquared;
        public Vector2 whiteBoxVelocity;
        public Vector2 whiteBoxLastMovementDirection;
        public Vector2 whiteBoxLastPosition;
        public Vector2 whiteBoxLastFrameVelocity;
        public Vector2 whiteBoxLastFramePosition;
        public Vector2 whiteBoxLastFrameMovement;
        public bool whiteBoxMovedLastFrame;
        public bool whiteBoxNeedsWarp;
        public int whiteBoxWarpTimer;
        public aStar whiteBoxAStar;
        public Queue<Vector2> whiteBoxPath;
        public Vector2 whiteBoxPathNode;
        public float whiteBoxPathfindNodeGoalTolerance;
        public bool whiteBoxFollow;
        private Vector2 negativeOne = new Vector2(-1, -1);

        public Farmer farmer;
        public Vector2 farmerLastTile;

        public int fullTile;
        public int halfTile;

        public override void Entry(IModHelper helper)
        {
            config = Helper.ReadConfig<ModConfig>();
            monitor = Monitor;
            whiteBoxFollowThreshold = 2;
            whiteBoxPathfindNodeGoalTolerance = 5f;
            whiteBoxFollowThresholdSquared = whiteBoxFollowThreshold * whiteBoxFollowThreshold;

            fullTile = Game1.tileSize;
            halfTile = (int) (Game1.tileSize * 0.5f);

            HarmonyInstance harmony = HarmonyInstance.Create("Redwood.FollowerNPC");

            ControlEvents.KeyReleased += ControlEvents_KeyReleased;
            GameEvents.UpdateTick += GameEvents_UpdateTick;
            GameEvents.QuarterSecondTick += GameEvents_QuarterSecondTick;
            SaveEvents.AfterLoad += SaveEvents_AfterLoad;
            PlayerEvents.Warped += PlayerEvents_Warped;
            MineEvents.MineLevelChanged += MineEvents_MineLevelChanged;
        }
        #endregion

        #region Event Functions
        private void ControlEvents_KeyReleased(object sender, EventArgsKeyPressed e)
        {
            //TO-DO: Utilize Path.Combine
            if (!Context.IsWorldReady)
                return;

            if (e.KeyPressed == Keys.O && !spawned)
            {
                spawned = true;
                AnimatedSprite sprite = new AnimatedSprite("Characters\\Abigail",0,Game1.tileSize / 4, (Game1.tileSize * 2)/4);
                whiteBox = new NPC(sprite, Game1.player.Position, "SeedShop", 2, "Abigail", true, null, Game1.content.Load<Texture2D>("Portraits\\Abigail"));
                //AnimatedSprite sprite = new AnimatedSprite("Characters\\Maru", 0, Game1.tileSize / 4, (Game1.tileSize * 2) / 4);
                //whiteBox = new NPC(sprite, Game1.player.Position, "ScienceHouse", 2, "Maru", true, null, Game1.content.Load<Texture2D>("Portraits\\Maru"));
                Game1.player.currentLocation.addCharacter(whiteBox);
                whiteBoxAStar = new aStar(farmer.currentLocation, whiteBox.Name);
                whiteBox.showTextAboveHead("Hey " + farmer.Name + "!", -1, 2, 3000, 0);
                whiteBoxSpeed = 5f;
                whiteBoxAnimationSpeed = 10f;
                whiteBoxFollow = false;
            }

            else if (e.KeyPressed == Keys.P && spawned)
            {
                spawned = false;
                Game1.removeCharacterFromItsLocation("Abigail");
                //Game1.removeCharacterFromItsLocation("Maru");
                whiteBox = null;
            }

            else if (e.KeyPressed == Keys.U && spawned)
            {
                monitor.Log(whiteBox?.currentLocation.Name + " : " + whiteBox?.getTileLocation());
            }

            else if (e.KeyPressed == Keys.I)
            {
                monitor.Log(farmer?.currentLocation.Name + " : " + farmer?.getTileLocation());
            }

            else if (e.KeyPressed == Keys.L && spawned)
            {
                Game1.warpCharacter(whiteBox, farmer.currentLocation, farmer.getTileLocation());
            }

            else if (e.KeyPressed == Keys.J && spawned)
            {
                whiteBoxPath = whiteBoxAStar.Pathfind(whiteBox.getTileLocation(), farmer.getTileLocation());
                foreach (Vector2 node in whiteBoxAStar.consolidatedPath)
                    monitor.Log("consolidated path: " + node.ToString());
                foreach (Vector2 n in whiteBoxPath)
                    monitor.Log("returned path: " + n.ToString());
            }

            else if (e.KeyPressed == Keys.K && spawned)
            {
                whiteBoxPath = whiteBoxAStar.Pathfind(whiteBox.getTileLocation(), farmer.getTileLocation());
                whiteBoxPathNode = whiteBoxPath != null && whiteBoxPath.Count != 0 ? whiteBoxPath.Dequeue() : negativeOne;
                monitor.Log(whiteBoxPathNode + "");
                whiteBoxFollow = !whiteBoxFollow;
            }

            else if (e.KeyPressed == Keys.B)
            {
                foreach (StardewValley.Object o in farmer.currentLocation.Objects.Values)
                    monitor.Log(o.name);
            }

            else if (e.KeyPressed == Keys.N)
            {
                foreach (StardewValley.TerrainFeatures.TerrainFeature tf in farmer.currentLocation.terrainFeatures
                    .Values)
                    monitor.Log(tf.ToString());
            }

            else if (e.KeyPressed == Keys.M)
            {
                foreach (StardewValley.TerrainFeatures.LargeTerrainFeature ltf in farmer.currentLocation
                    .largeTerrainFeatures)
                {
                    monitor.Log(ltf.ToString());
                }
            }

            else if (e.KeyPressed == Keys.H)
            {
                int x = 10;//Int32.Parse(Console.ReadLine());
                int y = 13;//Int32.Parse(Console.ReadLine());
                string walkable = whiteBoxAStar.IsWalkableTile(new Vector2(x, y)) ? "true" : "false";
                //string walkable = whiteBoxAStar.gameLocation.isObjectAtTile(x, y) ? "true" : "false";
                //string walkable = whiteBoxAStar.gameLocation.getObjectAtTile(x, y)?.name;// ? "true" : "false";

                monitor.Log(walkable);

                string walkableBack = whiteBoxAStar.gameLocation.doesTileHavePropertyNoNull(x, y, "NoFurniture", "Back");
                string walkableBuildings = whiteBoxAStar.gameLocation.doesTileHavePropertyNoNull(x, y, "NoFurniture", "Buildings");
                string walkablePaths = whiteBoxAStar.gameLocation.doesTileHavePropertyNoNull(x, y, "NoFurniture", "Paths");
                string walkableFront = whiteBoxAStar.gameLocation.doesTileHavePropertyNoNull(x, y, "NoFurniture", "Front");
                string walkableAlwaysFront = whiteBoxAStar.gameLocation.doesTileHavePropertyNoNull(x, y, "NoFurniture", "AlwaysFront");
                xTile.Tiles.Tile t = whiteBoxAStar.gameLocation.map.GetLayer("Back").PickTile(new xTile.Dimensions.Location(x * Game1.tileSize, y * Game1.tileSize), Game1.viewport.Size);

                //for (int X = 0; X < whiteBoxAStar.gameLocation.map.Layers[0].LayerWidth; X++)
                //{
                //    for (int Y = 0; Y < whiteBoxAStar.gameLocation.map.Layers[0].LayerHeight; Y++)
                //    {
                //        xTile.ObjectModel.IPropertyCollection props = ta[X,Y]?.Properties;
                //        if (props != null)
                //            foreach (KeyValuePair<string, xTile.ObjectModel.PropertyValue> kvp in props)
                //                monitor.Log(X+","+Y+": "+kvp.Key);
                //    }
                //}
                //xTile.ObjectModel.IPropertyCollection p = t.Properties;
                //foreach (KeyValuePair<string, xTile.ObjectModel.PropertyValue> kvp in p)
                //{
                //    monitor.Log(kvp.Key);
                //}

                //monitor.Log(walkable);
            }
        }

        private void GameEvents_UpdateTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || !(whiteBox != null) || !(farmer != null))
                return;

            if (whiteBoxFollow)
            {
                PathfindingNodeUpdateCheck();
                MovementAndAnimationUpdate();
                //HighlightPath();
            }
        }

        private void GameEvents_QuarterSecondTick(object sender, EventArgs e)
        {
            if (!Context.IsWorldReady || !(whiteBox != null) || !(farmer != null))
                return;

            DelayedWarp();

            if (whiteBoxFollow)
                PathfindingRemakeCheck();
        }

        private void SaveEvents_AfterLoad(object sender, EventArgs e)
        {
            farmer = Game1.player;
            spawned = false;
            whiteBox = null;
        }

        private void PlayerEvents_Warped(object sender, EventArgsPlayerWarped e)
        {
            if (!Context.IsWorldReady || !spawned || !(whiteBox != null) || !(farmer != null))
                return;

            whiteBoxAStar.gameLocation = farmer.currentLocation;
            if (!farmer.isRidingHorse())
                Game1.warpCharacter(whiteBox, farmer.currentLocation, farmer.getTileLocation());
            else
            {
                whiteBoxNeedsWarp = true;
                whiteBoxFollow = false;
                whiteBoxWarpTimer = 4;
            }
        }

        private void MineEvents_MineLevelChanged(object sender, EventArgsMineLevelChanged e)
        {
            if (!Context.IsWorldReady || !spawned || !(whiteBox != null) || !(farmer != null))
                return;

            Game1.warpCharacter(whiteBox, farmer.currentLocation, farmer.getTileLocation());
        }
        #endregion

        #region Helpers

        private void FollowFarmer()
        {
            Point f = farmer.GetBoundingBox().Center;
            Point w = whiteBox.GetBoundingBox().Center;
            Vector2 diff = new Vector2(f.X, f.Y) - new Vector2(w.X, w.Y);
            float diffLen = diff.Length();
            if (diffLen > Game1.tileSize * whiteBoxFollowThreshold)
            {
                
                if (whiteBoxPathNode != negativeOne)
                {
                    Point n = new Point((int)whiteBoxPathNode.X * Game1.tileSize, (int)whiteBoxPathNode.Y * Game1.tileSize);
                    Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                    float nodeDiffLen = nodeDiff.Length();
                    while (nodeDiffLen <= whiteBoxPathfindNodeGoalTolerance)
                    {
                        if (whiteBoxPath.Count == 0)
                            return;
                        whiteBoxPathNode = whiteBoxPath.Dequeue();
                        monitor.Log(whiteBoxPathNode + "");
                        n = new Point((int)whiteBoxPathNode.X * Game1.tileSize, (int)whiteBoxPathNode.Y * Game1.tileSize);
                        nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                        nodeDiffLen = nodeDiff.Length();
                    }
                    nodeDiff /= nodeDiffLen;
                }
            }
            else if (whiteBoxMovedLastFrame)
            {
                whiteBox.Sprite.faceDirectionStandard(GetFacingDirectionFromMovement(whiteBoxLastMovementDirection));
                whiteBoxMovedLastFrame = false;

                whiteBoxPath = null;
                whiteBoxPathNode = negativeOne;
            }
        }

        /// <summary>
        /// Performs pathfinding update functions if the white box is far enough away
        /// from the farmer
        /// </summary>
        private void PathfindingUpdate()
        {
            Point f = farmer.GetBoundingBox().Center;
            Point w = whiteBox.GetBoundingBox().Center;
            Vector2 diff = new Vector2(f.X, f.Y) - new Vector2(w.X, w.Y);
            if (diff.LengthSquared() >= whiteBoxFollowThresholdSquared)
            {
                PathfindingRemakeCheck();
                PathfindingNodeUpdateCheck();
            }
        }

        /// <summary>
        /// Remakes the white box's path when the farmer has changed tiles.
        /// </summary>
        private void PathfindingRemakeCheck()
        {
            Vector2 farmerCurrentTile = farmer.getTileLocation();

            if (farmerLastTile != farmerCurrentTile)
            {
                whiteBoxPath = whiteBoxAStar.Pathfind(whiteBox.getTileLocation(), farmerCurrentTile);
                if (whiteBox.getTileLocation() != whiteBoxPathNode)
                    whiteBoxPathNode = whiteBoxPath != null && whiteBoxPath.Count != 0 ? whiteBoxPath.Dequeue() : negativeOne;
            }

            farmerLastTile = farmerCurrentTile;
        }

        /// <summary>
        /// Iterates to the next goal node in the white box's current path if the current
        /// goal node has been reached
        /// </summary>
        private void PathfindingNodeUpdateCheck()
        {
            if (whiteBoxPathNode != negativeOne)
            {
                Point w = whiteBox.GetBoundingBox().Center;
                Point n = new Point(((int)whiteBoxPathNode.X * fullTile) + halfTile, ((int)whiteBoxPathNode.Y * Game1.tileSize) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= whiteBoxPathfindNodeGoalTolerance)
                {
                    if (whiteBoxPath.Count == 0)
                    {
                        whiteBoxPath = null;
                        whiteBoxPathNode = negativeOne;
                        return;
                    }
                    whiteBoxPathNode = whiteBoxPath.Dequeue();
                    n = new Point(((int)whiteBoxPathNode.X * fullTile) + halfTile, ((int)whiteBoxPathNode.Y * Game1.tileSize) + halfTile);
                    nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                    nodeDiffLen = nodeDiff.Length();
                }
            }
        }

        /// <summary>
        /// Provides updates to the white box's movement and animation
        /// </summary>
        private void MovementAndAnimationUpdate()
        {
            Vector2 whiteBoxBoundingBox =
                new Vector2(whiteBox.GetBoundingBox().Center.X, whiteBox.GetBoundingBox().Center.Y);
            whiteBoxLastFrameMovement = whiteBoxBoundingBox - whiteBoxLastFramePosition;

            Point f = farmer.GetBoundingBox().Center;
            Point w = whiteBox.GetBoundingBox().Center;
            Vector2 diff = new Vector2(f.X, f.Y) - new Vector2(w.X, w.Y);
            float diffLen = diff.Length();
            if (diffLen > Game1.tileSize * whiteBoxFollowThreshold && whiteBoxPathNode != negativeOne)
            {
                Point n = new Point(((int)whiteBoxPathNode.X * fullTile) + halfTile, ((int)whiteBoxPathNode.Y * fullTile) + halfTile);
                Vector2 nodeDiff = new Vector2(n.X, n.Y) - new Vector2(w.X, w.Y);
                float nodeDiffLen = nodeDiff.Length();
                if (nodeDiffLen <= whiteBoxPathfindNodeGoalTolerance)
                    return;
                nodeDiff /= nodeDiffLen;

                whiteBox.xVelocity = nodeDiff.X * whiteBoxSpeed;
                whiteBox.yVelocity = -nodeDiff.Y * whiteBoxSpeed;
                HandleWallSliding();
                whiteBoxLastFrameVelocity = new Vector2(whiteBox.xVelocity, whiteBox.yVelocity);
                whiteBoxLastFramePosition = new Vector2(whiteBox.GetBoundingBox().Center.X, whiteBox.GetBoundingBox().Center.Y);

                whiteBox.faceDirection(GetFacingDirectionFromMovement(nodeDiff));
                SetMovementDirectionAnimation(whiteBox.FacingDirection);
                whiteBox.MovePosition(Game1.currentGameTime, Game1.viewport, whiteBox.currentLocation);
                whiteBoxLastMovementDirection = nodeDiff;

                whiteBoxMovedLastFrame = true;
            }
            else if (whiteBoxMovedLastFrame)
            {
                whiteBox.Sprite.faceDirectionStandard(GetFacingDirectionFromMovement(whiteBoxLastMovementDirection));
                whiteBoxMovedLastFrame = false;
            }
        }

        private void DelayedWarp()
        {
            if (whiteBoxNeedsWarp)
                if (--whiteBoxWarpTimer <= 0)
                {
                    whiteBoxFollow = true;
                    Game1.warpCharacter(whiteBox, farmer.currentLocation, farmer.getTileLocation());
                    whiteBoxNeedsWarp = false;
                }
        }

        private int GetFacingDirectionFromMovement(Vector2 movement)
        {
            int dir = 2;
            if (Math.Abs(movement.X) > Math.Abs(movement.Y))
                dir = movement.X > 0 ? 1 : 3;
            else if (Math.Abs(movement.X) < Math.Abs(movement.Y))
                dir = movement.Y > 0 ? 2 : 0;
            return dir;
        }

        private void SetMovementDirectionAnimation(int dir)
        {
            switch (dir)
            {
                case 0:
                    whiteBox.SetMovingOnlyUp();
                    whiteBox.Sprite.AnimateUp(Game1.currentGameTime, (int)(whiteBoxSpeed * -whiteBoxAnimationSpeed), ""); break;
                case 1:
                    whiteBox.SetMovingOnlyRight();
                    whiteBox.Sprite.AnimateRight(Game1.currentGameTime, (int)(whiteBoxSpeed * -whiteBoxAnimationSpeed), ""); break;
                case 2:
                    whiteBox.SetMovingOnlyDown();
                    whiteBox.Sprite.AnimateDown(Game1.currentGameTime, (int)(whiteBoxSpeed * -whiteBoxAnimationSpeed), ""); break;
                case 3:
                    whiteBox.SetMovingOnlyLeft();
                    whiteBox.Sprite.AnimateLeft(Game1.currentGameTime, (int)(whiteBoxSpeed * -whiteBoxAnimationSpeed), ""); break;

            }
        }

        private void HighlightPath()
        {
            Game1.spriteBatch.Begin();
            Rectangle? r = new Rectangle(194, 388, 16, 16);
            foreach (Vector2 tile in whiteBoxAStar.consolidatedPath)
            {
                Vector2 v = new Vector2(tile.X * Game1.tileSize, tile.Y * Game1.tileSize);
                Game1.spriteBatch.Draw(Game1.mouseCursors, Game1.GlobalToLocal(v),
                    r, Color.White, 0f, Vector2.Zero, Game1.pixelZoom,
                    SpriteEffects.None, 0.01f);
            }

            Game1.spriteBatch.End();
        }

        private void HandleWallSliding()
        {
            if (whiteBoxLastFrameVelocity != Vector2.Zero && whiteBoxLastFrameMovement == Vector2.Zero &&
                (whiteBox.xVelocity != 0 || whiteBox.yVelocity != 0))
            {
                Rectangle wbBB = whiteBox.GetBoundingBox();
                int ts = Game1.tileSize;

                if (whiteBox.xVelocity != 0)
                {
                    int velocitySign = Math.Sign(whiteBox.xVelocity) * 5;
                    int leftOrRight = ((whiteBox.xVelocity > 0 ? wbBB.Right : wbBB.Left) + velocitySign) / ts;
                    monitor.Log(whiteBox.xVelocity + " > 0 ? " + wbBB.Right + " : " + wbBB.Left + ") + " + velocitySign + ") / " + ts);
                    bool[] xTiles = new bool[3];
                    xTiles[0] = whiteBoxAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Top / ts));
                    xTiles[1] = whiteBoxAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Center.Y / ts));
                    xTiles[2] = whiteBoxAStar.IsWalkableTile(new Vector2(leftOrRight, wbBB.Bottom / ts));
                    foreach (bool b in xTiles)
                    {
                        if (!b)
                            whiteBox.xVelocity = 0;
                    }
                }

                if (whiteBox.yVelocity != 0)
                {
                    int velocitySign = Math.Sign(whiteBox.yVelocity) * 5;
                    int topOrBottom = ((whiteBox.yVelocity < 0 ? wbBB.Bottom : wbBB.Top) - velocitySign) / ts;
                    monitor.Log(whiteBox.yVelocity + " < 0 ? " + wbBB.Bottom + " : " + wbBB.Top + ") - " + velocitySign + ") / " + ts);
                    bool[] yTiles = new bool[3];
                    yTiles[0] = whiteBoxAStar.IsWalkableTile(new Vector2(wbBB.Left / ts, topOrBottom));
                    yTiles[1] = whiteBoxAStar.IsWalkableTile(new Vector2(wbBB.Center.X / ts, topOrBottom));
                    yTiles[2] = whiteBoxAStar.IsWalkableTile(new Vector2(wbBB.Right / ts, topOrBottom));
                    foreach (bool b in yTiles)
                    {
                        if (!b)
                            whiteBox.yVelocity = 0;
                    }
                }
            }
                

            //if (whiteBox.xVelocity != 0)
            //{
            //    Vector2 x = new Vector2(whiteBox.getTileX() + Math.Sign(whiteBox.xVelocity), whiteBox.getTileY());
            //    StardewValley.Object xObject = whiteBoxAStar.gameLocation.getObjectAtTile((int)x.X, (int)x.Y);
            //    if (xObject != null)
            //    {
            //        Rectangle xCollider = xObject.getBoundingBox(x);
            //        if (whiteBox.GetBoundingBox().Intersects(xCollider))
            //            whiteBox.xVelocity = 0;
            //    }
            //}

            //if (whiteBox.yVelocity != 0)
            //{
            //    Vector2 y = new Vector2(whiteBox.getTileX(), whiteBox.getTileY() + Math.Sign(whiteBox.yVelocity));
            //    StardewValley.Object yObject = whiteBoxAStar.gameLocation.getObjectAtTile((int)y.X, (int)y.Y);
            //    if (yObject != null)
            //    {
            //        Rectangle yCollider = yObject.getBoundingBox(y);
            //        if (whiteBox.GetBoundingBox().Intersects(yCollider))
            //            whiteBox.yVelocity = 0;
            //    }
            //}
        }

        private bool FloatsApproximatelyEqual(float a, float b, float threshold)
        {
            return Math.Abs(a - b) < threshold;
        }
        #endregion
    }
}

