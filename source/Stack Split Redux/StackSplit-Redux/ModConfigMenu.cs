/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/pepoluan/StackSplitRedux
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using System;

namespace StackSplitRedux
    {
    public interface IGenericModConfigMenuAPI
        {
        void RegisterModConfig(IManifest mod, Action revertToDefault, Action saveToFile);
        void UnregisterModConfig(IManifest mod);
        void SetDefaultIngameOptinValue(IManifest mod, bool optedIn);
        void StartNewPage(IManifest mod, string pageName);
        void OverridePageDisplayName(IManifest mod, string pageName, string displayName);
        void RegisterLabel(IManifest mod, string labelName, string labelDesc);
        void RegisterPageLabel(IManifest mod, string labelName, string labelDesc, string newPage);
        void RegisterParagraph(IManifest mod, string paragraph);
        void RegisterImage(IManifest mod, string texPath, Rectangle? texRect = null, int scale = 4);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc,
                                  Func<bool> optionGet, Action<bool> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc,
                                  Func<int> optionGet, Action<int> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc,
                                  Func<float> optionGet, Action<float> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc,
                                  Func<string> optionGet, Action<string> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc,
                                  Func<SButton> optionGet, Action<SButton> optionSet);
        void RegisterSimpleOption(IManifest mod, string optionName, string optionDesc,
                                  Func<KeybindList> optionGet, Action<KeybindList> optionSet);
        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc,
                                   Func<int> optionGet, Action<int> optionSet,
                                   int min, int max);
        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc,
                                   Func<float> optionGet, Action<float> optionSet,
                                   float min, float max);
        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc,
                                   Func<int> optionGet, Action<int> optionSet,
                                   int min, int max, int interval);
        void RegisterClampedOption(IManifest mod, string optionName, string optionDesc,
                                   Func<float> optionGet, Action<float> optionSet,
                                   float min, float max, float interval);
        void RegisterChoiceOption(IManifest mod, string optionName, string optionDesc,
                                  Func<string> optionGet, Action<string> optionSet,
                                  string[] choices);
        void RegisterComplexOption(IManifest mod, string optionName, string optionDesc,
                                   Func<Vector2, object, object> widgetUpdate,
                                   Func<SpriteBatch, Vector2, object, object> widgetDraw,
                                   Action<object> onSave);
        void SubscribeToChange(IManifest mod, Action<string, bool> changeHandler);
        void SubscribeToChange(IManifest mod, Action<string, int> changeHandler);
        void SubscribeToChange(IManifest mod, Action<string, float> changeHandler);
        void SubscribeToChange(IManifest mod, Action<string, string> changeHandler);
        void OpenModMenu(IManifest mod);
        }

    class ModConfigMenu
        {
        private readonly IGenericModConfigMenuAPI Api;
        private readonly IManifest ModManifest;
        private readonly IModHelper Helper;

        internal ModConfigMenu() {
            this.Api = Mod.Registry.GetApi<IGenericModConfigMenuAPI>("spacechase0.GenericModConfigMenu");
            if (this.Api is null) {
                Log.Trace("GMCM not available, skipping Mod Config Menu");
                return;
                }
            this.Helper = Mod.Instance.Helper;
            this.ModManifest = Mod.Instance.ModManifest;
            RegisterMenu();
            }

        void RegisterMenu() {
            this.Api.RegisterModConfig(
                this.ModManifest,
                () => Mod.Config = new ModConfig(),
                () => this.Helper.WriteConfig<ModConfig>(Mod.Config)
                );

            this.Api.SetDefaultIngameOptinValue(this.ModManifest, true);
            // Everything from this point on are available in-game

            this.Api.RegisterSimpleOption(
                this.ModManifest,
                "Default cooking/crafting amount",
                "The default amount when you Shift+RightClick in the Cooking / Crafting menu",
                () => Mod.Config.DefaultCraftingAmount,   // a function with no input, but outputs value of type T
                (int value) => Mod.Config.DefaultCraftingAmount = value
                );

            this.Api.RegisterSimpleOption(
                this.ModManifest,
                "Default shop amount",
                "The default amount when you Shift+RightClick in a Shop menu",
                () => Mod.Config.DefaultShopAmount,   // a function with no input, but outputs value of type T
                (int value) => Mod.Config.DefaultShopAmount = value
                );

            }
        }
    }
