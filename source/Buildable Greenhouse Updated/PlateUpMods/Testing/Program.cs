/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Yariazen/YariazenMods
**
*************************************************/

using KitchenData;
using KitchenLib;
using KitchenLib.Customs;
using KitchenLib.References;
using KitchenLib.src.ContentPack;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Testing
{
    public class Main : BaseMod
    {
        internal const string MOD_ID = $"{MOD_AUTHOR}.{MOD_NAME}";
        internal const string MOD_NAME = "Test Mod";
        internal const string MOD_VERSION = "1.0.0";
        internal const string MOD_AUTHOR = "Yariazen";
        internal const string PLATEUP_VERSION = "1.1.2";

        internal static Item Tomato => GetExistingGDO<Item>(ItemReferences.Tomato);
        internal static Item Plate => GetExistingGDO<Item>(ItemReferences.Plate);

        public Main() : base(MOD_ID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, $"{PLATEUP_VERSION}", Assembly.GetExecutingAssembly()) { }

        protected override void Initialise()
        {
            AddGameDataObject<TestItemGroup>();
            ContentPackManager manager;
        }

        private static T1 GetModdedGDO<T1, T2>() where T1 : GameDataObject
        {
            return (T1)GDOUtils.GetCustomGameDataObject<T2>().GameDataObject;
        }

        private static T GetExistingGDO<T>(int id) where T : GameDataObject
        {
            return (T)GDOUtils.GetExistingGDO(id);
        }

        internal class TestItemGroup : CustomItemGroup
        {
            public override string UniqueNameID => "TestItemGroup";
            public override GameObject Prefab => Tomato.Prefab;
            public override ItemCategory ItemCategory => ItemCategory.Generic;
            public override ItemStorage ItemStorageFlags => ItemStorage.StackableFood;
            public override List<ItemGroup.ItemSet> Sets => new List<ItemGroup.ItemSet>()
            {
                new ItemGroup.ItemSet()
                {
                    Max = 2,
                    Min = 2,
                    Items = new List<Item>()
                    {
                        Honey,
                        Plate
                    }
                }
            };

            public override void OnRegister(GameDataObject gdo)
            {
                gdo.name = "Ingredient - Raw Noodles";
            }
        }
    }
}
