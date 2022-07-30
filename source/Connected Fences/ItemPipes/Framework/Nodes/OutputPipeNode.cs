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
        public List<InputPipeNode> SavedState { get; set; }
        public bool RoundRobin { get; set; }
        public int RotateIndex { get; set; }
        public OutputPipeNode() : base()
        {
            RoundRobin = false;
            RotateIndex = 0;
        }
        public OutputPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedInputs = new Dictionary<InputPipeNode, List<PipeNode>>();
            RoundRobin = false;
            RotateIndex = 0;
        }

        public void ChangeMode()
        {
            if(RoundRobin)
            {
                RoundRobin = false;
            }
            else
            {
                RoundRobin = true;
            }
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
            if (ConnectedContainer != null && !ConnectedContainer.IsEmpty()
                && ConnectedInputs.Count > 0 && Signal.Equals("on"))
            {
                try
                {
                    ParentNetwork.Update();
                    StartExchage();
                }
                catch (ThreadInterruptedException exception)
                {
                }
            }
        }

        public bool CanSendItems(InputPipeNode input)
        {
            bool canSend = false;
            if (ConnectedContainer != null && ConnectedContainer.CanSendItems()
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
            Item item = null;
            int index = 0;
            Dictionary<InputPipeNode, List<PipeNode>> priorityInputs = ConnectedInputs;
            priorityInputs = priorityInputs.
                OrderByDescending(pair => pair.Key.Priority).
                ThenBy(pair => pair.Value.Count).
                ToDictionary(x => x.Key, x => x.Value);
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
                                Animator.AnimateItemMovement(path, input, this, item);
                            }
                            else if (StoredItem != null)
                            {
                                //Printer.Info($"Output locked");
                                //Output locked
                            }
                            else if (item == null)
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
                List<InputPipeNode> rotatedInputs;
                if (SavedState != null)
                {
                    rotatedInputs = SavedState;
                }
                else
                {
                    rotatedInputs = RotateLeft(priorityInputs.Keys.ToList(), RotateIndex);
                }
                while (index < rotatedInputs.Count && item == null)
                {
                    InputPipeNode input = rotatedInputs[index];
                    if (input.Signal.Equals("on"))
                    {
                        List<PipeNode> path = priorityInputs[rotatedInputs[index]];
                        input.UpdateFilter();
                        if (CanSendItems(input))
                        {
                            item = GetItemFor(input);
                            if (item != null && StoredItem == null)
                            {
                                Animator.AnimateItemMovement(path, input, this, item);
                            }
                            else if (StoredItem != null)
                            {
                                //Printer.Info($"Output locked");
                                //Output locked
                            }
                            else if (item == null)
                            {
                                //Printer.Info($"Item is null");
                                //Item is null
                            }
                        }
                    }
                    index++;
                }
                if(item == null)
                {
                    SavedState = rotatedInputs;
                }
                else
                {
                    SavedState = null;
                }
            }
        }

        public List<InputPipeNode> RotateLeft(List<InputPipeNode> items, int places)
        {
            if(places >= items.Count)
            {
                places = 0;
                RotateIndex = 0;
            }
            InputPipeNode[] range = new InputPipeNode[places];
            items.CopyTo(items.Count - places, range, 0, places);
            items.RemoveRange(items.Count - places, places);
            items.InsertRange(0, range);
            RotateIndex++;
            return items;
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
            if (ConnectedContainer != null && input.ConnectedContainer != null)
            {
                List<PipeNode> path;
                path = GetPath(input);
                if(path.Count > 0)
                {
                    added = true;
                    ConnectedInputs.Add(input, path);
                    RotateIndex = 0;
                    Animator.AnimatePipeConnection(path);
                }
            }
            return added;
        }

        public bool RemoveConnectedInput(InputPipeNode input)
        {
            bool removed = false;
            if (ConnectedInputs.Keys.Contains(input))
            {
                removed = true;
                ConnectedInputs.Remove(input);
                RotateIndex = 0;
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
