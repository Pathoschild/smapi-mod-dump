using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TehPers.Stardew.SCCL.Items;

namespace TehPers.Stardew.SCCL.Testing {
    public class ItemCustom : ItemTemplate {
        public ItemCustom(string id) : base(id) { }

        public override string GetDescription(Dictionary<string, object> data) {
            return "An item used solely for testing";
        }

        public override string GetName(Dictionary<string, object> data) {
            return "Test Item";
        }

        public override Texture2D GetTexture(Dictionary<string, object> data) {
            return null; // Shouldn't do anything atm anyway
        }
    }
}
