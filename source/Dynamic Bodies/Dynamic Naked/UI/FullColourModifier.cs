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
    
	internal class FullColourModifier : BodyModifier
	{
		private const int leah_cost = 100;
		public FullColourModifier(bool isWizardSubmenu = false) : base(528, isWizardSubmenu)
		{
			CharacterBackgroundRect = new Rectangle(64, 32, 32, 48);

			base.cost = isWizardSubmenu ? 0 : leah_cost;
			if (ModEntry.Config.freecustomisation) cost = 0;
			setUpPositions();
		}

		public override void setUpPositions()
		{
			setupGeneralPositions(T("color_specialist") + ": ");
			this.colorPickerCCs.Clear();

			int leftPadding = 64 + 4;
			int yOffset = 32;
			int label_col1_width = 42 * 4;
			int arrow_size = 64;

			int leftSelectionXOffset = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? (-20) : 0);
			int label_col2_position = portraitBox.X + portraitBox.Width + 12 * 4;
			int label_col2_width = 40 * 4;

			//Items next to portrait box

			//Line below 
			yOffset += 32;
			Point top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			//Eye colour picker
			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_EyeColor")));

			this.eyeColorPicker = new ColorPicker("Eyes", top.X, top.Y);
			this.eyeColorPicker.setColor(who.newEyeColor.Value);
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
			{
				myID = region_colorPicker1,
				downNeighborID = -99998,
				upNeighborID = region_eyeToggle,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
			{
				myID = region_colorPicker2,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
			{
				myID = region_colorPicker3,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});

			eyeToggleButton = new ClickableComponent(new Rectangle(top.X+174, top.Y +14, 16 * 2, 16 * 2), "EyeToggle")
			{
				myID = region_eyeToggle,
				upNeighborID = 604,//pants toggle
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = region_colorPicker1
			};

			//Next line
			yOffset += 68;
			top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			//Hair Colour
			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor")));

			this.hairColorPicker = new ColorPicker("Hair", top.X, top.Y);
			this.hairColorPicker.setColor(who.hairstyleColor.Value);
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
			{
				myID = region_colorPicker4,
				downNeighborID = -99998,
				upNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
			{
				myID = region_colorPicker5,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
			{
				myID = region_colorPicker6,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});

			//Next line
			yOffset += 68;
			top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			//Dark hair
			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), T("hair_dark") + ":"));
			this.hairDarkColorPicker = new ColorPicker("HairDark", top.X, top.Y);

			if (who.modData.ContainsKey("DB.darkHair"))
			{
				this.hairDarkColorPicker.setColor(new Color(uint.Parse(who.modData["DB.darkHair"])));
			}
			else
			{
				//57 grey is often the darket colour of a hair style
				this.hairDarkColorPicker.setColor(new Color(57, 57, 57));
			}
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
			{
				myID = region_colorPicker7,
				downNeighborID = -99998,
				upNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
			{
				myID = region_colorPicker8,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
			{
				myID = region_colorPicker9,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});

			//Next line
			yOffset += 68;
			top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			//Eye sclera color
			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), T("sclera_color") + ":"));
			this.scleraColorPicker = new ColorPicker("EyeS", top.X, top.Y);

			if (who.modData.ContainsKey("DB.eyeColorS"))
			{
				this.scleraColorPicker.setColor(new Color(uint.Parse(who.modData["DB.eyeColorS"])));
			}
			else
			{
				this.scleraColorPicker.setColor(new Color(255, 253, 252));
			}
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
			{
				myID = region_colorPicker13,
				downNeighborID = -99998,
				upNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
			{
				myID = region_colorPicker14,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
			{
				myID = region_colorPicker15,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});

			//Next line
			yOffset += 68;
			top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			//Eye sclera color
			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), T("lash_color") + ":"));
			this.lashColorPicker = new ColorPicker("Lash", top.X, top.Y);

			if (who.modData.ContainsKey("DB.lash"))
			{
				this.lashColorPicker.setColor(new Color(uint.Parse(who.modData["DB.lash"])));
			}
			else
			{
				this.lashColorPicker.setColor(new Color(15, 10, 8));
			}
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y, 128, 20), "")
			{
				myID = region_colorPicker10,
				downNeighborID = -99998,
				upNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 20, 128, 20), "")
			{
				myID = region_colorPicker11,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});
			this.colorPickerCCs.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 40, 128, 20), "")
			{
				myID = region_colorPicker12,
				upNeighborID = -99998,
				downNeighborID = -99998,
				leftNeighborImmutable = true,
				rightNeighborImmutable = true
			});

			//Next line
			yOffset += 68;


			//Wider selections
			label_col1_width += 48;
			label_col2_position = base.xPositionOnScreen + leftPadding + label_col1_width + arrow_size / 2 + (12 * 4);

			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
		}

	}
}
