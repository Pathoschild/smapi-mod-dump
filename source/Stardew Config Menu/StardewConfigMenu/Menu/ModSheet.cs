using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewConfigFramework;
using StardewConfigMenu.UI;
using StardewValley;
using StardewValley.Menus;

namespace StardewConfigMenu {
	public class ModSheet: IClickableMenu {

		int SelectedTab = 0;
		int FirstShownTab = 0;
		IOptionsPackage Package;
		readonly List<ModTab> Tabs = new List<ModTab>();
		readonly List<SCMTexturedLabel> UITabs = new List<SCMTexturedLabel>();

		private bool _visible = false;

		internal bool Visible {
			set {
				if (!value)
					Tabs.ForEach(x => {
						x.Visible = value;
					});
				else
					setVisibleTabs();

				_visible = value;
			}
			get => _visible;
		}

		private void setVisibleTabs() {
			for (int i = 0; i < Tabs.Count; i++) {
				Tabs[i].Visible = (i == SelectedTab);
			}
		}

		public ModSheet(IOptionsPackage package, int x, int y, int width, int height) : base(x, y, width, height) {
			Package = package;

			LoadTabs(package.Tabs);
			AddListeners();
		}

		private void LoadTabs(IOrderedDictionary<IOptionsTab> tabs) {
			// Use previous tabs instead of clearing and reloading to maintain state

			for (int i = 0; i < tabs.Count; i++) {
				// if past end must be new
				if (i >= Tabs.Count) {
					Tabs.Add(new ModTab(tabs[i], xPositionOnScreen, yPositionOnScreen, width, height));
					UITabs.Add(new SCMTexturedLabel(tabs[i].Label, Game1.smallFont));
					continue;
				}
				// check if tab already exists
				if (Tabs[i] == tabs[i]) {
					continue;
				}
				// doesnt match last existing tab, must have been replaced
				if (i + 1 >= Tabs.Count || i + 1 >= tabs.Count) {
					Tabs.RemoveAt(i);
					UITabs.RemoveAt(i);
					Tabs.Add(new ModTab(tabs[i], xPositionOnScreen, yPositionOnScreen, width, height));
					UITabs.Add(new SCMTexturedLabel(tabs[i].Label, Game1.smallFont));
					continue;
				}
				// Tab after matches, must be an deletion
				if (Tabs[i + 1] == tabs[i]) {
					Tabs.RemoveAt(i);
					UITabs.RemoveAt(i);
					continue;
				}
				// if next new tab matches existing tab must be an addition
				if (tabs[i + 1] == Tabs[i]) {
					Tabs.Insert(i, new ModTab(tabs[i], xPositionOnScreen, yPositionOnScreen, width, height));
					UITabs.Insert(i, new SCMTexturedLabel(tabs[i].Label, Game1.smallFont));
					continue;
				}
			}

			if (Tabs.Count > tabs.Count) {
				Tabs.RemoveRange(tabs.Count, Tabs.Count - tabs.Count);
				UITabs.RemoveRange(tabs.Count, Tabs.Count - tabs.Count);
			}

			if (SelectedTab >= Tabs.Count) {
				SelectedTab = Tabs.Count - 1;
			}
		}

		public void AddListeners() {
			RemoveListeners();
			Package.Tabs.ContentsDidChange += LoadTabs;
		}

		public void RemoveListeners(bool children = false) {
			if (children) {
				Tabs.ForEach(x => { x.RemoveListeners(children); });
			}
			Package.Tabs.ContentsDidChange -= LoadTabs;
		}

		public override void receiveRightClick(int x, int y, bool playSound = true) {
			if (!Visible || Tabs.Count < 1)
				return;

			Tabs[SelectedTab].receiveRightClick(x, y, playSound);
		}

		public override void receiveLeftClick(int x, int y, bool playSound = true) {
			if (!Visible || Tabs.Count < 1)
				return;

			Tabs[SelectedTab].receiveLeftClick(x, y, playSound);

			if (Tabs.Count < 2)
				return;

			for (int i = 0; i < UITabs.Count; i++) {
				if (UITabs[i].Bounds.Contains(x, y)) {
					Tabs[SelectedTab].Visible = false;
					SelectedTab = i;
					Tabs[i].Visible = true;
				}
			}
		}

		public override void leftClickHeld(int x, int y) {
			if (!Visible || Tabs.Count < 1)
				return;

			Tabs[SelectedTab].leftClickHeld(x, y);
		}

		public override void releaseLeftClick(int x, int y) {
			if (!Visible || Tabs.Count < 1)
				return;

			Tabs[SelectedTab].releaseLeftClick(x, y);
		}

		public override void receiveScrollWheelAction(int direction) {
			if (!Visible || Tabs.Count < 1)
				return;

			Tabs[SelectedTab].receiveScrollWheelAction(direction);
		}

		public override void draw(SpriteBatch b) {
			if (Tabs.Count < 1)
				return;

			Tabs[SelectedTab].draw(b);

			if (Tabs.Count < 2)
				return;

			int tabPosition = 0;

			for (int i = FirstShownTab; i < Tabs.Count; i++) {
				var xOffset = Game1.pixelZoom * ((i == SelectedTab) ? 6 : 9);
				UITabs[i].Transparency = (i == SelectedTab) ? 1f : 0.85f;
				UITabs[i].DrawAt(b, xPositionOnScreen - UITabs[i].Width - xOffset, yPositionOnScreen + (UITabs[i].Height * tabPosition) + (tabPosition * Game1.pixelZoom));
				tabPosition++;
			}
		}
	}
}
