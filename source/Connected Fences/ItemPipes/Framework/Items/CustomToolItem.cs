/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/sergiomadd/StardewValleyMods
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.Tools;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ItemPipes.Framework.Util;

namespace ItemPipes.Framework.Items
{
    public abstract class CustomToolItem : Tool
    {
        public string IDName { get; set; }
        public string Description { get; set; }
        public Texture2D SpriteTexture { get; set; }
        public string SpriteTexturePath { get; set; }
        public Texture2D ItemTexture { get; set; }
        public string ItemTexturePath { get; set; }
        public CustomToolItem Value { get; set; }

        public CustomToolItem() : base()
        {
            Init();
            this.Stackable = false;
            this.numAttachmentSlots.Value = 0;
            this.InstantUse = true;
            Value = this;
        }
        public void Init()
        {
            IDName = Utilities.GetIDNameFromType(GetType());
            DataAccess DataAccess = DataAccess.GetDataAccess();
            ItemTexture = DataAccess.Sprites[IDName + "_item"];
            Name = DataAccess.ItemNames[IDName];
            DisplayName = DataAccess.ItemNames[IDName];
            Description = DataAccess.ItemDescriptions[IDName];
            parentSheetIndex.Value = DataAccess.ItemIDs[IDName];
        }

        public virtual Tool Save()
        {
            if (!modData.ContainsKey("ItemPipes"))
            {
                modData.Add("ItemPipes", "true");
            }
            else
            {
                modData["ItemPipes"] = "true";
            }
            if (!modData.ContainsKey("Type"))
            {
                modData.Add("Type", IDName);
            }
            else
            {
                modData["Type"] = IDName;
            }
            Axe axe = new Axe();
            axe.modData = modData;
            return axe;
        }

        public virtual void Load(ModDataDictionary data)
        {
            modData = data;
        }

        public override bool CanAddEnchantment(BaseEnchantment enchantment)
        {
            return false;
        }

        public abstract override Item getOne();
        public override string getCategoryName()
        {
            return "Item Pipes";
        }
        public override Color getCategoryColor()
        {
            return Color.Black;
        }

        public override string getDescription()
        {
            return Description;
        }

        protected override string loadDescription()
        {
            return Description;
        }

        protected override string loadDisplayName()
        {
            return Name;
        }
    }
}
