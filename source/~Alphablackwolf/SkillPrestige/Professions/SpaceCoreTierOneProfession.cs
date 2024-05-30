/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Alphablackwolf/SkillPrestige
**
*************************************************/

using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using SpaceCore;

namespace SkillPrestige.Professions;

public class SpaceCoreTierOneProfession : TierOneProfession
{
    public SpaceCoreTierOneProfession() {}

    public SpaceCoreTierOneProfession(Skills.Skill.Profession spaceCoreProfession)
    {
        this.DisplayName = spaceCoreProfession.GetName();
        this.EffectText = new List<string> { spaceCoreProfession.GetDescription() };
        this.Id = spaceCoreProfession.GetVanillaId();
        this.Texture = spaceCoreProfession.Icon;
    }

    public sealed override Texture2D Texture { get; set; }

    public override Rectangle IconSourceRectangle => new (0, 0, 16, 16);
}
