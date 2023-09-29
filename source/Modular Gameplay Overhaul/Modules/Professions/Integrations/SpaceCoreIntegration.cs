/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/daleao/modular-overhaul
**
*************************************************/

namespace DaLion.Overhaul.Modules.Professions.Integrations;

#region using directives

using DaLion.Shared.Attributes;
using DaLion.Shared.Integrations;
using DaLion.Shared.Integrations.SpaceCore;

#endregion using directives

[ModRequirement("spacechase0.SpaceCore", "SpaceCore", "1.12.0")]
internal sealed class SpaceCoreIntegration : ModIntegration<SpaceCoreIntegration, ISpaceCoreApi>
{
    /// <summary>Initializes a new instance of the <see cref="SpaceCoreIntegration"/> class.</summary>
    internal SpaceCoreIntegration()
        : base(ModHelper.ModRegistry)
    {
    }

    /// <summary>Instantiates and caches one instance of every <see cref="SCSkill"/>.</summary>
    internal void LoadSpaceCoreSkills()
    {
        this.AssertLoaded();
        foreach (var skillId in this.ModApi.GetCustomSkills())
        {
            // checking if the skill is loaded first avoids re-instantiating the skill
            if (SCSkill.Loaded.ContainsKey(skillId))
            {
                continue;
            }

            SCSkill.Loaded[skillId] = new SCSkill(skillId);
            Log.D($"[PROFS]: Successfully loaded the custom skill {skillId}.");
        }
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.LoadSpaceCoreSkills();
        Log.D("[PROFS]: Registered the SpaceCore integration.");
        return base.RegisterImpl();
    }
}
