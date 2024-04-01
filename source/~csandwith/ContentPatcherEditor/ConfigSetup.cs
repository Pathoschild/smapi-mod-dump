/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/csandwith/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewValley.Menus;
using System.Collections.Generic;

namespace ContentPatcherEditor
{
    public class ConfigSetup
    {
        public string oldKey;
        public Dictionary<Vector2, string> labels = new();
        public TextBox Key;
        public TextBox Default;
        public List<TextBox> AllowValues = new();
        public ClickableTextureComponent DeleteCC;
        public ClickableTextureComponent AllowValuesAddCC;
        public List<ClickableTextureComponent> AllowValuesSubCCs = new();
        public ClickableTextureComponent AllowBlank;
        public ClickableTextureComponent AllowMultiple;
    }
}