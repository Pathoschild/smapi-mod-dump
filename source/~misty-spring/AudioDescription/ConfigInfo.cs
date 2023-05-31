/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI.Events;
using StardewValley;
using System.Collections.Generic;
using System.Linq;
using StardewModdingAPI;

namespace AudioDescription
{
    internal static class ConfigInfo
    {
        internal static void SaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            ModEntry.MuteIcon = Game1.content.Load<Texture2D>("LooseSprites/mute_voice_icon");

            //ModEntry._lastTrack = Game1.currentSong.Name;

            ModEntry.AllowedCues?.Clear();

            if (ModEntry.Config.Environment)
            {
                ModEntry.AllowedCues?.AddRange(new List<string>
                {
                    "doorClose",
                    "cricketsAmbient",
                    "boulderCrack",
                    "dropItemInWater",
                    "explosion",
                    "crafting",
                    "stoneCrack",
                    "wind",
                    "SpringBirds",
                    "Ship",
                    "phone",
                    "thunder",
                    "crickets",
                    "cavedrip",
                    "treethud",
                    "treecrack",
                    "leafrustle",
                    "crystal",
                    "potterySmash",
                    "busDriveOff",
                    "Stadium_cheer",
                    "submarine_landing",
                    "thunder_small",
                    "trainWhistle",
                    "distantTrain",
                    "Meteorite",
                    "bubbles",
                    "boulderBreak",
                    "dirtyHit",
                    "newArtifact",
                    "secret1",
                    "jingle1",
                    "waterSlosh",
                    "robotSoundEffects",
                    "robotBLASTOFF",
                    "slosh",
                    "cameraNoise",
                    "mouseClick",
                    "whistle",
                    "barrelBreak",
                    "cracklingFire"
                });
            }

            if (ModEntry.Config.NPCs)
            {
                ModEntry.AllowedCues?.AddRange(new List<string>
                {
                    "ghost",
                    "cluck",
                    "Duggy",
                    "rabbit",
                    "goat",
                    "cow",
                    "pig",
                    "croak",
                    "batScreech",
                    "seagulls",
                    "shadowDie",
                    "owl",
                    "dogs",
                    "Duck",
                    "sheep",
                    "killAnimal",
                    "junimoMeep1",
                    "dogWhining",
                    "crow",
                    "rooster",
                    "dog_pant",
                    "dog_bark",
                    "cat",
                    "parrot",
                    "fireball",
                    "flameSpellHit",
                    "flameSpell",
                    "monsterdead",
                    "rockGolemSpawn",
                    "shadowpeep",
                    "batFlap",
                    "dustMeep",
                    "serpentDie",
                    "cacklingWitch"
                });
            }

            if (ModEntry.Config.FishingCatch)
            {
                ModEntry.AllowedCues?.AddRange(new List<string>
                {
                    "fishBite",
                    "FishHit",
                    "fishEscape",
                    "fishSlap"
                });
            }

            if (ModEntry.Config.ItemSounds)
            {
                ModEntry.AllowedCues?.AddRange(new List<string>
                {
                    "cut",
                    "axe",
                    "wateringCan",
                    "openChest",
                    "parry",
                    "clank",
                    "toyPiano",
                    "trashcan",
                    "trashcanlid",
                    "scissors",
                    "Milking",
                    "breakingGlass",
                    "glug",
                    "doorCreakReverse",
                    "openBox",
                    "axchop",
                    "seeds",
                    //"detector",
                    "crit"
                });
            }

            if (ModEntry.Config.PlayerSounds)
            {
                ModEntry.AllowedCues?.AddRange(new List<string>
                {
                    "eat",
                    "gulp",
                    "powerup",
                    "toolCharge",
                    "sipTea",
                    "slingshot",
                    "woodWhack",
                    "stairsdown",
                    "fallDown",
                    "doorCreak",
                    "doorOpen",
                    "pickUpItem",
                    "furnace",
                    "discoverMineral"
                });
            }

            if (ModEntry.Config.Minigames)
            {
                ModEntry.AllowedCues?.AddRange(new List<string>
                {
                    "Cowboy_Secret",
                    "Cowboy_monsterDie",
                    "Cowboy_gunshot",
                    "cowboy_dead",
                    "Cowboy_Footstep",
                    "Cowboy_undead",
                    "cowboy_powerup",
                    "cowboy_gunload",
                    "Pickup_Coin15",
                    "cowboy_monsterhit",
                    "cowboy_gopher",
                    "cowboy_explosion"

                });
            }

            if (string.IsNullOrWhiteSpace(ModEntry.Config.Blacklist)) return;
            
            var cleanlist = ParseBlackList();
            foreach (var sound in cleanlist)
            {
                ModEntry.AllowedCues?.Remove(sound);
            }
        }

        private static List<string> ParseBlackList()
        {
            ModEntry.Mon.Log("Getting raw blacklist.");
            var blacklistRaw = ModEntry.Config.Blacklist;
            if (blacklistRaw is null)
            {
                ModEntry.Mon.Log("No characters in blacklist.");
            }

            var charsToRemove = new string[] { "-", ",", ".", ";", "\"", "\'", "/" };
            foreach (var c in charsToRemove)
            {
                blacklistRaw = blacklistRaw?.Replace(c," "); //string.Empty erased them. this ensures they still have a separator
            }
            ModEntry.Mon.Log($"Raw blacklist: \n {blacklistRaw} \nWill be parsed to list now.", LogLevel.Debug);
            var blacklistParsed = blacklistRaw?.Split(' ').ToList();

            return blacklistParsed;
        }
    }
}