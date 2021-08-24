/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aedenthorn/StardewValleyMods
**
*************************************************/

using StardewModdingAPI;

namespace ProceduralDialogue
{
    public class ModConfig
    {
        public bool Enabled { get; set; } = true;
        public SButton ModButton { get; set; } = SButton.LeftAlt;
        public SButton AskButton { get; set; } = SButton.MouseLeft;
        public SButton AnswerButton { get; set; } = SButton.MouseRight;
        public int MaxPlayerQuestions { get; set; } = 4;
    }
}
