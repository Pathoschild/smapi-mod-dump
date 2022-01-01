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
using ItemPipes.Framework.Objects;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace ItemPipes.Framework
{
    public class Output : IOPipe
    {
        public Dictionary<Input, List<Node>> ConnectedInputs { get; set; }

        public Output(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedInputs = new Dictionary<Input, List<Node>>();

        }

        public void ProcessExchanges()
        {
            if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] Procesing Exchanges..."); }
            if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] Are there connected input? " + (ConnectedInputs.Count > 0).ToString()); }
            if (ConnectedContainer != null && !ConnectedContainer.IsEmpty() && ConnectedInputs.Count > 0 && State.Equals("on"))
            {
                if (Globals.Debug) { Printer.Info($"[{ ParentNetwork.ID}] Is output empty? " + ConnectedContainer.IsEmpty().ToString()); }
                if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] CREATED NEW THREAD"); }
                Thread thread = new Thread(new ThreadStart(StartExchage));
                thread.Start();
                //StartExchage();
            }
        }

        public void StartExchage()
        {
            if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] Number of inputs: " + ConnectedInputs.Count.ToString()); }
            Item item = null;
            int index = 0;
            Dictionary<Input, List<Node>> priorityInputs = ConnectedInputs;
            priorityInputs = priorityInputs.
                OrderByDescending(pair => pair.Key.Priority).
                ThenBy(pair => pair.Value.Count).
                ToDictionary(x => x.Key, x => x.Value);
            index = 0;
            while (index < priorityInputs.Count && item == null)
            {
                Input input = priorityInputs.Keys.ToList()[index];
                if(input.State.Equals("on"))
                {
                    List<Node> path = priorityInputs.Values.ToList()[index];
                    if (input is PolymorphicPipe)
                    {
                        PolymorphicPipe poly = (PolymorphicPipe)input;
                        poly.UpdateFilter();
                    }
                    else if (input is FilterPipe)
                    {
                        FilterPipe filter = input as FilterPipe;
                        filter.UpdateFilter();
                    }
                    if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] INPUT: "); input.Print(); }
                    if (ConnectedContainer != null && input.ConnectedContainer != null)
                    {
                        if (ConnectedContainer.Type.Equals("Chest") && input.ConnectedContainer.Type.Equals("ShippingBin"))
                        {
                            ShippingBinContainer shipBin = (ShippingBinContainer)input.ConnectedContainer;
                            ChestContainer outChest = (ChestContainer)ConnectedContainer;
                            item = outChest.GetItemToShip(input);
                            if (item != null)
                            {
                                Animator.AnimateItemSending(path);
                                shipBin.ShipItem(item);
                                if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] END animation"); }
                            }
                        }
                        else if (ConnectedContainer.Type.Equals("Chest") && input.ConnectedContainer.Type.Equals("Chest"))
                        {
                            ChestContainer inChest = (ChestContainer)input.ConnectedContainer;
                            ChestContainer outChest = (ChestContainer)ConnectedContainer;
                            item = outChest.CanSendItem(inChest);
                            if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] Can send? " + (item != null).ToString()); }
                            if (item != null)
                            {
                                Animator.AnimateItemSending(path);
                                if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] ITEM CORRECTLY SENT"); }
                                if (outChest != null && inChest != null)
                                {
                                    bool sent = outChest.SendItem(inChest, item);
                                    if (!sent)
                                    {
                                        if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] CANT ENTER, REVERSE"); }
                                        List<Node> reversePath = path;
                                        reversePath.Reverse();
                                        Animator.AnimateItemSending(reversePath);
                                    }
                                }
                            }
                        }
                    }
                }
                index++;
            }
        }

        public bool IsInputConnected(Input input)
        {
            bool connected = false;
            if (ConnectedInputs.Keys.Contains(input))
            {
                connected = true;
            }
            return connected;
        }

        public bool AddConnectedInput(Input input)
        {
            bool added = false;
            if (!ConnectedInputs.Keys.Contains(input))
            {
                if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] Output container null? " + (ConnectedContainer == null).ToString()); }
                if (Globals.Debug) { Printer.Info($"[{ParentNetwork.ID}] Input container null? " + (input.ConnectedContainer == null).ToString()); }
                if (ConnectedContainer != null && input.ConnectedContainer != null)
                {
                    added = true;
                    List<Node> path;
                    path = ConnectedContainer.GetPath(input.ConnectedContainer);
                    Animator.AnimateInputConnection(path);
                    ConnectedInputs.Add(input, path);
                }
            }
            return added;
        }

        public bool RemoveConnectedInput(Input input)
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
