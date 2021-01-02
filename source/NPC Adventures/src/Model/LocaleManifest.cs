/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/purrplingcat/PurrplingMod
**
*************************************************/

using StardewModdingAPI;

namespace NpcAdventure.Model
{
    internal class LocaleManifest
    {
        public string Language { get; set; }
        public string Code { get; set; }
        public ISemanticVersion Version { get; set; }
        public string Translator { get; set; }
    }
}
