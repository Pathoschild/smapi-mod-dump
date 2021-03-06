/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/UpgradeablePan
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.Tools;
using StardewValley;
using System;
using System.Collections.Generic;

namespace UpgradablePan
{
	public class PanPatches
	{
		public static bool beginUsing_Prefix(ref Pan __instance, GameLocation location, int x, int y, Farmer who, ref bool __result)
		{
			float maxDistance = 192f;
			switch (__instance.UpgradeLevel)
			{
				default: // Copper pan.  Just use standard functionality.
					return true;
				case 2:
					maxDistance = 208f;
					break;
				case 3:
					maxDistance = 224f;
					break;
				case 4:
					maxDistance = 256f;
					break;
			}
			__instance.CurrentParentTileIndex = 12;
			__instance.IndexOfMenuItemView = 12;
			bool overrideCheck = false;
			Rectangle orePanRect = new Rectangle(location.orePanPoint.X * 64 - 64, location.orePanPoint.Y * 64 - 64, 256, 256);
			if (orePanRect.Contains(x, y) && Utility.distance(who.getStandingX(), orePanRect.Center.X, who.getStandingY(), orePanRect.Center.Y) <= maxDistance)
			{
				overrideCheck = true;
			}
			who.lastClick = Vector2.Zero;
			x = (int)who.GetToolLocation().X;
			y = (int)who.GetToolLocation().Y;
			who.lastClick = new Vector2(x, y);
			if (location.orePanPoint != null && !location.orePanPoint.Equals(Point.Zero))
			{
				Rectangle panRect = who.GetBoundingBox();
				if (overrideCheck || panRect.Intersects(orePanRect))
				{
					who.faceDirection(2);
					int exhaustion = (who.Stamina <= 0f && who.usingTool.Value) ? 2 : 1;
					who.FarmerSprite.animateOnce(303, 50f, 4);
					__result = true;
					return false;
				}
			}
			who.forceCanMove();
			__result = true;
			return false;
		}

		public static bool getPanItems_Prefix(ref Pan __instance, GameLocation location, Farmer who, ref List<Item> __result)
		{
			Random r = new Random(location.orePanPoint.X + location.orePanPoint.Y * 1000 + (int)Game1.stats.DaysPlayed);

			float ironChance = 0f;
			float goldChance = 0f;
			float iridiumChance = 0f;
			float radioactiveChance = 0f;

			float extraChance = 0f;
			int orePieces = 0;
			int extraPieces = 0;

			Dictionary<int, KeyValuePair<float, int>> extraItems = new Dictionary<int, KeyValuePair<float, int>>();
			switch (__instance.UpgradeLevel)
			{
				default: // Copper pan.  Just use standard functionality.
					return true;

				case 2: // Steel pan.
				{
					ironChance = 0.4f;
					goldChance = 0.28f;
					iridiumChance = 0.02f;
					radioactiveChance = 0f;
					
					orePieces = r.Next(5) + 1 + (int)((r.NextDouble() + 0.1 + (double)((float)(int)who.luckLevel / 10f) + who.DailyLuck) * 2.0);
					extraChance = 0.4f + who.LuckLevel * 0.04f;
					extraPieces = r.Next(5) + 1 + (int)((r.NextDouble() + 0.1 + (double)((float)(int)who.luckLevel / 10f)) * 2.0);

					//extraItems.Add(StardewValley.Object.coal,					new KeyValuePair<float, int>(0.45f, extraPieces));
					extraItems.Add(StardewValley.Object.diamondIndex,			new KeyValuePair<float, int>(0.025f + who.luckLevel * 0.02f, 1));
					extraItems.Add(StardewValley.Object.emeraldIndex,			new KeyValuePair<float, int>(0.016f, 1));
					extraItems.Add(StardewValley.Object.aquamarineIndex,		new KeyValuePair<float, int>(0.016f, 1));
					extraItems.Add(StardewValley.Object.rubyIndex,				new KeyValuePair<float, int>(0.016f, 1));
					extraItems.Add(StardewValley.Object.amethystClusterIndex,	new KeyValuePair<float, int>(0.016f, 1));
					extraItems.Add(StardewValley.Object.sapphireIndex,			new KeyValuePair<float, int>(0.016f, 1));
					extraItems.Add(749 /* Omni Geode */,						new KeyValuePair<float, int>(0.275f, extraPieces / 2));
					extraItems.Add(82  /* Fire Quartz */,						new KeyValuePair<float, int>(0.04f, 1));
					extraItems.Add(84  /* Frozen Tear */,						new KeyValuePair<float, int>(0.04f, 1));
					extraItems.Add(86  /* Earth Crystal */,						new KeyValuePair<float, int>(0.04f, 1));
					extraItems.Add(770 /* Mixed Seeds */,						new KeyValuePair<float, int>(0.05f, extraPieces));
					break;
				}
				case 3: // Gold pan.
				{
					ironChance = 0.4f;
					goldChance = 0.31f;
					iridiumChance = 0.04f;
					radioactiveChance = 0f;
					
					orePieces = r.Next(5) + 1 + (int)((r.NextDouble() + 0.1 + (double)((float)(int)who.luckLevel / 10f) + who.DailyLuck) * 2.0);
					extraChance = 0.45f + who.LuckLevel * 0.04f;
					extraPieces = r.Next(5) + 1 + (int)((r.NextDouble() + 0.1 + (double)((float)(int)who.luckLevel / 10f)) * 2.0);

					//extraItems.Add(StardewValley.Object.coal,					new KeyValuePair<float, int>(0.4f, extraPieces));
					extraItems.Add(StardewValley.Object.diamondIndex,			new KeyValuePair<float, int>(0.03f + who.luckLevel * 0.02f, 1));
					extraItems.Add(StardewValley.Object.emeraldIndex,			new KeyValuePair<float, int>(0.02f, 1));
					extraItems.Add(StardewValley.Object.aquamarineIndex,		new KeyValuePair<float, int>(0.02f, 1));
					extraItems.Add(StardewValley.Object.rubyIndex,				new KeyValuePair<float, int>(0.02f, 1));
					extraItems.Add(559 /* Pyrite */,							new KeyValuePair<float, int>(0.02f, r.Next(2, 4)));
					extraItems.Add(StardewValley.Object.sapphireIndex,			new KeyValuePair<float, int>(0.02f, 1));
					extraItems.Add(749 /* Omni Geode */,						new KeyValuePair<float, int>(0.29f, extraPieces / 2));
					extraItems.Add(82  /* Fire Quartz */,						new KeyValuePair<float, int>(0.04f, 1));
					extraItems.Add(84  /* Frozen Tear */,						new KeyValuePair<float, int>(0.04f, 1));
					extraItems.Add(86  /* Earth Crystal */,						new KeyValuePair<float, int>(0.02f, 1));
					extraItems.Add(573 /* Hematite */,							new KeyValuePair<float, int>(0.02f, 1));
					extraItems.Add(114 /* Ancient Seed */,						new KeyValuePair<float, int>(0.01f, 1));
					extraItems.Add(770 /* Mixed Seeds */,						new KeyValuePair<float, int>(0.05f, extraPieces));
					break;
				}
				case 4: // Iridium pan.
				{
					ironChance = 0.3f;
					goldChance = 0.36f;
					iridiumChance = 0.12f;
					radioactiveChance = 0.02f;
					
					orePieces = r.Next(5) + 2 + (int)((r.NextDouble() + 0.1 + (double)((float)(int)who.luckLevel / 10f) + who.DailyLuck) * 2.0);
					extraChance = 0.5f + who.LuckLevel * 0.04f;
					extraPieces = r.Next(5) + 2 + (int)((r.NextDouble() + 0.1 + (double)((float)(int)who.luckLevel / 10f)) * 2.0);

					// extraItems.Add(StardewValley.Object.coal,					new KeyValuePair<float, int>(0.35f, extraPieces));
					extraItems.Add(StardewValley.Object.diamondIndex,			new KeyValuePair<float, int>(0.05f + who.luckLevel * 0.02f, 1));
					extraItems.Add(StardewValley.Object.emeraldIndex,			new KeyValuePair<float, int>(0.0275f, 1));
					extraItems.Add(StardewValley.Object.rubyIndex,				new KeyValuePair<float, int>(0.0275f, 1));
					extraItems.Add(559 /* Pyrite */,							new KeyValuePair<float, int>(0.0275f, r.Next(2, 4)));
					extraItems.Add(StardewValley.Object.sapphireIndex,			new KeyValuePair<float, int>(0.0275f, 1));
					extraItems.Add(573 /* Hematite */,							new KeyValuePair<float, int>(0.0275f, 1));
					extraItems.Add(560 /* Ocean Stone */,						new KeyValuePair<float, int>(0.0275f, 1));
					extraItems.Add(749 /* Omni Geode */,						new KeyValuePair<float, int>(0.29f, extraPieces / 2));
					extraItems.Add(82  /* Fire Quartz */,						new KeyValuePair<float, int>(0.05f, 1));
					extraItems.Add(114 /* Ancient Seed */,						new KeyValuePair<float, int>(0.01f, 1));
					extraItems.Add(StardewValley.Object.prismaticShardIndex,	new KeyValuePair<float, int>(0.01f, 1));
					extraItems.Add(692 /* Lead Bobber */,						new KeyValuePair<float, int>(0.025f, 1));
					extraItems.Add(770 /* Mixed Seeds */,						new KeyValuePair<float, int>(0.05f, extraPieces));
					break;
				}
			}

			List<Item> items = new List<Item>();
			int whichOre = StardewValley.Object.copper;
			int whichExtra = -1;
			double roll = r.NextDouble() - (double)(int)who.luckLevel * 0.001 - who.DailyLuck;
			if (roll < iridiumChance)
			{
				whichOre = StardewValley.Object.iridium;
			}
			else if (roll < iridiumChance + radioactiveChance)
			{
				whichOre = 909; // Radioactive ore.
				orePieces--;
			}
			else if (roll < radioactiveChance + iridiumChance + goldChance)
			{
				whichOre = StardewValley.Object.gold;
			}
			else if (roll < radioactiveChance + iridiumChance + goldChance + ironChance)
			{
				whichOre = StardewValley.Object.iron;
			}

			roll = r.NextDouble() - who.DailyLuck;
			if (roll < extraChance)
			{
				roll = r.NextDouble() - who.DailyLuck;
				whichExtra = StardewValley.Object.coal;

				double current = 0; 
				foreach (var item in extraItems)
				{
					current += item.Value.Key;
					if (roll < current)
					{
						whichExtra = item.Key;
						extraPieces = item.Value.Value;
						break;
					}
				}

				if (roll < (double)who.LuckLevel * 0.002)
				{
					// Lucky ring
					items.Add(new Ring(859));
				}
			}

			items.Add(new StardewValley.Object(whichOre, orePieces));
			if (whichExtra != -1)
			{
				items.Add(new StardewValley.Object(whichExtra, extraPieces));
			}

			// Now add special items.
			bool gotSpecial = false;
			if (location is IslandNorth && (bool)(Game1.getLocationFromName("IslandNorth") as IslandNorth).bridgeFixed && r.NextDouble() < 0.2)
			{
				// Fossilized tail
				items.Add(new StardewValley.Object(822, 1));
				gotSpecial = true;
			}
			else if (location is IslandLocation && r.NextDouble() < 0.2)
			{
				// Taro tuber
				items.Add(new StardewValley.Object(831, r.Next(2, 6)));
				gotSpecial = true;
			}

			if (__instance.UpgradeLevel > 1)
			{
				if ((location is Beach || location is BeachNightMarket || (location is Farm && Game1.whichFarm == Farm.beach_layout)) && r.NextDouble() < 0.1)
				{
					// Dried starfish
					items.Add(new StardewValley.Object(116, 1));
					gotSpecial = true;
				}
			}

			if (__instance.UpgradeLevel > 2)
			{
				if (location is Woods && r.NextDouble() < 0.1)
				{
					// Elvish Jewelry
					items.Add(new StardewValley.Object(104, 1));
					gotSpecial = true;
				}
			}

			if (__instance.UpgradeLevel > 3)
			{
				if (location is Town && r.NextDouble() < 0.08)
				{
					// Goggles
					items.Add(new StardewValley.Objects.Hat(89));
					gotSpecial = true;
				}

				// Only get weather-based items if you haven't already gotten a location-based item.
				if (!gotSpecial)
				{
					if (Game1.IsRainingHere() && r.NextDouble() < 0.08)
					{
						// Pearl
						items.Add(new StardewValley.Object(797, 1));
					}
					else if (Game1.IsLightningHere() && r.NextDouble() < 0.1)
					{
						// Immunity Band / Thorns Ring
						items.Add(new StardewValley.Object(r.NextDouble() < 0.5 ? 887 : 839, 1));
					}
				}
			}
			__result = items;
			return false;
		}
	}
}