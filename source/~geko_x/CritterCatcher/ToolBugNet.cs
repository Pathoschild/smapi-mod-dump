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
using StardewValley.Tools;
using StardewValley;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using PyTK.CustomElementHandler;
using Microsoft.Xna.Framework;
using StardewValley.BellsAndWhistles;

namespace CritterCatcher {
	class ToolBugNet : Tool, ISaveElement, ICustomObject {

		public Texture2D texture;
		private string textureName;

		public CustomObjectData customObjectData;
		public ModObjectData modObjectData;

		public int itemId => customObjectData.sdvId;

		public ToolBugNet(CustomObjectData data) : this("bugnet2") {

		}

		public ToolBugNet(string textureName) {
			this.textureName = textureName;
			loadTexture();
		}

		public ToolBugNet() : base("Bug Net", -1, 105, 131, false) {
			this.textureName = "bugnet2";
		}

		public void Init(string itemName) {
			loadTexture();

			modObjectData = new ModObjectData(itemName, loadDisplayName(), loadDescription());
			modObjectData.ObjectType = Game1.content.LoadString("Strings\\StringsFromCSFiles:Tool.cs.14307");
			modObjectData.ObjectCategory = Constants.CATEGORY_WEAPON;
			modObjectData.Price = 100;
			customObjectData = modObjectData.CreateObjectData(texture, typeof(ToolBugNet));

		}

		public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who) {
			base.DoFunction(location, x, y, power, who);

			Utility.clampToTile(new Vector2(x, y));
			int tileX = x / 64;
			int tileY = y / 64;
			Vector2 tile = new Vector2(tileX, tileY);

			Critter c = ModEntry.INSTANCE.AttemptCatchCritter(tile);
			if(c == null) {
				return;
			}

			who.addItemByMenuIfNecessary(ModEntry.INSTANCE.critterObj.getOne());
		}

		protected override string loadDescription() {
			return ModEntry.i18n.Get("desc_bugnet");
		}

		protected override string loadDisplayName() {
			return ModEntry.i18n.Get("name_bugnet");
		}

		private void loadTexture() {
			if (texture == null)
				texture = ModEntry.INSTANCE.Helper.Content.Load<Texture2D>($"assets/{this.textureName}.png", ContentSource.ModFolder);
		}

		public object getReplacement() {
			return new ToolBugNet();
		}

		public Dictionary<string, string> getAdditionalSaveData() {
			return new Dictionary<string, string>();
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
			
		}

		public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement) {
			return new ObjectCritter(customObjectData);
		}

		public override bool canBeTrashed() {
			return false;
		}

		public override bool canBePlacedHere(GameLocation l, Vector2 tile) {
			return false;
		}

		public override Item getOne() {
			var item = new ToolBugNet();
			return item;
		}
	}
}
