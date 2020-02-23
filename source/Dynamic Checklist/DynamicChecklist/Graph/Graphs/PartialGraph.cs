namespace DynamicChecklist.Graph
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using DynamicChecklist.Graph.Edges;
    using DynamicChecklist.Graph.Vertices;
    using Microsoft.Xna.Framework;
    using StardewValley;
    using StardewValley.Buildings;

    public class PartialGraph : StardewGraph
    {
        public PartialGraph(GameLocation location)
            : base()
        {
            this.Location = location;
        }

        public MovableVertex PlayerVertex { get; private set; }

        public MovableVertex TargetVertex { get; private set; }

        public List<PlayerEdge> PlayerEdges { get; private set; }

        public GameLocation Location { get; private set; }

        public void Populate()
        {
            var vertexToInclude = new List<WarpVertex>();
            List<Warp> warps = new List<Warp>(this.Location.warps); // Shallow copy to allow adding to list without changing original list

            if (this.Location is Farm)
            {
                var farmBuildings = ((Farm)this.Location).buildings;
                var greenhouse = Game1.getLocationFromName("Greenhouse");
                var w = greenhouse.warps[0];
                warps.Add(new Warp(w.TargetX, w.TargetY, "Greenhouse", 0, 0, false));

                foreach (Building building in farmBuildings)
                {
                    var indoors = building.indoors.Value;
                    if (indoors != null && indoors is AnimalHouse)
                    {
                        var doorLoc = new Vector2(building.tileX.Value + building.humanDoor.X, building.tileY.Value + building.humanDoor.Y);

                        // Target location does not matter since an animal house is always at the end of the path
                        var vertexNew = new WarpVertex(this.Location, doorLoc, indoors, new Vector2(0, 0));
                        this.AddVertex(vertexNew);
                    }
                }
            }

            for (int i = 0; i < warps.Count; i++)
            {
                var warp = warps.ElementAt(i);
                var vertexNew = new WarpVertex(this.Location, new Vector2(warp.X, warp.Y), Game1.getLocationFromName(warp.TargetName), new Vector2(warp.TargetX, warp.TargetY));
                bool shouldAdd = true;
                foreach (WarpVertex extWarpIncluded in vertexToInclude)
                {
                    if (vertexNew.TargetLocation == extWarpIncluded.TargetLocation && StardewVertex.Distance(vertexNew, extWarpIncluded) < 5)
                    {
                        shouldAdd = false;
                        break;
                    }
                }

                if (shouldAdd)
                {
                    vertexToInclude.Add(vertexNew);
                    this.AddVertex(vertexToInclude.Last());
                }
            }

            for (int i = 0; i < vertexToInclude.Count; i++)
            {
                var vertex1 = vertexToInclude.ElementAt(i);

                for (int j = 0; j < vertexToInclude.Count; j++)
                {
                    var locTo = Game1.getLocationFromName(this.Location.warps.ElementAt(j).TargetName);
                    var vertex2 = vertexToInclude.ElementAt(j);
                    var path = PathFindController.findPath(new Point((int)vertex1.Position.X, (int)vertex1.Position.Y), new Point((int)vertex2.Position.X, (int)vertex2.Position.Y), new PathFindController.isAtEnd(PathFindController.isAtEndPoint), this.Location, Game1.player, 9999);

                    // TODO Use Pathfinder distance
                    double dist;
                    string edgeLabel;
                    if (path != null)
                    {
                        dist = (float)path.Count;

                        // TODO Player can run diagonally. Account for that.
                        edgeLabel = this.Location.Name + " - " + dist + "c";
                    }
                    else
                    {
                        dist = (int)StardewVertex.Distance(vertex1, vertex2);
                        edgeLabel = this.Location.Name + " - " + dist + "d";
                    }

                    var edge = new StardewEdge(vertex1, vertex2, edgeLabel);
                    this.AddEdge(edge);
                }

                this.AddVertex(vertex1);
            }

            this.AddPlayerVertex(new MovableVertex(this.Location, new Vector2(0, 0)));
            this.AddTargetVertex(new MovableVertex(this.Location, new Vector2(0, 0)));
            this.ConnectPlayerVertex();
        }

        [Obsolete] // Maybe needed later for pathfinder calculation
        public void UpdatePlayerEdgeCosts(Vector2 position)
        {
            this.PlayerVertex.SetPosition(position);
            foreach (PlayerEdge edge in this.PlayerEdges)
            {
            }
        }

        private void AddPlayerVertex(MovableVertex vertex)
        {
            if (this.PlayerVertex == null)
            {
                this.AddVertex(vertex);
                this.PlayerVertex = vertex;
            }
            else
            {
                throw new InvalidOperationException("Player vertex already added");
            }
        }

        private void ConnectPlayerVertex()
        {
            this.PlayerEdges = new List<PlayerEdge>();
            foreach (StardewVertex vertex in this.Vertices)
            {
                if (vertex != this.PlayerVertex)
                {
                    var newEdge = new PlayerEdge(this.PlayerVertex, vertex);
                    this.AddEdge(newEdge);
                    this.PlayerEdges.Add(newEdge);
                }
            }
        }

        private void AddTargetVertex(MovableVertex w)
        {
            if (this.TargetVertex == null)
            {
                this.AddVertex(w);
                this.TargetVertex = w;
            }
            else
            {
                throw new InvalidOperationException("Target vertex already added");
            }
        }
    }
}
