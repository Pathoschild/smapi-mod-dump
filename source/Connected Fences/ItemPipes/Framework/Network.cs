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

namespace ItemPipes.Framework
{
    public class Network
    {
        public int ID { get; set; }
        public List<Node> Nodes { get; set; }
        public List<OutputPipeNode> Outputs { get; set; }
        public List<InputPipeNode> Inputs { get; set; }
        public List<ConnectorPipeNode> Connectors { get; set; }
        public bool IsPassable { get; set; }
        public PPMNode Invis { get; set; }

        public Network() { }
        public Network(int id)
        {
            ID = id;
            Nodes = new List<Node>();
            Outputs = new List<OutputPipeNode>();
            Inputs = new List<InputPipeNode>();
            Connectors = new List<ConnectorPipeNode>();
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
        }

        public void ProcessExchanges(int tier)
        {
            Update();
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
                else if (node is PPMNode && Invis == null)
                {
                    Invis = (PPMNode)node;
                }
            }
            else if(node.ParentNetwork != this)
            {
                //Printer.Info($"Tried to add {node.Print()} to N{ID}, but they dont match.");
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
                else if (node is PPMNode && Invis != null)
                {
                    Invis = null;
                    if(IsPassable)
                    {
                        Deinvisibilize((PPMNode)node);
                    }
                }
            }
            return removed;
        }

        public bool TryConnectNodes(OutputPipeNode output, InputPipeNode input)
        {
            bool connected = false;
            if (output != null && input != null)
            {
                if (!output.IsInputConnected(input))
                {
                    if (output.CanConnectedWith(input))
                    {
                        output.AddConnectedInput(input);

                        connected = true;
                    }
                }
            }
            return connected;
        }

        public bool TryConnectOutput(OutputPipeNode output)
        {
            bool canConnect = false;
            if (output != null)
            {
                if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Trying connecting {output.Print()}"); }
                if(Inputs.Count == 0)
                {
                    if (Globals.UltraDebug) { Printer.Info($"[N{ID}] No inputs to connect."); }
                }
                else
                {
                    if (Globals.UltraDebug) { Printer.Info($"[N{ID}] {Inputs.Count} inputs to connect."); }
                    foreach (InputPipeNode input in Inputs)
                    {
                        if (!output.IsInputConnected(input))
                        {
                            if (Globals.UltraDebug) { Printer.Info($"[N{ID}] {input.Print()} not already connected"); }
                            if (output.CanConnectedWith(input))
                            {
                                canConnect = output.AddConnectedInput(input);
                                if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Can connect with {input.Print()}? -> {canConnect}"); }
                            }
                            else
                            {
                                if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Cannot connect with {input.Print()}"); }
                            }
                        }
                        else
                        {
                            if (Globals.UltraDebug) { Printer.Info($"[N{ID}] {input.Print()} already connected"); }
                            if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Updating path from {output.Print()} to {input.Print()} connected"); }
                            List<PipeNode> path = output.GetPath(input);
                            if (path.Count > 0 && path.Last().Equals(input))
                            {
                                output.ConnectedInputs[input] = path;
                                if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Path succesfully updated!"); }
                            }
                            else
                            {
                                if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Error updating path"); }
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
                if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Trying disconnecting {input.Print()}"); }
                foreach (OutputPipeNode output in Outputs)
                {
                    if (output.IsInputConnected(input))
                    {
                        if (Globals.UltraDebug) { Printer.Info($"[N{ID}] {input.Print()} already connected"); }
                        if (!output.CanConnectedWith(input) || input.ConnectedContainer == null)
                        {
                            canDisconnect = output.RemoveConnectedInput(input);
                            if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Can disconnect with {input.Print()}? -> {canDisconnect}"); }
                        }
                        else
                        {
                            if (Globals.UltraDebug) { Printer.Info($"[N{ID}] Cannot disconnect with {input.Print()}"); }
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

        public void Invisibilize(PPMNode invis)
        {
            Invis = invis;
            IsPassable = true;
            foreach(Node node in Nodes)
            {
                node.Passable = true;
            }
        }

        public void Deinvisibilize(PPMNode invis)
        {
            Invis = invis;
            IsPassable = false;
            foreach (Node node in Nodes)
            {
                node.Passable = false;
            }
        }

        public void RemoveAllAdjacents()
        {
            foreach (Node node in Nodes)
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
                    graph.Append(input.Print() + ", ");
                }
                graph.Append("\n");
                graph.Append("Outputs: \n");
                foreach (OutputPipeNode output in Outputs)
                {
                    graph.Append(output.Print() + ", \n");
                    foreach (InputPipeNode input in output.ConnectedInputs.Keys)
                    {
                        graph.Append("Output Connected Inputs: \n");
                        graph.Append(input.Print() + " | ");
                    }
                    graph.Append("\n");
                }
                graph.Append("Connectors: \n");
                foreach (ConnectorPipeNode conn in Connectors)
                {
                    graph.Append(conn.Print() + ", ");
                }
                graph.Append("\n");
            }
            else
            {
                graph.Append($"Network {ID} is only chests.");
            }
            return graph.ToString();
        }
    }
}
