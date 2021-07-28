/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework.Graphics;
using PlatoTK;
using PlatoTK.Content;
using PlatoTK.Objects;
using StardewModdingAPI;
using StardewValley;
using System.IO;
using System.Text.RegularExpressions;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb : PlatoSObject<SObject>
	{
		internal new protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ModConfig Config => ModConfig.Instance;
		internal protected static HarmonyInstance Harmony => ModEntry.Instance.harmony;
		internal protected static IPlatoHelper PlatoHelper => ModEntry.Instance.platoHelper;
		internal protected static Multiplayer MP => Helper.Reflection.GetField<Multiplayer>
			(typeof (Game1), "multiplayer").GetValue ();

		private static ISaveIndex EmptySaveIndex;
		private static ISaveIndex FullSaveIndex;
		public static int TileIndex => EmptySaveIndex?.Index ?? -1;

		private static Texture2D EmptyTexture;
		private static Texture2D FullTexture;

		internal static void Register ()
		{
			// Load the textures for an empty (seedless) and full (seeded) bomb.
			EmptyTexture = Helper.Content.Load<Texture2D> (
				Path.Combine ("assets", "FlowerBomb-empty.png"));
			FullTexture = Helper.Content.Load<Texture2D> (
				Path.Combine ("assets", "FlowerBomb-full.png"));

			// Request the regular object ID for the Flower Bomb.
			EmptySaveIndex = PlatoHelper.Content.GetSaveIndex ("kdau.FlowerBombs.FlowerBomb",
				() => Game1.objectInformation,
				(handle) => handle.Value.StartsWith ("Flower Bomb/"),
				(handle) =>
				{
					// Inject the object information.
					string displayName = Helper.Translation.Get ("FlowerBomb.name");
					string description = Helper.Translation.Get ("FlowerBomb.description");
					PlatoHelper.Content.Injections.InjectDataInsert ("Data/ObjectInformation",
						handle.Index, $"Flower Bomb/50/-300/Crafting -8/{displayName}/{description}");
					Helper.Content.InvalidateCache ("Data/ObjectInformation");

					// Inject the crafting recipe: 4 Clay and 1 Mudstone.
					PlatoHelper.Content.Injections.InjectDataInsert ("Data/CraftingRecipes",
						"Flower Bomb", $"330 4 574 1/Field/{handle.Index}/false/none/{displayName}");
					Helper.Content.InvalidateCache ("Data/CraftingRecipes");

					// Draw the empty texture by default.
					PlatoHelper.Harmony.PatchTileDraw ("kdau.FlowerBombs.FlowerBomb",
						Game1.objectSpriteSheet,
						(t) => t.Equals (Game1.objectSpriteSheet) ||
							(t.Name != null && Regex.IsMatch (t.Name, @"^Maps.springobjects$")),
						EmptyTexture, null,
						handle.Index);
				});

			// Request an additional object ID for the full (with-seed) state.
			FullSaveIndex = PlatoHelper.Content.GetSaveIndex ("kdau.FlowerBombs.FlowerBomb.full",
				() => Game1.objectInformation,
				(_handle) => true,
				(handle) =>
				{
					// Draw the full texture when requested.
					PlatoHelper.Harmony.PatchTileDraw ("kdau.FlowerBombs.FlowerBomb.full",
						Game1.objectSpriteSheet,
						(t) => t.Equals (Game1.objectSpriteSheet) ||
							(t.Name != null && Regex.IsMatch (t.Name, @"^Maps.springobjects$")),
						FullTexture, null,
						handle.Index);
				});

			// Make the magic happen.
			PlatoHelper.Harmony.LinkContruction<SObject, FlowerBomb> ();
			PlatoHelper.Harmony.LinkTypes (typeof (SObject), typeof (FlowerBomb));

			// Draw the seed attachment. Patching this through PlatoTK crashes
			// the game, so patch it manually with a postfix instead.
			Harmony.Patch (
				original: AccessTools.Method (typeof (Item),
					nameof (Item.drawAttachments)),
				postfix: new HarmonyMethod (typeof (FlowerBomb),
					nameof (FlowerBomb.DrawAttachments_Postfix))
			);

			// Hook onto the sprinkler code to ensure germination. This runs on
			// the sprinkler object, not the target object.
			Harmony.Patch (
				original: AccessTools.Method (typeof (SObject),
					nameof (SObject.ApplySprinkler)),
				postfix: new HarmonyMethod (typeof (FlowerBomb),
					nameof (FlowerBomb.ApplySprinkler_Postfix))
			);
		}

		public override bool CanLinkWith (object linkedObject)
		{
			return base.CanLinkWith (linkedObject) ||
				(linkedObject is Item item &&
					Utility.IsNormalObjectAtParentSheetIndex (item, TileIndex));
		}

		private static bool TryGetLinked (object linkedObject, out FlowerBomb linked)
		{
			if (!PlatoHelper.Harmony.TryGetLink (linkedObject, out object trace) ||
				Helper.Reflection.GetField<object> (trace, "Target").GetValue () is not FlowerBomb bomb)
			{
				linked = null;
				return false;
			}
			linked = bomb;
			return true;
		}

		public override void OnConstruction (IPlatoHelper helper, object linkedObject)
		{
			base.OnConstruction (helper, linkedObject);
			EmptySaveIndex.ValidateIndex ();
			if (Base != null && Base.ParentSheetIndex != TileIndex)
				Base.ParentSheetIndex = TileIndex;
		}

		public override Item getOne ()
		{
			return GetNew (seed);
		}

		public static SObject GetNew (SObject seed = null, int stack = 1)
		{
			EmptySaveIndex.ValidateIndex ();
			if (seed != null)
				stack = 1;
			var newObject = new SObject (TileIndex, stack);
			PlatoObject<SObject>.SetIdentifier (newObject, typeof (FlowerBomb));
			if (seed != null)
				newObject.heldObject.Value = seed.getOne () as SObject;
			return newObject;
		}
	}
}
