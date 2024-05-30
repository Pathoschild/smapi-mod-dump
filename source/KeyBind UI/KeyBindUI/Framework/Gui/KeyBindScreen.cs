/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/KeyBindUI
**
*************************************************/

using System.Reflection;
using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using KeyBindUI.Framework.Entity;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;

namespace KeyBindUI.Framework.Gui;

public class KeyBindScreen : ScreenGui
{
    public KeyBindScreen() : base("Key Bind UI")
    {
        foreach (var modInfo in ModEntry.GetInstance().Helper.ModRegistry.GetAll())
        {
            var mod = (modInfo.GetType().GetMethod("get_Mod")!.Invoke(modInfo, null) as IMod)!;
            var configField = mod.GetType()
                .GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance)
                .FirstOrDefault(it =>
                    it.Name.Contains("Config") || it.Name.Contains("config") || it.Name.Contains("_config"));

            if (configField == null) continue;

            var config = configField.GetValue(mod);

            if (config == null) continue;
            var keyBinds = config.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static |
                               BindingFlags.Instance)
                .Where(it => it.PropertyType == typeof(KeybindList) || it.PropertyType == typeof(SButton)).ToList();
            if (keyBinds.Any())
            {
                AddElement(new Button(modInfo.Manifest.Name, modInfo.Manifest.Description)
                {
                    OnLeftClicked = () =>
                    {
                        OpenScreenGui(
                            new KeyBindListScreen(new KeyBindInfoList((modInfo.Manifest.Name, mod),
                                keyBinds.Select(it => (it, config)).ToList())));
                    }
                });
            }
        }
    }
}