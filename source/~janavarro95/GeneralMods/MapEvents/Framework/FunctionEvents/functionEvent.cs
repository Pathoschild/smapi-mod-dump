using System.Collections.Generic;
using static EventSystem.Framework.Delegates;

namespace EventSystem.Framework
{
    /// <summary>Used to pair a function and a parameter list using the super object class to run virtually any function for map events.</summary>
    public class functionEvent
    {
        public paramFunction function;
        public List<object> parameters;

        /// <summary>Construct an instance.</summary>
        /// <param name="Function">The function to be called when running an event.</param>
        /// <param name="Parameters">The list of system.objects to be used in the function. Can include objects,strings, ints, etc. Anything can be passed in as a parameter or can be passed in as empty. Passing in null will just create an empty list.</param>
        public functionEvent(paramFunction Function, List<object> Parameters)
        {
            if (this.parameters == null) this.parameters = new List<object>();
            this.function = Function;
            this.parameters = Parameters;
        }

        /// <summary>Runs the function with the passed in parameters.</summary>
        public void run()
        {
            this.function.Invoke(this.parameters);
        }

        /// <summary>Simply swaps out the old parameters list for a new one.</summary>
        /// <param name="newParameters"></param>
        public void updateParameters(List<object> newParameters)
        {
            this.parameters = newParameters;
        }
    }
}
