/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/flowerbombs
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PyTK.CustomElementHandler;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using SObject = StardewValley.Object;

namespace FlowerBombs
{
	public partial class FlowerBomb : PySObject
	{
		internal protected static IModHelper Helper => ModEntry.Instance.Helper;
		internal protected static IMonitor Monitor => ModEntry.Instance.Monitor;
		internal protected static ModConfig Config => ModConfig.Instance;

		private static CustomObjectData EmptyData;
		public static int EmptyID => EmptyData?.sdvId ?? -1;

		private static CustomObjectData FullData;
		public static int FullID => FullData?.sdvId ?? -1;

		internal static void Register ()
		{
			if (EmptyData != null)
				return;

			CraftingData recipe = new ("Flower Bomb",
				"330 4 574 1", delivery: "none"); // 4 Clay, 1 Mudstone

			string displayName = Helper.Translation.Get ("FlowerBomb.name");
			string description = Helper.Translation.Get ("FlowerBomb.description");

			Texture2D emptyTexture = Helper.Content.Load<Texture2D> (
				Path.Combine ("assets", "FlowerBomb-empty.png"));
			EmptyData = new CustomObjectData ("kdau.FlowerBombs.FlowerBomb",
				$"Flower Bomb/50/-300/Crafting -8/{displayName}/{description}",
				emptyTexture, Color.White, type: typeof (FlowerBomb),
				craftingData: recipe);

			Texture2D fullTexture = Helper.Content.Load<Texture2D> (
				Path.Combine ("assets", "FlowerBomb-full.png"));
			FullData = new CustomObjectData ("kdau.FlowerBombs.FlowerBombFull",
				$"Flower Bomb (full)/50/-300/Crafting -8/{displayName}/{description}",
				fullTexture, Color.White, type: typeof (FlowerBomb));
		}

		public FlowerBomb ()
		{
			setUpDataTracking ();
		}

		public FlowerBomb (CustomObjectData data)
		: base (data)
		{
			setUpDataTracking ();
		}

		public override Item getOne ()
		{
			FlowerBomb one = new (data)
			{
				TileLocation = Vector2.Zero,
				name = name,
				Price = price,
				Quality = quality
			};
			if (heldObject.Value != null)
				one.heldObject.Value = (SObject) heldObject.Value.getOne ();
			return one;
		}

		public override object getReplacement ()
		{
			Chest chest = (Chest) base.getReplacement ();
			if (heldObject.Value != null)
				chest.addItem (heldObject.Value);
			return chest;
		}

		public override Dictionary<string, string> getAdditionalSaveData ()
		{
			Dictionary<string, string> additionalSaveData =
				base.getAdditionalSaveData ();
			if (heldObject.Value != null)
			{
				additionalSaveData["seed"] =
					heldObject.Value.ParentSheetIndex.ToString ();
			}
			return additionalSaveData;
		}

		public override void rebuild (Dictionary<string, string> additionalSaveData,
			object replacement)
		{
			base.rebuild (additionalSaveData, replacement);
			if (additionalSaveData.ContainsKey ("seed"))
			{
				heldObject.Value = new SObject (Int32
					.Parse (additionalSaveData["seed"]), 1);
			}
		}

		public override ICustomObject recreate (Dictionary<string, string> additionalSaveData,
			object replacement)
		{
			FlowerBomb self = new (CustomObjectData
				.collection[additionalSaveData["id"]]);
			if (additionalSaveData.ContainsKey ("seed"))
			{
				self.heldObject.Value = new SObject (Int32
					.Parse (additionalSaveData["seed"]), 1);
			}
			return self;
		}
	}
}
