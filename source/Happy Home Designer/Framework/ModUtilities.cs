/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/HappyHomeDesigner
**
*************************************************/

using HappyHomeDesigner.Integration;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.GameData.Shops;
using StardewValley.Internal;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace HappyHomeDesigner.Framework
{
	public static class ModUtilities
	{
		[Flags]
		public enum CatalogType {None = 0, Furniture = 1, Wallpaper = 2, Collector = 4};

		private static readonly FieldInfo OldValueBackingField =
			typeof(MouseWheelScrolledEventArgs).GetField("<OldValue>k__BackingField", 
				BindingFlags.Instance | BindingFlags.NonPublic);

		public static bool CanDelete(this Item item, ICollection<string> knownIDs)
		{
			return knownIDs is not null && item is not null && knownIDs.Contains(item.QualifiedItemId);
		}

		public static bool TryFindAssembly(string name, [NotNullWhen(true)] out Assembly assembly)
		{
			foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
			{
				if (asm.GetName().Name == name)
				{
					assembly = asm;
					return true;
				}
			}
			assembly = null;
			return false;
		}

		public static void Suppress(this MouseWheelScrolledEventArgs e)
		{
			// suppress game
			Game1.oldMouseState = Game1.input.GetMouseState();

			// suppress event
			OldValueBackingField.SetValue(e, e.NewValue);
		}

		public static bool TryGetGenericOf(this Type type, int index, [NotNullWhen(true)] out Type generic)
		{
			generic = null;
			if (!type.IsGenericType)
				return false;
			var generics = type.GetGenericArguments();
			if (generics.Length <= index)
				return false;
			generic = generics[index];
			return true;
		}

		public static void DrawFrame(this SpriteBatch b, Texture2D texture, Rectangle dest, Rectangle source, int padding, int scale, Color color, int top = 0)
		{
			int destPad = padding * scale;
			int dTop = top * scale + destPad;
			int sTop = top + padding;

			// top
			int dy = dest.Y;
			int sy = source.Y;
			b.Draw(texture,
				new Rectangle(dest.X, dy, destPad, dTop),
				new Rectangle(source.X, sy, padding, sTop),
				color);
			b.Draw(texture,
				new Rectangle(dest.X + destPad, dy, dest.Width - destPad * 2, dTop),
				new Rectangle(source.X + padding, sy, source.Width - padding * 2, sTop),
				color);
			b.Draw(texture,
				new Rectangle(dest.X + dest.Width - destPad, dy, destPad, dTop),
				new Rectangle(source.X + source.Width - padding, sy, padding, sTop),
				color
				);

			// mid
			dy += dTop;
			sy += sTop;
			b.Draw(texture,
				new Rectangle(dest.X, dy, destPad, dest.Height - destPad * 2),
				new Rectangle(source.X, sy, padding, source.Height - padding * 2),
				color);
			b.Draw(texture,
				new Rectangle(dest.X + dest.Width - destPad, dy, destPad, dest.Height - destPad * 2),
				new Rectangle(source.X + source.Width - padding, sy, padding, source.Height - padding * 2),
				color
				);

			// bottom
			dy = dest.Y + dest.Height - destPad;
			sy = source.Y + source.Height - padding;
			b.Draw(texture,
				new Rectangle(dest.X, dy, destPad, destPad),
				new Rectangle(source.X, sy, padding, padding),
				color);
			b.Draw(texture,
				new Rectangle(dest.X + destPad, dy, dest.Width - destPad * 2, destPad),
				new Rectangle(source.X + padding, sy, source.Width - padding * 2, padding),
				color);
			b.Draw(texture,
				new Rectangle(dest.X + dest.Width - destPad, dy, destPad, destPad),
				new Rectangle(source.X + source.Width - padding, sy, padding, padding),
				color
				);
		}

		public static T ToDelegate<T>(this MethodInfo method, object target) where T : Delegate
			=> (T)Delegate.CreateDelegate(typeof(T), target, method);

		public static void QuickBind(this IGMCM gmcm, IManifest manifest, object config, string name)
		{
			var prop = config.GetType().GetProperty(name) ??
				throw new ArgumentException($"Public property of name '{name}' not found on config.");

			var title = $"config.{prop.Name}.name";
			var desc = $"config.{prop.Name}.desc";
			var type = prop.PropertyType;

			if (type == typeof(bool))
				gmcm.AddBoolOption(manifest,
				prop.GetMethod.ToDelegate<Func<bool>>(config),
				prop.SetMethod.ToDelegate<Action<bool>>(config),
				() => ModEntry.i18n.Get(title),
				() => ModEntry.i18n.Get(desc));

			else if (type == typeof(KeybindList))
				gmcm.AddKeybindList(manifest,
				prop.GetMethod.ToDelegate<Func<KeybindList>>(config),
				prop.SetMethod.ToDelegate<Action<KeybindList>>(config),
				() => ModEntry.i18n.Get(title),
				() => ModEntry.i18n.Get(desc));

			else
				throw new ArgumentException($"Config property '{name}' is of unsupported type '{type.FullName}'.");
		}

		public static bool AssertValid(this CodeMatcher matcher, string message, LogLevel level = LogLevel.Debug)
		{
			if (message is not null && matcher.IsInvalid)
				ModEntry.monitor.Log(message, level);
			return matcher.IsValid;
		}

		public static void Log(this ITranslationHelper helper, string key, object args = null, LogLevel level = LogLevel.Debug)
		{
			ModEntry.monitor.Log(helper.Get(key, args), level);
		}

		public static int Find<T>(this IReadOnlyList<T> items, T which) where T : class
		{
			int count = items.Count;
			for (int i = 0; i < count; i++)
				if (items[i] == which)
					return i;
			return -1;
		}

		public static Rectangle ToRect(this xTile.Dimensions.Rectangle rect)
			=> new(rect.X, rect.Y, rect.Width, rect.Height);

		public static IEnumerable<ISalable> GetAdditionalCatalogItems(this IEnumerable<ISalable> original, string ID)
		{
			var output = original;

			if (CustomFurniture.Installed && ID.Contains("Furniture"))
				output = output.Concat(CustomFurniture.customFurniture);

			return output;
		}

		public static bool CountsAsCatalog(this ShopMenu shop, bool ignore_config = false)
		{
			return shop.ShopId switch
			{
				"Furniture Catalogue" => (ignore_config || ModEntry.config.ReplaceFurnitureCatalog),
				"Catalogue" => (ignore_config || ModEntry.config.ReplaceWallpaperCatalog),
				_ =>
					(ignore_config || ModEntry.config.ReplaceRareCatalogs) &&
					shop.ShopData is ShopData data &&
					data.CustomFields is Dictionary<string, string> fields &&
					fields.ContainsKey("HappyHomeDesigner/Catalogue")
			};
		}

		public static IEnumerable<ISalable> GenerateCombined(CatalogType catalog)
		{
			IEnumerable<ISalable> output = Array.Empty<ISalable>();
			var shopData = DataLoader.Shops(Game1.content);

			if (catalog.HasFlag(CatalogType.Furniture) && shopData.TryGetValue("Furniture Catalogue", out var data))
				output = output.Concat(ShopBuilder.GetShopStock("Furniture Catalogue", data).Keys);

			if (catalog.HasFlag(CatalogType.Wallpaper) && shopData.TryGetValue("Catalogue", out data))
				output = output.Concat(ShopBuilder.GetShopStock("Catalogue", data).Keys);

			if (catalog.HasFlag(CatalogType.Collector))
			{
				foreach ((var id, var sdata) in shopData)
				{
					if (sdata.CustomFields is Dictionary<string, string> fields && 
						fields.ContainsKey("HappyHomeDesigner/Catalogue"))
					{
						output = output.Concat(ShopBuilder.GetShopStock(id, sdata).Keys);
					}
				}
			}

			return output;
		}

		public static bool TryPatch(this Harmony harmony, MethodInfo method, HarmonyMethod prefix = null, 
			HarmonyMethod postfix = null, HarmonyMethod transpiler = null, HarmonyMethod finalizer = null, 
			[CallerMemberName] string source = null)
		{
			try
			{
				harmony.Patch(method, prefix, postfix, transpiler, finalizer);
			} 
			catch (Exception e)
			{
				ModEntry.monitor.Log($"Failed to patch {method?.Name ?? "NULL"} from {source ?? "NULL"}:\t {e}", LogLevel.Error);
				return false;
			}
			return true;
		}
	}
}
