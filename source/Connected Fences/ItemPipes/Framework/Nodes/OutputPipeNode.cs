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
                if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] Is output empty? " + ConnectedContainer.IsEmpty().ToString()); }
                try
                {
                    Thread thread = new Thread(new ThreadStart(StartExchage2));
                    if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] CREATED NEW THREAD WITH ID [{thread.ManagedThreadId}]"); }
                    DataAccess.GetDataAccess().Threads.Add(thread);
                    thread.Start();
                }
                catch (ThreadInterruptedException exception)
                {
                }
            }
        }
        /*
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
            index = 0;
            //Mirar para hacer round robin
            while (index < priorityInputs.Count && item == null)
            {
                InputPipeNode input = priorityInputs.Keys.ToList()[index];
                if (input.Signal.Equals("on"))
                {
                    List<PipeNode> path = priorityInputs.Values.ToList()[index];
                    input.UpdateFilter();
                    if (ConnectedContainer is ChestContainerNode && input.ConnectedContainer is ChestContainerNode)
                    {
                        ChestContainerNode outChest = (ChestContainerNode)ConnectedContainer;
                        ChestContainerNode inChest = (ChestContainerNode)input.ConnectedContainer;
                        if(!outChest.IsEmpty())
                        {
                            if (Globals.UltraDebug) {Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}][{ParentNetwork.ID}] its not emppty");}
                            item = outChest.CanSendItem(inChest);
                            if (Globals.UltraDebug) { Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}][{ParentNetwork.ID}] Can send {item.Name}? " + (item != null).ToString()); }
                            if (item != null)
                            {
                                if (Globals.UltraDebug)
                                {
                                    Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] PATH---------------");
                                    foreach (Node node in path)
                                    {
                                        Printer.Info(node.Print());
                                    }
                                    Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] PATH---------------");
                                }
                                PipeNode broken = MoveItem(item, input, 0, path);
                                //Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] IS IT BROKEN " +broken);
                                //Check with try connect/discoonnect also
                                if (outChest != null && inChest != null)
                                {
                                    if (broken == null)
                                    {
                                        bool sent = outChest.SendItem(inChest, item);
                                        if (!sent)
                                        {
                                            if (Globals.UltraDebug) { Printer.Info($"T[{Thread.CurrentThread.ManagedThreadId}][{ParentNetwork.ID}] {item.Name} CANT ENTER, REVERSE"); }
                                            List<PipeNode> reversePath = path;
                                            reversePath.Reverse();
                                            input.MoveItem(item, this, 0, reversePath);
                                        }
                                        else
                                        {
                                            if (Globals.UltraDebug) { Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}][{ParentNetwork.ID}] {item.Name} CORRECTLY SENT"); }
                                        }
                                    }
                                    else
                                    {
                                        if (Globals.UltraDebug) { Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}][{ParentNetwork.ID}] {item.Name} PATH BROKEN, REVERSE"); }
                                        List<PipeNode> reversePath = path;
                                        reversePath.Reverse();
                                        int brokenIndex = reversePath.IndexOf(broken);
                                        input.MoveItem(item, this, brokenIndex + 1, reversePath);
                                        inChest.SendItem(outChest, item);
                                    }
                                }
                            }
                        }
                    }
                    else if (ConnectedContainer is ChestContainerNode && input.ConnectedContainer is ShippingBinContainerNode)
                    {
                        ShippingBinContainerNode shipBin = (ShippingBinContainerNode)input.ConnectedContainer;
                        ChestContainerNode outChest = (ChestContainerNode)ConnectedContainer;
                        if (!outChest.IsEmpty())
                        {
                            item = outChest.GetItemToShip(input);
                            if (item != null)
                            {
                                PipeNode broken = MoveItem(item, input, 0, path);
                                if (outChest != null)
                                {
                                    if (broken == null)
                                    {
                                        shipBin.ShipItem(item);
                                    }
                                    else
                                    {
                                        if (Globals.UltraDebug) { Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}][{ParentNetwork.ID}] {item.Name} PATH BROKEN, REVERSE"); }
                                        List<PipeNode> reversePath = path;
                                        reversePath.Reverse();
                                        int brokenIndex = reversePath.IndexOf(broken);
                                        input.MoveItem(item, this, brokenIndex + 1, reversePath);
                                        outChest.InsertItem(item);
                                    }
                                }
                            }
                        }
                    }
                }
                index++;
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
        */
        public bool CanSendItems(InputPipeNode input)
        {
            bool canSend = false;
            if(ConnectedContainer != null && ConnectedContainer.CanSendItems() 
                && input.CanRecieveItems())
            {
                canSend = true;
            }
            return canSend;
        }

        public Item GetItemFor(InputPipeNode input)
        {
            Item item = null;
            item = ConnectedContainer.GetItemForInput(input);
            return item;
        }

        public void StartExchage2()
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
                                Printer.Info($"Item to send: {item.Name}({item.Stack})");
                                SendItem(item, input);
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
                }
            }
            /*
                            else if (ConnectedContainer is ChestContainerNode && input.ConnectedContainer is ShippingBinContainerNode)
                            {
                                ShippingBinContainerNode shipBin = (ShippingBinContainerNode)input.ConnectedContainer;
                                ChestContainerNode outChest = (ChestContainerNode)ConnectedContainer;
                                if (!outChest.IsEmpty())
                                {
                                    item = outChest.GetItemToShip(input);
                                    if (item != null)
                                    {
                                        PipeNode broken = MoveItem(item, input, 0, path);
                                        if (outChest != null)
                                        {
                                            if (broken == null)
                                            {
                                                shipBin.ShipItem(item);
                                            }
                                            else
                                            {
                                                if (Globals.UltraDebug) { Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}][{ParentNetwork.ID}] {item.Name} PATH BROKEN, REVERSE"); }
                                                List<PipeNode> reversePath = path;
                                                reversePath.Reverse();
                                                int brokenIndex = reversePath.IndexOf(broken);
                                                input.MoveItem(item, this, brokenIndex + 1, reversePath);
                                                outChest.InsertItem(item);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        index++;
                    }
                }
                else
                {

                }


            /*/
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
            if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] Does {Print()} have a valid adjacent container? " + (ConnectedContainer != null).ToString()); }
            if (Globals.UltraDebug) { Printer.Info($"[N{ParentNetwork.ID}] Does {input.Print()} have a valid adjacent container? " + (input.ConnectedContainer != null).ToString()); }
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
            ConnectPipe(path.Last(), 0, path);
            try
            {
                if(DataAccess.GetDataAccess().Threads.Contains(Thread.CurrentThread))
                {
                    DataAccess.GetDataAccess().Threads.Remove(Thread.CurrentThread);
                }
            }
            catch(Exception e)
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
    }
}
