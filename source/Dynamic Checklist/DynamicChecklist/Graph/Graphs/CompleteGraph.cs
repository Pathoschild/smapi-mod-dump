namespace DynamicChecklist.Graph.Graphs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using DynamicChecklist.Graph.Vertices;
    using Microsoft.Xna.Framework;
    using QuickGraph.Algorithms.Observers;
    using QuickGraph.Algorithms.ShortestPath;
    using StardewValley;
    using StardewValley.Buildings;

    public class CompleteGraph : StardewGraph
    {
        private IList<GameLocation> gameLocations;
        private DijkstraShortestPathAlgorithm<StardewVertex, StardewEdge> dijkstra;
        private VertexDistanceRecorderObserver<StardewVertex, StardewEdge> distObserver;
        private VertexPredecessorRecorderObserver<StardewVertex, StardewEdge> predecessorObserver;

        public CompleteGraph(IList<GameLocation> gameLocations)
        {
            this.gameLocations = gameLocations;
        }

        public IList<PartialGraph> PartialGraphs { get; private set; } = new List<PartialGraph>();

        public void Populate()
        {
            foreach (GameLocation location in this.gameLocations)
            {
                var partialGraph = new PartialGraph(location);
                partialGraph.Populate();
                this.PartialGraphs.Add(partialGraph);
            }

            var farmBuildings = Game1.getFarm().buildings;
            foreach (Building building in farmBuildings)
            {
                var indoors = building.indoors.Value;
                if (indoors != null && indoors is AnimalHouse)
                {
                    var partialGraph = new PartialGraph((AnimalHouse)indoors);
                    partialGraph.Populate();
                    this.PartialGraphs.Add(partialGraph);
                }
            }

            foreach (PartialGraph pgSource in this.PartialGraphs)
            {
                foreach (PartialGraph pgTarget in this.PartialGraphs)
                {
                    if (pgSource != pgTarget)
                    {
                        this.ConnectPartialGraph(pgSource, pgTarget);
                    }
                }
            }

            foreach (PartialGraph partialGraph in this.PartialGraphs)
            {
                this.AddVertexRange(partialGraph.Vertices);
                this.AddEdgeRange(partialGraph.Edges);
            }

            Func<StardewEdge, double> edgeCost = (x) => x.Cost;
            this.dijkstra = new DijkstraShortestPathAlgorithm<StardewVertex, StardewEdge>(this, edgeCost);
            this.distObserver = new VertexDistanceRecorderObserver<StardewVertex, StardewEdge>(edgeCost);
            this.distObserver.Attach(this.dijkstra);
            this.predecessorObserver = new VertexPredecessorRecorderObserver<StardewVertex, StardewEdge>();
            this.predecessorObserver.Attach(this.dijkstra);
        }

        public void Calculate(GameLocation sourceLocation)
        {
            var partialGraphs = this.FindPartialGraph(sourceLocation);
            var playerVertex = partialGraphs.PlayerVertex;
            this.dijkstra.ClearRootVertex();
            this.dijkstra.Compute(playerVertex);
        }

        public ShortestPath GetPathToTarget(GameLocation sourceLocation, GameLocation targetLocation)
        {
            var partialGraphs = this.FindPartialGraph(sourceLocation);
            var playerVertex = partialGraphs.PlayerVertex;

            this.dijkstra.Compute(playerVertex);
            var b = this.distObserver;
            var c = this.predecessorObserver;

            var targetVertex = this.FindPartialGraph(targetLocation).TargetVertex;
            var path = (IEnumerable<StardewEdge>)new List<StardewEdge>();
            var success = this.predecessorObserver.TryGetPath(targetVertex, out path);
            var pathSimple = new ShortestPath();
            if (success)
            {
                foreach (var pathPart in path)
                {
                    if (pathPart.Source != playerVertex)
                    {
                        pathSimple.AddStep(pathPart.Source.Location, pathPart.Source.Position);
                    }
                }

                return pathSimple;
            }
            else
            {
                return null;
            }
        }

        public void SetTargetLocation(GameLocation location, Vector2 position)
        {
            this.FindPartialGraph(location).TargetVertex.SetPosition(position);
        }

        public void SetPlayerPosition(GameLocation location, Vector2 position)
        {
            var partialGraph = this.FindPartialGraph(location);
            var tilePosition = new Vector2(position.X / Game1.tileSize, position.Y / Game1.tileSize);
            partialGraph.PlayerVertex.SetPosition(tilePosition);
        }

        public bool LocationInGraph(GameLocation loc)
        {
            var correspondingGraph = this.PartialGraphs.FirstOrDefault(x => x.Location == loc);
            return correspondingGraph != null;
        }

        private void ConnectPartialGraph(PartialGraph pgSource, PartialGraph pgTarget)
        {
            foreach (StardewVertex vertex in pgSource.Vertices)
            {
                if (vertex is WarpVertex)
                {
                    var warpVertex = (WarpVertex)vertex;
                    if (warpVertex.TargetLocation == pgTarget.Location)
                    {
                        var newVertex = new StardewVertex(pgTarget.Location, warpVertex.TargetPosition);
                        pgTarget.AddVertex(newVertex);
                        var newEdge = new StardewEdge(vertex, newVertex, "Partial graph connection");
                        pgSource.AddEdge(newEdge);
                        foreach (StardewVertex targetVertex in pgTarget.Vertices)
                        {
                            // Player vertex only needs to connect away from itself, all warp vertices and the target vertex must have an edge going to them
                            if (targetVertex != pgTarget.PlayerVertex)
                            {
                                var e = new StardewEdge(newVertex, targetVertex, $"From {newVertex.Location} to {targetVertex.Location}");
                                pgTarget.AddEdge(e);
                            }
                        }
                    }
                }
            }
        }

        private PartialGraph FindPartialGraph(GameLocation loc)
        {
            foreach (PartialGraph p in this.PartialGraphs)
            {
                if (p.Location == loc)
                {
                    return p;
                }
            }

            throw new KeyNotFoundException("Location not found in graph");
        }
    }
}