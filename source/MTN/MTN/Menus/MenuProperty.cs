using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN.Menus
{
    /// <summary>
    /// Needed for CharacterCustomizationWithCustom Menu. This simply retains information used to recalibrate the Farm Type Buttons
    /// when a player scrolls through the list.
    /// </summary>
    public class MenuProperty
    {
        public int yOffSet;
        public int ID;
        public int downNeighborID;
        public int upNeightborID;
        public int leftNeighborID;
        public int rightNeighborID;

        public MenuProperty() { }

        public MenuProperty(int yOffSet, int ID, int downNeighborID, int upNeightborID, int leftNeighborID, int rightNeighborID)
        {
            this.yOffSet = yOffSet;
            this.ID = ID;
            this.downNeighborID = downNeighborID;
            this.upNeightborID = upNeightborID;
            this.leftNeighborID = leftNeighborID;
            this.rightNeighborID = rightNeighborID;
        }
    }
}
