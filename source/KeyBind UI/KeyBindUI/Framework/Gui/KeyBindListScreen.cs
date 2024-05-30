/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using System;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using KeyBindUI.Framework.Entity;

namespace KeyBindUI.Framework.Gui;

public class KeyBindListScreen : ScreenGui
{
    public KeyBindListScreen(KeyBindInfoList keyBindInfoList) : base(keyBindInfoList.Mod.Name)
    {
        foreach (var info in keyBindInfoList.Infos)
        {
            AddElement(new Button(info.PropertyInfo.Name,
                ModEntry.GetInstance().Helper.Translation.Get("KeyBindUI.Framework.Screen.ModsGui.KeySetting"))
            {
                OnLeftClicked = () => { OpenScreenGui(new KeyListScreen(keyBindInfoList.Mod, info)); }
            });
        }
    }
}