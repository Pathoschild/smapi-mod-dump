/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/aEnigmatic/StardewValley
**
*************************************************/

#nullable enable
using System;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;

namespace ConvenientChests;

public class ModConfigOptions {
    public IGenericModConfigMenuApi ConfigMenu { get; set; }
    public IManifest Manifest { get; set; }
    public string? Section { get; private set; }

    public ModConfigOptions(IGenericModConfigMenuApi configMenu, IManifest manifest) {
        ConfigMenu = configMenu;
        Manifest = manifest;
    }

    public void Register(Action reset, Action save)
        => ConfigMenu.Register(Manifest, reset, save);

    public void AddSection(string text, string? tooltip = null) {
        Section = text;
        ConfigMenu.AddSectionTitle(Manifest, () => text, tooltip == null ? null : () => tooltip);
    }

    public void Add(Func<bool> getter, Action<bool> setter, string name, string? description = null)
        => ConfigMenu.AddBoolOption(Manifest, getter, setter, () => name, () => description);

    public void Add(Func<string> getter, Action<string> setter, string name, string? description = null)
        => ConfigMenu.AddTextOption(Manifest, getter, setter, () => name, () => description);

    public void Add(Func<SButton> getter, Action<SButton> setter, string name, string? description = null)
        => ConfigMenu.AddKeybind(Manifest, getter, setter, () => name, () => description);

    public void Add(Func<int> getter, Action<int> setter, string name, string? description = null)
        => ConfigMenu.AddNumberOption(Manifest, getter, setter, () => name, () => description);

    public void Add(Func<float> getter, Action<float> setter, string name, string? description = null)
        => ConfigMenu.AddNumberOption(Manifest, getter, setter, () => name, () => description);

    public void Add(Func<int> getter, Action<int> setter, string name, int min, int max, string? description = null)
        => ConfigMenu.AddNumberOption(Manifest, getter, setter, () => name, () => description, min, max);

    public void Add(Func<float> getter, Action<float> setter, string name, float min, float max, string? description = null)
        => ConfigMenu.AddNumberOption(Manifest, getter, setter, () => name, () => description, min, max);
}