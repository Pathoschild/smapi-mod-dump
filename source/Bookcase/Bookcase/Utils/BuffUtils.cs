/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Stardew-Valley-Modding/Bookcase
**
*************************************************/

using StardewValley;
using System.Reflection;
using StardewValley.Menus;
using System.Collections.Generic;


namespace Bookcase.Utils {

    public class BuffUtils {

        /// <summary>
        /// Reflection field info for Buff.buffAttributes.
        /// </summary>
        private static FieldInfo buffAttributes = typeof(Buff).GetField("buffAttributes", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Reflection field info for BuffsDisplay.buffs.
        /// </summary>
        private static FieldInfo displayBuffs = typeof(BuffsDisplay).GetField("buffs", BindingFlags.NonPublic | BindingFlags.Instance);

        /// <summary>
        /// Creates a copy of a buff as a new object. 
        /// </summary>
        /// <param name="toCopy">The buff to make a copy of.</param>
        /// <returns>The copied buff.</returns>
        public static Buff CopyBuff(Buff toCopy) {

            if (toCopy == null) {

                return null;
            }

            Buff copy = new Buff(toCopy.description, toCopy.millisecondsDuration, toCopy.source, toCopy.sheetIndex);

            SetBuffAttributes(copy, GetBuffAttributes(toCopy));
            copy.which = toCopy.which;
            copy.displaySource = toCopy.displaySource;
            copy.total = toCopy.total;
            copy.glow = toCopy.glow;

            return copy;
        }

        /// <summary>
        /// Gets the attributes of a buff.
        /// </summary>
        /// <param name="buff">The attributes of the buff.</param>
        /// <returns></returns>
        public static int[] GetBuffAttributes(Buff buff) {

            return (int[])buffAttributes.GetValue(buff);
        }

        /// <summary>
        /// Sets the buff attributes to a new array.
        /// </summary>
        /// <param name="buff">The buff to modify.</param>
        /// <param name="attributes">The new buff attributes.</param>
        public static void SetBuffAttributes(Buff buff, int[] attributes) {

            buffAttributes.SetValue(buff, attributes);
        }

        /// <summary>
        /// Gets the buff icons.
        /// </summary>
        /// <returns>A dictionary of buff icons.</returns>
        public static Dictionary<ClickableTextureComponent, Buff> GetBuffIcons() {

            return GetBuffIcons(Game1.buffsDisplay);
        }

        /// <summary>
        /// Gets the buff icons.
        /// </summary>
        /// <param name="display">The BuffsDisplay to pull from.</param>
        /// <returns>A dictionary of buff icons.</returns>
        public static Dictionary<ClickableTextureComponent, Buff> GetBuffIcons(BuffsDisplay display) {

            return (Dictionary<ClickableTextureComponent, Buff>)displayBuffs.GetValue(display);
        }
    }
}