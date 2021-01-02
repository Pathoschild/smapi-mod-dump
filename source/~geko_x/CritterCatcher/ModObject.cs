/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/geko_x/stardew-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using StardewValley.Objects;
using SObject = StardewValley.Object;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using PyTK.CustomElementHandler;

namespace CritterCatcher {
	internal class ModObjectData {
		public string ObjectID = "ModObjectID";
		public string Name = "ModObjectName";
		public string Description = "ModObjectDescription";
		public string ObjectType = Constants.TYPE_CRAFTING;
		public int ObjectCategory = Constants.CATEGORY_JUNK;
		public int Price = 1;
		public string Crafting = null;
		public int Edibility = -300;

		public ModObjectData() {
		}

		public ModObjectData(string ObjectID, string Name, string Description) {
			this.ObjectID = ObjectID;
			this.Name = Name;
			this.Description = Description;
		}

		public CustomObjectData CreateObjectData(Texture2D texture, Type type, int index = 0) {
			return new CustomObjectData(
				this.ObjectID,
				GetFormattedObjectData(),
				texture,
				Color.White,
				index,
				false,
				type,
				Crafting != null ? new CraftingData(ObjectID, Crafting) : null);
		}

		public string GetFormattedObjectData() {
			return $"{ObjectID}/{Price}/{Edibility}/{ObjectType} {ObjectCategory}/{Name}/{Description}";
		}
	}

	abstract class ModObject : SObject, ICustomObject, ISaveElement, IDrawFromCustomObjectData {

		public Texture2D texture;
		public ModObjectData modObjectData;
		public int itemId;

		public abstract CustomObjectData data { get; }

		public ModObject() {

		}

		public ModObject(CustomObjectData data) : base(data.sdvId, 0) {

		}

		public ModObject(CustomObjectData data, int parentSheetIndex) : base(data.sdvId, 0) {

		}

		public Dictionary<string, string> getAdditionalSaveData() {
			return new Dictionary<string, string>() {
				{ "name", this.name }
			};
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
			name = additionalSaveData["name"];
		}

		public override bool canBeDropped() {
			return true;
		}

		public override bool canBePlacedHere(GameLocation l, Vector2 tile) {
			return false;
		}

		public override bool isPlaceable() {
			return false;
		}

		public override bool canBeShipped() {
			return true;
		}

		public abstract void build();

		public abstract object getReplacement();

		public abstract ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement);
	}
}