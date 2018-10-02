using Microsoft.Xna.Framework.Graphics;
using Newtonsoft.Json;
using StardewModdingAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTN
{
    public enum fileType
    {
        raw,
        xnb
    }

    /// <summary>
    /// This class is nothing more than to retain information for a specific custom farm map. This is CustomFarmType, lite mode (So we don't have a massive amount of memory being taken).
    /// This is the version CharacterCustomizationWithCustoms will read. 
    /// 
    /// Populated by JsonSerializer
    /// </summary>
    public class CustomFarmEntry
    {
        //Required Information
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Folder { get; set; }
        public string Icon { get; set; }
        public Single version { get; set; }

        [JsonIgnore]
        public Texture2D IconSource { get; set; }
        [JsonIgnore]
        public IContentPack contentpack { get; set; }

        //Multiplayer
        public int cabinCapacity = 3;
        public bool allowClose = true;
        public bool allowSeperate = true;

        public CustomFarmEntry() { }

        public CustomFarmEntry(int ID, string Name, string Description, string Icon, int cabinCapacity, bool allowClose, bool allowSeperate)
        {
            this.ID = ID;
            this.Name = Name;
            this.Description = Description;
            this.Icon = Icon;
            this.cabinCapacity = cabinCapacity;
            this.allowClose = allowClose;
            this.allowSeperate = allowSeperate;
        }
    }
}
