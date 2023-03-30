/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

namespace Ink.Parsed {
    public class Identifier {
        public string name;
        public Runtime.DebugMetadata debugMetadata;

        public override string ToString()
        {
            return name;
        }

        public static Identifier Done = new Identifier { name = "DONE", debugMetadata = null };
    }
}
