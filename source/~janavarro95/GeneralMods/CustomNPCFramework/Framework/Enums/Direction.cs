using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomNPCFramework.Framework.Enums
{
    /// <summary>
    /// An enum to be used to signify directions.
    /// The enum order corresponds to the same order Stardew Valley uses for directions where
    /// Up=0
    /// Right=1
    /// Down=2
    /// Left=3
    /// </summary>
    public enum Direction
    {
        /// <summary>
        /// Used to signify something to face/move up.
        /// </summary>
        up,
        /// <summary>
        /// Used to signify something to face/move right.
        /// </summary>
        right,
        /// <summary>
        /// Used to signify something to face/move down.
        /// </summary>
        down,
        /// <summary>
        /// Used to signify something to face/move left.
        /// </summary>
        left
    }
}
