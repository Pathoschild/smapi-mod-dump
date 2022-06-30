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
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Tools;
using StardewValley.Menus;
using System.Collections.Generic;
using System.Reflection;
using StardewValley.Locations;
using Microsoft.Xna.Framework.Graphics;
using SpaceShared.APIs;
using EnhancedSlingshots.Framework.Patch;
using EnhancedSlingshots.Framework.Enchantments;
using Microsoft.Xna.Framework;

namespace EnhancedSlingshots
{
    public class ModEntry : Mod, IAssetEditor
    {
        internal ITranslationHelper i18n => Helper.Translation;
        internal Config config;           
        private Harmony harmony;
        private Texture2D infinitySlingTexture;
        public static ModEntry Instance;
 
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            harmony = new Harmony(ModManifest.UniqueID);
            config = Helper.ReadConfig<Config>();
            InitializeMonitors();
           
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
                api.RegisterSerializerType(typeof(GeminiEnchantment));
                api.RegisterSerializerType(typeof(AutomatedEnchantment));
                api.RegisterSerializerType(typeof(ExpertEnchantment));
                api.RegisterSerializerType(typeof(HunterEnchantment));
                api.RegisterSerializerType(typeof(MinerEnchantment));               
                api.RegisterSerializerType(typeof(PreciseEnchantment));
                api.RegisterSerializerType(typeof(SwiftEnchantment));
                api.RegisterSerializerType(typeof(MagneticEnchantment));
                api.RegisterSerializerType(typeof(Framework.Enchantments.BugKillerEnchantment));
                api.RegisterSerializerType(typeof(Framework.Enchantments.PreservingEnchantment));
                api.RegisterSerializerType(typeof(Framework.Enchantments.VampiricEnchantment));
            }
        }

        public void OnMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (Game1.currentLocation is AdventureGuild && e?.NewMenu is ShopMenu shopMenu)
            {
                if (config.EnableGalaxySligshot && Game1.player.hasSkullKey)
                {
                    Tool slingshot = new Slingshot(Slingshot.galaxySlingshot);
                    shopMenu.itemPriceAndStock.Add(slingshot, new int[2] { config.GalaxySlingshotPrice, 1 });
                    shopMenu.forSale.Insert(0, slingshot);
                }                
            }
        }

        public bool CanEdit<T>(IAssetInfo asset)
        {
            return
                asset.Name.IsEquivalentTo("Data/Weapons") ||
                asset.Name.IsEquivalentTo("TileSheets/weapons");
        }

        public void Edit<T>(IAssetData asset)
        {
            if (asset.Name.IsEquivalentTo("Data/Weapons"))
            {
                IDictionary<int, string> data = asset.AsDictionary<int, string>().Data;
                data.Add(config.InfinitySlingshotId, GetInfinitySlingshotData());             
            }

            if (asset.Name.IsEquivalentTo("TileSheets/weapons"))
            {
                var weapons = asset.AsImage();
                Rectangle area = GetTargetArea(config.InfinitySlingshotId);
                weapons.ExtendImage(minWidth: weapons.Data.Width, minHeight: area.Y+16);
                weapons.PatchImage(infinitySlingTexture, targetArea: area);               
            }
        }
        private Rectangle GetTargetArea(int id)
            => new Rectangle((id%8)*16, (id/8)*16, 16, 16);
        private string GetInfinitySlingshotData()
            => $"Infinity Slingshot/{i18n.Get("InfinitySlingshotDescription")}/1/3/1/308/0/0/4/-1/-1/0/.02/3/{i18n.Get("InfinitySlingshotName")}";
        
        private void InitializeMonitors()
        {
            BaseEnchantmentPatchs.Initialize(Monitor);           
            GameLocationPatchs.Initialize(Monitor);
            ProjectilePatchs.Initialize(Monitor);
            SlingshotPatchs.Initialize(Monitor);
            ToolPatchs.Initialize(Monitor);
        }
    }
}
