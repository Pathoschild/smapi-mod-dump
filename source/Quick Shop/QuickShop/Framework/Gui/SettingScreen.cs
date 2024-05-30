/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/QuickShop
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;

namespace QuickShop.Framework.Gui;

public class SettingScreen : ScreenGui
{
    public SettingScreen() : base(ModEntry.GetInstance().GetSettingTranslation("title"))
    {
        AddElement(new CheckBox(ModEntry.GetInstance().GetSettingTranslation("allowToolUpgradeAgain"),
            ModEntry.GetInstance().GetSettingTranslation("allowToolUpgradeAgain.description"))
        {
            Current = ModEntry.GetInstance().Config.AllowToolUpgradeAgain,
            OnCurrentChanged = value => { ModEntry.GetInstance().Config.AllowToolUpgradeAgain = value; }
        });
        AddElement(new CheckBox(ModEntry.GetInstance().GetSettingTranslation("allowBuildingAgain"),
            ModEntry.GetInstance().GetSettingTranslation("allowBuildingAgain.description"))
        {
            Current = ModEntry.GetInstance().Config.AllowBuildingAgain,
            OnCurrentChanged = value => { ModEntry.GetInstance().Config.AllowBuildingAgain = value; }
        });
    }
}