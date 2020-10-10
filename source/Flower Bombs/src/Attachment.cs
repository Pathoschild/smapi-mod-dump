/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using System;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		public override int maximumStackSize () =>
			(heldObject.Value != null) ? 1 : 999;

		public override int attachmentSlots () => 1;

		private bool appearsFull
		{
			get
			{
				return ParentSheetIndex == FullID;
			}
			set
			{
				ParentSheetIndex = value ? FullID : EmptyID;
				if (sObject != null)
					sObject.ParentSheetIndex = ParentSheetIndex;
			}
		}

		private void setUpDataTracking ()
		{
			parentSheetIndex.fieldChangeEvent += (_field, _oldValue, newValue) =>
			{
				data = (newValue == FullID) ? FullData : EmptyData;
			};
		}

		public override void drawAttachments (SpriteBatch b, int x, int y)
		{
			b.Draw (Game1.menuTexture, new Vector2 (x, y),
				Game1.getSourceRectForStandardTileSheet (Game1.menuTexture,
					(heldObject.Value == null) ? 43 : 10),
				Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);

			string attachmentTitle;
			Color attachmentColor;
			if (heldObject.Value != null)
			{
				heldObject.Value.drawInMenu (b, new Vector2 (x, y), 0.75f);
				attachmentTitle = heldObject.Value.DisplayName;
				attachmentColor = heldObject.Value.getCategoryColor ();
			}
			else
			{
				attachmentTitle = Helper.Translation.Get ("FlowerBomb.empty");
				attachmentColor = Color.DarkSlateGray;
			}

			int hoverWidth = 32 + Math.Max (
				(int) Game1.smallFont.MeasureString (getDescription ()).X,
				(int) Game1.dialogueFont.MeasureString (DisplayName).X);
			attachmentTitle = Game1.parseText (attachmentTitle, Game1.smallFont,
				hoverWidth - 16 - 64 - 8 - 16);
			Utility.drawTextWithShadow (b, attachmentTitle, Game1.smallFont,
				new Vector2 (x + 64 + 8, y), attachmentColor);
		}

		private SObject attach (SObject seed)
		{
			Game1.playSound ("seeds");

			SObject seedReturn = null;
			if (seed.Stack == 1)
			{
				Game1.player.removeItemFromInventory (seed);
				seed.NetFields.Parent = null;
			}
			else
			{
				--seed.Stack;
				seedReturn = seed;
				seed = (SObject) seed.getOne ();
			}

			if (Stack == 1)
			{
				heldObject.Value = seed;
				appearsFull = true;
				return seedReturn;
			}
			else
			{
				FlowerBomb fusion = (FlowerBomb) getOne ();
				--Stack;
				fusion.heldObject.Value = seed;
				fusion.appearsFull = true;
				if (seedReturn != null)
					Game1.player.addItemToInventory (seedReturn);
				return fusion;
			}
		}

		private SObject detach (bool playSound = true)
		{
			if (playSound)
				Game1.playSound ("dwop");
			SObject seed = heldObject.Value;
			heldObject.Value = null;
			seed.NetFields.Parent = null;
			appearsFull = false;
			return seed;
		}

		public override bool minutesElapsed (int minutes,
			GameLocation environment)
		{
			// Don't show as readyForHarvest; our heldObject stays private.
			return false;
		}
	}
}
