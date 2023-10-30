/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MindMeltMax/Stardew-Valley-Mods
**
*************************************************/

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fishnets.Data
{
    public class FishNetSerializable
    {
        public long Owner { get; set; } = 0L;

        public int Bait { get; set; } = -1;

        public int BaitQuality { get; set; } = 0;

        public string ObjectName { get; set; } = "";

        public int ObjectId { get; set; } = -1;

        public int ObjectStack { get; set; } = -1;

        public int ObjectQuality { get; set; } = 0;

        public bool IsJAObject { get; set; } = false;

        public bool IsDGAObject { get; set; } = false;

        public Vector2 Tile { get; set; }

        public FishNetSerializable() { }

        public FishNetSerializable(Fishnet f)
        {
            Owner = f.owner.Value;
            if (f.bait.Value is not null)
            {
                Bait = f.bait.Value.ParentSheetIndex;
                BaitQuality = f.bait.Value.Quality;
            }
            if (f.heldObject.Value is not null)
            {
                ObjectName = f.heldObject.Value.Name;
                ObjectId = f.heldObject.Value.ParentSheetIndex;
                ObjectStack = f.heldObject.Value.Stack;
                ObjectQuality = f.heldObject.Value.Quality;
                if (ModEntry.HasJsonAssets)
                    IsJAObject = ModEntry.IJsonAssetsApi.GetObjectId(ObjectName) != -1;
                if (ModEntry.HasDynamicGameAssets)
                    IsDGAObject = ModEntry.IDynamicGameAssetsApi.GetDGAItemId(f.heldObject.Value) is not null;

                if (IsDGAObject)
                    ObjectName = ModEntry.IDynamicGameAssetsApi.GetDGAItemId(f.heldObject.Value);
            }
            Tile = f.TileLocation;
        }
    }
}
