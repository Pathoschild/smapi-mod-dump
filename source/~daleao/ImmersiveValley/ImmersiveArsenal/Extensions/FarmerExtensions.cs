/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Stardew.Arsenal.Extensions;

#region using directives

using StardewValley.Tools;

#endregion using directives

/// <summary>Extensions for the <see cref="Farmer"/> class.</summary>
public class FarmerExtensions
{
    /// <summary>Transform the currently held weapon into the Holy Blade.</summary>
    internal static void GetHolyBlade()
    {
        Game1.flashAlpha = 1f;
        Game1.player.holdUpItemThenMessage(new MeleeWeapon(Constants.HOLY_BLADE_INDEX_I));
        ((MeleeWeapon)Game1.player.CurrentTool).transform(Constants.HOLY_BLADE_INDEX_I);
        Game1.player.mailReceived.Add("holyBlade");
        Game1.player.jitterStrength = 0f;
        Game1.screenGlowHold = false;
    }
}