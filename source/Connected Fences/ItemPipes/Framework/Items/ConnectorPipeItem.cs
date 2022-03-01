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
using Microsoft.Xna.Framework.Graphics;
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Model;
using ItemPipes.Framework.Nodes;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Objects;
using SObject = StardewValley.Object;
using System.Threading;
using System.Xml.Serialization;


namespace ItemPipes.Framework.Items
{
    public abstract class ConnectorPipeItem : PipeItem
    {
        public ConnectorPipeItem() : base()
        {
            
        }
        public ConnectorPipeItem(Vector2 position) : base(position)
        {
            
        }

        public override bool performToolAction(Tool t, GameLocation location)
        {
            if (t is Pickaxe)
            {
                var who = t.getLastFarmerToUse();
                this.performRemoveAction(this.TileLocation, location);
                Debris deb = new Debris(getOne(), who.GetToolLocation(), new Vector2(who.GetBoundingBox().Center.X, who.GetBoundingBox().Center.Y));
                Game1.currentLocation.debris.Add(deb);
                DataAccess DataAccess = DataAccess.GetDataAccess();
                List<Node> nodes = DataAccess.LocationNodes[Game1.currentLocation];
                Node node = nodes.Find(n => n.Position.Equals(TileLocation));
                if (node != null && node is ConnectorPipeNode)
                {
                    ConnectorPipeNode pipe = (ConnectorPipeNode)node;
                    if (pipe.StoredItem != null)
                    {
                            
                        Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] GET OUT");
                        Printer.Info($"[T{Thread.CurrentThread.ManagedThreadId}] "+pipe.StoredItem.Stack.ToString());
                        pipe.Print();
                        Debris itemDebr = new Debris(pipe.StoredItem, who.GetToolLocation(), new Vector2(who.GetBoundingBox().Center.X, who.GetBoundingBox().Center.Y));
                        Game1.currentLocation.debris.Add(itemDebr);
                        pipe.Broken = true;
                    }
                }
                Game1.currentLocation.objects.Remove(this.TileLocation);
                return false;
            }
            return false;
        }

        public override bool countsForDrawing(SObject adj)
        {
            if (adj is PipeItem && !(adj is ConnectorPipeItem))
            {
                return true;
            }
            else if(adj is PipeItem && adj is ConnectorPipeItem)
            {
                if(adj.GetType().Equals(this.GetType()))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
