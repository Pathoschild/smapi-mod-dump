/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv-mods
**
*************************************************/

namespace DaLion.Shared.Integrations.GenericModConfigMenu;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;

#endregion using directives

[RequiresMod("jltaylor-us.GMCMOptions", "GMCM Options")]
[ImplicitIgnore]
internal sealed class GenericModConfigMenuOptionsIntegration : ModIntegration<GenericModConfigMenuOptionsIntegration, IGenericModConfigMenuOptionsApi>
{
    /// <summary>Initializes a new instance of the <see cref="GenericModConfigMenuOptionsIntegration"/> class.</summary>
    internal GenericModConfigMenuOptionsIntegration()
        : base("jltaylor-us.GMCMOptions", "GMCM Options", "1.2.0", ModHelper.ModRegistry)
    {
    }
}
