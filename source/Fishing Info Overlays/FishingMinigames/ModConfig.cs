/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/barteke22/StardewMods
**
*************************************************/

using StardewModdingAPI.Utilities;

namespace FishingMinigames
{
    public class ModConfig
    {
        public int VoiceVolume { get; set; } = 100;
        public int[] VoicePitch { get; set; }
        public string[] Voice_Test_Ignore_Me { get; set; }//internal for voice setting change check
        public string[] KeyBinds { get; set; } = { "MouseLeft, Space, ControllerX", "MouseLeft, Space, ControllerX", "MouseLeft, Space, ControllerX", "MouseLeft, Space, ControllerX" };
        public int[] StartMinigameStyle { get; set; } = { 1, 1, 1, 1 };
        public int[] EndMinigameStyle { get; set; } = { 2, 2, 2, 2 };
        public float StartMinigameScale { get; set; } = 1f;
        public float[] EndMinigameDamage { get; set; } = { 1f, 1f, 1f, 1f };
        public float[] MinigameDifficulty { get; set; } = { 1f, 1f, 1f, 1f };
        public bool ConvertToMetric { get; set; } = false;
        public bool RealisticSizes { get; set; } = true;
        public int[] FestivalMode { get; set; } = { 2, 2, 2, 2 };


        public ModConfig()
        {
            VoicePitch = new int[4];
            Voice_Test_Ignore_Me = new string[4];
            for (int i = 0; i < 4; i++)
            {
                int rnd = StardewValley.Game1.random.Next(-70, 71);
                VoicePitch[i] = rnd;
                Voice_Test_Ignore_Me[i] = "100/" + rnd;
            }
        }
    }
}
