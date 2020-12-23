/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Monsters;
using FarmTypeManager.Monsters;

namespace FarmTypeManager
{
    public partial class ModEntry : Mod
    {
        /// <summary>Methods used repeatedly by other sections of this mod, e.g. to locate tiles.</summary>
        private static partial class Utility
        {
            /// <summary>Generates a monster and places it on the specified map and tile.</summary>
            /// <param name="monsterType">The monster type's name and an optional dictionary of monster-specific settings.</param>
            /// <param name="location">The GameLocation where the monster should be spawned.</param>
            /// <param name="tile">The x/y coordinates of the tile where the monster should be spawned.</param>
            /// <param name="areaID">The UniqueAreaID of the related SpawnArea. Required for log messages.</param>
            /// <returns>Returns the monster's ID value, or null if the spawn process failed.</returns>
            public static int? SpawnMonster(MonsterType monsterType, GameLocation location, Vector2 tile, string areaID = "")
            {
                Monster monster = null; //an instatiated monster, to be spawned into the world later

                Color? color = null; //the monster's color (used by specific monster types)
                if (monsterType.Settings != null) //if settings were provided
                {
                    if (monsterType.Settings.ContainsKey("Color")) //if this setting was provided
                    {
                        string[] colorText = ((string)monsterType.Settings["Color"]).Trim().Split(' '); //split the setting string into strings for each number
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
                        else //if the setting did not include an "A" value
                        {
                            a = 255; //default to no transparency
                        }

                        color = new Color(r, g, b, a); //set the color
                    }
                    else if (monsterType.Settings.ContainsKey("MinColor") && monsterType.Settings.ContainsKey("MaxColor")) //if color wasn't provided, but mincolor & maxcolor were
                    {
                        string[] minColorText = ((string)monsterType.Settings["MinColor"]).Trim().Split(' '); //split the setting string into strings for each number
                        List<int> minColorNumbers = new List<int>();
                        foreach (string text in minColorText) //for each string
                        {
                            int num = Convert.ToInt32(text); //convert it to a number
                            if (num < 0) { num = 0; } //minimum 0
                            else if (num > 255) { num = 255; } //maximum 255
                            minColorNumbers.Add(num); //add it to the list
                        }

                        string[] maxColorText = ((string)monsterType.Settings["MaxColor"]).Trim().Split(' '); //split the setting string into strings for each number
                        List<int> maxColorNumbers = new List<int>();
                        foreach (string text in maxColorText) //for each string
                        {
                            int num = Convert.ToInt32(text); //convert it to a number
                            if (num < 0) { num = 0; } //minimum 0
                            else if (num > 255) { num = 255; } //maximum 255
                            maxColorNumbers.Add(num); //convert to number
                        }

                        for (int x = 0; x < minColorNumbers.Count && x < maxColorNumbers.Count; x++) //for each pair of values
                        {
                            if (minColorNumbers[x] > maxColorNumbers[x]) //if min > max
                            {
                                //swap min and max
                                int temp = minColorNumbers[x];
                                minColorNumbers[x] = maxColorNumbers[x];
                                maxColorNumbers[x] = temp;
                            }
                        }

                        //pick random RGBA values between min and max
                        int r = RNG.Next(minColorNumbers[0], maxColorNumbers[0] + 1);
                        int g = RNG.Next(minColorNumbers[1], maxColorNumbers[1] + 1);
                        int b = RNG.Next(minColorNumbers[2], maxColorNumbers[2] + 1);
                        int a;
                        if (minColorNumbers.Count > 3 && maxColorNumbers.Count > 3) //if both settings included an "A" value
                        {
                            a = RNG.Next(minColorNumbers[3], maxColorNumbers[3] + 1);
                        }
                        else //if one/both of the settings did not include an "A" value
                        {
                            a = 255; //default to no transparency
                        }

                        color = new Color(r, g, b, a); //set the color
                    }
                }

                bool seesPlayers = false; //whether the monster automatically "sees" players at spawn (handled differently by some monster types)
                if (monsterType.Settings != null) //if settings were provided
                {
                    if (monsterType.Settings.ContainsKey("SeesPlayersAtSpawn")) //if this setting was provided
                    {
                        seesPlayers = (bool)monsterType.Settings["SeesPlayersAtSpawn"]; //use the provided setting
                    }
                }

                int facingDirection = 2; //the direction the monster should be facing at spawn (handled differently by some monster types)
                if (monsterType.Settings != null) //if settings were provided
                {
                    if (monsterType.Settings.ContainsKey("FacingDirection")) //if this setting was provided
                    {
                        string directionString = (string)monsterType.Settings["FacingDirection"]; //get the provided setting
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
                    }
                }

                //create a new monster based on the provided name & apply type-specific settings
                switch (monsterType.MonsterName.ToLower()) //avoid most casing issues by making this lower-case
                {
                    case "bat":
                        monster = new BatFTM(tile, 0);
                        break;
                    case "frostbat":
                    case "frost bat":
                        monster = new BatFTM(tile, 40);
                        break;
                    case "lavabat":
                    case "lava bat":
                        monster = new BatFTM(tile, 80);
                        break;
                    case "iridiumbat":
                    case "iridium bat":
                        monster = new BatFTM(tile, 171);
                        break;
                    case "doll":
                    case "curseddoll":
                    case "cursed doll":
                        monster = new BatFTM(tile, -666);
                        break;
                    case "skull":
                    case "hauntedskull":
                    case "haunted skull":
                        monster = new BatFTM(tile, 77377);
                        break;
                    case "magmasprite":
                    case "magma sprite":
                        monster = new BatFTM(tile, -555);
                        break;
                    case "magmasparker":
                    case "magma sparker":
                        monster = new BatFTM(tile, -556);
                        break;
                    case "bigslime":
                    case "big slime":
                    case "biggreenslime":
                    case "big green slime":
                        monster = new BigSlimeFTM(tile, 0);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        if (seesPlayers) //if the "SeesPlayersAtSpawn" setting is true
                        {
                            monster.IsWalkingTowardPlayer = true;
                        }
                        break;
                    case "bigblueslime":
                    case "big blue slime":
                    case "bigfrostjelly":
                    case "big frost jelly":
                        monster = new BigSlimeFTM(tile, 40);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        if (seesPlayers) //if the "SeesPlayersAtSpawn" setting is true
                        {
                            monster.IsWalkingTowardPlayer = true;
                        }
                        break;
                    case "bigredslime":
                    case "big red slime":
                    case "bigredsludge":
                    case "big red sludge":
                        monster = new BigSlimeFTM(tile, 80);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        if (seesPlayers) //if the "SeesPlayersAtSpawn" setting is true
                        {
                            monster.IsWalkingTowardPlayer = true;
                        }
                        break;
                    case "bigpurpleslime":
                    case "big purple slime":
                    case "bigpurplesludge":
                    case "big purple sludge":
                        monster = new BigSlimeFTM(tile, 121);
                        if (color.HasValue) //if color was provided
                        {
                            ((BigSlimeFTM)monster).c.Value = color.Value; //set its color after creation
                        }
                        if (seesPlayers) //if the "SeesPlayersAtSpawn" setting is true
                        {
                            monster.IsWalkingTowardPlayer = true;
                        }
                        break;
                    case "bluesquid":
                    case "blue squid":
                        monster = new BlueSquid(tile);
                        break;
                    case "bug":
                        monster = new Bug(tile, 0);
                        break;
                    case "armoredbug":
                    case "armored bug":
                        monster = new Bug(tile, 121);
                        break;
                    case "dino":
                    case "dinomonster":
                    case "dino monster":
                    case "pepper":
                    case "pepperrex":
                    case "pepper rex":
                    case "rex":
                        monster = new DinoMonster(tile);
                        break;
                    case "duggy":
                        monster = new DuggyFTM(tile);
                        break;
                    case "magmaduggy":
                    case "magma duggy":
                        monster = new DuggyFTM(tile, true);
                        break;
                    case "dust":
                    case "sprite":
                    case "dustsprite":
                    case "dust sprite":
                    case "spirit":
                    case "dustspirit":
                    case "dust spirit":
                        monster = new DustSpirit(tile);
                        break;
                    case "dwarvishsentry":
                    case "dwarvish sentry":
                    case "dwarvish":
                    case "sentry":
                        monster = new DwarvishSentry(tile);
                        for (int x = Game1.delayedActions.Count - 1; x >= 0; x--) //check each existing DelayedAction (from last to first)
                        {
                            if (Game1.delayedActions[x].stringData == "DwarvishSentry") //if this action seems to be playing this monster's sound effect
                            {
                                Game1.delayedActions.Remove(Game1.delayedActions[x]); //remove the action (preventing this monster's global sound effect after creation)
                                break; //skip the rest of the actions
                            }
                        }
                        break;
                    case "ghost":
                        monster = new GhostFTM(tile);
                        break;
                    case "carbonghost":
                    case "carbon ghost":
                        monster = new GhostFTM(tile, "Carbon Ghost");
                        break;
                    case "putridghost":
                    case "putrid ghost":
                        monster = new GhostFTM(tile, "Putrid Ghost");
                        break;
                    case "slime":
                    case "greenslime":
                    case "green slime":
                        monster = new GreenSlime(tile, 0);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "blueslime":
                    case "blue slime":
                    case "frostjelly":
                    case "frost jelly":
                        monster = new GreenSlime(tile, 40);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "redslime":
                    case "red slime":
                    case "redsludge":
                    case "red sludge":
                        monster = new GreenSlime(tile, 80);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "purpleslime":
                    case "purple slime":
                    case "purplesludge":
                    case "purple sludge":
                        monster = new GreenSlime(tile, 121);
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "tigerslime":
                    case "tiger slime":
                        monster = new GreenSlime(tile, 0); //create any "normal" slime
                        ((GreenSlime)monster).makeTigerSlime(); //convert it into a tiger slime
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "prismaticslime":
                    case "prismatic slime":
                        monster = new GreenSlime(tile, 0); //create any "normal" slime
                        ((GreenSlime)monster).makePrismatic(); //convert it into a prismatic slime
                        if (color.HasValue) //if color was also provided
                        {
                            ((GreenSlime)monster).color.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "grub":
                    case "cavegrub":
                    case "cave grub":
                        monster = new GrubFTM(tile, false);
                        break;
                    case "fly":
                    case "cavefly":
                    case "cave fly":
                        monster = new FlyFTM(tile, false);
                        break;
                    case "mutantgrub":
                    case "mutant grub":
                        monster = new GrubFTM(tile, true);
                        break;
                    case "mutantfly":
                    case "mutant fly":
                        monster = new FlyFTM(tile, true);
                        break;
                    case "metalhead":
                    case "metal head":
                        monster = new MetalHead(tile, 0);
                        if (color.HasValue) //if color was provided
                        {
                            ((MetalHead)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "hothead":
                    case "hot head":
                        monster = new HotHead(tile);
                        if (color.HasValue) //if color was provided
                        {
                            ((HotHead)monster).c.Value = color.Value; //set its color after creation
                        }
                        break;
                    case "lavalurk":
                    case "lava lurk":
                        monster = new LavaLurk(tile);
                        break;
                    case "leaper":
                        monster = new Leaper(tile);
                        break;
                    case "mummy":
                        monster = new MummyFTM(tile);
                        break;
                    case "rockcrab":
                    case "rock crab":
                        monster = new RockCrab(tile);
                        break;
                    case "lavacrab":
                    case "lava crab":
                        monster = new LavaCrab(tile);
                        break;
                     case "iridiumcrab":
                    case "iridium crab":
                        monster = new RockCrab(tile, "Iridium Crab");
                        break;
                    case "falsemagmacap":
                    case "false magma cap":
                    case "magmacap":
                    case "magma cap":
                        monster = new RockCrab(tile, "False Magma Cap");
                        monster.HideShadow = true; //hide shadow, making them look more like "real" magma caps
                        break;
                    case "stickbug":
                    case "stick bug":
                        monster = new RockCrab(tile);
                        (monster as RockCrab).makeStickBug();
                        break;
                    case "rockgolem":
                    case "rock golem":
                    case "stonegolem":
                    case "stone golem":
                        monster = new RockGolemFTM(tile);
                        break;
                    case "wildernessgolem":
                    case "wilderness golem":
                        monster = new RockGolemFTM(tile, Game1.player.CombatLevel);
                        break;
                    case "serpent":
                        monster = new SerpentFTM(tile);
                        break;
                    case "royalserpent":
                    case "royal serpent":
                        monster = new SerpentFTM(tile, "Royal Serpent");
                        break;
                    case "brute":
                    case "shadowbrute":
                    case "shadow brute":
                        monster = new ShadowBrute(tile);
                        break;
                    case "shaman":
                    case "shadowshaman":
                    case "shadow shaman":
                        monster = new ShadowShaman(tile);
                        break;
                    case "sniper":
                    case "shadowsniper":
                    case "shadow sniper":
                        monster = new Shooter(tile);
                        break;
                    case "skeleton":
                        monster = new Skeleton(tile);
                        if (seesPlayers) //if the "SeesPlayersAtSpawn" setting is true
                        {
                            IReflectedField<bool> spottedPlayer = Helper.Reflection.GetField<bool>(monster, "spottedPlayer"); //try to access this skeleton's private "spottedPlayer" field
                            spottedPlayer.SetValue(true);
                            monster.IsWalkingTowardPlayer = true;
                        }
                        break;
                    case "skeletonmage":
                    case "skeleton mage":
                        monster = new Skeleton(tile, true);
                        if (seesPlayers) //if the "SeesPlayersAtSpawn" setting is true
                        {
                            IReflectedField<bool> spottedPlayer = Helper.Reflection.GetField<bool>(monster, "spottedPlayer"); //try to access this skeleton's private "spottedPlayer" field
                            spottedPlayer.SetValue(true);
                            monster.IsWalkingTowardPlayer = true;
                        }
                        break;
                    case "spiker":
                        monster = new Spiker(tile, facingDirection);
                        break;
                    case "squidkid":
                    case "squid kid":
                        monster = new SquidKidFTM(tile);
                        break;
                    default: //if the name doesn't match any directly known monster types
                        Type externalType = GetTypeFromName(monsterType.MonsterName, typeof(Monster)); //find a monster subclass with a matching name
                        monster = (Monster)Activator.CreateInstance(externalType, tile); //create a monster with the Vector2 constructor
                        break;
                }

                if (monster == null)
                {
                    Monitor.Log($"The monster to be spawned (\"{monsterType.MonsterName}\") doesn't match any known monster types. Make sure that name isn't misspelled in your config file.", LogLevel.Info);
                    return null;
                }

                int? ID = MonsterTracker.AddMonster(monster); //generate an ID for this monster
                if (!ID.HasValue)
                {
                    Monitor.Log("A new monster ID could not be generated. This is may be due to coding issue; please report it to this mod's developer. This monster won't be spawned.", LogLevel.Warn);
                    return null;
                }
                monster.id = ID.Value; //assign the ID to this monster

                monster.MaxHealth = monster.Health; //some monster types set Health on creation and expect MaxHealth to be updated like this

                ApplyMonsterSettings(monster, monsterType.Settings, areaID); //adjust the monster based on any other provided optional settings

                //spawn the completed monster at the target location
                Monitor.VerboseLog($"Spawning monster. Type: {monsterType.MonsterName}. Location: {tile.X},{tile.Y} ({location.Name}).");
                monster.currentLocation = location;
                monster.setTileLocation(tile);
                location.addCharacter(monster);
                return monster.id;
            }
        }
    }
}