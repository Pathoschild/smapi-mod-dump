/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpnetDeepwoods
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;

namespace WarpnetDeepwoods
{
    class Config
    {
        public bool AfterObelisk { set; get; }

        public void ResetToDefault()
        {
            AfterObelisk = true;
        }
        public void RegisterModConfigMenu(IModHelper helper, IManifest manifest)
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
                return;
            IGMCMAPI api = helper.ModRegistry.GetApi<IGMCMAPI>("spacechase0.GenericModConfigMenu");
            api.RegisterModConfig(manifest, ResetToDefault, () => helper.WriteConfig(this));
            api.SetDefaultIngameOptinValue(manifest, true);

            api.RegisterLabel(manifest, manifest.Name, manifest.Description);

            api.RegisterSimpleOption(
                manifest,
                helper.Translation.Get("cfg-afterobelisk.label"),
                helper.Translation.Get("cfg-afterobelisk.desc"),
                () => AfterObelisk,
                (bool b) => AfterObelisk = b
            );
        }
        public Config()
        {
            ResetToDefault();
        }
    }
    public interface IGMCMAPI
    {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);

        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc, Func<SButton> optionGet, Action<SButton> optionSet);

        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<int> optionGet, Action<int> optionSet, int min, int max);
        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc, Func<float> optionGet, Action<float> optionSet, float min, float max);

        void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc, Func<string> optionGet, Action<string> optionSet, string[] choices);

        void RegisterComplexOption(IManifest mod, string optionName, string optionDesc,
                                   Func<Vector2, object, object> widgetUpdate,
                                   Func<SpriteBatch, Vector2, object, object> widgetDraw,
                                   Action<object> onSave);
    }
}
