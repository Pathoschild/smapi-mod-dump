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
using ItemPipes.Framework.Items.Objects;
using ItemPipes.Framework.Util;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;
using SObject = StardewValley.Object;


namespace ItemPipes.Framework.Nodes.ObjectNodes
{
    public class InvisibilizerNode : PipeNode
    {
        public InvisibilizerNode() { }
        public InvisibilizerNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ItemTimer = 100;
            State = "off";
            LoadState();
        }

        

        public void LoadState()
        {
            SObject item;
            if (Location.objects.TryGetValue(Position, out item))
            {
                InvisibilizerItem pipo = (InvisibilizerItem)item;
                State = pipo.State;
            }
            if (State.Equals("on"))
            {
                Passable = true;
            }
            else
            {
                Passable = false;
            }
        }

        public bool ChangeState()
        {
            if(State.Equals("on"))
            {
                State = "off";
                Passable = false;
                ParentNetwork.Deinvisibilize(this);
                return false;
            }
            else
            {
                State = "on";
                Passable = true;
                ParentNetwork.Invisibilize(this);
                return true;
            }
        }

        public void UpdateItemTimer()
        {
            List<Node> nonNullAdj = Adjacents.Values.Where(a => a != null && a is PipeNode).ToList();
            if (nonNullAdj.Count > 0)
            {
                ItemTimer = (nonNullAdj.OrderBy(a => (a as PipeNode).ItemTimer).ToList()[0] as PipeNode).ItemTimer;
            }
            else
            {
                ItemTimer = 1000;
            }
        }

        public override bool AddAdjacent(Side side, Node node)
        {
            bool added = base.AddAdjacent(side, node);
            UpdateItemTimer();
            return added;
        }

        public override bool RemoveAdjacent(Side side, Node node)
        {
            bool removed = base.RemoveAdjacent(side, node);
            UpdateItemTimer();
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
            return removed;
        }
    }
}
