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
    internal class SimpleColourModifier : BodyModifier
	{
		private const int pam_cost = 25;
		public SimpleColourModifier(bool isWizardSubmenu = false) : base(378, isWizardSubmenu)
		{
			Texture2D hair_texture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/pam_hairColors.png");
			hairSwatchColors = new Color[hair_texture.Width * hair_texture.Height];
			hair_texture.GetData(hairSwatchColors, 0, hairSwatchColors.Length);

			Texture2D haird_texture = Game1.content.Load<Texture2D>("Mods/ribeena.dynamicbodies/assets/Interface/pam_hairDarkColors.png");
			hairDarkSwatchColors = new Color[haird_texture.Width * haird_texture.Height];
			haird_texture.GetData(hairDarkSwatchColors, 0, hairDarkSwatchColors.Length);

			CharacterBackgroundRect = new Rectangle(96, 32, 32, 48);


			base.cost = isWizardSubmenu ? 0 : pam_cost;
			if (ModEntry.Config.freecustomisation) cost = 0;
			setUpPositions();
		}

		public override void setUpPositions()
		{
			setupGeneralPositions(T("box_dye_user") + ": ");
			bool allow_accessory_changes = true;

			this.swatches.Clear();

			int leftPadding = 64 + 4;
			int yOffset = 32;
			int label_col1_width = 42 * 4;
			int arrow_size = 64;

			int leftSelectionXOffset = ((LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.ru || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.es || LocalizedContentManager.CurrentLanguageCode == LocalizedContentManager.LanguageCode.pt) ? (-20) : 0);


			int label_col2_position = portraitBox.X + portraitBox.Width + 12 * 4;
			int label_col2_width = 40 * 4;

			//Items next to portrait box

			//Next line
			yOffset += 32;
			Point top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);


			top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			//Hair Colour
			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_HairColor")));

			swatches.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 12, swatchsize, swatchsize), "HairSwatchLeft")
			{
				myID = region_hairSwatch1,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			ClickableComponent hairSwatchSelected = new ClickableComponent(new Rectangle(top.X + swatchsize + 4, top.Y + 12, swatchsize, swatchsize), "HairSwatch0")
			{
				myID = region_hairSwatch2,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};
			hairSwatchSelected.scale = 1.1f;
			swatches.Add(hairSwatchSelected);

			swatches.Add(new ClickableComponent(new Rectangle(top.X + swatchsize * 2 + 8, top.Y + 12, swatchsize, swatchsize), "HairSwatch1")
			{
				myID = region_hairSwatch3,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			swatches.Add(new ClickableComponent(new Rectangle(top.X + swatchsize * 3 + 12, top.Y + 12, swatchsize, swatchsize), "HairSwatch2")
			{
				myID = region_hairSwatch4,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			//Next line
			yOffset += 58;


			//Hair Dark Colour
			top = new Point(label_col2_position + label_col2_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset);

			this.labels.Add(new ClickableComponent(new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), "Hair Dark:"));
			swatches.Add(new ClickableComponent(new Rectangle(top.X, top.Y + 12, swatchsize, swatchsize), "DarkHairSwatchLeft")
			{
				myID = region_hairDarkSwatch1,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			ClickableComponent darkHairSwatchSelected = new ClickableComponent(new Rectangle(top.X + swatchsize + 4, top.Y + 12, swatchsize, swatchsize), "DarkHairSwatch0")
			{
				myID = region_hairDarkSwatch2,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			};
			darkHairSwatchSelected.scale = 1.1f;
			swatches.Add(darkHairSwatchSelected);

			swatches.Add(new ClickableComponent(new Rectangle(top.X + swatchsize * 2 + 8, top.Y + 12, swatchsize, swatchsize), "DarkHairSwatch1")
			{
				myID = region_hairDarkSwatch3,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			swatches.Add(new ClickableComponent(new Rectangle(top.X + swatchsize * 3 + 12, top.Y + 12, swatchsize, swatchsize), "DarkHairSwatch2")
			{
				myID = region_hairDarkSwatch4,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});


			//After portraitbox
			yOffset = 176;

			//Wider selections
			label_col1_width += 48;
			label_col2_position = base.xPositionOnScreen + leftPadding + label_col1_width + arrow_size / 2 + (12 * 4);

			//Hair Style
			this.leftSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(base.xPositionOnScreen + leftPadding + leftSelectionXOffset - arrow_size / 2, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, arrow_size, arrow_size), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
			{
				myID = region_hairLeft,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});
			this.hairLabel = new ClickableComponent(new Rectangle(base.xPositionOnScreen + leftPadding + (label_col1_width / 2) + (leftSelectionXOffset / 2), base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Hair"));
			this.labels.Add(this.hairLabel);
			this.rightSelectionButtons.Add(new ClickableTextureComponent("Hair", new Rectangle(base.xPositionOnScreen + leftPadding + label_col1_width, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
			{
				myID = region_hairRight,
				upNeighborID = -99998,
				leftNeighborID = -99998,
				rightNeighborID = -99998,
				downNeighborID = -99998
			});

			if (allow_accessory_changes)
			{
				this.leftSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(label_col2_position, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 44), 1f)
				{
					myID = region_accLeft,
					upNeighborID = -99998,
					leftNeighborID = -99998,
					rightNeighborID = -99998,
					downNeighborID = -99998
				});
				this.labels.Add(this.accLabel = new ClickableComponent(new Rectangle(label_col2_position + arrow_size / 2 + label_col1_width / 2, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset + 16, 1, 1), Game1.content.LoadString("Strings\\UI:Character_Accessory")));
				this.rightSelectionButtons.Add(new ClickableTextureComponent("Acc", new Rectangle(label_col2_position + label_col1_width + arrow_size / 2, base.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + yOffset, 64, 64), null, "", Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 33), 1f)
				{
					myID = region_accRight,
					upNeighborID = -99998,
					leftNeighborID = -99998,
					rightNeighborID = -99998,
					downNeighborID = -99998
				});
			}


			if (Game1.options.snappyMenus && Game1.options.gamepadControls)
			{
				base.populateClickableComponentList();
				this.snapToDefaultClickableComponent();
			}
		}

	}
}
