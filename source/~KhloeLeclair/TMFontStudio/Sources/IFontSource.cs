/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/KhloeLeclair/StardewMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Leclair.Stardew.ThemeManagerFontStudio.Models;

namespace Leclair.Stardew.ThemeManagerFontStudio.Sources;

public interface IFontSource {

	Task<IFontData?> GetFont(string uniqueId);

	Task<IFontData?> LoadFont(IFontData data);

	IAsyncEnumerable<IFontData> GetAllFonts(IProgress<int> progress);

}
