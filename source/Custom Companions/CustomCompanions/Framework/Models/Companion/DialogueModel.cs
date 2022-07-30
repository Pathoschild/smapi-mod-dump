/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/CustomCompanions
**
*************************************************/

namespace CustomCompanions.Framework.Models.Companion
{
    public class DialogueModel
    {
        public string Text { get; set; }
        public bool DisplayOnce { get; set; }
        internal bool HasBeenDisplayed { get; set; }
        public int PortraitIndex { get; set; } = -1;

        public override string ToString()
        {
            return $"[Text: {Text} | DisplayOnce: {DisplayOnce}]";
        }
    }
}
