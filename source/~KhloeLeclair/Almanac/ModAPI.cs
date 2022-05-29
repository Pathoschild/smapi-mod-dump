/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using Leclair.Stardew.Common;

using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Locations;

using Leclair.Stardew.Almanac.Models;

namespace Leclair.Stardew.Almanac {

	public interface IAlmanacAPI {

		/// <summary>
		/// The detected number of days in a month, used when drawing calendar
		/// pages. Some mods can change the length of a month, though Almanac
		/// itself doesn't have the ability.
		/// </summary>
		int DaysPerMonth { get; }

		#region Custom Pages

		/*void RegisterPage(
			IManifest manifest,
			string id,
			// State
			Func<IClickableMenu, bool> Enabled = null,
			Func<IClickableMenu, object> saveState = null,
			Action<IClickableMenu, object> loadState = null,

			// IAlmanacPage
			bool magicTheme = false,
			bool calendar = false,

			Action<IClickableMenu> onActivate = null,
			Action<IClickableMenu> onDeactivate = null,
			Action<IClickableMenu, WorldDate, WorldDate> onDateChange = null,

			Action<IClickableMenu> onUpdateComponents = null,
			Func<IClickableMenu, ClickableComponent> getComponents = null,

			Func<IClickableMenu, Buttons, bool> onGamePadButton = null,
			Func<IClickableMenu, Keys, bool> onKeyPress = null,
			Func<IClickableMenu, int, int, int, bool> onScroll = null,
			Func<IClickableMenu, int, int, bool, bool> onLeftClick = null,
			Func<IClickableMenu, int, int, bool, bool> onRightClick = null,
			Action<IClickableMenu, int, int, Action<string>, Action<Item>> onHover = null,
			Action<IClickableMenu, SpriteBatch> onDraw = null,

			// ITab
			int tabSort = 100,
			bool? tabMagic = null,
			Func<IClickableMenu, string> tabTooltip = null,
			Func<IClickableMenu, Texture2D> tabTexture = null,
			Func<IClickableMenu, Rectangle?> tabSource = null,
			Func<IClickableMenu, float?> tabScale = null,

			// ICalendar
			Func<IClickableMenu, bool> dimPastCells = null,
			Func<IClickableMenu, bool> highlightToday = null,
			Action<IClickableMenu, SpriteBatch, WorldDate, Rectangle> onDrawUnderCell = null,
			Action<IClickableMenu, SpriteBatch, WorldDate, Rectangle> onDrawOverCell = null,

			Func<IClickableMenu, int, int, WorldDate, Rectangle, bool> onCellLeftClick = null,
			Func<IClickableMenu, int, int, WorldDate, Rectangle, bool> onCellRightClick = null,
			Action<IClickableMenu, int, int, WorldDate, Rectangle, Action<string>, Action<Item>> onCellHover = null
		);

		void UnregisterPage(IManifest manifest, string id);*/

		#endregion

		#region Crops Page

		void AddCropProvider(ICropProvider provider);

		void RemoveCropProvider(ICropProvider provider);

		void SetCropPriority(IManifest manifest, int priority);

		void SetCropCallback(IManifest manifest, Action action);

		void ClearCropCallback(IManifest manifest);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			int regrow,
			bool isGiantCrop,
			bool isPaddyCrop,
			bool isTrellisCrop,

			WorldDate start,
			WorldDate end,

			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> sprite,
			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> giantSprite,

			IReadOnlyCollection<int> phases,
			IReadOnlyCollection<Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?>> phaseSprites
		);

		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			int regrow,
			bool isGiantCrop,
			bool isPaddyCrop,
			bool isTrellisCrop,

			Item[] seeds,

			WorldDate start,
			WorldDate end,

			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> sprite,
			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> giantSprite,

			IReadOnlyCollection<int> phases,
			IReadOnlyCollection<Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?>> phaseSprites
		);


		void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			int regrow,
			bool isGiantCrop,
			bool isPaddyCrop,
			bool isTrellisCrop,

			WorldDate start,
			WorldDate end,

			SpriteInfo sprite,
			SpriteInfo giantSprite,

			IReadOnlyCollection<int> phases,
			IReadOnlyCollection<SpriteInfo> phaseSprites
		);

		void RemoveCrop(IManifest manifest, string id);

		void ClearCrops(IManifest manifest);

		List<CropInfo> GetSeasonCrops(int season);

		List<CropInfo> GetSeasonCrops(string season);

		void InvalidateCrops();

		#endregion

		#region Fortunes Page

		/// <summary>
		/// Register a new hook for describing random nightly events for Almanac
		/// to list in its Fortune page. A hook function is called whenever we
		/// need to check what nightly events will happen on a given day, and
		/// it's called with the unique world ID as the first parameter and the
		/// date we want to know about as a WorldDate for the second argument.
		///
		/// The function is expected to return one or more tuples containing,
		/// in order, the following:
		///
		/// 1. A boolean that, if true, hides the vanilla generated event for
		///    that night.
		///
		/// 2. A string that is displayed to the user in the Almanac. This
		///    supports Almanac's rich text formatting.
		///
		/// 3. An optional texture, with a Texture2D and source Rectangle?
		///
		/// 4. An optional item. The item is used for compatibility with
		///    Lookup Anything. Users will be able to hover over the
		///    entry and open Lookup Anything to the item.
		///
		///    Additionally, if no sprite is provided but an item is provider,
		///    the item will be used as a sprite. To disable this behavior,
		///    return Rectangle.Empty for the source rectangle.
		/// 
		/// </summary>
		/// <param name="manifest">The manifest of the mod registering a hook</param>
		/// <param name="hook">The hook function</param>
		void SetFortuneHook(IManifest manifest, Func<ulong, WorldDate, IEnumerable<Tuple<bool, string, Texture2D, Rectangle?, Item>>> hook);

		/// <summary>
		/// Register a new hook. This is similar to the previous function, but
		/// rather than returning a string and texture separately, here we
		/// just return an IRichEvent using our knowledge of Almanac's interfaces.
		/// </summary>
		/// <param name="manifest">The manifest of the mod registering a hook</param>
		/// <param name="hook">The hook function</param>
		void SetFortuneHook(IManifest manifest, Func<ulong, WorldDate, IEnumerable<Tuple<bool, IRichEvent>>> hook);

		/// <summary>
		/// Unregister the fortunes hook for the given mod.
		/// </summary>
		/// <param name="manifest">The manifest of the mod</param>
		void ClearFortuneHook(IManifest manifest);

		#endregion

		#region Notices Page

		/// <summary>
		/// Register a new hook for describing daily events for Almanac
		/// to list in its Local Notices page. A hook function is called whenever we
		/// need to check what daily notices should be displayed, and
		/// it's called with the unique world ID as the first parameter and the
		/// date we want to know about as a WorldDate for the second argument.
		///
		/// The function is expected to return one or more tuples containing,
		/// in order, the following:
		///
		/// 1. A string that is displayed to the user in the Almanac. This
		///    supports Almanac's rich text formatting.
		///
		/// 2. An optional texture, with a Texture2D and source Rectangle?
		///
		/// 3. An optional item. The item is used for compatibility with
		///    Lookup Anything. Users will be able to hover over the
		///    entry and open Lookup Anything to the item.
		///
		///    Additionally, if no sprite is provided but an item is provider,
		///    the item will be used as a sprite. To disable this behavior,
		///    return Rectangle.Empty for the source rectangle.
		/// 
		/// </summary>
		/// <param name="manifest">The manifest of the mod registering a hook</param>
		/// <param name="hook">The hook function</param>
		void SetNoticesHook(IManifest manifest, Func<int, WorldDate, IEnumerable<Tuple<string, Texture2D, Rectangle?, Item>>> hook);

		/// <summary>
		/// Register a new hook. This is similar to the previous function, but
		/// rather than returning a string and texture separately, here we
		/// just return an IRichEvent using our knowledge of Almanac's interfaces.
		/// </summary>
		/// <param name="manifest">The manifest of the mod registering a hook</param>
		/// <param name="hook">The hook function</param>
		void SetNoticesHook(IManifest manifest, Func<int, WorldDate, IEnumerable<IRichEvent>> hook);

		/// <summary>
		/// Unregister the notices hook for the given mod.
		/// </summary>
		/// <param name="manifest">The manifest of the mod</param>
		void ClearNoticesHook(IManifest manifest);

		#endregion

		#region Weather

		/// <summary>
		/// Get the weather for a specific date, within the location context for the
		/// given location.
		/// </summary>
		/// <param name="date">The date we want weather for.</param>
		/// <param name="location">The location we want weather for.</param>
		string GetWeatherForDate(WorldDate date, GameLocation location);

		/// <summary>
		/// Get the weather for a specific date, within a specific location context.
		/// </summary>
		/// <param name="date">The date we want weather for.</param>
		/// <param name="context">The context we want weather for.</param>
		string GetWeatherForDate(WorldDate date, string context = "Default");

		#endregion
	}

	public class ModAPI : IAlmanacAPI {
		private readonly ModEntry Mod;

		public ModAPI(ModEntry mod) {
			Mod = mod;
		}

		public int DaysPerMonth => ModEntry.DaysPerMonth;

		#region Crop Providers

		public void AddCropProvider(ICropProvider provider) {
			Mod.Crops.AddProvider(provider);
		}

		public void RemoveCropProvider(ICropProvider provider) {
			Mod.Crops.RemoveProvider(provider);
		}

		#endregion

		#region Manual Mod Crops

		public void InvalidateCrops() {
			Mod.Crops.Invalidate();
		}

		public void SetCropPriority(IManifest manifest, int priority) {
			var provider = Mod.Crops.GetModProvider(manifest, priority != 0);
			if (provider != null && provider.Priority != priority) {
				provider.Priority = priority;
				Mod.Crops.SortProviders();
			}
		}

		public void SetCropCallback(IManifest manifest, Action? action) {
			var provider = Mod.Crops.GetModProvider(manifest, action != null);
			if (provider != null)
				provider.SetCallback(action);
		}

		public void ClearCropCallback(IManifest manifest) {
			SetCropCallback(manifest, null);
		}

		private static SpriteInfo? HydrateSprite(Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> input) {
			if (input?.Item1 == null)
				return null;

			return new SpriteInfo(
				texture: input.Item1,
				baseSource: input.Item2 ?? input.Item1.Bounds,
				baseColor: input.Item3,
				overlayTexture: input.Item4,
				overlaySource: input.Item5,
				overlayColor: input.Item6
			);
		}

		private static List<SpriteInfo?>? HydrateSprites(IEnumerable<Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?>> input) {
			if (input == null)
				return null;

			List<SpriteInfo?> result = new();
			foreach (var def in input) {
				result.Add(HydrateSprite(def));
			}

			return result;
		}

		public void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			int regrow,
			bool isGiantCrop,
			bool isPaddyCrop,
			bool isTrellisCrop,

			WorldDate start,
			WorldDate end,

			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> sprite,
			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> giantSprite,

			IReadOnlyCollection<int> phases,
			IReadOnlyCollection<Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?>> phaseSprites
		) {
			var provider = Mod.Crops.GetModProvider(manifest);
			provider!.AddCrop(
				id: id,
				item: item,
				name: name,
				sprite: sprite == null ? (item == null ? null : SpriteHelper.GetSprite(item)) : HydrateSprite(sprite),
				isTrellisCrop: isTrellisCrop,
				isGiantCrop: isGiantCrop,
				giantSprite: HydrateSprite(giantSprite),
				seeds: null,
				isPaddyCrop: isPaddyCrop,
				phases: phases,
				phaseSprites: HydrateSprites(phaseSprites),
				regrow: regrow,
				start: start,
				end: end
			);
		}


		public void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			int regrow,
			bool isGiantCrop,
			bool isPaddyCrop,
			bool isTrellisCrop,

			Item[] seeds,

			WorldDate start,
			WorldDate end,

			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> sprite,
			Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?> giantSprite,

			IReadOnlyCollection<int> phases,
			IReadOnlyCollection<Tuple<Texture2D, Rectangle?, Color?, Texture2D, Rectangle?, Color?>> phaseSprites
		) {
			var provider = Mod.Crops.GetModProvider(manifest);
			provider!.AddCrop(
				id: id,
				item: item,
				name: name,
				sprite: sprite == null ? (item == null ? null : SpriteHelper.GetSprite(item)) : HydrateSprite(sprite),
				isTrellisCrop: isTrellisCrop,
				isGiantCrop: isGiantCrop,
				giantSprite: HydrateSprite(giantSprite),
				seeds: seeds,
				isPaddyCrop: isPaddyCrop,
				phases: phases,
				phaseSprites: HydrateSprites(phaseSprites),
				regrow: regrow,
				start: start,
				end: end
			);
		}


		public void AddCrop(
			IManifest manifest,

			string id,

			Item item,
			string name,

			int regrow,
			bool isGiantCrop,
			bool isPaddyCrop,
			bool isTrellisCrop,

			WorldDate start,
			WorldDate end,

			SpriteInfo sprite,
			SpriteInfo giantSprite,

			IReadOnlyCollection<int> phases,
			IReadOnlyCollection<SpriteInfo> phaseSprites
		) {
			var provider = Mod.Crops.GetModProvider(manifest);
			provider!.AddCrop(
				id: id,
				item: item,
				name: name,
				sprite: sprite,
				isTrellisCrop: isTrellisCrop,
				isGiantCrop: isGiantCrop,
				giantSprite: giantSprite,
				seeds: null,
				isPaddyCrop: isPaddyCrop,
				phases: phases,
				phaseSprites: phaseSprites,
				regrow: regrow,
				start: start,
				end: end
			);
		}

		public void RemoveCrop(IManifest manifest, string id) {
			var provider = Mod.Crops.GetModProvider(manifest, false);
			if (provider != null)
				provider.RemoveCrop(id);
		}

		public void ClearCrops(IManifest manifest) {
			var provider = Mod.Crops.GetModProvider(manifest, false);
			if (provider != null)
				provider.ClearCrops();
		}

		#endregion

		#region Get Crops

		public List<CropInfo> GetSeasonCrops(int season) {
			return Mod.Crops.GetSeasonCrops(season);
		}

		public List<CropInfo> GetSeasonCrops(string season) {
			return Mod.Crops.GetSeasonCrops(season);
		}

		#endregion

		#region Fortune Telling

		public void SetFortuneHook(IManifest manifest, Func<ulong, WorldDate, IEnumerable<Tuple<bool, string, Texture2D, Rectangle?, Item>>> hook) {
			Mod.Luck.RegisterHook(manifest, hook);
		}

		public void SetFortuneHook(IManifest manifest, Func<ulong, WorldDate, IEnumerable<Tuple<bool, IRichEvent>>> hook) {
			Mod.Luck.RegisterHook(manifest, hook);
		}

		public void ClearFortuneHook(IManifest manifest) {
			Mod.Luck.ClearHook(manifest);
		}

		#endregion

		#region Local Notices

		public void SetNoticesHook(IManifest manifest, Func<int, WorldDate, IEnumerable<Tuple<string, Texture2D, Rectangle?, Item>>> hook) {
			Mod.Notices.RegisterHook(manifest, hook);
		}

		public void SetNoticesHook(IManifest manifest, Func<int, WorldDate, IEnumerable<IRichEvent>> hook) {
			Mod.Notices.RegisterHook(manifest, hook);
		}

		public void ClearNoticesHook(IManifest manifest) {
			Mod.Notices.ClearHook(manifest);
		}

		#endregion

		#region Weather

		public string GetWeatherForDate(WorldDate date, GameLocation location) {
			if (location is null)
				throw new ArgumentNullException(nameof(location));

			string val;

			if (location is Desert)
				val = "Desert";
			else {
				GameLocation.LocationContext ctx = location.GetLocationContext();
				switch(ctx) {
					case GameLocation.LocationContext.Default:
						val = "Default";
						break;
					case GameLocation.LocationContext.Island:
						val = "Island";
						break;
					default:
						throw new ArgumentException("Invalid location context");
				}
			}

			return GetWeatherForDate(date, val);
		}

		public string GetWeatherForDate(WorldDate date, string context = "Default") {
			GameLocation.LocationContext ctx;
			switch(context) {
				case "Default":
					ctx = GameLocation.LocationContext.Default;
					break;
				case "Island":
					ctx = GameLocation.LocationContext.Island;
					break;
				case "Desert":
					return "Sun";
				default:
					throw new ArgumentException("Invalid location context");
			}

			int weather = Mod.Weather.GetWeatherForDate(Mod.GetBaseWorldSeed(), date, ctx);
			return WeatherHelper.GetWeatherStringID(weather);
		}

		#endregion

	}
}
