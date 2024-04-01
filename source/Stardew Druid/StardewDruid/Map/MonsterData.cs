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
        public static StardewValley.Monsters.Monster CreateMonster(int spawnMob, Vector2 spawnVector, int combatModifier = -1, bool champion = false)
        {

            if(combatModifier == -1)
            {

                combatModifier = Mod.instance.CombatModifier();

            }

            StardewValley.Monsters.Monster theMonster;

            switch (spawnMob)
            {
                
                default:
                case 0: // Green Slime

                    theMonster = new Slime(spawnVector, combatModifier, champion);

                    break;

                case 1: // Shadow Brute

                    theMonster = new Shadow(spawnVector, combatModifier, champion);


                    break;

                case 2: // Skeleton

                    theMonster = new Skeleton(spawnVector, combatModifier, champion);

                    break;

                case 3: // Golem

                    theMonster = new Golem(spawnVector, combatModifier, champion);

                    break;

                case 4: // DustSpirit

                    theMonster = new Spirit(spawnVector, combatModifier, champion);

                    break;

                case 5: // Bat

                    theMonster = new Bat(spawnVector, combatModifier, champion);

                    break;

                case 6:

                    theMonster = new Gargoyle(spawnVector, combatModifier);

                    if (champion)
                    {

                        (theMonster as Gargoyle).ChampionMode();

                    }

                    break;

            }

            return theMonster;

        }

        public static bool BossMonster(StardewValley.Monsters.Monster monster)
        {

            if(monster is StardewDruid.Monster.Boss.Boss)
            {

                return true;

            }

            List<System.Type> customMonsters = new()
            {
                typeof(BigBat),
                typeof(Shooter),
                typeof(BigSlime),

            };

            if (customMonsters.Contains(monster.GetType()))
            {

                return true;

            }

            return false;

        }

        public static Texture2D MonsterTexture(string characterName)
        {

            if (characterName == "Dinosaur")
            {

                return Game1.content.Load<Texture2D>("Characters\\Monsters\\Pepper Rex");

            }

            Texture2D characterTexture = Mod.instance.Helper.ModContent.Load<Texture2D>(Path.Combine("Images", characterName + ".png"));

            return characterTexture;

        }

        public static StardewValley.AnimatedSprite MonsterSprite(string characterName)
        {

            StardewValley.AnimatedSprite characterSprite;

            characterSprite = new();

            characterSprite.textureName.Set("18465_" + characterName);

            characterSprite.spriteTexture = MonsterTexture(characterName);

            characterSprite.loadedTexture = "18465_" + characterName;

            if (characterName.Contains("Dragon"))
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
