/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Berisan/SpawnMonsters
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;

namespace Spawn_Monsters
{
    class MonsterSelectionTab : IClickableMenu
	{
		private ClickableTextureComponent leftArrow;
		private ClickableTextureComponent rightArrow;

		private List<List<ClickableComponent>> Pages { get; set; } = new List<List<ClickableComponent>>();
		private int currentPage = 0;

		public MonsterSelectionTab() : base(0, 0, 0, 0, true) {
			this.width = 600;
			this.height = 600;
			this.xPositionOnScreen = Game1.viewport.Width / 2 - this.width / 2;
			this.yPositionOnScreen = Game1.viewport.Height / 2 - this.height / 2;
			Random r = new Random();
			leftArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width / 3, this.yPositionOnScreen + this.height, 44, 48), Game1.mouseCursors, new Rectangle(352, 495, 12, 11), 4f, false);
			rightArrow = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width / 3 * 2, this.yPositionOnScreen + this.height, 44, 48), Game1.mouseCursors, new Rectangle(365, 495, 12, 11), 4f, false);

			List<ClickableComponent> page0 = new List<ClickableComponent> {
				new MultiClickableMonsterComponent(new string[]{ "Green Slime", "Green Slime", "Green Slime", "Green Slime", "Green Slime", "Green Slime", "Green Slime" }, new Color[]{ Color.Green, Color.DarkTurquoise, Color.Red, Color.Purple, Color.Yellow, Color.Black, Color.DarkOrange }, xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2, new object[]{ 0, 40, 80, 121, new Color(255, 255, 50), new Color(40 + r.Next(10), 40 + r.Next(10), 40 + r.Next(10)), 77377 } , 16, 24) {
					myID = 0,
					rightNeighborID = 1,
					downNeighborID = 2,
				},

				new MultiClickableMonsterComponent(new string[]{ "Bat", "Frost Bat", "Lava Bat", "Iridium Bat", "Haunted Skull", "Iridium Bat"}, new Color[]{ Color.Brown, Color.Blue, Color.DarkRed, Color.Purple, Color.DarkGray, Color.BlueViolet}, xPositionOnScreen + this.width / 2, yPositionOnScreen, this.width / 2, this.height / 2, new object[]{ null, 40, 80, 171, 77377, -666 }) {
					myID = 1,
					leftNeighborID = 0,
					downNeighborID = 3
				},

				new MultiClickableMonsterComponent(new string[]{ "Bug", "Armored Bug" }, new Color[]{ Color.DarkTurquoise, Color.Salmon}, xPositionOnScreen, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, new object[]{ 0, 121 }, 16, 16) {
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
			List<ClickableComponent> page1 = new List<ClickableComponent> {
				new ClickableMonsterComponent("Dust Spirit", xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2) {
					myID = 0,
					rightNeighborID = 1,
					downNeighborID = 2
				},

				new MultiClickableMonsterComponent(new string[]{"Fly", "Fly" }, new Color[]{ Color.White, Color.Lime }, xPositionOnScreen + this.width / 2, yPositionOnScreen, this.width / 2, this.height / 2, new object[]{ null, true }) {
					myID = 1,
					leftNeighborID = 0,
					downNeighborID = 3
				},

				new MultiClickableMonsterComponent(new string[]{"Ghost", "Carbon Ghost" }, new Color[]{ Color.Beige, Color.Gray }, xPositionOnScreen, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, new object[]{ null,  "Carbon Ghost" }) {
					myID = 2,
					rightNeighborID = 3,
					upNeighborID = 0
				},

				new MultiClickableMonsterComponent(new string[]{"Grub", "Grub" }, new Color[]{ Color.White, Color.Lime }, xPositionOnScreen + this.width / 2, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, new object[]{ null, true }) {
					myID = 3,
					leftNeighborID = 2,
					upNeighborID = 1
				}
			};

			//PAGE 2
			List<ClickableComponent> page2 = new List<ClickableComponent> {
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

				new MultiClickableMonsterComponent(new string[]{ "Rock Crab", "Lava Crab", "Iridium Crab"}, new Color[]{ Color.Gray, Color.DarkRed, Color.Violet }, xPositionOnScreen, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, new object[]{ null, null, "Iridium Crab" }) {
					myID = 2,
					rightNeighborID = 3,
					upNeighborID = 0
				},

				new MultiClickableMonsterComponent(new string[]{"Stone Golem", "Wilderness Golem" }, new Color[]{ Color.Gray, Color.Green }, xPositionOnScreen + this.width / 2, yPositionOnScreen + this.height / 2, this.width / 2, this.height / 2, new object[]{ null, 5 }) {
					myID = 3,
					leftNeighborID = 2,
					upNeighborID = 1
				}
			};

			//PAGE 3
			List<ClickableComponent> page3 = new List<ClickableComponent> {
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
            List<ClickableComponent> page4 = new List<ClickableComponent> {
                new ClickableMonsterComponent("Squid Kid", xPositionOnScreen, yPositionOnScreen, this.width / 2, this.height / 2, 16, 16) {
                    myID = 0,
                    rightNeighborID = 1,
                    downNeighborID = 2
                },

                new ClickableMonsterComponent("Pepper Rex", xPositionOnScreen, yPositionOnScreen, this.width/2, this.height/2){
                    myID = 1,
                    leftNeighborID = 0,
                    downNeighborID = 3
                }
			};

			Pages.Add(page0);
			Pages.Add(page1);
			Pages.Add(page2);
			Pages.Add(page3);
			Pages.Add(page4);

			this.initializeUpperRightCloseButton();
			if (!Game1.options.snappyMenus || !Game1.options.gamepadControls)
				return;
			this.populateClickableComponentList();
			this.snapToDefaultClickableComponent();
		}


		public override void draw(SpriteBatch b) {
			b.Draw(Game1.fadeToBlackRect, destinationRectangle: Game1.graphics.GraphicsDevice.Viewport.Bounds, color: Color.Black * 0.4f);

			Game1.drawDialogueBox(base.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true, (string)null, false, false);

			Game1.drawDialogueBox(this.xPositionOnScreen - 300, this.yPositionOnScreen, 300, this.height / 2, false, true, (string)null, false, false);

			b.DrawString(Game1.dialogueFont, "Select a\nmonster\nto spawn", new Vector2(xPositionOnScreen - 240, yPositionOnScreen + 110), Color.Black);

			foreach (ClickableComponent c in Pages[currentPage]) {
				if (c.GetType() == typeof(MultiClickableMonsterComponent)) {
					MultiClickableMonsterComponent m = (MultiClickableMonsterComponent)c;
					m.Draw(b);
				} else {
					ClickableMonsterComponent m = (ClickableMonsterComponent)c;
					m.Draw(b);
				}

			}

			leftArrow.draw(b);
			rightArrow.draw(b);

			b.DrawString(Game1.dialogueFont, $"{currentPage + 1}/{Pages.Count}", new Vector2(this.xPositionOnScreen + (float)((this.width / 3) * 1.5), this.yPositionOnScreen + this.height), Color.White);

			this.drawMouse(b);
			base.draw(b);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			if (this.isWithinBounds(x, y)) {

				foreach (ClickableComponent monster in Pages[currentPage]) {
					if (monster.GetType() == typeof(MultiClickableMonsterComponent)) {
						MultiClickableMonsterComponent m = (MultiClickableMonsterComponent)monster;
						m.ReceiveLeftClick(x, y);
					} else if (monster.containsPoint(x, y)) {
						ClickableMonsterComponent m = (ClickableMonsterComponent)monster;
						Game1.activeClickableMenu = new MonsterPlaceMenu(monster.name, null, m.sprite);
					}
				}
				base.receiveLeftClick(x, y, true);

			} else if (leftArrow.containsPoint(x, y)) {
				currentPage = currentPage - 1 < 0 ? Pages.Count - 1 : currentPage - 1;
				Game1.playSound("smallSelect");

			} else if (rightArrow.containsPoint(x, y)) {
				currentPage = currentPage + 1 == Pages.Count ? 0 : currentPage + 1;
				Game1.playSound("smallSelect");

			} else {
				Game1.exitActiveMenu();
			}
		}
		public override void performHoverAction(int x, int y) {
			base.performHoverAction(x, y);
			foreach (ClickableComponent monster in Pages[currentPage]) {
				if (monster.GetType() == typeof(ClickableMonsterComponent)) {
					ClickableMonsterComponent m = (ClickableMonsterComponent)monster;
					m.PerformHoverAction(x, y);
				} else {
					MultiClickableMonsterComponent m = (MultiClickableMonsterComponent)monster;
					m.PerformHoverAction(x, y);
				}

			}
		}
		public override void snapToDefaultClickableComponent() {
			currentlySnappedComponent = getComponentWithID(0);
			snapCursorToCurrentSnappedComponent();
		}
	}
}
