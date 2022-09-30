/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using System.Threading;
using StardewModdingAPI;
using StardewValley;
using ItemPipes.Framework.Nodes.ObjectNodes;
using MaddUtil;


namespace ItemPipes.Framework
{
    public class Network
    {
        public long ID { get; set; }
        public List<Node> Nodes { get; set; }
        public List<OutputPipeNode> Outputs { get; set; }
        public List<InputPipeNode> Inputs { get; set; }
        public List<ConnectorPipeNode> Connectors { get; set; }
        public bool IsPassable { get; set; }
        public List<InvisibilizerNode> Invisibilizers { get; set; }

        public Network() { }
        public Network(long id)
        {
            ID = id;
            Nodes = new List<Node>();
            Outputs = new List<OutputPipeNode>();
            Inputs = new List<InputPipeNode>();
            Connectors = new List<ConnectorPipeNode>();
            Invisibilizers = new List<InvisibilizerNode>();
            IsPassable = false;
        }

        public void Update()
        {
            foreach(OutputPipeNode output in Outputs)
            {
                foreach (InputPipeNode input in output.ConnectedInputs.Keys.ToList())
                {
                    TryDisconnectInput(input);
                    input.UpdateSignal();
                }
                TryConnectOutput(output);
                output.UpdateSignal();
            }
            if (Invisibilizers.Count == 0)
            {
                Deinvisibilize(null);
            }
        }

        public void ProcessExchanges(int tier)
        {
            //Update();
            foreach (OutputPipeNode output in Outputs)
            {
                if (output.Tier == tier)
                {
                    output.ProcessExchanges();
                }
            }

        }

        public bool AddNode(Node node)
        {
            bool added = false;
            if (node.ParentNetwork == this && !Nodes.Contains(node))
            {
                added = true;
                if (IsPassable && node is not InvisibilizerNode)
                {
                    node.Passable = true;
                }
                Nodes.Add(node);
                if (node is OutputPipeNode && !Outputs.Contains(node))
                {
                    Outputs.Add((OutputPipeNode)node);
                }
                else if (node is InputPipeNode && !Inputs.Contains(node))
                {
                    Inputs.Add((InputPipeNode)node);
                }
                else if (node is ConnectorPipeNode && !Connectors.Contains(node))
                {
                    Connectors.Add((ConnectorPipeNode)node);
                }
                else if (node is InvisibilizerNode && !Invisibilizers.Contains(node))
                {
                    Invisibilizers.Add((InvisibilizerNode)node);
                    if (!IsPassable && (node as InvisibilizerNode).State == "on")
                    {
                        Invisibilize((InvisibilizerNode)node);
                    }
                    else if (IsPassable && (node as InvisibilizerNode).State == "off")
                    {
                        Deinvisibilize((InvisibilizerNode)node);
                    }
                }
            }
            else if(node.ParentNetwork != this)
            {
                if(!(node is ChestContainerNode))
                {
                    Printer.Warn($"Tried to add {node.PrintHash()} to N{ID}, but they dont match.");
                }
            }
            return added;
        }

        public bool RemoveNode(Node node)
        {
            bool removed = false;
            if (Nodes.Contains(node))
            {
                removed = true;
                Nodes.Remove(node);
                if (Outputs.Contains(node))
                {
                    Outputs.Remove((OutputPipeNode)node);
                }
                else if (Inputs.Contains(node))
                {
                    Inputs.Remove((InputPipeNode)node);
                }
                else if (Connectors.Contains(node))
                {
                    Connectors.Remove((ConnectorPipeNode)node);
                }
                else if (node is InvisibilizerNode && Invisibilizers != null)
                {
                    Invisibilizers.Remove((InvisibilizerNode)node);
                    if (!IsPassable && (node as InvisibilizerNode).State == "on")
                    {
                        Invisibilize((InvisibilizerNode)node);
                    }
                    else if (IsPassable && (node as InvisibilizerNode).State == "off")
                    {
                        Deinvisibilize((InvisibilizerNode)node);
                    }
                }
            }
            return removed;
        }

        public bool TryConnectOutput(OutputPipeNode output)
        {
            bool canConnect = false;
            if (output != null)
            {
                if(Inputs.Count > 0)
                {
                    foreach (InputPipeNode input in Inputs)
                    {
                        if (!output.IsInputConnected(input))
                        {
                            if (output.CanConnectedWith(input))
                            {
                                canConnect = output.AddConnectedInput(input);
                                if (ModEntry.config.DebugMode) { Printer.Debug($"[N{ID}] {input.PrintHash()} connected with {output.PrintHash()}"); }
                            }
                        }
                        else
                        {
                            List<PipeNode> path = output.GetPath(input);
                            if (path.Count > 0 && path.Last().Equals(input))
                            {
                                output.ConnectedInputs[input] = path;
                            }
                            else
                            {
                                if (ModEntry.config.DebugMode) { Printer.Warn($"[N{ID}] Error updating path from {output.PrintHash()} to {input.PrintHash()}"); }
                            }
                        }
                        input.UpdateSignal();
                    }
                    output.UpdateSignal();
                }
            }
            return canConnect;
        }

        public bool TryDisconnectInput(InputPipeNode input)
        {
            bool canDisconnect = false;
            if (input != null)
            {
                foreach (OutputPipeNode output in Outputs)
                {
                    if (output.IsInputConnected(input))
                    {
                        if (input.ConnectedContainer == null || !output.CanConnectedWith(input))
                        {
                            canDisconnect = output.RemoveConnectedInput(input);
                            if (ModEntry.config.DebugMode) { Printer.Debug($"[N{ID}] {input.PrintHash()} disconnected"); }
                        }
                    }
                    output.UpdateSignal();
                }
                input.UpdateSignal();
            }
            return canDisconnect;
        }

        public bool ContainsVector2(Vector2 position)
        {
            bool contains = false;
            if (Nodes.Any(x => x.Position == position))
            {
                contains = true;
            }
            return contains;
        }
        public void Delete()
        {
            foreach (Node node in Nodes)
            {
                node.ParentNetwork = null;
            }
        }

        public void Invisibilize(InvisibilizerNode invis)
        {
            if (Invisibilizers.All(p => p.Passable))
            {
                IsPassable = true;
                foreach (Node node in Nodes)
                {
                    if (node is not InvisibilizerNode)
                    {
                        node.Passable = true;
                    }
                }
            }
        }

        public void Deinvisibilize(InvisibilizerNode invis)
        {
            if(Invisibilizers.Count == 0)
            {
                IsPassable = false;
                foreach (Node node in Nodes)
                {
                    if (node is not InvisibilizerNode)
                    {
                        node.Passable = false;
                    }
                }
            }
            else if (Invisibilizers.Any(p => !p.Passable))
            {
                IsPassable = false;
                foreach (Node node in Nodes)
                {
                    if(node is not InvisibilizerNode)
                    {
                        node.Passable = false;
                    }
                }
            }
        }

        public void RemoveAllAdjacents()
        {
            foreach (Node node in Nodes.ToList())
            {
                node.RemoveAllAdjacents();
            }
        }
        public string Print()
        {
            StringBuilder graph = new StringBuilder();
            if (!Nodes.All(n=>n is ContainerNode))
            {
                graph.Append($"\n----------------------------");
                graph.Append($"\nPriting Network [{ID}] {this.GetHashCode().ToString()}: \n");
                graph.Append("Networks: \n");
                graph.Append("Inputs: \n");
                foreach (InputPipeNode input in Inputs)
                {
                    graph.Append(input.PrintHash() + ", ");
                }
                graph.Append("\n");
                graph.Append("Outputs: \n");
                foreach (OutputPipeNode output in Outputs)
                {
                    graph.Append(output.PrintHash() + ", \n");
                    graph.Append("Output Connected Inputs: \n");
                    foreach (InputPipeNode input in output.ConnectedInputs.Keys)
                    {
                        graph.Append(input.PrintHash() + " | ");
                    }
                    graph.Append("\n");
                }
                graph.Append("Connectors: \n");
                foreach (ConnectorPipeNode conn in Connectors)
                {
                    graph.Append(conn.PrintHash() + ", ");
                }
                graph.Append("\n");
                graph.Append("PIPOs: \n");
                foreach (InvisibilizerNode pipo in Invisibilizers)
                {
                    graph.Append(pipo.PrintHash() + $" {pipo.State} {pipo.Passable} " + ", ");
                }
                graph.Append("\n");
            }
            else
            {
                graph.Append($"Network {ID} is only chests.");
            }
            return graph.ToString();
        }

        public string PrintGraph()
        {
            //NEED TESTING
            int minWidth = (int)Math.Round(Nodes.Min(n => n.Position.X))-1;
            int minHeight = (int)Math.Round(Nodes.Min(n => n.Position.Y))-1;
            int maxWidth = (int)Math.Round(Nodes.Max(n => n.Position.X))+2;
            int maxHeight = (int)Math.Round(Nodes.Max(n => n.Position.Y))+2;

            PipeNode[,] matrix = new PipeNode[maxWidth, maxHeight];

            StringBuilder graph = new StringBuilder();
            graph.AppendLine($"Network {ID} graph: ");
            graph.Append($"   ");
            string sB = "";
            string sA = "";
            if(maxWidth > 100)
            {
                //numeros que sean 016
                sB = "  ";
                sA = "  ";
            }
            else if(maxWidth < 100 && maxWidth >= 10)
            {
                sB = "  ";
                sA = " ";
            }
            else
            {
                sB = " ";
                sA = " ";
            }
            for (int j = minWidth; j < maxWidth; j++)
            {
                graph.Append($" {j} ");
            }
            for (int i = minHeight; i < maxHeight; i++)
            {
                graph.Append("\n");
                graph.Append($" {i}");
                for (int j = minWidth; j < maxWidth; j++)
                {
                    Node node = Nodes.Find(n => n.Position.X == j && n.Position.Y == i);
                    if (node != null)
                    {
                        if(node is ConnectorPipeNode)
                        {
                            graph.Append($"{sB}C{sA}");
                        }
                        else if(node is OutputPipeNode)
                        {
                            graph.Append($"{sB}O{sA}");
                        }
                        else if (node is InputPipeNode)
                        {
                            graph.Append($"{sB}I{sA}");
                        }
                        else if (node is InvisibilizerNode)
                        {
                            graph.Append($"{sB}P{sA}");
                        }
                        else
                        {
                            graph.Append($"{sB}X{sA}");
                        }
                    }
                    else
                    {
                        graph.Append($"{sB} {sA}");
                    }
                }
            }
            return graph.ToString();
        }
    }
}
