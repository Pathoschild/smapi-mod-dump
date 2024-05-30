/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/sdv
**
*************************************************/

namespace DaLion.Professions.Framework.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using SpaceShared.APIs;

#endregion using directives

/// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
[ModRequirement("spacechase0.SpaceCore", "SpaceCore")]
internal sealed class SpaceCoreIntegration()
    : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>(ModHelper.ModRegistry)
{
    /// <summary>Gets the SpaceCore API.</summary>>
    internal static ISpaceCoreApi Api => Instance!.ModApi!; // guaranteed not null by dependency

    /// <summary>Attempts to instantiate and cache one instance of every <see cref="CustomSkill"/>.</summary>
    /// <returns><see langword="true"/> if a new instance of <see cref="CustomSkill"/> was loaded, otherwise <see langword="false"/>.</returns>
    internal bool TryLoadSpaceCoreSkills()
    {
        this.AssertLoaded();

        var anyLoaded = false;
        foreach (var skillId in this.ModApi.GetCustomSkills())
        {
            // checking if the skill is loaded first avoids re-instantiating the skill
            if (CustomSkill.Loaded.ContainsKey(skillId))
            {
                continue;
            }

            CustomSkill.Initialize(skillId);
            anyLoaded = true;
        }

        return anyLoaded;
    }

    /// <summary>Gets SpaceCore's internal list of unrealized new levels.</summary>
    /// <returns>A <see cref="List{T}"/> of <see cref="KeyValuePair"/>s with <see cref="SCSkill"/> ID <see cref="string"/> keys and <see cref="int"/> level values.</returns>
    internal List<KeyValuePair<string, int>> GetNewLevels()
    {
        return Reflector
            .GetStaticFieldGetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
            .Invoke();
    }

    /// <summary>Sets SpaceCore's internal list of unrealized new levels.</summary>
    internal void SetNewLevels(List<KeyValuePair<string, int>> newLevels)
    {
        Reflector
            .GetStaticFieldSetter<List<KeyValuePair<string, int>>>(typeof(SCSkills), "NewLevels")
            .Invoke(newLevels);
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.TryLoadSpaceCoreSkills();
        Log.D("Registered the SpaceCore integration.");
        return base.RegisterImpl();
    }
}
