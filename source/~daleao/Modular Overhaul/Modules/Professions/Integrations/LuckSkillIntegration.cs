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
using DaLion.Shared.Integrations.LuckSkill;

#endregion using directives

[ModRequirement("spacechase0.LuckSkill", "Luck Skill", "1.2.3")]
internal sealed class LuckSkillIntegration : ModIntegration<LuckSkillIntegration, ILuckSkillApi>
{
    /// <summary>Initializes a new instance of the <see cref="LuckSkillIntegration"/> class.</summary>
    internal LuckSkillIntegration()
        : base("spacechase0.LuckSkill", "Luck Skill", "1.2.3", ModHelper.ModRegistry)
    {
    }

    /// <inheritdoc />
    protected override bool RegisterImpl()
    {
        this.LoadLuckSkill();
        return base.RegisterImpl();
    }

    /// <summary>Instantiates and caches the <see cref="LuckSkill"/> instance.</summary>
    private void LoadLuckSkill()
    {
        this.AssertLoaded();
        if (LuckSkill.Instance is not null)
        {
            return;
        }

        var luckSkill = new LuckSkill();
        Skill.Luck = luckSkill;
        SCSkill.Loaded["spacechase0.LuckSkill"] = luckSkill;
        Log.T($"[PROFS]: Successfully loaded the custom skill {this.ModId}.");
    }
}
