using System;

namespace FunnySnek.AntiCheat.Server.Framework
{
    internal class PatchDescriptor
    {
        /*********
        ** Accessors
        *********/
        public Type TargetType;
        public string TargetMethodName;
        public Type[] TargetMethodArguments;


        /*********
        ** Public methods
        *********/
        /// <param name="targetType">Don't use typeof() or it won't work on other platforms</param>
        /// <param name="targetMethodName">Null if constructor is desired</param>
        /// <param name="targetMethodArguments">Null if no method abiguity</param>
        public PatchDescriptor(Type targetType, string targetMethodName, Type[] targetMethodArguments = null)
        {
            this.TargetType = targetType;
            this.TargetMethodName = targetMethodName;
            this.TargetMethodArguments = targetMethodArguments;
        }
    }
}
