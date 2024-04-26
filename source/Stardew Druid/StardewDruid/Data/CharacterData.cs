/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Neosinf/StardewDruid
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewDruid.Character;
using StardewDruid.Location;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Quests;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using xTile;
using static StardewDruid.Data.CharacterData;

namespace StardewDruid.Data
{
    public static class CharacterData
    {

        public enum locations
        {
            home,
            farm
        }

        public enum characters
        {   
            none,
            disembodied,
            effigy
        }
        
        public static Dictionary<characters, string> CharacterNames()
        {

            Dictionary<characters, string> names = new()
            {
                [characters.disembodied] = "Disembodied",
                [characters.effigy] = "Effigy"

            };

            return names;

        }

        public static string GetCharacterName(characters characters)
        {
            return CharacterNames()[characters];

        }

        public static Dictionary<string, characters> CharacterTypes()
        {
            Dictionary<string, characters> names = new()
            {
                ["Actor"] = characters.disembodied,
                ["Effigy"] = characters.effigy,

            };

            return names;

        }

        public static Vector2 CharacterStart(locations location)
        {

            switch (location)
            {
                case locations.home:

                    return WarpData.WarpStart(LocationData.druid_grove_name);

                case locations.farm:

                    return WarpData.WarpStart("Farm");

            }

            return Vector2.Zero;

        }

        public static string CharacterLocation(locations location)
        {

            switch (location)
            {

                case locations.home:

                    return LocationData.druid_grove_name;

                case locations.farm:

                    return "Farm";

            }

            return null;

        }

        public static void CharacterWarp(StardewDruid.Character.Character entity, locations destination)
        {
            
            if (entity.currentLocation != null)
            {
                entity.currentLocation.characters.Remove(entity);

            }

            entity.Position = CharacterStart(destination);

            entity.currentLocation = Game1.getLocationFromName(CharacterLocation(destination));

            entity.currentLocation.characters.Add(entity);

        }

        public static void CharacterRemove(characters character)
        {

            if(Mod.instance.characters.ContainsKey(character))
            {

                StardewDruid.Character.Character entity = Mod.instance.characters[character];

                if (entity.currentLocation != null)
                {
                    entity.currentLocation.characters.Remove(entity);

                }

                Mod.instance.characters.Remove(character);

            }

        }

        public static void CharacterLoad(characters character, StardewDruid.Character.Character.mode mode)
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            if (Mod.instance.characters.ContainsKey(character))
            {

                Mod.instance.characters[character].SwitchToMode(mode, Game1.player);

                return;

            }

            switch (character)
            {

                case characters.effigy:


                    Mod.instance.characters[characters.effigy] = new Effigy(characters.effigy);

                    Mod.instance.dialogue[characters.effigy] = new(Mod.instance.characters[characters.effigy]);

                    Mod.instance.characters[characters.effigy].SwitchToMode(mode, Game1.player);

                    break;
                

            }

        }

        public static void CharacterFind(characters character)
        {

            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {

                if (location.characters.Count > 0)
                {
                    for (int index = location.characters.Count - 1; index >= 0; --index)
                    {
                        NPC npc = location.characters[index];

                        if (npc is StardewDruid.Character.Character entity)
                        {

                            if (entity.characterType == character)
                            {
                                
                                Mod.instance.characters[character] = entity;

                            }

                        }

                    }

                }

            }

            if(Context.IsMainPlayer)
            {

                CharacterLoad(character, Character.Character.mode.home);

            }

        }

        public static Texture2D CharacterTexture(characters character)
        {

            switch (character)
            {
                /*case characters.jester:
                case characters.shadowtin:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", CharacterNames()[character] + ".png"));*/

                default:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "Effigy.png"));

            }

        }

        public static Texture2D CharacterPortrait(characters character)
        {

            switch (character)
            {
                /*case characters.jester:
                case characters.shadowtin:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", CharacterNames()[character] + "Portrait.png"));*/

                default:

                    return Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EffigyPortrait.png"));

            }

        }

        public static CharacterDisposition CharacterDisposition(characters character)
        {

            CharacterDisposition disposition = new()
            {
                Age = 30,
                Manners = 2,
                SocialAnxiety = 1,
                Optimism = 0,
                Gender = 0,
                datable = false,
                Birthday_Season = "summer",
                Birthday_Day = 27,
                id = 18465001,
                speed = 2,

            };
            /*
            if (characterName == "Jester")
            {

                disposition.id += 1;
                disposition.Birthday_Season = "fall";

            }

            if (characterName == "Shadowtin")
            {

                disposition.id += 2;
                disposition.Birthday_Season = "winter";

            }*/

            return disposition;

        }

        public static void CharacterQuery(characters character, string eventQuery = "CharacterFollow")
        {

            if (Context.IsMultiplayer)
            {

                QueryData queryData = new()
                {
                    name = character.ToString(),
                    longId = Game1.player.UniqueMultiplayerID,
                    value = eventQuery,
                };

                Mod.instance.EventQuery(queryData, "CharacterCommand");

            }

        }

        public static void QueryCommand(QueryData queryData)
        {

            characters character = (characters)Enum.Parse(typeof(characters), queryData.name);

            string Name = CharacterNames()[character];

            switch (queryData.value)
            {

                case "CharacterStandby":
                    
                    Mod.instance.characters[character].SwitchToMode(Character.Character.mode.scene, Game1.player);

                    Mod.instance.CastMessage(Name + " stands by for " + Game1.getFarmer(queryData.longId).Name);
                    
                    break;
               
                case "CharacterFollow":

                    Farmer follow = Game1.getFarmer(queryData.longId);

                    Mod.instance.characters[character].SwitchToMode(Character.Character.mode.track,follow);

                    Mod.instance.CastMessage(Name + " is following " + follow.Name);
                
                    break;
                
                case "CharacterRoam":

                    Mod.instance.characters[character].SwitchToMode(Character.Character.mode.roam, Game1.player);

                    Mod.instance.CastMessage(Name + " sent to the farm by " + Game1.getFarmer(queryData.longId).Name);
                    
                    break;

                case "CharacterHome":
                    
                    Mod.instance.characters[character].SwitchToMode(Character.Character.mode.home, Game1.player);

                    Mod.instance.CastMessage(Name + " sent to home by " + Game1.getFarmer(queryData.longId).Name);
                    
                    break;

            }

        }

    }

    public class CharacterDisposition
    {
        public int Age;
        public int Manners;
        public int SocialAnxiety;
        public int Optimism;
        public Gender Gender;
        public bool datable;
        public string Birthday_Season;
        public int Birthday_Day;
        public int id;
        public int speed;
    }

}
