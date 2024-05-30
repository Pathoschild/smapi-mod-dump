/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ylsama/RightClickMoveMode
**
*************************************************/

using StardewValley;
using StardewModdingAPI;
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace MouseMoveMode
{
    class PathFindingHelper
    {
        // Logic control
        public bool debugVisitedTile = false;
        public bool debugVerbose = false;
        public bool debugLineToTiles = false;
        public bool debugPathSmothing = false;

        public bool usePathSmothing = false;

        private IMonitor Monitor;
        private List<DrawableNode> visitedNodes = new List<DrawableNode>();
        private Stack<DrawableNode> pathNodes = new Stack<DrawableNode>();

        // only being initilized when enable debug logic control flag
        private List<DrawableNode> lineToTileNodes;

        // only being initilized when enable debug logic control flag
        private List<DrawableNode> smothPathNodes;

        private DrawableNode targetNode = null;

        private float microTileDelta = 0.0001f;
        private float microPositionDelta = 25f;

        private PriorityQueue<Vector2, float> pq = new PriorityQueue<Vector2, float>();
        private HashSet<Vector2> visited = new HashSet<Vector2>();
        private Dictionary<Vector2, Vector2> cameFrom = new Dictionary<Vector2, Vector2>();
        private Dictionary<Vector2, float> gScore = new Dictionary<Vector2, float>();
        private Dictionary<Vector2, float> fScore = new Dictionary<Vector2, float>();

        private Vector2 destination = new Vector2(0, 0);
        private Vector2 destinationTile = new Vector2(0, 0);
        private List<Vector2> path = new List<Vector2>();
        private List<Vector2> pathPreSmoth = new List<Vector2>();
        private int currentStep = 0;

        public static bool isBestScoreFront { get; private set; }
        public Vector2 bestNext { get; private set; }

        public void flushCache()
        {
            this.pq.Clear();
            this.visited.Clear();
            this.cameFrom.Clear();
            this.gScore.Clear();
            this.fScore.Clear();
            this.path.Clear();

            if (this.usePathSmothing)
            {
                if (this.debugPathSmothing)
                {
                    this.pathPreSmoth.Clear();
                    this.smothPathNodes.Clear();
                }
            }

            this.visitedNodes.Clear();
            this.pathNodes.Clear();
        }

        public PathFindingHelper()
        {
            this.Monitor = ModEntry.getMonitor();

            // Only show when debuging
            if (this.usePathSmothing)
            {
                if (this.debugPathSmothing)
                    this.smothPathNodes = new List<DrawableNode>();
            }

            if (this.debugLineToTiles)
                this.lineToTileNodes = new List<DrawableNode>();
        }

        /**
         * @brief This use real postiotion line to draw a tile lines along them
         */
        public void updateLineToTiles(Vector2 start, Vector2 destination, bool flushAllDrawedNode = true)
        {
            if (this.debugVerbose)
            {
                this.Monitor.Log(String.Format("Try draw line from tile {0} to {1}", start, destination), LogLevel.Info);
            }
            if (flushAllDrawedNode)
                this.lineToTileNodes.Clear();
            List<Vector2> line = lineToTiles(Util.toTile(start), Util.toTile(destination));
            for (var i = 0; i < line.Count; i += 1)
            {
                var tile = Util.fixFragtionTile(line[i]);
                var node = new DrawableNode(Util.toBoxPosition(tile));
                if (i >= 1)
                {
                    var prev = Util.fixFragtionTile(line[i - 1]);
                    if (!isValidMovement(prev, tile))
                    {
                        node.color = Color.Red;
                    }
                }
                if (this.debugVerbose)
                {
                    this.Monitor.Log(String.Format("Line to tiles {0} (pre-fixed: {1}) or {2}", tile, line[i], this.addPadding(tile)), LogLevel.Info);
                }
                this.lineToTileNodes.Add(node);
            }
        }

        public void drawVisitedNodes(SpriteBatch b)
        {
            foreach (var node in this.visitedNodes)
                node.draw(b, Color.Aqua);
        }

        public void drawLineToTiles(SpriteBatch b)
        {
            foreach (var node in this.lineToTileNodes)
                node.draw(b);
        }

        public void drawPath(SpriteBatch b)
        {
            foreach (var node in this.pathNodes)
                node.draw(b);
            if (targetNode is not null)
                targetNode.draw(b, Color.Red);
        }

        public void updateSmothNodes(List<Vector2> preSmothPath, bool flushNodes = true)
        {
            if (flushNodes)
            {
                this.smothPathNodes.Clear();
            }

            foreach (var pos in preSmothPath)
            {
                var node = new DrawableNode(pos);
                node.color = Color.Red;
                this.smothPathNodes.Add(node);
            }
        }

        public void drawSmothNodes(SpriteBatch b)
        {
            foreach (var node in this.smothPathNodes)
                node.draw(b);
            if (targetNode is not null)
                targetNode.draw(b, Color.Red);
        }

        public void drawIndicator(SpriteBatch b)
        {
            if (this.debugVisitedTile)
                this.drawVisitedNodes(b);

            if (Util.debugPassable)
                Util.drawPassable(b);

            if (this.debugLineToTiles)
                this.drawLineToTiles(b);

            if (this.usePathSmothing)
                if (this.debugPathSmothing)
                    this.drawSmothNodes(b);

            this.drawPath(b);

        }

        public void loadMap()
        {
            this.flushCache();
            Util.flushCache();
        }

        public void changeDes(Vector2 destination)
        {
            if (this.debugVerbose)
            {
                this.Monitor.Log(String.Format("Change destination from {0} to {1}, tile value {2} to {3}", this.destination, destination, Util.toTile(this.destination), Util.toTile(destination)), LogLevel.Info);
            }

            this.flushCache();
            Util.flushCache();
            targetNode = new DrawableNode(Util.toBoxPosition(destination));
            currentStep = 0;
            aStarPathFinding(destination);

        }

        /**
         * @brief Path-finding scaled down destination tile locatiton
         */
        public Vector2 getCurrentDestinationTile()
        {
            return Util.toTile(this.destination);
        }

        private bool isValidMovement(Vector2 current, int i, int j)
        {
            Vector2 neighbor = new Vector2(current.X + i, current.Y + j);
            return isValidMovement(current, neighbor);
        }

        /**
         * @brief the neighbor should only be a tile away from the current one
         */
        private bool isValidMovement(Vector2 current, Vector2 neighbor)
        {
            var gl = Game1.player.currentLocation;
            var i = neighbor.X - current.X;
            var j = neighbor.Y - current.Y;

            // Why we stand still
            if (i == 0 && j == 0)
                return false;

            // tile isn't passable 
            if (!Util.isTilePassable(neighbor) || !Util.isTilePassable(current))
                return false;

            // diagonal special case handling, just don't do it if there is a blockage
            if ((i == 1 || i == -1) && (j == 1 || j == -1))
            {
                if (!Util.isTilePassable(current.X, neighbor.Y) || !Util.isTilePassable(neighbor.X, current.Y))
                {
                    return false;
                }
            }

            // horse riding will make player bigger, which can't go up and down into 1 tile gap. Skip anything like this unless we goes through fence gate
            // ?-any, O-passable, X-unpassable, P-current tile (which isn't gate)
            // This isn't ok
            // ?O?    OOX    OO?    XOO    ?OO
            // XPX    XP?    XP?    ?PX    ?PX
            // ?O?    OO?    OOX    ?OO    XOO
            if (Game1.player.isRidingHorse())
            {
                bool neighborIsGate = gl.getObjectAtTile((int)neighbor.X, (int)neighbor.Y) is Fence;
                if (neighborIsGate)
                {
                    if (this.debugVerbose)
                    {
                        this.Monitor.Log("Found gate", LogLevel.Info);
                        this.Monitor.Log(String.Format("From {0} to {1}", current, neighbor));
                    }
                }

                bool checkBlockage = false;
                if (!Util.isTilePassable(neighbor.X - 1, neighbor.Y))
                {
                    checkBlockage = checkBlockage || (!Util.isTilePassable(neighbor.X + 1, neighbor.Y - 1));
                    checkBlockage = checkBlockage || (!Util.isTilePassable(neighbor.X + 1, neighbor.Y));
                    checkBlockage = checkBlockage || (!Util.isTilePassable(neighbor.X + 1, neighbor.Y + 1));
                }

                if (!Util.isTilePassable(neighbor.X + 1, neighbor.Y))
                {
                    checkBlockage = checkBlockage || (!Util.isTilePassable(neighbor.X - 1, neighbor.Y - 1));
                    checkBlockage = checkBlockage || (!Util.isTilePassable(neighbor.X - 1, neighbor.Y + 1));
                }

                // We can squeze through gate when ringind horse ONLY WHEN MOVE UP AND DOWN
                bool squezeToGate = (neighbor.X == current.X) && neighborIsGate;
                if (squezeToGate)
                {
                    if (this.debugVerbose)
                        this.Monitor.Log("Seem like we going through gate now");
                }

                if (checkBlockage && !squezeToGate)
                    return false;
            }

            return true;
        }

        /**
         * @brief Score calculate between two node (to chose a path)
         * the lower the better
         */
        private float calculateGScore(Vector2 srcPos, Vector2 desPos)
        {
            return Vector2.Distance(srcPos, desPos);
        }

        /**
         * @brief Score calculate for a tile to the end/destination (of a path)
         * the lower the better
         */
        private float calculateFScore(Vector2 srcPos)
        {
            return Vector2.Distance(srcPos, this.destination);
        }

        public void aStarPathFinding(Vector2 destination)
        {
            GameLocation gl = Game1.player.currentLocation;
            this.destination = destination;

            // This preventing error when changing or rounding destination multiple times
            this.destinationTile = Util.toTile(this.destination);

            this.pq.Enqueue(getPlayerStandingTile(), 0);

            Vector2 start = getPlayerStandingTile();

            // This also favor consider player in front of destination
            isBestScoreFront = false;

            Vector2 bestScoreTile = start;

            // We favor destination that closer to the destination
            float bestScore = Vector2.Distance(Util.toPosition(start), this.destination);

            gScore.Add(start, 0);
            fScore.Add(start, calculateFScore(this.destination));

            // I just too dumb so let limit the time we try to find best past
            // we have a quite small available click screen so it fine as long
            // as the limit can match the total screen tile
            int limit = ModEntry.config.PathFindLimit;
            while (pq.Count > 0 && limit > 0)
            {
                limit -= 1;
                var current = pq.Dequeue();
                if (debugVerbose)
                {
                    ModEntry.getMonitor().Log(String.Format("[Step {2}] Current {3} or in tile {0} = {1} fScore {4}", current, gScore[current], 100 - limit, addPadding(current), fScore[current]), LogLevel.Info);
                }

                if (Vector2.Distance(current, this.destinationTile) < this.microTileDelta)
                {
                    this.destinationTile = current;
                    if (this.debugVerbose)
                        this.Monitor.Log("Found path!", LogLevel.Info);
                    updatePath();
                    return;
                }

                List<Vector2> neighborList = new List<Vector2>();
                for (int i = -1; i <= 1; i += 1)
                    for (int j = -1; j <= 1; j += 1)
                    {
                        Vector2 neighbor = new Vector2(current.X + i, current.Y + j);
                        if (visited.Contains(neighbor))
                        {
                            continue;
                        }
                        if (isValidMovement(current, neighbor))
                        {
                            // Pass all checked, this tile could be consider to be use
                            neighborList.Add(neighbor);
                        }
                    }

                foreach (var neighbor in neighborList)
                {
                    visited.Add(neighbor);
                    this.visitedNodes.Add(new DrawableNode(Util.toBoxPosition(neighbor)));

                    var temp = gScore[current] + calculateGScore(Util.toPosition(current), Util.toPosition(neighbor));
                    if (gScore.ContainsKey(neighbor))
                    {
                        if (gScore[neighbor] < temp)
                        {
                            continue;
                        }
                        else
                        {
                            this.cameFrom[neighbor] = current;
                            gScore[neighbor] = temp;
                            fScore[neighbor] = temp + calculateFScore(Util.toPosition(neighbor));
                        }
                    }
                    else
                    {
                        this.cameFrom.Add(neighbor, current);
                        gScore.Add(neighbor, temp);
                        fScore.Add(neighbor, temp + calculateFScore(Util.toPosition(neighbor)));
                    }

                    pq.Enqueue(neighbor, fScore[neighbor]);

                    // We favor player to be infront of the destination, this make the path found more reliable-ish
                    // Again, within-limit of the effective reach for the object. I just hard code 1 tiles here atm
                    if (neighbor.X == this.destinationTile.X && neighbor.Y - this.destinationTile.Y >= 0 && neighbor.Y - this.destinationTile.Y < 1.1f)
                    {
                        if (!isBestScoreFront)
                        {
                            bestScore = Vector2.Distance(Util.toPosition(neighbor), this.destination);
                            bestScoreTile = neighbor;
                            if (this.debugVerbose)
                                this.Monitor.Log(String.Format("Update closest tile to {0}, with distance {1}", bestScoreTile, bestScore), LogLevel.Info);
                        }
                        isBestScoreFront = true;
                        if (isBestScoreFront && (bestScore > Vector2.Distance(Util.toPosition(neighbor), this.destination)))
                        {
                            bestScore = Vector2.Distance(Util.toPosition(neighbor), this.destination);
                            bestScoreTile = neighbor;
                            if (this.debugVerbose)
                                this.Monitor.Log(String.Format("Update closest tile to {0}, with distance {1}", bestScoreTile, bestScore), LogLevel.Info);

                        }
                        // Some time, destination tile isn't passable, we found a closest one in front then it good enough to end here
                        if (!Util.isTilePassable(this.destinationTile) && bestScore < 80f)
                        {
                            this.destinationTile = bestScoreTile;
                            updatePath();
                            return;
                        }
                    }
                    else
                    {
                        if (!isBestScoreFront && (bestScore > Vector2.Distance(Util.toPosition(neighbor), this.destination)))
                        {
                            bestScore = Vector2.Distance(Util.toPosition(neighbor), this.destination);
                            bestScoreTile = neighbor;
                            if (this.debugVerbose)
                                this.Monitor.Log(String.Format("Update closest tile to {0}, with distance {1}", bestScoreTile, bestScore), LogLevel.Info);

                            // Some time, destination tile isn't passable, we found a closest one that good enough to end here
                            if (!Util.isTilePassable(this.destinationTile + new Vector2(0, 1)) && !Util.isTilePassable(this.destinationTile) && bestScore < 80f)
                            {
                                this.destinationTile = bestScoreTile;
                                updatePath();
                                return;
                            }
                        }
                    }
                }
            }

            // Destination tile is unreach-able (or no path found within limit),
            // so we goes to the closest tile or the in-front tile
            if (this.debugVerbose)
                this.Monitor.Log("Can't found path within time!, change to closest tile found" + bestScoreTile, LogLevel.Info);
            this.destinationTile = bestScoreTile;
            updatePath();
        }

        /**
         * @brief Trace back and getting shortest path from a_star camefrom map
         */
        public List<Vector2> tracebackAndUpscalePath()
        {
            var temp = new Stack<Vector2>();
            // We may not add destination here, as it could be un reachable
            if (this.cameFrom.ContainsKey(Util.toTile(this.destination)))
            {
                if (debugVerbose)
                    ModEntry.getMonitor().Log(String.Format("Path traceback (start): {0} or in tile {1}", this.destination, Util.toTile(this.destination)));
                temp.Push(this.destination);
            }

            Vector2 startTile = getPlayerStandingTile();

            // Destination tile shoul alway be access-able, but it may doublicate
            // with the destination
            Vector2 pointerTile = this.destinationTile;
            while (Vector2.Distance(pointerTile, startTile) > this.microTileDelta)
            {
                var traceBackPosition = this.addPadding(pointerTile);
                if (debugVerbose)
                    ModEntry.getMonitor().Log(String.Format("Path traceback (loop): {0} or in tile {1}", traceBackPosition, pointerTile));
                // if (this.debugVerbose) this.Monitor.Log("Path: " + traceBackPosition, LogLevel.Info);

                temp.Push(traceBackPosition);
                pointerTile = this.cameFrom[pointerTile];
            }

            // Could be needed, but player already consider standing on this tile
            // it for extra protection that player can still stuck for any reason
            temp.Push(this.addPadding(startTile));
            if (debugVerbose)
                ModEntry.getMonitor().Log(String.Format("Path traceback (end): {0} or in tile {1}", temp.Peek(), startTile));

            List<Vector2> result = new List<Vector2>();

            while (temp.Count > 0)
            {
                result.Add(temp.Pop());
            }
            return result;
        }

        /**
         * @brief A tiles path can be upscale back to position path
         */
        public List<Vector2> upscalePath(List<Vector2> tilePath)
        {
            List<Vector2> result = new List<Vector2>();
            foreach (var tile in tilePath)
            {
                result.Add(this.addPadding(tile));
            }
            return result;
        }

        public List<Vector2> populateSmothPath(List<Vector2> smothPath)
        {
            var temp = new List<Vector2>();
            if (smothPath.Count > 1)
                for (var i = 1; i < smothPath.Count; i++)
                {
                    var start = Util.toTile(smothPath[i - 1]);
                    var end = Util.toTile(smothPath[i]);
                    foreach (var tile in lineToTiles(start, end, skipStartTile: i == 0))
                    {
                        var fixTile = Util.fixFragtionTile(tile);

                        if (Util.isTilePassable(fixTile))
                            temp.Add(this.addPadding(fixTile));
                    }
                }

            return temp;
        }

        public void updatePath()
        {
            var path = tracebackAndUpscalePath();

            if (usePathSmothing)
            {
                // Add back the missing node
                var smoth = this.findSmothPath(path);

                if (debugPathSmothing)
                {
                    updateSmothNodes(smoth);
                    updateSmothNodes(populateSmothPath(smoth), flushNodes: false);
                }

                //path = smoth;
            }

            // Update path to use the smoothen path
            this.path.Clear();
            for (var i = 0; i < path.Count; i += 1)
            {
                this.path.Add(path[i]);
                if (this.debugVerbose)
                {
                    this.Monitor.Log(String.Format("Path: {0} of in tile {1}", path[i], Util.toTile(path[i])), LogLevel.Info);
                }
            }

            // Update path indicator to use the smoothen path
            this.pathNodes.Clear();
            for (var i = path.Count - 1; i >= 0; i -= 1)
            {
                var node = new DrawableNode(path[i]);
                this.pathNodes.Push(node);
            }
        }

        public Vector2 addPadding(Vector2 positionTile)
        {
            Rectangle box = Game1.player.GetBoundingBox();
            var padX = 32;
            var padY = 32;
            var microRounding = 16;

            // blockage on left side by any mean
            bool checkLeft = !Util.isTilePassable(positionTile.X - 1, positionTile.Y);
            checkLeft = checkLeft || !Util.isTilePassable(positionTile.X - 1, positionTile.Y - 1);
            checkLeft = checkLeft || !Util.isTilePassable(positionTile.X - 1, positionTile.Y + 1);

            // blockage on right side by any mean
            bool checkRight = !Util.isTilePassable(positionTile.X + 1, positionTile.Y);
            checkRight = checkRight || !Util.isTilePassable(positionTile.X + 1, positionTile.Y - 1);
            checkRight = checkRight || !Util.isTilePassable(positionTile.X + 1, positionTile.Y + 1);

            if (checkLeft ^ checkRight)
            {
                if (checkLeft)
                {
                    //if (this.debugVerbose) this.Monitor.Log("Left blockage", LogLevel.Info);
                    padX = (box.Width / 2 + microRounding);
                }
                if (checkRight)
                {
                    //if (this.debugVerbose) this.Monitor.Log("Right blockage", LogLevel.Info);
                    padX = 64 - (box.Width / 2 + microRounding);
                }
            }

            // blockage on top side by any mean
            bool checkTop = !Util.isTilePassable(positionTile.X - 1, positionTile.Y - 1);
            checkTop = checkTop || !Util.isTilePassable(positionTile.X, positionTile.Y - 1);
            checkTop = checkTop || !Util.isTilePassable(positionTile.X + 1, positionTile.Y - 1);

            // blockage on bottom side by any mean
            bool checkBottom = !Util.isTilePassable(positionTile.X - 1, positionTile.Y + 1);
            checkBottom = checkBottom || !Util.isTilePassable(positionTile.X, positionTile.Y + 1);
            checkBottom = checkBottom || !Util.isTilePassable(positionTile.X + 1, positionTile.Y + 1);

            if (checkTop ^ checkBottom)
            {
                if (checkTop)
                {
                    if (this.debugVerbose)
                    {
                        //this.Monitor.Log("Top blockage", LogLevel.Info);

                    }
                    padY = (box.Height / 2 + microRounding);
                }
                if (checkBottom)
                {
                    if (this.debugVerbose)
                    {
                        //this.Monitor.Log("Bottom blockage", LogLevel.Info);
                    }
                    padY = 64 - (box.Height / 2 + microRounding);
                }
            }
            var res = Util.toPosition(positionTile, padX, padY);
            return res;
        }

        /**
         * @brief From A to B have some tile in between, it is as close to be
         * a line as you wanted, it from A -> B for the tiles result. 
         *
         * @param A line from tileA
         * @param B to tileB
         */
        public List<Vector2> lineToTiles(Vector2 A, Vector2 B, bool skipStartTile = false)
        {
            var lineTilesX = new List<Vector2>();

            var stepX = B.X - A.X;
            if (stepX > 0) stepX = 1;
            if (stepX < 0) stepX = -1;

            var stepY = B.Y - A.Y;
            if (stepY > 0) stepY = 1;
            if (stepY < 0) stepY = -1;

            if (A.X != B.X)
            {
                // We are sure that it work on xAxis now
                var line = (B.Y - A.Y) / (B.X - A.X);
                var constant = A.Y - line * A.X;

                var i = A.X;
                while (i != B.X)
                {
                    var xAxis = i;
                    var yAxis = line * i + constant;
                    if (!(i == A.X && skipStartTile))
                        lineTilesX.Add(new Vector2(xAxis, yAxis));
                    i += stepX;
                }
            }

            var lineTilesY = new List<Vector2>();
            if (A.Y != B.Y)
            {
                var line = (B.X - A.X) / (B.Y - A.Y);
                var constant = A.X - line * A.Y;

                var j = A.Y;
                while (j != B.Y)
                {
                    var yAxis = j;
                    var xAxis = line * j + constant;
                    if (!(j == A.Y && skipStartTile))
                        lineTilesY.Add(new Vector2(xAxis, yAxis));
                    j += stepY;
                }
            }

            // Seeing who do the job better, which just mean there is more node
            if (lineTilesX.Count > lineTilesY.Count)
                return lineTilesX;

            return lineTilesY;
        }


        /**
         * @brief This scall thing back to tile base and check every movement
         * is valid or not, skip path expect to be call in when calculating the
         * next node in path to goes to
         */
        public bool checkValidLine(Vector2 start, Vector2 end)
        {
            Vector2 startTile = Util.toTile(start);
            Vector2 endTile = Util.toTile(end);

            // The line goes from skipping tile to start tile
            var lineTiles = lineToTiles(startTile, endTile);
            var isBlocked = false;
            for (var i = 0; i < lineTiles.Count; i += 1)
            {
                var tile = Util.fixFragtionTile(lineTiles[i]);
                if (i > 0)
                {
                    var prev = Util.fixFragtionTile(lineTiles[i - 1]);
                    // But we going from start, so this need to walk backward
                    if (!isValidMovement(tile, prev))
                    {
                        isBlocked = true;
                        break;
                    }
                }
            }

            return !isBlocked;
        }

        public List<Vector2> findSmothPath(List<Vector2> path)
        {
            if (path.Count <= 2)
            {
                return path;
            }
            var t = new List<Vector2>();
            var s = path;

            // For debuging
            bool flushAllDrawedNode = true;

            t.Add(s[0]);
            int currentIndex = 0;
            int nextIndex = 0;
            while (currentIndex < s.Count - 1)
            {
                nextIndex = currentIndex + 1;
                for (var i = s.Count - 1; i > currentIndex; i -= 1)
                {
                    if (nextIndex >= i) continue;
                    if (checkValidLine(s[currentIndex], s[i]))
                    {
                        if (this.debugLineToTiles)
                        {
                            updateLineToTiles(s[currentIndex], s[i], flushAllDrawedNode: flushAllDrawedNode);
                            flushAllDrawedNode = false;
                        }
                        nextIndex = i;
                        break;
                    }
                }
                t.Add(s[nextIndex]);
                currentIndex = nextIndex;
            }

            return t;
        }


        /**
         * @brief This just give the next path if player has reach the current
         * path node
         */
        public Nullable<Vector2> nextPath()
        {
            if (this.path.Count == currentStep)
            {
                if (debugVerbose)
                {
                    // this.Monitor.Log("This greate, we reach the end " + currentStep, LogLevel.Info);
                    // foreach (var t in this.path)
                    // {
                    //     this.Monitor.Log("Path visited: " + t, LogLevel.Info);
                    // }
                }
                return null;
            }

            Vector2 start = Game1.player.GetBoundingBox().Center.ToVector2();
            Vector2 next = this.path[currentStep];

            if (Vector2.Distance(start, next) > this.microPositionDelta)
                return this.path[currentStep];
            currentStep += 1;
            this.pathNodes.Pop();
            return this.nextPath();
        }

        public Vector2 moveDirection()
        {
            var optionalNext = this.nextPath();
            Vector2 next = this.destination;
            if (optionalNext is not null)
                next = optionalNext.Value;
            Vector2 playerPosition = Game1.player.GetBoundingBox().Center.ToVector2();
            Vector2 direction = Vector2.Subtract(next, playerPosition);

            // When destination seem reach-able
            if (Util.isTilePassable(Util.toTile(this.destination)))
            {
                // and we found the path lead to it
                if (Util.toTile(this.destination) == this.destinationTile)
                {
                    // Which mean moving will end when we reach the destination tile
                    // which also mean we finish all node inside path
                    if (optionalNext is null)
                    {
                        if (this.debugVerbose)
                        {
                            this.Monitor.Log("Destination tile seem reachable, We found the right path too", LogLevel.Info);
                        }
                        return new Vector2(0, 0);
                    }
                }

                // We can't found path to it, though it worth just try to reach
                // destination after we finish all node inside path
                if (optionalNext is null)
                {
                    if (this.debugVerbose)
                    {
                        this.Monitor.Log("Destination tile seem unreachable, as we haven't found the path to it yet", LogLevel.Info);
                    }
                    if (Vector2.Distance(playerPosition, direction) < this.microPositionDelta)
                    {
                        this.Monitor.Log("Destination tile reach? nice", LogLevel.Info);
                        return new Vector2(0, 0);
                    }
                    return direction;
                }
            }

            // So if destination is unreachable, player will kept moving till
            // we got stuck colliding with the destination?
            if (optionalNext is null)
            {
                if (!ModEntry.checkColidingIfMoving())
                {
                    return direction;
                }
                else
                {
                    if (this.debugVerbose)
                    {
                        this.Monitor.Log("Destination tile is unreachable, but we have collided to it!", LogLevel.Info);
                    }
                    return new Vector2(0, 0);
                }
            }

            // We reach the end
            return direction;
        }

        public static Vector2 getPlayerStandingTile()
        {
            return Game1.player.Tile;
        }
    }
}
