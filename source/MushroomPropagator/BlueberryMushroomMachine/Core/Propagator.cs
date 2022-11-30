/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using System;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using Object = StardewValley.Object;

namespace BlueberryMushroomMachine
{
    [XmlType("Mods_BlueberryMushroomMachine")]
	public class Propagator : StardewValley.Object
	{
		// Source mushroom (placed by player)
		public string SourceMushroomName;
		public int SourceMushroomIndex;
		public int SourceMushroomQuality;
		
		// Extra mushrooms (grown overnight)
		public readonly int DefaultDaysToMature;
		public float RateToMature;
		public float DaysToMature;
		public int MaximumStack;

		public Propagator() : this(Vector2.Zero)
		{
		}
		
		public Propagator(Vector2 tileLocation)
		{
			TileLocation = tileLocation;
			Initialise();
			DefaultDaysToMature = ModEntry.Instance.Config.MaximumDaysToMature;
		}

		protected override string loadDisplayName()
		{
			return ModEntry.Instance.i18n.Get("machine.name");
		}

		public override string getDescription()
		{
			return ModEntry.Instance.i18n.Get("machine.desc");
		}
		
		private void Initialise()
		{
            if (ModEntry.Instance.Config.DebugMode)
            {
                Log.D($"Initialise propagator at {TileLocation}",
                    ModEntry.Instance.Config.DebugMode);
            }

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

			boundingBox.Value = new Rectangle((int)TileLocation.X * 64, (int)TileLocation.Y * 64, 64, 64);
		}

		/// <summary>
		/// Adds an instance of the given item to be used as a source mushroom by the machine,
		/// and resets all growth and harvest variables.
		/// </summary>
		/// <param name="dropIn">Some instance of an object, hopefully a mushroom.</param>
		public void PutSourceMushroom(Object dropIn)
		{
			ModEntry.GetMushroomGrowthRate(dropIn, out RateToMature);
			ModEntry.GetMushroomMaximumQuantity(dropIn, out MaximumStack);
			SourceMushroomName = dropIn.Name;
			SourceMushroomIndex = dropIn.ParentSheetIndex;
			SourceMushroomQuality = dropIn.Quality;
			DaysToMature = 0;
			minutesUntilReady.Value = 999999;

			Log.D($"PutSourceMushroom(item: [{dropIn.ParentSheetIndex}] {dropIn.Name} Q{dropIn.Quality}), stack to {MaximumStack}" +
				$" at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);
		}

		public void PutExtraHeldMushroom(float daysToMature)
		{
			heldObject.Value = new Object(SourceMushroomIndex, 1);
			DaysToMature = daysToMature;
			readyForHarvest.Value = false;
			minutesUntilReady.Value = 999999;
		}

		public bool PopByAction()
		{
			Log.D($"PopByAction at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);
			if (SourceMushroomIndex > 0)
			{
				PopExposedMushroom(false);
				return true;
			}
			return false;
		}

		public bool PopByTool()
		{
			Log.D($"PopByTool at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);

			if (SourceMushroomIndex > 0)
			{
				// Extract any held mushrooms from machine
				PopExposedMushroom(true);
			}
			else
			{
				// Extract machine from location
				PopMachine();
			}
			return false;
		}

		public void PopExtraHeldMushrooms(bool giveNothing)
		{
			Log.D($"PopExtraHeldMushrooms at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);

			// Incorporate Gatherer's skill effects for bonus production
			var popQuantity = heldObject.Value.Stack;
			if (Game1.player.professions.Contains(Farmer.gatherer)
				&& new Random().Next(5) == 0)
				popQuantity += 1;

			var popQuality = Game1.player.professions.Contains(Farmer.botanist) ? 4 : SourceMushroomQuality;
			var popObject = new Object(SourceMushroomIndex, 1, false, -1, popQuality);

			// Create mushroom item drops in the world from the machine
			if (!giveNothing)
			{
				for (var i = 0; i < popQuantity; ++i)
				{
					Game1.createItemDebris(popObject.getOne(),
						new Vector2(TileLocation.X * 64 + 32, TileLocation.Y * 64 + 32), -1);
				}
			}

			// Clear the extra mushroom data
			heldObject.Value = null;
		}

		/// <summary>
		/// Pops the extra mushrooms in the 'heldItem' slot if they exist, otherwise pops the source mushroom and resets to 'empty'.
		/// </summary>
		/// <param name="forceRemoveSource">
		/// Whether or not to pop the source mushroom in addition to any extra mushrooms, leaving the machine considered 'empty'.
		/// </param>
		public void PopExposedMushroom(bool forceRemoveSource)
		{
            if (ModEntry.Instance.Config.DebugMode)
            {
                Log.D($"PopExposedMushroom(forceRemoveSource: {forceRemoveSource})"
                    + $" (item: [{SourceMushroomIndex}] {SourceMushroomName} Q{SourceMushroomQuality})" +
                    $" at {Game1.currentLocation?.Name} {TileLocation}",
                    ModEntry.Instance.Config.DebugMode);
            }

			Game1.playSound("harvest");
			var popSource = forceRemoveSource || heldObject.Value == null;

			// Pop the extra mushrooms, leaving the source mushroom to continue producing
			if (heldObject.Value != null)
			{
				PopExtraHeldMushrooms(giveNothing: false);
				minutesUntilReady.Value = 999999;
			}

			// Pop the source mushroom, resetting the machine to default
			if (popSource)
			{
				Log.D("PopExposed source",
					ModEntry.Instance.Config.DebugMode);
				Game1.createItemDebris(new Object(SourceMushroomIndex, 1) { Quality = SourceMushroomQuality },
					new Vector2(TileLocation.X * 64 + 32, TileLocation.Y * 64 + 32), -1);
				MaximumStack = 1;
				SourceMushroomName = null;
				SourceMushroomIndex = 0;
				SourceMushroomQuality = 0;
				minutesUntilReady.Value = -1;
			}

			// Reset growing and harvesting info
			readyForHarvest.Value = false;
			DaysToMature = 0;
		}

		/// <summary>
		/// Behaviours for tool actions to uproot the machine itself.
		/// </summary>
		public void PopMachine()
		{
			Log.D($"PopMachine at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);
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
		/// Perform all start-of-day checks for the Propagator to handle held object events.
		/// </summary>
		internal void DayUpdate()
		{
			Log.D($"DayUpdate (item: [{SourceMushroomIndex}] {SourceMushroomName} Q{SourceMushroomQuality} at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);

			// Indexing inconsistencies with JA/CFR
			ParentSheetIndex = ModValues.PropagatorIndex;

			// Grow mushrooms overnight
			if (SourceMushroomIndex > 0)
			{
				GrowHeldMushroom();
			}
		}

		/// <summary>
		/// Updates object quantity as the per-day maturity timer counts up to its threshold for this type of mushroom.
		/// </summary>
		internal void GrowHeldMushroom()
		{
			if (heldObject.Value == null)
			{
				// Set the extra mushroom object
				PutExtraHeldMushroom(daysToMature: 0);
				Log.D("==> Set first extra mushroom",
					ModEntry.Instance.Config.DebugMode);
				return;
			}
			else
			{
				// Stop adding to the stack when its limit is reached
				if (heldObject.Value.Stack >= MaximumStack)
				{
					Log.D("==> Reached max stack size, ready for harvest",
						ModEntry.Instance.Config.DebugMode);
					minutesUntilReady.Value = 0;
					readyForHarvest.Value = true;
					return;
				}

				// Progress the growth of the stack per each mushroom's rate
				DaysToMature += (int)(Math.Floor(Math.Max(1, DefaultDaysToMature * RateToMature)));
				minutesUntilReady.Value = 999999;

				if (DaysToMature >= DefaultDaysToMature)
				{
					// When the held mushroom reaches a maturity stage, the stack grows
					++heldObject.Value.Stack;
					DaysToMature = 0;
				}
			}
			Log.D($"==> Grown to {heldObject.Value.Stack} ({DaysToMature}/{DefaultDaysToMature} days +{RateToMature}), stack {heldObject.Value.Stack}/{MaximumStack}",
				ModEntry.Instance.Config.DebugMode);
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
			if (!justCheckingForActivity)
				Log.D($"checkForAction at {Game1.currentLocation?.Name} {TileLocation}",
					ModEntry.Instance.Config.DebugMode);
			
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
			Log.D($"performUseAction at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);

			return PopByAction();
		}

		/// <summary>
		/// Overrides the usual hit-with-tool behaviour to change the requirements
		/// and allow for popping held mushrooms at different stages.
		/// </summary>
		/// <returns>Whether or not to continue with base behaviour.</returns>
		public override bool performToolAction(Tool t, GameLocation location)
		{
			Log.D($"performToolAction at {Game1.currentLocation?.Name} {TileLocation}",
				ModEntry.Instance.Config.DebugMode);

			// Ignore usages that wouldn't trigger actions for other machines
			if (t == null || !t.isHeavyHitter() || t is StardewValley.Tools.MeleeWeapon)
				return base.performToolAction(t, location);

			location.playSound("woodWhack");
			return PopByTool();
		}

		/// <summary>
		/// Overrides usual use-with-item behaviours to limit the set to working in
		/// specific locations with specific items, as well as other funky behaviour.
		/// </summary>
		/// <param name="dropIn">Our candidate item.</param>
		/// <param name="probe">Base game check for determining outcomes without consequences.</param>
		/// <param name="who">Farmer using the machine.</param>
		/// <returns>
		/// Whether the dropIn object is appropriate for this machine in this context.
		/// </returns>
		public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
		{
			if (!probe)
				Log.D($"performObjectDropInAction(dropIn:{dropIn?.Name ?? "null"}) at {Game1.currentLocation?.Name} {TileLocation}",
					ModEntry.Instance.Config.DebugMode);

            // Ignore usages with inappropriate items
            if (dropIn == null)
            {
                return false;
            }

            // Ignore Truffles
            if (Utility.IsNormalObjectAtParentSheetIndex(dropIn, 430))
            {
                if (!probe)
                    Game1.showRedMessage(ModEntry.Instance.i18n.Get("error.truffle"));
                return false;
            }

            // Ignore things that are not mushrooms.
            if (dropIn is not Object obj || obj.bigCraftable.Value || !ModEntry.IsValidMushroom(obj))
            {
                if (!probe)
                    Log.D($"Invalid mushroom: [{dropIn.ParentSheetIndex}] {dropIn.Name}",
                        ModEntry.Instance.Config.DebugMode);
                return false;
            }

            // Determine if being used in an appropriate location
            if (who != null)
            {

                if (!((who.currentLocation is Cellar && ModEntry.Instance.Config.WorksInCellar)
                            || (who.currentLocation is FarmCave && ModEntry.Instance.Config.WorksInFarmCave)
                            || (who.currentLocation is BuildableGameLocation && ModEntry.Instance.Config.WorksInBuildings)
                            || (who.currentLocation is FarmHouse && ModEntry.Instance.Config.WorksInFarmHouse)
                            || (who.currentLocation.IsGreenhouse && ModEntry.Instance.Config.WorksInGreenhouse)
                            || (who.currentLocation.IsOutdoors && ModEntry.Instance.Config.WorksOutdoors)))
                {
                    // Ignore bad machine locations
                    if (!probe)
                        Game1.showRedMessage(ModEntry.Instance.i18n.Get("error.location"));
                    return false;
                }
            }

            // don't make state changes if just checking.
            if (probe)
                return true;

            // Extract held mushrooms prematurely
            if (SourceMushroomIndex > 0)
			{
				if (heldObject.Value != null)
				{
					// Get a copy of the root mushroom
					PopExposedMushroom(false);
				}
				else if (!ModEntry.Instance.Config.OnlyToolsCanRemoveRootMushrooms)
				{
					// Remove the root mushroom if it hasn't settled overnight
					PopExposedMushroom(true);
				}
			}

			// Accept the deposited item as the new source mushroom
			PutSourceMushroom(obj);
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

			// Add this propagator to the location as a placed object
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

			if (SourceMushroomIndex < 1)
				return;

			// Draw the held object overlay
			var isCustomMushroom = !Enum.IsDefined(typeof(ModEntry.Mushrooms), SourceMushroomIndex);

			var whichFrame = ModEntry.GetOverlayGrowthFrame(DaysToMature, DefaultDaysToMature, heldObject.Value != null ? heldObject.Value.Stack : 0, MaximumStack);
			sourceRect = ModEntry.GetOverlaySourceRect(SourceMushroomIndex, whichFrame);

			if (isCustomMushroom)
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
				!isCustomMushroom ? ModEntry.OverlayTexture : Game1.objectSpriteSheet,
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
	}
}
