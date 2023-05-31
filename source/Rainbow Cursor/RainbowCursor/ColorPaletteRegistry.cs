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
using System.Collections;
using System.Collections.Generic;
using StardewModdingAPI;

namespace RainbowCursor {
    internal class ColorPaletteRegistry : IEnumerable<ColorPalette> {
        private readonly ModEntry theMod;
        private readonly ColorPalette defaultPalette;
        private readonly List<ColorPalette> orderedPalettes = new();
        private readonly Dictionary<string, ColorPalette> palettesById = new();

        public ColorPaletteRegistry(ModEntry theMod, ColorPalette defaultPalette) {
            this.theMod = theMod;
            this.defaultPalette = defaultPalette;
        }

        public void Add(ColorPalette p) {
            if (palettesById.TryGetValue(p.Id, out var conflicting)) {
                theMod.Monitor.Log(
                    I18n.Palette_Conflict(
                        id: p.Id,
                        modName: p.ProvidedBy.Name,
                        conflictingModName: conflicting.ProvidedBy.Name
                    ),
                    LogLevel.Warn);
                return;
            }
            if (p.Colors.Count < 1) {
                theMod.Monitor.Log(I18n.Palette_NoColors(modName: p.ProvidedBy.Name, paletteName: p.GetName(), id: p.Id), LogLevel.Warn);
                return;
            }
            orderedPalettes.Add(p);
            palettesById.Add(p.Id, p);
        }

        public ColorPalette Get(uint pos) {
            if (pos >= orderedPalettes.Count) return defaultPalette;
            return orderedPalettes[(int)pos];
        }

        public ColorPalette Get(string id) {
            if (palettesById.TryGetValue(id, out var result)) {
                return result;
            }
            return defaultPalette;
        }

        public uint IndexOf(string id) {
            var v = orderedPalettes.IndexOf(Get(id));
            if (v < 0) return 0;
            return (uint)v;
        }

        public int Count => orderedPalettes.Count;

        public IEnumerator<ColorPalette> GetEnumerator() {
            return ((IEnumerable<ColorPalette>)orderedPalettes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return ((IEnumerable)orderedPalettes).GetEnumerator();
        }
    }
}

