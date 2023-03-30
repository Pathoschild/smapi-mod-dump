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
    public class Wrap<T> : Parsed.Object where T : Runtime.Object
    {
        public Wrap (T objToWrap)
        {
            _objToWrap = objToWrap;
        }

        public override Runtime.Object GenerateRuntimeObject ()
        {
            return _objToWrap;
        }

        T _objToWrap;
    }

    // Shorthand for writing Parsed.Wrap<Runtime.Glue> and Parsed.Wrap<Runtime.Tag>
    public class Glue : Wrap<Runtime.Glue> {
        public Glue (Runtime.Glue glue) : base(glue) {}
    }
    public class LegacyTag : Wrap<Runtime.Tag> {
        public LegacyTag (Runtime.Tag tag) : base (tag) { }
    }
    
}

