/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.GameData.Objects;
using StardewValley.GameData.Powers;
using StardewValley.GameData.Shops;
using StardewValley.GameData.Tools;
using System.Collections.Generic;

namespace HappyHomeDesigner.Framework
{
	internal class AssetManager
	{
		private const string MOD_ID = ModEntry.MOD_ID;

		public const string CATALOGUE_ID = MOD_ID + "_Catalogue";
		public const string COLLECTORS_ID = MOD_ID + "_CollectorsCatalogue";
		public const string DELUXE_ID = MOD_ID + "_DeluxeCatalogue";
		public const string CARD_ID = MOD_ID + "_MembershipCard";
		public const string CARD_MAIL = MOD_ID + "_CardMail";
		public const string CARD_FLAG = MOD_ID + "_IsCollectorMember";
		public const string PORTABLE_ID = MOD_ID + "_HandCatalogue";
		public const string FAIRY_MAIL = MOD_ID + "_FairyMail";

		public const string TEXTURE_PATH = "Mods/" + MOD_ID + "/Catalogue";
		public const string UI_PATH = "Mods/" + MOD_ID + "/UI";
		public const string MAIL_BG = "Mods/" + MOD_ID + "/Mail";

		private static string whichUI = "ui";
		private static ITranslationHelper i18n;

		private static readonly string[] RareCatalogueShops = 
			{ "JunimoFurnitureCatalogue", "TrashFurnitureCatalogue", "RetroFurnitureCatalogue", "WizardFurnitureCatalogue", "JojaFurnitureCatalogue" };

		public static void Init(IModHelper helper)
		{
			whichUI =

				helper.ModRegistry.IsLoaded("Maraluna.OvergrownFloweryInterface") ?
				"ui_overgrown" : // overgrown flowery

				helper.ModRegistry.IsLoaded("ManaKirel.VintageInterface2") ?
				"ui_vintage" : // vintage v2

				"ui"; // vanilla

			i18n = helper.Translation;
			helper.Events.Content.AssetRequested += ProvideData;
		}

		public static void ProvideData(object sender, AssetRequestedEventArgs e)
		{
			var name = e.NameWithoutLocale;

			if (name.IsEquivalentTo(UI_PATH))
				e.LoadFromModFile<Texture2D>($"assets/{whichUI}.png", AssetLoadPriority.Low);

			else if (name.IsEquivalentTo(TEXTURE_PATH))
				e.LoadFromModFile<Texture2D>("assets/catalog.png", AssetLoadPriority.Low);

			else if (name.IsEquivalentTo(MAIL_BG))
				e.LoadFromModFile<Texture2D>("assets/mail.png", AssetLoadPriority.Low);

			else if (name.IsEquivalentTo("Data/Shops"))
				e.Edit(TagShops, AssetEditPriority.Default);

			else if (!ModEntry.config.ClientMode)
			{
				if (name.IsEquivalentTo("Data/Furniture"))
					e.Edit(AddCatalogues, AssetEditPriority.Early);

				else if (name.IsEquivalentTo("Data/Powers"))
					e.Edit(AddCardPower, AssetEditPriority.Early);

				else if (name.IsEquivalentTo("Data/Mail"))
					e.Edit(AddMail, AssetEditPriority.Early);

				else if (name.IsEquivalentTo("Data/Objects"))
					e.Edit(AddCardItem, AssetEditPriority.Early);

				else if (name.IsEquivalentTo("Data/Tools"))
					e.Edit(AddHandCatalogue, AssetEditPriority.Early);
			}
		}

		private static void AddHandCatalogue(IAssetData asset)
		{
			if (asset.Data is Dictionary<string, ToolData> data)
			{
				data.TryAdd(
					PORTABLE_ID,
					new()
					{
						Name = "Magic Catalogue",
						DisplayName = i18n.Get("item.portable.name"),
						Description = i18n.Get("item.portable.desc"),
						Texture = TEXTURE_PATH,
						SpriteIndex = 11,
						ClassName = "GenericTool",
						SetProperties = new()
						{
							{nameof(Tool.InstantUse), "True"}
						}
					}
				);
			}
		}

		private static void AddCardItem(IAssetData asset)
		{
			if (asset.Data is Dictionary<string, ObjectData> data)
			{
				data.TryAdd(
					CARD_ID,
					new()
					{
						Name = "Collector's Card",
						DisplayName = i18n.Get("item.card.name"),
						Texture = TEXTURE_PATH,
						SpriteIndex = 8,
						Category = Object.trinketCategory,
						ExcludeFromFishingCollection = true,
						ExcludeFromRandomSale = true,
						ExcludeFromShippingCollection = true
					}
				);
			}
		}

		private static void AddMail(IAssetData asset)
		{
			if (asset.Data is Dictionary<string, string> data)
			{
				data.TryAdd(CARD_MAIL,
					$"[letterbg {MAIL_BG} 0]^{i18n.Get("mail.collectorAcceptance.text")}" + 
					$" ^ ^\t\t-Esme Blackbriar%item id (O){CARD_ID} 1 %%[#]{i18n.Get("mail.collectorAcceptance.name")}"
				);

				data.TryAdd(FAIRY_MAIL,
					$"[letterbg 2]{i18n.Get("mail.fairyDust.text")}[#]{i18n.Get("mail.fairyDust.name")}"
				);
			}
		}

		private static void AddCardPower(IAssetData asset)
		{
			if (asset.Data is Dictionary<string, PowersData> data)
			{
				data.TryAdd(
					CARD_ID, new()
					{
						DisplayName = i18n.Get("item.card.name"),
						Description = i18n.Get("item.card.desc"),
						TexturePath = TEXTURE_PATH,
						TexturePosition = new(32, 32),
						UnlockedCondition = "PLAYER_HAS_MAIL Current " + CARD_FLAG
					}
				);
			}
		}

		private static void TagShops(IAssetData asset)
		{
			if (asset.Data is Dictionary<string, ShopData> data)
			{
				for (int i = 0; i < RareCatalogueShops.Length; i++)
					if (data.TryGetValue(RareCatalogueShops[i], out var shop))
						(shop.CustomFields ??= new())["HappyHomeDesigner/Catalogue"] = "True";

				if (data.TryGetValue("Carpenter", out var carpenter))
					carpenter.Items.Add(new() { 
						Id = COLLECTORS_ID,
						ItemId = "(F)" + COLLECTORS_ID,
						Condition = "PLAYER_HAS_MAIL Current " + CARD_FLAG
					});

				#if DEBUG

				if (data.TryGetValue("Catalogue", out var catalogue))
				{
					catalogue.Items.Add(new() {
						Id = "DEBUG_HAPPYHOME_HOUSEPLANT",
						ItemId = "(BC)7",
						Price = 0
					});

					catalogue.Items.Add(new() {
						Id = "DEBUG_HAPPYHOME_STONE",
						ItemId = "(O)390",
						Price = 0
					});
				}

				#endif
			}
		}

		private static void AddCatalogues(IAssetData asset)
		{
			if (asset.Data is Dictionary<string, string> data)
			{
				var entries = ModEntry.helper.ModContent.Load<Dictionary<string, string>>("assets/furniture.json");

				data.TryAdd(CATALOGUE_ID, GetEntry(entries, "furniture", "Catalogue"));
				data.TryAdd(COLLECTORS_ID, GetEntry(entries, "furniture", "CollectorsCatalogue"));
				data.TryAdd(DELUXE_ID, GetEntry(entries, "furniture", "DeluxeCatalogue"));
			}
		}

		private static string GetEntry(IDictionary<string, string> data, string prefix, string name)
		{
			return string.Format(
				data[name],
				i18n.Get($"{prefix}.{name}.name"),
				"Mods\\" + MOD_ID + "\\Catalogue"
			);
		}
	}
}
