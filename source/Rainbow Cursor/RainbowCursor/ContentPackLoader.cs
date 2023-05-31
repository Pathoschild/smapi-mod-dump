/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewRainbowCursor
**
*************************************************/

// Copyright 2023 Jamie Taylor
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using RainbowCursor.Models;
using StardewModdingAPI;

namespace RainbowCursor {
    internal static class ContentPackLoader {
        public static readonly ISemanticVersion CurrentFormatVersion = new SemanticVersion("1.0.0");
        public static void LoadContentPacks(ModEntry mod) {
            foreach (IContentPack contentPack in mod.Helper.ContentPacks.GetOwned()) {
                LoadContentPack(mod, contentPack);
            }
        }

        private static void LoadContentPack(ModEntry mod, IContentPack contentPack) {
            string cpName = contentPack.Manifest.Name;
            mod.Monitor.Log($"Reading content pack: {cpName} {contentPack.Manifest.Version} from {contentPack.DirectoryPath}", LogLevel.Trace);
            PalettesConfig? palettesConfig;
            try {
                palettesConfig = contentPack.ReadJsonFile<PalettesConfig>("palettes.json");
            } catch (Exception e) {
                mod.Monitor.Log(I18n.ContentPack_Palettes_Error(cpName: cpName, details: e.Message), LogLevel.Warn);
                return;
            }
            if (palettesConfig is null) {
                mod.Monitor.Log(I18n.ContentPack_Palettes_Missing(cpName: cpName), LogLevel.Warn);
                return;
            }
            if (palettesConfig.FormatVersion is null) {
                mod.Monitor.Log(I18n.ContentPack_Palettes_FormatMissing(cpName: cpName), LogLevel.Warn);
                return;
            }
            if (palettesConfig.FormatVersion.IsNewerThan(CurrentFormatVersion)) {
                if (palettesConfig.FormatVersion.MajorVersion == CurrentFormatVersion.MajorVersion) {
                    mod.Monitor.Log(I18n.ContentPack_Palettes_NewMinorFormat(cpName: cpName, cpFormatVersion: palettesConfig.FormatVersion, modName: mod.ModManifest.Name, modFormatVersion: CurrentFormatVersion), LogLevel.Info);
                } else {
                    mod.Monitor.Log(I18n.ContentPack_Palettes_NewMajorFormat(cpName: cpName, cpFormatVersion: palettesConfig.FormatVersion, modName: mod.ModManifest.Name, modFormatVersion: CurrentFormatVersion), LogLevel.Warn);
                }
            }
            mod.Monitor.Log($"Read model from {cpName}: Format Version {palettesConfig.FormatVersion}", LogLevel.Debug);
            int paletteIdx = 0;
            if (palettesConfig.Palettes is not null) {
                IRainbowCursorAPI rcApi = new ContentPackAPIWrapper(contentPack.Manifest);
                foreach (PaletteConfig palette in palettesConfig.Palettes) {
                    if (palette.Id is null) {
                        mod.Monitor.Log(I18n.ContentPack_Palette_MissingId(cpName: cpName, paletteIndex: paletteIdx), LogLevel.Warn);
                        continue;
                    }
                    if (palette.Colors is null) {
                        mod.Monitor.Log(I18n.ContentPack_Palette_MissingColors(cpName: cpName, paletteName: palette.Name ?? palette.Id), LogLevel.Warn);
                        continue;
                    }
                    List<Color> colorsWithAlpha = new(palette.Colors.Count);
                    foreach(Color c in palette.Colors) {
                        colorsWithAlpha.Add(new Color(c.R, c.G, c.B, (byte)255));
                    }
                    Func<string> nameGetter = () => {
                        Translation t = contentPack.Translation.Get($"palette.{palette.Id}.name");
                        if (t.HasValue()) {
                            return t.ToString();
                        }
                        return palette.Name ?? palette.Id;
                    };
                    Func<string?> makeTranslationGetter(string key) {
                        return () => {
                            Translation t = contentPack.Translation.Get($"palette.{palette.Id}.{key}");
                            if (t.HasValue()) {
                                return t.ToString();
                            }
                            return null;
                        };
                    }
                    rcApi.AddColorPalette(palette.Id, nameGetter, colorsWithAlpha,
                        makeTranslationGetter("tooltipTitle"), makeTranslationGetter("tooltip"));
                    paletteIdx++;
                }
                if (paletteIdx == 0) {
                    mod.Monitor.Log(I18n.ContentPack_Palettes_Read0(cpName: cpName), LogLevel.Info);
                } else if (paletteIdx == 1) {
                    mod.Monitor.Log(I18n.ContentPack_Palettes_Read1(cpName: cpName), LogLevel.Info);
                } else {
                    mod.Monitor.Log(I18n.ContentPack_Palettes_ReadN(cpName: cpName, count: paletteIdx), LogLevel.Info);
                }
            }
        }
    }

    internal class ContentPackAPIWrapper : APIWrapper {
        public ContentPackAPIWrapper(IManifest providedByManifest) : base(providedByManifest) {
        }

        override protected string TransformId(string id) {
            return $"{providedByManifest.UniqueID}.{id}";
        }
    }
}

