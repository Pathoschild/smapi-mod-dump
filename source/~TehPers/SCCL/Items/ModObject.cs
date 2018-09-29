using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using TehPers.Stardew.Framework;

namespace TehPers.Stardew.SCCL.Items {
    public class ModObject : StardewValley.Object {

        public ItemTemplate Template { get; set; }
        public Dictionary<string, object> Data { get; set; }

        public ModObject(ItemTemplate template) : this(template, new Dictionary<string, object>()) { }

        public ModObject(ItemTemplate template, Dictionary<string, object> data) : this(template, data, 1) { }

        public ModObject(ItemTemplate template, Dictionary<string, object> data, int count) : base(Vector2.Zero, Objects.STONE, count) {
            this.Template = template ?? throw new ArgumentNullException(nameof(template), $"{nameof(template)} is null.");
            this.Data = data ?? throw new ArgumentException($"{nameof(data)} is null. Use new ModObject(ItemTemplate) instead.", nameof(data));

            this.name = template.GetName(data);
            this.price = template.GetPrice(data);
            this.edibility = template.GetEdibility(data);
        }

        public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, bool drawStackNumber) {
            Texture2D texture = this.Template.GetTexture(this.Data);
            base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber);
        }
    }
}
