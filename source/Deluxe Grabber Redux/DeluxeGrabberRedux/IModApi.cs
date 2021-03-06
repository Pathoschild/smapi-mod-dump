/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ferdaber/sdv-mods
**
*************************************************/

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using StardewValley;
using SObject = StardewValley.Object;

namespace DeluxeGrabberRedux
{
    public interface IDeluxeGrabberReduxApi
    {
        /// <summary>
        /// This is a delegate that should be set by other mods to override the default harvest outcome of a mushroom.
        /// <br /><br />
        /// <b>Parameter types:</b>
        /// <list type="number">
        ///     <item>
        ///         <term>SObject mushroom</term>
        ///         <description>The original object to be harvested (will be a mushroom object).</description>
        ///     </item>
        ///     <item>
        ///         <term>Vector2 tile</term>
        ///         <description>The tile where the mushroom box was located.</description>
        ///     </item>
        ///     <item>
        ///         <term>GameLocation location</term>
        ///         <description>The location that the harvest occurred in.</description>
        ///     </item>
        /// </list>
        /// <br /><br />
        /// <b>Return type (KeyValuePair):</b>
        /// <list type="number">
        ///     <item>
        ///         <term>SObject newMushroom</term>
        ///         <description>The object that will be harvested instead.</description>
        ///     </item>
        ///     <item>
        ///         <term>int expGain</term>
        ///         <description>The foraging skill EXP to be gained if the harvest was succesful.</description>
        ///     </item>
        /// </list>
        /// </summary>
        Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> GetMushroomHarvest { get; set; }

        /// <summary>
        /// This is a delegate that should be set by other mods to override the default harvest outcome of a berry bush.
        /// <br /><br />
        /// <b>Parameter types:</b>
        /// <list type="number">
        ///     <item>
        ///         <term>SObject berry</term>
        ///         <description>The original object to be harvested (will be a berry or tea leaves).</description>
        ///     </item>
        ///     <item>
        ///         <term>Vector2 tile</term>
        ///         <description>The tile where the bush was located.</description>
        ///     </item>
        ///     <item>
        ///         <term>GameLocation location</term>
        ///         <description>The location that the harvest occurred in.</description>
        ///     </item>
        /// </list>
        /// <br /><br />
        /// <b>Return type (KeyValuePair):</b>
        /// <list type="number">
        ///     <item>
        ///         <term>SObject newBerry</term>
        ///         <description>The object that will be harvested instead.</description>
        ///     </item>
        ///     <item>
        ///         <term>int expGain</term>
        ///         <description>The foraging skill EXP to be gained if the harvest was succesful.</description>
        ///     </item>
        /// </list>
        /// </summary>
        Func<SObject, Vector2, GameLocation, KeyValuePair<SObject, int>> GetBerryBushHarvest { get; set; }
    }
}
