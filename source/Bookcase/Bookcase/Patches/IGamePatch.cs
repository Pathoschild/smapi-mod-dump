using System.Reflection;
using System;

namespace Bookcase.Patches {

    /// <summary>
    /// This interface is used to mark a class as a loadable patch by Bookcase's patch loader.
    /// </summary>
    internal interface IGamePatch {

        /// <summary>
        /// Gets the type that this patch is targeting. You can just use typeof.
        /// </summary>
        /// <returns>The type this patch is attempting to modify.</returns>
        Type TargetType { get; }

        /// <summary>
        /// Gets the target method to patch. 
        /// </summary>
        /// <returns>The method to apply the patches to.</returns>
        MethodBase TargetMethod { get; }
    }
}