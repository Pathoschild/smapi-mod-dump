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

namespace Fishnets.Data
{
    public class FishNetSerializable
    {
        public long Owner { get; set; } = 0L;

        public string Bait { get; set; } = "";

        public int BaitQuality { get; set; } = 0;

        public string ObjectName { get; set; } = "";

        public string ObjectId { get; set; } = "";

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
                Bait = f.bait.Value.QualifiedItemId;
                BaitQuality = f.bait.Value.Quality;
            }
            if (f.heldObject.Value is not null)
            {
                ObjectName = f.heldObject.Value.Name;
                ObjectId = f.heldObject.Value.QualifiedItemId;
                ObjectStack = f.heldObject.Value.Stack;
                ObjectQuality = f.heldObject.Value.Quality;
                if (ModEntry.HasJsonAssets)
                    IsJAObject = !string.IsNullOrWhiteSpace(ModEntry.IJsonAssetsApi.GetObjectId(ObjectName));
                if (ModEntry.HasDynamicGameAssets)
                    IsDGAObject = ModEntry.IDynamicGameAssetsApi.GetDGAItemId(f.heldObject.Value) is not null;

                if (IsDGAObject)
                    ObjectName = ModEntry.IDynamicGameAssetsApi.GetDGAItemId(f.heldObject.Value);
            }
            Tile = f.TileLocation;
        }
    }
}
