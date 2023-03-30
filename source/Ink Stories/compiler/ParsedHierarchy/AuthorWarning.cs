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
    public class AuthorWarning : Parsed.Object
    {
        public string warningMessage;

        public AuthorWarning(string message)
        {
            warningMessage = message;
        }

        public override Runtime.Object GenerateRuntimeObject ()
        {
            Warning (warningMessage);
            return null;
        }
    }
}

