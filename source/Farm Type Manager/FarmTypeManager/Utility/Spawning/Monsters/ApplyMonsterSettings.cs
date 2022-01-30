/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using FarmTypeManager.Monsters;
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
            /// <summary>Apply any provided non-type-specific settings to a monster.</summary>
            /// <param name="monster">The monster to be customized. This will be modified by reference.</param>
            /// <param name="settings">A dictionary of setting names and values. If null, this method will do nothing.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            public static void ApplyMonsterSettings(Monster monster, Dictionary<string, object> settings, string areaID = "")
            {
                if (settings == null) //if no settings were provided
                {
                    return; //do nothing
                }

                //set max HP
                if (settings.ContainsKey("HP"))
                {
                    monster.MaxHealth = Convert.ToInt32(settings["HP"]);
                }

                //set damage
                if (settings.ContainsKey("Damage"))
                {
                    monster.DamageToFarmer = Convert.ToInt32(settings["Damage"]); //set DamageToFarmer

                    if (monster is ICustomDamage cd) //if this monster type uses the custom damage interface
                    {
                        cd.CustomDamage = Convert.ToInt32(settings["Damage"]); //set CustomDamage as well
                    }
                }

                //set defense
                if (settings.ContainsKey("Defense"))
                {
                    monster.resilience.Value = Convert.ToInt32(settings["Defense"]);
                }

                //set dodge chance
                if (settings.ContainsKey("DodgeChance"))
                {
                    monster.missChance.Value = ((double)Convert.ToInt32(settings["DodgeChance"])) / 100;
                }

                //set experience points
                if (settings.ContainsKey("EXP"))
                {
                    monster.ExperienceGained = Convert.ToInt32(settings["EXP"]);
                }

                //multiply HP and/or damage based on players' highest level in a skill
                if (settings.ContainsKey("RelatedSkill"))
                {
                    //parse the provided skill into an enum
                    Utility.Skills skill = (Utility.Skills)Enum.Parse(typeof(Utility.Skills), ((string)settings["RelatedSkill"]).Trim(), true); //parse while trimming whitespace and ignoring case

                    //multiply HP
                    if (settings.ContainsKey("PercentExtraHPPerSkillLevel"))
                    {
                        //calculate HP multiplier based on skill level
                        double skillMultiplier = Convert.ToInt32(settings["PercentExtraHPPerSkillLevel"]);
                        skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. "10" (10% per level) converts to "0.1"
                        int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                        }
                        skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. if the setting is "10", this is "1.0" at level 0, "1.7" at level 7, etc

                        //apply the multiplier to the monster's max HP
                        skillMultiplier *= monster.MaxHealth; //multiply the current max HP by the skill multiplier
                        monster.MaxHealth = Math.Max((int)skillMultiplier, 1); //set the monster's new max HP (rounded down to the nearest integer & minimum 1)
                    }

                    //multiply damage
                    if (settings.ContainsKey("PercentExtraDamagePerSkillLevel"))
                    {
                        //calculate damage multiplier based on skill level
                        double skillMultiplier = Convert.ToInt32(settings["PercentExtraDamagePerSkillLevel"]);
                        skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. "10" (10% per level) converts to "0.1"
                        int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                        }
                        skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. if the setting is "10", this is "1.0" at level 0, "1.7" at level 7, etc

                        //apply the multiplier to the monster's damage
                        skillMultiplier *= monster.DamageToFarmer; //multiply the current damage by the skill multiplier
                        monster.DamageToFarmer = Math.Max((int)skillMultiplier, 0); //set the monster's new damage (rounded down to the nearest integer & minimum 0)
                    }

                    //multiply defense
                    if (settings.ContainsKey("PercentExtraDefensePerSkillLevel"))
                    {
                        //calculate defense multiplier based on skill level
                        double skillMultiplier = Convert.ToInt32(settings["PercentExtraDefensePerSkillLevel"]);
                        skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. "10" (10% per level) converts to "0.1"
                        int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                        }
                        skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. if the setting is "10", this is "1.0" at level 0, "1.7" at level 7, etc

                        //apply the multiplier to the monster's defense
                        skillMultiplier *= monster.resilience.Value; //multiply the current damage by the skill multiplier
                        monster.resilience.Value = Math.Max((int)skillMultiplier, 0); //set the monster's new defense (rounded down to the nearest integer & minimum 0)
                    }

                    //multiply dodge chance
                    if (settings.ContainsKey("PercentExtraDodgeChancePerSkillLevel"))
                    {
                        //calculate dodge chance multiplier based on skill level
                        double skillMultiplier = Convert.ToInt32(settings["PercentExtraDodgeChancePerSkillLevel"]);
                        skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. "10" (10% per level) converts to "0.1"
                        int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                        }
                        skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. if the setting is "10", this is "1.0" at level 0, "1.7" at level 7, etc

                        //apply the multiplier to the monster's dodge chance
                        skillMultiplier *= monster.missChance.Value; //multiply the current damage by the skill multiplier
                        monster.missChance.Value = Math.Max((int)skillMultiplier, 0); //set the monster's new dodge chance (rounded down to the nearest integer & minimum 0)
                    }

                    //multiply experience points
                    if (settings.ContainsKey("PercentExtraEXPPerSkillLevel"))
                    {
                        //calculate exp multiplier based on skill level
                        double skillMultiplier = Convert.ToInt32(settings["PercentExtraEXPPerSkillLevel"]);
                        skillMultiplier = (skillMultiplier / 100); //converted to percent, e.g. "10" (10% per level) converts to "0.1"
                        int highestSkillLevel = 0; //highest skill level among all existing farmers, not just the host
                        foreach (Farmer farmer in Game1.getAllFarmers())
                        {
                            highestSkillLevel = Math.Max(highestSkillLevel, farmer.getEffectiveSkillLevel((int)skill)); //record the new level if it's higher than before
                        }
                        skillMultiplier = 1.0 + (skillMultiplier * highestSkillLevel); //final multiplier; e.g. if the setting is "10", this is "1.0" at level 0, "1.7" at level 7, etc

                        //apply the multiplier to the monster's exp
                        skillMultiplier *= monster.ExperienceGained; //multiply the current exp by the skill multiplier
                        monster.ExperienceGained = Math.Max((int)skillMultiplier, 0); //set the monster's new exp (rounded down to the nearest integer & minimum 0)
                    }
                }

                //set loot (i.e. items dropped on death by the monster)
                if (settings.ContainsKey("Loot"))
                {
                    List<SavedObject> loot = ((JArray)settings["Loot"]).ToObject<List<SavedObject>>(); //cast this list of saved objects (already validated and parsed elsewhere)
                    MonsterTracker.SetLoot(monster, loot); //set the monster's loot in the monster tracker
                    monster.objectsToDrop.Clear(); //clear any "default" loot the monster might've had
                }

                //set current HP
                //NOTE: do this after any max HP changes, because it may be contingent on max HP)
                if (settings.ContainsKey("CurrentHP"))
                {
                    monster.Health = Convert.ToInt32(settings["CurrentHP"]);
                }
                else //if no current HP setting was provided
                {
                    monster.Health = monster.MaxHealth; //set it to max HP
                }

                //set whether the monster automatically sees players at spawn
                if (settings.ContainsKey("SeesPlayersAtSpawn"))
                {
                    if ((bool)settings["SeesPlayersAtSpawn"]) //if the setting is true (note: supposedly, some monsters behave incorrectly if focusedOnFarmers is set to false)
                    {
                        monster.focusedOnFarmers = true; //set the monster focus
                    }
                }

                //set sprite
                if (settings.ContainsKey("Sprite"))
                {
                    //replace the monster's sprite, using its existing settings where possible
                    monster.Sprite = new AnimatedSprite((string)settings["Sprite"], monster.Sprite.CurrentFrame, monster.Sprite.SpriteWidth, monster.Sprite.SpriteHeight);
                }

                //set facing direction
                if (settings.ContainsKey("FacingDirection"))
                {
                    int facingDirection = 2;
                    string directionString = (string)settings["FacingDirection"]; //get the provided setting
                    switch (directionString.Trim().ToLower())
                    {
                        //get an integer representing the provided direction
                        case "up":
                            facingDirection = 0;
                            break;
                        case "right":
                            facingDirection = 1;
                            break;
                        case "down":
                            facingDirection = 2;
                            break;
                        case "left":
                            facingDirection = 3;
                            break;
                    }
                    monster.faceDirection(facingDirection); //set monster direction
                }

                //set segments
                if (settings.ContainsKey("Segments"))
                {
                    int segments = Convert.ToInt32(settings["Segments"]);

                    if (monster is GreenSlime slime) //if this monster is a type of slime
                    {
                        segments = Math.Max(0, segments); //minimum = 0

                        slime.stackedSlimes.Value = segments; //set the number of additional slimes stacked on top of this slime
                    }
                    else if (monster is Serpent serpent) //if this monster is a type of serpent
                    {
                        if (segments < 2)
                            segments = 0; //minimum 0, but treat 1 as 0 (to avoid potential issues with royal/non-royal serpent distinctions)

                        serpent.segmentCount.Value = segments; //set the number of additional body segments this serpent has
                    }
                }
            }
        }
    }
}