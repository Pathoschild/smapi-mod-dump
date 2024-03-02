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
using HappyHomeDesigner.Menus;
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
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace HappyHomeDesigner.Framework
{
	public static class ModUtilities
	{
		private static readonly FieldInfo OldValueBackingField =
			typeof(MouseWheelScrolledEventArgs).GetField("<OldValue>k__BackingField", 
				BindingFlags.Instance | BindingFlags.NonPublic);

		public static bool CanDelete(this Item item)
		{
			if (item is not Furniture furn)
				return false;

			return FurniturePage.knownFurnitureIDs is not null && FurniturePage.knownFurnitureIDs.Contains(furn.ItemId);
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

		public static T ToDelegate<T>(this MethodInfo method) where T : Delegate
			=> (T)Delegate.CreateDelegate(typeof(T), method);

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

		public static IEnumerable<ISalable> GetCatalogItems(bool furniture, ShopMenu existing = null)
		{
			var name = furniture ? "Furniture Catalogue" : "Catalogue";
			IEnumerable<ISalable> output;

			if (existing is null)
			{

				if (!DataLoader.Shops(Game1.content).TryGetValue(name, out var catalog))
					return Array.Empty<ISalable>();

				 output = ShopBuilder.GetShopStock(name, catalog).Keys;
			} else
			{
				output = existing.forSale;
			}

			if (CustomFurniture.Installed && furniture)
				output = output.Concat(CustomFurniture.customFurniture);

			return output;
		}

		public static IEnumerable<MethodInfo> GetMethodsNamed(this Type type, string name, BindingFlags flags = BindingFlags.Default)
		{
			foreach (var item in type.GetMethods(flags))
			{
				if (item.Name == name)
					yield return item;
			}
		}
	}
}
