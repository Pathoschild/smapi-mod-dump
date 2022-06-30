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
using System.Threading;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using ItemPipes.Framework.Nodes.ObjectNodes;


namespace ItemPipes.Framework
{
    public abstract class OutputPipeNode : IOPipeNode
    {
        public int Tier { get; set; }
        public int Flux { get; set; }
        public Dictionary<InputPipeNode, List<PipeNode>> ConnectedInputs { get; set; }
        public bool RoundRobin { get; set; }
        public OutputPipeNode() : base()
        {
            RoundRobin = false;
        }
        public OutputPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedInputs = new Dictionary<InputPipeNode, List<PipeNode>>();
        }

        public override void UpdateSignal()
        {
            if(!Signal.Equals("off"))
            {
                if (ConnectedContainer == null)
                {
                    Signal = "nochest";
                }
                else if (ConnectedInputs.Count < 1)
                {
                    Signal = "unconnected";
                }
                else if (ConnectedContainer != null && ConnectedInputs.Count >= 1)
                {
                    Signal = "on";
                }
            }
        }

        public override void ChangeSignal()
        {
            if (Signal.Equals("off"))
            {
                if (ConnectedContainer == null)
                {
                    Signal = "nochest";
                }
                else if (ConnectedInputs.Count < 1)
                {
                    Signal = "unconnected";
                }
                else if (ConnectedContainer != null && ConnectedInputs.Count >= 1)
                {
                    Signal = "on";
                }
            }
            else
            {
                Signal = "off";
            }
        }

        public void ProcessExchanges()
        {
            if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] Procesing Exchanges..."); }
            if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] Are there connected input? " + (ConnectedInputs.Count > 0).ToString()); }
            if (ConnectedContainer != null && !ConnectedContainer.IsEmpty()
                && ConnectedInputs.Count > 0 && Signal.Equals("on"))
            {
                if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] Is output ({Print()}) empty? " + ConnectedContainer.IsEmpty().ToString()); }
                try
                {
                    Thread thread = new Thread(new ThreadStart(StartExchage));
                    if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] CREATED NEW THREAD WITH ID [{thread.ManagedThreadId}]"); }
                    DataAccess.GetDataAccess().Threads.Add(thread);
                    thread.Start();
                }
                catch (ThreadInterruptedException exception)
                {
                }
            }
        }

        public bool CanSendItems(InputPipeNode input)
        {
            bool canSend = false;
            if(ConnectedContainer != null && ConnectedContainer.CanSendItems() 
                && (input.CanRecieveItems() || input.ConnectedContainer.CanStackItems())
                && StoredItem == null)
            {
                canSend = true;
            }
            return canSend;
        }

        public Item GetItemFor(InputPipeNode input)
        {
            Item item = null;
            item = ConnectedContainer.GetItemForInput(input, Flux);
            return item;
        }

        public void StartExchage()
        {
            if (Globals.UltraDebug) { Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}][N{ParentNetwork.ID}] Number of inputs: " + ConnectedInputs.Count.ToString()); }
            Item item = null;
            int index = 0;
            Dictionary<InputPipeNode, List<PipeNode>> priorityInputs = ConnectedInputs;
            priorityInputs = priorityInputs.
                OrderByDescending(pair => pair.Key.Priority).
                ThenBy(pair => pair.Value.Count).
                ToDictionary(x => x.Key, x => x.Value);
            //Mirar para hacer round robin
            if (!RoundRobin)
            {
                while (index < priorityInputs.Count && item == null)
                {
                    InputPipeNode input = priorityInputs.Keys.ToList()[index];
                    if (input.Signal.Equals("on"))
                    {
                        List<PipeNode> path = priorityInputs.Values.ToList()[index];
                        input.UpdateFilter();
                        if (CanSendItems(input))
                        {
                            item = GetItemFor(input);
                            if (item != null && StoredItem == null)
                            {
                                //Printer.Info($"Item to send: {item.Name}({item.Stack})");
                                Node ret = SendItem(item, input);
                                if(ret == null)
                                {
                                    input.SendItem(item, this);
                                }
                            }
                            else if(StoredItem != null)
                            {
                                //Printer.Info($"Output locked");
                                //Output locked
                            }
                            else if(item == null)
                            {
                                //Printer.Info($"Item is null");
                                //Item is null
                            }
                        }
                    }
                    index++;
                }
            }
            else
            {
                //RoundRobin
            }
            try
            {
                if (DataAccess.GetDataAccess().Threads.Contains(Thread.CurrentThread))
                {
                    DataAccess.GetDataAccess().Threads.Remove(Thread.CurrentThread);
                }
            }
            catch (Exception e)
            {
                DataAccess.GetDataAccess().Threads.Clear();
            }
        }

        public bool IsInputConnected(InputPipeNode input)
        {
            bool connected = false;
            if (ConnectedInputs.Keys.Contains(input))
            {
                connected = true;
            }
            return connected;
        }

        public bool AddConnectedInput(InputPipeNode input)
        {
            bool added = false;
            if (Globals.UltraDebug) { Printer.Debug($"[N{ParentNetwork.ID}] Does {Print()} have a valid adjacent container? " + (ConnectedContainer != null).ToString()); }
            if (Globals.UltraDebug) { Printer.Debug($"[N{ParentNetwork.ID}] Does {input.Print()} have a valid adjacent container? " + (input.ConnectedContainer != null).ToString()); }
            if (ConnectedContainer != null && input.ConnectedContainer != null)
            {
                List<PipeNode> path;
                path = GetPath(input);
                if(path.Count > 0)
                {
                    added = true;
                    ConnectedInputs.Add(input, path);
                    var t = new Thread(() => AnimateConnection(path));
                    t.Start();
                    DataAccess.GetDataAccess().Threads.Add(t);
                }
            }
            return added;
        }

        private void AnimateConnection(List<PipeNode> path)
        {
            ConnectPipe(path.Last());
            try
            {
                if (DataAccess.GetDataAccess().Threads.Contains(Thread.CurrentThread))
                {
                    DataAccess.GetDataAccess().Threads.Remove(Thread.CurrentThread);
                }
            }
            catch (Exception e)
            {
                DataAccess.GetDataAccess().Threads.Clear();
            }
        }

        public bool RemoveConnectedInput(InputPipeNode input)
        {
            bool removed = false;
            if (ConnectedInputs.Keys.Contains(input))
            {
                removed = true;
                ConnectedInputs.Remove(input);
            }
            return removed;
        }

        public override bool RemoveAllAdjacents()
        {
            bool removed = false;
            foreach (KeyValuePair<Side, Node> adj in Adjacents.ToList())
            {
                if (adj.Value != null)
                {
                    removed = true;
                    RemoveAdjacent(adj.Key, adj.Value);
                }
            }
            ConnectedInputs.Clear();
            return removed;
        }
    }
}
