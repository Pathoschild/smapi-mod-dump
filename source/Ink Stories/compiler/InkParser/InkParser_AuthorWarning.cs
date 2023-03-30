/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

using Ink.Parsed;

namespace Ink
{
    public partial class InkParser
    {
        protected AuthorWarning AuthorWarning()
        {
            Whitespace ();

            var identifier = Parse (IdentifierWithMetadata);
            if (identifier == null || identifier.name != "TODO")
                return null;

            Whitespace ();

            ParseString (":");

            Whitespace ();

            var message = ParseUntilCharactersFromString ("\n\r");

            return new AuthorWarning (message);
        }

    }
}

