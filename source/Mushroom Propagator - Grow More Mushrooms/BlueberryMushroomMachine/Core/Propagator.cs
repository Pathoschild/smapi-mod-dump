using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewValley;
using StardewValley.Objects;
using StardewValley.Locations;

using PyTK.CustomElementHandler;

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
			// Load derived fields.
			IsRecipe = isRecipe;
			TileLocation = tileLocation;
			loadDefaultValues();
			defaultDaysToMature
				= ModEntry.Instance.Helper.ReadConfig<Config>().MaximumDaysToMature;
			loadObjectData();

			// Load custom fields.
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
		
		private void loadDefaultValues()
		{
			canBeSetDown.Value = true;
			bigCraftable.Value = true;
			initializeLightSource(TileLocation);
		}
		
		/// <summary>
		/// Initialises the machine with a collection of preset values.
		/// </summary>
		private void loadObjectData()
		{
			Name = Const.PropagatorInternalName;
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
			if (index < 0 || quality < 0 || quantity < 0)
				return;

			Item obj = new StardewValley.Object(index, 1)
				{ Quality = quality };

			if (Const.MushroomGrowingRates.TryGetValue(index, out var rate))
			{
				putObject(obj.getOne(), rate);
				Quantity = quantity;
				daysToMature.Value = days;
				ProduceExtra = produceExtra;
			}
			else
			{
				Log.E("Failed to reload held object: See TRACE");
			}
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
			var maths =
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
			// Incorporate Gatherer's skill effects for extra production.
			var extra = 0;
			if (ProduceExtra && Game1.player.professions.Contains(Farmer.gatherer)
				&& new Random().Next(5) == 0)
				extra = 1;
			
			// Extract held object.
			Game1.playSound("coin");
			Game1.createMultipleObjectDebris(heldObject.Value.ParentSheetIndex,
				(int)TileLocation.X, (int)TileLocation.Y, Quantity + extra,
				Game1.player.UniqueMultiplayerID);

			// Reset the harvest.
			var obj = heldObject.Value;
			if (remove)
			{
				heldObject.Value = null;
				minutesUntilReady.Value = -1;
				Quantity = 0;
			}
			else
			{
				putObject(obj.getOne(), Const.MushroomGrowingRates[obj.ParentSheetIndex]);
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
		/// Band-aid function to perform all end-of-day checks
		/// for the Propagator to handle held object events.
		/// </summary>
		internal void TemporaryDayUpdate()
		{
			// Indexing inconsistencies with JA/CFR.
			ParentSheetIndex = ModValues.PropagatorIndex;

			if (heldObject.Value == null)
				return;

			// Mark the machine as having grown overnight, allowing
			// the user to pop out extra mushrooms.
			ProduceExtra = true;
			
			checkForMaturity();
		}

		/// <summary>
		/// Runs through all start-of-day checks.
		/// Temporarily stubbed for incompatibilities.
		/// </summary>
		/// <param name="location">Used for default game behaviours.</param>
		public override void DayUpdate(GameLocation location)
		{
		}

		/// <summary>
		/// Updates object quantity as the per-day maturity timer counts down.
		/// </summary>
		public new void checkForMaturity()
		{
			// Stop adding to the stack when the limit is reached.
			Const.MushroomQuantityLimits.TryGetValue(
				heldObject.Value.ParentSheetIndex, out var max);
			if (Quantity >= max)
				return;

			// Progress the growth of the stack per each mushroom's rate.
			daysToMature.Value += Math.Max(1, defaultDaysToMature * agingRate);
			minutesUntilReady.Value = 999999;

			if (!(daysToMature >= defaultDaysToMature))
				return;

			// Mature the held mushroom over time, growing the quantity.
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

			location.playSound("woodWhack");
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
				var flag = (who.currentLocation is Cellar && ModEntry.Instance.Config.WorksInCellar)
				            || (who.currentLocation is FarmCave && ModEntry.Instance.Config.WorksInFarmCave)
				            || (who.currentLocation is BuildableGameLocation && ModEntry.Instance.Config.WorksInBuildings)
				            || (who.currentLocation is FarmHouse && ModEntry.Instance.Config.WorksInFarmHouse)
				            || (who.currentLocation.IsGreenhouse && ModEntry.Instance.Config.WorksInGreenhouse)
				            || (who.currentLocation.IsOutdoors && ModEntry.Instance.Config.WorksOutdoors);

				if (!flag)
				{
					// Ignore bad machine locations.
					Game1.showRedMessage(ModEntry.Instance.i18n.Get("error.location"));
					return false;
				}
			}
			
			// Ignore Truffles.
			if (!probe && dropIn.ParentSheetIndex.Equals(430))
			{
				Game1.showRedMessage(ModEntry.Instance.i18n.Get("error.truffle"));
				return false;
			}

			// nanda kore wa
			if (quality >= 4)
				return false;

			// Ignore wrong items.
			if (!Const.MushroomGrowingRates.TryGetValue(dropIn.ParentSheetIndex, out var rate))
				return false;

			if (probe)
				return true;

			// Accept the deposited item.
			putObject(dropIn, rate);
			who.currentLocation.playSound("Ship");
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
			var index1 = new Vector2(x / 64, y / 64);
			health = 10;

			// Determine player.
			if (who != null)
				owner.Value = who.UniqueMultiplayerID;
			else
				owner.Value = Game1.player.UniqueMultiplayerID;

			// Spawn object.
			location.objects.Add(index1, new Propagator(index1));
			location.playSound("hammer");
			if (!performDropDownAction(who))
			{
				var one = (StardewValley.Object)getOne();
				one.shakeTimer = 50;
				one.TileLocation = index1;

				// Avoid placement conflicts.
				if (location.objects.ContainsKey(index1))
				{
					if (location.objects[index1].ParentSheetIndex != ParentSheetIndex)
					{
						Game1.createItemDebris(location.objects[index1], index1 * 64f, -1);
						location.objects[index1] = one;
					}
				}
				else
					location.objects.Add(index1, one);
				one.initializeLightSource(index1);
			}
			location.playSound("woodyStep");
			return true;
		}
		
		public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
		{
			// Draw the base sprite.
			var whichMushroom = 0;
			if (heldObject.Value != null)
				Const.MushroomSourceRects.TryGetValue(
					heldObject.Value.ParentSheetIndex, out whichMushroom);
			var vector2 = getScale() * 4f;
			var local = Game1.GlobalToLocal(Game1.viewport, 
				new Vector2( x * 64, y * 64 - 64));
			var destRect = new Rectangle(
					(int)(local.X - vector2.X / 2f) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0),
					(int)(local.Y - vector2.Y / 2f) + (shakeTimer > 0 ? Game1.random.Next(-1, 2) : 0),
					(int)(64f + vector2.X), 
					(int)(128f + vector2.Y / 2f));
			b.Draw(
					Game1.bigCraftableSpriteSheet,
					destRect,
					getSourceRectForBigCraftable(ParentSheetIndex),
					Color.White * alpha, 
					0f, 
					Vector2.Zero,
					SpriteEffects.None,
					Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f)
					+ (Game1.currentLocation.IsOutdoors ? 0f : x * 1f / 10000f));

			if (heldObject.Value == null)
				return;

			// Draw the held object overlay.
			Const.MushroomQuantityLimits.TryGetValue(
				heldObject.Value.ParentSheetIndex, out var max);
			var whichFrame = getWhichFrameForOverlay(
				(int)daysToMature.Value, Quantity, max);

			b.Draw(ModEntry.OverlayTexture,
					destRect,
					getSourceRectForOverlay(whichMushroom, whichFrame),
					Color.White,
					0f,
					Vector2.Zero,
					SpriteEffects.None,
					Math.Max(0.0f, ((y + 1) * 64 - 24) / 10000f) 
					+ (Game1.currentLocation.IsOutdoors ? 0f : x * 1f / 10000f) + 1f / 10000f);
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
			float.TryParse(additionalSaveData["tileLocationX"], out var x);
			float.TryParse(additionalSaveData["tileLocationY"], out var y);
			TileLocation = new Vector2(x, y);

			int.TryParse(additionalSaveData["days"], out var days);
			bool.TryParse(additionalSaveData["produceExtra"], out var produceExtra);
			int.TryParse(additionalSaveData["heldObjectIndex"], out var heldObjectIndex);
			int.TryParse(additionalSaveData["heldObjectQuality"], out var heldObjectQuality);
			int.TryParse(additionalSaveData["heldObjectQuantity"], out var heldObjectQuantity);
			loadHeldObject(heldObjectIndex, heldObjectQuality, heldObjectQuantity, days, produceExtra);

			loadDefaultValues();
			loadObjectData();
			
			Log.T($"Rebuilt {Name} ({ParentSheetIndex}) at({TileLocation.X}/{TileLocation.Y})");
		}
	}
}
