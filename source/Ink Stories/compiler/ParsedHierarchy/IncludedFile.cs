/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/


namespace Ink.Parsed
{
    public class IncludedFile : Parsed.Object
    {
        public Parsed.Story includedStory { get; private set; }

        public IncludedFile (Parsed.Story includedStory)
        {
            this.includedStory = includedStory;
        }

        public override Runtime.Object GenerateRuntimeObject ()
        {
            // Left to the main story to process
            return null;
        }
    }
}

