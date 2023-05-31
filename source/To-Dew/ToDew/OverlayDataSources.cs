/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/jltaylor-us/StardewToDew
**
*************************************************/

// Copyright 2023 Jamie Taylor
using System;
using System.Collections.Generic;

namespace ToDew {
    public class OverlayDataSources {
        private readonly SortedDictionary<ulong, IToDewOverlayDataSource> sources = new();
        private ulong NextId = 0;
        public event Action? OnRefresh;

        public OverlayDataSources() {
        }

        public IEnumerable<IToDewOverlayDataSource> DataSources => sources.Values;

        public ulong Add(IToDewOverlayDataSource source) {
            sources.Add(NextId, source);
            OnRefresh?.Invoke();
            return NextId++;
        }

        public void Remove(ulong id) {
            sources.Remove(id);
            OnRefresh?.Invoke();
        }

        public void Refresh() {
            OnRefresh?.Invoke();
        }
    }
}

