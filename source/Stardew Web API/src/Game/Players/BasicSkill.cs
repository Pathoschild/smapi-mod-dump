/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/zunderscore/StardewWebApi
**
*************************************************/

using System.Text.Json.Serialization;
using StardewModdingAPI.Enums;
using StardewValley.Menus;
using StardewWebApi.Types;

namespace StardewWebApi.Game.Players;

public class BasicSkill
{
    private readonly List<int> _professionIds;

    public BasicSkill(int id, int level, IEnumerable<int>? professionIds = null)
    {
        Id = id;
        Level = level;
        _professionIds = professionIds is not null
            ? new(professionIds)
            : new();
    }

    public int Id { get; }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    public SkillType Name => (SkillType)Id;

    public int Level { get; }

    public IEnumerable<NumericIdNameDescription> Professions => _professionIds.Select(id =>
        {
            var description = LevelUpMenu.getProfessionDescription(id);
            return new NumericIdNameDescription(id, description[0], description[1]);
        });
}
