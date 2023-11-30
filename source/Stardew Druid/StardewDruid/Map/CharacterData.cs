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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace StardewDruid.Map
{
    public static class CharacterData
    {

        public static void CharacterLoad(string characterName, string startMap)
        {

            if (Mod.instance.characters.ContainsKey(characterName))
            {

                Mod.instance.characters[characterName].WarpToDefault();

                return;

            }

            // ------------------------------ Effigy

            if (characterName == "Effigy")
            {

                Vector2 position = CharacterPosition(startMap);

                Effigy npcEffigy = new(position, startMap);

                GameLocation startLocation = Game1.getLocationFromName(startMap);

                startLocation.characters.Add(npcEffigy);

                npcEffigy.update(Game1.currentGameTime, startLocation);

                if (startMap == "Farm")
                {

                    npcEffigy.SwitchRoamMode();

                }

                Mod.instance.characters.Add("Effigy", npcEffigy);

                Mod.instance.dialogue["Effigy"] = new Dialogue.Effigy() { npc = Mod.instance.characters["Effigy"] };

                // Cave choice event triggered
                if (Game1.player.caveChoice.Value == 0 && Game1.player.totalMoneyEarned > 25000)
                {

                    Mod.instance.dialogue["Effigy"].specialDialogue.Add("Demetrius", new() { "I had a peculiar visitor", "Did you meet Demetrius?", });

                }

            }

            // ----------------------------- Jester

            if (characterName == "Jester")
            {

                Vector2 position = CharacterPosition(startMap);

                Jester npcJester = new(position, startMap);

                GameLocation startLocation = Game1.getLocationFromName(startMap);

                startLocation.characters.Add(npcJester);

                npcJester.update(Game1.currentGameTime, startLocation);

                if (startMap == "Farm")
                {

                    npcJester.SwitchRoamMode();

                }

                Mod.instance.characters.Add("Jester", npcJester);

                Mod.instance.dialogue["Jester"] = new Dialogue.Jester() { npc = Mod.instance.characters["Jester"] };

            }

        }

        public static Texture2D CharacterTexture(string characterName)
        {

            Texture2D characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + ".png"));

            return characterTexture;

        }

        public static AnimatedSprite CharacterSprite(string characterName)
        {

            AnimatedSprite characterSprite = new();

            characterSprite.textureName.Value = "18465_" + characterName + "_Sprite";

            characterSprite.spriteTexture = CharacterTexture(characterName);

            characterSprite.loadedTexture = "18465_" + characterName + "_Sprite";

            switch (characterName)
            {

                case "Jester":


                    characterSprite.SpriteHeight = 32;

                    characterSprite.SpriteWidth = 32;

                    characterSprite.framesPerAnimation = 6;

                    break;

                case "Disembodied":

                    characterSprite.SpriteHeight = 16;

                    characterSprite.SpriteWidth = 16;

                    break;

                default:


                    characterSprite.SpriteHeight = 32;

                    characterSprite.SpriteWidth = 16;

                    break;

            }

            return characterSprite;

        }

        public static Texture2D CharacterPortrait(string characterName)
        {

            Texture2D characterPortrait = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + "Portrait.png"));

            return characterPortrait;

        }

        public static Dictionary<int, int[]> CharacterSchedule(string characterName)
        {

            return new Dictionary<int, int[]>();

        }

        public static CharacterDisposition CharacterDisposition(string characterName)
        {

            return new CharacterDisposition()
            {
                Age = 1,
                Manners = 2,
                SocialAnxiety = 1,
                Optimism = 0,
                Gender = 0,
                datable = false,
                Birthday_Season = "fall",
                Birthday_Day = 27,
                id = 18465001,
                speed = 1,

            };

        }

        public static Vector2 CharacterPosition(string defaultMap = "FarmCave")
        {

            switch (defaultMap)
            {

                case "Mountain":


                    return new Vector2(6176, 1728);


                default:

                    Dictionary<string, Vector2> farmPositions = new() { ["FarmCave"] = new Vector2(6, 6) * 64, ["Farm"] = Vector2.One * 64 };

                    foreach (Warp warp in Game1.getFarm().warps)
                    {

                        if (warp.TargetName == "FarmCave")
                        {

                            Vector2 cavePosition = new Vector2(warp.TargetX, warp.TargetY - 2) * 64;

                            //Vector2 caveOffset = (cavePosition.Y <= 512) ? new(0, 64) : new(0, -64);

                            Vector2 farmPosition = new Vector2(warp.X, warp.Y + 4) * 64;

                            //Vector2 farmOffset = (farmPosition.Y <= 512) ? new(0, 64) : new(0, -64);

                            farmPositions = new() { ["FarmCave"] = cavePosition, ["Farm"] = farmPosition, };

                        }

                    }

                    return farmPositions[defaultMap];

            }

        }

        public static StardewDruid.Character.Character DisembodiedVoice(GameLocation location, Vector2 position)
        {

            StardewDruid.Character.Character disembodied = new(position, location.Name, "Disembodied");

            //disembodied.frozenMode = true;

            disembodied.SwitchFrozenMode();

            disembodied.IsInvisible = true;

            disembodied.eventActor = true;

            //disembodied.forceUpdateTimer = 9999;

            disembodied.collidesWithOtherCharacters.Value = true;

            disembodied.farmerPassesThrough = true;

            return disembodied;

        }


        /*public static NPC RetrieveVoice(GameLocation location, Vector2 position)
        {

            if (Mod.instance.characters.ContainsKey("Disembodied"))
            {

                Character.Character disembodied = Mod.instance.characters["Disembodied"];

                GameLocation previous = disembodied.currentLocation;

                if (previous != null)
                {

                    if (previous != location)
                    {

                        previous.characters.Remove(disembodied);

                        location.characters.Add(disembodied);

                    }

                }
                else
                {
                    location.characters.Add(disembodied);

                }

            }
            else
            {

                Character.Character disembodied = new(position, location.Name, "Disembodied");

                //disembodied.frozenMode = true;

                disembodied.SwitchFrozenMode();

                disembodied.IsInvisible = true;

                disembodied.eventActor = true;

                //disembodied.forceUpdateTimer = 9999;

                disembodied.collidesWithOtherCharacters.Value = true;

                disembodied.farmerPassesThrough = true;

                location.characters.Add(disembodied);

                Mod.instance.characters["Disembodied"] = disembodied;

            }

            return Mod.instance.characters["Disembodied"];

        }
        */

    }

    public class CharacterDisposition
    {
        public int Age;
        public int Manners;
        public int SocialAnxiety;
        public int Optimism;
        public int Gender;
        public bool datable;
        public string Birthday_Season;
        public int Birthday_Day;
        public int id;
        public int speed;
    }

}
