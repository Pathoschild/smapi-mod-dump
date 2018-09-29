using System;
using StardewValley;
using StardewValley.Objects;
using Microsoft.Xna.Framework;
using Netcode;
using StardewValley.Locations;

namespace CasksAnywhere
{
	public class HijackCask : Cask
	{
		public HijackCask() { }

		public HijackCask(Cask b) : base(b.TileLocation)
		{
			heldObject.Value = b.heldObject.Value;
			agingRate.Value = b.agingRate.Value;
			daysToMature.Value = b.daysToMature.Value;
			MinutesUntilReady = b.MinutesUntilReady;
		}

		public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
		{
			if (dropIn != null && dropIn is StardewValley.Object && ((dropIn as StardewValley.Object).bigCraftable.Value) || heldObject.Value != null)
				return false;

			if (Quality >= 4)
				return false;

			bool flag = false;
			float num = 1f;

			switch (dropIn.ParentSheetIndex)
			{
				case 303:
					flag = true;
					num = 1.66f;
					break;
				case 346:
					flag = true;
					num = 2f;
					break;
				case 348:
					flag = true;
					num = 1f;
					break;
				case 424:
					flag = true;
					num = 4f;
					break;
				case 426:
					flag = true;
					num = 4f;
					break;
				case 459:
					flag = true;
					num = 2f;
					break;
			}

			if (!flag)
				return false;

			heldObject.Value = dropIn.getOne() as StardewValley.Object;

			if (!probe)
			{
				agingRate.Value = num;
				daysToMature.Value = 56f;
				MinutesUntilReady = 999999;
				if (heldObject.Value.Quality == 1)
					daysToMature.Value = 42f;
				else if (heldObject.Value.Quality == 2)
					daysToMature.Value = 28f;
				else if (heldObject.Value.Quality == 4)
				{
					daysToMature.Value = 0.0f;
					MinutesUntilReady = 1;
				}
				who.currentLocation.playSound("Ship");
				who.currentLocation.playSound("bubbles");
				CasksAnywhere.helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer").GetValue().broadcastSprites(who.currentLocation, new TemporaryAnimatedSprite[1]
				{
					new TemporaryAnimatedSprite("TileSheets\\animations", new Rectangle(256, 1856, 64, 128), 80f, 6, 999999, this.TileLocation * 64f + new Vector2(0.0f, (float)sbyte.MinValue), false, false, (float)(((double)this.TileLocation.Y + 1.0) * 64.0 / 10000.0 + 9.99999974737875E-05), 0.0f, Color.Yellow * 0.75f, 1f, 0.0f, 0.0f, 0.0f, false)
					{
						alphaFade = 0.005f
					}
				});
			}
			return true;
		}

	}
}
