/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/drbirbdev/StardewValley
**
*************************************************/

using BirbCore.Attributes;
using StardewModdingAPI;

namespace SlimingSkill;

[SAsset]
internal class Assets
{

    [SAsset.Asset("assets/skill_texture.png")]
    public IRawTextureData SkillTexture { get; set; }
}
