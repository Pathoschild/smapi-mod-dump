/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

namespace Ink.Runtime
{
    public class VariableReference : Runtime.Object
    {
        // Normal named variable
        public string name { get; set; }

        // Variable reference is actually a path for a visit (read) count
        public Path pathForCount { get; set; }

        public Container containerForCount {
            get {
                return this.ResolvePath (pathForCount).container;
            }
        }
            
        public string pathStringForCount { 
            get {
                if( pathForCount == null )
                    return null;

                return CompactPathString(pathForCount);
            }
            set {
                if (value == null)
                    pathForCount = null;
                else
                    pathForCount = new Path (value);
            }
        }

        public VariableReference (string name)
        {
            this.name = name;
        }

        // Require default constructor for serialisation
        public VariableReference() {}

        public override string ToString ()
        {
            if (name != null) {
                return string.Format ("var({0})", name);
            } else {
                var pathStr = pathStringForCount;
                return string.Format("read_count({0})", pathStr);
            }
        }
    }
}

