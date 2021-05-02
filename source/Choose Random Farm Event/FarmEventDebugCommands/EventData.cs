/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/valruno/ChooseRandomFarmEvent
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Events;
using StardewValley.Locations;
using StardewValley.TerrainFeatures;

namespace ChooseRandomFarmEvent
{
    internal class EventData
    {
        internal string Name { get; }
        internal List<KeyValuePair<Func<bool>, string>> Conditions { get; set; }
        internal FarmEvent FarmEvent { get; set; }
        internal FarmEvent PersonalFarmEvent { get; set; }

        internal string SuccessMessage { get; set; }
        internal string FailureMessage { get; set; }
        internal static List<string> EventTypes { get; } = new List<string>() { "capsule", "meteorite", "wild_animal_attack", "owl_statue", "fairy", "witch", 
            "NPC_child_request", "PC_child_request", "animal_birth" };

        internal Vector2 tile { get; }
        internal GameLocation location { get; }
        internal GiantCrop giantCrop { get; set; }
        private int id { get; set; } = -1;

        private static IModHelper Helper;
        private static IMonitor Monitor;
        public static void Initialize(IMonitor monitor, IModHelper helper)
        {
            Monitor = monitor;
            Helper = helper;
        }

        internal EventData(string n)
        {
            Name = n;
        }

        // for giant crops
        internal EventData(string n, int x, int y)
        {
            Name = n;
            tile = new Vector2(x, y);
            location = Game1.player.currentLocation;
        }

        // for giant crops with specified crop ID
        internal EventData(string n, int x, int y, int ID)
        {
            Name = n;
            tile = new Vector2(x, y);
            location = Game1.player.currentLocation;
            id = ID;
        }

        internal void SetUp()
        {
            FailureMessage = $"Could not set tonight's event to {Name}: a condition for this event has not been fulfilled, or another event is taking precedence.";
            Conditions = new List<KeyValuePair<Func<bool>, string>>();
            AddCondition(() => !Game1.weddingToday, "there's a wedding today");

            switch (Name)
            {
                case "capsule":
                    FarmEvent = new SoundInTheNightEvent(0);
                    PersonalFarmEvent = null;
                    SuccessMessage = "A strange capsule event will occur tonight.";
                    AddCondition(() => Game1.year > 1, 
                        "the game year is 1");
                    AddCondition(() => !Game1.MasterPlayer.mailReceived.Contains("Got_Capsule"), 
                        "the strange capsule event has happened before");
                    break;

                case "meteorite":
                    FarmEvent = new SoundInTheNightEvent(1);
                    PersonalFarmEvent = null;
                    SuccessMessage = "A meteorite event will occur tonight.";
                    break;

                case "wild_animal_attack":
                    FarmEvent = null;
                    PersonalFarmEvent = new SoundInTheNightEvent(2);
                    SuccessMessage = "A wild animal attack event will occur tonight.";
                    break;

                case "owl_statue":
                    FarmEvent = new SoundInTheNightEvent(3);
                    PersonalFarmEvent = null;
                    SuccessMessage = "An owl statue event will occur tonight.";
                    break;

                case "fairy":
                    FarmEvent = new FairyEvent();
                    PersonalFarmEvent = null;
                    SuccessMessage = "A crop fairy event will occur tonight.";
                    AddCondition(() => !Game1.currentSeason.Equals("winter"), "it's winter");
                    break;

                case "witch":
                    FarmEvent = new WitchEvent();
                    PersonalFarmEvent = null;
                    SuccessMessage = "A witch event will occur tonight.";
                    break;

                case "NPC_child_request":
                    FarmEvent = null;
                    PersonalFarmEvent = new QuestionEvent(1);
                    SuccessMessage = "Your NPC spouse will request a child tonight.";
                    AddCondition(() => Game1.player.isMarried(), 
                        "you are not married");
                    AddCondition(() => Game1.player.spouse != null, 
                        "you do not have a spouse");
                    AddCondition(() => Game1.getCharacterFromName(Game1.player.spouse).canGetPregnant(), 
                        "your spouse cannot have children (or is a roommate)");
                    AddCondition(() => Game1.player.currentLocation == Game1.getLocationFromName(Game1.player.homeLocation), 
                        "you are not at home");
                    break;

                case "PC_child_request":
                    FarmEvent = null;
                    PersonalFarmEvent = new QuestionEvent(3);
                    SuccessMessage = "You or your PC spouse will request a child tonight.";
                    AddCondition(() => Context.IsMultiplayer, "this is a single-player game");
                    AddCondition(() => Game1.player.isMarried(), "you are not married");
                    AddCondition(() => Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).HasValue, 
                        "you are not married to another farmer");
                    AddCondition(() => Game1.player.GetSpouseFriendship().NextBirthingDate == null, 
                        "you are already going to have a child");
                    AddCondition(() => Game1.otherFarmers.ContainsKey(Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value), 
                        "your spouse is not in the game");
                    AddCondition(() => Game1.otherFarmers[Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value].currentLocation == Game1.player.currentLocation, 
                        "you and your spouse are not in the same location");
                    AddCondition(() => Game1.otherFarmers[Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value].currentLocation == Game1.getLocationFromName(Game1.otherFarmers[Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value].homeLocation) ||
                        Game1.otherFarmers[Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value].currentLocation == Game1.getLocationFromName(Game1.player.homeLocation), 
                        "you and your spouse are not in either of your houses");
                    AddCondition(() => Helper.Reflection.GetMethod(typeof(Utility), "playersCanGetPregnantHere").Invoke<bool>(Game1.otherFarmers[Game1.player.team.GetSpouse(Game1.player.UniqueMultiplayerID).Value].currentLocation as FarmHouse), 
                        "you don't have a crib, your crib is already occupied, or you already have 2 children");
                    break;

                case "animal_birth":
                    FarmEvent = null;
                    PersonalFarmEvent = new QuestionEvent(2);
                    SuccessMessage = "An animal birth event will occur tonight.";
                    break;

                case "giant_crop":
                    if (tile == null)
                        break;

                    // players can only specify a crop ID when conditions aren't being enforced
                    if (id > -1)
                    {
                        giantCrop = new GiantCrop(id, new Vector2((int)tile.X - 1, (int)tile.Y - 1));
                        giantCrop.modData.Add(ModEntry.mod.ModManifest.UniqueID, SDate.Now().ToString());
                        break;
                    }

                    // weddings don't prevent giant crops from spawning
                    Conditions.Clear();

                    // first check whether there's actually a crop there; this will be a hard condition regardless of config
                    AddCondition(() => location.terrainFeatures.ContainsKey(tile) && location.terrainFeatures[tile] is HoeDirt, 
                        $"there is no hoe dirt at ({(int)tile.X}, {(int)tile.Y})");
                    AddCondition(() => (location.terrainFeatures[tile] as HoeDirt).crop != null, 
                        $"there is no crop at ({(int)tile.X}, {(int)tile.Y})");
                    if (!EnforceEventConditions(out string message))
                    {
                        Monitor.Log($"Could not set giant crop spawn for tonight because {message}.", LogLevel.Info);
                        break;
                    }

                    SuccessMessage = $"A giant crop will spawn at ({(int)tile.X}, {(int)tile.Y}) tonight.";

                    giantCrop = new GiantCrop((location.terrainFeatures[tile] as HoeDirt).crop.indexOfHarvest, new Vector2((int)tile.X - 1, (int)tile.Y - 1));
                    giantCrop.modData.Add(ModEntry.mod.ModManifest.UniqueID, SDate.Now().ToString());

                    if (!Helper.ModRegistry.IsLoaded("spacechase0.moregiantcrops") && !Helper.ModRegistry.IsLoaded("spacechase0.jsonassets"))
                    {
                        AddCondition(() => (location.terrainFeatures[tile] as HoeDirt).crop.indexOfHarvest == 276
                            || (location.terrainFeatures[tile] as HoeDirt).crop.indexOfHarvest == 190
                            || (location.terrainFeatures[tile] as HoeDirt).crop.indexOfHarvest == 254, 
                            "this type of crop cannot be giant");
                    }
                    AddCondition(() => location is Farm,
                        "you are not in a farm location");
                    AddCondition(() => (location.terrainFeatures[tile] as HoeDirt).state.Value == 1,
                        $"the crop at ({(int)tile.X}, {(int)tile.Y}) is not watered");
                    AddCondition(() => (int)(location.terrainFeatures[tile] as HoeDirt).crop.currentPhase == (location.terrainFeatures[tile] as HoeDirt).crop.phaseDays.Count - 1, 
                        $"the crop at ({(int)tile.X}, {(int)tile.Y}) is not fully grown");
                    AddCondition(() => CheckGiantCropSquareForCrops(), 
                        $"the crop at ({(int)tile.X}, {(int)tile.Y}) is not at the center of a 3x3 square of crops of the same type");
                    AddCondition(() => CheckGiantCropSquareForCharacters(),
                        $"there is a character in the way");
                    break;

                default:
                    break;

            }
        }

        internal bool EnforceEventConditions(out string message)
        {
            message = "";
            bool fulfillConditions = true;
            foreach (var condition in Conditions)
            {
                if (!condition.Key.Invoke())
                {
                    message = condition.Value;
                    fulfillConditions = false;
                    break;
                }
            }
            return fulfillConditions;
        }

        private bool CheckGiantCropSquareForCrops()
        {
            //Farm environment;
            //try { environment = location as Farm; }
            //catch { return false; }

            GameLocation environment = location;
                
            for (int x = (int)tile.X - 1; x <= (int)tile.X + 1; x++)
            {
                for (int y = (int)tile.Y - 1; y <= (int)tile.Y + 1; y++)
                {
                    Vector2 v = new Vector2(x, y);
                    if (!environment.isTileHoeDirt(v)
                        || (environment.terrainFeatures[v] as HoeDirt).crop == null 
                        || (environment.terrainFeatures[v] as HoeDirt).crop.indexOfHarvest != (environment.terrainFeatures[tile] as HoeDirt).crop.indexOfHarvest)
                    {
                        return false; ;
                    }
                }
            }
            return true;
        }

        private bool CheckGiantCropSquareForCharacters()
        {
            for (int x = (int)tile.X - 1; x <= (int)tile.X + 1; x++)
            {
                for (int y = (int)tile.Y - 1; y <= (int)tile.Y + 1; y++)
                {
                    Vector2 v = new Vector2(x, y);

                    foreach (Farmer farmer in location.farmers)
                    {
                        if (farmer.getTileLocation() == v)
                            return false;
                    }
                    foreach (Character character in location.characters)
                    {
                        if (character.getTileLocation() == v)
                            return false;
                    }
                }
            }
            return true;
        }

        private void AddCondition(Func<bool> condition, string failureReason)
        {
            Conditions.Add(new KeyValuePair<Func<bool>, string>(condition, failureReason));
        }
    }

}