/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace FishingMinigames
{
    public class ModConfig
    {
        public int VoiceVolume { get; set; } = 100;
        public string[] VoiceType { get; set; } = { "TTS Brian", "TTS Brian", "TTS Brian", "TTS Brian" };
        public int[] VoicePitch { get; set; }
        public string[] Voice_Test_Ignore_Me { get; set; }//internal for voice setting change check
        public string[] KeyBinds { get; set; } = { "MouseLeft, C, ControllerX", "MouseLeft, C, ControllerX", "MouseLeft, C, ControllerX", "MouseLeft, C, ControllerX" };
        public bool[] FreeAim { get; set; } = { false, false, false, false };
        public int[] StartMinigameStyle { get; set; } = { 1, 1, 1, 1 };
        public int[] EndMinigameStyle { get; set; } = { 2, 2, 2, 2 };
        public float StartMinigameScale { get; set; } = 1f;
        public bool[] EndLoseTreasureIfFailed { get; set; } = { true, true, true, true };
        public float[] EndMinigameDamage { get; set; } = { 1f, 1f, 1f, 1f };
        public float[] MinigameDifficulty { get; set; } = { 1f, 1f, 1f, 1f };
        public bool ConvertToMetric { get; set; } = false;
        public bool RealisticSizes { get; set; } = true;
        public bool FishTankHoldSprites { get; set; } = true;
        public int[] FestivalMode { get; set; } = { 3, 3, 3, 3 };
        public bool[] TutorialSkip { get; set; } = { false, false, false, false };
        public Color MinigameColor { get; set; } = Color.Cyan;
        public bool BossTransparency { get; set; } = true;

        public Dictionary<string, Dictionary<string, int>> SeeInfoForBelowData { get; set; } = new Dictionary<string, Dictionary<string, int>>()
        {
            //rods
            {"Training Rod", new Dictionary<string, int>(){     { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 50 }, { "TREASURE", 0 } }},
            {"Bamboo Pole", new Dictionary<string, int>(){      { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 0 }, { "TREASURE", 0 } }},
            {"Fiberglass Rod", new Dictionary<string, int>(){   { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 20 }, { "TREASURE", 0 } }},
            {"Iridium Rod", new Dictionary<string, int>(){      { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 40 }, { "TREASURE", 0 } }},

            //baits
            {"Bait", new Dictionary<string, int>(){             { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "QUALITY", 0 }, { "SIZE", 10 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Magnet", new Dictionary<string, int>(){           { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "QUALITY", 0 }, { "SIZE", -10 }, { "SPEED", 0 }, { "TREASURE", 15 }, { "UNBREAKING", 0 } }},
            {"Wild Bait", new Dictionary<string, int>(){        { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 2 }, { "EXTRA_CHANCE", 20 }, { "QUALITY", 0 }, { "SIZE", 20 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Magic Bait", new Dictionary<string, int>(){       { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 30 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},

            //tackle
            {"Spinner", new Dictionary<string, int>(){          { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 3 }, { "QUALITY", 0 }, { "SIZE", 10 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Dressed Spinner", new Dictionary<string, int>(){  { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 10 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 7 }, { "QUALITY", 0 }, { "SIZE", 20 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Trap Bobber", new Dictionary<string, int>(){      { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 1 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 20 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Cork Bobber", new Dictionary<string, int>(){      { "AREA", 40 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 10 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Lead Bobber", new Dictionary<string, int>(){      { "AREA", 0 }, { "DAMAGE", 50 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Treasure Hunter", new Dictionary<string, int>(){  { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 0 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 0 }, { "TREASURE", 5 }, { "UNBREAKING", 0 } }},
            {"Barbed Hook", new Dictionary<string, int>(){      { "AREA", 10 }, { "DAMAGE", 0 }, { "DIFFICULTY", 20 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 2 }, { "QUALITY", 0 }, { "SIZE", 0 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
            {"Curiosity Lure", new Dictionary<string, int>(){   { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 0 }, { "QUALITY", 0 }, { "SIZE", 10 }, { "SPEED", 0 }, { "TREASURE", 1 }, { "UNBREAKING", 0 } }},
            {"Quality Bobber", new Dictionary<string, int>(){   { "AREA", 0 }, { "DAMAGE", 0 }, { "DIFFICULTY", 0 }, { "EXTRA_MAX", 0 }, { "EXTRA_CHANCE", 0 }, { "LIFE", 0 }, { "QUALITY", 1 }, { "SIZE", 20 }, { "SPEED", 0 }, { "TREASURE", 0 }, { "UNBREAKING", 0 } }},
        };


        public ModConfig()
        {
            VoicePitch = new int[4];
            Voice_Test_Ignore_Me = new string[4];
            for (int i = 0; i < 4; i++)
            {
                int rnd = StardewValley.Game1.random.Next(-70, 71);
                VoicePitch[i] = rnd;
                Voice_Test_Ignore_Me[i] = "100/TTS Brian/" + rnd;
            }
        }
    }
}
