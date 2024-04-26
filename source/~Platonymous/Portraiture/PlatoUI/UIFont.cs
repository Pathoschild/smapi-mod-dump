/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Platonymous/Stardew-Valley-Mods
**
*************************************************/

using BmFont;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using StardewModdingAPI;
using System.Collections.Generic;
using System.IO;

namespace Portraiture.PlatoUI
{
    public class UIFont
    {
        public virtual string Id { get; set; } = "";
        public virtual FontFile FontFile { get; set; } = null;

        public virtual Dictionary<char, FontChar> CharacterMap { get; set; } = null;

        public virtual List<Texture2D> FontPages { get; set; } = null;

        public UIFont(IModHelper helper, string assetName, string id = "")
        {
            if (id == "")
                id = assetName;

            Id = id;

            FontFile = FontLoader.Parse(File.ReadAllText(Path.Combine(helper.DirectoryPath, assetName)));

            CharacterMap = new Dictionary<char, FontChar>();

            foreach (FontChar fontChar in FontFile.Chars)
            {
                char cid = (char)fontChar.ID;
                CharacterMap.Add(cid, fontChar);
            }

            FontPages = new List<Texture2D>();

            foreach (FontPage page in FontFile.Pages)
                FontPages.Add(helper.ModContent.Load<Texture2D>($"{Path.GetDirectoryName(assetName)}/{page.File}"));
        }
    }
}
