/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://gitlab.com/daleao/smapi-mods
**
*************************************************/

#nullable enable
namespace DaLion.Stardew.Professions.Framework;

#region using directives

using Common.Integrations.SpaceCore;
using System.Collections.Generic;
using System.Linq;

#endregion using directives

/// <summary>Represents a SpaceCore-provided custom skill.</summary>
public sealed class CustomSkill : ISkill
{
    private readonly ISpaceCoreAPI _api;

    /// <inheritdoc />
    public string StringId { get; }

    /// <inheritdoc />
    public string DisplayName { get; }

    /// <inheritdoc />
    public int CurrentExp => ExtendedSpaceCoreAPI.GetCustomSkillExp.Value(Game1.player, StringId);

    /// <inheritdoc />
    public int CurrentLevel => _api.GetLevelForCustomSkill(Game1.player, StringId);

    /// <inheritdoc />
    public IEnumerable<int> NewLevels => ExtendedSpaceCoreAPI.GetCustomSkillNewLevels.Value()
        .Where(pair => pair.Key == StringId).Select(pair => pair.Value);

    /// <inheritdoc />
    public IList<IProfession> Professions { get; } = new List<IProfession>();

    /// <inheritdoc />
    public IDictionary<int, ProfessionPair> ProfessionPairs { get; } = new Dictionary<int, ProfessionPair>();

    /// <summary>Construct an instance.</summary>
    internal CustomSkill(string id, ISpaceCoreAPI api)
    {
        _api = api;
        StringId = id;

        var instance = ExtendedSpaceCoreAPI.GetCustomSkillInstance.Value(id);
        DisplayName = ExtendedSpaceCoreAPI.GetSkillName.Value(instance);

        var professions = ExtendedSpaceCoreAPI.GetProfessions.Value(instance).Cast<object>()
            .ToList();
        var i = 0;
        foreach (var profession in professions)
        {
            var professionStringId = ExtendedSpaceCoreAPI.GetProfessionStringId.Value(profession);
            var displayName = ExtendedSpaceCoreAPI.GetProfessionDisplayName.Value(profession);
            var description = ExtendedSpaceCoreAPI.GetProfessionDescription.Value(profession);
            var vanillaId = ExtendedSpaceCoreAPI.GetProfessionVanillaId.Value(profession);
            var level = i++ < 2 ? 5 : 10;
            Professions.Add(new CustomProfession(professionStringId, displayName, description, vanillaId, level, this));
        }

        if (Professions.Count != 6)
            ThrowHelper.ThrowInvalidOperationException(
                $"The custom skill {id} did not provide the expected number of professions.");

        ProfessionPairs[-1] = new(Professions[0], Professions[1], null, 5);
        ProfessionPairs[Professions[0].Id] = new(Professions[2], Professions[3], Professions[0], 10);
        ProfessionPairs[Professions[1].Id] = new(Professions[4], Professions[5], Professions[1], 10);
    }
}