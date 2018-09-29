using System.Diagnostics;
using Microsoft.Xna.Framework;
using StardewValley.Objects;
using Object = StardewValley.Object;

namespace FarmAutomation.Common
{
    [DebuggerDisplay("{Location} : {Object.Name} => {Chest.Name}")]
    public class ConnectedTile
    {
        public Object Object { get; set; }
        public Chest Chest { get; set; }
        public Vector2 Location { get; set; }

        public bool Equals(ConnectedTile obj)
        {
            return Location.Equals(obj?.Location);
        }
    }
}
