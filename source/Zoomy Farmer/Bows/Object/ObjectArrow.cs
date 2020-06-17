using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewValley;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley.Objects;
using PyTK.Extensions;
using SObject = StardewValley.Object;

namespace GekosBows {

	class ObjectArrow : ModObject {

		public static CustomObjectData customObjectData;

		public ObjectArrow() {
			build();
		}

		public ObjectArrow(CustomObjectData data) : base(data, 0) {
			build();
		}

		public override CustomObjectData data => customObjectData;

		public void Init() {

			if (texture == null)
				texture = ModEntry.INSTANCE.Helper.Content.Load<Texture2D>("assets/arrow.png", StardewModdingAPI.ContentSource.ModFolder);

			modObjectData = new ModObjectData("Arrow", ModEntry.i18n.Get("name_arrow"), ModEntry.i18n.Get("desc_arrow"));
			modObjectData.Crafting = "390 1 388 1";
			modObjectData.ObjectCategory = Constants.CATEGORY_BUILDING_MATERIALS;
			customObjectData = modObjectData.CreateObjectData(texture, typeof(ObjectArrow));
			itemID = customObjectData.sdvId;

		}

		public override void build() {
			this.CanBeSetDown = false;
		}

		public override object getReplacement() {
			return new ObjectArrow();
		}

		public override ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement) {
			return new ObjectArrow(customObjectData);
		}

		public override Item getOne() {
			return new ObjectArrow(customObjectData);
		}

		public override int salePrice() {
			return 10;
		}
	}
}
