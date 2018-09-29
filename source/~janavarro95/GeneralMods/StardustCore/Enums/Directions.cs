using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardustCore.Enums
{
    /// <summary>
    /// A custom class used to wrap Stardew Valley's direction conventions.
    /// </summary>
    public class Directions
    {
        /// <summary>
        /// An enum used to wrap directions.
        /// </summary>
        public enum Direction
        {
            [Description("Up")]
            up,
            [Description("Right")]
            right,
            [Description("Down")]
            down,
            [Description("Left")]
            left
        }

        /// <summary>
        /// Get the up direction.
        /// </summary>
        /// <returns></returns>
        public static Direction GetUp()
        {
            return Direction.up;
        }
        
        /// <summary>
        /// Get the down direction.
        /// </summary>
        /// <returns></returns>
        public static Direction GetDown()
        {
            return Direction.down;
        }

        /// <summary>
        /// Get the left direction.
        /// </summary>
        /// <returns></returns>
        public static Direction GetLeft()
        {
            return Direction.left;
        }

        /// <summary>
        /// Get the right dirction.
        /// </summary>
        /// <returns></returns>
        public static Direction GetRight()
        {
            return Direction.right;
        }

        /// <summary>
        /// Converts a direction into it's stardew valley direction equivelent.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static int DirectionToInt(Direction dir)
        {
            return (int)dir;
        }

        /// <summary>
        /// Get's the direction's enum description so we can convert this to a string.
        /// </summary>
        /// <param name="dir"></param>
        /// <returns></returns>
        public static string getString(Direction dir)
        {
            return dir.GetEnumDescription();
        }
    }
}
