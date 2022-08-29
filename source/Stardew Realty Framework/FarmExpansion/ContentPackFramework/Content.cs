/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Prism-99/FarmExpansion
**
*************************************************/

using System;
using System.Collections.Generic;
using StardewModdingAPI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using FarmExpansion.Framework;

using xTile;

namespace ContentPackFramework.ContentPacks
{
    internal class Content
    {
        public ISemanticVersion Format { get; set; }
        internal IContentPack Owner { get; set; }
        public Map ExpansionMap { get; set; }
        public string Requirements { get; set; }
        public bool AddedToFarm { get; set; } = false;
        public bool AddedToDataLocation { get; set; } = false;
        public string MapName { get; set; }
        public string ParentMap { get; set; }
        public string LocationName { get; set; }
        public string LocationDefinition { get; set; }
        public string DisplayName { get; set; }
        public int Cost { get; set; }
        public string Vendor { get; set; }
        public string Description { get; set; }
        public string MailId { get; set; }
        public string MailContent { get; set; }
        public string ThumbnailName { get; set; }
        public Texture2D Thumbnail { get; set; }
        public bool Purchased { get; set; }
        public Rectangle WorldMapLocation { get; set; }
        public Dictionary<string, Tuple<List<Tuple<string, int, int, int, int>>, List<Tuple<string, int, int, int, int>>, List<Tile>>> TilePatches { get; set; }
        //
        //   Tuple<season id,list<Tuple<tolocname,fromx,fromy,tox,toy>>,List<Tuple<fromlocname,fromx,fromy,tox,toy>>, List<Tile>>>
        //                       from exp warps                       to exp warps
    }
}
