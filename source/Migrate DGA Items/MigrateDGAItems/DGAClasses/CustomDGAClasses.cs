/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/elizabethcd/MigrateDGAItems
**
*************************************************/

using System;
using System.Xml.Serialization;
using StardewValley;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace MigrateDGAItems.DGAClasses
{
    [XmlType("Mods_DGABasicFurniture")]
    public class CustomBasicFurniture: Furniture
    {
        public CustomBasicFurniture()
        {
        }
    }

    [XmlType("Mods_DGABedFurniture")]
    public class CustomBedFurniture : BedFurniture
    {
        public CustomBedFurniture()
        {
        }
    }

    [XmlType("Mods_DGABigCraftable")]
    public class CustomBigCraftable : StardewValley.Object
    {
        public CustomBigCraftable()
        {
        }
    }

    [XmlType("Mods_DGABoots")]
    public class CustomBoots : Boots
    {
        public CustomBoots()
        {
        }
    }

    [XmlType("Mods_DGACrop")]
    public class CustomCrop : Crop
    {
        public CustomCrop()
        {
        }
    }

    [XmlType("Mods_DGAFence")]
    public class CustomFence : Fence
    {
        public CustomFence()
        {
        }
    }

    [XmlType("Mods_DGAFishTankFurniture")]
    public class CustomFishTankFurniture : FishTankFurniture
    {
        public CustomFishTankFurniture()
        {
        }
    }

    [XmlType("Mods_DGAFruitTree")]
    public class CustomFruitTree : FruitTree
    {
        public CustomFruitTree()
        {
        }
    }

    [XmlType("Mods_DGAGiantCrop")]
    public class CustomGiantCrop : GiantCrop
    {
        public CustomGiantCrop()
        {
        }
    }

    [XmlType("Mods_DGAHat")]
    public class CustomHat : Hat
    {
        public CustomHat()
        {
        }
    }

    [XmlType("Mods_DGAMeleeWeapon")]
    public class CustomMeleeWeapon : MeleeWeapon
    {
        public CustomMeleeWeapon()
        {
        }
    }

    [XmlType("Mods_DGAObject")]
    public class CustomObject : StardewValley.Object
    {
        public CustomObject()
        {
        }
    }

    [XmlType("Mods_DGAPants")]
    public class CustomPants : Clothing
    {
        public CustomPants()
        {
        }
    }

    [XmlType("Mods_DGAShirt")]
    public class CustomShirt : Clothing
    {
        public CustomShirt()
        {
        }
    }

    [XmlType("Mods_DGAStorageFurniture")]
    public partial class CustomStorageFurniture : StorageFurniture
    {
        public CustomStorageFurniture()
        {
        }
    }

    [XmlType("Mods_DGATVFurniture")]
    public partial class CustomTVFurniture : TV
    {
        public CustomTVFurniture()
        {
        }
    }
}

