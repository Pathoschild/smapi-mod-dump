/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/CaptainSully/StardewMods
**
*************************************************/

using System.Xml.Serialization;

namespace BetterTappers
{
	[XmlType("Mods_CaptainSully_BetterTappers_Tapper")] // SpaceCore serialisation signature

	public class Tapper : SObject
	{
		/*********
        ** Fields
        *********/
		/// <summary>Logging tool.</summary>
		//internal static readonly Log log = ModEntry.Instance.log;
		/// <summary>The mod configuration.</summary>
		//public ModConfig Config { get; set; } = ModEntry.Config;
		/// <summary>Number of times the tapper has been harvested since placed.</summary>
		public int TimesHarvested { get; set; } = 0;


		/*********
        ** Constructors
        *********/
		/*
		/// <summary>Construct an instance.</summary>
		public Tapper() : base() { }

		/// <summary>Construct an instance with an ID.</summary>
		/// <param name="parentSheetIndex">The item ID for the new tapper.</param>
		public Tapper(int parentSheetIndex) : base()
        {
			ParentSheetIndex = parentSheetIndex; 
		}

		/// <summary>Construct an instance with an ID and location.</summary>
		/// <param name="tileLocation">The location of the new tapper.</param>
		/// <param name="parentSheetIndex">The item ID for the new tapper.</param>
		/// <param name="isRecipe">					edit			</param>
		public Tapper(Vector2 tileLocation, int parentSheetIndex, bool isRecipe = false)
			: base(tileLocation, parentSheetIndex, isRecipe) { }
		*/

		/*********
        ** Public methods
        *********/
		
		// Method overrides
		/* this is actually currently useless since tapper objects only exist outside the inventory
		 public override int maximumStackSize()
		{
			if (Stackable) { return 999; }
			return 1;
		}*/

		/* potentially useful for making new tapper types, or adding them to different trees etc.
		 public override bool canBePlacedHere(GameLocation l, Vector2 tile)
		{

		}*/

		/*
		/// <summary>Returns a new tapper object.</summary>
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
			orderData.Value = (source as SObject).orderData.Value;
			base._GetOneFrom(source);
		}

		// Mostly vanilla behaviour thats been stripped of things unrelated to tappers.
		/// <summary>Checks if player interacted with the tapper.</summary>
		/// <param name="who">The current player.</param>
		/// <param name="justCheckingForActivity">		edit		 .</param>
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

			SObject objectThatWasHeld = heldObject.Value;

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
					Config = ModEntry.Config;
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
							int q = CoreLogic.GetQualityLevel(who, CoreLogic.GetTreeAgeMonths(tree), TimesHarvested);
							log.D("New quality: " + q, Config.DebugMode);
							objectThatWasHeld.Quality = q;
						}
						objectThatWasHeld.Stack += CoreLogic.TriggerGathererPerk(who);
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
					tree.UpdateTapperProduct(this, objectThatWasHeld);
				}

				readyForHarvest.Value = false;
				showNextIndex.Value = false;

				if (check_for_reload)
				{
					AttemptAutoLoad(who);
				}
				return false;
			}
			return false;
		}


		// Custom methods
		public void CopyObjTapper(SObject parent)
		{
			name = parent.name;
			DisplayName = parent.DisplayName;
			SpecialVariable = parent.SpecialVariable;
			_GetOneFrom(parent);
		}
		*/



		/*********
        ** Private methods
        *********/

	}
}
