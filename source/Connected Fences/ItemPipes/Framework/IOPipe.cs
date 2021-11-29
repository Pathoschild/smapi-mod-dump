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
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace ItemPipes.Framework
{
    public class IOPipe : Node
    {
        public Container ConnectedContainer { get; set; }
        public string State { get; set; }
        public IOPipe(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedContainer = null;
            State = "unconnected";
        }

        public override bool AddAdjacent(Side side, Node entity)
        {
            bool added = false;
            if (Adjacents[side] == null)
            {
                if (ConnectedContainer == null && entity is Container)
                {
                    Container container = (Container)entity;
                    if ((this is Output && container.Output == null) ||
                        (this is Input && container.Input == null))
                    {
                        ConnectedContainer = (Container)entity;
                        State = "on";
                        if (Globals.Debug) { Printer.Info($"[?] CONNECTED CONTAINER ADDED"); }
                    }

                }
                else
                {
                    if (Globals.Debug) { Printer.Info($"[?] Didnt add adj container"); }
                }
                added = true;
                Adjacents[side] = entity;
                entity.AddAdjacent(Sides.GetInverse(side), this);
            }
            return added;
        }

        public override bool RemoveAdjacent(Side side, Node entity)
        {
            bool removed = false;
            if (Adjacents[side] != null)
            {
                removed = true;
                if (ConnectedContainer != null && entity is Container)
                {
                    ConnectedContainer = null;
                    State = "unconnected";
                    if (Globals.Debug) { Printer.Info($"[?] CONNECTED CONTAINER REMOVED"); }
                }
                Adjacents[side] = null;
                entity.RemoveAdjacent(Sides.GetInverse(side), this);
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
                    Adjacents[adj.Key] = null;
                }
            }
            return removed;
        }

    }
}
