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
    public class Return : Parsed.Object
    {
        public Expression returnedExpression { get; protected set; }

        public Return (Expression returnedExpression = null)
        {            
            if (returnedExpression) {
                this.returnedExpression = AddContent(returnedExpression);
            }
        }

        public override Runtime.Object GenerateRuntimeObject ()
        {
            var container = new Runtime.Container ();

            // Evaluate expression
            if (returnedExpression) {
                container.AddContent (returnedExpression.runtimeObject);
            } 

            // Return Runtime.Void when there's no expression to evaluate
            // (This evaluation will just add the Void object to the evaluation stack)
            else {
                container.AddContent (Runtime.ControlCommand.EvalStart ());
                container.AddContent (new Runtime.Void());
                container.AddContent (Runtime.ControlCommand.EvalEnd ());
            }

            // Then pop the call stack
            // (the evaluated expression will leave the return value on the evaluation stack)
            container.AddContent (Runtime.ControlCommand.PopFunction());

            return container;
        }
    }
}

