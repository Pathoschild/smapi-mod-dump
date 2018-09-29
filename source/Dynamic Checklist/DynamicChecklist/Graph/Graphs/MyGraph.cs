using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QuickGraph.Graphviz;
using QuickGraph.Graphviz.Dot;
using QuickGraph.Algorithms.ShortestPath;
using StardewValley;
using QuickGraph;
using System.IO;
using Microsoft.Xna.Framework;
using QuickGraph.Algorithms.Observers;

namespace DynamicChecklist.Graph.Graphs
{
    public class MyGraph : AdjacencyGraph<ExtendedWarp, LabelledEdge<ExtendedWarp>>, IEdgeListGraph<ExtendedWarp, LabelledEdge<ExtendedWarp>>
    {
        // IMPORTANT: Rewrite, subclass, organize code. ExtendedWarp makes no sense as neither Vertex nor Edge class. Create better classes for the job (subclass edge, not general);
        private Dictionary<GameLocation, ExtendedWarp> targetVertices;
        private Dictionary<GameLocation, ExtendedWarp> playerVertices;

        private Dictionary<GameLocation, PartialGraph> partialGraphs;

        public MyGraph() : base(false)
        {

        }
        public void Calculate()
        {
            Func<LabelledEdge<ExtendedWarp>, double> edgeCost = (x) => x.Cost;
            //TryFunc<ExtendedWarp, LabelledEdge<ExtendedWarp>> tryGetPaths = this           
            var alg = new DijkstraShortestPathAlgorithm<ExtendedWarp, LabelledEdge<ExtendedWarp>>(this, edgeCost);

            var distObserver = new VertexDistanceRecorderObserver<ExtendedWarp, LabelledEdge<ExtendedWarp>>(edgeCost);
            distObserver.Attach(alg);
            var predecessorObserver = new VertexPredecessorRecorderObserver<ExtendedWarp, LabelledEdge<ExtendedWarp>>();
            predecessorObserver.Attach(alg);
            alg.Compute(this.Vertices.ElementAt(0));

            // Example: Get to town from farm hous

        }
        public void Create()
        {
            var a = Game1.currentLocation.warps;

            foreach (GameLocation loc in Game1.locations)
            {
                //graph.AddVertex(loc.Name);
            }
            partialGraphs = new Dictionary<GameLocation, PartialGraph>();
            //var edgeCosts = new List<Dictionary<Edge<ExtendedWarp>, double>>();          
            foreach (GameLocation loc in Game1.locations)
            {
                if (loc.Name == "Farm")
                {
                    // TODO Connect barns/coops
                }
                // TODO add player and target vertex and connect every node to it
                var partialGraph = new PartialGraph(loc);
                // Calculate which warps should correspond to a vertex. Dont add warps which are very close.
                var extWarpsToInclude = new List<ExtendedWarp>();
                for (int i = 0; i < loc.warps.Count; i++)
                {
                    var extWarpNew = new ExtendedWarp(loc.warps.ElementAt(i), loc);
                    bool shouldAdd = true;
                    foreach (ExtendedWarp extWarpIncluded in extWarpsToInclude)
                    {
                        if (extWarpNew.TargetLocation == extWarpIncluded.TargetLocation && ExtendedWarp.Distance(extWarpNew, extWarpIncluded) < 5)
                        {
                            shouldAdd = false;
                            break;
                        }
                    }
                    if (shouldAdd)
                    {
                        extWarpsToInclude.Add(extWarpNew);
                        partialGraph.AddVertex(extWarpsToInclude.Last());
                    }

                }
                // Create edges for partial graphs
                for (int i = 0; i < extWarpsToInclude.Count; i++)
                {
                    var extWarp1 = extWarpsToInclude.ElementAt(i);


                    for (int j = 0; j < extWarpsToInclude.Count; j++)
                    {
                        var LocTo = Game1.getLocationFromName(loc.warps.ElementAt(j).TargetName);
                        var extWarp2 = extWarpsToInclude.ElementAt(j);
                        var path = PathFindController.findPath(new Point(extWarp1.X, extWarp1.Y), new Point(extWarp2.X, extWarp2.Y), new PathFindController.isAtEnd(PathFindController.isAtEndPoint), loc, Game1.player, 9999);
                        double dist;
                        string edgeLabel;
                        if (path != null)
                        {
                            dist = (float)path.Count;
                            // TODO Player can run diagonally. Account for that.
                            edgeLabel = loc.Name + " - " + dist + "c";

                        }
                        else
                        {
                            dist = (int)Vector2.Distance(new Vector2(extWarp1.X, extWarp1.Y), new Vector2(extWarp2.X, extWarp2.Y));
                            edgeLabel = loc.Name + " - " + dist + "d";
                        }

                        var edge = new LabelledEdge<ExtendedWarp>(extWarp1, extWarp2, edgeLabel, new GraphvizColor(255, 255, 255, 255), dist);
                        partialGraph.AddEdge(edge);

                    }
                    partialGraph.AddVertex(extWarp1);
                }
                partialGraph.AddPlayerVertex(new ExtendedWarp(new Warp(0, 0, "None", 0, 0, false), loc));
                partialGraph.ConnectPlayerVertex();
                partialGraphs.Add(loc, partialGraph);
            }
            // Combine partial graphs into one
            foreach (var partialGraph in partialGraphs.Values)
            {
                this.AddVertexRange(partialGraph.Vertices);
                this.AddEdgeRange(partialGraph.Edges);
            }
            for (int i = 0; i < partialGraphs.Count; i++)
            {
                var graph1 = partialGraphs.Values.ElementAt(i);
                for (int j = 0; j < partialGraphs.Count; j++)
                {
                    var graph2 = partialGraphs.Values.ElementAt(j);
                    foreach (ExtendedWarp warp1 in graph1.Vertices)
                    {
                        if (warp1.OriginLocation.name == "Saloon")
                        {

                        }
                        if(graph2.VertexCount>0 && warp1.TargetLocation == graph2.Vertices.ElementAt(0).OriginLocation)
                        {
                            var copyVertex = new ExtendedWarp((Warp)warp1, warp1.OriginLocation);// copy to make the graph visually clearer
                            var edgeToCopy = new LabelledEdge<ExtendedWarp>(warp1, copyVertex, "Warp", new GraphvizColor(255, 255, 255, 255), 0);

                            this.AddVertex(copyVertex);
                            this.AddEdge(edgeToCopy);
                            foreach (ExtendedWarp warp2 in graph2.Vertices)
                            {
                                var newEdge = new LabelledEdge<ExtendedWarp>(copyVertex, warp2, warp2.OriginLocation.Name, new GraphvizColor(255, 255, 255, 255), ExtendedWarp.Distance(copyVertex, warp2));
                                // TODO: Calculate cost
                                this.AddEdge(newEdge);
                                if (warp2.TargetLocation?.Name == "Saloon")
                                {

                                }
                            }
                        }
                    }
                }
            }
            GraphvizAlgorithm<ExtendedWarp, LabelledEdge<ExtendedWarp>> graphviz = new GraphvizAlgorithm<ExtendedWarp, LabelledEdge<ExtendedWarp>>(this);
            graphviz.FormatVertex += (sender2, args) => args.VertexFormatter.Label = args.Vertex.Label;
            graphviz.FormatEdge += (sender2, args) => { args.EdgeFormatter.Label.Value = args.Edge.Label; };
            graphviz.FormatEdge += (sender2, args) => { args.EdgeFormatter.FontGraphvizColor = args.Edge.Color; };
            graphviz.ImageType = GraphvizImageType.Jpeg;

            graphviz.Generate(new FileDotEngine(), "C:\\Users\\Gunnar\\Desktop\\graph123.jpeg");


            //var alg = new QuickGraph.Algorithms.ShortestPath.UndirectedDijkstraShortestPathAlgorithm<ExtendedWarp, Edge<ExtendedWarp>>;
        }
    }

    public class ExtendedWarp : Warp
    {
        public GameLocation OriginLocation;
        public GameLocation TargetLocation;
        public string Label;

        public ExtendedWarp(Warp w, GameLocation originLocation) : base(w.X, w.Y, w.TargetName, w.TargetX, w.TargetY, false)
        {
            this.OriginLocation = originLocation;
            TargetLocation = Game1.getLocationFromName(w.TargetName);
            this.Label = originLocation.name + " to " + w.TargetName;
        }

        public static bool AreCorresponding(ExtendedWarp warp1, ExtendedWarp warp2)
        {
            if (warp1.OriginLocation == warp2.TargetLocation && warp1.TargetLocation == warp2.OriginLocation)
            {
                if (Math.Abs(warp1.X - warp2.TargetX) + Math.Abs(warp1.Y - warp2.TargetY) < 5)
                {
                    return true;
                }
            }
            return false;
        }
        public static int Distance(ExtendedWarp warp1, ExtendedWarp warp2)
        {
            return Math.Abs(warp1.X - warp2.X) + Math.Abs(warp1.Y - warp2.Y);
        }
    }

    public class FileDotEngine : IDotEngine
    {
        public string Run(GraphvizImageType imageType, string dot, string outputFileName)
        {
            //using (StreamWriter writer = new StreamWriter(outputFileName))
            //{
            //    writer.Write(dot);
            //}

            //return System.IO.Path.GetFileName(outputFileName);

            string output = outputFileName;
            File.WriteAllText(output, dot);

            // assumes dot.exe is on the path:
            var args = string.Format(@"{0} -Tjpg -O", output);
            System.Diagnostics.Process.Start(@"C:\Users\Gunnar\Desktop\release\bin\dot.exe", args);
            return output;
        }
    }
    public class LabelledEdge<TVertex> : Edge<TVertex>
    {
        public string Label { get; private set; }
        public GraphvizColor Color { get; private set; }
        public double Cost { get; set; }
        public LabelledEdge(TVertex source, TVertex target, string label, GraphvizColor color, double cost) : base(source, target)
        {
            this.Label = label;
            this.Color = color;
            this.Cost = cost;
        }
    }
}
