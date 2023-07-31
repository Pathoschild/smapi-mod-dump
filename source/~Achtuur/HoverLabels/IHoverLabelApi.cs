/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Achtuur/StardewTravelSkill
**
*************************************************/

using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace HoverLabels;
public interface IHoverLabelApi
{
    /// <summary>
    /// Register <paramref name="label"/> so it is recognised by this mod and appears in game.
    /// </summary>
    /// <param name="mod">Your mod manifest id</param>
    /// <param name="name">Name of your label. This is the name shown in game in the config menu for example</param>
    /// <param name="label">your <see cref="IHoverLabel"/> class that you want to register</param>
    public void RegisterLabel(IManifest mod, string name, IHoverLabel label);

    /// <summary>
    /// Returns true when the 'Show details' button is pressed. This button can be set in the config and defaults to <see cref="SButton.LeftControl"/>
    /// </summary>
    /// <returns></returns>
    public bool IsShowDetailsButtonPressed();

    /// <summary>
    /// Returns true when the 'Alternative sort' button is pressed. This button can be set in the config and defaults to <see cref="SButton.LeftShift"/>
    /// </summary>
    /// <returns></returns>
    public bool IsAlternativeSortButtonPressed();

}
