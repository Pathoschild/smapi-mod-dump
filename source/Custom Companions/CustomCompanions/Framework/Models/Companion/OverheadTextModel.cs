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
    public class OverheadTextModel
    {
        public string Text { get; set; }
        public float ChanceWeight { get; set; } = 1f;
        public int TextLifetime { get; set; } = 3000;

        public override string ToString()
        {
            return $"[Text: {Text} | ChanceWeight: {ChanceWeight}]";
        }
    }
}
