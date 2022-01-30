/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using Newtonsoft.Json.Linq;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Monsters;
using System;
using System.Collections.Generic;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Removes any invalid monster types and/or settings from a list.</summary>
            /// <param name="monsterTypes">A list of monster type data.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>A new list of MonsterTypes with any invalid monster types and/or settings removed.</returns>
            public static List<MonsterType> ValidateMonsterTypes(List<MonsterType> monsterTypes, string areaID = "")
            {
                if (monsterTypes == null || monsterTypes.Count <= 0) //if the provided list is null or empty
                {
                    return new List<MonsterType>(); //return an empty list
                }

                List<MonsterType> validTypes = Clone(monsterTypes); //create a new copy of the list, to be validated and returned

                for (int x = validTypes.Count - 1; x >= 0; x--) //for each monster type in the new list (iterating backward to allow safe removal)
                {
                    //validate monster names
                    bool validName = false;

                    //NOTE: these switch cases are copied from SpawnMonster.cs; update them manually when new monsters are added
                    switch (validTypes[x].MonsterName.ToLower()) //avoid any casing issues by making this lower-case
                    {
                        case "bat":
                        case "frostbat":
                        case "frost bat":
                        case "lavabat":
                        case "lava bat":
                        case "iridiumbat":
                        case "iridium bat":
                        case "doll":
                        case "curseddoll":
                        case "cursed doll":
                        case "skull":
                        case "hauntedskull":
                        case "haunted skull":
                        case "magmasprite":
                        case "magma sprite":
                        case "magmasparker":
                        case "magma sparker":
                        case "bigslime":
                        case "big slime":
                        case "biggreenslime":
                        case "big green slime":
                        case "bigblueslime":
                        case "big blue slime":
                        case "bigfrostjelly":
                        case "big frost jelly":
                        case "bigredslime":
                        case "big red slime":
                        case "bigredsludge":
                        case "big red sludge":
                        case "bigpurpleslime":
                        case "big purple slime":
                        case "bigpurplesludge":
                        case "big purple sludge":
                        case "bluesquid":
                        case "blue squid":
                        case "bug":
                        case "armoredbug":
                        case "armored bug":
                        case "dino":
                        case "dinomonster":
                        case "dino monster":
                        case "pepper":
                        case "pepperrex":
                        case "pepper rex":
                        case "rex":
                        case "duggy":
                        case "magmaduggy":
                        case "magma duggy":
                        case "dust":
                        case "sprite":
                        case "dustsprite":
                        case "dust sprite":
                        case "spirit":
                        case "dustspirit":
                        case "dust spirit":
                        case "dwarvishsentry":
                        case "dwarvish sentry":
                        case "dwarvish":
                        case "sentry":
                        case "ghost":
                        case "carbonghost":
                        case "carbon ghost":
                        case "putridghost":
                        case "putrid ghost":
                        case "slime":
                        case "greenslime":
                        case "green slime":
                        case "blueslime":
                        case "blue slime":
                        case "frostjelly":
                        case "frost jelly":
                        case "redslime":
                        case "red slime":
                        case "redsludge":
                        case "red sludge":
                        case "purpleslime":
                        case "purple slime":
                        case "purplesludge":
                        case "purple sludge":
                        case "tigerslime":
                        case "tiger slime":
                        case "prismaticslime":
                        case "prismatic slime":
                        case "grub":
                        case "cavegrub":
                        case "cave grub":
                        case "fly":
                        case "cavefly":
                        case "cave fly":
                        case "mutantgrub":
                        case "mutant grub":
                        case "mutantfly":
                        case "mutant fly":
                        case "metalhead":
                        case "metal head":
                        case "hothead":
                        case "hot head":
                        case "lavalurk":
                        case "lava lurk":
                        case "leaper":
                        case "mummy":
                        case "rockcrab":
                        case "rock crab":
                        case "lavacrab":
                        case "lava crab":
                        case "iridiumcrab":
                        case "iridium crab":
                        case "falsemagmacap":
                        case "false magma cap":
                        case "stickbug":
                        case "stick bug":
                        case "magmacap":
                        case "magma cap":
                        case "rockgolem":
                        case "rock golem":
                        case "stonegolem":
                        case "stone golem":
                        case "wildernessgolem":
                        case "wilderness golem":
                        case "serpent":
                        case "royalserpent":
                        case "royal serpent":
                        case "brute":
                        case "shadowbrute":
                        case "shadow brute":
                        case "shaman":
                        case "shadowshaman":
                        case "shadow shaman":
                        case "sniper":
                        case "shadowsniper":
                        case "shadow sniper":
                        case "skeleton":
                        case "skeletonmage":
                        case "skeleton mage":
                        case "spiker":
                        case "squidkid":
                        case "squid kid":
                            validName = true; //the name is valid
                            break;
                        default: //if the name doesn't match any directly known monster types
                            Type externalType = GetTypeFromName(validTypes[x].MonsterName, typeof(Monster)); //find a monster subclass with a matching name
                            if (externalType != null) //if a matching type was found
                            {
                                if (externalType.GetConstructor(new[] { typeof(Vector2) }) != null) //if this type has a constructor that takes a Vector2
                                {
                                    validName = true; //the name is valid
                                }

                                /* NOTE: Accepting monsters' default constructors would be dangerous and is not recommended.
                                 * Many monsters' defaults create them without filling game-critical fields, and this code can't reasonably account for them.
                                 * The game will often freeze or crash if they are used here.
                                 */
                            }
                            break;
                    }

                    if (!validName) //if the name is invalid
                    {
                        Monitor.Log($"A listed monster (\"{validTypes[x].MonsterName}\") doesn't match any known monster types. Make sure that name isn't misspelled in your config file.", LogLevel.Info);
                        Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                        validTypes.RemoveAt(x); //remove this type from the valid list
                        continue; //skip to the next monster type
                    }

                    if (validTypes[x].Settings == null) //if no settings were provided at all
                    {
                        validTypes[x].Settings = new Dictionary<string, object>(); //create a blank list of settings
                    }

                    //validate HP
                    if (validTypes[x].Settings.ContainsKey("HP"))
                    {
                        if (validTypes[x].Settings["HP"] is long) //if this is a readable integer
                        {
                            int HP = Convert.ToInt32(validTypes[x].Settings["HP"]);
                            if (HP < 1) //if the setting is too low
                            {
                                Monitor.Log($"The \"HP\" setting for monster type \"{validTypes[x].MonsterName}\" is {HP}. Setting it to 1.", LogLevel.Trace);
                                validTypes[x].Settings["HP"] = (long)1; //set the validated setting to 1
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"HP\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("HP"); //remove the setting
                        }
                    }

                    //validate damage
                    if (validTypes[x].Settings.ContainsKey("Damage"))
                    {
                        if (validTypes[x].Settings["Damage"] is long) //if this is a readable integer
                        {
                            int damage = Convert.ToInt32(validTypes[x].Settings["Damage"]);
                            if (damage < 0) //if the setting is too low
                            {
                                Monitor.Log($"The \"Damage\" setting for monster type \"{validTypes[x].MonsterName}\" is {damage}. Setting it to 0.", LogLevel.Trace);
                                validTypes[x].Settings["Damage"] = (long)0; //set the validated setting to 0
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"Damage\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Damage"); //remove the setting
                        }
                    }

                    //validate defense
                    if (validTypes[x].Settings.ContainsKey("Defense"))
                    {
                        if (validTypes[x].Settings["Defense"] is long) //if this is a readable integer
                        {
                            int defense = Convert.ToInt32(validTypes[x].Settings["Defense"]);
                            if (defense < 0) //if the setting is too low
                            {
                                Monitor.Log($"The \"Defense\" setting for monster type \"{validTypes[x].MonsterName}\" is {defense}. Setting it to 0.", LogLevel.Trace);
                                validTypes[x].Settings["Defense"] = (long)0; //set the validated setting to 1
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"Defense\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Defense"); //remove the setting
                        }
                    }

                    //validate dodge chance
                    if (validTypes[x].Settings.ContainsKey("DodgeChance"))
                    {
                        if (validTypes[x].Settings["DodgeChance"] is long) //if this is a readable integer
                        {
                            int dodge = Convert.ToInt32(validTypes[x].Settings["DodgeChance"]);
                            if (dodge < 1) //if the setting is too low
                            {
                                Monitor.Log($"The \"DodgeChance\" setting for monster type \"{validTypes[x].MonsterName}\" is {dodge}. Setting it to 1.", LogLevel.Trace);
                                validTypes[x].Settings["DodgeChance"] = (long)1; //set the validated setting to 1
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"DodgeChance\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("DodgeChance"); //remove the setting
                        }
                    }

                    //validate experience points
                    if (validTypes[x].Settings.ContainsKey("EXP"))
                    {
                        if (validTypes[x].Settings["EXP"] is long) //if this is a readable integer
                        {
                            int exp = Convert.ToInt32(validTypes[x].Settings["EXP"]);
                            if (exp < 0) //if the setting is too low
                            {
                                Monitor.Log($"The \"EXP\" setting for monster type \"{validTypes[x].MonsterName}\" is {exp}. Setting it to 0.", LogLevel.Trace);
                                validTypes[x].Settings["EXP"] = (long)0; //set the validated setting to 0
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"EXP\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("EXP"); //remove the setting
                        }
                    }

                    //validate the related skill setting
                    if (validTypes[x].Settings.ContainsKey("RelatedSkill"))
                    {
                        if (validTypes[x].Settings["RelatedSkill"] is string) //if this is a readable string
                        {
                            string relatedSkill = ((string)validTypes[x].Settings["RelatedSkill"]).Trim().ToLower(); //parse the provided skill, trim whitespace, and lower case
                            bool isSkill = false;

                            foreach (string skill in Enum.GetNames(typeof(Skills))) //for each known in-game skill
                            {
                                if (relatedSkill.Trim().ToLower() == skill.Trim().ToLower()) //if the provided skill name matches this known skill
                                {
                                    isSkill = true; //the provided skill is valid
                                }
                            }

                            if (!isSkill) //if this isn't a known skill
                            {
                                Monitor.Log($"The \"RelatedSkill\" setting for monster type \"{validTypes[x].MonsterName}\" doesn't seem to be a known skill. Please make sure it's spelled correctly.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                                validTypes[x].Settings.Remove("RelatedSkill"); //remove the setting
                            }
                        }
                        else //if this isn't a readable string
                        {
                            Monitor.Log($"The \"RelatedSkill\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's a valid string (text inside quotation marks).", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("RelatedSkill"); //remove the setting
                        }
                    }

                    //validate minimum skill requirement
                    if (validTypes[x].Settings.ContainsKey("MinimumSkillLevel"))
                    {
                        if (validTypes[x].Settings["MinimumSkillLevel"] is long) //if this is a readable integer
                        {
                            if (validTypes[x].Settings.ContainsKey("RelatedSkill")) //if a RelatedSkill has been provided
                            {
                                int required = Convert.ToInt32(validTypes[x].Settings["MinimumSkillLevel"]);
                                int highestSkillLevel = 0; //highest skill level among all existing farmers (not just the host)
                                Enum.TryParse((string)validTypes[x].Settings["RelatedSkill"], true, out Skills skill); //parse the RelatedSkill setting into an enum (note: the setting should be validated earlier in this method)

                                foreach (Farmer farmer in Game1.getAllFarmers()) //for each farmer
                                {

                                    highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the farmer's skill level if it's higher than before
                                }

                                if (required > highestSkillLevel) //if the skill requirement is higher than the farmers' highest skill
                                {
                                    Monitor.VerboseLog($"Skipping monster type \"{validTypes[x].MonsterName}\" in spawn area \"{areaID}\" due to minimum skill level.");
                                    validTypes.RemoveAt(x); //remove this type from the valid list
                                    continue; //skip to the next monster type
                                }
                            }
                            else //if a RelatedSkill was not provided
                            {
                                Monitor.Log($"Monster type \"{validTypes[x].MonsterName}\" has a valid setting for \"MinimumSkillLevel\" but not \"RelatedSkill\". The requirement will be skipped.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                                validTypes[x].Settings.Remove("MinimumSkillLevel"); //remove the setting
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"MinimumSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("MinimumSkillLevel"); //remove the setting
                        }
                    }

                    //validate maximum skill requirement
                    if (validTypes[x].Settings.ContainsKey("MaximumSkillLevel"))
                    {
                        if (validTypes[x].Settings["MaximumSkillLevel"] is long) //if this is a readable integer
                        {
                            if (validTypes[x].Settings.ContainsKey("RelatedSkill")) //if a RelatedSkill has been provided
                            {
                                int required = Convert.ToInt32(validTypes[x].Settings["MaximumSkillLevel"]);
                                int highestSkillLevel = 0; //highest skill level among all existing farmers (not just the host)
                                Enum.TryParse((string)validTypes[x].Settings["RelatedSkill"], true, out Skills skill); //parse the RelatedSkill setting into an enum (note: the setting should be validated earlier in this method)

                                foreach (Farmer farmer in Game1.getAllFarmers()) //for each farmer
                                {

                                    highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the farmer's skill level if it's higher than before
                                }

                                if (required < highestSkillLevel) //if the skill requirement is lower than the farmers' highest skill
                                {
                                    Monitor.VerboseLog($"Skipping monster type \"{validTypes[x].MonsterName}\" in spawn area \"{areaID}\" due to maximum skill level.");
                                    validTypes.RemoveAt(x); //remove this type from the valid list
                                    continue; //skip to the next monster type
                                }
                            }
                            else //if a RelatedSkill was not provided
                            {
                                Monitor.Log($"Monster type \"{validTypes[x].MonsterName}\" has a valid setting for \"MaximumSkillLevel\" but not \"RelatedSkill\". The requirement will be skipped.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                                validTypes[x].Settings.Remove("MaximumSkillLevel"); //remove the setting
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"MaximumSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("MaximumSkillLevel"); //remove the setting
                        }
                    }

                    //validate HP multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraHPPerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraHPPerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraHPPerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraHPPerSkillLevel"); //remove the setting
                        }

                    }

                    //validate damage multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraDamagePerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraDamagePerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraDamagePerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraDamagePerSkillLevel"); //remove the setting
                        }
                    }

                    //validate defense multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraDefensePerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraDefensePerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraDefensePerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraDefensePerSkillLevel"); //remove the setting
                        }
                    }

                    //validate dodge chance multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraDodgeChancePerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraDodgeChancePerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraDodgeChancePerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraDodgeChancePerSkillLevel"); //remove the setting
                        }
                    }

                    //validate experience multiplier
                    if (validTypes[x].Settings.ContainsKey("PercentExtraEXPPerSkillLevel"))
                    {
                        if (!(validTypes[x].Settings["PercentExtraEXPPerSkillLevel"] is long)) //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"PercentExtraEXPPerSkillLevel\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PercentExtraEXPPerSkillLevel"); //remove the setting
                        }
                    }

                    //validate loot and parse the provided objects into IDs
                    if (validTypes[x].Settings.ContainsKey("Loot"))
                    {
                        List<object> rawList = null;

                        try
                        {
                            rawList = ((JArray)validTypes[x].Settings["Loot"]).ToObject<List<object>>(); //cast this list to catch formatting/coding errors
                        }
                        catch (Exception)
                        {
                            Monitor.Log($"The \"Loot\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's a correctly formatted list.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Loot"); //remove the setting
                        }

                        if (validTypes[x].Settings.ContainsKey("Loot")) //if no exception happened
                        {
                            if (rawList == null) //if a null list was provided
                            {
                                validTypes[x].Settings["Loot"] = new List<SavedObject>(); //use an empty list
                            }
                            else //if an actual list was provided
                            {
                                List<SavedObject> lootList = ParseSavedObjectsFromItemList(rawList, areaID); //parse the object list into a SavedObject list

                                foreach (SavedObject loot in lootList)
                                {
                                    //convert any "object" categories to "item" for the loot list
                                    string category = loot.ConfigItem?.Category?.ToLower();
                                    if (category == "object" || category == "objects")
                                    {
                                        loot.ConfigItem.Category = "item";
                                    }
                                }

                                validTypes[x].Settings["Loot"] = lootList;
                            }
                        }
                    }

                    //validate persistent HP
                    if (validTypes[x].Settings.ContainsKey("PersistentHP"))
                    {
                        if (!(validTypes[x].Settings["PersistentHP"] is bool)) //if this is NOT a readable boolean
                        {
                            Monitor.Log($"The \"PersistentHP\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's true or false (without quotation marks).", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("PersistentHP"); //remove the setting
                        }
                    }

                    //validate current HP
                    if (validTypes[x].Settings.ContainsKey("CurrentHP"))
                    {
                        if (validTypes[x].Settings["CurrentHP"] is long) //if this is a readable integer
                        {
                            int currentHP = Convert.ToInt32(validTypes[x].Settings["CurrentHP"]);
                            if (currentHP < 1) //if the current HP setting is too low
                            {
                                Monitor.Log($"The \"CurrentHP\" setting for monster type \"{validTypes[x].MonsterName}\" is {currentHP}. Setting it to 1.", LogLevel.Trace);
                                monsterTypes[x].Settings["CurrentHP"] = (long)1; //set the original provided setting to 1
                                validTypes[x].Settings["CurrentHP"] = (long)1; //set the validated setting to 1
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"CurrentHP\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("CurrentHP"); //remove the setting
                        }
                    }

                    //validate seeing players at spawn
                    if (validTypes[x].Settings.ContainsKey("SeesPlayersAtSpawn"))
                    {
                        if (!(validTypes[x].Settings["SeesPlayersAtSpawn"] is bool)) //if this is NOT a readable boolean
                        {
                            Monitor.Log($"The \"SeesPlayersAtSpawn\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's true or false (without quotation marks).", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("SeesPlayersAtSpawn"); //remove the setting
                        }
                    }

                    //validate color
                    if (validTypes[x].Settings.ContainsKey("Color")) //if color was provided
                    {
                        //try a trimmed copy of the color application code
                        try
                        {
                            string[] colorText = ((string)validTypes[x].Settings["Color"]).Trim().Split(' '); //split the color string into strings for each number
                            List<int> colorNumbers = new List<int>();
                            foreach (string text in colorText) //for each string
                            {
                                int num = Convert.ToInt32(text); //convert it to a number
                                if (num < 0) { num = 0; } //minimum 0
                                else if (num > 255) { num = 255; } //maximum 255
                                colorNumbers.Add(num); //add it to the list
                            }

                            //convert strings into RGBA values
                            int r = Convert.ToInt32(colorNumbers[0]);
                            int g = Convert.ToInt32(colorNumbers[1]);
                            int b = Convert.ToInt32(colorNumbers[2]);
                            int a;
                            if (colorNumbers.Count > 3) //if the setting included an "A" value
                            {
                                a = Convert.ToInt32(colorNumbers[3]);
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log($"The \"Color\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it follows the correct format, e.g. \"255 255 255\" or \"255 255 255 255\".", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Color"); //remove the setting
                        }
                    }
                    else if (validTypes[x].Settings.ContainsKey("MinColor") && validTypes[x].Settings.ContainsKey("MaxColor")) //if color wasn't provided, but mincolor & maxcolor were
                    {
                        //try a trimmed copy of the min/max color application code
                        try
                        {
                            string[] minColorText = ((string)validTypes[x].Settings["MinColor"]).Trim().Split(' '); //split the setting string into strings for each number
                            List<int> minColorNumbers = new List<int>();
                            foreach (string text in minColorText) //for each string
                            {
                                int num = Convert.ToInt32(text); //convert it to a number
                                if (num < 0) { num = 0; } //minimum 0
                                else if (num > 255) { num = 255; } //maximum 255
                                minColorNumbers.Add(num); //add it to the list
                            }

                            string[] maxColorText = ((string)validTypes[x].Settings["MaxColor"]).Trim().Split(' '); //split the setting string into strings for each number
                            List<int> maxColorNumbers = new List<int>();
                            foreach (string text in maxColorText) //for each string
                            {
                                int num = Convert.ToInt32(text); //convert it to a number
                                if (num < 0) { num = 0; } //minimum 0
                                else if (num > 255) { num = 255; } //maximum 255
                                maxColorNumbers.Add(num); //convert to number
                            }
                        }
                        catch (Exception)
                        {
                            Monitor.Log($"The \"MinColor\" and/or \"MaxColor\" settings for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure they follow the correct format, e.g. \"255 255 255\".", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            //remove the settings
                            validTypes[x].Settings.Remove("MinColor");
                            validTypes[x].Settings.Remove("MaxColor");
                        }
                    }

                    //validate sprite
                    if (validTypes[x].Settings.ContainsKey("Sprite"))
                    {
                        if (validTypes[x].Settings["Sprite"] is string spriteText) //if this is a readable string
                        {
                            try
                            {
                                AnimatedSprite sprite = new AnimatedSprite(spriteText);
                            }
                            catch (Exception)
                            {
                                Monitor.Log($"The \"Sprite\" setting for monster type \"{validTypes[x].MonsterName}\" failed to load. Please make sure the setting is spelled correctly.", LogLevel.Info);
                                Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                                validTypes[x].Settings.Remove("Sprite"); //remove the setting
                            }
                        }
                        else //if this is NOT a readable string
                        {
                            Monitor.Log($"The \"Sprite\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's a valid string (text inside quotation marks).", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Sprite"); //remove the setting
                        }
                    }

                    //validate spawn weight
                    if (validTypes[x].Settings.ContainsKey("SpawnWeight"))
                    {
                        if (validTypes[x].Settings["SpawnWeight"] is long) //if this is a readable integer
                        {
                            int weight = Convert.ToInt32(validTypes[x].Settings["SpawnWeight"]);
                            if (weight < 1) //if the setting is too low
                            {
                                Monitor.Log($"The \"SpawnWeight\" setting for monster type \"{validTypes[x].MonsterName}\" is {weight} and will be ignored. Please use a number above 0.", LogLevel.Trace);
                                validTypes[x].Settings["SpawnWeight"] = (long)1; //set to 1
                            }
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"SpawnWeight\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("SpawnWeight"); //remove the setting
                        }
                    }

                    //validate facing direction
                    if (validTypes[x].Settings.ContainsKey("FacingDirection"))
                    {
                        if (validTypes[x].Settings["FacingDirection"] is string direction) //if this is a string
                        {
                            switch (direction.Trim().ToLower())
                            {
                                case "up":
                                case "right":
                                case "down":
                                case "left":
                                    break;
                                default:
                                    Monitor.Log($"The \"FacingDirection\" setting for monster type \"{validTypes[x].MonsterName}\" was not recognized and will be ignored: \"{direction}\".", LogLevel.Info);
                                    Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);
                                    validTypes[x].Settings.Remove("FacingDirection"); //remove the setting
                                    break;
                            }
                        }
                        else //if this is NOT a string
                        {
                            Monitor.Log($"The \"FacingDirection\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's a string.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("FacingDirection"); //remove the setting
                        }
                    }

                    //validate segments
                    if (validTypes[x].Settings.ContainsKey("Segments"))
                    {
                        if (validTypes[x].Settings["Segments"] is long) //if this is a readable integer
                        {
                            //do nothing; minimum values will vary between monster types
                        }
                        else //if this isn't a readable integer
                        {
                            Monitor.Log($"The \"Segments\" setting for monster type \"{validTypes[x].MonsterName}\" couldn't be parsed. Please make sure it's an integer.", LogLevel.Info);
                            Monitor.Log($"Affected spawn area: {areaID}", LogLevel.Info);

                            validTypes[x].Settings.Remove("Segments"); //remove the setting
                        }
                    }
                }

                return validTypes;
            }
        }
    }
}