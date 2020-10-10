/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SObject = StardewValley.Object;

namespace GekosBows {

	class OldArrow : SObject, ICustomObject {

		public static Texture2D texture;
		private string description;

		private int damage = 1;

		public OldArrow() {

		}

		public OldArrow(CustomObjectData data)
			: this(data.id.Split('.')[2]) {
		}

		public OldArrow(string id) {
			build();
		}

		public OldArrow(string name, string description) {

			this.name = name;
			this.displayName = name;
			this.description = description;

			build();
		}

		public override string Name { get => name; }

		public override string DisplayName { get => name; set => name = value; }

		// Set additional class data here
		private void build() {
			Name = ModEntry.i18n.Get("name_arrow");
			description = ModEntry.i18n.Get("desc_arrow");
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
			build();
		}

		public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement) {
			return new OldArrow();
		}

		public object getReplacement() {
			return new SObject(685, 1);
		}

		public Dictionary<string, string> getAdditionalSaveData() {
			Dictionary<string, string> data = new Dictionary<string, string>();

			return data;
		}

		public override bool canBeDropped() {
			return true;
		}

		public override bool canBeGivenAsGift() {
			return false;
		}

		public override bool canBeTrashed() {
			return true;
		}

		public override int maximumStackSize() {
			return 999;
		}

		public override Item getOne() {
			return new OldArrow();
		}

		public override string getDescription() {
			string text = this.description;
			SpriteFont smallFont = Game1.smallFont;
			int width = Game1.tileSize * 4 + Game1.tileSize / 4;
			return Game1.parseText(text, smallFont, width);
		}

		public override bool canStackWith(ISalable other) {
			if (other is OldArrow)
				return true;

			return false;
		}

		public override void drawInMenu(SpriteBatch b, Vector2 location, float scaleSize, float transparency, float depth, StackDrawType drawStackNumber, Color color, bool drawShadow) {
			if (texture == null)
				return;

			base.drawInMenu(b, location, scaleSize, transparency, depth, drawStackNumber, color, drawShadow);
		}

		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer who) {
			if (texture == null)
				return;
			Rectangle sourceRectangle = new Rectangle(0, 0, 16, 16);
			spriteBatch.Draw(texture, objectPosition, new Rectangle?(sourceRectangle), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, Math.Max(0.0f, (float)(who.getStandingY() + 2) / 10000f));
		}
	}
}
