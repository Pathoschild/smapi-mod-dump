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
using GenericModConfigMenu;
using Microsoft.Xna.Framework.Content;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley.GameData.Shops;
using StardewValley.Objects;

namespace FurnitureFramework
{

	class FurniturePack
	{
		const int FORMAT = 2;
		const string DEFAULT_PATH = "content.json";
		const string CONFIG_PATH = "config.json";


		private class IncludedPack
		{
			public readonly FurniturePack pack;
			string name;
			string? description;
			public bool enabled;
			public readonly bool default_enabled = true;

			public readonly bool is_valid = false;

			string page_id;

			#region Parsing

			public IncludedPack(IContentPack pack, string name, JObject include_obj)
			{
				this.name = name;

				JToken? token = include_obj.GetValue("Path");
				if (token is null || token.Type != JTokenType.String)
				{
					ModEntry.log($"Missing or invalid include path, skipping include {name}.", LogLevel.Warn);
					return;
				}
				string path = token.ToString();

				this.pack = new(pack, path);
				if (!this.pack.is_valid)
				{
					ModEntry.log($"Error when loading included Furniture at {path}, skipping include {name}.", LogLevel.Warn);
					return;
				}

				is_valid = true;

				token = include_obj.GetValue("Description");
				if (token is not null && token.Type == JTokenType.String)
				{
					description = token.ToString();
				}

				token = include_obj.GetValue("Enabled");
				if (token is not null && token.Type == JTokenType.Boolean)
				{
					default_enabled = (bool)token;
				}

				enabled = default_enabled;

				page_id = $"{pack.Manifest}.{name}";
			}

			#endregion

			#region Config

			public void add_config(IGenericModConfigMenuApi config_menu)
			{
				Func<string>? tooltip = null;
				if (description is not null) tooltip = () => description;

				config_menu.AddBoolOption(
					mod: pack.content_pack.Manifest,
					getValue: () => enabled,
					setValue: (value) => {
						enabled = value;
						update_after_config_change();
					},
					name: () => name,
					tooltip: tooltip
				);

				if (!pack.has_config()) return;
				
				config_menu.AddPageLink(
					mod: pack.content_pack.Manifest,
					pageId: page_id,
					text: () => $"{name} Config",
					tooltip: () => $"Additional config options for the {name} part of this Furniture Pack."
				);
			}

			public void add_config_page(IGenericModConfigMenuApi config_menu)
			{
				if (!pack.has_config()) return;

				config_menu.AddPage(
					mod: pack.content_pack.Manifest,
					pageId: page_id,
					pageTitle: () => $"{name} Config"
				);

				pack.add_config(config_menu);
			}

			public void read_config(JObject config_data, string prefix)
			{
				string key = $"{prefix}.{name}";

				JToken? config_token = config_data.GetValue(key);
				if (config_token is not null && config_token.Type == JTokenType.Boolean)
					enabled = (bool)config_token;
				
				pack.read_config(config_data, key);
			}

			public void save_config(JObject data, string prefix)
			{
				string key = $"{prefix}.{name}";

				data.Add(key, new JValue(enabled));
				
				pack.save_config(data, key);
			}

			#endregion
		}


		static Dictionary<string, FurniturePack> packs = new();
		static Dictionary<string, string> type_ids = new();


		string UID;
		IContentPack content_pack;
		Dictionary<string, FurnitureType> types = new();
		Dictionary<string, List<string>> shops = new();
		List<IncludedPack> included_packs = new();
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

		private void load(string path = DEFAULT_PATH)
		{
			JObject data;
			try
			{
				data = content_pack.ModContent.Load<JObject>(path);
			}
			catch (ContentLoadException ex)
			{
				ModEntry.log($"Could not load {path} for {UID}:\n{ex}", LogLevel.Error);
				return;
			}

			#region Format

			if (path == DEFAULT_PATH)
			{
				JToken? format_token = data.GetValue("Format");
				if (format_token is null || format_token.Type != JTokenType.Integer)
				{
					ModEntry.log("Missing or invalid Format, skipping Furniture Pack.", LogLevel.Error);
					return;
				}
				
				int format = (int)format_token;
				if(!check_format(format)) return;
			}

			#endregion
			
			#region Furniture

			JToken? fs_token = data.GetValue("Furniture");
			if (fs_token is JObject fs_object)
			{
				read_furniture(fs_object);
			}

			#endregion

			#region Includes

			JToken? includes_token = data.GetValue("Included");
			if (includes_token is JObject includes_obj)
			{
				included_packs.Clear();
				foreach (JProperty property in includes_obj.Properties())
				{
					if (property.Value is not JObject include_obj)
						continue;
					
					IncludedPack included_pack = new(content_pack, property.Name, include_obj);
					if (included_pack.is_valid)
						included_packs.Add(included_pack);
				}
			}

			#endregion

			if (types.Count == 0 && included_packs.Count == 0)
			{
				ModEntry.log("This Furniture Pack is empty!", LogLevel.Warn);
				return;
			}

			#region Config

			if (path == DEFAULT_PATH)
			{
				JObject? config_data = null;
				try
				{
					config_data = content_pack.ModContent.Load<JObject>(CONFIG_PATH);
				}
				catch (ContentLoadException)
				{
					save_config();
				}

				if (config_data is not null)
					read_config(config_data);
			}

			#endregion

			is_valid = true;
		}

		private void read_furniture(JObject fs_object)
		{
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
				if (type_ids.ContainsKey(type.id) && type_ids[type.id] != UID)
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
		}

		private FurniturePack(IContentPack pack, string path = DEFAULT_PATH)
		{
			UID = pack.Manifest.UniqueID;
			content_pack = pack;

			load(path);
		}

		public static void load_pack(IContentPack pack)
		{
			string UID = pack.Manifest.UniqueID;

			if (packs.ContainsKey(UID))
			{
				ModEntry.log($"Furniture Pack {UID} is already loaded, loading canceled.", LogLevel.Warn);
				return;
			}

			ModEntry.log($"Loading Furniture Pack {UID}...");
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

			foreach (IncludedPack sub_pack in included_packs)
			{
				sub_pack.pack.clear_types();
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
			bool found = false;

			// prioritize included files to overload definition
			foreach (IncludedPack sub_pack in included_packs)
			{
				if (!sub_pack.enabled) continue;
				found |= sub_pack.pack.try_get_type_pack(f_id, ref type);
				if (found) break;
			}

			if (!found)
			{
				found |= types.TryGetValue(f_id, out type);
			}

			return found;
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

			foreach (IncludedPack sub_pack in included_packs)
			{
				if (sub_pack.enabled) sub_pack.pack.add_data_furniture(editor);
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

			foreach (IncludedPack sub_pack in included_packs)
			{
				if (sub_pack.enabled) sub_pack.pack.add_data_shop(editor);
			}
		}

		public static void edit_data_shop(IAssetData asset)
		{
			var editor = asset.AsDictionary<string, ShopData>().Data;

			foreach (FurniturePack pack in packs.Values)
				pack.add_data_shop(editor);
		}

		#endregion
	
		#region Config

		private bool has_config()
		{
			return included_packs.Count > 0;
		}

		private void reset_config()
		{
			foreach (IncludedPack sub_pack in included_packs)
			{
				sub_pack.enabled = sub_pack.default_enabled;
				sub_pack.pack.reset_config();
			}
		}

		private void read_config(JObject config_data, string? prefix = null)
		{
			prefix ??= "";

			foreach (IncludedPack sub_pack in included_packs)
			{
				sub_pack.read_config(config_data, prefix);
			}
		}

		private void save_config()
		{
			JObject data = new();

			save_config(data, "");

			string path = Path.Combine(content_pack.DirectoryPath, CONFIG_PATH);
			File.WriteAllText(path, data.ToString());
		}

		private void save_config(JObject data, string prefix)
		{
			foreach (IncludedPack sub_pack in included_packs)
			{
				sub_pack.save_config(data, prefix);
			}
		}

		private void register_pack_config(IGenericModConfigMenuApi config_menu)
		{
			if (!has_config()) return;

			config_menu.Register(
				mod: content_pack.Manifest,
				reset: reset_config,
				save: () => save_config()
			);

			add_config(config_menu);
		}

		private void add_config(IGenericModConfigMenuApi config_menu)
		{
			if (!has_config()) return;

			foreach (IncludedPack sub_pack in included_packs)
			{
				sub_pack.add_config(config_menu);
			}

			foreach (IncludedPack sub_pack in included_packs)
			{
				sub_pack.add_config_page(config_menu);
			}
		}

		public static void register_config(IGenericModConfigMenuApi config_menu)
		{
			foreach (FurniturePack pack in packs.Values)
			{
				pack.register_pack_config(config_menu);
			}
		}

		private static void update_after_config_change()
		{
			IGameContentHelper helper = ModEntry.get_helper().GameContent;
			helper.InvalidateCache("Data/Furniture");
			helper.InvalidateCache("Data/Shops");
		}

		#endregion
	}
}