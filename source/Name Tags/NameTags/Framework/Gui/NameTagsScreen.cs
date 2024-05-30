/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Enaium-StardewValleyMods/NameTags
**
*************************************************/

using EnaiumToolKit.Framework.Screen;
using EnaiumToolKit.Framework.Screen.Elements;

namespace NameTags.Framework.Gui;

public class NameTagsScreen : ScreenGui
{
    public NameTagsScreen()
    {
        var renderMonster = new ToggleButton(Get("nameTags.toggle.renderMonster"),
            Get("nameTags.toggle.renderMonster.description"))
        {
            Current = ModEntry.Config.RenderMonster,
            OnCurrentChanged = (toggle) =>
            {
                ModEntry.Config.RenderMonster = toggle;
                ModEntry.ConfigReload();
            }
        };

        var renderPet = new ToggleButton(Get("nameTags.toggle.renderPet"),
            Get("nameTags.toggle.renderPet.description"))
        {
            Current = ModEntry.Config.RenderPet,
            OnCurrentChanged = (toggle) =>
            {
                ModEntry.Config.RenderPet = toggle;
                ModEntry.ConfigReload();
            }
        };

        var renderHorse = new ToggleButton(Get("nameTags.toggle.renderHorse"),
            Get("nameTags.toggle.renderHorse.description"))
        {
            Current = ModEntry.Config.RenderHorse,
            OnCurrentChanged = (toggle) => { ModEntry.Config.RenderHorse = toggle; }
        };

        var renderChild = new ToggleButton(Get("nameTags.toggle.renderChild"),
            Get("nameTags.toggle.renderChild.description"))
        {
            Current = ModEntry.Config.RenderChild,
            OnCurrentChanged = (toggle) =>
            {
                ModEntry.Config.RenderChild = toggle;
                ModEntry.ConfigReload();
            }
        };

        var renderVillager = new ToggleButton(Get("nameTags.toggle.renderVillager"),
            Get("nameTags.toggle.renderVillager.description"))
        {
            Current = ModEntry.Config.RenderVillager,
            OnCurrentChanged = (toggle) =>
            {
                ModEntry.Config.RenderVillager = toggle;
                ModEntry.ConfigReload();
            }
        };

        var renderJunimo = new ToggleButton(Get("nameTags.toggle.renderJunimo"),
            Get("nameTags.toggle.renderJunimo.description"))
        {
            Current = ModEntry.Config.RenderJunimo,
            OnCurrentChanged = (toggle) =>
            {
                ModEntry.Config.RenderJunimo = toggle;
                ModEntry.ConfigReload();
            }
        };
        var targetLine = new ToggleButton(Get("nameTags.toggle.targetLine"),
            Get("nameTags.toggle.targetLine.description"))
        {
            Current = ModEntry.Config.TargetLine,
            OnCurrentChanged = (toggle) =>
            {
                ModEntry.Config.TargetLine = toggle;
                ModEntry.ConfigReload();
            }
        };
        var textColor = new ColorPicker(Get("nameTags.button.nameTagColor"),
            Get("nameTags.button.nameTagColor.description"), ModEntry.Config.Color)
        {
            OnCurrentChanged = (color) => { ModEntry.Config.Color = color; }
        };
        var textBackgroundColor = new ColorPicker(Get("nameTags.button.nameTagBackgroundColor"),
            Get("nameTags.button.nameTagBackgroundColor.description"), ModEntry.Config.BackgroundColor)
        {
            OnCurrentChanged = (color) => { ModEntry.Config.BackgroundColor = color; }
        };
        AddElementRange(renderMonster, renderPet, renderHorse, renderChild, renderVillager, renderJunimo, targetLine,
            textColor, textBackgroundColor);
    }

    public string Get(string key)
    {
        return ModEntry.GetInstance().Helper.Translation.Get(key);
    }
}