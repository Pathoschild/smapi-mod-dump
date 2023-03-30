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
    public class Tag : Parsed.Object
    {

        public bool isStart;
        public bool inChoice;
        
        public override Runtime.Object GenerateRuntimeObject ()
        {
            if( isStart )
                return Runtime.ControlCommand.BeginTag();
            else
                return Runtime.ControlCommand.EndTag();
        }

        public override string ToString ()
        {
            if( isStart )
                return "#StartTag";
            else
                return "#EndTag";
        }
    }
}

