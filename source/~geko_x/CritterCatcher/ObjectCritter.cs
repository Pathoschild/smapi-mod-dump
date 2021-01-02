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

namespace CritterCatcher {

	class ObjectCritter : ModObject {

		public int baseFrame;

		public static CustomObjectData customObjectData;

		public ObjectCritter() {
			build();
		}

		public ObjectCritter(CustomObjectData data) : base(data, 0) {
			build();
		}

		public override CustomObjectData data => customObjectData;

		public void Init() {

			if (texture == null)
				texture = ModEntry.INSTANCE.Helper.Content.Load<Texture2D>("assets/critterBag.png", StardewModdingAPI.ContentSource.ModFolder);

			modObjectData = new ModObjectData("Critter Bag", ModEntry.i18n.Get("name_critter"), ModEntry.i18n.Get("desc_critter"));
			modObjectData.ObjectType = Constants.TYPE_BASIC;
			modObjectData.ObjectCategory = Constants.CATEGORY_ARTISAN;
			customObjectData = modObjectData.CreateObjectData(texture, typeof(ObjectCritter));
			itemId = customObjectData.sdvId;

		}

		public override void build() {
			this.CanBeSetDown = false;
		}

		public override object getReplacement() {
			return new ObjectCritter();
		}

		public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement) {
			return new ObjectCritter(customObjectData);
		}

		public override Item getOne() {
			return new ObjectCritter(customObjectData);
		}

		public override int salePrice() {
			return 10;
		}

		//public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow) {
		//	base.drawInMenu(spriteBatch, location, scaleSize, transparency, layerDepth, drawStackNumber, color, drawShadow);
		//}
	}
}
