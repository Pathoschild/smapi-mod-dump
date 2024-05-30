/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Leroymilo/FurnitureFramework
**
*************************************************/

using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley.GameData.Shops;
using StardewValley.Objects;

namespace FurnitureFramework
{

	class FurniturePack
	{
		const int FORMAT = 2;

		static Dictionary<string, FurniturePack> packs = new();
		static Dictionary<string, string> type_ids = new();


		string UID;
		IContentPack content_pack;
		Dictionary<string, FurnitureType> types = new();
		Dictionary<string, List<string>> shops = new();
		bool is_valid = false;


		#region Loading

		private bool check_format(int format)
		{
			switch (format)
			{
				case > FORMAT:
				case < 1:
					ModEntry.log($"Invalid Format: {format}, skipping Furniture Pack.", LogLevel.Error);
					return false;
				case < FORMAT:
					ModEntry.log($"Format {format} is outdated, skipping Furniture Pack.", LogLevel.Error);
					ModEntry.log("If you are a user, wait for an update for this Furniture Pack,", LogLevel.Info);
					ModEntry.log($"or use a version of the Furniture Framework starting with {format}.", LogLevel.Info);
					ModEntry.log("If you are the author, check the Format changeModEntry.logs in the documentation to update your Pack.", LogLevel.Info);
					return false;
				case FORMAT: return true;
			}
		}

		private void load()
		{
			ModEntry.log($"Loading Furniture Pack {UID}...");

			JObject data;
			try
			{
				data = content_pack.ModContent.Load<JObject>("content.json");
			}
			catch (ContentLoadException ex)
			{
				ModEntry.log($"Could not load content.json for {UID}:\n{ex}", LogLevel.Error);
				return;
			}

			JToken? format_token = data.GetValue("Format");
			if (format_token is null || format_token.Type != JTokenType.Integer)
			{
				ModEntry.log("Missing or invalid Format, skipping Furniture Pack.", LogLevel.Error);
				return;
			}
			
			int format = (int)format_token;
			if(!check_format(format)) return;
			
			JToken? fs_token = data.GetValue("Furniture");
			if (fs_token is not JObject fs_object)
			{
				ModEntry.log("Missing or invalid \"Furniture\" field in content.json, skipping Furniture Pack.", LogLevel.Error);
				return;
			}

			List<FurnitureType> read_types = new();
			foreach((string key, JToken? f_data) in fs_object)
			{

				if (f_data is not JObject f_obj)
				{
					ModEntry.log($"No data for Furniture \"{key}\", skipping entry.", LogLevel.Warn);
					continue;
				}

				try
				{
					FurnitureType.make_furniture(
						content_pack, key,
						f_obj,
						read_types
					);
				}
				catch (Exception ex)
				{
					ModEntry.log(ex.ToString(), LogLevel.Error);
					ModEntry.log($"Failed to load data for Furniture \"{key}\", skipping entry.", LogLevel.Warn);
					continue;
				}
			}

			types.Clear();
			shops.Clear();
			foreach (FurnitureType type in read_types)
			{
				if (type_ids.ContainsKey(type.id))
				{
					ModEntry.log($"Duplicate Furniture: {type.id}, skipping Furniture.", LogLevel.Warn);
					continue;
				}

				types[type.id] = type;
				type_ids[type.id] = UID;

				if (type.shop_id != null)
				{
					if (!shops.ContainsKey(type.shop_id))
					{
						shops[type.shop_id] = new();
					}
				}

				foreach (string shop_id in type.shops)
				{
					if (!shops.ContainsKey(shop_id))
					{
						shops[shop_id] = new();
					}

					shops[shop_id].Add(type.id);
				}
			}

			is_valid = true;
		}

		private FurniturePack(IContentPack pack)
		{
			UID = pack.Manifest.UniqueID;
			content_pack = pack;

			load();
		}

		public static void load_pack(IContentPack pack)
		{
			string UID = pack.Manifest.UniqueID;

			if (packs.ContainsKey(UID))
			{
				ModEntry.log($"Furniture Pack {UID} is already loaded, loading canceled.", LogLevel.Warn);
				return;
			}

			FurniturePack new_pack = new(pack);
			if (!new_pack.is_valid) return;
			packs[new_pack.UID] = new_pack;
		}

		#endregion

		#region Reloading

		private void clear_types()
		{
			foreach (string type_id in types.Keys)
			{
				if (type_ids[type_id] == UID)
					type_ids.Remove(type_id);
			}
		}

		public static void reload_pack(string command, string[] args)
		{
			if (args.Count() == 0)
			{
				ModEntry.log("No ModID given.", LogLevel.Warn);
				return;
			}
			string UID = args[0];
		
			bool found = false;

			if (packs.TryGetValue(UID, out FurniturePack? pack))
			{
				found = true;

				pack.clear_types();
				pack.load();
			}

			else
			{
				ModEntry.log($"{UID} was not found in loaded packs, checking for unloaded packs...");

				foreach (IContentPack content_pack in ModEntry.get_helper().ContentPacks.GetOwned())
				{
					if (content_pack.Manifest.UniqueID == UID)
					{
						pack = new(content_pack);
						packs[UID] = pack;
						found = true;
						break;
					}
				}
			}

			if (!found)
			{
				ModEntry.log($"Could not find Furniture Pack {UID}.", LogLevel.Warn);
				return;
			}


			if (pack is not null && pack.is_valid)
			{
				ModEntry.log($"Pack {UID} successfully loaded!");
			}
			else
			{
				ModEntry.log($"Error while reloading pack {UID}, it will be removed to avoid errors.", LogLevel.Warn);
				packs.Remove(UID);
			}
			
			IGameContentHelper helper = ModEntry.get_helper().GameContent;
			helper.InvalidateCache("Data/Furniture");
			helper.InvalidateCache("Data/Shops");
		}

		#endregion

		#region Getters

		private bool try_get_type_pack(string f_id, [MaybeNullWhen(false)] ref FurnitureType? type)
		{
			if (!types.TryGetValue(f_id, out FurnitureType? found_type))
			{
				type = null;
				return false;
			}

			type = found_type;
			return true;
		}

		public static bool try_get_type(string f_id, [MaybeNullWhen(false)] out FurnitureType type)
		{
			type = null;

			if (!type_ids.TryGetValue(f_id, out string? UID))
				return false;

			return packs[UID].try_get_type_pack(f_id, ref type);
		}

		public static bool try_get_type(Furniture furniture, [MaybeNullWhen(false)] out FurnitureType type)
		{
			return try_get_type(furniture.ItemId, out type);
		}

		#endregion

		#region Asset Requests

		private void add_data_furniture(IDictionary<string, string> editor)
		{
			foreach ((string id, FurnitureType f) in types)
			{
				editor[id] = f.get_string_data();
			}
		}

		public static void edit_data_furniture(IAssetData asset)
		{
			var editor = asset.AsDictionary<string, string>().Data;

			foreach (FurniturePack pack in packs.Values)
				pack.add_data_furniture(editor);
		}

		private static bool has_shop_item(ShopData shop_data, string f_id)
		{
			foreach (ShopItemData shop_item_data in shop_data.Items)
			{
				if (shop_item_data.ItemId == $"(F){f_id}")
					return true;
			}
			return false;
		}

		private void add_data_shop(IDictionary<string, ShopData> editor)
		{
			foreach ((string shop_id, List<string> f_ids) in shops)
			{
				if (!editor.ContainsKey(shop_id))
				{
					ShopData catalogue_shop_data = new()
					{
						CustomFields = new Dictionary<string, string>() {
							{"HappyHomeDesigner/Catalogue", "true"}
						},
						Owners = new List<ShopOwnerData>() { 
							new() { Name = "AnyOrNone" }
						}
					};
					editor[shop_id] = catalogue_shop_data;
				}

				foreach (string f_id in f_ids)
				{
					if (!has_shop_item(editor[shop_id], f_id))
					{
						ShopItemData shop_item_data = new()
						{
							Id = f_id,
							ItemId = $"(F){f_id}",
							// Price = types[f_id].price
						};

						editor[shop_id].Items.Add(shop_item_data);
					}
				}
			}
		}

		public static void edit_data_shop(IAssetData asset)
		{
			var editor = asset.AsDictionary<string, ShopData>().Data;

			foreach (FurniturePack pack in packs.Values)
				pack.add_data_shop(editor);
		}

		#endregion
	}
}