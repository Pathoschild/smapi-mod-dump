using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.ModularNPCS.ModularRenderers
{
    /// <summary>
    /// A class used instead of an enum to hold keys for all of the different animations.
    /// </summary>
    public class AnimationKeys
    {
        /// <summary>
        /// The string that is used for the standing animation.
        /// </summary>
        public static string standingKey = "standing";
        /// <summary>
        /// The string that is used for the walking/moving animation.
        /// </summary>
        public static string walkingKey = "walking";
        /// <summary>
        /// The string that is used for the sitting animation.
        /// </summary>
        public static string sittingKey = "sitting";
        /// <summary>
        /// The string that is used for the swimming animation.
        /// </summary>
        public static string swimmingKey = "swimming";
    }
}
