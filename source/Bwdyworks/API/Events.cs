using bwdyworks.Events;
using Microsoft.Xna.Framework;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace bwdyworks.API
{
    public class Events
    {
        //NPC Check Action - called when a player activates an NPC. Cancellable.
        public event NPCCheckActionHandler NPCCheckAction;
        public delegate void NPCCheckActionHandler(object sender, NPCCheckActionEventArgs args);
        internal NPCCheckActionEventArgs NPCCheckActionEvent(Farmer who, NPC npc)
        {
            var args = new NPCCheckActionEventArgs
            {
                NPC = npc,
                Farmer = who,
                Cancelled = false
            };
            if (NPCCheckAction != null) NPCCheckAction.Invoke(this, args);
            return args;
        }

        //Tile Check Action - called when a player activates a tile with an Action property. Cancellable.
        public event TileCheckActionHandler TileCheckAction;
        public delegate void TileCheckActionHandler(object sender, TileCheckActionEventArgs args);
        internal TileCheckActionEventArgs TileCheckActionEvent(Farmer who, GameLocation location, Vector2 tile, string action)
        {
            var args = new TileCheckActionEventArgs
            {
                GameLocation = location,
                TileLocation = tile,
                Action = action,
                Farmer = who,
                Cancelled = false
            };
            if (TileCheckAction != null) TileCheckAction.Invoke(this, args);
            return args;
        }

        //ItemEaten - called when a player starts eating an item. Not cancellable (because of how it's detected)
        public event ItemEatenHandler ItemEaten;
        public delegate void ItemEatenHandler(object sender, ItemEatenEventArgs args);
        internal ItemEatenEventArgs ItemEatenEvent(Farmer who, StardewValley.Item item)
        {
            var args = new ItemEatenEventArgs
            {
                Farmer = who,
                Item = item
            };
            ItemEaten?.Invoke(this, args);
            return args;
        }
    }
}
