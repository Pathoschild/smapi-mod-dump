/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Esca-MMC/FarmTypeManager
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Monsters;

namespace FarmTypeManager.Monsters
{
    /// <summary>A subclass of Stardew's Mummy class, adjusted for use by this mod.</summary>
    public class MummyFTM : Mummy
    {
        private bool seesPlayersAtSpawn = false;

        /// <summary>Creates an instance of Stardew's Mummy class, but with adjustments made for this mod.</summary>
        public MummyFTM()
            : base()
        {

        }

        /// <summary>Creates an instance of Stardew's Mummy class, but with adjustments made for this mod.</summary>
        /// <param name="position">The x,y coordinates of this monster's location.</param>
        public MummyFTM(Vector2 position)
            : base(position)
        {

        }

        //this override fixes the following BigSlime behavioral bugs:
        // * mummies continuing to move in their "crumbled" state when the "SeesPlayersAtSpawn" setting is enabled
        public override void behaviorAtGameTick(GameTime time)
        {
            if (focusedOnFarmers) //if the monster is focused on farmers (via this mod's customization settings)
            {
                focusedOnFarmers = false; //undo the setting
                seesPlayersAtSpawn = true; //record it locally
            }

            base.behaviorAtGameTick(time);

            if (seesPlayersAtSpawn == true && moveTowardPlayerThreshold.Value > 0) //if "sees players at spawn" is set, and the mummy is currently able to "see" players
            {
                moveTowardPlayerThreshold.Value = 999; //maximize the mummy's sight range
            }
        }
    }
}
