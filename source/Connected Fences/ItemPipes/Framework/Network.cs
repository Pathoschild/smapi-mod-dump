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
using ItemPipes.Framework.Objects;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using System.Threading;
using StardewModdingAPI;
using StardewValley;

namespace ItemPipes.Framework
{
    public class Network
    {
        public int ID { get; set; }
        public List<Node> Nodes { get; set; }
        public List<Output> Outputs { get; set; }
        public List<Input> Inputs { get; set; }
        public List<Connector> Connectors { get; set; }
        public bool IsPassable { get; set; }
        public Invisibilizer Invis { get; set; }

        public Network(int id)
        {
            ID = id;
            Nodes = new List<Node>();
            Outputs = new List<Output>();
            Inputs = new List<Input>();
            Connectors = new List<Connector>();
            IsPassable = false;
        }

        public void Update()
        {
            foreach(Output output in Outputs)
            {
                foreach (Input input in output.ConnectedInputs.Keys.ToList())
                {
                    TryDisconnectInput(input);
                }
                TryConnectOutput(output);
            }

        }

        public void ProcessExchanges()
        {
            Update();
            foreach (Output output in Outputs)
            {
                output.ProcessExchanges();
            }
        }

        public bool AddNode(Node node)
        {
            bool added = false;
            if (!Nodes.Contains(node))
            {
                added = true;
                Nodes.Add(node);
                if (node is Output && !Outputs.Contains(node))
                {
                    Outputs.Add((Output)node);
                }
                else if (node is Input && !Inputs.Contains(node))
                {
                    Inputs.Add((Input)node);
                }
                else if (node is Connector && !Connectors.Contains(node))
                {
                    Connectors.Add((Connector)node);
                }
                else if (node is Invisibilizer && Invis == null)
                {
                    Invis = (Invisibilizer)node;
                    Thread thread = new Thread(new ThreadStart(ChangePassable));
                    thread.Start();
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
                    Outputs.Remove((Output)node);
                }
                else if (Inputs.Contains(node))
                {
                    Inputs.Remove((Input)node);
                }
                else if (Connectors.Contains(node))
                {
                    Connectors.Remove((Connector)node);
                }
                else if (node is Invisibilizer && Invis != null)
                {
                    Thread thread = new Thread(new ThreadStart(ChangePassable));
                    thread.Start();
                    Invis = null;
                }
            }
            return removed;
        }

        public bool TryConnectNodes(Output output, Input input)
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

        public bool TryConnectOutput(Output output)
        {
            if (Globals.Debug) { Printer.Info($"[{ID}] Trying output connection..."); }
            bool canConnect = false;
            if (output != null)
            {
                foreach (Input input in Inputs)
                {
                    if (!output.IsInputConnected(input))
                    {
                        if (Globals.Debug) { Printer.Info($"[{ID}] Input not connected"); }
                        if (Globals.Debug) { input.Print(); }
                        if (output.CanConnectedWith(input))
                        {
                            if (Globals.Debug) { Printer.Info($"[{ID}] Can connect with input"); }
                            canConnect = output.AddConnectedInput(input);
                            if (Globals.Debug) { Printer.Info($"[{ID}] CONNECTED? " + canConnect.ToString()); }
                        }
                    }
                }
            }
            return canConnect;
        }

        public bool TryDisconnectInput(Input input)
        {
            if (Globals.Debug) { Printer.Info($"[{ID}] Trying input disconnection"); Print(); }
            bool canDisconnect = false;
            if (input != null)
            {
                if (Globals.Debug) { Printer.Info($"[{ID}] Input not null"); }
                foreach (Output output in Outputs)
                {
                    if (Globals.Debug) { Printer.Info($"[{ID}] Output has input? " + output.IsInputConnected(input).ToString()); }
                    if (output.IsInputConnected(input))
                    {
                        if (!output.CanConnectedWith(input))
                        {
                            if (Globals.Debug) { Printer.Info($"[{ID}] Can connect with input"); }
                            canDisconnect = output.RemoveConnectedInput(input);
                            if (Globals.Debug) { Printer.Info($"[{ID}] Disconnected?  " + canDisconnect.ToString()); }
                        }

                    }
                }
            }
            return canDisconnect;
        }

        public void ChangePassable()
        {
            List<Node> path = Invis.TraverseAll();
            Animator.AnimateChangingPassable(path);
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

        public string Print()
        {
            StringBuilder graph = new StringBuilder();
            graph.Append("\nPriting Networks: \n");
            graph.Append("Networks: \n");
            graph.Append("Inputs: \n");
            foreach (Input input in Inputs)
            {
                graph.Append(input.Obj.Name + input.Position.ToString() + input.GetHashCode().ToString() + ", ");
            }
            graph.Append("\n");
            graph.Append("Outputs: \n");
            foreach (Output output in Outputs)
            {
                graph.Append(output.Obj.Name + output.Position.ToString() + output.GetHashCode().ToString() + ", \n");
                foreach (Input input in output.ConnectedInputs.Keys)
                {
                    graph.Append("Output Connected Inputs: \n");
                    graph.Append(input.Obj.Name + input.Position.ToString() + input.GetHashCode().ToString() + " | ");
                }
                graph.Append("\n");
            }
            graph.Append("Connectors: \n");
            foreach (Connector conn in Connectors)
            {
                graph.Append(conn.Obj.Name + conn.Position.ToString() + conn.GetHashCode().ToString() + ", ");
            }
            graph.Append("\n");
            return graph.ToString();
        }
    }
}
