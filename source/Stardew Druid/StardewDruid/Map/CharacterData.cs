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

namespace StardewDruid.Map
{
    public static class CharacterData
    {

        public static void CharacterCheck(int progress)
        {

            if(progress >= 1 && Mod.instance.CharacterMap("Effigy") == null)
            {

                Mod.instance.CharacterRegister("Effigy", "FarmCave");

            }

            if (progress >= 20 && Mod.instance.CharacterMap("Jester") == null)
            {

                Mod.instance.CharacterRegister("Jester", "FarmCave");

            }

            if (progress >= 35 && Mod.instance.CharacterMap("Shadowtin") == null)
            {

                Mod.instance.CharacterRegister("Shadowtin", "FarmCave");

            }

        }

        public static bool CharacterFind(string characterName)
        {
            foreach (GameLocation location in (IEnumerable<GameLocation>)Game1.locations)
            {

                if (location.characters.Count > 0)
                {
                    for (int index = location.characters.Count - 1; index >= 0; --index)
                    {
                        NPC character = location.characters[index];

                        if (character is StardewDruid.Character.Character druidCharacter)
                        {
                            if(druidCharacter.Name == characterName)
                            {
                                Mod.instance.characters[characterName] = druidCharacter;

                            }
                        }

                    }
                }

            }
            return false;

        }

        public static void CharacterDefault(string characterName, string characterStart)
        {

            if (!Context.IsMainPlayer)
            {

                return;

            }

            string startMap = characterStart;

            if (characterStart == "Follow")
            {

                startMap = "Farm";

            }

            if (Mod.instance.characters.ContainsKey(characterName))
            {

                Mod.instance.characters[characterName].DefaultMap = startMap;

                Mod.instance.characters[characterName].DefaultPosition = WarpData.WarpStart(startMap);

            }

        }

        public static void CharacterLoad(string characterName, string characterStart)
        {
            
            if (!Context.IsMainPlayer)
            { 
            
                return; 
            
            }

            string startMap = characterStart;

            if (characterStart == "Follow")
            {

                startMap = "Farm";

            }

            if (Mod.instance.characters.ContainsKey(characterName))
            {

                Mod.instance.characters[characterName].WarpToDefault();

                return;

            }

            // ------------------------------ Effigy

            if (characterName == "Effigy")
            {

                Vector2 position = WarpData.WarpStart(startMap);

                Effigy npcEffigy = new(position, startMap);

                GameLocation startLocation = Game1.getLocationFromName(startMap);

                startLocation.characters.Add(npcEffigy);

                npcEffigy.currentLocation = startLocation;

                npcEffigy.update(Game1.currentGameTime, startLocation);

                if (characterStart == "Follow")
                {

                    npcEffigy.SwitchFollowMode(Game1.player);

                }
                else if(startMap == "Farm")
                {

                    npcEffigy.SwitchRoamMode();

                }

                Mod.instance.characters.Add("Effigy", npcEffigy);

                Mod.instance.dialogue["Effigy"] = new Dialogue.Effigy() { npc = Mod.instance.characters["Effigy"] };

                // Cave choice event triggered
                if (Game1.player.caveChoice.Value == 0 && Game1.player.totalMoneyEarned > 25000 && Game1.player.totalMoneyEarned < 30000)
                {

                    Mod.instance.dialogue["Effigy"].AddSpecial("Effigy", "Demetrius");

                }

            }

            // ----------------------------- Jester

            if (characterName == "Jester")
            {

                Vector2 position = WarpData.WarpStart(startMap);

                Jester npcJester = new(position, startMap);

                GameLocation startLocation = Game1.getLocationFromName(startMap);

                startLocation.characters.Add(npcJester);

                npcJester.currentLocation = startLocation;

                npcJester.update(Game1.currentGameTime, startLocation);

                if (characterStart == "Follow")
                {

                    npcJester.SwitchFollowMode(Game1.player);

                }
                else if (startMap == "Farm")
                {

                    npcJester.SwitchRoamMode();

                }

                Mod.instance.characters.Add("Jester", npcJester);

                Mod.instance.dialogue["Jester"] = new Dialogue.Jester() { npc = Mod.instance.characters["Jester"] };

            }

            // ----------------------------- Shadowtin

            if (characterName == "Shadowtin")
            {

                Vector2 position = WarpData.WarpStart(startMap);

                Shadowtin npcShadowtin = new(position, startMap);

                GameLocation startLocation = Game1.getLocationFromName(startMap);

                startLocation.characters.Add(npcShadowtin);

                npcShadowtin.currentLocation = startLocation;

                npcShadowtin.update(Game1.currentGameTime, startLocation);

                if (characterStart == "Follow")
                {

                    npcShadowtin.SwitchFollowMode(Game1.player);

                }
                else if (startMap == "Farm")
                {

                    npcShadowtin.SwitchRoamMode();

                }

                Mod.instance.characters.Add("Shadowtin", npcShadowtin);

                Mod.instance.dialogue["Shadowtin"] = new Dialogue.Shadowtin() { npc = Mod.instance.characters["Shadowtin"] };

            }

        }

        public static Texture2D CharacterTexture(string characterName)
        {

            Texture2D characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + ".png"));

            return characterTexture;

        }

        public static Texture2D CharacterPortrait(string characterName)
        {
            Texture2D characterPortrait;

            switch (characterName)
            {
                case "Jester":
                case "Shadowtin":

                    characterPortrait = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + "Portrait.png"));

                    break;

                default:

                    characterPortrait = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", "EffigyPortrait.png"));

                    break;

            }

            return characterPortrait;

        }

        public static Dictionary<int, int[]> CharacterSchedule(string characterName)
        {

            return new Dictionary<int, int[]>();

        }

        public static CharacterDisposition CharacterDisposition(string characterName)
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

            if (characterName == "Jester")
            {

                disposition.id += 1;
                disposition.Birthday_Season = "fall";

            }

            if(characterName == "Shadowtin")
            {

                disposition.id += 2;
                disposition.Birthday_Season = "winter";

            }

            return disposition;

        }

        public static StardewDruid.Character.Actor DisembodiedVoice(GameLocation location, Vector2 position)
        {

            Actor actor = new Actor(position, location.Name, "Disembodied");
            actor.SwitchSceneMode();
            //actor.IsInvisible = true;
            //actor.eventActor = true;
            actor.collidesWithOtherCharacters.Value = true;
            actor.farmerPassesThrough = true;
            return actor;

        }

        public static void RelocateTo(string name, string locate)
        {

            Mod.instance.CharacterRegister(name, locate);

            if (!Context.IsMainPlayer)
            {

                QueryData queryData = new()
                {
                    name = name,
                    longId = Game1.player.UniqueMultiplayerID,
                    location = locate,
                };

                string queryString = locate == "Follow" ? "CharacterFollow" : "CharacterRelocate";

                Mod.instance.EventQuery(queryData, queryString);

                return;

            }

            CharacterDefault(name, locate);

            if (locate == "Follow")
            {
                
                Mod.instance.characters[name].SwitchFollowMode();

                return;

            }

            Mod.instance.characters[name].WarpToDefault();

            if(locate == "Farm")
            {

                Mod.instance.characters[name].SwitchRoamMode();

                return;

            }

            Mod.instance.characters[name].SwitchDefaultMode();

        }

        public static void CharacterQuery(string name, string eventQuery = "CharacterFollow")
        {

            if (Context.IsMultiplayer)
            {
                
                QueryData queryData = new()
                {
                    name = name,
                    longId = Game1.player.UniqueMultiplayerID,
                    value = eventQuery,
                };

                Mod.instance.EventQuery(queryData, "CharacterCommand");

            }

        }

        public static void QueryCommand(QueryData queryData)
        {

            switch (queryData.value)
            {

                case "CharacterContinue":
                    Mod.instance.characters[queryData.name].DeactivateStandby();

                    Mod.instance.CastMessage(queryData.name + " on task for " + Game1.getFarmer(queryData.longId).Name);
                    break;
                case "CharacterStandby":
                    Mod.instance.characters[queryData.name].ActivateStandby();

                    Mod.instance.CastMessage(queryData.name + " stands by for " + Game1.getFarmer(queryData.longId).Name);
                    break;
                case "CharacterFollow":
                    Farmer follow = Game1.getFarmer(queryData.longId);

                    Mod.instance.characters[queryData.name].SwitchFollowMode(follow);

                    Mod.instance.CastMessage(queryData.name + " is following " + follow.Name);
                    break;
                case "CharacterRelocate":
                    RelocateTo(queryData.name, queryData.location);

                    Mod.instance.CastMessage(queryData.name + " sent to " + queryData.location + " by " + Game1.getFarmer(queryData.longId).Name);
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
        public StardewValley.Gender Gender;
        public bool datable;
        public string Birthday_Season;
        public int Birthday_Day;
        public int id;
        public int speed;
    }

}
