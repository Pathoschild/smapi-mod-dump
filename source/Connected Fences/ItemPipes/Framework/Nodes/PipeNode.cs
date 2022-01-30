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
    public class PipeNode : Node
    {
        public Item StoredItem { get; set; }
        public int ItemTimer { get; set; }
        public bool PassingItem { get; set; }
        public bool Broken { get; set; }

        public PipeNode() : base()
        {

        }
        public PipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            PassingItem = false;
            Broken = false;
        }

        public Node MoveItem(Item item, Node target, int index, List<Node> path)
        {
            Node broken = null;
            DisplayItem(item);
            if (!this.Equals(target))
            {
                //Printer.Info($"[T{ Thread.CurrentThread.ManagedThreadId}] Path lenght: "+path.Count);
                //Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] Index: " + index);
                if (index < path.Count-1)
                {
                    index++;
                    Node nextNode = path[index];
                    if (Location.getObjectAtTile((int)nextNode.Position.X, (int)nextNode.Position.Y) != null)
                    {
                        if (nextNode is PipeNode)
                        {
                            PipeNode pipe = (PipeNode)nextNode;
                            //Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] Index: " + index);
                            //Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] Broken? " + Broken);
                            if (!Broken)
                            {
                                broken = pipe.MoveItem(item, target, index, path);
                            }
                            else
                            {
                                //Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] Broken? when true " + Broken);
                                broken = nextNode;
                                return broken;
                            }
                        }
                    }
                    else
                    {
                        broken = nextNode;
                        return broken;
                    }
                }
            }

            return broken;
        }

        public bool DisplayItem(Item item)
        {
            bool canLoad = false;
            if (StoredItem == null)
            {
                StoredItem = item;
                PassingItem = true;
                Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] Passing {item.Name} ");
                System.Threading.Thread.Sleep(ItemTimer);
                StoredItem = null;
                PassingItem = false;
            }
            return canLoad;
        }

    }
}
