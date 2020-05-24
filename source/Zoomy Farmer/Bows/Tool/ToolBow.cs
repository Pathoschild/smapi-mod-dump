using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace GekosBows {

	class ToolBow : Slingshot, ISaveElement, ICustomObject {

		public static Texture2D texture { get; set; }
		private bool isUsing = false;

		public object getReplacement() {
			throw new NotImplementedException();
		}

		public Dictionary<string, string> getAdditionalSaveData() {
			Dictionary<string, string> savedata = new Dictionary<string, string>();
			savedata.Add("name", Name);
			return savedata;
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
			build();
		}

		private void build() {
			//this.bowPrefix = bowMaterial;

			//// Attachments are an items inventory slot


			Name = ModEntry.i18n.Get("bow_name");
			description = ModEntry.i18n.Get("desc_bow");

			this.numAttachmentSlots.Value = 1;
			this.attachments.SetCount(numAttachmentSlots);
		}

		public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement) {
			return new ToolBow();
		}

		public ToolBow() : base() {
			build();
		}

		public ToolBow(CustomObjectData data) : this() {

		}

		public override Item getOne() {
			return new ToolBow();
		}

		public override bool canBeTrashed() {
			return false;
		}

		//protected override string loadDisplayName() {
		//	//return $"{this.bowPrefix} {Constants.BASE_NAME_BOW}";
		//	switch(this.upgradeLevel) {
		//		case 0:
		//			return $"{Constants.MATERIAL_STONE} {Constants.BASE_NAME_BOW}";

		//		case 1:
		//			return $"{Constants.MATERIAL_STONE} {Constants.BASE_NAME_BOW}";

		//		case 2:
		//			return $"{Constants.MATERIAL_STONE} {Constants.BASE_NAME_BOW}";

		//		case 3:
		//			return $"{Constants.MATERIAL_STONE} {Constants.BASE_NAME_BOW}";

		//		default:
		//			return Constants.BASE_NAME_BOW;
		//	}
		//}

		public override string DisplayName { get => ModEntry.i18n.Get("name_bow"); set => base.DisplayName = ModEntry.i18n.Get("name_bow"); }

		protected override string loadDescription() {
			return ModEntry.i18n.Get("desc_bow");
		}

		public override int attachmentSlots() {
			return numAttachmentSlots.Value;
		}

		// Called when used
		public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who) {
			base.DoFunction(location, x, y, power, who);
		}

		// Add / remove attachment
		public override SObject attach(SObject o) {
			//if (o != null && o.Category == -21) {
			//	SObject @object = this.attachments[0];
			//	if (@object != null && @object.canStackWith((ISalable)o)) {
			//		@object.Stack = o.addToStack((Item)@object);
			//		if (@object.Stack <= 0)
			//			@object = null;
			//	}
			//	this.attachments[0] = o;
			//	Game1.playSound("button1");
			//	return @object;
			//}

			//if (o == null) {
			//	if (this.attachments[0] != null) {
			//		SObject attachment = this.attachments[0];
			//		this.attachments[0] = null;
			//		Game1.playSound("dwop");
			//		return attachment;
			//	}

			//}
			//return null;

			SObject oldAttach = null;

			// We have an attachment, and there's a new one trying to be added
			if (o != null && o.Category == -21 && this.attachments[0] != null) {
				oldAttach = new SObject(Vector2.Zero, attachments[0].ParentSheetIndex, attachments[0].Stack);
			}

			// Removing attachment
			if (o == null) {
				if (this.attachments[0] != null) {
					oldAttach = new SObject(Vector2.Zero, attachments[0].ParentSheetIndex, attachments[0].Stack);
				}

				Game1.playSound("dwop");
				return oldAttach;
			}

			// No old attachment and adding a new attachment
			if(canThisBeAttached(o) && o.category == -21) {
				this.attachments[0] = o;

				Game1.playSound("button1");
				return oldAttach;
			}

			return null;

		}

		public override bool canThisBeAttached(SObject o) {
			return false;
		}

		public override void drawInMenu(SpriteBatch b, Vector2 loc, float scale, float transparency, float depth, StackDrawType stackDrawType, Color color, bool shadow) {
			b.Draw(texture, loc + new Vector2(32, 29), null, Color.White * transparency, 0, new Vector2(8, 8), scale * 4, SpriteEffects.None, depth);
			if (stackDrawType == StackDrawType.Hide || this.attachments == null || this.attachments[0] == null)
				return;

			Utility.drawTinyDigits(this.attachments[0].Stack, b, loc + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(this.attachments[0].Stack, 3f * scale)) + 3f * scale, (float)(64.0 - 18.0 * (double)scale + 2.0)), 3f * scale, 1f, Color.White);
		}

		public override void leftClick(Farmer who) {
			base.leftClick(who);
		}

		public override bool onRelease(GameLocation location, int x, int y, Farmer who) {
			isUsing = false;
			return base.onRelease(location, x, y, who);
		}

		public override bool beginUsing(GameLocation location, int x, int y, Farmer who) {
			isUsing = true;
			return base.beginUsing(location, x, y, who);
		}
	}
}
