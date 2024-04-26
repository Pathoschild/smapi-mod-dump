/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/agilbert1412/StardewArchipelago
**
*************************************************/

using System.Collections.Generic;

namespace StardewArchipelago.Constants.Modded
{
    public class ModVersions
    {
        public static readonly Dictionary<string, string> Versions = new Dictionary<string, string>()
        {
            { ModNames.ALEC, "2.1.0"},
            { ModNames.ARCHAEOLOGY, "1.6.0"},
            { ModNames.AYEISHA, "0.6.2-alpha"},
            { ModNames.BIGGER_BACKPACK, "6.0.0"},
            { ModNames.BINNING, "1.2.7"},
            { ModNames.COOKING, "1.4.5"},
            { ModNames.DEEP_WOODS, "3.1.0-beta"},
            { ModNames.DELORES, "1.1.2"},
            { ModNames.EUGENE, "1.3.1"},
            { ModNames.JASPER, "1.7.8"},
            { ModNames.JUNA, "2.1.4"},
            { ModNames.LUCK, "1.2.4"},
            { ModNames.MAGIC, "0.8.2"},
            { ModNames.MISTER_GINGER, "1.5.9"},
            { ModNames.RILEY, "1.2.2"},
            { ModNames.SHIKO, "1.2.0"},
            { ModNames.SKULL_CAVERN_ELEVATOR, "1.5.0"},
            { ModNames.SOCIALIZING, "1.1.5"},
            { ModNames.TRACTOR, "4.16.6"},
            { ModNames.WELLWICK, "1.0.0"},
            { ModNames.YOBA, "1.0.0"},
            { ModNames.SVE, "1.14.24"},
            { ModNames.ALECTO, "1.1.7"},
            { ModNames.DISTANT_LANDS, "1.0.8"},
            { ModNames.LACEY, "1.1.2"},
            { ModNames.BOARDING_HOUSE, "4.0.16"}
        };

        public class ContentPatcherRequirement{
            public string ContentPatcherMod {get; private set;}
            public string ContentPatcherVersion {get; private set;}

            public ContentPatcherRequirement(string patchName, string patchVersion)
            {
                ContentPatcherMod = patchName;
                ContentPatcherVersion = patchVersion;
            }
        }

        public static readonly Dictionary<string, ContentPatcherRequirement> CPVersions = new(){
            {ModNames.SVE, new ContentPatcherRequirement(ModNames.AP_SVE, "1.1.0")},
            {ModNames.JASPER, new ContentPatcherRequirement(ModNames.AP_JASPER, "1.0.0")}
        };
}
}
