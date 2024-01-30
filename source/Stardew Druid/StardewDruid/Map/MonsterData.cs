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
using StardewDruid.Monster;
using StardewDruid.Monster.Boss;
using StardewDruid.Monster.Template;
using StardewModdingAPI;
using StardewValley;
using System.Collections.Generic;
using System.IO;

namespace StardewDruid.Map
{
    static class MonsterData
    {
        public static StardewValley.Monsters.Monster CreateMonster(int spawnMob, Vector2 spawnVector, int combatModifier = -1)
        {

            if(combatModifier == -1)
            {

                combatModifier = Mod.instance.CombatModifier();

            }

            StardewValley.Monsters.Monster theMonster;

            /*
              
             Medium
                Start + combat 2
                (2+1)^2 * 7 = 63
                Water + combat 5
                (2+2)^2 * 10 = 160
                Stars + combat 8
                (2+3)^2 * 13 = 325
             
             Hard
                Start + combat 2
                (3+1)^2 * 7 = 112
                Water + combat 5
                (3+2)^2 * 10 = 250
                Stars + combat 8
                (3+3)^2 * 13 = 468          

             */

            switch (spawnMob)
            {

                default: // Bat

                    theMonster = new Monster.Template.Bat(spawnVector, combatModifier);

                    break;

                case 0: // Green Slime

                    theMonster = new Slime(spawnVector, combatModifier);

                    break;

                case 1: // Shadow Brute

                    theMonster = new Shadow(spawnVector, combatModifier);

                    break;

                case 2: // Skeleton

                    theMonster = new Monster.Template.Skeleton(spawnVector, combatModifier);

                    break;

                case 3: // Golem

                    theMonster = new Golem(spawnVector, combatModifier);

                    break;

                case 4: // DustSpirit

                    theMonster = new Spirit(spawnVector, combatModifier);

                    break;



                // ------------------ Bosses

                case 11:

                    theMonster = new BigBat(spawnVector, combatModifier);

                    break;

                case 12:

                    theMonster = new Shooter(spawnVector, combatModifier);

                    break;

                case 13:

                    theMonster = new BigSlime(spawnVector, combatModifier);

                    break;

                case 14:

                    theMonster = new Dino(spawnVector, combatModifier);

                    break;

                case 16:

                    theMonster = new Dragon(spawnVector, combatModifier);

                    break;

                case 17:

                    theMonster = new Reaper(spawnVector, combatModifier);

                    break;

                case 18:

                    theMonster = new Shadowtin(spawnVector, combatModifier);

                    break;

                case 19:

                    theMonster = new Scavenger(spawnVector, combatModifier);

                    break;

                case 20:

                    theMonster = new Rogue(spawnVector, combatModifier);

                    break;

                case 21:

                    theMonster = new Prime(spawnVector, combatModifier);

                    break;

                // ------------------ Firebird

                case 41:

                    theMonster = new Firebird(spawnVector, combatModifier, "EmeraldFirebird");

                    break;
                case 42:

                    theMonster = new Firebird(spawnVector, combatModifier, "AquamarineFirebird");

                    break;
                case 43:

                    theMonster = new Firebird(spawnVector, combatModifier, "RubyFirebird");

                    break;
                case 44:

                    theMonster = new Firebird(spawnVector, combatModifier, "AmethystFirebird");

                    break;
                case 45:

                    theMonster = new Firebird(spawnVector, combatModifier, "TopazFirebird");

                    break;
                // ------------------ Solaris

                case 51:

                    theMonster = new Solaris(spawnVector, combatModifier, "SolarisZero");

                    break;
                case 52:

                    theMonster = new Solaris(spawnVector, combatModifier, "VoidleZero");

                    break;
                case 53:

                    theMonster = new Solaris(spawnVector, combatModifier, "Solaris");

                    break;
                case 54:

                    theMonster = new Solaris(spawnVector, combatModifier, "Voidle");

                    break;
                case 55:

                    theMonster = new Solaris(spawnVector, combatModifier, "SolarisPrime");

                    break;
                case 56:

                    theMonster = new Solaris(spawnVector, combatModifier, "VoidlePrime");

                    break;

            }

            return theMonster;

        }

        public static List<System.Type> CustomMonsters()
        {
            List<System.Type> customMonsters = new()
            {
                typeof(BigBat),
                typeof (Dino),
                typeof (Dragon),
                typeof (Reaper),
                typeof (Scavenger),
                typeof (Rogue),
                typeof (Prime),
                typeof(Shooter),
                typeof(BigSlime),
                typeof(Firebird),
                typeof(Monster.Boss.Solaris),
            };

            return customMonsters;

        }

        public static Texture2D MonsterTexture(string characterName)
        {

            if (characterName == "Dinosaur")
            {

                return Game1.content.Load<Texture2D>("Characters\\Monsters\\Pepper Rex");

            }

            if (characterName.Contains("Firebird"))
            {

                return Game1.content.Load<Texture2D>("LooseSprites\\GemBird");

            }

            Texture2D characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + ".png"));

            return characterTexture;

        }

        public static StardewValley.AnimatedSprite MonsterSprite(string characterName)
        {

            StardewValley.AnimatedSprite characterSprite;

            characterSprite = new();

            characterSprite.textureName.Value = "18465_" + characterName;

            characterSprite.spriteTexture = MonsterTexture(characterName);

            characterSprite.loadedTexture = "18465_" + characterName;

            if (characterName.Contains("Zero"))
            {

                characterSprite.SpriteHeight = 16;

                characterSprite.SpriteWidth = 16;

            }
            else if (characterName.Contains("Dragon"))
            {

                characterSprite.SpriteHeight = 64;

                characterSprite.SpriteWidth = 64;

            }
            else if (characterName.Contains("Reaper"))
            {
                characterSprite.SpriteHeight = 48;

                characterSprite.SpriteWidth = 64;
            }
            else
            {
                characterSprite.SpriteHeight = 32;

                characterSprite.SpriteWidth = 32;

            }

            characterSprite.UpdateSourceRect();

            return characterSprite;

        }

    }

}
