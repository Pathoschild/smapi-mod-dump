/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/SimpleHUD
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;
using SimpleHUD.Framework.Screen;

namespace SimpleHUD.Framework.Gui
{
    public class SettingGui : ScreenGui
    {
        protected override void Init()
        {
            base.Init();
            var toggleButton = new ToggleButton("", "");
            toggleButton.Toggled = ModEntry.Config.Enable;
            toggleButton.Title = ModEntry.Config.Enable ? Get("setting.enable") : Get("setting.disable");
            toggleButton.OnLeftClicked = () =>
            {
                ModEntry.Config.Enable = toggleButton.Toggled;
                ModEntry.GetInstance().Helper.WriteConfig(ModEntry.Config);
            };
            AddElement(toggleButton);
            var title = new Button(Get("setting.title"), "")
            {
                OnLeftClicked = () =>
                {
                    OpenScreenGui(new InputStringScreen(input =>
                        {
                            ModEntry.Config.Title = input;
                            ModEntry.GetInstance().Helper.WriteConfig(ModEntry.Config);
                        }
                    ));
                }
            };
            AddElement(title);
            AddElement(new Button(Get("setting.textColor"), "setting.textColor")
            {
                OnLeftClicked = () =>
                {
                    OpenScreenGui(new SettingColorScreen(select =>
                    {
                        ModEntry.Config.TextColor = select;
                        ModEntry.GetInstance().Helper.WriteConfig(ModEntry.Config);
                    }));
                }
            });
        }

        private string Get(string key)
        {
            return ModEntry.GetInstance().Helper.Translation.Get(key);
        }
    }
}