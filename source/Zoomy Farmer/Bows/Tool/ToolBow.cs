using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Netcode;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using SObject = StardewValley.Object;

namespace GekosBows {

	class ToolBow : Slingshot, ISaveElement, ICustomObject, IDrawFromCustomObjectData {

		public Texture2D texture;
		private string textureName;
		public CustomObjectData customObjectData;
		public ModObjectData modObjectData;

		private static Dictionary<Farmer, bool> isFarmerUsing = new Dictionary<Farmer, bool>();

		private int[] damageValues = { 1, 5, 10, 20 };
		private int[] drawTimes = { 60, 60, 60, 60 };

		[XmlIgnore]
		public readonly NetPoint AimingAt = new NetPoint().Interpolated(true, true);
		[XmlIgnore]
		public readonly NetInt UseTime = new NetInt();
		private int usingTime = 0;

		CustomObjectData IDrawFromCustomObjectData.data { get => customObjectData; }

		[XmlIgnore]
		private readonly NetEvent0 finishEvent = new NetEvent0(false);

		public ToolBow() : this(0, "allbow") {

		}

		public ToolBow(CustomObjectData data) : this(data.tileIndex, "allbow")  {

		}

		public ToolBow(int upgradeLevel, string textureName) {
			build(upgradeLevel, textureName);
		}

		protected override void initNetFields() {
			base.initNetFields();
			this.NetFields.AddFields((INetSerializable)this.finishEvent, (INetSerializable)this.aimPos);
			this.finishEvent.onEvent += new NetEvent0.Event(this.doFinish);
		}

		public object getReplacement() {
			throw new NotImplementedException();
		}

		public Dictionary<string, string> getAdditionalSaveData() {
			return new Dictionary<string, string>() {
				{ "name", this.Name }
			};
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement) {
			this.Name = additionalSaveData["name"];
		}

		protected void build(int upgradeLevel, string textureName) {
			this.numAttachmentSlots.Value = 1;
			this.attachments.SetCount(numAttachmentSlots);
			this.UpgradeLevel = upgradeLevel;
			this.textureName = textureName;
			loadTexture();
		}

		public void Init(string itemName) {

			loadTexture();

			modObjectData = new ModObjectData(itemName, getNameForUpgradeLevel(this.UpgradeLevel), getDescriptionForUpgradeLevel(this.UpgradeLevel));
			modObjectData.ObjectCategory = Constants.CATEGORY_WEAPON;
			customObjectData = modObjectData.CreateObjectData(texture, typeof(ToolBow), this.UpgradeLevel);
		}

		private void loadTexture() {
			if (texture == null)
				texture = ModEntry.INSTANCE.Helper.Content.Load<Texture2D>($"assets/{this.textureName}.png", ContentSource.ModFolder);
		}

		public ICustomObject recreate(Dictionary<string, string> additionalSaveData, object replacement) {
			var bow = new ToolBow();
			return bow;
		}

		public override Item getOne() {
			var bow = new ToolBow();
			bow.UpgradeLevel = this.UpgradeLevel;

			return (Item)bow;
		}

		public override bool canBeTrashed() {
			return false;
		}

		public override bool canBeDropped() {
			return true;
		}

		public override bool canBePlacedHere(GameLocation l, Vector2 tile) {
			return false;
		}

		protected override string loadDisplayName() {
			return getNameForUpgradeLevel((int)(NetFieldBase<int, NetInt>)this.upgradeLevel);
		}

		public override string DisplayName {
			get => (getNameForUpgradeLevel((int)(NetFieldBase<int, NetInt>)this.upgradeLevel));
		}

		private static string getNameForUpgradeLevel(int level) {
			switch (level) {
				case 0:
					return $"{ModEntry.i18n.Get("name_bow_0")}";
				case 1:
					return $"{ModEntry.i18n.Get("name_bow_1")}";
				case 2:
					return $"{ModEntry.i18n.Get("name_bow_2")}";
				case 3:
					return $"{ModEntry.i18n.Get("name_bow_3")}";
				default:
					return $"{ModEntry.i18n.Get("name_bow")}";
			}
		}

		private static string getDescriptionForUpgradeLevel(int level) {
			switch (level) {
				case 0:
					return $"{ModEntry.i18n.Get("desc_bow_0")}";
				case 1:
					return $"{ModEntry.i18n.Get("desc_bow_1")}";
				case 2:
					return $"{ModEntry.i18n.Get("desc_bow_2")}";
				case 3:
					return $"{ModEntry.i18n.Get("desc_bow_3")}";
				default:
					return $"{ModEntry.i18n.Get("desc_bow")}";
			}
		}

		private static int getUpgradeLevelForItemID(string itemID)  {
			switch(itemID) {
				case "Training Bow":
					return 0;
				case "Shortbow":
					return 1;
				case "Compound Bow":
					return 2;
				case "Galaxy Bow":
					return 3;
				default:
					return 0;
			}
		}

		private int getDamage() {
			int level = (int)(NetFieldBase<int, NetInt>)this.upgradeLevel;
			level = (level < 0) ? 0 : (level >= damageValues.Length) ? damageValues.Length - 1 : level;
			return this.damageValues[level];
		}

		private int getDrawTime() {
			int level = (int)(NetFieldBase<int, NetInt>)this.upgradeLevel;
			level = (level < 0) ? 0 : (level >= drawTimes.Length) ? drawTimes.Length - 1 : level;
			return this.drawTimes[level];
		}

		new public string BaseName {
			get {
				return "Bow";
			}

			set { }
		}

		public override string Name {
			get {
				switch ((int)(NetFieldBase<int, NetInt>)this.upgradeLevel) {
					case 0:
						return "Training Bow";
					case 1:
						return "Shortbow";
					case 2:
						return "Compound Bow";
					case 3:
						return "Galaxy Bow";
					default:
						return this.BaseName;
				}
			}
			set {
				this.BaseName = value;
			}
		}

		protected override string loadDescription() {
			return getDescriptionForUpgradeLevel((int)(NetFieldBase<int, NetInt>)this.upgradeLevel);
		}

		public override int salePrice() {
			switch ((int)(NetFieldBase<int, NetInt>)this.upgradeLevel) {
				case 0:
					return 500;
				case 1:
					return 2000;
				case 2:
					return 5000;
				case 3:
					return 15000;
				default:
					return 500;
			}
		}

		public override int attachmentSlots() {
			return numAttachmentSlots.Value;
		}

		// Add / remove attachment
		public override SObject attach(SObject o) {

			SObject oldAttach = null;

			// We have an attachment, and there's a new one trying to be added
			if (o != null && o is ObjectArrow && this.attachments[0] != null) {
				oldAttach = new SObject(Vector2.Zero, attachments[0].ParentSheetIndex, attachments[0].Stack);
			}

			// Removing attachment
			if (o == null) {
				if (this.attachments[0] != null) {
					oldAttach = new SObject(Vector2.Zero, attachments[0].ParentSheetIndex, attachments[0].Stack);
					attachments[0] = null;
				}

				Game1.playSound("dwop");
				return oldAttach;
			}

			// No old attachment and adding a new attachment
			if(canThisBeAttached(o) && o is ObjectArrow) {
				this.attachments[0] = o;

				Game1.playSound("button1");
				return oldAttach;
			}

			return null;

		}

		public override bool canThisBeAttached(SObject o) {
			return o is ObjectArrow;
		}

		public override void draw(SpriteBatch b) {
			if (!this.lastUser.usingSlingshot || !this.lastUser.IsLocalPlayer)
				return;

			Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(
				new Point(this.lastUser.getStandingX(), this.lastUser.getStandingY() + 32),
				new Vector2((float)this.aimPos.X, (float)this.aimPos.Y), 256f);

			//if ((double)Math.Abs(velocityTowardPoint.X) < 1.0) {
			//	int mouseDragAmount = this.mouseDragAmount;
			//}

			//double dist = Math.Sqrt((double)velocityTowardPoint.X * (double)velocityTowardPoint.X + (double)velocityTowardPoint.Y * (double)velocityTowardPoint.Y) - 181.0;
			//double velXNormalized = (double)velocityTowardPoint.X / 256.0;
			//double velYNormalized = (double)velocityTowardPoint.Y / 256.0;

			//int x = (int)((double)velocityTowardPoint.X - dist * velXNormalized);
			//int y = (int)((double)velocityTowardPoint.Y - dist * velYNormalized);

			//b.Draw(Game1.mouseCursors,
			//	Game1.GlobalToLocal(Game1.viewport,new Vector2((float)(this.lastUser.getStandingX() - x), (float)(this.lastUser.getStandingY() - 64 - 8 - y))),
			//	new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43, -1, -1)),
			//	Color.White, 0.0f, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0.999999f);

			Vector2 mousePos = new Vector2(Game1.getMouseX(), Game1.getMouseY());
			b.Draw(Game1.mouseCursors, mousePos, new Rectangle?(Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 43, -1, -1)), Color.White, 0, new Vector2(32f, 32f), 1f, SpriteEffects.None, 0.999999f);
		}

		public override void drawInMenu(SpriteBatch b, Vector2 loc, float scale, float transparency, float depth, StackDrawType stackDrawType, Color color, bool shadow) {
			Rectangle srcRec = new Rectangle(16 * this.UpgradeLevel, 0, 16, 16);
			b.Draw(texture, loc + new Vector2(32, 29), srcRec, Color.White * transparency, 0, new Vector2(8, 8), scale * 4, SpriteEffects.None, depth);
			if (stackDrawType == StackDrawType.Hide || this.attachments == null || this.attachments[0] == null)
				return;

			Utility.drawTinyDigits(this.attachments[0].Stack, b, loc + new Vector2((float)(64 - Utility.getWidthOfTinyDigitString(this.attachments[0].Stack, 3f * scale)) + 3f * scale, (float)(64.0 - 18.0 * (double)scale + 2.0)), 3f * scale, 1f, Color.White);
		}

		public override void DoFunction(GameLocation location, int x, int y, int power, Farmer who) {
			this.IndexOfMenuItemView = this.InitialParentTileIndex;
			if (this.attachments[0] != null) {
				this.updateAimPos();
				int x1 = this.aimPos.X;
				int y1 = this.aimPos.Y;
				int num1 = Math.Min(20, (int)Vector2.Distance(new Vector2((float)who.getStandingX(), (float)(who.getStandingY() - 64)), new Vector2((float)x1, (float)y1)) / 20);
				Vector2 velocityTowardPoint = Utility.getVelocityTowardPoint(new Point(who.getStandingX(), who.getStandingY() + 64), new Vector2((float)x1, (float)(y1 + 64)), (float)(15 + Game1.random.Next(4, 6)) * (1f + who.weaponSpeedModifier));
				if (num1 > 4) {
					StardewValley.Object one = (StardewValley.Object)this.attachments[0].getOne();
					--this.attachments[0].Stack;
					if (this.attachments[0].Stack <= 0)
						this.attachments[0] = (StardewValley.Object)null;
					int num2 = 1;
					BasicProjectile.onCollisionBehavior collisionBehavior = (BasicProjectile.onCollisionBehavior)null;
					string collisionSound = "hammer";
					float num3 = 1f;
					if (this.InitialParentTileIndex == 33)
						num3 = 2f;
					else if (this.InitialParentTileIndex == 34)
						num3 = 4f;
					switch (one.ParentSheetIndex) {
						case 378:
							num2 = 10;
							++one.ParentSheetIndex;
							break;
						case 380:
							num2 = 20;
							++one.ParentSheetIndex;
							break;
						case 382:
							num2 = 15;
							++one.ParentSheetIndex;
							break;
						case 384:
							num2 = 30;
							++one.ParentSheetIndex;
							break;
						case 386:
							num2 = 50;
							++one.ParentSheetIndex;
							break;
						case 388:
							num2 = 2;
							++one.ParentSheetIndex;
							break;
						case 390:
							num2 = 5;
							++one.ParentSheetIndex;
							break;
						case 441:
							num2 = 20;
							collisionBehavior = new BasicProjectile.onCollisionBehavior(BasicProjectile.explodeOnImpact);
							collisionSound = "explosion";
							break;
					}
					if (one.Category == -5)
						collisionSound = "slimedead";
					NetCollection<Projectile> projectiles = location.projectiles;
					BasicProjectile basicProjectile = new BasicProjectile((int)((double)num3 * (double)(num2 + Game1.random.Next(-(num2 / 2), num2 + 2)) * (1.0 + (double)who.attackIncreaseModifier)), one.ParentSheetIndex, 0, 0, 0, -velocityTowardPoint.X, -velocityTowardPoint.Y, new Vector2((float)(who.getStandingX() - 16), (float)(who.getStandingY() - 64 - 8)), collisionSound, "", false, true, location, (Character)who, true, collisionBehavior);
					basicProjectile.IgnoreLocationCollision = Game1.currentLocation.currentEvent != null;
					projectiles.Add((Projectile)basicProjectile);
				}
			}
			
			else
				Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Slingshot.cs.14254"));

			//if(who.IsLocalPlayer) {

			//}

			this.finish();
		}

		private void doFinish() {
			//this.lastUser.usingSlingshot = false;
			this.lastUser.canReleaseTool = true;
			this.lastUser.UsingTool = false;
			this.lastUser.canMove = true;
			this.lastUser.Halt();
		}

		public override bool onRelease(GameLocation location, int x, int y, Farmer who) {
			this.DoFunction(location, x, y, 1, who);
			return true;
		}

		public override bool beginUsing(GameLocation location, int x, int y, Farmer who) {
			//who.usingSlingshot = true;
			who.canReleaseTool = false;
			this.mouseDragAmount = 0;
			int facingSprite = who.FacingDirection == 3 || who.FacingDirection == 1 ? 1 : (who.FacingDirection == 0 ? 2 : 0);
			who.FarmerSprite.setCurrentFrame(42 + facingSprite);
			if (!who.IsLocalPlayer)
				return true;
			double posX = (double)(Game1.getOldMouseX() + Game1.viewport.X - who.getStandingX());
			double posY = (double)(Game1.getOldMouseY() + Game1.viewport.Y - who.getStandingY());
			double num4;
			double num5;
			if (Math.Abs(posX) > Math.Abs(posY)) {
				num4 = posX / Math.Abs(posX);
				num5 = 0.5;
			} else {
				num5 = posY / Math.Abs(posY);
				num4 = 0.0;
			}
			double num6 = num4 * 16.0;
			double num7 = num5 * 16.0;
			Game1.lastMousePositionBeforeFade = Game1.getMousePosition();
			this.lastClickX = Game1.getOldMouseX() + Game1.viewport.X;
			this.lastClickY = Game1.getOldMouseY() + Game1.viewport.Y;
			this.updateAimPos();

			this.usingTime = 0;

			return true;
		}

		private void updateAimPos() {
			if (this.lastUser == null || !this.lastUser.IsLocalPlayer)
				return;

			Point point = Game1.getMousePosition();
			//if (Game1.options.gamepadControls) {
			//	Vector2 standingPosition = this.lastUser.getStandingPosition();
			//	GamePadThumbSticks thumbSticks = Game1.oldPadState.ThumbSticks;
			//	double x = (double)thumbSticks.Left.X;
			//	thumbSticks = Game1.oldPadState.ThumbSticks;
			//	double y = -(double)thumbSticks.Left.Y;
			//	Vector2 vector2 = new Vector2((float)x, (float)y) * 64f * 4f;
			//	point = Utility.Vector2ToPoint(standingPosition + vector2);
			//	point.X -= Game1.viewport.X;
			//	point.Y -= Game1.viewport.Y;
			//}

			//this.aimPos.X = point.X + Game1.viewport.X;
			//this.aimPos.Y = point.Y + Game1.viewport.Y;
			this.aimPos.X = point.X;
			this.aimPos.Y = point.Y;
		}

		public override void tickUpdate(GameTime time, Farmer who) {
			this.lastUser = who;
			this.finishEvent.Poll();
			if (!who.usingSlingshot)
				return;
			if (who.IsLocalPlayer) {

				this.usingTime++;

				this.updateAimPos();
				int x = this.aimPos.X;
				int y = this.aimPos.Y;
				Game1.debugOutput = "playerPos: " + who.getStandingPosition().ToString() + ", mousePos: " + (object)x + ", " + (object)y;
				++this.mouseDragAmount;
				who.faceGeneralDirection(new Vector2((float)x, (float)y), 0, false);
				this.lastClickX = x;
				this.lastClickY = y;
				Game1.mouseCursor = -1;
			}
			int num = who.FacingDirection == 3 || who.FacingDirection == 1 ? 1 : (who.FacingDirection == 0 ? 2 : 0);
			who.FarmerSprite.setCurrentFrame(42 + num);
		}

	}
}
