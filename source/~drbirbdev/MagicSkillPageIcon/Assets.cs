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
using Microsoft.Xna.Framework.Graphics;

namespace MagicSkillPageIcon;

[SAsset]
public class Assets
{
    [SAsset.Asset("assets/magicskillpageicon.png")]
    public Texture2D SkillPageIcon { get; set; }
}
