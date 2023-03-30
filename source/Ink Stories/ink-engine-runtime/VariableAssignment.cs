/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/inkle/ink
**
*************************************************/

using System.ComponentModel;

namespace Ink.Runtime
{
    // The value to be assigned is popped off the evaluation stack, so no need to keep it here
    public class VariableAssignment : Runtime.Object
    {
        public string variableName { get; protected set; }
        public bool isNewDeclaration { get; protected set; }
        public bool isGlobal { get; set; }

        public VariableAssignment (string variableName, bool isNewDeclaration)
        {
            this.variableName = variableName;
            this.isNewDeclaration = isNewDeclaration;
        }

        // Require default constructor for serialisation
        public VariableAssignment() : this(null, false) {}

        public override string ToString ()
        {
            return "VarAssign to " + variableName;
        }
    }
}

