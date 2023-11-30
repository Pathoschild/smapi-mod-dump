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
using StardewModdingAPI;

namespace BinningSkill;

[SAsset(Priority = 0)]
public class Assets
{
    [SAsset.Asset("assets/skill_texture.png")]
    public IRawTextureData SkillTexture;

    [SAsset.Asset("assets/spring_tilesheet_trashcans.png")]
    public Texture2D TrashCanTilesheet;
    public IAssetName TrashCanTilesheetAssetName;

    [SAsset.Asset("assets/animation_copper_can.png")]
    public Texture2D AnimationCopper;
    public IAssetName AnimationCopperAssetName;

    [SAsset.Asset("assets/animation_iron_can.png")]
    public Texture2D AnimationIron;
    public IAssetName AnimationIronAssetName;

    [SAsset.Asset("assets/animation_gold_can.png")]
    public Texture2D AnimationGold;
    public IAssetName AnimationGoldAssetName;

    [SAsset.Asset("assets/animation_iridium_can.png")]
    public Texture2D AnimationIridium;
    public IAssetName AnimationIridiumAssetName;

    [SAsset.Asset("assets/animation_radioactive_can.png")]
    public Texture2D AnimationRadioactive;
    public IAssetName AnimationRadioactiveAssetName;

    [SAsset.Asset("assets/animation_prismatic_can.png")]
    public Texture2D AnimationPrismatic;
    public IAssetName AnimationPrismaticAssetName;
}
