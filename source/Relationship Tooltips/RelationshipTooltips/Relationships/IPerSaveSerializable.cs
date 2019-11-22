using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
namespace RelationshipTooltips.Relationships
{
    public interface IPerSaveSerializable
    {
        Action<IModHelper> SaveData { get; }
        Action<IModHelper> LoadData { get; }
    }
}
