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
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using System;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb
	{
		public SObject seed
		{
			get
			{
				return Base?.heldObject?.Value;
			}
			private set
			{
				Base.heldObject.Value = value;
			}
		}

		public override int maximumStackSize () => (seed != null) ? 1 : 999;

		public override int attachmentSlots () => 1;

		private SObject attach (SObject newSeed)
		{
			Game1.playSound ("seeds");

			SObject seedReturn = null;
			if (newSeed.Stack == 1)
			{
				Game1.player.removeItemFromInventory (newSeed);
				newSeed.NetFields.Parent = null;
			}
			else
			{
				--newSeed.Stack;
				seedReturn = newSeed;
				newSeed = (SObject) newSeed.getOne ();
			}

			if (Base.Stack == 1)
			{
				seed = newSeed;
				return seedReturn;
			}
			else
			{
				SObject fusion = GetNew (newSeed);
				--Base.Stack;
				if (seedReturn != null)
					Game1.player.addItemToInventory (seedReturn);
				return fusion;
			}
		}

		private SObject detach (bool playSound = true)
		{
			if (playSound)
				Game1.playSound ("dwop");
			SObject oldSeed = seed;
			seed = null;
			if (oldSeed != null)
				oldSeed.NetFields.Parent = null;
			return oldSeed;
		}

		public override bool minutesElapsed (int minutes, GameLocation environment)
		{
			// Don't show as readyForHarvest; our heldObject stays private.
			return false;
		}

		private static void DrawAttachments_Postfix (Item __instance, SpriteBatch b,
			int x, int y)
		{
			try
			{
				if (TryGetLinked (__instance, out FlowerBomb bomb))
					bomb.drawAttachments (b, x, y);
			}
			catch (Exception e)
			{
				Monitor.Log ($"Failed in {nameof (DrawAttachments_Postfix)}:\n{e}",
					LogLevel.Error);
			}
		}

		private new void drawAttachments (SpriteBatch b, int x, int y)
		{
			// Don't draw the attachments in the crafting menu, as they overdraw
			// the "Number Crafted" line. Unfortunately this also hides the
			// attachments on actual objects in the inventory.
			if (Game1.activeClickableMenu is GameMenu gm &&
					gm.pages[gm.currentTab] is CraftingPage)
				return;

			b.Draw (Game1.menuTexture, new Vector2 (x, y),
				Game1.getSourceRectForStandardTileSheet (Game1.menuTexture,
					(seed == null) ? 43 : 10),
				Color.White, 0f, Vector2.Zero, 1f, SpriteEffects.None, 0.86f);

			string attachmentTitle;
			Color attachmentColor;
			if (seed != null)
			{
				seed.drawInMenu (b, new Vector2 (x, y), 0.75f);
				attachmentTitle = seed.DisplayName;
				attachmentColor = seed.getCategoryColor ();
			}
			else
			{
				attachmentTitle = Helper.Translation.Get ("FlowerBomb.empty");
				attachmentColor = Color.DarkSlateGray;
			}

			int hoverWidth = 32 + Math.Max (
				(int) Game1.smallFont.MeasureString (Base.getDescription ()).X,
				(int) Game1.dialogueFont.MeasureString (Base.DisplayName).X);
			attachmentTitle = Game1.parseText (attachmentTitle, Game1.smallFont,
				hoverWidth - 16 - 64 - 8 - 16);
			Utility.drawTextWithShadow (b, attachmentTitle, Game1.smallFont,
				new Vector2 (x + 64 + 8, y), attachmentColor);
		}

		// The remaining methods wrap the various SObject.draw* methods to
		// temporarily switch to the index of the full texture when a seed is
		// present. This is a weird hack but it works.

		public override void drawWhenHeld (SpriteBatch spriteBatch,
			Vector2 objectPosition, Farmer f)
		{
			if (seed != null)
				Base.ParentSheetIndex = FullSaveIndex.Index;
			try
			{
				Link.CallUnlinked<SObject> (o => o.drawWhenHeld (spriteBatch,
					objectPosition, f));
			}
			finally
			{
				Base.ParentSheetIndex = TileIndex;
			}
		}

		public override void drawInMenu (SpriteBatch spriteBatch, Vector2 location,
			float scaleSize, float transparency, float layerDepth,
			StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			if (seed != null)
				Base.ParentSheetIndex = FullSaveIndex.Index;
			try
			{
				Link.CallUnlinked<SObject> (o => o.drawInMenu (spriteBatch, location,
					scaleSize, transparency, layerDepth, drawStackNumber, color,
					drawShadow));
			}
			finally
			{
				Base.ParentSheetIndex = TileIndex;
			}
		}

		public override void drawAsProp (SpriteBatch b)
		{
			if (seed != null)
				Base.ParentSheetIndex = FullSaveIndex.Index;
			try
			{
				Link.CallUnlinked<SObject> (o => o.drawAsProp (b));
			}
			finally
			{
				Base.ParentSheetIndex = TileIndex;
			}
		}

		public override void draw (SpriteBatch spriteBatch, int x, int y,
			float alpha = 1f)
		{
			if (seed != null)
				Base.ParentSheetIndex = FullSaveIndex.Index;
			try
			{
				Link.CallUnlinked<SObject> (o => o.draw (spriteBatch, x, y, alpha));
			}
			finally
			{
				Base.ParentSheetIndex = TileIndex;
			}
		}
	}
}
