/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/LeFauxMatt/StardewMods
**
*************************************************/

namespace StardewMods.SpritePatcher.Framework.Services.Migrations;

using StardewMods.SpritePatcher.Framework.Interfaces;

/// <inheritdoc />
internal sealed class Migration_1_0 : BaseMigration
{
    /// <summary>Initializes a new instance of the <see cref="Migration_1_0"/> class.</summary>
    public Migration_1_0()
        : base(new SemanticVersion(1, 0, 0)) { }
}