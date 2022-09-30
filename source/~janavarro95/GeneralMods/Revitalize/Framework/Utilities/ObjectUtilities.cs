/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/janavarro95/Stardew_Valley_Mods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Omegasis.Revitalize.Framework.Constants;

namespace Omegasis.Revitalize.Framework.Utilities
{
    public class ObjectUtilities
    {
        /// <summary>
        /// Gets a Stardew Valley object from the predefined enum.
        /// </summary>
        /// <param name="sdvObject"></param>
        /// <param name="InitialStack"></param>
        /// <returns></returns>
        public static StardewValley.Object getStardewObjectFromEnum(Enums.SDVObject sdvObject, int InitialStack = 1)
        {
            return new StardewValley.Object((int)sdvObject, InitialStack);
        }

        public static bool IsObjectHoldingItem(StardewValley.Object obj)
        {
            if (obj.heldObject.Value != null) return true;
            else return false;
        }

        /// <summary>
        /// Checks to see if the given object is a SDV vanilla furnace.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool IsObjectFurnace(StardewValley.Object obj)
        {
            if (obj.ParentSheetIndex == 13 && obj.bigCraftable.Value && obj.Category == -9 && obj.Name == "Furnace")
                return true;
            else return false;
        }

        /// <summary>
        /// Gets a dimension offset depending on the size of the object passed in.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static Vector2 GetDimensionOffsetFromItem(StardewValley.Object obj)
        {
            if (TypeUtilities.IsSameType(typeof(StardewValley.Object), obj.GetType()))
                return new Vector2(64f, 64f);

            return new Vector2(64f, 64f);
        }

        /// <summary>
        /// Gets the height of an object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetHeightOffsetFromItem(StardewValley.Object obj)
        {
            return GetDimensionOffsetFromItem(obj).Y;
        }

        /// <summary>
        /// Gets the width of an item.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static float GetWidthOffsetFromItem(StardewValley.Object obj)
        {
            return GetDimensionOffsetFromItem(obj).X;
        }
    }
}
