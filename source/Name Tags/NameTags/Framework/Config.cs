/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/NameTags
**
*************************************************/

using EnaiumToolKit.Framework.Utils;
using StardewModdingAPI;

namespace NameTags.Framework;

public class Config
{
    public SButton OpenSetting { get; set; } = SButton.N;
    public ColorUtils.NameType TextColor = ColorUtils.NameType.White;
    public bool RenderMonster { get; set; } = true;
    public bool RenderPet { get; set; } = true;
    public bool RenderHorse { get; set; } = true;
    public bool RenderChild { get; set; } = true;
    public bool RenderVillager { get; set; } = true;
    public bool RenderJunimo { get; set; } = true;
}