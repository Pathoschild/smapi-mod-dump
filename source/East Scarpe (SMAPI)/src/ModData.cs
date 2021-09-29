/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/eastscarpe
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using System.Collections.Generic;

namespace EastScarp
{
#pragma warning disable IDE1006

	public abstract class AreaSpecific
	{
		public string Location { get; set; }
		public Rectangle Area { get; set; } = new Rectangle (0, 0, -1, -1);

		public Rectangle adjustArea (GameLocation location)
		{
			Rectangle area = Area;
			if (area.Width == -1)
				area.Width = location.map.Layers[0]?.LayerWidth ?? 0;
			if (area.Height == -1)
				area.Height = location.map.Layers[0]?.LayerHeight ?? 0;
			return area;
		}

		public bool checkArea (GameLocation location, Vector2 tilePosition)
		{
			return checkArea (location, Utility.Vector2ToPoint (tilePosition));
		}

		public bool checkArea (GameLocation location, Point tilePosition)
		{
			return location.Name == Location &&
				adjustArea (location).Contains (tilePosition);
		}
	}

	public class Conditions : List<string>
	{
		public bool check ()
		{
			if (Count == 0)
				return true;
			else
				return ModEntry.Instance.conditionsChecker.CheckConditions (ToArray ());
		}
	}

	public class AmbientSound : AreaSpecific
	{
		public Conditions Conditions { get; set; } = new ();
		public float Chance { get; set; } = 0f;
		public string Sound { get; set; }
	}

	public class CrabPotCatch : AreaSpecific
	{
		public Conditions Conditions { get; set; } = new ();
		public int FishingArea { get; set; } = -1;
		public bool OceanCatches { get; set; } = false;
		public float ExtraTrashChance { get; set; } = 0f;
	}

	public enum CritterType
	{
		BrownBird,
		BlueBird,
		SpecialBlueBird,
		SpecialRedBird,
		Butterfly,
		IslandButterfly,
		CalderaMonkey,
		Cloud,
		Crab,
		Crow,
		Firefly,
		Frog,
		OverheadParrot,
		Owl,
		Rabbit,
		Seagull,
		Squirrel,
		// Woodpecker requires Tree
	}

	public class CritterSpawn : AreaSpecific
	{
		public Conditions Conditions { get; set; } = new ();
		public float ChanceOnEntry { get; set; } = 0f;
		public float ChanceOnTick { get; set; } = 0f;
		public int MinClusters { get; set; } = 1;
		public int MaxClusters { get; set; } = 1;
		public int MinPerCluster { get; set; } = 1;
		public int MaxPerCluster { get; set; } = 1;
		public float ChanceOnWater { get; set; } = 0;
		public CritterType Type { get; set; }
	}

	public class FishingArea : AreaSpecific
	{
		public Conditions Conditions { get; set; } = new ();
		public int Index { get; set; }
	}

	public class ObeliskWarp
	{
		public string Name { get; set; }
		public string Location { get; set; }
		public int X { get; set; }
		public int Y { get; set; }
	}

	public class RainWateringArea : AreaSpecific
	{ }

	public class SeaMonsterSpawn : AreaSpecific
	{
		public Conditions Conditions { get; set; } = new ();
		public float Chance { get; set; } = 0f;
	}

	public class WaterColor
	{
		public Conditions Conditions { get; set; } = new ();
		public string Location { get; set; }
		public Color Color { get; set; }
	}

	public class WaterEffect : AreaSpecific
	{
		public Conditions Conditions { get; set; } = new ();
		public bool Apply { get; set; }
	}

	public class WinterGrass : AreaSpecific
	{ }

	public class ModData
	{
		public List<AmbientSound> AmbientSounds { get; set; } = new ();

		public List<CrabPotCatch> CrabPotCatches { get; set; } = new ();

		public List<CritterSpawn> CritterSpawns { get; set; } = new ();

		public List<FishingArea> FishingAreas { get; set; } = new ();

		public List<string> FruitTreeLocations { get; set; } = new ();

		public ObeliskWarp ObeliskWarp { get; set; } = new ();

		public List<RainWateringArea> RainWateringAreas { get; set; } = new ();

		public List<SeaMonsterSpawn> SeaMonsterSpawns { get; set; } = new ();

		public List<WaterColor> WaterColors { get; set; } = new ();

		public List<WaterEffect> WaterEffects { get; set; } = new ();

		public List<WinterGrass> WinterGrasses { get; set; } = new ();
	}

#pragma warning restore IDE1006
}
