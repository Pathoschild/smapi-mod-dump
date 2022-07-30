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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewModdingAPI.Events;
using SObject = StardewValley.Object;
using StardewValley.Tools;
using System.Xml.Serialization;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Model;
using System.Threading;
using System.Threading.Tasks;

namespace ItemPipes.Framework.Nodes
{
    public abstract class PipeNode : Node
    {
        public Item StoredItem { get; set; }
        public int ItemTimer { get; set; }
        public bool PassingItem { get; set; }
        public bool Connecting { get; set; }
        public bool Broken { get; set; }
        public Queue<double> StartTimeQ { get; set; }
        public Queue<double> EndTimeQ { get; set; }
        public Queue<Item> ItemQ { get; set; }
        public Queue<Tuple<IOPipeNode, IOPipeNode>> InOutQ { get; set; }
        public bool ItemStuck { get; set; }

        public PipeNode() : base()
        {
            State = "default";
        }
        public PipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            State = "default";

            PassingItem = false;
            Connecting = false;
            Broken = false;
            ItemStuck = false;

            StartTimeQ = new Queue<double>();
            EndTimeQ = new Queue<double>();
            ItemQ = new Queue<Item>();
            InOutQ = new Queue<Tuple<IOPipeNode, IOPipeNode>>();
        }

        public bool CanConnectedWith(PipeNode target)
        {
            bool connected = false;
            List<PipeNode> path = GetPath(target);
            if (path.Count > 0 && path.Last().Equals(target))
            {
                connected = true;
            }
            return connected;
        }

        public List<PipeNode> GetPath(PipeNode target)
        {
            List<PipeNode> path = new List<PipeNode>();
            path = GetPathRecursive(target, path);
            return path;
        }

        public List<PipeNode> GetPathRecursive(PipeNode target, List<PipeNode> path)
        {
            Node adj;
            if (path.Contains(target))
            {
                return path;
            }
            else
            {
                path.Add(this);
                adj = Adjacents[Sides.North];
                if (!path.Contains(target) && adj != null && adj is PipeNode && !path.Contains(adj))
                {
                    PipeNode adjPipe = (PipeNode)adj;
                    path = adjPipe.GetPathRecursive(target, path);
                }
                adj = Adjacents[Sides.South];
                if (!path.Contains(target) && adj != null && adj is PipeNode && !path.Contains(adj))
                {
                    PipeNode adjPipe = (PipeNode)adj;
                    path = adjPipe.GetPathRecursive(target, path);
                }
                adj = Adjacents[Sides.East];
                if (!path.Contains(target) && adj != null && adj is PipeNode && !path.Contains(adj))
                {
                    PipeNode adjPipe = (PipeNode)adj;
                    path = adjPipe.GetPathRecursive(target, path);
                }
                adj = Adjacents[Sides.West];
                if (!path.Contains(target) && adj != null && adj is PipeNode && !path.Contains(adj))
                {
                    PipeNode adjPipe = (PipeNode)adj;
                    path = adjPipe.GetPathRecursive(target, path);
                }
                if (!path.Contains(target))
                {
                    path.Remove(this);
                }
                return path;
            }
        }

        public bool ReadyForAnimation()
        {
            if(StartTimeQ.Count > 0 && EndTimeQ.Count > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void StartItemMovementAnimation(double current, IOPipeNode target, IOPipeNode source, Item item)
        {
            StartTimeQ.Enqueue(current);
            EndTimeQ.Enqueue(current + ItemTimer);
            ItemQ.Enqueue(item);
            InOutQ.Enqueue(new Tuple<IOPipeNode, IOPipeNode>(target, source));
        }
        public void StartConnectionAnimation(double current, double endTime)
        {
            StartTimeQ.Enqueue(current);
            EndTimeQ.Enqueue(endTime);
        }

        public void EndAnimation()
        {
            if (StartTimeQ.Count > 0 && EndTimeQ.Count > 0)
            {
                StartTimeQ.Dequeue();
                EndTimeQ.Dequeue();
                if(ItemQ.Count > 0 && InOutQ.Count > 0 && PassingItem)
                {
                    Item item = ItemQ.Dequeue();
                    Tuple<IOPipeNode, IOPipeNode> inout = InOutQ.Dequeue();
                    if (this.Equals(inout.Item1))
                    {
                        if(inout.Item1.ConnectedContainer != null )
                        {
                            if(inout.Item1.ConnectedContainer.InsertItem(item))
                            {
                                if (ModEntry.config.DebugMode) { Printer.Debug($"[N{ID}] Inserted {item.Name}({item.Stack}) to {inout.Item1.Print()}"); }
                                StoredItem = null;
                            }
                            else
                            {
                                Printer.Warn($"[N{ID}] Coudn't insert {item.Name}({item.Stack}) to {inout.Item1.Print()}");
                                if (this is OutputPipeNode)
                                {
                                    Utilities.DropItem(item, Position, Location);
                                    Game1.addHUDMessage(new HUDMessage($"Dropped {item.Stack} {item.Name} at {Position} {Location.Name}", 3));
                                    Game1.addHUDMessage(new HUDMessage(DataAccess.GetDataAccess().Warnings["outputFull"], 3));
                                    Printer.Error($"{DataAccess.GetDataAccess().Warnings["outputFull"]} at: {Position} {Location.Name} dropped {item.Stack} {item.Name}");

                                }
                                else
                                {
                                    List<PipeNode> reversePath = GetPath(inout.Item2);
                                    Animator.AnimateItemMovement(reversePath, inout.Item2, null, item);
                                    Game1.addHUDMessage(new HUDMessage($" At: {Position} {Location.Name}", 3));
                                    Game1.addHUDMessage(new HUDMessage(DataAccess.GetDataAccess().Warnings["inputFull"], 3));
                                    Printer.Warn($"{DataAccess.GetDataAccess().Warnings["inputFull"]} at: {Position} {Location.Name}");
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        StoredItem = null;
                    }
                }
  
            }
        }

        public void FlushPipe()
        {
            if (StartTimeQ.Count > 0 && EndTimeQ.Count > 0 && ItemQ.Count > 0 && InOutQ.Count > 0)
            {
                int i = 0;
                int itemCount = ItemQ.Count;
                int inoutCount = InOutQ.Count;
                StartTimeQ.Clear();
                EndTimeQ.Clear();
                while (i < itemCount && i < inoutCount)
                {
                    Item item = ItemQ.Dequeue();
                    Tuple<IOPipeNode, IOPipeNode> inout = InOutQ.Dequeue();                    
                    if (this.Equals(inout.Item1))
                    {
                        if (inout.Item1.ConnectedContainer != null)
                        {
                            if (inout.Item1.ConnectedContainer.InsertItem(item))
                            {
                                StoredItem = null;
                            }
                            else
                            {
                                if (this is OutputPipeNode)
                                {
                                    Utilities.DropItem(item, Position, Location);
                                    Game1.addHUDMessage(new HUDMessage($"Dropped {item.Stack} {item.Name} at {Position} {Location.Name}", 3));
                                    Game1.addHUDMessage(new HUDMessage(DataAccess.GetDataAccess().Warnings["outputFull"], 3));
                                    Printer.Error($"{DataAccess.GetDataAccess().Warnings["outputFull"]} at: {Position} {Location.Name} dropped {item.Stack} {item.Name}");

                                }
                                else
                                {
                                    List<PipeNode> reversePath = GetPath(inout.Item2);
                                    Animator.AnimateItemMovement(reversePath, inout.Item2, null, item);
                                    //Game1.addHUDMessage(new HUDMessage($" At: {Position} {Location.Name}", 3));
                                    //Game1.addHUDMessage(new HUDMessage(DataAccess.GetDataAccess().Warnings["inputFull"], 3));
                                    Printer.Warn($"{DataAccess.GetDataAccess().Warnings["inputFull"]} at: {Position} {Location.Name}");
                                    inout.Item2.FlushPipe();
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                    else
                    {
                        StoredItem = null;
                    }
                    i++;
                }
            }
        }
    }
}
