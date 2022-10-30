/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/slothsoft/stardew-challenger
**
*************************************************/

using Slothsoft.Challenger.Models;
using Slothsoft.Challenger.Objects;

namespace Slothsoft.Challenger.Api;

/// <summary>
/// Record for what to replace the magical object with.
/// </summary>
public record MagicalReplacement(
    // See https://stardewcommunitywiki.com/Modding:Big_craftables_data
    int ParentSheetIndex,
    string Name
) {
    public static readonly MagicalReplacement Default = new(ObjectIds.PinkyBunny, MagicalObject.ObjectName);
    
    public static readonly MagicalReplacement Keg = new(ObjectIds.Keg, "Keg"); 
    public static readonly MagicalReplacement SeedMaker = new(ObjectIds.SeedMaker, "Seed Maker"); 
}