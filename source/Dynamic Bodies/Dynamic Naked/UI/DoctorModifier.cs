/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ribeena/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley.BellsAndWhistles;
using StardewValley.Characters;
using StardewValley.Objects;
using StardewValley.Menus;
using StardewValley;
using StardewModdingAPI;

using DynamicBodies.Data;

namespace DynamicBodies.UI
{
    internal class DoctorModifier : IClickableMenu
	{
		private const int windowHeight = 512;
		public const int doctor_cost = 250;
		public bool isWizardSubmenu = false;

		public const int region_bodyTab = 12340, region_faceTab = 12341;

		Texture2D UItexture;

		public int currentTab;
		public string hoverText = "";
		public string descriptionText = "";

		public List<ClickableComponent> tabs = new List<ClickableComponent>();

		public List<IClickableMenu> pages = new List<IClickableMenu>();

		public DoctorModifier(bool isWizardSubmenu = false)
			: base(Game1.uiViewport.Width / 2 - (632 + IClickableMenu.borderWidth* 2) / 2, Game1.uiViewport.Height / 2 - (windowHeight + IClickableMenu.borderWidth* 2) / 2 - 64, 632 + IClickableMenu.borderWidth* 2, windowHeight + IClickableMenu.borderWidth* 2 + 64)
		
        {
			this.isWizardSubmenu = isWizardSubmenu;

			this.UItexture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/ui.png");

			this.pages.Add(new DoctorModifierBodyPage(this, isWizardSubmenu, windowHeight));
			this.tabs.Add(new ClickableComponent(new Rectangle(base.xPositionOnScreen + 64, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "body", T("body"))
			{
				myID = region_bodyTab,
				downNeighborID = -99999,
				rightNeighborID = region_faceTab,
				tryDefaultIfNoDownNeighborExists = true,
				fullyImmutable = true
			});
			this.pages.Add(new DoctorModifierFacePage(this, isWizardSubmenu, windowHeight));
			this.tabs.Add(new ClickableComponent(new Rectangle(base.xPositionOnScreen + 128, base.yPositionOnScreen + IClickableMenu.tabYPositionRelativeToMenuY + 64, 64, 64), "face", T("face"))
			{
				myID = region_faceTab,
				downNeighborID = -99999,
				leftNeighborID = region_bodyTab,
				tryDefaultIfNoDownNeighborExists = true,
				fullyImmutable = true
			});

			currentTab = 0;

			this.pages[this.currentTab].populateClickableComponentList();
			this.AddTabsToClickableComponents(this.pages[this.currentTab]);
			if (Game1.options.SnappyMenus)
			{
				this.snapToDefaultClickableComponent();
			}
		}

		//Make translation easier
		static public string T(string toTranslate)
		{
			return ModEntry.translate(toTranslate);
		}

		public void AddTabsToClickableComponents(IClickableMenu menu)
		{
			menu.allClickableComponents.AddRange(this.tabs);
		}

		public override void automaticSnapBehavior(int direction, int oldRegion, int oldID)
		{
			if (this.GetCurrentPage() != null)
			{
				this.GetCurrentPage().automaticSnapBehavior(direction, oldRegion, oldID);
			}
			else
			{
				base.automaticSnapBehavior(direction, oldRegion, oldID);
			}
		}

		public override void snapToDefaultClickableComponent()
		{
			if (this.currentTab < this.pages.Count)
			{
				this.pages[this.currentTab].snapToDefaultClickableComponent();
			}
		}

		public override void receiveGamePadButton(Buttons b)
		{
			base.receiveGamePadButton(b);
			switch (b)
			{
				case Buttons.RightTrigger:
					if (this.currentTab < 2 && this.pages[this.currentTab].readyToClose())
					{
						this.changeTab(this.currentTab + 1);
					}
					return;
				case Buttons.LeftTrigger:
					if (this.currentTab > 0 && this.pages[this.currentTab].readyToClose())
					{
						this.changeTab(this.currentTab - 1);
					}
					return;
			}
			this.pages[this.currentTab].receiveGamePadButton(b);
		}

		public override void setUpForGamePadMode()
		{
			base.setUpForGamePadMode();
			if (this.pages.Count > this.currentTab)
			{
				this.pages[this.currentTab].setUpForGamePadMode();
			}
		}

		public override ClickableComponent getCurrentlySnappedComponent()
		{
			return this.pages[this.currentTab].getCurrentlySnappedComponent();
		}

		public override void setCurrentlySnappedComponentTo(int id)
		{
			this.pages[this.currentTab].setCurrentlySnappedComponentTo(id);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true)
		{
			base.receiveLeftClick(x, y, playSound);
			
			if (!GameMenu.forcePreventClose)
			{
				for (int i = 0; i < this.tabs.Count; i++)
				{
					if (this.tabs[i].containsPoint(x, y) && this.currentTab != i && this.pages[this.currentTab].readyToClose())
					{
						this.changeTab(this.getTabNumberFromName(this.tabs[i].name));
						return;
					}
				}
			}
			this.pages[this.currentTab].receiveLeftClick(x, y);
		}

		public static string getLabelOfTabFromIndex(int index)
		{
			return index switch
			{
				0 => T("body"),
				1 => T("face"),
				_ => "",
			};
		}

		public override void receiveRightClick(int x, int y, bool playSound = true)
		{
			this.pages[this.currentTab].receiveRightClick(x, y);
		}

		public override void receiveScrollWheelAction(int direction)
		{
			base.receiveScrollWheelAction(direction);
			this.pages[this.currentTab].receiveScrollWheelAction(direction);
		}

		public override void performHoverAction(int x, int y)
		{
			base.performHoverAction(x, y);
			this.hoverText = "";
			this.pages[this.currentTab].performHoverAction(x, y);
			foreach (ClickableComponent c in this.tabs)
			{
				if (c.containsPoint(x, y))
				{
					this.hoverText = c.label;
					break;
				}
			}
		}

		public int getTabNumberFromName(string name)
		{
			int whichTab = -1;
			switch (name)
			{
				case "body":
					whichTab = 0;
					break;
				case "face":
					whichTab = 1;
					break;
			}
			return whichTab;
		}

		public override void update(GameTime time)
		{
			base.update(time);
			this.pages[this.currentTab].update(time);
		}

		public override void releaseLeftClick(int x, int y)
		{
			base.releaseLeftClick(x, y);
			this.pages[this.currentTab].releaseLeftClick(x, y);
		}

		public override void leftClickHeld(int x, int y)
		{
			base.leftClickHeld(x, y);
			this.pages[this.currentTab].leftClickHeld(x, y);
		}

		public override bool readyToClose()
		{
			if (!GameMenu.forcePreventClose)
			{
				return this.pages[this.currentTab].readyToClose();
			}
			return false;
		}

		public void changeTab(int whichTab, bool playSound = true)
		{
			//Return clothing
			(this.pages[currentTab] as BodyModifier).RevertClothing();
			//Swap tab
			this.currentTab = this.getTabNumberFromName(this.tabs[whichTab].name);

			if (playSound)
			{
				Game1.playSound("smallSelect");
			}
			this.pages[this.currentTab].populateClickableComponentList();
			this.AddTabsToClickableComponents(this.pages[this.currentTab]);
			if (Game1.options.SnappyMenus)
			{
				this.snapToDefaultClickableComponent();
			}
		}

		public IClickableMenu GetCurrentPage()
		{
			if (this.currentTab >= this.pages.Count || this.currentTab < 0)
			{
				return null;
			}
			return this.pages[this.currentTab];
		}

		public override void draw(SpriteBatch b)
		{

			Game1.drawDialogueBox(base.xPositionOnScreen, base.yPositionOnScreen, this.pages[this.currentTab].width, this.pages[this.currentTab].height, speaker: false, drawOnlyBox: true);
			b.End();
			b.Begin(SpriteSortMode.FrontToBack, BlendState.AlphaBlend, SamplerState.PointClamp);
			foreach (ClickableComponent c in this.tabs)
			{
				int sheetIndex = 0;
				switch (c.name)
				{
					case "body":
						sheetIndex = 0;
						break;
					case "face":
						sheetIndex = 1;
						break;
				}
				b.Draw(UItexture, new Vector2(c.bounds.X, c.bounds.Y + ((this.currentTab == this.getTabNumberFromName(c.name)) ? 8 : 0)), new Rectangle(sheetIndex * 16+128, 0, 16, 16), Color.White, 0f, Vector2.Zero, 4f, SpriteEffects.None, 0.0001f);
			}
			b.End();
			b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp);
			this.pages[this.currentTab].draw(b);
			if (!this.hoverText.Equals(""))
			{
				IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont);
			}
			

			if ((!Game1.options.SnappyMenus || !(this.pages[this.currentTab] is CollectionsPage) || (this.pages[this.currentTab] as CollectionsPage).letterviewerSubMenu == null) && !Game1.options.hardwareCursor)
			{
				base.drawMouse(b, ignore_transparency: true);
			}
		}

		public override bool areGamePadControlsImplemented()
		{
			return false;
		}

		public override void receiveKeyPress(Keys key)
		{
			if (Game1.options.menuButton.Contains(new InputButton(key)) && this.readyToClose())
			{
				Game1.exitActiveMenu();
				Game1.playSound("bigDeSelect");
			}
			this.pages[this.currentTab].receiveKeyPress(key);
		}

		public override void emergencyShutDown()
		{
			base.emergencyShutDown();
			this.pages[this.currentTab].emergencyShutDown();
		}

		protected override void cleanupBeforeExit()
		{
			base.cleanupBeforeExit();
			if (Game1.options.optionsDirty)
			{
				Game1.options.SaveDefaultOptions();
			}
		}
	}
}
