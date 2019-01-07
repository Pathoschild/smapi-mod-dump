using System.Collections.Generic;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace Spawn_Monsters
{
	/// <summary>
	/// A menu for selecting a monster to spawn
	/// </summary>
	class MonsterMenu : IClickableMenu
	{
		private ClickableTextureComponent leftArrow;
		private ClickableTextureComponent rightArrow;

		private List<List<ClickableMonsterComponent>> Pages { get; set; } = new List<List<ClickableMonsterComponent>>();
		private List<ClickableMonsterComponent> CurrentPage { get; set; } = new List<ClickableMonsterComponent>();
		private int CurrentPageI { get; set; }
		private int SlimeColor { get; set; }
		private int BatColor { get; set; }
		private int GhostColor { get; set; }
		private int CrabColor { get; set; }
		private int FlyColor { get; set; }
		private int GrubColor { get; set; }
		private int GolemColor { get; set; }

		public MonsterMenu()
			: base(0, 0, 0, 0, true) {
			this.width = 600;
			this.height = 600;
			this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
			Game1.playSound("bigSelect");

			leftArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width / 3, this.yPositionOnScreen + this.height, 44, 48), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false);
			rightArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width / 3 * 2, this.yPositionOnScreen + this.height, 44, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false);

			List<ClickableMonsterComponent> page0 = new List<ClickableMonsterComponent> {
				new ClickableMonsterComponent("Green Slime", xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2, 16, 24) {
					myID = 0,
					rightNeighborID = 1,
					downNeighborID = 2
				},

				new ClickableMonsterComponent("Bat", xPositionOnScreen + this.width / 2, yPositionOnScreen, this.width / 2, this.height / 2) {
					myID = 1,
					leftNeighborID = 0,
					downNeighborID = 3
				},

				new ClickableMonsterComponent("Bug", xPositionOnScreen, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, 16, 16) {
					myID = 2,
					rightNeighborID = 3,
					upNeighborID = 0
				},

				new ClickableMonsterComponent("Duggy", xPositionOnScreen + this.width / 2, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, 16, 24, 0, 9) {
					myID = 3,
					leftNeighborID = 2,
					upNeighborID = 1
				}
			};

			//PAGE1
			List<ClickableMonsterComponent> page1 = new List<ClickableMonsterComponent> {
				new ClickableMonsterComponent("Dust Spirit", xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2) {
					myID = 0,
					rightNeighborID = 1,
					downNeighborID = 2
				},

				new ClickableMonsterComponent("Fly", xPositionOnScreen + this.width / 2, yPositionOnScreen, this.width / 2, this.height / 2) {
					myID = 1,
					leftNeighborID = 0,
					downNeighborID = 3
				},

				new ClickableMonsterComponent("Ghost", xPositionOnScreen, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2) {
					myID = 2,
					rightNeighborID = 3,
					upNeighborID = 0
				},

				new ClickableMonsterComponent("Grub", xPositionOnScreen + this.width / 2, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, 16, 24) {
					myID = 3,
					leftNeighborID = 2,
					upNeighborID = 1
				}
			};

			//PAGE 2
			List<ClickableMonsterComponent> page2 = new List<ClickableMonsterComponent> {
				new ClickableMonsterComponent("Metal Head", xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2, 16, 16) {
					myID = 0,
					rightNeighborID = 1,
					downNeighborID = 2
				},

				new ClickableMonsterComponent("Mummy", xPositionOnScreen + this.width / 2, yPositionOnScreen, this.width / 2, this.height / 2, 16, 32) {
					myID = 1,
					leftNeighborID = 0,
					downNeighborID = 3
				},

				new ClickableMonsterComponent("Rock Crab", xPositionOnScreen, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2) {
					myID = 2,
					rightNeighborID = 3,
					upNeighborID = 0
				},

				new ClickableMonsterComponent("Stone Golem", xPositionOnScreen + this.width / 2, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2) {
					myID = 3,
					leftNeighborID = 2,
					upNeighborID = 1
				}
			};

			//PAGE 3
			List<ClickableMonsterComponent> page3 = new List<ClickableMonsterComponent> {
				new ClickableMonsterComponent("Serpent", xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2, 32, 32, 0, 9) {
					myID = 0,
					rightNeighborID = 1,
					downNeighborID = 2
				},

				new ClickableMonsterComponent("Shadow Brute", xPositionOnScreen + this.width / 2, yPositionOnScreen, this.width / 2, this.height / 2, 16, 32) {
					myID = 1,
					leftNeighborID = 0,
					downNeighborID = 3
				},

				new ClickableMonsterComponent("Shadow Shaman", xPositionOnScreen, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2) {
					myID = 2,
					rightNeighborID = 3,
					upNeighborID = 0
				},

				new ClickableMonsterComponent("Skeleton", xPositionOnScreen + this.width / 2, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, 16, 32) {
					myID = 3,
					leftNeighborID = 2,
					upNeighborID = 1
				}
			};

			//PAGE 4
			List<ClickableMonsterComponent> page4 = new List<ClickableMonsterComponent> {
				new ClickableMonsterComponent("Squid Kid", xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2, 16, 16) {
					myID = 0,
					rightNeighborID = 1,
					downNeighborID = 2
				}
			};

			Pages.Add(page0);
			Pages.Add(page1);
			Pages.Add(page2);
			Pages.Add(page3);
			Pages.Add(page4);

			CurrentPage = page0;
			CurrentPageI = 0;

			this.initializeUpperRightCloseButton();
			if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
				return;
			this.populateClickableComponentList();
			this.snapToDefaultClickableComponent();
		}

		public override void draw(SpriteBatch b) {
			b.Draw(Game1.fadeToBlackRect, destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds, color: Color.Black * 0.4f);

			Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, (string)null, false, false);

			Game1.drawDialogueBox(this.xPositionOnScreen - 300, this.yPositionOnScreen, 300, this.height / 2, false, true, (string)null, false, false);

			b.DrawString(Game1.dialogueFont, "Select a\nmonster\nto spawn", new Vector2(xPositionOnScreen - 240, yPositionOnScreen + 110), Color.Black);

			foreach (ClickableMonsterComponent c in CurrentPage) {
				c.Draw(b);
			}

			leftArrow.draw(b);
			rightArrow.draw(b);

			b.DrawString(Game1.dialogueFont, $"{CurrentPageI+1}/{Pages.Count}", new Vector2(this.xPositionOnScreen + (float)((this.width / 3) * 1.5), this.yPositionOnScreen + this.height), Color.White);

			this.drawMouse(b);
			base.draw(b);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			if (this.isWithinBounds(x, y)) {

				foreach (ClickableMonsterComponent monster in CurrentPage) {

					if (monster.containsPoint(x, y)) {
						Game1.activeClickableMenu = new MonsterPlaceMenu(monster.name);
					}
				}
				base.receiveLeftClick(x, y, true);

			} else if (leftArrow.containsPoint(x, y)) {

				CurrentPageI--;
				if (CurrentPageI < 0) {
					CurrentPage = Pages.ElementAt(Pages.Count - 1);
					CurrentPageI = Pages.Count - 1;
				} else {
					CurrentPage = Pages.ElementAt(CurrentPageI);
				}
				Game1.playSound("smallSelect");
			} else if (rightArrow.containsPoint(x, y)) {

				CurrentPageI++;
				if (CurrentPageI == Pages.Count) {
					CurrentPage = Pages.ElementAt(0);
					CurrentPageI = 0;
				} else {
					CurrentPage = Pages.ElementAt(CurrentPageI);
				}
				Game1.playSound("smallSelect");
			} else {
				Game1.exitActiveMenu();
			}
		}

		public override void performHoverAction(int x, int y) {
			base.performHoverAction(x, y);
			foreach (ClickableMonsterComponent monster in this.CurrentPage) {
				monster.PerformHoverAction(x, y);
			}
		}

		public override void snapToDefaultClickableComponent() {
			this.currentlySnappedComponent = this.getComponentWithID(0);
			this.snapCursorToCurrentSnappedComponent();
		}
	}
}
