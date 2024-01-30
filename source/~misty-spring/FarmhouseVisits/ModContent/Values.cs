/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using StardewValley.Extensions;
using StardewValley.Locations;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using lv = StardewModdingAPI.LogLevel;
using FarmVisitors.Models;
using StardewValley.GameData.Objects;

namespace FarmVisitors.Visit;

public enum ItemType
{
    Animal,
    Crop,
    Furniture
}

public enum DialogueType
{
    //about the visit
    Introduce,
    WalkIn,
    Furniture,
    Greet,
    Retiring,
    Thanking,
    Rejected,

    //about the farm
    Animal,
    Crop,
    NoneYet,
    Winter
}
internal static class Values
{
    internal static bool IsVisitor(string c)
    {
        return c.Equals(ModEntry.Visitor.Name);
    }
    
    /// <summary>
    /// Checks if a NPC is free to visit player.
    /// </summary>
    /// <param name="who">NPC to check</param>
    /// <param name="isRandom">If it's a randomly-chosen visit.</param>
    /// <returns>Whether the NPC is fully free (ie not invisible, not in island, etc)</returns>
    internal static bool IsFree(NPC who, bool isRandom = true)
    {
        //if character is robin and there's a construction ongoing
        if (who.Name == "Robin" && (Game1.getFarm().isThereABuildingUnderConstruction() || who.currentLocation.Equals(Game1.getFarm())))
        {
            ModEntry.Log(
                $"{who.displayName} is building in the Farm. Visit will be cancelled.", 
                ModEntry.Level);
            return false;
        }

        var isAnimating = who.doingEndOfRouteAnimation.Value;
        var isInvisible = who.IsInvisible;
        var isHospitalDay = Utility.IsHospitalVisitDay(who.Name);
        var visitedToday = ModEntry.TodaysVisitors.Contains(who.Name);
        var visitingIsland = Game1.IsVisitingIslandToday(who.Name);
        var isSleeping = who.isSleeping.Value;

        var defaultReqs = !isHospitalDay && !visitingIsland && !isSleeping && !isAnimating && !isInvisible;

        if (visitedToday && !isRandom)
        {
            ModEntry.Log($"{who.displayName} has already visited the Farm today!");
        }
        if (isHospitalDay)
        {
            ModEntry.Log($"{who.displayName} has a hospital visit scheduled today. They won't visit the farmer.");
        }
        if (visitingIsland)
        {
            ModEntry.Log($"{who.displayName} is visiting the island today!");
        }
        if (isSleeping)
        {
            ModEntry.Log($"{who.displayName} is sleeping right now.");
        }

        if (isRandom)
        {
            return !visitedToday && defaultReqs;
        }

        return defaultReqs;
    }
    
    /// <summary>
    /// Retrieves a dialogue of the specified type.
    /// </summary>
    /// <param name="c">The NPC whose info to use.</param>
    /// <param name="which">The type of dialogue.</param>
    /// <returns></returns>
    internal static string GetDialogueType(NPC c, DialogueType which)
    {
        //allowed values: Introduce, Rejected, Greet, Retiring, WalkIn, Furniture
        
        var r = Game1.random.Next(1, 4);
        var type = Game1.random.Next(0, 3);

        if (which == DialogueType.Retiring)
            return RetiringDialogue(c);
        
        if (type == 0)
        {
            if (c.SocialAnxiety.Equals(0))
            {
                return ModEntry.TL.Get($"NPC{which}.Outgoing{r}");
            }

            return ModEntry.TL.Get($"NPC{which}.Shy{r}");
        }

        if (c.Manners.Equals(1)) //polite
        {
            return ModEntry.TL.Get($"NPC{which}.Polite{r}");
        }

        if (c.Manners.Equals(2)) //rude
        {
            return ModEntry.TL.Get($"NPC{which}.Rude{r}");
        }

        //neutral
        return ModEntry.TL.Get($"NPC{which}.Neutral{r}");
    }

    private static string RetiringDialogue(NPC c)
    {
        //30% of unique dialogue when enabled
        if (Game1.random.Next(0, 3) == 2 && ModEntry.Config.UniqueDialogue)
            return Game1.random.ChooseFrom(ModEntry.RetiringDialogue[c.Name]);

        if (Game1.random.Next(0, 3) == 0)
        {
            if (c.SocialAnxiety.Equals(0))
            {
                return Game1.random.ChooseFrom(ModEntry.RetiringDialogue["Outgoing"]);
            }

            return Game1.random.ChooseFrom(ModEntry.RetiringDialogue["Shy"]);
        }

        if (c.Manners.Equals(1)) //polite
        {
            Game1.random.ChooseFrom(ModEntry.RetiringDialogue["Polite"]);
        }

        if (c.Manners.Equals(2)) //rude
        {
            Game1.random.ChooseFrom(ModEntry.RetiringDialogue["Rude"]);
        }

        //neutral
        return Game1.random.ChooseFrom(ModEntry.RetiringDialogue["Neutral"]);
    }

    /// <summary>
    /// Returns dialogue related to gifting.
    /// </summary>
    /// <param name="c">Character whose data is used.</param>
    /// <returns></returns>
    internal static string GetGiftDialogue(NPC c)
    {
        var r = Game1.random.Next(1, 6);

        if(Game1.random.Next(0,4).Equals(1))
        {
            if (c.SocialAnxiety.Equals(0))
            {
                return ModEntry.TL.Get($"NPCGift.Normal{r}");
            }

            return ModEntry.TL.Get($"NPCGift.Kind{r}");
        }

        if (c.Manners.Equals(1)) //polite
        {
            return ModEntry.TL.Get($"NPCGift.Kind{r}");
        }

        if (c.Manners.Equals(2)) //rude
        {
            return ModEntry.TL.Get($"NPCGift.Lax{r}");
        }

        return ModEntry.TL.Get($"NPCGift.Normal{r}");
    }
    
    /// <summary>
    /// Returns translation string, for when NPC leaves and player isn't home.
    /// </summary>
    /// <param name="isCellar">Whether the player is in the Cellar.</param>
    /// <returns></returns>
    internal static string GetNpcGone(bool isCellar)
    {
        if(isCellar)
        {
            return ModEntry.TL.Get("NPCGone_Cellar");
        }

        return ModEntry.TL.Get("NPCGoneWhileOutside");
    }
    
    /// <summary>
    /// Return gift IDs depending on season.
    /// </summary>
    /// <returns></returns>
    internal static string GetSeasonalGifts()
    {
        const string defaultgifts = "419 246 423 216 247 688 176 180 436 174 182 184 424 186 438"; //Vinegar, Wheat Flour, Rice, Bread, Oil, Warp Totem: Farm, small Egg (2 types),Large Egg (2 types), Goat Milk, Milk, Cheese, Large Milk, L. Goat Milk
        if (Game1.IsSpring)
        {
            return $"{defaultgifts} 400 252 222 190 610 715"; //Strawberry, Rhubarb, Rhubarb Pie, Cauliflower, Fruit Salad, Lobster
        }

        if (Game1.IsSummer)
        {
            return $"{defaultgifts} 636 256 258 270 690 260"; //Peach, Tomato, Blueberry, Corn, Warp Totem: Beach, Hot Pepper
        }

        if (Game1.IsFall)
        {
            return $"{defaultgifts} 276 284 282 395 689 613"; //Pumpkin, Beet, Cranberries, Coffee, Warp Totem: Mountains, Apple
        }

        if (Game1.IsWinter)
        {
            return $"{defaultgifts} 414 144 147 178 787 440"; //Crystal Fruit, Pike, Herring, Hay, Battery Pack, Wool
        }

        return defaultgifts;
    }
    
    /// <summary>
    /// Updates furniture list.
    /// </summary>
    /// <param name="farmHouse">The farmhouse whose data to use.</param>
    /// <returns></returns>
    internal static List<string> UpdateFurniture(FarmHouse farmHouse)
    {
        List<string> templist = new();
        foreach (var f in farmHouse.furniture)
        {
            //templist.Add(f.DisplayName);
            templist.Add($"\"{f.DisplayName.ToLower()}\"");
        }
        return templist;
    }
    
    /// <summary>
    /// Gets the index and name of all crop types in a given location.
    /// </summary>
    /// <returns>A dictionary with cropId and display name</returns>
    internal static Dictionary<string,string> GetCrops(GameLocation where = null)
    {
        try
        {
            var stringsObjects = ModEntry.Help.GameContent.Load<Dictionary<string, string>>("Strings/Objects");
            var objectData = ModEntry.Help.GameContent.Load<Dictionary<string, ObjectData>>("Data/Objects");

            Dictionary<string, string> tempdict = new();
            GameLocation farm;

            if (where != null)
                farm = where;
            else
                farm = Game1.getFarm();
            
            foreach (var value in farm.terrainFeatures.Values)
            {
                var isDirt = value is HoeDirt;
                if (!isDirt) //if not dirt
                    continue;

                var crop = (value as HoeDirt).crop;

                //if no crop, wilted, or error
                if (crop == null || crop.dead.Value || crop.IsErrorCrop()) 
                    continue;

                var data = crop.GetData();

                if (data == null)
                    continue;

                var Id = data.HarvestItemId;

                if(ModEntry.Config.Debug)
                {
                    ModEntry.Log($"crop ID: {Id}", lv.Info);
                }

                if (tempdict.ContainsKey(Id))
                    continue;

                if (!objectData.TryGetValue(Id, out var harvestData))
                    continue;
                
                var name = harvestData.DisplayName;
                if(name.StartsWith("[Localized"))
                {
                    var source = name.Remove(0, 15).Replace("]", "");
                    if(source.StartsWith("Strings:Objects"))
                    {
                        var trimmed = source.Remove(0, 17);
                        name = stringsObjects[trimmed];
                    }
                    else
                        name = Game1.content.LoadString(source);
                }
                if (ModEntry.Config.Debug)
                {
                    ModEntry.Log($"Crop's harvest Id = {Id}; display name= {name}");
                }

                tempdict.Add(Id, name);
            }

            return tempdict;
        }
        catch(Exception ex)
        {
            ModEntry.Log($"Error while getting crops: {ex}", lv.Error);
            return null;
        }
    }

    /// <summary>
    /// Gets the index and name of all crop types in a location.
    /// </summary>
    /// <returns>A list with crop's display name</returns>
    internal static List<string> GetCropsNameOnly(GameLocation where)
    {
        //yes, i could use Linq, but i'd rather do it myself
        try
        {
            List<string> templist = new();

            var crops = GetCrops(where);
            
            foreach (var t in crops)
                templist.Add(t.Value);

            return templist;
        }
        catch (Exception ex)
        {
            ModEntry.Log($"Error while getting crops' names: {ex}", lv.Error);
            return null;
        }
    }

    /// <summary>
    /// Returns name of all animals.
    /// </summary>
    /// <returns></returns>
    internal static List<string> GetAnimals()
    {
        var all = Game1.getFarm().getAllFarmAnimals();

        List<string> templist = new();
        foreach (var animal in all)
        {
            templist.Add($"\"{animal.displayName}\"");
        }
        return templist;
    }

    /// <summary>
    /// Checks if it's close to sleep time for the NPC.
    /// </summary>
    /// <param name="who"></param>
    /// <returns></returns>
    public static bool IsCloseToSleepTime(VisitData context)
    {
        var time = Game1.timeOfDay;
        var schedule = context.Scheduledata;
        if (schedule == null)
            return false;

        foreach (var data in schedule)
        {
            if (ModEntry.Config.Debug)
                ModEntry.Log($"Checking schedule point: {data.Key} at {time}", lv.Debug);

            //if already passed, OR if distance is more than 30
            if (data.Key <= time) continue;
            if ((data.Key - time) != 70) continue;
            
            if(ModEntry.Config.Debug)
                ModEntry.Log("Endofroutebehavior: " + data.Value.endOfRouteBehavior);

            var behavior = data.Value.endOfRouteBehavior;
            if (behavior == null)
                continue;

            if (behavior.Contains("sleep"))
            {
                ModEntry.Log("NPC should be going to sleep now.");
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Get a bed spot for the visitor. Will try finding a single bed, and fallback to double one.
    /// </summary>
    /// <returns>A bed spot, or Point.Zero</returns>
    public static Point GetBedSpot()
    {
        //if no bed is found, the code returns this
        var none = new Point(-1000, -1000);

        //BedFurniture.BedType bedType = BedFurniture.BedType.Any
        var home = Utility.getHomeOfFarmer(Game1.player);
        var bed = home.getBedSpot(BedFurniture.BedType.Single);
        if (bed == none)
        {
            bed = home.getBedSpot(BedFurniture.BedType.Double);
            bed.X++;
        }

        if (ModEntry.Config.Debug)
            ModEntry.Log("New bed spot:" + bed, lv.Info);

        if (bed == none)
            return Point.Zero;
        else
            return bed;
    }

    /// <summary>
    /// Checks if the visitor is on-screen.
    /// </summary>
    /// <returns></returns>
    internal static bool NPCinScreen()
    {
        var farm = Game1.getLocationFromName("Farm");

        var x = ((int)(ModEntry.Visitor.Position.X / 64));
        var y = ((int)(ModEntry.Visitor.Position.Y / 64));

        if (ModEntry.Config.Debug)
        {
            ModEntry.Log($"farm name = {farm.Name}, visitor position = ({x}, {y})", lv.Info);
        }

        //return Utility.isOnScreen(who.Position.ToPoint(), 0, farm);
        return Utility.isOnScreen(new Point(x,y), 0, farm);
    }
}