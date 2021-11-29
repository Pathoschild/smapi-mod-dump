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
using StardewValley.Objects;
using ItemPipes.Framework.Model;
using Netcode;

namespace ItemPipes.Framework
{
    public class Container : Node
    {
        public string Type { get; set; }
        public Output Output { get; set; }
        public Input Input { get; set; }
        public List<string> Filter { get; set; }

        public Container(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            Type = "";
            Output = null;
            Input = null;
            Filter = new List<string>();
        }
        public override bool AddAdjacent(Side side, Node entity)
        {
            bool added = false;
            if (Adjacents[side] == null)
            {
                added = true;
                Adjacents[side] = entity;
                entity.AddAdjacent(Sides.GetInverse(side), this);
                if (Output == null && entity is Output)
                {
                    Output = (Output)entity;
                    if (Globals.Debug) { Printer.Info($"[?] OUTPUT ADDED"); }
                }
                else if (Input == null && entity is Input)
                {
                    Input = (Input)entity;
                    if (Globals.Debug) { Printer.Info($"[?] INPUT ADDED"); }
                }
            }
            return added;
        }

        public override bool RemoveAdjacent(Side side, Node entity)
        {
            bool removed = false;
            if (Adjacents[side] != null)
            {
                removed = true;
                if (Output != null && entity is Output)
                {
                    Output = null;
                    if (Globals.Debug) { Printer.Info($"[?] OUTPUT REMOVED"); }
                }
                else if (Input != null && entity is Input)
                {
                    Input = null;
                    if (Globals.Debug) { Printer.Info($"[?] INPUT REMOVED"); }
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

        public virtual bool IsEmpty()
        {
            return false;
        }

        public virtual List<string> UpdateFilter(NetObjectList<Item> filteredItems)
        {
            return null;
        }
    }
}
