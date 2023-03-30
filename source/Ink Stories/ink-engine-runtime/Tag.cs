/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

using System;

namespace Ink.Runtime
{
    // New version of tags is dynamic - it constructs the tags
    // at runtime based on BeginTag and EndTag control commands.
    // Plain text that's in the output stream is turned into tags
    // when you do story.currentTags.
    // The only place this is used is when flattening tags down
    // to string in advance, during dynamic string generation if
    // there's a tag embedded in it. See how ControlCommand.EndString
    // is implemented in Story.cs for more details + comment
    public class Tag : Runtime.Object {
        public string text { get; private set; }

        public Tag (string tagText)
        {
            this.text = tagText;
        }

        public override string ToString ()
        {
            return "# " + text;
        }
    }
}

