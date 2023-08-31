/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/b-b-blueberry/BlueberryMushroomMachine
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Locations;
using System;
using System.Xml.Serialization;
using Object = StardewValley.Object;

namespace BlueberryMushroomMachine
{
	[XmlType("Mods_BlueberryMushroomMachine")]
	public class Propagator : Object
	{
		// Source mushroom (placed by player)

		/// <summary>
		/// Name of root mushroom used as a template for the grown held object.
		/// Used for persistent custom objects in SDV 1.5.
		/// </summary>
		public string SourceMushroomName;
		/// <summary>
		/// Unique ID of root mushroom used as a template for the grown held object.
		/// Used for persistent base game objects in SDV 1.5.
		/// </summary>
		public int SourceMushroomIndex;
		/// <summary>
		/// Quality of root mushroom used as a template for the grown held object.
		/// All grown objects will copy the quality of the source mushroom.
		/// </summary>
		public int SourceMushroomQuality;

		// Extra mushrooms (grown overnight)

		/// <summary>
		/// Number of days, including day of placement, before a held object
		/// with a growth rate of 1 will have its stack increased.
		/// </summary>
		public static int DefaultDaysToGrow => ModEntry.Config.MaximumDaysToMature;
		/// <summary>
		/// Number of days, including day of placement, before held object stack increases by one.
		/// </summary>
		public int DaysToGrowOnce => (int)(Propagator.DefaultDaysToGrow / this.GrowthRatePerDay);
		/// <summary>
		/// Number of days, including day of placement, before growth complete with maximum stack.
		/// </summary>
		public int DaysToReady => this.DaysToGrowOnce * this.MaximumStack;
		/// <summary>
		/// Rate to increase growth value as scaled to <see cref="Propagator.DefaultDaysToGrow"/>.
		/// </summary>
		public float GrowthRatePerDay;
		/// <summary>
		/// Current growth value as a proportion of <see cref="Propagator.DefaultDaysToGrow"/>.
		/// Starting value of 0, increases each day, reset on reaching value of <see cref="Propagator.DefaultDaysToGrow"/>
		/// </summary>
		public float Growth;
		/// <summary>
		/// Stack size of held object on growth complete.
		/// </summary>
		public int MaximumStack;

		// Common definitions

		/// <summary>
		/// Display name of all <see cref="Propagator"/> objects.
		/// </summary>
		public static string PropagatorDisplayName => ModEntry.I18n.Get("machine.name");
		/// <summary>
		/// Display name of all <see cref="Propagator"/> objects.
		/// </summary>
		public static string PropagatorDescription => ModEntry.I18n.Get("machine.desc");
		/// <summary>
		/// Dimensions of machine sprite in source texture.
		/// </summary>
		public static Point MachineSize => new(x: Game1.smallestTileSize, y: Game1.smallestTileSize * 2);
		/// <summary>
		/// Dimensions of overlay sprite in source texture.
		/// </summary>
		public static Point OverlaySize => new(x: 24, y: 32);
		/// <summary>
		/// Minutes between the current time and the next ready day.
		/// This value does not affect actual growth rate, only the player-visible timer: growth occurs separately after each day.
		/// </summary>
		public int PropagatorWorkingMinutes => Utility.CalculateMinutesUntilMorning(
			currentTime: Game1.timeOfDay,
			daysElapsed: this.DaysToReady - 1);

		public Propagator() : this(tileLocation: Vector2.Zero)
		{
		}

		public Propagator(Vector2 tileLocation)
		{
			this.TileLocation = tileLocation;
			this.Initialise();
		}

		protected override string loadDisplayName()
		{
			return Propagator.PropagatorDisplayName;
		}

		public override string getDescription()
		{
			return Propagator.PropagatorDescription;
		}

		/// <summary>
		/// Assigns members based on definition given in <see cref="Game1.bigCraftablesInformation"/> from <see cref="ModValues.ObjectData"/>.
		/// </summary>
		private void Initialise()
		{
			// Basic properties
			this.Name = ModValues.PropagatorInternalName;
			this.ParentSheetIndex = ModValues.PropagatorIndex;
			this.DisplayName = this.loadDisplayName();

			// Craftable properties
			this.CanBeSetDown = true;
			this.bigCraftable.Value = true;
			this.boundingBox.Value = new Rectangle(
				location: (this.TileLocation * Game1.tileSize).ToPoint(),
				size: new Point(Game1.tileSize));

			// Object properties from data values
			if (ModValues.ObjectData is null)
			{
				// Ignore setup if data values not defined,
				// e.g. objects populated for save on main menu
				return;
			}

			string[] fields = ModValues.ObjectData.Split('/');
			this.Price = Convert.ToInt32(fields[1]);
			this.Edibility = Convert.ToInt32(fields[2]);
			string[] typeAndCategory = fields[3].Split(' ');
			this.Type = typeAndCategory[0];
			if (typeAndCategory.Length > 1)
			{
				this.Category = Convert.ToInt32(typeAndCategory[1]);
			}
			this.setOutdoors.Value = Convert.ToBoolean(fields[5]);
			this.setIndoors.Value = Convert.ToBoolean(fields[6]);
			this.Fragility = Convert.ToInt32(fields[7]);
			this.isLamp.Value = fields.Length > 8 && Convert.ToBoolean(fields[8]);
			this.initializeLightSource(tileLocation: this.TileLocation);
		}

		/// <summary>
		/// Adds an instance of the given item to be used as a source mushroom by the machine,
		/// and resets all growth and harvest variables.
		/// </summary>
		/// <param name="dropIn">
		/// Some instance of an object, hopefully a mushroom.
		/// </param>
		public void SetSourceObject(Object dropIn)
		{
			Utils.GetMushroomGrowthRate(o: dropIn, rate: out this.GrowthRatePerDay);
			Utils.GetMushroomMaximumQuantity(o: dropIn, quantity: out this.MaximumStack);

			this.SourceMushroomName = dropIn.Name;
			this.SourceMushroomIndex = dropIn.ParentSheetIndex;
			this.SourceMushroomQuality = dropIn.Quality;
			this.Growth = 0;
			this.MinutesUntilReady = this.PropagatorWorkingMinutes;
		}

		/// <summary>
		/// Starts propagator growth by adding a single held object.
		/// </summary>
		/// <param name="daysToMature">
		/// Initial maturity value.
		/// </param>
		public void SetHeldObject(float daysToMature)
		{
			this.heldObject.Value = new Object(parentSheetIndex: this.SourceMushroomIndex, initialStack: 1);
			this.Growth = daysToMature;
			this.readyForHarvest.Value = false;
			this.MinutesUntilReady = this.PropagatorWorkingMinutes;
		}

		public bool PopByAction()
		{
			return this.PopHeldOrSourceObject(isSourceForcedOut: false);
		}

		public bool PopByTool()
		{
			if (this.SourceMushroomIndex > 0)
			{
				// Extract any held mushrooms from machine
				this.PopHeldOrSourceObject(isSourceForcedOut: true);
			}
			else
			{
				// Extract machine from location
				this.PopMachine();
			}
			return false;
		}

		/// <summary>
		/// Pops held objects, dropping them on the ground with respect to player professions.
		/// </summary>
		/// <param name="giveNothing">
		/// Whether to destroy objects rather than dropping.
		/// </param>
		/// <returns>
		/// Whether any objects were dropped when popped.
		/// </returns>
		public bool PopHeldObject(bool giveNothing)
		{
			// Incorporate Gatherer's skill effects for bonus quantity
			int popQuantity = this.heldObject.Value.Stack;
			if (Game1.player.professions.Contains(Farmer.gatherer) && new Random().Next(5) == 0)
			{
				popQuantity += 1;
			}

			// Incorporate Botanist's skill effects for bonus quality
			int popQuality = Game1.player.professions.Contains(Farmer.botanist)
				? Object.bestQuality
				: this.SourceMushroomQuality;
			Object popObject = new(
				parentSheetIndex: this.SourceMushroomIndex,
				initialStack: 1,
				isRecipe: false,
				price: -1,
				quality: popQuality);

			// Create mushroom item drops in the world from the machine
			if (!giveNothing)
			{
				for (int i = 0; i < popQuantity; ++i)
				{
					Game1.createItemDebris(
						item: popObject.getOne(),
						origin: (this.TileLocation + new Vector2(0.5f)) * Game1.tileSize,
						direction: -1);
				}
			}

			// Clear the extra mushroom data
			this.heldObject.Value = null;
			this.readyForHarvest.Value = false;
			this.Growth = 0;

			return popQuantity > 0 && !giveNothing;
		}

		/// <summary>
		/// Pops the extra mushrooms in the 'heldItem' slot if they exist, otherwise pops the source mushroom and resets to 'empty'.
		/// </summary>
		/// <param name="isSourceForcedOut">
		/// Whether or not to pop the source mushroom in addition to any extra mushrooms, leaving the machine considered 'empty'.
		/// </param>
		/// <returns>
		/// Whether any objects were popped.
		/// </returns>
		public bool PopHeldOrSourceObject(bool isSourceForcedOut)
		{
			bool isPopped = false;
			bool isPoppingSource = isSourceForcedOut || this.heldObject.Value is null;

			// Pop the extra mushrooms, leaving the source mushroom to continue producing
			if (this.heldObject.Value is not null)
			{
				Game1.playSound("harvest");
				isPopped = this.PopHeldObject(giveNothing: false);
				this.MinutesUntilReady = this.PropagatorWorkingMinutes;
			}

			// Pop the source mushroom, resetting the machine to default
			if (isPoppingSource && this.SourceMushroomIndex > 0)
			{
				Game1.playSound("harvest");
				isPopped = true;
				Object o = new (parentSheetIndex: this.SourceMushroomIndex, initialStack: 1)
				{
					Quality = this.SourceMushroomQuality
				};
				Game1.createItemDebris(
					item: o,
					origin: (this.TileLocation + new Vector2(value: 0.5f)) * Game1.tileSize, direction: -1);
				this.MaximumStack = 1;
				this.SourceMushroomName = null;
				this.SourceMushroomIndex = 0;
				this.SourceMushroomQuality = 0;
				this.MinutesUntilReady = -1;
			}

			return isPopped;
		}

		/// <summary>
		/// Behaviours for tool actions to uproot the machine itself.
		/// </summary>
		public void PopMachine()
		{
			Vector2 toolPosition = Game1.player.GetToolLocation();
			Vector2 propagatorPosition = this.boundingBox.Center.ToVector2();
			Vector2 key = Vector2.Floor(toolPosition / Game1.tileSize);
			Game1.currentLocation.debris.Add(new Debris(
				item: new Propagator(tileLocation: this.TileLocation),
				debrisOrigin: toolPosition,
				targetLocation: propagatorPosition));
			Game1.currentLocation.Objects.Remove(key);
		}

		/// <summary>
		/// Perform all start-of-day checks for the Propagator to handle held object events.
		/// </summary>
		public override void DayUpdate(GameLocation location)
		{
			// Grow mushrooms overnight
			this.GrowHeldObject();
		}

		/// <summary>
		/// Updates object quantity as the per-day maturity timer counts up to its threshold for this type of mushroom.
		/// </summary>
		public void GrowHeldObject()
		{
			if (this.SourceMushroomIndex <= 0)
			{
				return;
			}
			else if (this.heldObject.Value is null)
			{
				// Set the extra mushroom object
				this.SetHeldObject(daysToMature: 0);
				return;
			}
			else if (this.heldObject.Value.Stack >= this.MaximumStack)
			{
				// Stop adding to the stack when its limit is reached
				return;
			}
			else
			{
				// Progress the growth of the stack per each mushroom's rate
				this.Growth += (int)Math.Floor(Math.Max(1, this.GrowthRatePerDay * Propagator.DefaultDaysToGrow));

				// When the held mushroom reaches a maturity stage, the stack grows
				if (this.Growth >= Propagator.DefaultDaysToGrow)
				{
					++this.heldObject.Value.Stack;
					this.Growth = 0;
				}

				// Mark as ready on max stack size
				if (this.heldObject.Value.Stack >= this.MaximumStack)
				{
					this.MinutesUntilReady = 0;
					this.readyForHarvest.Value = true;
				}
			}
		}

		/// <summary>
		/// Override method for any player cursor passive or active interactions with the machine.
		/// Permits triggering behaviours to pop mushrooms before they're ready with the action hotkey.
		/// </summary>
		/// <param name="who">
		/// Farmer interacting with the machine.
		/// </param>
		/// <param name="justCheckingForActivity">
		/// Whether the cursor hovered or clicked.
		/// </param>
		/// <returns>
		/// Whether to continue with base method.
		/// </returns>
		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			Point tile = new(x: who.getTileX(), y: who.getTileY());
			if (!justCheckingForActivity && who is not null
					&& who.currentLocation.isObjectAtTile(tile.X, tile.Y - 1)
					&& who.currentLocation.isObjectAtTile(tile.X, tile.Y + 1)
					&& who.currentLocation.isObjectAtTile(tile.X + 1, tile.Y)
					&& who.currentLocation.isObjectAtTile(tile.X - 1, tile.Y)
					&& !who.currentLocation.getObjectAtTile(tile.X, tile.Y - 1).isPassable()
					&& !who.currentLocation.getObjectAtTile(tile.X, tile.Y + 1).isPassable()
					&& !who.currentLocation.getObjectAtTile(tile.X - 1, tile.Y).isPassable()
					&& !who.currentLocation.getObjectAtTile(tile.X + 1, tile.Y).isPassable())
			{
				this.performToolAction(t: null, location: who.currentLocation);
			}

			return justCheckingForActivity || base.checkForAction(who: who, justCheckingForActivity: justCheckingForActivity);
		}

		/// <summary>
		/// Allows the user to pop extra mushrooms before they're ready,
		/// and pop root mushrooms without extras.
		/// Author's note: The mushrooms are never ready.
		/// </summary>
		/// <returns>
		/// Whether to continue with base behaviour.
		/// </returns>
		public override bool performUseAction(GameLocation location)
		{
			return this.PopByAction();
		}

		/// <summary>
		/// Overrides the usual hit-with-tool behaviour to change the requirements
		/// and allow for popping held mushrooms at different stages.
		/// </summary>
		/// <returns>
		/// Whether or not to continue with base behaviour.
		/// </returns>
		public override bool performToolAction(Tool t, GameLocation location)
		{
			// Ignore usages that wouldn't trigger actions for other machines
			if (t is null || !t.isHeavyHitter() || t is StardewValley.Tools.MeleeWeapon)
			{
				return base.performToolAction(t, location);
			}

			location.playSound("woodWhack");
			return this.PopByTool();
		}

		/// <summary>
		/// Overrides usual use-with-item behaviours to limit the set to working in
		/// specific locations with specific items, as well as other funky behaviour.
		/// </summary>
		/// <param name="dropIn">
		/// Our candidate item.
		/// </param>
		/// <param name="probe">
		/// Base game check for determining outcomes without consequences.
		/// </param>
		/// <param name="who">
		/// Farmer using the machine.
		/// </param>
		/// <returns>
		/// Whether the dropIn object is appropriate for this machine in this context.
		/// </returns>
		public override bool performObjectDropInAction(Item dropIn, bool probe, Farmer who)
		{
			// Ignore usages with inappropriate items
			if (dropIn is null)
			{
				return false;
			}

			// Ignore Truffles
			if (Utility.IsNormalObjectAtParentSheetIndex(dropIn, 430))
			{
				if (!probe)
				{
					Game1.showRedMessage(message: ModEntry.I18n.Get("error.truffle"));
				}
				return false;
			}

			// Ignore things that are not mushrooms
			if (dropIn is not Object obj || obj.bigCraftable.Value || !Utils.IsValidMushroom(o: obj))
			{
				return false;
			}

			// Ignore if location is not appropriate
			if (who is not null)
			{
				if (!((who.currentLocation is Cellar && ModEntry.Config.WorksInCellar)
					|| (who.currentLocation is FarmCave or IslandFarmCave && ModEntry.Config.WorksInFarmCave)
					|| (who.currentLocation is BuildableGameLocation && ModEntry.Config.WorksInBuildings)
					|| (who.currentLocation is FarmHouse && ModEntry.Config.WorksInFarmHouse)
					|| (who.currentLocation.IsGreenhouse && ModEntry.Config.WorksInGreenhouse)
					|| (who.currentLocation.IsOutdoors && ModEntry.Config.WorksOutdoors)))
				{
					// Ignore bad machine locations
					if (!probe)
					{
						Game1.showRedMessage(message: ModEntry.I18n.Get("error.location"));
					}
					return false;
				}
			}

			// Don't make state changes if just checking
			if (probe)
			{
				return true;
			}

			// Extract held mushrooms prematurely
			if (this.SourceMushroomIndex > 0)
			{
				if (this.heldObject.Value is not null)
				{
					// Remove held objects
					this.PopHeldOrSourceObject(isSourceForcedOut: false);
				}
				else if (!ModEntry.Config.OnlyToolsCanRemoveRootMushrooms)
				{
					// Replace the source object
					this.PopHeldOrSourceObject(isSourceForcedOut: true);
				}
			}

			// Set dropIn object as source mushroom
			if (this.SourceMushroomIndex <= 0)
			{
				this.SetSourceObject(dropIn: obj);
				who?.currentLocation.playSound("Ship");
				return true;
			}

			return false;
		}

		/// <summary>
		/// Awkward override to specifically place a Propagator instead of a BigCraftable Object.
		/// </summary>
		/// <returns>
		/// True
		/// </returns>
		public override bool placementAction(GameLocation location, int x, int y, Farmer who = null)
		{
			Vector2 tile = new Vector2(x: x, y: y) / Game1.tileSize;
			this.health = 10;

			// Determine player
			this.owner.Value = who?.UniqueMultiplayerID ?? Game1.player.UniqueMultiplayerID;

			// Add this propagator to the location as a placed object
			if (!this.performDropDownAction(who) && this.getOne() is Propagator propagator)
			{
				propagator.TileLocation = tile;
				propagator.shakeTimer = 50;

				if (location.objects.ContainsKey(tile))
				{
					if (location.objects[tile] is not Propagator)
					{
						Game1.createItemDebris(
							item: location.objects[tile],
							origin: tile * Game1.tileSize, direction: -1);
						location.objects[tile] = propagator;
					}
				}
				else
				{
					location.objects.Add(tile, propagator);
				}
				propagator.initializeLightSource(tileLocation: tile);
			}
			location.playSound("woodyStep");
			return true;
		}

		public override void draw(SpriteBatch b, int x, int y, float alpha = 1f)
		{
			Point scaleSizeToPulse(Point size, Vector2 pulse) => (size.ToVector2() * Game1.pixelZoom + new Vector2(x: pulse.X, y: pulse.Y / 2)).ToPoint();
			
			Point shake = this.shakeTimer < 1
				? Point.Zero
				: new Point(x: Game1.random.Next(-1, 2), Game1.random.Next(-1, 2));
			Vector2 pulse = ModEntry.Config.PulseWhenGrowing
				? this.getScale() * Game1.pixelZoom
				: Vector2.One;
			Vector2 position = Game1.GlobalToLocal(
				viewport: Game1.viewport,
				globalPosition: new Vector2(x: x, y: y - 1) * Game1.tileSize);
			Rectangle destination = new(
				location: (position - pulse / 2).ToPoint() + shake,
				size: scaleSizeToPulse(size: Propagator.MachineSize, pulse: pulse));
			Rectangle source = Utils.GetMachineSourceRect(
				location: Game1.currentLocation,
				tile: this.TileLocation);
			float layerDepth = Math.Max(0.0f, ((y + 1) * Game1.tileSize - 24) / 10000f)
				+ (Game1.currentLocation.IsOutdoors ? 0f : x * 1f / 10000f);
			bool isFlipped = Utils.GetMachineIsFlipped(tile: this.TileLocation);

			// Draw the base sprite
			Propagator.DrawMachine(
				spriteBatch: b,
				destination: destination,
				origin: Vector2.Zero,
				color: Color.White,
				alpha: alpha,
				layerDepth: layerDepth,
				source: source,
				isFlipped: isFlipped);

			// End here if no mushrooms are held
			if (this.SourceMushroomIndex < 1)
			{
				return;
			}

			// Draw the held object overlay
			bool isBasicMushroom = Enum.IsDefined(enumType: typeof(ModEntry.Mushrooms), value: this.SourceMushroomIndex);
			int whichFrame = Utils.GetOverlayGrowthFrame(
				currentDays: this.Growth,
				goalDays: Propagator.DefaultDaysToGrow,
				currentStack: this.heldObject.Value?.Stack ?? 0,
				goalStack: this.MaximumStack);
			int frames = ModValues.OverlayMushroomFrames;

			if (isBasicMushroom)
			{
				// Centre mushroom overlay on base sprite
				destination.Offset(amount: (source.Size.ToVector2() - Propagator.OverlaySize.ToVector2()) * Game1.pixelZoom / 2);
				destination.Size = scaleSizeToPulse(size: Propagator.OverlaySize, pulse: pulse);
				source = Utils.GetOverlaySourceRect(
					location: Game1.currentLocation,
					index: this.SourceMushroomIndex,
					whichFrame: whichFrame);
			}
			else
			{
				// Scale custom mushroom object sprite to growth ratio
				float growthRatio = whichFrame / frames;
				float growthScale = Math.Min(0.8f, growthRatio) + 0.2f;
				destination = new Rectangle(
					x: (int)(position.X - pulse.X / 2f) + shake.X + (int)(32 * (1 - growthScale))
						+ (int)(pulse.X * growthScale / 4),
					y: (int)(position.Y - pulse.Y / 2f) + shake.Y + 48 + (int)(32 * (1 - growthScale))
						+ (int)(pulse.Y * growthScale / 8),
					width: (int)((Game1.tileSize + pulse.X) * growthScale),
					height: (int)((Game1.tileSize + pulse.Y / 2f) * growthScale));
			}


			b.Draw(
				texture: isBasicMushroom ? ModEntry.OverlayTexture : Game1.objectSpriteSheet,
				destinationRectangle: destination,
				sourceRectangle: source,
				color: Color.White,
				rotation: 0f,
				origin: Vector2.Zero,
				effects: isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				layerDepth: Math.Max(0.0f, ((y + 1) * Game1.tileSize - 24) / 10000f)
					+ (Game1.currentLocation.IsOutdoors ? 0f : x * 1f / 10000f) + 1f / 10000f + 1f / 10000f);
		}

		// Other draw method overrides added only to use custom machine texture in place of objects/craftables texture for base sprite:

		public override void draw(SpriteBatch spriteBatch, int xNonTile, int yNonTile, float layerDepth, float alpha = 1f)
		{
			if (this.isTemporarilyInvisible)
			{
				return;
			}

			Vector2 scaleFactor = this.getScale() * Game1.pixelZoom;
			Vector2 position = Game1.GlobalToLocal(
				viewport: Game1.viewport,
				globalPosition: new Vector2(xNonTile, yNonTile));
			Rectangle destination = new(
				x: (int)(position.X - (scaleFactor.X / 2f)) + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
				y: (int)(position.Y - (scaleFactor.Y / 2f)) + ((this.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0),
				width: (int)(Game1.tileSize + scaleFactor.X),
				height: (int)(Game1.tileSize * 2 + (scaleFactor.Y / 2f)));
			Propagator.DrawMachine(
				spriteBatch: spriteBatch,
				destination: destination,
				origin: Vector2.Zero,
				color: Color.White,
				alpha: alpha,
				layerDepth: layerDepth,
				source: Utils.GetMachineSourceRect(location: Game1.currentLocation, tile: this.TileLocation),
				isFlipped: Utils.GetMachineIsFlipped(tile: this.TileLocation));
		}

		public override void drawAsProp(SpriteBatch b)
		{
			if (this.isTemporarilyInvisible)
			{
				return;
			}

			Vector2 scale = this.getScale() * Game1.pixelZoom;
			Vector2 position = Game1.GlobalToLocal(
				viewport: Game1.viewport,
				globalPosition: (this.TileLocation + new Vector2(x: 0, y: -1)) * Game1.tileSize);
			Rectangle destination = new(
				x: (int)(position.X - (scale.X / 2f)),
				y: (int)(position.Y - (scale.Y / 2f)),
				width: (int)(Game1.tileSize + scale.X),
				height: (int)(Game1.tileSize * 2 + (scale.Y / 2f)));
			float layerDepth = Math.Clamp(value: ((this.TileLocation.Y + 1) * Game1.tileSize - 1) / 10000f, min: 0, max: 1);
			Propagator.DrawMachine(
				spriteBatch: b,
				destination: destination,
				origin: Vector2.Zero,
				color: Color.White,
				alpha: 1f,
				layerDepth: layerDepth,
				source: Utils.GetMachineSourceRect(location: Game1.currentLocation, tile: this.TileLocation),
				isFlipped: Utils.GetMachineIsFlipped(tile: this.TileLocation));
		}

		public override void drawWhenHeld(SpriteBatch spriteBatch, Vector2 objectPosition, Farmer f)
		{
			Rectangle destination = new(
				location: objectPosition.ToPoint(),
				size: (Propagator.MachineSize.ToVector2() * Game1.pixelZoom).ToPoint());
			float layerDepth = Math.Max(0f, (f.getStandingY() + 3f) / 10000f);
			Propagator.DrawMachine(
				spriteBatch: spriteBatch,
				destination: destination,
				origin: Vector2.Zero,
				color: Color.White,
				alpha: 1f,
				layerDepth: layerDepth,
				source: Utils.GetMachineSourceRect(location: Game1.currentLocation, tile: this.TileLocation));
		}

		public override void drawInMenu(SpriteBatch spriteBatch, Vector2 location, float scaleSize, float transparency, float layerDepth, StackDrawType drawStackNumber, Color color, bool drawShadow)
		{
			const float tinyScale = 3f;
			bool shouldDrawStackNumber = ((drawStackNumber == StackDrawType.Draw && this.maximumStackSize() > 1 && this.Stack > 1)
					|| drawStackNumber == StackDrawType.Draw_OneInclusive)
				&& scaleSize > 0.3f
				&& this.Stack != int.MaxValue;
			if (this.IsRecipe)
			{
				shouldDrawStackNumber = false;
				transparency = 0.5f;
				scaleSize *= 0.75f;
			}

			float scale = Game1.pixelZoom * (((double)scaleSize < 0.2) ? scaleSize : (scaleSize / 2f));
			Vector2 position = location + new Vector2(value: 1) * Game1.tileSize / 2;
			Rectangle destination = new(
				location: position.ToPoint(),
				size: (Propagator.MachineSize.ToVector2() * scale).ToPoint());
			Propagator.DrawMachine(
				spriteBatch: spriteBatch,
				destination: destination,
				origin: Propagator.MachineSize.ToVector2() / 2,
				color: color,
				alpha: transparency,
				layerDepth: layerDepth,
				source: Game1.uiMode ? null : Utils.GetMachineSourceRect(location: Game1.currentLocation, tile: this.TileLocation));

			if (shouldDrawStackNumber)
			{
				Utility.drawTinyDigits(
					toDraw: this.Stack,
					b: spriteBatch,
					position: location + new Vector2(
						x: Game1.tileSize - Utility.getWidthOfTinyDigitString(this.Stack, tinyScale * scaleSize) + (tinyScale * scaleSize),
						y: Game1.tileSize - (18f * scaleSize) + 2f),
					scale: tinyScale * scaleSize,
					layerDepth: 1f,
					c: color);
			}

			if (this.IsRecipe)
			{
				const int size = Game1.smallestTileSize;
				spriteBatch.Draw(
					texture: Game1.objectSpriteSheet,
					position: location + new Vector2(value: size),
					sourceRectangle: Game1.getSourceRectForStandardTileSheet(
						tileSheet: Game1.objectSpriteSheet,
						tilePosition: 451,
						width: size,
						height: size),
					color: color,
					rotation: 0f,
					origin: Vector2.Zero,
					scale: tinyScale,
					effects: SpriteEffects.None,
					layerDepth: layerDepth + 0.0001f);
			}
		}

		public override Item getOne()
		{
			return new Propagator(tileLocation: Vector2.Zero);
		}

		protected static void DrawMachine(SpriteBatch spriteBatch, Rectangle destination, Vector2 origin, Color color, float alpha, float layerDepth, Rectangle? source = null, bool isFlipped = false)
		{
			spriteBatch.Draw(
				texture: ModEntry.MachineTexture,
				destinationRectangle: destination,
				sourceRectangle: source ?? new Rectangle(location: Point.Zero, size: Propagator.MachineSize),
				color: Color.White * alpha,
				rotation: 0f,
				origin: origin,
				effects: isFlipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
				layerDepth: layerDepth);
		}
	}
}
