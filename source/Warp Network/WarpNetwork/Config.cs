/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/tlitookilakin/WarpNetwork
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using System;
using System.Text;

namespace WarpNetwork
{
    enum WarpEnabled
    {
        AfterObelisk,
        Always,
        Never
    }
    class Config
    {
        public WarpEnabled VanillaWarpsEnabled { get; set; }
        public WarpEnabled FarmWarpEnabled { get; set; }
        public bool AccessFromDisabled { get; set; }
        public bool AccessFromWand { get; set; }
        public bool PatchObelisks { get; set; }
        public bool MenuEnabled { get; set; }

        public void ResetToDefault()
        {
            VanillaWarpsEnabled = WarpEnabled.AfterObelisk;
            FarmWarpEnabled = WarpEnabled.AfterObelisk;
            AccessFromDisabled = false;
            AccessFromWand = false;
            PatchObelisks = true;
            MenuEnabled = true;
        }
        public void RegisterModConfigMenu(IModHelper helper, IManifest manifest)
        {
            if (!helper.ModRegistry.IsLoaded("spacechase0.GenericModConfigMenu"))
                return;
            IGMCMAPI api = helper.ModRegistry.GetApi<IGMCMAPI>("spacechase0.GenericModConfigMenu");
            api.RegisterModConfig(manifest, ResetToDefault, () => helper.WriteConfig(this));
            api.SetDefaultIngameOptinValue(manifest, true);

            api.RegisterLabel(manifest, manifest.Name, manifest.Description);

            api.RegisterChoiceOption(
                manifest, 
                helper.Translation.Get("cfg-warpsenabled.label"), 
                helper.Translation.Get("cfg-warpsenabled.desc"), 
                () => VanillaWarpsEnabled.ToString(), 
                (string c) => VanillaWarpsEnabled = Utils.ParseEnum<WarpEnabled>(c), 
                Enum.GetNames(typeof(WarpEnabled))
            );
            api.RegisterChoiceOption(
                manifest,
                helper.Translation.Get("cfg-farmenabled.label"),
                helper.Translation.Get("cfg-farmenabled.desc"),
                () => FarmWarpEnabled.ToString(),
                (string c) => FarmWarpEnabled = Utils.ParseEnum<WarpEnabled>(c),
                Enum.GetNames(typeof(WarpEnabled))
            );
            api.RegisterSimpleOption(
                manifest,
                helper.Translation.Get("cfg-accessdisabled.label"),
                helper.Translation.Get("cfg-accessdisabled.desc"),
                () => AccessFromDisabled,
                (bool b) => AccessFromDisabled = b
            );
            api.RegisterSimpleOption(
                manifest,
                helper.Translation.Get("cfg-accesswand.label"),
                helper.Translation.Get("cfg-accesswand.desc"),
                () => AccessFromWand,
                (bool b) => AccessFromWand = b
            );
            api.RegisterSimpleOption(
                manifest,
                helper.Translation.Get("cfg-obeliskpatch.label"),
                helper.Translation.Get("cfg-obeliskpatch.desc"),
                () => PatchObelisks,
                (bool b) => PatchObelisks = b
            );
            api.RegisterSimpleOption(
                manifest,
                helper.Translation.Get("cfg-menu.label"),
                helper.Translation.Get("cfg-menu.desc"),
                () => MenuEnabled,
                (bool b) => MenuEnabled = b
            );
        }
        internal string AsText()
        {
            StringBuilder sb = new StringBuilder(8);
            sb.AppendLine().AppendLine("Config:");
            sb.Append("\tVanillaWarpsEnabled: ").AppendLine(VanillaWarpsEnabled.ToString());
            sb.Append("\tFarmWarpEnabled: ").AppendLine(FarmWarpEnabled.ToString());
            sb.Append("\tAccessFromDisabled: ").AppendLine(AccessFromDisabled.ToString());
            sb.Append("\tAccessFromWand: ").AppendLine(AccessFromWand.ToString());
            sb.Append("\tPatchObelisks: ").AppendLine(PatchObelisks.ToString());
            sb.Append("\tMenuEnabled: ").AppendLine(MenuEnabled.ToString());
            return sb.ToString();
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
