/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/YTSC/StardewValleyMods
**
*************************************************/

using HarmonyLib;
using Microsoft.Xna.Framework.Input;
using EnhancedSlingshots.Patch;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Projectiles;
using StardewValley.Tools;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.Reflection;
using StardewValley.Locations;
using Microsoft.Xna.Framework.Graphics;
using System.Text;
using SpaceShared.APIs;
using EnhancedSlingshots.Enchantments;
using Microsoft.Xna.Framework;

namespace EnhancedSlingshots
{
    public class ModEntry : Mod, IAssetEditor
    {
        internal Config config;
        public static ModEntry Instance;       
        private Harmony harmony;
        private Texture2D infinitySlingTexture;
        internal ITranslationHelper i18n => Helper.Translation;
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            harmony = new Harmony(ModManifest.UniqueID);
            
            BaseEnchantmentPatchs.Initialize(Monitor);
            GameLocationPatchs.Initialize(Monitor);
            SlingshotPatchs.Initialize(Monitor);
            ToolPatchs.Initialize(Monitor);

            config = Helper.ReadConfig<Config>();
            infinitySlingTexture = helper.Content.Load<Texture2D>("assets/InfinitySlingshot.png", ContentSource.ModFolder);   
            Helper.Events.Display.MenuChanged += OnMenuChanged;
            Helper.Events.GameLoop.GameLaunched += OnGameLaunched;    
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }     

        public void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            ISpaceCoreApi api = Helper.ModRegistry.GetApi<ISpaceCoreApi>("spacechase0.SpaceCore");
            if (api != null)
            {                
                api.RegisterSerializerType(typeof(AutomatedEnchantment));
                api.RegisterSerializerType(typeof(ExpertEnchantment));
                api.RegisterSerializerType(typeof(HunterEnchantment));
                api.RegisterSerializerType(typeof(MinerEnchantment));
                api.RegisterSerializerType(typeof(PotentEnchantment));
                api.RegisterSerializerType(typeof(PreciseEnchantment));
                api.RegisterSerializerType(typeof(SwiftEnchantment));
                api.RegisterSerializerType(typeof(Enchantments.BugKillerEnchantment));
                api.RegisterSerializerType(typeof(Enchantments.PreservingEnchantment));
                api.RegisterSerializerType(typeof(Enchantments.VampiricEnchantment));
            }
        }

        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e?.NewMenu is ShopMenu shopMenu && Game1.currentLocation is AdventureGuild)
            {
                if (config.EnableGalaxySligshot && Game1.player.hasSkullKey)
                {
                    Tool slingshot = new Slingshot(34);
                    shopMenu.itemPriceAndStock.Add(slingshot, new int[2] { config.GalaxySlingshotPrice, 1 });
                    shopMenu.forSale.Insert(0, slingshot);
                }                
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return
                asset.AssetNameEquals("Data/Weapons") ||
                asset.AssetNameEquals("TileSheets/weapons");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("Data/Weapons"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                data.Add(config.InfinitySlingshotId, GetInfinitySlingshotData());             
            }

            if (asset.AssetNameEquals("TileSheets/weapons"))
            {
                var weapons = asset.AsImage();
                Rectangle area = GetTargetArea(config.InfinitySlingshotId);
                weapons.ExtendImage(minWidth: weapons.Data.Width, minHeight: area.Y+16);
                weapons.PatchImage(infinitySlingTexture, targetArea: area);
            }
        }
        private Rectangle GetTargetArea(int id) => new Rectangle((id%8)*16, (id/8)*16, 16, 16);
        private string GetInfinitySlingshotData()
        {
            return $"Infinity Slingshot/{i18n.Get("InfinitySlingshotDescription")}/1/3/1/308/0/0/4/-1/-1/0/.02/3/{i18n.Get("InfinitySlingshotName")}";
        }

    }
}
