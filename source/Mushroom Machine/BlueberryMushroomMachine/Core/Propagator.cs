using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Locations;
using StardewValley.Network;

using StardewModdingAPI;

using PyTK.CustomElementHandler;

/*
 * 
 * todo:
 * 
 *		fix outdoors collisions
 *		
 *		look into automate integration
 * 
 */

namespace BlueberryMushroomMachine
{
	public class Propagator : Cask, ISaveElement
	{
		// Custom members
		public int Quantity;
		public bool ProduceExtra;
		private static Texture2D SOverlayTexture;

		// Hidden members
		public new readonly int defaultDaysToMature
			= PropagatorMod.SHelper.ReadConfig<Config>().MaximumDaysToMature;

		public Propagator()
		{
		}
		
		public Propagator(Vector2 tileLocation)
		{
			// Take derived fields.
			IsRecipe = isRecipe;
			TileLocation = tileLocation;
			loadDefaultValues();

			// Load custom fields.
			Quantity = 0;
			ProduceExtra = false;

			// Load derived fields.
			loadObjectData();
		}

		protected override string loadDisplayName()
		{
			return PropagatorMod.i18n.Get("machine.name");
		}

		public override string getDescription()
		{
			return PropagatorMod.i18n.Get("machine.desc");
		}
		
		private void loadDefaultValues()
		{
			canBeSetDown.Value = true;
			bigCraftable.Value = true;
			initializeLightSource(TileLocation, false);
		}
		
		/// <summary>
		/// Initialises the machine with a collection of preset values.
		/// </summary>
		private void loadObjectData()
		{
			loadOverlayTexture();

			Name = PropagatorData.PropagatorName;
			ParentSheetIndex = PropagatorData.PropagatorIndex;

			string[] strArray1 = PropagatorData.ObjectData.Split('/');
			displayName = strArray1[0];
			price.Value = Convert.ToInt32(strArray1[1]);
			edibility.Value = Convert.ToInt32(strArray1[2]);
			string[] strArray2 = strArray1[3].Split(' ');
			type.Value = strArray2[0];
			if (strArray2.Length > 1)
				Category = Convert.ToInt32(strArray2[1]);
			setOutdoors.Value = Convert.ToBoolean(strArray1[5]);
			setIndoors.Value = Convert.ToBoolean(strArray1[6]);
			fragility.Value = Convert.ToInt32(strArray1[7]);
			isLamp.Value = strArray1.Length > 8 && strArray1[8].Equals("true");

			boundingBox.Value = new Rectangle(
				(int)TileLocation.X * 64,
				(int)TileLocation.Y * 64,
				64,
				64);
		}

		/// <summary>
		/// Instantiates any mushroom objects currently attached to
		/// the machine when the farm is loaded.
		/// </summary>
		private void loadHeldObject(int index, int quality, int quantity, int days, bool produceExtra)
		{
			if (index >= 0 && quality >= 0 && quantity >= 0)
			{
				Item obj = new StardewValley.Object(index, 1)
					{ Quality = quality };

				if (PropagatorData.MushroomGrowingRates.TryGetValue(index, out float rate))
				{
					putObject(obj.getOne(), rate);
					Quantity = quantity;
					daysToMature.Value = days;
					ProduceExtra = produceExtra;
				}
				else
				{
					PropagatorMod.SMonitor.Log("Failed to reload held object: See TRACE",
						LogLevel.Error);
				}

				PropagatorData.MushroomQuantityLimits.TryGetValue(
					index, out int max);
				PropagatorMod.SMonitor.Log(
					"\nLoad: " + index
					+ " at(" + TileLocation.X + " " + TileLocation.Y
					+ ") val(" + quality + ") qty (" + quantity + "/" + max
					+ ") age(" + daysToMature + "/" + defaultDaysToMature
					+ " [+" + agingRate + "])",
					LogLevel.Trace);

			}
		}
		
		/// <summary>
		/// Shortcut for loading the collective texture for all mushroom overlays.
		/// </summary>
		private void loadOverlayTexture()
		{
			SOverlayTexture = PropagatorMod.SHelper.Content.Load<Texture2D>(PropagatorData.OverlayPath);
		}

		/// <summary>
		/// Determines the frame to be used for showing held mushroom growth.
		/// </summary>
		/// <param name="days">Current days since last growth.</param>
		/// <param name="quantity">Current count of mushrooms.</param>
		/// <param name="max">Maximum amount of mushrooms of this type.</param>
		/// <returns>Frame for mushroom growth progress.</returns>
		private int getWhichFrameForOverlay(int days, int quantity, int max)
		{
			float maths =
				(((quantity - 1) + ((float)days / defaultDaysToMature)) * defaultDaysToMature)
				/ ((max - 1) * defaultDaysToMature)
				* 3f;
			
			return (int)Math.Floor(maths);
		}

		/// <summary>
		/// Generates a clipping rectangle for the overlay appropriate
		/// to the current held mushroom, and its held quantity.
		/// </summary>
		/// <param name="whichMushroom">Tilesheet index of the held mushroom.</param>
		/// <param name="whichFrame">Progress frame of the held mushroom.</param>
		/// <returns></returns>
		private Rectangle getSourceRectForOverlay(int whichMushroom, int whichFrame)
		{
			return new Rectangle(whichFrame * 16, whichMushroom * 32, 16, 32);
		}

		/// <summary>
		/// Adds an instance of the given item to be held by the machine,
		/// and resets all countdown variables.
		/// </summary>
		/// <param name="dropIn">Generic instance of a mushroom item.</param>
		/// <param name="rate">Predetermined countdown rate in days for this type of mushroom.</param>
		private void putObject(Item dropIn, float rate)
		{
			heldObject.Value = dropIn.getOne() as StardewValley.Object;
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
		private void popObject(bool remove)
		{
			PropagatorData.MushroomQuantityLimits.TryGetValue(
				heldObject.Value.ParentSheetIndex, out int max);
			PropagatorMod.SMonitor.Log(
				"\nPop: " + heldObject.Value.DisplayName
				+ " at(" + TileLocation.X + " " + TileLocation.Y
				+ ") val(" + Quality + ") qty (" + Quantity + "/" + max
				+ ") age(" + daysToMature + "/" + defaultDaysToMature
				+ " [+" + agingRate + "])",
				LogLevel.Trace);

			// Incorporate Gatherer's skill effects for extra production.
			int extra = 0;
			if (ProduceExtra && Game1.player.professions.Contains(Farmer.gatherer)
				&& new Random().Next(5) == 0)
				extra = 1;
			
			// Extract held object.
			Game1.playSound("coin");
			Game1.createMultipleObjectDebris(heldObject.Value.ParentSheetIndex,
				(int) TileLocation.X, (int) TileLocation.Y, Quantity + extra,
				Game1.player.UniqueMultiplayerID);

			// Reset the harvest.
			StardewValley.Object obj = heldObject.Value;
			if (remove)
			{
				heldObject.Value = null;
				minutesUntilReady.Value = -1;
				Quantity = 0;
			}
			else
			{
				putObject(obj.getOne(), PropagatorData.MushroomGrowingRates[obj.ParentSheetIndex]);
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
		private void popMachine()
		{
			// Extract the machine.
			Vector2 key = Game1.player.GetToolLocation(false) / 64f;
			key.X = (int)key.X;
			key.Y = (int)key.Y;
			Vector2 toolLocation = Game1.player.GetToolLocation(false);
			Rectangle boundingBox = Game1.player.GetBoundingBox();
			double x = boundingBox.Center.X;
			double y = boundingBox.Center.Y;
			Vector2 playerPosition = new Vector2((float)x, (float)y);
			Game1.currentLocation.debris.Add(new Debris(
				new Propagator(TileLocation), toolLocation, playerPosition));
			Game1.currentLocation.Objects.Remove(key);
		}

		/// <summary>
		/// Band-aid function to perform all end-of-day checks
		/// for the Propagator to handle held object events.
		/// </summary>
		internal void TemporaryDayUpdate()
		{
			// Indexing inconsistencies with JA/CFR.
			ParentSheetIndex = PropagatorData.PropagatorIndex;

			if (heldObject.Value == null)
			{
				PropagatorMod.SMonitor.Log(
					"\nUpdate:"
					+ " (" + TileLocation.X + " " + TileLocation.Y
					+ ") is holding a null object.",
					LogLevel.Trace);
				return;
			}

			// Mark the machine as having grown overnight, allowing
			// the user to pop out extra mushrooms.
			ProduceExtra = true;

			PropagatorData.MushroomQuantityLimits.TryGetValue(
				heldObject.Value.ParentSheetIndex, out int max);
			PropagatorMod.SMonitor.Log(
				"\nUpdate: " + heldObject.Value.DisplayName
				+ " at(" + TileLocation.X + " " + TileLocation.Y
				+ ") val(" + Quality + ") qty (" + Quantity + "/" + max
				+ ") age(" + daysToMature + "/" + defaultDaysToMature
				+ " [+" + agingRate + "])",
				LogLevel.Trace);

			checkForMaturity();
		}

		/// <summary>
		/// Runs through all start-of-day checks.
		/// Temporarily stubbed for incompatibilities.
		/// </summary>
		/// <param name="location">Used for default game behaviours.</param>
		public override void DayUpdate(GameLocation location)
		{
			return;
		}

		/// <summary>
		/// Updates object quantity as the per-day maturity timer counts down.
		/// </summary>
		public new void checkForMaturity()
		{
			// Stop adding to the stack when the limit is reached.
			PropagatorData.MushroomQuantityLimits.TryGetValue(
				heldObject.Value.ParentSheetIndex, out int max);
			if (Quantity >= max)
				return;

			// Progress the growth of the stack per each mushroom's rate.
			daysToMature.Value += Math.Max(1, defaultDaysToMature * agingRate);
			minutesUntilReady.Value = 999999;

			// Mature the held mushroom over time, growing the quantity.
			if (daysToMature >= defaultDaysToMature)
			{
				++Quantity;
				daysToMature.Value = 0;
				
				PropagatorMod.SMonitor.Log(
					"Matured to val(" + Quality + ") qty(" + Quantity + "/" + max + ")",
					LogLevel.Trace);
			}
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

			if (justCheckingForActivity)
				return true;
			return base.checkForAction(who, justCheckingForActivity);
		}

		/// <summary>
		/// Allows the user to pop extra mushrooms before they're ready,
		/// and pop root mushrooms without extras.
		/// Author's note: The mushrooms are never ready.
		/// </summary>
		/// <param name="location">Default parameter.</param>
		/// <returns>Whether to continue with base method.</returns>
		public override bool performUseAction(GameLocation location)
		{
			if (heldObject.Value != null)
			{
				popObject(false);
				return true;
			}
			return false;
		}

		/// <summary>
		/// Overrides the usual hit-with-tool behaviour to change the requirements
		/// and allow for popping held mushrooms at different stages.
		/// </summary>
		/// <param name="t">Tool type. Default parameter.</param>
		/// <param name="location">Current location. Default parameter.</param>
		/// <returns>Whether or not to continue with base method.</returns>
		public override bool performToolAction(Tool t, GameLocation location)
		{
			// Ignore usages that wouldn't trigger actions for other machines.
			if (t == null || !t.isHeavyHitter() || t is StardewValley.Tools.MeleeWeapon)
				return base.performToolAction(t, location);

			location.playSound("woodWhack", NetAudio.SoundContext.Default);
			if (heldObject.Value != null)
				// Extract any held mushrooms from machine.
				popObject(true);
			else
				// Extract machine from location.
				popMachine();
			return false;
		}

		/// <summary>
		/// Overrides usual use-with-item behaviours to limit the set to working in
		/// specific locations with specific items, as well as other funky behaviour.
		/// </summary>
		/// <param name="dropIn">Our candidate item.</param>
		/// <param name="probe">???</param>
		/// <param name="who">Farmer using the machine.</param>
		/// <returns>
		/// Whether or not to continue with base functionalities.
		/// Falls through unless a mushroom was dropped in under good circumstances.
		/// </returns>
		public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
		{
			// Ignore usages with inappropriate items.
			if (dropIn != null && dropIn is StardewValley.Object
				&& (dropIn as StardewValley.Object).bigCraftable.Value)
				return false;

			// Extract held mushrooms prematurely.
			if (heldObject.Value != null)
				if (!readyForHarvest && ProduceExtra)
					// Get a copy of the root mushroom.
					popObject(false);
				else
					// Remove the root mushroom if it hasn't settled overnight.
					popObject(true);

			// Determine if being used in an appropriate location.
			if (!probe && who != null)
			{
				bool flag = false;
				if ((who.currentLocation is Cellar && PropagatorMod.SConfig.WorksInCellar)
				|| (who.currentLocation is FarmCave && PropagatorMod.SConfig.WorksInFarmCave)
				|| (who.currentLocation is BuildableGameLocation && PropagatorMod.SConfig.WorksInBuildings)
				|| (who.currentLocation is FarmHouse && PropagatorMod.SConfig.WorksInFarmHouse)
				|| (who.currentLocation.IsGreenhouse && PropagatorMod.SConfig.WorksInGreenhouse)
				|| (who.currentLocation.IsOutdoors && PropagatorMod.SConfig.WorksOutdoors))
					flag = true;
				
				if (!flag)
				{
					// Ignore bad machine locations.
					Game1.showRedMessage(PropagatorMod.i18n.Get("error.location"));
					return false;
				}
			}
			
			// Ignore Truffles.
			if (!probe && dropIn.ParentSheetIndex.Equals(430))
			{
				Game1.showRedMessage(PropagatorMod.i18n.Get("error.truffle"));
				return false;
			}

			// nanda kore wa
			if (quality >= 4)
				return false;

			// Ignore wrong items.
			if (!PropagatorData.MushroomGrowingRates.TryGetValue(dropIn.ParentSheetIndex, out float rate))
				return false;

			// Accept the deposited item.
			if (!probe)
			{
				putObject(dropIn, rate);
				who.currentLocation.playSound("Ship", NetAudio.SoundContext.Default);
			}
			return true;
		}

		/// <summary>
		/// Awkward override to specifically place a Propagator instead of a BigCraftable Object.
		/// </summary>
		/// <param name="location">Current location.</param>
		/// <param name="x">X-coord.</param>
		/// <param name="y">Y-coord.</param>
		/// <param name="who">Farmer placing the machine.</param>
		/// <returns>Always true.</returns>
		public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
		{
			Vector2 index1 = new Vector2(x / 64, y / 64);
			health = 10;

			// Determine player.
			if (who != null)
				owner.Value = who.UniqueMultiplayerID;
			else
				owner.Value = Game1.player.UniqueMultiplayerID;

			// Spawn object.
			location.objects.Add(index1, new Propagator(index1));
			location.playSound("hammer", NetAudio.SoundContext.Default);
			if (!performDropDownAction(who))
			{
				StardewValley.Object one = (StardewValley.Object)getOne();
				one.shakeTimer = 50;
				one.TileLocation = index1;

				// Avoid placement conflicts.
				if (location.objects.ContainsKey(index1))
				{
					if (location.objects[index1].ParentSheetIndex != ParentSheetIndex)
					{
						Game1.createItemDebris(location.objects[index1], index1 * 64f, -1, null, -1);
						location.objects[index1] = one;
					}
				}
				else
					location.objects.Add(index1, one);
				one.initializeLightSource(index1, false);
			}
			location.playSound("woodyStep", NetAudio.SoundContext.Default);
			return true;
		}
		
		public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
		{
			// Draw the base sprite.
			int whichMushroom = 0;
			if (heldObject.Value != null)
				PropagatorData.MushroomSourceRects.TryGetValue(
					heldObject.Value.ParentSheetIndex, out whichMushroom);
			Vector2 vector2 = getScale() * 4f;
			Vector2 local = Game1.GlobalToLocal(Game1.viewport, new Vector2( x * 64, y * 64 - 64));
			Rectangle destRect = new Rectangle(
					(int)(local.X - vector2.X / 2f) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), 
					(int)(local.Y - vector2.Y / 2f) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0), 
					(int)(64f + vector2.X), 
					(int)(128f + vector2.Y / 2f));
			b.Draw(
					Game1.bigCraftableSpriteSheet,
					destRect,
					new Rectangle?(getSourceRectForBigCraftable(ParentSheetIndex)),
					Color.White * alpha, 
					0f, 
					Vector2.Zero,
					SpriteEffects.None,
					Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000.0f) + x * 1.0f / 10000.0f);

			if (heldObject.Value == null)
				return;

			// Draw the held object overlay.
			PropagatorData.MushroomQuantityLimits.TryGetValue(
				heldObject.Value.ParentSheetIndex, out int max);
			int whichFrame = getWhichFrameForOverlay(
				(int)daysToMature.Value, Quantity, max);

			b.Draw(
					SOverlayTexture,
					destRect,
					getSourceRectForOverlay(whichMushroom, whichFrame),
					Color.White,
					0f,
					Vector2.Zero,
					SpriteEffects.None,
					Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) + x * 1f / 10000f + 1f / 10000f);
		}

		public override Item getOne()
		{
			return new Propagator();
		}

		/* PyTK ISaveElement */

		public object getReplacement()
		{
			return new Cask();
		}

		public Dictionary<string, string> getAdditionalSaveData()
		{
			int putIndex = heldObject.Value != null ? heldObject.Value.ParentSheetIndex : -1;
			int putQuality = heldObject.Value != null ? heldObject.Value.Quality : -1;
			int putQuantity = heldObject.Value != null ? Quantity : -1;

			return new Dictionary<string, string>()
			{
				{ "tileLocationX", TileLocation.X.ToString() },
				{ "tileLocationY", TileLocation.Y.ToString() },
				{ "heldObjectIndex", putIndex.ToString() },
				{ "heldObjectQuality", putQuality.ToString() },
				{ "heldObjectQuantity", putQuantity.ToString() },
				{ "produceExtra", ProduceExtra.ToString() },
				{ "days", daysToMature.Value.ToString() },
			};
		}

		public void rebuild(Dictionary<string, string> additionalSaveData, object replacement)
		{
			loadDefaultValues();
			loadObjectData();
			
			float.TryParse(additionalSaveData["tileLocationX"], out float x);
			float.TryParse(additionalSaveData["tileLocationY"], out float y);
			TileLocation = new Vector2(x, y);

			int.TryParse(additionalSaveData["days"], out int days);
			bool.TryParse(additionalSaveData["produceExtra"], out bool produceExtra);
			int.TryParse(additionalSaveData["heldObjectIndex"], out int heldObjectIndex);
			int.TryParse(additionalSaveData["heldObjectQuality"], out int heldObjectQuality);
			int.TryParse(additionalSaveData["heldObjectQuantity"], out int heldObjectQuantity);
			loadHeldObject(heldObjectIndex, heldObjectQuality, heldObjectQuantity, days, produceExtra);

			PropagatorMod.SMonitor.Log("Rebuilt " + Name + " (" + ParentSheetIndex + ") "
				+ " at " + TileLocation.X + " " + TileLocation.Y,
				LogLevel.Trace);
		}
	}
}
