/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.LoveOfCooking;

#endregion using directives

[ModRequirement("blueberry.LoveOfCooking", "Love Of Cooking", "1.0.27")]
internal sealed class LoveOfCookingIntegration : ModIntegration<LoveOfCookingIntegration, ICookingSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LoveOfCookingIntegration"/> class.</summary>
    internal LoveOfCookingIntegration()
        : base("blueberry.LoveOfCooking", "Love Of Cooking", "1.0.27", ModHelper.ModRegistry)
    {
    }
}
