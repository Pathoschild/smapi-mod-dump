/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/MZG
**
*************************************************/

using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.GameData.Buildings;
using StardewValley.GameData.Shops;
using StardewValley.Objects;

namespace ModularZenGarden {

	using Dict = Dictionary<string, object>;

	struct BorderPart
	{
		public Texture2D texture;
		public Point tile;

		public BorderPart(Texture2D texture, Point tile)
		{
			this.texture = texture;
			this.tile = tile;
		}
	}
	
	class GardenType
	{
		public readonly string name;
		private readonly string? author = null;
		public readonly Point size;
		private readonly string dim;
		public readonly Dictionary<string, Texture2D> bases = new();
		public readonly Dictionary<string, Texture2D> features = new();

		private static readonly Dictionary<string, GardenType> types = new();
		private static readonly Dictionary<Point, List<BorderPart>> default_borders = new();

		public static bool is_garden<T>(T garden_source) where T : notnull
		{
			try
			{
				get_type(garden_source);
				return true;
			}
			catch (InvalidDataException)
			{
				return false;
			}
		}

		public static GardenType get_type(string full_name)
		{
			if (full_name.StartsWith("MZG_B"))
			{
				// Handles the case of a building's skin texture
				full_name = full_name.Remove(full_name.LastIndexOf('@'));
				return types[full_name.Remove(0, "MZG_B ".Length)];
			}

			else if (full_name.StartsWith("MZG"))
			{
				// Handles the case of a furniture's garden type
				return types[full_name.Remove(0, "MZG ".Length)];
			}

			return full_name switch
			{
				// Handles the case of a building's garden type
				"Earth Obelisk" => types["stones"],
				"Water Obelisk" => types["tree"],
				"Desert Obelisk" => types["lantern"],
				"Island Obelisk" => types["parrot"],
				// Excludes the Island Obelisk for now
				_ => throw new InvalidDataException("This type name has no garden associated."),
			};
		}

		public static GardenType get_type<T>(T garden_source) where T : notnull
		{
			if (garden_source is Furniture)
				return get_type(((Furniture)(object)garden_source).ItemId);
			else if (garden_source is Building)
				return get_type(((Building)(object)garden_source).buildingType.Value);
			else throw new ArgumentException("The given object is neither a Furniture or a Building.");
		}

		public static void load_types(IModHelper helper)
		{

			// Hardcoded Garden Catalogue
			new GardenType(
				"catalogue",
					new Dict() {
					{"width", 1L},
					{"height", 1L},
					{"use_default_base", true}
				},
				helper
			);

			// Making blank types
			for (long w = 1; w < 4; w++)
			{
				for (long h = 1; h < 4; h++)
				{
					Dictionary<string, object> type_data = new() {
						{"width", w},
						{"height", h},
						{"use_default_base", true},
						{"use_default_feature", true}
					};
					new GardenType($"empty {w}x{h}", type_data, helper);
				}
			}

			// Loading gardens from assets/types.json
			Dictionary<string, Dict> types_data =
				helper.ModContent.Load<Dictionary<string, Dict>>("assets/types.json");
			foreach ((string type_name, var type_data) in types_data)
			{
				try
				{
					new GardenType(
						type_name, type_data, helper
					);
				}
				catch (Exception ex)
				{
					Utils.log($"Could not load Garden type {type_name} : {ex}", LogLevel.Warn);
				}
			}
		}

		public GardenType(string type_name, Dict type_data, IModHelper helper)
		{
			types[type_name] = this;

			name = type_name;

			if (type_data.ContainsKey("author"))
				author = (string)type_data["author"];
			
			size = new(
				(int)(long)type_data["width"],
				(int)(long)type_data["height"]
			);
			if (size.X < 1 || size.Y < 1)
				throw new ArgumentException($"Size of type {type_name} is invalid.");
			// if (size.X > 1 && size.Y == 1)
			// 	throw new ArgumentException("Garden of 1 tall are not supported. Consider using multiple 1x1 gardens.");
			dim = $"{size.X}x{size.Y}";

			// loading textures of bases and features
			string path = $"assets/gardens/{name}/";
			foreach (string season in Utils.seasons)
			{
				if ((bool)type_data.GetValueOrDefault("use_default_base", false))
					bases[season] = SpriteManager.get_default_base(size, season);
				
				else
				{
					try
					{
						// Searching for seasonal variant
						bases[season] = helper.ModContent.Load<Texture2D>(
							path + $"base_{season}.png"
						);
					}
					catch (Microsoft.Xna.Framework.Content.ContentLoadException)
					{
						try
						{
							// Searching for general sprite
							bases[season] = helper.ModContent.Load<Texture2D>(
								path + $"base.png"
							);
						}
						catch (Microsoft.Xna.Framework.Content.ContentLoadException)
						{
							Utils.log(
								$"Sprite for {season} base of {type_name} was not found, fallback to default.",
								LogLevel.Warn
							);
							// Loading default sprite
							bases[season] = SpriteManager.get_default_base(size, season);
						}
					}
				}

				if ((bool)type_data.GetValueOrDefault("use_default_feature", false))
					features[season] = SpriteManager.get_default_feature(size);

				else
				{
					try
					{
						// Searching for seasonal variant
						features[season] = helper.ModContent.Load<Texture2D>(
							path + $"feature_{season}.png"
						);
					}
					catch (Microsoft.Xna.Framework.Content.ContentLoadException)
					{
						try
						{
							// Searching for general sprite
							features[season] = helper.ModContent.Load<Texture2D>(
								path + $"feature.png"
							);
						}
						catch (Microsoft.Xna.Framework.Content.ContentLoadException)
						{
							Utils.log(
								$"Sprite for {season} feature of {type_name} was not found, fallback to default.",
								LogLevel.Warn
							);
							// Loading default sprite
							features[season] = SpriteManager.get_default_feature(size);
						}
					}
				}
				
			}
		}

		private string get_string_data(string full_name)
		{
			// Building the string to patch into Content/Data/Furniture.xnb

			string display_name;
			if (name == "catalogue")
				display_name = "/Zen Garden Catalogue";
			else
			{
				display_name = $"/Zen Garden {dim}";
				if (author != null) display_name += $" by {author}";
			}

			string string_data = full_name;						// internal name
			string_data += "/other";							// type
			string_data += $"/{size.X} {2*size.Y}";				// texture size (probably unused)
			string_data += $"/{size.X} {size.Y}";				// collision size
			string_data += "/1";								// rotation
			string_data += "/2000";								// price for catalog
			string_data += "/2";								// placeable outdoors & indoors
			string_data += display_name;						// display name
			string_data += "/0";								// sprite index
			string_data += $"/{full_name}";						// texture path (to direct load overwrite)
			string_data += "/true";								// true to prevent showing in vanilla catalog
			string_data += "/MZG_furniture";					// context tag to appear in custom catalog

			return string_data;
		}

		public static void patch_furniture_data(IAssetData asset)
		{
			var editor = asset.AsDictionary<string, string>();
			foreach ((string type_name, GardenType type) in types)
			{
				string full_name = $"MZG {type_name}";
				string data = type.get_string_data(full_name);
				editor.Data[full_name] = data;
			}
		}

		public static void patch_shop_data(IAssetData asset)
		{
			// the catalogue item sold at Robin's
			ShopItemData carpenter_shop_item_data = new()
			{
				Id = "MZG_catalogue",
				ItemId = "(F)MZG catalogue",
				Price = 2000
			};

			// the catalogue shop itself
			ShopData catalogue_shop_data = new()
			{
				Items = new List<ShopItemData>() {
					new() {
						Price = 0,
						Id = "Default",
						ItemId = "ALL_ITEMS (F)",
						PerItemCondition = "ITEM_CONTEXT_TAG Target MZG_furniture"
					}
				},
				CustomFields = new Dictionary<string, string>() {
					{"HappyHomeDesigner/Catalogue", "true"}
				},
				Owners = new List<ShopOwnerData>() { 
					new() { Name = "AnyOrNone" }
				}
			};

			var data = asset.AsDictionary<string, ShopData>().Data;
			// Adding its shop data
			data["MZG_catalogue"] = catalogue_shop_data;
			// Adding the furniture to Robin's shop
			ShopData carpenter = data["Carpenter"];
			int f_cat_index = carpenter.Items.FindIndex(
				shop_item => {return shop_item.ItemId == "(F)1226";}
			);
			carpenter.Items.Insert(f_cat_index + 1, carpenter_shop_item_data);
			// right after the vanilla Furniture Catalogue so it looks nice
		}

		public static void patch_building_data(IAssetData asset)
		{
			var data = asset.AsDictionary<string, BuildingData>().Data;

			foreach ((string b_type, BuildingData b_data) in data)
			{
				GardenType type;
				try
				{
					type = get_type(b_type);
				}
				catch (InvalidDataException)
				{
					continue;
				}

				b_data.Size = type.size;
				b_data.Texture = $"MZG {type.name}";
				b_data.DrawShadow = false;
				b_data.SortTileOffset = type.size.Y;

				if (!GardenCache.buildings.ContainsKey(b_type)) continue;
				foreach (Point pos in GardenCache.buildings[b_type])
				{

					Garden garden = GardenCache.get_garden(type, pos)
						?? throw new NullReferenceException(
							$"No registered garden matched at {pos}."
						);
					string skin_id = garden.get_skin_id();

					b_data.Skins.Add(new BuildingSkin() {
						Id = skin_id,
						Name = $"Modular Zen Garden skin",
						Description = $"You shouldn't be able to see that",
						Texture = skin_id,
						// Condition = "FALSE"	// to avoid showing in shop
					});
				}
			}
		}

		public Texture2D get_base()
		{
			Texture2D base_ = bases[SDate.Now().SeasonKey];

			Texture2D texture = new(
				GameRunner.instance.GraphicsDevice,
				base_.Width,
				base_.Height
			);

			texture.CopyFromTexture(base_);

			return texture;
		}

		public void patch_default_texture(IAssetData asset)
		{
			// Patching the image used by default (item icon, unconnected)

			IAssetDataForImage editor = asset.AsImage();

			foreach (BorderPart border_part in get_default_border())
			{
				Rectangle target_area = new(
					border_part.tile.X * SpriteManager.tile_size.X,
					(border_part.tile.Y + size.Y - 1) * SpriteManager.tile_size.X,
					SpriteManager.tile_size.X, SpriteManager.tile_size.Y
				);

				editor.PatchImage(
					source: border_part.texture,
					targetArea: target_area,
					patchMode: PatchMode.Overlay
				);
			}

			editor.PatchImage(
				source: features[SDate.Now().SeasonKey],
				patchMode: PatchMode.Overlay
			);
		}

		public void patch_building(IAssetData asset, string sprite_name)
		{
			string[] coords = sprite_name.Split('@').Last().Split('x', 2);
			Point pos = new(int.Parse(coords[0]), int.Parse(coords[1]));

			Garden garden = GardenCache.get_garden(this, pos)
				?? throw new NullReferenceException(
					$"No registered garden matched at {pos}."
				);

			IAssetDataForImage editor = asset.AsImage();

			foreach (BorderPart border_part in garden.border_parts)
			{
				Rectangle target_area = new(
					border_part.tile.X * SpriteManager.tile_size.X,
					(border_part.tile.Y + size.Y - 1) * SpriteManager.tile_size.X,
					SpriteManager.tile_size.X, SpriteManager.tile_size.Y
				);

				editor.PatchImage(
					source: border_part.texture,
					targetArea: target_area,
					patchMode: PatchMode.Overlay
				);
			}

			editor.PatchImage(
				source: features[SDate.Now().SeasonKey],
				patchMode: PatchMode.Overlay
			);
		}

		public void set_contacts(Garden garden, Point origin, bool value)
		{
			for (int x = 0; x < size.X; x++)
			{
				for (int y = 0; y < size.Y; y++)
				{
					garden.set_contact(origin + new Point(x, y), value);
				}
			}
		}

		public void get_border(Func<Point, int> c_value_getter, List<BorderPart> border_parts)
		{
			border_parts.Clear();

			// 1x1
			if (size.X == 1 && size.Y == 1)
			{
				border_parts.Add(new(
					SpriteManager.get_border_part(
						size, "top", c_value_getter(new(0, -1))
					), Point.Zero
				));
				border_parts.Add(new(
					SpriteManager.get_border_part(
						size, "left", c_value_getter(new(-1, 0))
					), Point.Zero
				));
				border_parts.Add(new(
					SpriteManager.get_border_part(
						size, "right", c_value_getter(new(1, 0))
					), Point.Zero
				));
				border_parts.Add(new(
					SpriteManager.get_border_part(
						size, "bottom", c_value_getter(new(0, 1))
					), Point.Zero
				));
			}

			// 1xN
			else if (size.X == 1 && size.Y > 1)
			{
				Point top = Point.Zero;

				// top
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "top", c_value_getter(top + new Point(0, -1))
					), top
				));

				// top left
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "top_left", c_value_getter(top + new Point(-1, 0))
					), top
				));

				// top right
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "top_right", c_value_getter(top + new Point(1, 0))
					), top
				));

				// left & right
				for (int y = 1; y < size.Y-1; y++)
				{
					Point tile = new(0, y);

					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "left", c_value_getter(tile + new Point(-1, 0))
						), tile
					));
						
					tile.X = size.X-1;
					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "right", c_value_getter(tile + new Point(1, 0))
						), tile
					));
				}

				Point bottom = new(0, size.Y-1);

				// bottom left
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "bottom_left", c_value_getter(bottom + new Point(-1, 0))
					), bottom
				));

				// bottom right
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "bottom_right", c_value_getter(bottom + new Point(1, 0))
					), bottom
				));

				// bottom
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "bottom", c_value_getter(bottom + new Point(0, 1))
					), bottom
				));
			}

			// Nx1
			else if (size.X > 1 && size.Y == 1)
			{
				Point left = Point.Zero;

				// left top
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "left_top", c_value_getter(left + new Point(0, -1))
					), left
				));

				// left
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "left", c_value_getter(left + new Point(-1, 0))
					), left
				));

				// left bottom
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "left_bottom", c_value_getter(left + new Point(0, 1))
					), left
				));

				// top
				for (int x = 1; x < size.X-1; x++)
				{
					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "top", c_value_getter(new(x, -1))
						), new(x, 0)
					));
				}

				Point right = new(size.X-1, 0);

				// right top
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "right_top", c_value_getter(right + new Point(0, -1))
					), right
				));

				// right
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "right", c_value_getter(right + new Point(1, 0))
					), right
				));

				// right bottom
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "right_bottom", c_value_getter(right + new Point(0, 1))
					), right
				));

				// bottom
				for (int x = 1; x < size.X-1; x++)
				{
					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "bottom", c_value_getter(new(x, 1))
						), new(x, 0)
					));
				}
			}

			// NxN
			else if (size.X > 1 && size.Y > 1)
			{
				// top left
				Point tile = Point.Zero;
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "top_left", c_value_getter(tile)
					), tile
				));

				// top
				for (tile.X = 1; tile.X < size.X-1; tile.X++)
				{
					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "top", c_value_getter(tile)
						), tile
					));
				}

				// top right
				tile.X = size.X-1;
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "top_right", c_value_getter(tile)
					), tile
				));

				// left & right
				for (tile.Y = 1; tile.Y < size.Y-1; tile.Y++)
				{
					tile.X = 0;
					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "left", c_value_getter(tile)
						), tile
					));
						
					tile.X = size.X-1;
					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "right", c_value_getter(tile)
						), tile
					));
				}

				// bottom left
				tile.X = 0; tile.Y = size.Y-1;
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "bottom_left", c_value_getter(tile)
					), tile
				));

				// bottom
				for (tile.X = 1; tile.X < size.X-1; tile.X++)
				{
					border_parts.Add(new (
						SpriteManager.get_border_part(
							size, "bottom", c_value_getter(tile)
						), tile
					));
				}

				// bottom right
				tile.X = size.X-1;
				border_parts.Add(new (
					SpriteManager.get_border_part(
						size, "bottom_right", c_value_getter(tile)
					), tile
				));
			}

			else throw new Exception("Unsupported Garden Size.");
		}

		public List<BorderPart> get_default_border()
		{
			if (default_borders.ContainsKey(size))
				return default_borders[size];

			List<BorderPart> border_parts = new();

			get_border(tile => {return 0;}, border_parts);

			default_borders[size] = border_parts;
			return border_parts;
		}

		// override object.ToString
		public override string ToString()
		{
			return name;
		}

		// override object.Equals
		public override bool Equals(object? obj)
		{	
			if (obj == null || GetType() != obj.GetType())
			{
				return false;
			}

			return name == obj.ToString();
		}
		
		// override object.GetHashCode
		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}

}