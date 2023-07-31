/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels;
public interface IHoverLabel
{
    /// <summary>
    /// Returns priority of this label, labels with a higher priority will appear over labels with a lower one. 
    /// If you have multiple labels with similar conditions, this can be useful.
    /// For example, if you have a label for trees and a label for maple trees, it is useful to set the priority of the maple tree label higher
    /// as every maple tree is a tree, but not every tree is a maple tree.
    /// 
    /// If you do not care about priority, set priority to 0.
    /// </summary>
    /// <returns></returns>
    public int Priority {get; set;}

    /// <summary>
    /// Gets the name of the label. This is the part that appears in the top part of the label in a larger font.
    /// </summary>
    /// <returns></returns>
    public string GetName();

    /// <summary>
    /// Gets the description part of the label. This is the bottom part of the label and has a smaller font.
    /// 
    /// <remark>
    ///     Every item in the list is separated by a newline. 
    ///     If you want no description, return an empty IEnumerable. In that case, only the name is shown.
    /// </remark>
    /// </summary>
    /// <returns></returns>
    public IEnumerable<string> GetDescription();

    /// <summary>
    /// Returns whether a label should be generated based on <paramref name="cursorTile"/>.
    /// </summary>
    /// <param name="cursorTile">The in-game tile the cursor is on. It is usually obtained by using <see cref="Game1.currentCursorTile"/></param>
    /// <returns></returns>
    public bool ShouldGenerateLabel(Vector2 cursorTile);

    /// <summary>
    /// The <see cref="IHoverLabel"/> instance is expected to generate a name (and description) based on the cursorTile inside this function.
    /// This function is only called after <see cref="ShouldGenerateLabel(Vector2)"/> returns true.
    /// </summary>
    /// <param name="cursorTile"></param>
    public void UpdateCursorTile(Vector2 cursorTile);


    /// <summary>
    /// Draw something on screen when your label is currently active. Examples of this are the sprinkler and scarecrow range showing when hovering over them.
    /// </summary>
    /// <param name="spriteBatch">the spritebatch</param>
    public void DrawOnOverlay(SpriteBatch spriteBatch);

}
