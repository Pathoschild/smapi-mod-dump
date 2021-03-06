/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mizzion/MyStardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ResetTerrainFeaturesRedux.Framework.Menu
{
    public class Menu : IClickableMenu
    {
        public List<ClickableComponent> optionSlots = new List<ClickableComponent>();

        private List<MenuComponent> options = new List<MenuComponent>();

        private string hoverText = "";

        
        private const int optionsRows = 10;
		
        public Menu(int x, int y, int width, int height) : base(Game1.viewport.Width / 2 - (600 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (660 + IClickableMenu.borderWidth * 2) / 2, 600 + IClickableMenu.borderWidth * 2, 870 + IClickableMenu.borderWidth * 2, true)
		{
			List<ClickableComponent> list = this.optionSlots;
			ClickableComponent clickableComponent = new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 40, width - 32, 40), "0");
			clickableComponent.myID = 0;
			clickableComponent.downNeighborID = 1;
			clickableComponent.upNeighborID = -7777;
			clickableComponent.fullyImmutable = true;
			list.Add(clickableComponent);
			ClickableComponent item = new ClickableComponent(new Rectangle(this.xPositionOnScreen - 48, this.yPositionOnScreen + 80, width - 32, 40), "1");
			clickableComponent.myID = 1;
			clickableComponent.downNeighborID = 2;
			clickableComponent.upNeighborID = 0;
			clickableComponent.fullyImmutable = true;
			list.Add(item);
			this.options.Add(new CheckBoxes("Affect all locations", "RESETALLLOCATIONS", -1, -1, null));
			this.options.Add(new MenuComponent("Select which to change:", true));
			for (int i = 0; i < 10; i++)
			{
				list.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 16, this.yPositionOnScreen + 160 + i * 40, 118, 40), string.Concat(i))
				{
					myID = i,
					downNeighborID = list.Count + 1,
					upNeighborID = list.Count - 1,
					fullyImmutable = true
				});
			}
			this.options.Add(new CheckBoxes("Bushes", "Bush", -1, -1, null));
			this.options.Add(new CheckBoxes("Trees", "Tree", -1, -1, null));
			this.options.Add(new CheckBoxes("Weeds", "Weeds", -1, -1, null));
			this.options.Add(new CheckBoxes("Grass", "Grass", -1, -1, null));
			this.options.Add(new CheckBoxes("Twigs", "Twig", -1, -1, null));
			this.options.Add(new CheckBoxes("Rocks", "Rock", -1, -1, null));
			this.options.Add(new CheckBoxes("Forage", "Forage", -1, -1, null));
			this.options.Add(new CheckBoxes("Stumps", "Stump", -1, -1, null));
			this.options.Add(new CheckBoxes("Logs", "Log", -1, -1, null));
			this.options.Add(new CheckBoxes("Boulders", "Boulder", -1, -1, null));
			for (int j = 0; j < 6; j++)
			{
				list.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + 266, this.yPositionOnScreen + 160 + j * 40, 118, 40), string.Concat(j))
				{
					myID = j,
					downNeighborID = list.Count + 1,
					upNeighborID = list.Count - 1,
					fullyImmutable = true
				});
			}
			this.options.Add(new CheckBoxes("Paths", "Path", -1, -1, null));
			this.options.Add(new CheckBoxes("Fences", "Fence", -1, -1, null));
			this.options.Add(new CheckBoxes("Crops", "Crop", -1, -1, null));
			this.options.Add(new CheckBoxes("Tilled Soil", "Soil", -1, -1, new List<CheckBoxes>
			{
				this.options[this.getIndexByLabel("Crops")] as CheckBoxes
			}));
			this.options.Add(new CheckBoxes("Objects", "Object", -1, -1, new List<CheckBoxes>
			{
				this.options[this.getIndexByLabel("Weeds")] as CheckBoxes,
				this.options[this.getIndexByLabel("Fences")] as CheckBoxes,
				this.options[this.getIndexByLabel("Twigs")] as CheckBoxes,
				this.options[this.getIndexByLabel("Rocks")] as CheckBoxes,
				this.options[this.getIndexByLabel("Forage")] as CheckBoxes
			}));
			this.options.Add(new CheckBoxes("Terrain Features", "TFeature", -1, -1, new List<CheckBoxes>
			{
				this.options[this.getIndexByLabel("Trees")] as CheckBoxes,
				this.options[this.getIndexByLabel("Grass")] as CheckBoxes,
				this.options[this.getIndexByLabel("Paths")] as CheckBoxes,
				this.options[this.getIndexByLabel("Tilled Soil")] as CheckBoxes,
				this.options[this.getIndexByLabel("Crops")] as CheckBoxes
			}));
			for (int k = 0; k < 3; k++)
			{
				list.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + -10 + k * 200, this.yPositionOnScreen + 400 + 230, width - 32, 40), string.Concat(k))
				{
					myID = k,
					downNeighborID = 10 + k,
					upNeighborID = 8 + k,
					fullyImmutable = true
				});
			}
			this.options.Add(new Buttons("Reset", delegate ()
			{
				this.resetFeatures();
			}));
			this.adjustSlotBounds(this.options.Count - 1);
			this.options.Add(new Buttons("Clear", delegate ()
			{
				this.clearFeatures();
			}));
			this.adjustSlotBounds(this.options.Count - 1);
			this.options.Add(new Buttons("Generate", delegate ()
			{
				this.generateFeatures();
			}));
			this.adjustSlotBounds(this.options.Count - 1);
			foreach (MenuComponent resetMenuComponent in this.options)
			{
				bool flag = resetMenuComponent is CheckBoxes && Generator.GeneratorOptions.ContainsKey((resetMenuComponent as CheckBoxes).Which) && !resetMenuComponent.Disabled;
				if (flag)
				{
					bool flag2 = Generator.GeneratorOptions[(resetMenuComponent as CheckBoxes).Which];
					if (flag2)
					{
						(resetMenuComponent as CheckBoxes).Check(true);
					}
				}
			}
			this.setButtonStates();
			bool flag3 = !Game1.options.snappyMenus || !Game1.options.gamepadControls;
			if (!flag3)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
		}


		public void adjustSlotBounds(int index)
		{
			Rectangle bounds = this.options[index].Bound;
			this.optionSlots[index].bounds.Width = bounds.Width;
			this.optionSlots[index].bounds.Height = bounds.Height;
		}

		public int getIndexByLabel(string name)
		{
			foreach (MenuComponent resetMenuComponent in this.options)
			{
				bool flag = resetMenuComponent.Name.Equals(name);
				if (flag)
				{
					return this.options.IndexOf(resetMenuComponent);
				}
			}
			return -1;
		}

		public void resetFeatures()
		{
			bool flag = !Context.IsMainPlayer;
			if (!flag)
			{
				bool flag2 = Generator.GeneratorOptions.ContainsKey("RESETALLLOCATIONS") && Generator.GeneratorOptions["RESETALLLOCATIONS"];
				if (flag2)
				{
					foreach (GameLocation gameLocation in Game1.locations)
					{
						bool isOutdoors = gameLocation.IsOutdoors;
						if (isOutdoors)
						{
							Generator.reload(gameLocation, Generator.getTypesFromOptions(), Generator.getIndicesFromOptions());
						}
					}
				}
				Generator.reload(Game1.currentLocation, Generator.getTypesFromOptions(), Generator.getIndicesFromOptions());
			}
		}

		public void clearFeatures()
		{
			bool flag = !Context.IsMainPlayer;
			if (!flag)
			{
				bool flag2 = Generator.GeneratorOptions.ContainsKey("RESETALLLOCATIONS") && Generator.GeneratorOptions["RESETALLLOCATIONS"];
				if (flag2)
				{
					foreach (GameLocation gameLocation in Game1.locations)
					{
						bool isOutdoors = gameLocation.IsOutdoors;
						if (isOutdoors)
						{
							Generator.clear(gameLocation, Generator.getTypesFromOptions(), Generator.getIndicesFromOptions());
						}
					}
				}
				Generator.clear(Game1.currentLocation, Generator.getTypesFromOptions(), Generator.getIndicesFromOptions());
			}
		}

		public void generateFeatures()
		{
			bool flag = !Context.IsMainPlayer;
			if (!flag)
			{
				bool flag2 = Generator.GeneratorOptions.ContainsKey("RESETALLLOCATIONS") && Generator.GeneratorOptions["RESETALLLOCATIONS"];
				if (flag2)
				{
					foreach (GameLocation gameLocation in Game1.locations)
					{
						bool isOutdoors = gameLocation.IsOutdoors;
						if (isOutdoors)
						{
							Generator.loadMapFeatures(gameLocation, Generator.getTypesFromOptions(), Generator.getIndicesFromOptions());
						}
					}
				}
				Generator.loadMapFeatures(Game1.currentLocation, Generator.getTypesFromOptions(), Generator.getIndicesFromOptions());
			}
		}

		public void setButtonStates()
		{
			foreach (MenuComponent resetMenuComponent in this.options)
			{
				bool flag = resetMenuComponent is CheckBoxes && Generator.GeneratorOptions.ContainsKey((resetMenuComponent as CheckBoxes).Which) && !resetMenuComponent.Disabled;
				if (flag)
				{
					bool flag2 = Generator.GeneratorOptions[(resetMenuComponent as CheckBoxes).Which];
					bool flag3 = !(resetMenuComponent as CheckBoxes).IsChecked && flag2;
					if (flag3)
					{
						(resetMenuComponent as CheckBoxes).Check(true);
					}
					(resetMenuComponent as CheckBoxes).IsChecked = flag2;
				}
			}
			bool flag4 = false;
			bool flag5 = false;
			foreach (MenuComponent resetMenuComponent2 in this.options)
			{
				bool flag6 = resetMenuComponent2 is CheckBoxes && Generator.canGenerate.Contains((resetMenuComponent2 as CheckBoxes).Which) && Generator.GeneratorOptions.ContainsKey((resetMenuComponent2 as CheckBoxes).Which) && Generator.GeneratorOptions[(resetMenuComponent2 as CheckBoxes).Which];
				if (flag6)
				{
					flag4 = true;
				}
			}
			foreach (MenuComponent resetMenuComponent3 in this.options)
			{
				bool flag7 = resetMenuComponent3 is CheckBoxes && ((resetMenuComponent3 as CheckBoxes).Which != "RESETALLLOCATIONS" && Generator.canGenerate.Contains((resetMenuComponent3 as CheckBoxes).Which) && Generator.GeneratorOptions.ContainsKey((resetMenuComponent3 as CheckBoxes).Which)) && Generator.GeneratorOptions[(resetMenuComponent3 as CheckBoxes).Which];
				if (flag7)
				{
					flag4 = false;
				}
				bool flag8 = resetMenuComponent3 is CheckBoxes && (resetMenuComponent3 as CheckBoxes).Which != "RESETALLLOCATIONS" && Generator.GeneratorOptions.ContainsKey((resetMenuComponent3 as CheckBoxes).Which) && Generator.GeneratorOptions[(resetMenuComponent3 as CheckBoxes).Which];
				if (flag8)
				{
					flag5 = true;
				}
			}
			bool flag9 = flag4;
			if (flag9)
			{
				this.options[this.getIndexByLabel("Reset")].Disabled = false;
				this.options[this.getIndexByLabel("Generate")].Disabled = false;
			}
			else
			{
				this.options[this.getIndexByLabel("Reset")].Disabled = true;
				this.options[this.getIndexByLabel("Generate")].Disabled = true;
			}
			bool flag10 = flag5;
			this.options[this.getIndexByLabel("Clear")].Disabled = !flag10;
		}


		public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            
            bool forcePreventClose = GameMenu.forcePreventClose;
            if (!forcePreventClose)
            {
                for (int i = 0; i < this.optionSlots.Count; i++)
                {
                    bool flag = this.optionSlots[i].bounds.Contains(x, y) && i < this.options.Count && this.options[i].Bound.Contains(x - this.optionSlots[i].bounds.X, y - this.optionSlots[i].bounds.Y);
                    if (flag)
                    {
                        this.options[i].ReceiveLeftClick(x - this.optionSlots[i].bounds.X, y - this.optionSlots[i].bounds.Y);
                        break;
                    }
                }
                base.receiveLeftClick(x, y, playSound);
				this.setButtonStates();
            }
        }
		public override void leftClickHeld(int x, int y)
        {
            bool forcePreventClose = GameMenu.forcePreventClose;
            if (!forcePreventClose)
            {
                base.leftClickHeld(x, y);
                for (int i = 0; i < this.optionSlots.Count; i++)
                {
                    bool flag = this.optionSlots[i].bounds.Contains(x, y) && i < this.options.Count && this.options[i].Bound.Contains(x - this.optionSlots[i].bounds.X, y - this.optionSlots[i].bounds.Y);
                    if (flag)
                    {
                        this.options[i].HoldLeftClick(x - this.optionSlots[i].bounds.X, y - this.optionSlots[i].bounds.Y);
                    }
                    else
                    {
                        this.options[i].ReceiveLeftClick(x - this.optionSlots[i].bounds.X, y - this.optionSlots[i].bounds.Y);
                    }
                }
            }
        }

		public override void releaseLeftClick(int x, int y)
        {
            bool forcePreventClose = GameMenu.forcePreventClose;
            if (!forcePreventClose)
            {
                base.releaseLeftClick(x, y);
                for (int i = 0; i < this.optionSlots.Count; i++)
                {
                    this.options[i].ReleaseLeftClick(x - this.optionSlots[i].bounds.X, y - this.optionSlots[i].bounds.Y);
                }
            }
        }

		public void drawOld(SpriteBatch b)
        {
            b.End();
            b.Begin(SpriteSortMode.FrontToBack, BlendState.NonPremultiplied, SamplerState.PointClamp, null, null);
            for (int i = 0; i < this.optionSlots.Count; i++)
            {
                bool flag = i < this.options.Count;
                if (flag)
                {
                    this.options[i].Draw(b, this.optionSlots[i].bounds.X, this.optionSlots[i].bounds.Y);
                }
            }
            b.End();
            b.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null);
            bool flag2 = this.hoverText.Equals("");
            if (!flag2)
            {
                IClickableMenu.drawHoverText(b, this.hoverText, Game1.smallFont, 0, 0, -1, null, -1, null, null, 0, -1, -1, -1, -1, 1f, null, null);
            }
        }

		public override void draw(SpriteBatch b)
        {
            bool flag = !Game1.options.showMenuBackground;
            if (flag)
            {
                b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            }
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - (256 + (40 + IClickableMenu.borderWidth * 2)) + 36, Color.White, 4f, true);
            IClickableMenu.drawTextureBox(b, Game1.mouseCursors, new Rectangle(384, 373, 18, 18), this.xPositionOnScreen, this.yPositionOnScreen + this.height - (256 + (40 + IClickableMenu.borderWidth * 2)) + 36, this.width, 40 + IClickableMenu.borderWidth * 2, Color.White, 4f, true);
            for (int i = 0; i < this.optionSlots.Count; i++)
            {
                bool flag2 = i < this.options.Count;
                if (flag2)
                {
                    this.options[i].Draw(b, this.optionSlots[i].bounds.X, this.optionSlots[i].bounds.Y);
                }
            }
            base.draw(b);
            base.drawMouse(b);
        }
	}
}
