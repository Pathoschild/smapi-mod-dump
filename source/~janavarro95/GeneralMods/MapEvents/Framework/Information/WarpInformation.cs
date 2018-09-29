using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventSystem.Framework.Information
{
    /// <summary>
    /// Used to store all of the information necessary to warp the farmer.
    /// </summary>
    public class WarpInformation
    {
        public string targetMapName;
        public int targetX;
        public int targetY;
        public int facingDirection;
        public bool isStructure;

        /// <summary>
        /// Constructor used to bundle together necessary information to warp the player character.
        /// </summary>
        /// <param name="MapName">The target map name to warp the farmer to.</param>
        /// <param name="TargetX">The target X location on the map to warp the farmer to.</param>
        /// <param name="TargetY">The target Y location on the map to warp the farmer to.</param>
        /// <param name="FacingDirection">The facing direction for the farmer to be facing after the warp.</param>
        /// <param name="IsStructure">Used to determine the position to be warped to when leaving a structure.</param>
        public WarpInformation(string MapName, int TargetX,int TargetY, int FacingDirection, bool IsStructure)
        {
            this.targetMapName = MapName;
            this.targetX = TargetX;
            this.targetY = TargetY;
            this.facingDirection = FacingDirection;
            this.isStructure = IsStructure;
        }
    }
}
