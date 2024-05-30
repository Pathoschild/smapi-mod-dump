/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/NameTags
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace NameTags.Framework;

public class Config
{
    public KeybindList OpenSetting { get; set; } = new(SButton.N);
    public Color Color { get; set; } = Color.White;
    public Color BackgroundColor { get; set; } = Color.Black;
    public bool RenderMonster { get; set; } = true;
    public bool RenderPet { get; set; } = true;
    public bool RenderHorse { get; set; } = true;
    public bool RenderChild { get; set; } = true;
    public bool RenderVillager { get; set; } = true;
    public bool RenderJunimo { get; set; } = true;
    public bool TargetLine { get; set; } = false;
}