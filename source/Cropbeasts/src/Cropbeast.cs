/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/kdau/cropbeasts
**
*************************************************/

using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using System;
using System.Reflection;
using SObject = StardewValley.Object;

namespace Cropbeasts
{
	public class Cropbeast : FarmMonster
	{
		private GuestCropTile cropTile_;

		private readonly NetInt harvestIndex = new ();
		private readonly NetBool giantCrop = new ();
		private readonly NetPoint tileLocation = new ();

		public readonly NetBool containsPlant = new ();
		public readonly NetBool containsPrimaryHarvest = new ();
		public readonly NetBool containsSecondaryHarvest = new ();

		public readonly NetColor primaryColor = new ();
		public readonly NetColor secondaryColor = new ();

		public static Color ContrastTint => Config.HighContrast
			? Color.Goldenrod : Color.LightYellow;
		public static Color PrecomposedTint => Config.HighContrast
			? Color.LightYellow : Color.White;

		protected bool calculatedHarvest;
		public SObject primaryHarvest { get; protected set; }
		public SObject secondaryHarvest { get; protected set; }
		public SObject tertiaryHarvest { get; protected set; }

		private new readonly NetInt experienceGained = new ();

		protected readonly NetBool canWater = new (false);
		protected readonly NetInt wateringCharges = new (0);
		public static readonly int MaxWateringCharges = 40;

		protected bool reverted { get; private set; }

		protected Cropbeast ()
		{ }

		protected Cropbeast (CropTile cropTile, bool containsPlant,
			bool containsPrimaryHarvest, bool containsSecondaryHarvest,
			string beastName = null)
		: base (beastName ?? cropTile.mapping.beastName, cropTile.location,
			Utility.PointToVector2 (cropTile.tileLocation) * 64f +
				new Vector2 (0f, -32f))
		{
			cropTile_ = cropTile;
			harvestIndex.Value = cropTile.harvestIndex;
			giantCrop.Value = cropTile.giantCrop;
			tileLocation.Value = cropTile.tileLocation;

			this.containsPlant.Value = containsPlant;
			this.containsPrimaryHarvest.Value = containsPrimaryHarvest;
			this.containsSecondaryHarvest.Value = containsSecondaryHarvest;

			// Calculate the beast's coloration, making special provision for
			// seasonal color changes in Fiber.
			if (harvestIndex.Value == 771 &&
				Game1.GetSeasonForLocation (cropTile.location) == "fall")
			{
				primaryColor.Value = new Color (222, 155, 70); // #de9b46
				secondaryColor.Value = new Color (124, 30, 51); // #7c1e33
			}
			else if (harvestIndex.Value == 771 &&
				Game1.GetSeasonForLocation (cropTile.location) == "winter")
			{
				primaryColor.Value = new Color (65, 222, 178); // #41deb2
				secondaryColor.Value = new Color (45, 107, 199); // #2d6bc7
			}
			else
			{
				primaryColor.Value = cropTile.mapping.primaryColor
					?? TailoringMenu.GetDyeColor (cropTile.mapping.harvestObject)
					?? Color.White;
				secondaryColor.Value = cropTile.mapping.secondaryColor
					?? primaryColor.Value;
			}

			// Scale health and damage based on combat skill of players present.
			// Base values in Monsters.json are for level zero.
			MaxHealth = Health = scaleByCombatSkill (Health, 1.2f);
			DamageToFarmer = scaleByCombatSkill (DamageToFarmer, 0.2f);

			// Calculate the normal gain of farming experience.
			int price = cropTile.mapping.harvestObject.Price;
			experienceGained.Value = (int) Math.Ceiling (8.0 *
				Math.Log (0.018 * (double) price + 1.0, Math.E));

			// If a paddy crop, set up for watering.
			if (cropTile.crop.isPaddyCrop ())
			{
				canWater.Value = true;
				wateringCharges.Value = MaxWateringCharges;
			}
		}

		protected override void initNetFields ()
		{
			base.initNetFields ();
			NetFields.AddFields (harvestIndex, giantCrop, tileLocation,
				containsPlant, containsPrimaryHarvest, containsSecondaryHarvest,
				primaryColor, secondaryColor, experienceGained, canWater,
				wateringCharges);
		}

		protected GuestCropTile cropTile
		{
			get
			{
				return cropTile_ ??= GuestCropTile.Create
					(harvestIndex.Value, giantCrop.Value, tileLocation.Value);
			}
		}

		protected int scaleByCombatSkill (int baseValue, float factor)
		{
			double skill = Game1.player.team.AverageSkillLevel
				(Farmer.combatSkill, currentLocation);
			return (int) Math.Round (baseValue * (1.0 + skill * factor));
		}

		protected virtual void calculateHarvest (bool recalculate = false)
		{
			// This method is based on GiantCrop.performToolAction and
			// Crop.harvest except as noted. Since the RNG is called in a
			// different order, the results will not match the specific original
			// crop, but should match the stock algorithm on average.

			if (calculatedHarvest && !recalculate)
				return;
			calculatedHarvest = true;

			// This can only be run from the host.
			if (cropTile_ is not CropTile cropTile)
				throw new Exception ("Cannot calculate harvest without true CropTile.");

			// Start with nothing.
			primaryHarvest = null;
			secondaryHarvest = null;
			tertiaryHarvest = null;

			// Chance of a quality bump for skilful combat, even to iridium.
			int qualityBonus =
				(cropTile.rng.NextDouble () < rateCombatPerformance ())
					? 1 : 0;

			// This should never happen.
			if (cropTile.harvestIndex <= 0)
				return;

			// Calculate any programmatic coloration of the harvest.
			Color? tintColor = null;
			if (cropTile.crop.programColored.Value)
				tintColor = cropTile.crop.tintColor.Value;

			// Secondary harvests always have regular quality.
			if (containsSecondaryHarvest.Value)
			{
				int secondaryQuantity = (!cropTile.spawnSecondaries && containsPrimaryHarvest.Value)
					? cropTile.baseQuantity - 1 : 1;
				secondaryHarvest = tintColor.HasValue
					? new ColoredObject (cropTile.harvestIndex, secondaryQuantity,
						tintColor.Value)
					: new SObject (cropTile.harvestIndex, secondaryQuantity);
			}

			// Primary harvest goes with tertiary harvest and special drops.
			if (!containsPrimaryHarvest.Value)
			{
				objectsToDrop.Clear ();
				return;
			}

			// Add the primary harvest with the determined quality.
			int quality = cropTile.giantCrop
				? qualityBonus * 2 // regular or gold
				: cropTile.baseQuality + qualityBonus;
			if (quality == 3)
				quality = 4;
			if (cropTile.harvestIndex == 771 || cropTile.harvestIndex == 889)
				quality = 0;
			primaryHarvest = tintColor.HasValue
				? new ColoredObject (cropTile.harvestIndex, 1, tintColor.Value)
				{ Quality = quality }
				: new SObject (cropTile.harvestIndex, 1, quality: quality);

			// Add any tertiary harvest. The special seed drop for Sunflowers is
			// omitted here since they do not become cropbeasts by default.

			// Wheat sometimes drops Hay.
			if (cropTile.harvestIndex == 262 && cropTile.rng.NextDouble () < 0.4)
			{
				tertiaryHarvest = new SObject (178, 1);
			}
			// Otherwise, cropbeasts rarely drop more seeds. Does not apply
			// to Coffee Beans (which are themselves seeds). Give between one
			// and five seeds, but not more than twice the harvest's worth.
			else if (cropTile.harvestIndex != cropTile.seedIndex &&
				cropTile.rng.NextDouble () < 0.05)
			{
				tertiaryHarvest = new SObject (cropTile.seedIndex, 1);
				tertiaryHarvest.Stack = cropTile.rng.Next (1, Math.Min (6,
					1 + (2 * primaryHarvest.Price * cropTile.baseQuantity)
						/ Math.Max (10, tertiaryHarvest.Price)));
			}
		}

		public override void shedChunks (int number, float scale)
		{
			// Shed crop-colored generic chunks instead of the monster sprite.
			Game1.createRadialDebris (currentLocation, Game1.animationsName,
				new Rectangle (128, 3200, 64, 64), 64,
				GetBoundingBox ().Center.X, GetBoundingBox ().Center.Y,
				number, (int) getTileLocation ().Y, secondaryColor.Value * 0.8f,
				scale);
		}

		protected override void localDeathAnimation ()
		{
			// This method is called for every player.
			base.localDeathAnimation ();

			// Splay out the harvest as radial debris. Aside from the
			// impressive visual, this also prevents the Burglar Ring from
			// applying to cropbeasts.
			if (Context.IsMainPlayer && !reverted)
			{
				calculateHarvest ();

				if (primaryHarvest != null)
				{
					currentLocation.debris.Add (new Debris (primaryHarvest,
						Position, spreadPosition (32)));
				}

				if (secondaryHarvest != null)
				{
					for (int i = 0; i < secondaryHarvest.Stack; ++i)
					{
						currentLocation.debris.Add (new Debris (secondaryHarvest
							.getOne (), Position, spreadPosition (64)));
					}
				}

				if (tertiaryHarvest != null)
				{
					currentLocation.debris.Add (new Debris (tertiaryHarvest,
						Position, spreadPosition (32)));
				}
			}
		}

		protected override void sharedDeathAnimation ()
		{
			// This method is only called for the player who slayed the monster.
			base.sharedDeathAnimation ();

			if (containsPrimaryHarvest.Value)
			{
				Monitor.Log ($"Player {Game1.player.Name} slayed {Name} and gained {experienceGained.Value} XP in Farming and Combat.",
					LogLevel.Debug);

				// Distribute the normal gain of farming experience between the
				// farming and combat skills, at least one point to each. The
				// normal monster experience gain has been suppressed.
				Game1.player.gainExperience (Farmer.farmingSkill,
					experienceGained.Value);
				Game1.player.gainExperience (Farmer.combatSkill,
					experienceGained.Value);

				// Update statistics, and handle the first of the type specially.
				Game1.stats.monsterKilled (Name);
				if (Game1.stats.getMonstersKilled (Name) == 1)
				{
					// Check for attaining the achievement.
					if (!Assets.AchievementEditor.CheckAchievement
						(out int slain, out int total))
					{
						// If the type is newly slain but the achievement has
						// not been attained, show a progress message.
						string message = Helper.Translation.Get
							("achievement.progress", new
							{
								playerName = Game1.player.Name,
								beastName = displayName,
								slainCount = slain,
								totalCount = total,
							});
						Game1.playSound ("reward");
						Game1.addHUDMessage (new HUDMessage (message,
							HUDMessage.achievement_type));
					}
				}
			}
		}

		private static bool IsNearWater (GameLocation location, int x, int y)
		{
			for (int dx = -3; dx <= 3; ++dx)
			{
				for (int dy = -3; dy <= 3; ++dy)
				{
					if (Math.Abs (dx) + Math.Abs (dy) < 4 &&
							location.isOpenWater (x + dx, y + dy))
						return true;
				}
			}
			return false;
		}

		private void waterHoeDirt (HoeDirt hd)
		{
			--wateringCharges.Value;
			hd.state.Value = HoeDirt.watered;
			currentLocation.playSound ("waterSlosh");
		}

		public override void behaviorAtGameTick (GameTime time)
		{
			// Intentionally not calling base to remove default movement style.

			// Safety check for going too far out of bounds.
			if (Position.X <= -640f || Position.Y <= -640f ||
					Position.X >= (float) (currentLocation.Map.Layers[0].LayerWidth * 64 + 640) ||
					Position.Y >= (float) (currentLocation.Map.Layers[0].LayerHeight * 64 + 640))
				revert ();

			// If derived from a paddy crop, act like an automatic watering can.
			if (canWater.Value)
			{
				Point tileLoc = getTileLocationPoint ();
				if (wateringCharges.Value == 0 &&
					IsNearWater (currentLocation, tileLoc.X, tileLoc.Y))
				{
					wateringCharges.Value = MaxWateringCharges;
					currentLocation.playSound ("glug");
				}
				if (wateringCharges.Value > 0)
				{
					if (currentLocation.terrainFeatures
						.TryGetValue (getTileLocation (), out TerrainFeature tf) &&
						tf is HoeDirt hd && hd.state.Value == HoeDirt.dry)
					{
						waterHoeDirt (hd);
					}
					else if (currentLocation.getObjectAtTile (tileLoc.X, tileLoc.Y)
						is IndoorPot ip && ip.hoeDirt.Value != null &&
						ip.hoeDirt.Value.state.Value == HoeDirt.dry)
					{
						waterHoeDirt (ip.hoeDirt.Value);
					}
				}
			}
		}

		public virtual void revert ()
		{
			if (reverted)
				return;

			if (cropTile_ is not CropTile cropTile)
				throw new Exception ("Cannot revert cropbeast without true CropTile.");

			reverted = true;

			Monitor.Log ($"Reverting a {Name} to a {cropTile.logDescription}.",
				LogLevel.Debug);

			if (containsPlant.Value)
				ModEntry.Instance.chooser?.unchoose (cropTile);

			if (containsPrimaryHarvest.Value)
				cropTile.restoreFully ();
			else if (containsPlant.Value && cropTile.repeatingCrop)
				cropTile.restorePlant ();

			if (currentLocation != null)
				currentLocation.characters.Remove (this);
			Health = -500;
		}

		protected void poofTo (GameLocation location, Vector2 position)
		{
			currentLocation.characters.Remove (this);
			location.addCharacter (this);
			currentLocation = location;
			this.position.X = position.X;
			this.position.Y = position.Y;
			xVelocity = 0f;
			yVelocity = 0f;
			Utilities.MagicPoof (location, position, 2 * Sprite.SpriteWidth,
				Sprite.SpriteWidth, secondaryColor, "wand");
		}

		protected T spawnChild<T> (int spreadRadius) where T : Cropbeast
		{
			ConstructorInfo ctor = typeof (T).GetConstructor
				(new Type[] { typeof (CropTile), typeof (bool) });
			if (ctor == null)
				throw new Exception ($"Invalid cropbeast type '{typeof (T).Name}'.");

			T child = ctor.Invoke (new object[] { cropTile, false }) as T;

			Vector2 pos = spreadPosition (spreadRadius);
			child.position.X = pos.X;
			child.position.Y = pos.Y;

			ModEntry.Instance.registerMonster (child);
			DelayedAction.functionAfterDelay (() => child.spawn (), 100);

			return child;
		}

		protected Vector2 spreadPosition (int spreadRadius)
		{
			return Position + new Vector2
				(Game1.random.Next (-spreadRadius, spreadRadius + 1),
				Game1.random.Next (-spreadRadius, spreadRadius + 1));
		}
	}
}
