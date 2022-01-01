/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.TerrainFeatures;
using System;
using System.Xml.Serialization;
using SullySDVcore;
using StardewObject = StardewValley.Object;


namespace BetterTappers
{
	[XmlType("Mods_CaptainSully_BetterTappers_Tapper")] // SpaceCore serialisation signature
	public class Tapper : StardewObject
	{
		// Custom variables
		internal static readonly Log log = BetterTappers.Instance.log;
		public Config Config { get; set; } = BetterTappers.Config;

		public int TimesHarvested { get; set; } = 0;
		public long TmpUMID { get; set; } = -1;


		// Overrides
        public Tapper() : base() { }

		public Tapper(int parentSheetIndex) : base()
        {
			ParentSheetIndex = parentSheetIndex; 
		}

		public Tapper(Vector2 tileLocation, int parentSheetIndex, bool isRecipe = false)
			: base(tileLocation, parentSheetIndex, isRecipe) { }

		//this is actually currently useless since tapper objects only exist outside the inventory
		/*public override int maximumStackSize()
		{
			if (Stackable) { return 999; }
			return 1;
		}*/

		//potentially useful for making new tapper types, or adding them to different trees etc.
		/*public override bool canBePlacedHere(GameLocation l, Vector2 tile)
		{

		}*/

		public override Item getOne()
        {
			Tapper @tapper = new(tileLocation, ParentSheetIndex)
			{
				name = name,
				DisplayName = DisplayName,
				SpecialVariable = SpecialVariable,
            };
            @tapper._GetOneFrom(this);
			return @tapper;
		}
		public override void _GetOneFrom(Item source)
		{
			orderData.Value = (source as StardewObject).orderData.Value;
			base._GetOneFrom(source);
		}

		public void SetOwnerVal(long uniqueMPID)
        {
			owner.Value = uniqueMPID;
        }
		// Mostly vanilla behaviour thats been stripped of things unrelated to tappers. New things have comments
		public override bool checkForAction(Farmer who, bool justCheckingForActivity = false)
		{
			if (isTemporarilyInvisible || justCheckingForActivity)
			{
				return true;
			}
			if (!justCheckingForActivity && who is not null && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() - 1) && who.currentLocation.isObjectAtTile(who.getTileX(), who.getTileY() + 1) && who.currentLocation.isObjectAtTile(who.getTileX() + 1, who.getTileY()) && who.currentLocation.isObjectAtTile(who.getTileX() - 1, who.getTileY()) && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() - 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX(), who.getTileY() + 1).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() - 1, who.getTileY()).isPassable() && !who.currentLocation.getObjectAtTile(who.getTileX() + 1, who.getTileY()).isPassable())
			{
				performToolAction(null, who.currentLocation);
			}

			StardewObject objectThatWasHeld = heldObject.Value;

			if (readyForHarvest.Value)
			{
				if (who.isMoving())
				{
					Game1.haltAfterCheck = false;
				}
				bool check_for_reload = false;

				Tree tree = null;
				if (who.IsLocalPlayer)
				{
					Config = BetterTappers.Config;
					heldObject.Value = null;

					//Change quality value of objectThatWasHeld, then apply gatherer perk
					int ogStackSize = objectThatWasHeld.Stack;
					int ogQuality = objectThatWasHeld.Quality;
					log.D("Og Stack Size: " + ogStackSize + "    Og Quality: " + ogQuality, Config.DebugMode);
					if (who.currentLocation.terrainFeatures.ContainsKey(tileLocation) && who.currentLocation.terrainFeatures[tileLocation] is Tree)
					{
						tree = (who.currentLocation.terrainFeatures[tileLocation] as Tree);
						if (tree.treeType.Value is not 8)
						{
							int q = GetQualityLevel(who, CoreLogic.GetTreeAgeMonths(tree));
							log.D("New quality: " + q, Config.DebugMode);
							objectThatWasHeld.Quality = q;
						}
						objectThatWasHeld.Stack += TriggerGathererPerk(who);
						log.D("New Stack Size: " + objectThatWasHeld.Stack, Config.DebugMode);
					}

					if (!who.addItemToInventoryBool(objectThatWasHeld))
					{
						//if harvesting failed, reset quality of the ready item back to low and stack size back to 1
						objectThatWasHeld.Quality = ogQuality;
						objectThatWasHeld.Stack = ogStackSize;
						heldObject.Value = objectThatWasHeld;
						Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Crop.cs.588"));
						return false;
					}

					Game1.playSound("coin");
					check_for_reload = true;
				}

				//vanilla if statement moved up because quality needs to know if there's a tree. replaced with this check.
				if (tree is not null)
				{
					TmpUMID = who.UniqueMultiplayerID;
					tree.UpdateTapperProduct(this, objectThatWasHeld);
				}

				readyForHarvest.Value = false;
				showNextIndex.Value = false;

				if (check_for_reload)
				{
					AttemptAutoLoad(who);
				}
				return true;
			}
			return false;
		}


		// Custom methods
		public void CopyObjTapper(StardewObject parent)
		{
			name = parent.name;
			DisplayName = parent.DisplayName;
			SpecialVariable = parent.SpecialVariable;
			_GetOneFrom(parent);
		}

		private int TriggerGathererPerk(Farmer who)
        {
			if (!Config.DebugMode && Config.GathererAffectsTappers && who.professions.Contains(Farmer.gatherer) && Game1.random.NextDouble() < 0.2)
            {
				log.D("Gatherer perk applied", Config.DebugMode);
				return 1;
			}
			return 0;
		}
		
		public int CalculateTapperMinutes(int treeType, int parentSheetIndex)
		{
			if (Config.DisableAllModEffects || !Config.ChangeTapperTimes)
			{
				return 0;
			}
			log.D("Calculating modded tapper minutes...", Config.DebugMode);

			float days_configured = 1f;
			float time_multiplier = 1f;
			int result;

			if (parentSheetIndex == 264)
			{
				time_multiplier = Config.HeavyTapperMultiplier;
			}
			log.D("Time multiplier: " + time_multiplier, Config.DebugMode);

			switch (treeType)
			{
				case 1:
				case 2:
				case 3:
					days_configured = Config.DaysForSyrups;
					break;
				case 7:
					days_configured = Config.DaysForMushroom;
					break;
				case 8:
					days_configured = Config.DaysForSap;
					break;
			}

			days_configured *= time_multiplier;
			log.D("Days calculated: " + days_configured, Config.DebugMode);
			if (days_configured < 1)
			{
				result = (int)MathHelper.Max(1440 * days_configured, 5);
			}
			else
            {
				result = Utility.CalculateMinutesUntilMorning(Game1.timeOfDay, (int)Math.Max(1f, days_configured));
			}
			log.D("Changing minutes until ready as per configs: " + result, Config.DebugMode);
			return result;
		}

		public int GetQualityLevel(Farmer who, int age)
        {
			log.D("Quality check requested...", Config.DebugMode);
			if (Config.DisableAllModEffects || !Config.TappersUseQuality)
			{
				log.D("Quality disabled", Config.DebugMode);
				return lowQuality;
			}
			if ((!Config.ForageLevelAffectsQuality || who is null) && (!Config.TreeAgeAffectsQuality || age < 1) && 
				(!Config.TimesHarvestedAffectsQuality || TimesHarvested < 1))
            {
				log.D("Quality all types disabled", Config.DebugMode);
				return lowQuality;
            }

			int quality = DetermineQuality(who.foragingLevel.Value, age);
			if (Config.BotanistAffectsTappers)
			{
				if (who is not null && who.professions.Contains(Farmer.botanist))
				{
					log.D("Botanist perk applied.", Config.DebugMode);
					return bestQuality;
				}
				return Math.Min(quality, highQuality);
			}
			if (quality == 3)
            {
				return highQuality;
            }
			return quality;
		}

		private int DetermineQuality(int foragingLevel, int age = 0)
		{
			log.D("Determining quality...", Config.DebugMode);
			int n, FLQ, TAQ, THQ, t;
			n = FLQ = TAQ = THQ = 0;

			if (Config.ForageLevelAffectsQuality)
            {
				FLQ = GetQualityPart(foragingLevel);
				n++;
			}
			if (Config.TreeAgeAffectsQuality)
			{
				TAQ = GetQualityPart(age);
				n++;
			}
			if (Config.TimesHarvestedAffectsQuality)
			{
				THQ = GetQualityPart(TimesHarvested);
				n++;
			}

			log.D("QualitiesActive: " + n + "    FLQ: " + FLQ + "    TAQ: " + TAQ + "    THQ: " + THQ, Config.DebugMode);
			t = (FLQ + TAQ + THQ);
			log.D("Sum of qualty pieces: " + t, Config.DebugMode);
			switch (n)
            {
				case 3:
					if (t == 6)
					{
						return bestQuality;
					}
					return (int)Math.Floor(t * 0.5);

				case 2:
					return (int)Math.Floor(t * 0.75);
				case 1:
					return t;
				//these shouldn't happen, but if they do return low
				case 0:
				default:
					log.D("Problem: shouldn't asking for quality when no quality types are enabled. Defaulted to low.", true);
					return lowQuality;
			}
		}

		private int GetQualityPart(int lvl)
        {
			log.D("Getting quality piece...", Config.DebugMode);
			if (lvl > 0)
			{
				double ran = Game1.random.NextDouble();
				switch (CoreLogic.formula)
				{
					case 0:
					default:
						if (ran < (Math.Min(lvl, CoreLogic.LvlCap) / 30f))
						{
							return highQuality;
						}
						else if (ran < (Math.Min(lvl, CoreLogic.LvlCap) / 15f))
						{
							return medQuality;
						}
						break;
				}
			}
			return lowQuality;
		}
	}//END class
}//END namespace
