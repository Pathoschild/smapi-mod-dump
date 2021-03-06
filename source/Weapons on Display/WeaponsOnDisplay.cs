/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/AHilyard/WeaponsOnDisplay
**
*************************************************/

using System;
using System.IO;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Tools;
using StardewValley.Objects;
using StardewModdingAPI;
using Microsoft.Xna.Framework.Graphics;
using Harmony;
using System.Xml.Serialization;

namespace WeaponsOnDisplay
{
	/// <summary>The mod entry point.</summary>
	public class ModEntry : Mod
	{
		public static IMonitor M = null;
		/*********
		** Public methods
		*********/
		/// <summary>The mod entry point, called after the mod is first loaded.</summary>
		/// <param name="helper">Provides simplified APIs for writing mods.</param>
		public override void Entry(IModHelper helper)
		{
			M = Monitor;
			HarmonyInstance harmony = HarmonyInstance.Create(ModManifest.UniqueID);

			// Patch furniture's pick up, drop item, and render functionality to support weapons.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Objects.Furniture), nameof(StardewValley.Objects.Furniture.clicked)),
						  prefix: new HarmonyMethod(typeof(FurniturePatches), nameof(FurniturePatches.clicked_Prefix)));
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Objects.Furniture), nameof(StardewValley.Objects.Furniture.performObjectDropInAction)),
						  postfix: new HarmonyMethod(typeof(FurniturePatches), nameof(FurniturePatches.performObjectDropInAction_Postfix)));
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Objects.Furniture), nameof(StardewValley.Objects.Furniture.draw),
						  new Type[] { typeof(SpriteBatch), typeof(int), typeof(int), typeof(float) }),
						  prefix: new HarmonyMethod(typeof(FurniturePatches), nameof(FurniturePatches.draw_Prefix)));

			// Pass the game's action button functionality to allow weapons to be dropped onto furniture.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.Game1), nameof(StardewValley.Game1.pressActionButton)),
						  prefix: new HarmonyMethod(typeof(Game1Patches), nameof(Game1Patches.pressActionButton_Prefix)));

			// Patch the game location's "check action" to allow weapons to be dropped onto tables.
			harmony.Patch(original: AccessTools.Method(typeof(StardewValley.GameLocation), nameof(StardewValley.GameLocation.checkAction)),
						  prefix: new HarmonyMethod(typeof(GameLocationPatches), nameof(GameLocationPatches.checkAction_Prefix)));

			// Save handlers to prevent custom objects from being saved to file.
			helper.Events.GameLoop.Saving += (s, e) => makePlaceholderObjects();
			helper.Events.GameLoop.Saved += (s, e) => restorePlaceholderObjects();
			helper.Events.GameLoop.SaveLoaded += (s, e) => restorePlaceholderObjects();
		}

		private static T XmlDeserialize<T>(string toDeserialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(typeof(T));
			using(StringReader textReader = new StringReader(toDeserialize))
			{
				return (T)xmlSerializer.Deserialize(textReader);
			}
		}

		private static string XmlSerialize<T>(T toSerialize)
		{
			XmlSerializer xmlSerializer = new XmlSerializer(toSerialize.GetType());
			using(StringWriter textWriter = new StringWriter())
			{
				xmlSerializer.Serialize(textWriter, toSerialize);
				return textWriter.ToString();
			}
		}

		/// <summary>Replaces all instances of WeaponProxy with placeholder objects.</summary>
		private void makePlaceholderObjects()
		{
			// Find all instances of WeaponProxy objects and replace them with standard placeholder objects.
			foreach (GameLocation location in Game1.locations)
			{
				foreach (Furniture furniture in location.furniture)
				{
					if (furniture.heldObject.Value is WeaponProxy proxy)
					{
						StardewValley.Object placeholder = new StardewValley.Object(furniture.heldObject.Value.TileLocation, 0);
						placeholder.Name = $"WeaponProxy:{XmlSerialize(proxy.Weapon)}";
						furniture.heldObject.Set(placeholder);
					}
				}
			}

			// Ensure we check additional buildings like sheds and cabins.
			foreach (Building building in Game1.getFarm().buildings)
			{
				foreach (Furniture furniture in building.indoors.Value.furniture)
				{
					if (furniture.heldObject.Value is WeaponProxy proxy)
					{
						StardewValley.Object placeholder = new StardewValley.Object(furniture.heldObject.Value.TileLocation, 0);
						placeholder.Name = $"WeaponProxy:{XmlSerialize(proxy.Weapon)}";
						furniture.heldObject.Set(placeholder);
					}
				}
			}
		}

		/// <summary>Replaces all placeholder objects with their WeaponProxy counterparts.</summary>
		private void restorePlaceholderObjects()
		{
			// Find all instances of placeholder objects and replace with WeaponProxies.
			foreach (GameLocation location in Game1.locations)
			{
				foreach (Furniture furniture in location.furniture)
				{
					if (furniture.heldObject.Value != null && furniture.heldObject.Value.Name.StartsWith("WeaponProxy:"))
					{
						string xmlString = furniture.heldObject.Value.Name.Substring("WeaponProxy:".Length);
						MeleeWeapon weapon = XmlDeserialize<MeleeWeapon>(xmlString);
						furniture.heldObject.Set(new WeaponProxy(weapon));
					}
				}
			}

			// Ensure we check additional buildings like sheds and cabins.
			foreach (Building building in Game1.getFarm().buildings)
			{
				foreach (Furniture furniture in building.indoors.Value.furniture)
				{
					if (furniture.heldObject.Value != null && furniture.heldObject.Value.Name.StartsWith("WeaponProxy:"))
					{
						string xmlString = furniture.heldObject.Value.Name.Substring("WeaponProxy:".Length);
						MeleeWeapon weapon = XmlDeserialize<MeleeWeapon>(xmlString);
						furniture.heldObject.Set(new WeaponProxy(weapon));
					}
				}
			}
		}

	}
}