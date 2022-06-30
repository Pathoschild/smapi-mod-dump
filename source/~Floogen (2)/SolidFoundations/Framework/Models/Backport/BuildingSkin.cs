/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Floogen/SolidFoundations
**
*************************************************/

using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;

namespace SolidFoundations.Framework.Models.Backport
{
    // TODO: When updated to SDV v1.6, this class should be deleted in favor of StardewValley.GameData.BuildingSkin
    public class BuildingSkin
    {
        public string ID;

        [ContentSerializer(Optional = true)]
        public string Name;

        [ContentSerializer(Optional = true)]
        public string Description;

        public string Texture;

        [ContentSerializer(Optional = true)]
        public Dictionary<string, string> Metadata = new Dictionary<string, string>();
    }
}