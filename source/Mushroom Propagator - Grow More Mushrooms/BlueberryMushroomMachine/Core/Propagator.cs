/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/razikh-git/BlueberryMushroomMachine
**
*************************************************/

using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Locations;
using Object = StardewValley.Object;

using PyTK.CustomElementHandler;

// TODO: REWRITE: Convert machine to Json Assets

namespace BlueberryMushroomMachine
{
	public class Propagator : Cask, ISaveElement
	{
		// Custom members
		public int Quantity;
		public bool ProduceExtra;

		// Hidden members
		private new readonly int defaultDaysToMature;

		public Propagator() : this(Vector2.Zero)
		{
		}
		
		public Propagator(Vector2 tileLocation)
		{
			// Load derived fields
			IsRecipe = isRecipe;
			TileLocation = tileLocation;
			InitialisePropagator();
			defaultDaysToMature = ModEntry.Instance.Config.MaximumDaysToMature;

			// Load custom fields
			Quantity = 0;
			ProduceExtra = false;
		}

		protected override string loadDisplayName()
		{
			return ModEntry.Instance.i18n.Get("machine.name");
		}

		public override string getDescription()
		{
			return ModEntry.Instance.i18n.Get("machine.desc");
		}
		
		private void InitialisePropagator()
		{
			Name = ModValues.PropagatorInternalName;
			ParentSheetIndex = ModValues.PropagatorIndex;
			DisplayName = loadDisplayName();

			var strArray1 = ModValues.ObjectData.Split('/');
			price.Value = Convert.ToInt32(strArray1[1]);
			edibility.Value = Convert.ToInt32(strArray1[2]);
			var strArray2 = strArray1[3].Split(' ');
			type.Value = strArray2[0];
			if (strArray2.Length > 1)
				Category = Convert.ToInt32(strArray2[1]);
			setOutdoors.Value = Convert.ToBoolean(strArray1[5]);
			setIndoors.Value = Convert.ToBoolean(strArray1[6]);
			fragility.Value = Convert.ToInt32(strArray1[7]);
			isLamp.Value = strArray1.Length > 8 && strArray1[8].Equals("true");
			
			canBeSetDown.Value = true;
			bigCraftable.Value = true;
			initializeLightSource(TileLocation);

			boundingBox.Value = new Rectangle(
				(int)TileLocation.X * 64,
				(int)TileLocation.Y * 64,
				64,
				64);
		}

		/// <summary>
		/// Instantiates any mushroom objects currently attached to the machine when the farm is loaded.
		/// </summary>
		private void LoadHeldObject(int index, int quality, int quantity, int days, bool produceExtra)
		{
			Log.T($"LoadHeldObject(index: {index})",
				ModEntry.Instance.Config.DebugMode);

			if (index < 0 || quality < 0 || quantity < 0)
				return;

			var o = new Object(index, 1)
				{ Quality = quality };
			ModEntry.GetMushroomGrowthRate(o, out var rate);
			PutObject(o.getOne(), rate);
			Quantity = quantity;
			daysToMature.Value = days;
			ProduceExtra = produceExtra;
		}

		/// <summary>
		/// Adds an instance of the given item to be held by the machine,
		/// and resets all countdown variables.
		/// </summary>
		/// <param name="dropIn">Some instance of an object, hopefully a mushroom.</param>
		/// <param name="rate">Rate relative to lowest-tier mushrooms (1.0f) for this type of mushroom to grow.</param>
		private void PutObject(Item dropIn, float rate)
		{
			Log.T($"PutObject(item: [{dropIn.ParentSheetIndex}] {dropIn.Name}, rate: {rate:.00})",
				ModEntry.Instance.Config.DebugMode);

			heldObject.Value = dropIn.getOne() as Object;
			agingRate.Value = rate;
			daysToMature.Value = 0;
			minutesUntilReady.Value = 999999;
			Quantity = 1;
		}

		/// <summary>
		/// Ejects a duplicate of the originally-inserted mushroom if the machine
		/// has held it overnight, otherwise ejects the original mushroom and resets to empty.
		/// </summary>
		/// <param name="remove">
		/// Whether or not to nullify the held mushroom, ejecting the originally-inserted
		/// mushroom and leaving the machine empty.
		/// </param>
		private void PopObject(bool remove)
		{
			Log.T($"PopObject(remove: {remove})"
			      + $"\n>> HeldObject: [{heldObject.Value.ParentSheetIndex}] {heldObject.Value.Name} x{Quantity}",
				ModEntry.Instance.Config.DebugMode);

			// Incorporate Gatherer's skill effects for extra production
			var popQuantity = Quantity;
			if (ProduceExtra && Game1.player.professions.Contains(Farmer.gatherer)
				&& new Random().Next(5) == 0)
				popQuantity += 1;

			// Extract held object
			Game1.playSound("harvest");

			var popQuality = !remove && Game1.player.professions.Contains(Farmer.botanist) ? 4 : heldObject.Value.Quality;
			var popObject = new Object(heldObject.Value.ParentSheetIndex, 1, false, -1, popQuality);

			for (var i = 0; i < popQuantity; ++i)
				Game1.createItemDebris(popObject.getOne(),
					new Vector2(TileLocation.X * 64 + 32, TileLocation.Y * 64 + 32), -1);
			
			// Reset the harvest
			if (remove)
			{
				heldObject.Value = null;
				minutesUntilReady.Value = -1;
				Quantity = 0;
			}
			else
			{
				ModEntry.GetMushroomGrowthRate(heldObject.Value, out var rate);
				PutObject(heldObject.Value.getOne(), rate);
				minutesUntilReady.Value = 999999;
				Quantity = 1;
			}

			ProduceExtra = false;
			readyForHarvest.Value = false;
			daysToMature.Value = 0;
		}

		/// <summary>
		/// Behaviours for tool actions to uproot the machine itself.
		/// </summary>
		private void PopMachine()
		{
			// Extract the machine
			var key = Game1.player.GetToolLocation() / 64f;
			key.X = (int)key.X;
			key.Y = (int)key.Y;
			var toolLocation = Game1.player.GetToolLocation();
			var x = (double)boundingBox.Center.X;
			var y = (double)boundingBox.Center.Y;
			var playerPosition = new Vector2((float)x, (float)y);
			Game1.currentLocation.debris.Add(new Debris(
				new Propagator(TileLocation), toolLocation, playerPosition));
			Game1.currentLocation.Objects.Remove(key);
		}

		/// <summary>
		/// Perform all end-of-day checks for the Propagator to handle held object events.
		/// </summary>
		internal void TemporaryDayUpdate()
		{
			// Indexing inconsistencies with JA/CFR
			ParentSheetIndex = ModValues.PropagatorIndex;

			if (heldObject.Value == null)
				return;

			// Mark the machine as having grown overnight, allowing the user to pop out extra mushrooms
			ProduceExtra = true;
			
			CheckForMaturity();
		}

		/// <summary>
		/// Updates object quantity as the per-day maturity timer counts down.
		/// </summary>
		public void CheckForMaturity()
		{
			// Stop adding to the stack when the limit is reached
			ModEntry.GetMushroomMaximumQuantity(heldObject.Value, out var max);
			if (Quantity >= max)
				return;

			// Progress the growth of the stack per each mushroom's rate
			daysToMature.Value += Math.Max(1, defaultDaysToMature * agingRate);
			minutesUntilReady.Value = 999999;

			if (!(daysToMature >= defaultDaysToMature))
				return;

			// Mature the held mushroom over time, growing the quantity
			++Quantity;
			daysToMature.Value = 0;
		}

		/// <summary>
		/// Override method for any player cursor passive or active interactions with the machine.
		/// Permits triggering behaviours to pop mushrooms before they're ready with the action hotkey.
		/// </summary>
		/// <param name="who">Farmer interacting with the machine.</param>
		/// <param name="justCheckingForActivity">Whether the cursor hovered or clicked.</param>
		/// <returns>Whether to continue with base method.</returns>
		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			if (!justCheckingForActivity && who != null
					&& who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1)
					&& who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1)
					&& who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY())
					&& who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY())
					&& !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable()
					&& !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable()
					&& !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable()
					&& !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
				performToolAction(null, who.currentLocation);

			return justCheckingForActivity || base.checkForAction(who, justCheckingForActivity);
		}

		/// <summary>
		/// Allows the user to pop extra mushrooms before they're ready,
		/// and pop root mushrooms without extras.
		/// Author's note: The mushrooms are never ready.
		/// </summary>
		/// <returns>Whether to continue with base behaviour.</returns>
		public override bool performUseAction(GameLocation location)
		{
			if (heldObject.Value != null)
			{
				PopObject(false);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Overrides the usual hit-with-tool behaviour to change the requirements
		/// and allow for popping held mushrooms at different stages.
		/// </summary>
		/// <returns>Whether or not to continue with base behaviour.</returns>
		public override bool performToolAction(Tool t, GameLocation location)
		{
			// Ignore usages that wouldn't trigger actions for other machines
			if (t == null || !t.isHeavyHitter() || t is StardewValley.Tools.MeleeWeapon)
				return base.performToolAction(t, location);

			location.playSound("woodWhack");
			if (heldObject.Value != null)
				// Extract any held mushrooms from machine
				PopObject(true);
			else
				// Extract machine from location
				PopMachine();
			return false;
		}

		/// <summary>
		/// Overrides usual use-with-item behaviours to limit the set to working in
		/// specific locations with specific items, as well as other funky behaviour.
		/// </summary>
		/// <param name="dropIn">Our candidate item.</param>
		/// <param name="probe">Base game check for determining outcomes without consequences.</param>
		/// <param name="who">Farmer using the machine.</param>
		/// <returns>
		/// Whether or not to continue with base behaviour.
		/// True unless a mushroom was dropped in under appropriate conditions.
		/// </returns>
		public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
		{
			// Ignore usages with inappropriate items
			if (dropIn == null)
			{
				return false;
			}

			// Extract held mushrooms prematurely
			if (heldObject.Value != null)
				if (!readyForHarvest && ProduceExtra)
					// Get a copy of the root mushroom
					PopObject(false);
				else if (!ModEntry.Instance.Config.OnlyToolsCanRemoveRootMushrooms)
					// Remove the root mushroom if it hasn't settled overnight
					PopObject(true);

			// Determine if being used in an appropriate location
			if (!probe && who != null)
			{
				var flag = (who.currentLocation is Cellar && ModEntry.Instance.Config.WorksInCellar)
				            || (who.currentLocation is FarmCave && ModEntry.Instance.Config.WorksInFarmCave)
				            || (who.currentLocation is BuildableGameLocation && ModEntry.Instance.Config.WorksInBuildings)
				            || (who.currentLocation is FarmHouse && ModEntry.Instance.Config.WorksInFarmHouse)
				            || (who.currentLocation.IsGreenhouse && ModEntry.Instance.Config.WorksInGreenhouse)
				            || (who.currentLocation.IsOutdoors && ModEntry.Instance.Config.WorksOutdoors);

				if (!flag)
				{
					// Ignore bad machine locations
					Game1.showRedMessage(ModEntry.Instance.i18n.Get("error.location"));
					return false;
				}
			}
			
			// Ignore Truffles
			if (!probe && dropIn.ParentSheetIndex.Equals(430))
			{
				Game1.showRedMessage(ModEntry.Instance.i18n.Get("error.truffle"));
				return false;
			}

			// nanda kore wa
			if (quality >= 4)
				return false;
			
			if (!(dropIn is Object o) || !ModEntry.IsValidMushroom(o))
			{
				if (!probe)
					Log.D($"Invalid mushroom: [{dropIn.ParentSheetIndex}] {dropIn.Name}",
						ModEntry.Instance.Config.DebugMode);
				return false;
			}

			if (probe)
				return true;

			// Accept the deposited item
			ModEntry.GetMushroomGrowthRate(o, out var rate);
			PutObject(dropIn, rate);
			who?.currentLocation.playSound("Ship");
			return true;
		}

		/// <summary>
		/// Awkward override to specifically place a Propagator instead of a BigCraftable Object.
		/// </summary>
		/// <returns>True</returns>
		public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
		{
			var tile = new Vector2(x / 64, y / 64);
			health = 10;

			// Determine player
			owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;

			// Spawn object
			location.objects.Add(tile, new Propagator(tile));
			location.playSound("hammer");
			if (!performDropDownAction(who))
			{
				var one = (Object) getOne();
				one.shakeTimer = 50;
				one.TileLocation = tile;

				// Avoid placement conflicts.
				if (location.objects.ContainsKey(tile))
				{
					if (location.objects[tile].ParentSheetIndex != ParentSheetIndex)
					{
						Game1.createItemDebris(location.objects[tile], tile * 64f, -1);
						location.objects[tile] = one;
					}
				}
				else
					location.objects.Add(tile, one);
				one.initializeLightSource(tile);
			}
			location.playSound("woodyStep");
			return true;
		}
		
		public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
		{
			var shake = shakeTimer < 1 ? Point.Zero : new Point(Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
			var pulseAmount = ModEntry.Instance.Config.PulseWhenGrowing ? getScale() * 4f : Vector2.One;
			var position = Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64, y * 64 - 64));
			var sourceRect = getSourceRectForBigCraftable(ParentSheetIndex);
			var destRect = new Rectangle(
					(int)(position.X - pulseAmount.X / 2f) + shake.X,
					(int)(position.Y - pulseAmount.Y / 2f) + shake.Y,
					(int)(64f + pulseAmount.X),
					(int)(128f + pulseAmount.Y / 2f));
			
			// Draw the base sprite
			b.Draw(
					Game1.bigCraftableSpriteSheet,
					destRect,
					sourceRect,
					Color.White * alpha,
					0f,
					Vector2.Zero,
					SpriteEffects.None,
					Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f)
					+ (Game1.currentLocation.IsOutdoors ? 0f : x * 1f / 10000f));

			if (heldObject.Value == null)
				return;

			// Draw the held object overlay
			var customMushroom = !Enum.IsDefined(typeof(ModEntry.Mushrooms), heldObject.Value.ParentSheetIndex);

			ModEntry.GetMushroomMaximumQuantity(heldObject.Value, out var max);
			var whichFrame = ModEntry.GetOverlayGrowthFrame((int)daysToMature.Value, defaultDaysToMature, Quantity, max);
			sourceRect = ModEntry.GetOverlaySourceRect(heldObject, whichFrame);

			if (customMushroom)
			{
				var growthRatio = (whichFrame + 1f) / (ModValues.OverlayMushroomFrames + 1f);
				var growthScale = Math.Min(0.8f, growthRatio) + 0.2f;
				destRect = new Rectangle(
					(int)(position.X - pulseAmount.X / 2f) + shake.X + (int)(32 * (1 - growthScale))
					+ (int)(pulseAmount.X * growthScale / 4),
					(int)(position.Y - pulseAmount.Y / 2f) + shake.Y + 48 + (int)(32 * (1 - growthScale))
					+ (int)(pulseAmount.Y * growthScale / 8),
					(int)((64f + pulseAmount.X) * growthScale),
					(int)((64f + pulseAmount.Y / 2f) * growthScale));
			}
			
			b.Draw(
				!customMushroom ? ModEntry.OverlayTexture : Game1.objectSpriteSheet,
				destRect,
				sourceRect,
				Color.White,
				0f,
				Vector2.Zero,
				SpriteEffects.None,
				Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f)
				+ (Game1.currentLocation.IsOutdoors ? 0f : x * 1f / 10000f) + 1f / 10000f + 1f / 10000f);
		}

		public override Item getOne()
		{
			return new Propagator(Vector2.Zero);
		}

		/* PyTK ISaveElement */

		public object getReplacement()
		{
			return new Cask();
		}

		public Dictionary<string, string> getAdditionalSaveData()
		{
			var putIndex = heldObject.Value?.ParentSheetIndex ?? -1;
			var putQuality = heldObject.Value?.Quality ?? -1;
			var putQuantity = heldObject.Value != null ? Quantity : -1;

			return new Dictionary<string, string>
			{
				{ "tileLocationX", TileLocation.X.ToString() },
				{ "tileLocationY", TileLocation.Y.ToString() },
				{ "heldObjectIndex", putIndex.ToString() },
				{ "heldObjectQuality", putQuality.ToString() },
				{ "heldObjectQuantity", putQuantity.ToString() },
				{ "produceExtra", ProduceExtra.ToString() },
				{ "days", daysToMature.Value.ToString() }
			};
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
		{
			float.TryParse(additionalSaveData["tileLocationX"], out var x);
			float.TryParse(additionalSaveData["tileLocationY"], out var y);
			TileLocation = new Vector2(x, y);

			int.TryParse(additionalSaveData["days"], out var days);
			bool.TryParse(additionalSaveData["produceExtra"], out var produceExtra);
			int.TryParse(additionalSaveData["heldObjectIndex"], out var heldObjectIndex);
			int.TryParse(additionalSaveData["heldObjectQuality"], out var heldObjectQuality);
			int.TryParse(additionalSaveData["heldObjectQuantity"], out var heldObjectQuantity);

			LoadHeldObject(heldObjectIndex, heldObjectQuality, heldObjectQuantity, days, produceExtra);
			InitialisePropagator();
			
			Log.T($"Rebuilt {Name} ({ParentSheetIndex}) at({TileLocation.X}/{TileLocation.Y})",
				ModEntry.Instance.Config.DebugMode);
		}
	}
}
