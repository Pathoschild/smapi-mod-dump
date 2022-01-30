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
using ItemPipes.Framework.Util;
using ItemPipes.Framework.Nodes;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Objects;

namespace ItemPipes.Framework
{
    public abstract class IOPipeNode : PipeNode
    {
        public ContainerNode ConnectedContainer { get; set; }
        public bool Connecting { get; set; }


        public IOPipeNode() : base()
        {
            ConnectedContainer = null;
            State = "unconnected";
            Connecting = false;
        }
        
        public IOPipeNode(Vector2 position, GameLocation location, StardewValley.Object obj) : base(position, location, obj)
        {
            ConnectedContainer = null;
            State = "nochest";
            Connecting = false;

        }

        public virtual void UpdateState()
        {
            if (ConnectedContainer == null)
            {
                State = "nochest";
            }
            else if (ConnectedContainer != null)
            {
                State = "on";
            }
        }

        public override string GetState()
        {
            if (Connecting)
            {
                return "connecting_"+State;
            }
            else
            {
                return State;
            }
        }

        public bool AddConnectedContainer(Node entity)
        {
            bool added = false;
            if (Globals.UltraDebug) { Printer.Info($"[?] Adding {entity.Name} container to {Print()} "); }
            if (Globals.UltraDebug) { Printer.Info($"[?] Alreadyhas a container? {ConnectedContainer != null}"); }
            if (ConnectedContainer == null && entity is ContainerNode)
            {
                if (Globals.UltraDebug) { Printer.Info($"[?] Connecting adjacent container.."); }
                ContainerNode container = (ContainerNode)entity;
                if ((this is OutputNode && container.Output == null) ||
                    (this is InputNode && container.Input == null))
                {
                    ConnectedContainer = (ContainerNode)entity;
                    ConnectedContainer.AddIOPipe(this);
                    if (Globals.UltraDebug) { Printer.Info($"[?] CONNECTED CONTAINER ADDED"); }
                }
                else
                {
                    if (Globals.UltraDebug) { Printer.Info($"[?] Didnt add adj container"); }
                }
            }
            else
            {
                if (Globals.UltraDebug) { Printer.Info($"[?] Didnt add adj container"); }
            }
            UpdateState();
            added = true;
            return added;
        }

        public bool RemoveConnectedContainer(Node entity)
        {
            bool removed = false;
            if (Globals.UltraDebug) { Printer.Info($"[?] Removing {entity.Name} container "); }
            if (ConnectedContainer != null && entity is ContainerNode)
            {
                ConnectedContainer.RemoveIOPipe(this);
                ConnectedContainer = null;
                if (Globals.UltraDebug) { Printer.Info($"[?] CONNECTED CONTAINER REMOVED"); }
                removed = true;
            }
            UpdateState();
            return removed;
        }
    }
}
