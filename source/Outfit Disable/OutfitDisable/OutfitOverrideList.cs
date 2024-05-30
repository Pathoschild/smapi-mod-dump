/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/idermailer/OutfitDisable
**
*************************************************/

namespace OutfitDisable
{
    public sealed class OutfitOverrideList
    {
        public bool Default { get; set; } = false;
        public Dictionary<string, bool> ExceptionsList { get; set; } = new Dictionary<string, bool>();
    }
}