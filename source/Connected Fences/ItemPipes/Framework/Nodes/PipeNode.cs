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

namespace ItemPipes.Framework.Nodes
{
    public abstract class PipeNode : Node
    {
        public Item StoredItem { get; set; }
        public int ItemTimer { get; set; }
        public bool PassingItem { get; set; }
        public bool Connecting { get; set; }
        public bool Broken { get; set; }

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
            if (Globals.UltraDebug) { Printer.Debug($"Getting path for {target.Print()}"); }
            List<PipeNode> path = new List<PipeNode>();
            path = GetPathRecursive(target, path);
            return path;
        }

        public List<PipeNode> GetPathRecursive(PipeNode target, List<PipeNode> path)
        {
            //if (Globals.UltraDebug) { Printer.Info(Print()); }
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

        public PipeNode SendItem(Item item, IOPipeNode input)
        {
            List<PipeNode> path = GetPath(input);
            /*
            Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] PATH---------------");
            foreach (Node node in path)
            {
                Printer.Info(node.Print());
            }
            Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] PATH---------------");
            */
            return MoveItemRecursive(item, input, path, 0);
        }

        public PipeNode MoveItemRecursive(Item item, IOPipeNode input, List<PipeNode> path, int index)
        {
            //Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}]Current node: {Print()}");
            if (this.Equals(input))
            {
                if(input.ConnectedContainer != null)
                {
                    StoredItem = item;
                    PassingItem = true;
                    bool interrupted = false;
                    while (input.ConnectedContainer != null && !input.ConnectedContainer.InsertItem(item) && !interrupted)
                    {
                        try
                        {
                            StoredItem = item;
                            PassingItem = true;
                            System.Threading.Thread.Sleep(ItemTimer);
                        }
                        catch (ThreadInterruptedException exception)
                        {
                            interrupted = true;
                        }
                    }
                    try
                    {
                        System.Threading.Thread.Sleep(ItemTimer);
                    }
                    catch (ThreadInterruptedException exception)
                    {
                    }
                    if(interrupted)
                    {
                        StoredItem = item;
                        PassingItem = true;
                    }
                    else
                    {
                        StoredItem = null;
                        PassingItem = false;
                    }
                    return this;
                }
                else
                {
                    return null;
                }
            }
            else 
            {
                if (index < path.Count - 1 && path[index + 1] != null)
                {
                    StoredItem = item;
                    PassingItem = true;
                    while (path[index + 1] != null && path[index + 1].StoredItem != null)
                    {
                        StoredItem = item;
                        PassingItem = true;
                    }
                    try
                    {
                        System.Threading.Thread.Sleep(ItemTimer);
                    }
                    catch (ThreadInterruptedException exception) { }
                    StoredItem = null;
                    PassingItem = false;
                    index++;
                    return path[index].MoveItemRecursive(item, input, path, index);
                }
            }
            return this;
        }

        public bool FlushPipe(Item item, IOPipeNode input)
        {
            bool flushed = false;
            if (input.ConnectedContainer != null)
            {
                StoredItem = item;
                PassingItem = true;
                bool interrupted = false;
                while (input.ConnectedContainer != null && !input.ConnectedContainer.InsertItem(item) && !interrupted)
                {
                    try
                    {
                        StoredItem = item;
                        PassingItem = true;
                    }
                    catch (ThreadInterruptedException exception)
                    {
                        interrupted = true;
                    }
                }
            }
            return flushed;
        }

        public void ConnectPipe(PipeNode target)
        {
            List<PipeNode> path = GetPath(target);
            /*
            Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] PATH---------------");
            foreach (Node node in path)
            {
                Printer.Info(node.Print());
            }
            Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] PATH---------------");
            */
            ConnectPipeRecursive(target, path, 0);
            foreach (PipeNode pipe in path)
            {
                pipe.Connecting = false;
                try
                {
                    System.Threading.Thread.Sleep(20);
                }
                catch (ThreadInterruptedException exception) { }
            }
        }
        public PipeNode ConnectPipeRecursive(PipeNode target, List<PipeNode> path, int index)
        {
            PipeNode node = null;
            if (this.Equals(target))
            {
                Connecting = true;
                try
                {
                    System.Threading.Thread.Sleep(60);
                }
                catch (ThreadInterruptedException exception) { }
                return target;
            }
            else
            {
                if (index < path.Count - 1 && path[index + 1] != null)
                {
                    Connecting = true;
                    try
                    {
                        System.Threading.Thread.Sleep(60);
                    }
                    catch (ThreadInterruptedException exception) { }
                    index++;
                    path[index].ConnectPipeRecursive(target, path, index);
                }
            }
            return node;
        }
    }
}
