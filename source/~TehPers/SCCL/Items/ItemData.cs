using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TehPers.Stardew.SCCL.Items {
    internal class ItemData {

        public string ItemID { get; set; } = null;
        public int Slot { get; set; } = 0;
        public string Location { get; set; } = null;
        public Vector2 Position { get; set; } = Vector2.Zero;
        public Dictionary<string, object> Data { get; set; } = new Dictionary<string, object>();

        public ItemData() { }

        public ItemData(string id, int slot, Dictionary<string, object> data) : this(id, slot, data, null, Vector2.Zero) { }

        public ItemData(string id, int slot, Dictionary<string, object> data, string location, Vector2 position) {
            this.ItemID = id;
            this.Slot = slot;
            this.Location = location;
            this.Position = position;
            this.Data = data;
        }
    }
}
