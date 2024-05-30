/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/ichortower/Nightshade
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using StardewValley;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;
using System.Reflection;

namespace ichortower
{
    public class GMCMIntegration
    {
        public static void Setup()
        {
            var gmcmApi = Nightshade.instance.Helper.ModRegistry.GetApi
                    <IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            if (gmcmApi is null) {
                return;
            }
            gmcmApi.Register(mod: Nightshade.instance.ModManifest,
                reset: () => {},
                save: () => {
                    Nightshade.instance.Helper.WriteConfig(Nightshade.Config);
                });
            gmcmApi.AddKeybindList(
                mod: Nightshade.instance.ModManifest,
                name: () => TR.Get("gmcm.MenuKeybind.name"),
                tooltip: () => TR.Get("gmcm.MenuKeybind.tooltip"),
                getValue: () => Nightshade.Config.MenuKeybind,
                setValue: (value) => {
                    Nightshade.Config.MenuKeybind = value;
                }
            );
            gmcmApi.AddParagraph(
                mod: Nightshade.instance.ModManifest,
                text: () => TR.Get("gmcm.Explainer.text")
            );
        }
    }
}

namespace GenericModConfigMenu
{
    public interface IGenericModConfigMenuApi
    {
        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);
        void AddParagraph(IManifest mod, Func<string> text);
        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);
        // TODO see if i can use this to open my menu
        //void AddComplexOption(IManifest mod, Func<string> name, Action<SpriteBatch, Vector2> draw, Func<string> tooltip = null, Action beforeMenuOpened = null, Action beforeSave = null, Action afterSave = null, Action beforeReset = null, Action afterReset = null, Action beforeMenuClosed = null, Func<int> height = null, string fieldId = null);
    }
}
