/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Extensions;
using Igorious.StardewValley.DynamicAPI.Interfaces;
using Newtonsoft.Json;
using Object = StardewValley.Object;

namespace Igorious.StardewValley.DynamicAPI.Data
{
    public class ItemInformation : IItem, IItemInformation
    {
        #region	Constructors

        [JsonConstructor]
        public ItemInformation() { }

        public ItemInformation(DynamicID<ItemID> id, string name, string description)
        {
            ID = id;
            Name = name;
            Description = description;
        }

        #endregion

        #region	Properties

        [JsonProperty(Required = Required.Always)]
        public DynamicID<ItemID> ID { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Name { get; set; }

        [JsonProperty]
        public int Price { get; set; }

        [JsonProperty, DefaultValue(Object.inedible)]
        public int Edibility { get; set; } = Object.inedible;

        [JsonProperty, DefaultValue(ObjectType.Basic)]
        public ObjectType Type { get; set; } = ObjectType.Basic;

        [JsonProperty]
        public CategoryID Category { get; set; }

        [JsonProperty(Required = Required.Always)]
        public string Description { get; set; }

        [JsonProperty]
        public MealCategory MealCategory { get; set; }

        [JsonProperty]
        public SkillUpInformation SkillUps { get; set; }

        [JsonProperty]
        public int? Duration { get; set; }

        [JsonProperty]
        public List<DayTime> FishDayTime { get; set; }

        [JsonProperty]
        public List<Season> FishSeason { get; set; }

        [JsonProperty]
        public List<ArchChance> ArchChances { get; set; }

        [JsonProperty]
        public string ArchAdditionalInfo { get; set; }

        [JsonProperty]
        public bool IsColored { get; set; }

        [JsonProperty]
        public int? ResourceIndex { get; set; }

        [DefaultValue(1)]
        public int ResourceLength { get; set; } = 1;

        [JsonProperty, DefaultValue(1)]
        public int ResourceHeight { get; set; } = 1;

        [JsonProperty, DefaultValue(1)]
        public int TileWidth { get; set; } = 1;

        [JsonProperty, DefaultValue(1)]
        public int TileHeight { get; set; } = 1;

        #endregion

        #region Serialization

        public static ItemInformation Parse(string objectInformation, int itemID = 0)
        {
            var info = new ItemInformation {ID = itemID};
            var parts = objectInformation.Split('/');
            info.Name = parts[0];
            info.Price = int.Parse(parts[1]);
            info.Edibility = int.Parse(parts[2]);
            var typeAndCategory = parts[3].Split(' ');
            info.Type = typeAndCategory[0].ToEnum<ObjectType>();
            if (typeAndCategory.Length > 1) info.Category = typeAndCategory[1].ToEnum<CategoryID>();
            info.Description = parts[4];
            if (parts.Length > 5)
            {
                if (info.Category == CategoryID.Fish)
                {
                    var dayTimeAndSeasons = parts[5].Split('^');
                    info.FishDayTime = dayTimeAndSeasons[0].Split(' ').Select(d => d.ToEnum<DayTime>()).ToList();
                    info.FishSeason = dayTimeAndSeasons[1].Split(' ').Select(d => d.ToEnum<Season>()).ToList();
                }
                else if (info.Type == ObjectType.Arch)
                {
                    var archChances = parts[5].Split(' ');
                    info.ArchChances = new List<ArchChance>();
                    for (var i = 0; i < archChances.Length; i += 2)
                    {
                        info.ArchChances.Add(new ArchChance
                        {
                            Location = archChances[i],
                            Chance = decimal.Parse(archChances[i + 1]),
                        });
                    }
                    info.ArchAdditionalInfo = parts[6];
                }
                else
                {
                    info.MealCategory = parts[5].ToEnum<MealCategory>();
                    info.SkillUps = SkillUpInformation.Parse(parts[6]);
                    info.Duration = int.Parse(parts[7]);
                }
            }
            return info;
        }

        public override string ToString()
        {
            var buffer = new StringBuilder($"{Name}/{Price}/{Edibility}/");
            if (Type == ObjectType.Interactive)
            {
                buffer.Append(Type.ToLower());
            }
            else
            {
                buffer.Append(Type);
            }

            if (Category != CategoryID.Undefined) buffer.Append(' ').Append((int)Category);
            buffer.Append('/').Append(Description);
            if (Category == CategoryID.Fish)
            {
                buffer.Append('/').Append(string.Join(" ", FishDayTime))
                    .Append('^').Append(string.Join(" ", FishSeason));
            }
            else if (Type == ObjectType.Arch)
            {
                buffer.Append('/').Append(string.Join(" ", ArchChances))
                    .Append('/').Append(ArchAdditionalInfo);
            }
            else if (MealCategory != MealCategory.Undefined)
            {
                buffer.Append('/').Append(MealCategory.ToLower())
                    .Append('/').Append(SkillUps ?? new SkillUpInformation())
                    .Append('/').Append(Duration ?? 0);
            }
            return buffer.ToString();
        }

        #endregion

        #region Explicit Interface Implemetation

        int IDrawable.TextureIndex => ID;

        int IInformation.ID => ID;

        int IItem.ID => ID;

        #endregion
    }
}