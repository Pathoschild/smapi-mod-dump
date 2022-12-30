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

using Leclair.Stardew.ThemeManager;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Newtonsoft.Json;

namespace Leclair.Stardew.ThemeManagerFontStudio.Models;

public class Theme {

	[JsonIgnore]
	public IThemeManifest? Manifest { get; set; }

	[JsonIgnore]
	public IThemeManager<Theme>? Manager { get; set; }

	[JsonConverter(typeof(VariableSetConverter))]
	public IVariableSet<Color>? ColorVariables { get; set; } = ModEntry.Instance?.TMApi?.CreateVariableSet<Color>();

	[JsonConverter(typeof(VariableSetConverter))]
	public IVariableSet<IManagedAsset<Texture2D>>? TextureVariables { get; set; } = ModEntry.Instance?.TMApi?.CreateTextureVariableSet();


}
