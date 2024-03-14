/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Goldenrevolver/Stardew-Valley-Mods
**
*************************************************/

namespace DropItHotkey
{
    using StardewModdingAPI;
    using StardewModdingAPI.Utilities;
    using System;

    public interface IGenericModConfigMenuApi
    {
        void AddKeybindList(IManifest mod, Func<KeybindList> getValue, Action<KeybindList> setValue, Func<string> name, Func<string> tooltip = null, string fieldId = null);

        void Register(IManifest mod, Action reset, Action save, bool titleScreenOnly = false);

        void SetTitleScreenOnlyForNextOptions(IManifest mod, bool titleScreenOnly);
    }

    public class Config
    {
        public KeybindList DropKey { get; set; } = KeybindList.Parse("LeftStick");

        public static void SetUpModConfigMenu(Config config, DropItHotkey mod)
        {
            IGenericModConfigMenuApi api = mod.Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");

            if (api == null)
            {
                return;
            }

            var manifest = mod.ModManifest;

            api.Register(
                mod: manifest,
                reset: () => config = new Config(),
                save: () => mod.Helper.WriteConfig(config)
            );

            api.SetTitleScreenOnlyForNextOptions(manifest, false);

            api.AddKeybindList(manifest, () => config.DropKey, (KeybindList keybindList) => config.DropKey = keybindList, () => "Drop Key");
        }
    }
}