/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/spacechase0/JsonAssets
**
*************************************************/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Harmony;
using JsonAssets.Data;
using JsonAssets.Other.ContentPatcher;
using JsonAssets.Overrides;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Netcode;
using Newtonsoft.Json;
using SpaceCore;
using SpaceCore.Events;
using SpaceShared;
using SpaceShared.APIs;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Buildings;
using StardewValley.Characters;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.Network;
using StardewValley.Objects;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;
using SObject = StardewValley.Object;

// TODO: Refactor recipes

namespace JsonAssets
{
    public class Mod : StardewModdingAPI.Mod
    {
        public static Mod instance;
        private HarmonyInstance harmony;
        private ContentInjector1 content1;
        private ContentInjector2 content2;
        internal ExpandedPreconditionsUtilityAPI epu;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            instance = this;
            Log.Monitor = Monitor;

            helper.ConsoleCommands.Add( "ja_summary", "Summary of JA ids", doCommands );
            helper.ConsoleCommands.Add( "ja_unfix", "Unfix IDs once, in case IDs were double fixed.", doCommands );

            helper.Events.Display.MenuChanged += onMenuChanged;
            helper.Events.GameLoop.Saving += onSaving;
            helper.Events.Player.InventoryChanged += onInventoryChanged;
            helper.Events.GameLoop.GameLaunched += onGameLaunched;
            helper.Events.GameLoop.SaveCreated += onCreated;
            helper.Events.GameLoop.UpdateTicked += onTick;
            helper.Events.Specialized.LoadStageChanged += onLoadStageChanged;
            helper.Events.Multiplayer.PeerContextReceived += clientConnected;

            helper.Content.AssetEditors.Add(content1 = new ContentInjector1());
            helper.Content.AssetLoaders.Add( content1 );

            SpaceCore.TileSheetExtensions.RegisterExtendedTileSheet( "Maps\\springobjects", 16 );
            SpaceCore.TileSheetExtensions.RegisterExtendedTileSheet("TileSheets\\Craftables", 32);
            SpaceCore.TileSheetExtensions.RegisterExtendedTileSheet("TileSheets\\crops", 32);
            SpaceCore.TileSheetExtensions.RegisterExtendedTileSheet("TileSheets\\fruitTrees", 80);
            SpaceCore.TileSheetExtensions.RegisterExtendedTileSheet("Characters\\Farmer\\shirts", 32);
            SpaceCore.TileSheetExtensions.RegisterExtendedTileSheet("Characters\\Farmer\\pants", 688);
            SpaceCore.TileSheetExtensions.RegisterExtendedTileSheet("Characters\\Farmer\\hats", 80);

            try
            {
                harmony = HarmonyInstance.Create("spacechase0.JsonAssets");

                // object patches
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.canBePlacedHere)),
                    prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.CanBePlacedHere_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.checkForAction)),
                    prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.CheckForAction_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), "loadDisplayName"),
                    prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.LoadDisplayName_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.getCategoryName)),
                    prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.GetCategoryName_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.isIndexOkForBasicShippedCategory)),
                    postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.IsIndexOkForBasicShippedCategory_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.getCategoryColor)),
                    prefix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.GetCategoryColor_Prefix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(SObject), nameof(SObject.canBeGivenAsGift)),
                    postfix: new HarmonyMethod(typeof(ObjectPatches), nameof(ObjectPatches.CanBeGivenAsGift_Postfix))
                );

                // ring patches
                harmony.Patch(
                    original: AccessTools.Method(typeof(Ring), "loadDisplayFields"),
                    prefix: new HarmonyMethod(typeof(RingPatches), nameof(RingPatches.LoadDisplayFields_Prefix))
                );

                // crop patches
                harmony.Patch(
                    original: AccessTools.Method(typeof(Crop), nameof(Crop.isPaddyCrop)),
                    prefix: new HarmonyMethod(typeof(CropPatches), nameof(CropPatches.IsPaddyCrop_Prefix))
                );

                harmony.Patch(
                    original: AccessTools.Method(typeof(Crop), nameof(Crop.newDay)),
                    transpiler: new HarmonyMethod(typeof(CropPatches), nameof(CropPatches.NewDay_Transpiler))
                );

                // GiantCrop patches
                harmony.Patch(
                    original: AccessTools.Method(typeof(GiantCrop), nameof(GiantCrop.draw)),
                    prefix: new HarmonyMethod(typeof(GiantCropPatches), nameof(GiantCropPatches.Draw_Prefix))
                );

                // item patches
                harmony.Patch(
                    original: AccessTools.Method(typeof(Item), nameof(Item.canBeDropped)),
                    postfix: new HarmonyMethod(typeof(ItemPatches), nameof(ItemPatches.CanBeDropped_Postfix))
                );
                harmony.Patch(
                    original: AccessTools.Method(typeof(Item), nameof(Item.canBeTrashed)),
                    postfix: new HarmonyMethod(typeof(ItemPatches), nameof(ItemPatches.CanBeTrashed_Postfix))
                );

                harmony.Patch( original: AccessTools.Constructor( typeof( Fence ), new Type[] { typeof( Vector2 ), typeof( int ), typeof( bool ) } ),
                    postfix: new HarmonyMethod( typeof( FenceConstructorPatch ), nameof( FenceConstructorPatch.Postfix ) ) );
                
                harmony.Patch(
                    original: AccessTools.Method(typeof(ForgeMenu), nameof(ForgeMenu.draw), new Type[] { typeof( SpriteBatch ) } ),
                    transpiler: new HarmonyMethod(typeof( ForgeDrawCostPatch ), nameof( ForgeDrawCostPatch .Transpiler) )
                );

                harmony.PatchAll();
            }
            catch (Exception e)
            {
                Log.error($"Exception doing harmony stuff: {e}");
            }
        }

        private Api api;
        public override object GetApi()
        {
            return api ?? (api = new Api(this.loadData));
        }

        private Dictionary<string, KeyValuePair<int, int>> MakeIdMapping(IDictionary<string, int> oldIds, IDictionary<string, int> newIds )
        {
            var ret = new Dictionary<string, KeyValuePair< int, int > >();
            if ( oldIds != null )
            {
                foreach ( var oldId in oldIds )
                {
                    ret.Add( oldId.Key, new KeyValuePair<int, int>( oldId.Value, -1 ) );
                }
            }
            foreach ( var newId in newIds )
            {
                if ( ret.ContainsKey( newId.Key ) )
                    ret[ newId.Key ] = new KeyValuePair<int, int>( ret[ newId.Key ].Key, newId.Value );
                else
                    ret.Add( newId.Key, new KeyValuePair<int, int>( -1, newId.Value ) );
            }
            return ret;
        }

        private void PrintIdMapping( string header, Dictionary<string, KeyValuePair<int, int>> mapping )
        {
            Log.info( header );
            Log.info( "-------------------------" );

            int len = 0;
            foreach ( var entry in mapping )
                len = Math.Max( len, entry.Key.Length );

            foreach ( var entry in mapping )
            {
                Log.info( string.Format( "{0,-" + len + "} | {1,5} -> {2,-5}",
                                          entry.Key,
                                          entry.Value.Key == -1 ? "" : entry.Value.Key.ToString(),
                                          entry.Value.Value == -1 ? "" : entry.Value.Value.ToString() ) );
            }
            Log.info( "" );
        }

        private void doCommands( string cmd, string[] args )
        {
            if ( !didInit )
            {
                Log.info( "A save must be loaded first." );
                return;
            }

            if ( cmd == "ja_summary" )
            {
                var objs = MakeIdMapping( oldObjectIds, objectIds );
                var crops = MakeIdMapping( oldCropIds, cropIds );
                var ftrees = MakeIdMapping( oldFruitTreeIds, fruitTreeIds );
                var bigs = MakeIdMapping( oldBigCraftableIds, bigCraftableIds);
                var hats = MakeIdMapping( oldHatIds, hatIds );
                var weapons = MakeIdMapping( oldWeaponIds, weaponIds );
                var clothings = MakeIdMapping( oldClothingIds, clothingIds );

                PrintIdMapping( "Object IDs", objs );
                PrintIdMapping( "Crop IDs", crops );
                PrintIdMapping( "Fruit Tree IDs", ftrees );
                PrintIdMapping( "Big Craftable IDs", bigs );
                PrintIdMapping( "Hat IDs", hats );
                PrintIdMapping( "Weapon IDs", weapons );
                PrintIdMapping( "Clothing IDs", clothings );
            }
            else if ( cmd == "ja_unfix" )
            {
                locationsFixedAlready.Clear();
                fixIdsEverywhere( reverse: true );
            }
        }

        private void onGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            epu = Helper.ModRegistry.GetApi<ExpandedPreconditionsUtilityAPI>( "Cherry.ExpandedPreconditionsUtility" );
            epu.Initialize( false, ModManifest.UniqueID );

            ContentPatcherIntegration.Initialize();
        }

        bool firstTick = true;
        private void onTick(object sender, UpdateTickedEventArgs e)
        {
            // This needs to run after GameLaunched, because of the event 
            if (firstTick)
            {
                firstTick = false;

                Log.info("Loading content packs...");
                foreach (IContentPack contentPack in this.Helper.ContentPacks.GetOwned())
                    try
                    {
                        loadData(contentPack);
                    }
                    catch (Exception e1)
                    {
                        Log.error("Exception loading content pack: " + e1);
                    }
                if (Directory.Exists(Path.Combine(Helper.DirectoryPath, "ContentPacks")))
                {
                    foreach (string dir in Directory.EnumerateDirectories(Path.Combine(Helper.DirectoryPath, "ContentPacks")))
                        try
                        {
                            loadData(dir);
                        }
                        catch (Exception e2)
                        {
                            Log.error("Exception loading content pack: " + e2);
                        }
                }
                api.InvokeItemsRegistered();

                resetAtTitle();
            }

        }

        private static readonly Regex nameToId = new Regex("[^a-zA-Z0-9_.]");
        private void loadData(string dir)
        {
            // read initial info
            IContentPack temp = this.Helper.ContentPacks.CreateFake(dir);
            ContentPackData info = temp.ReadJsonFile<ContentPackData>("content-pack.json");
            if (info == null)
            {
                Log.warn($"\tNo {dir}/content-pack.json!");
                return;
            }

            // load content pack
            string id = nameToId.Replace(info.Name, "");
            IContentPack contentPack = this.Helper.ContentPacks.CreateTemporary(dir, id: id, name: info.Name, description: info.Description, author: info.Author, version: new SemanticVersion(info.Version));
            this.loadData(contentPack);
        }

        internal Dictionary<IManifest, List<string>> objectsByContentPack = new Dictionary<IManifest, List<string>>();
        internal Dictionary<IManifest, List<string>> cropsByContentPack = new Dictionary<IManifest, List<string>>();
        internal Dictionary<IManifest, List<string>> fruitTreesByContentPack = new Dictionary<IManifest, List<string>>();
        internal Dictionary<IManifest, List<string>> bigCraftablesByContentPack = new Dictionary<IManifest, List<string>>();
        internal Dictionary<IManifest, List<string>> hatsByContentPack = new Dictionary<IManifest, List<string>>();
        internal Dictionary<IManifest, List<string>> weaponsByContentPack = new Dictionary<IManifest, List<string>>();
        internal Dictionary<IManifest, List<string>> clothingByContentPack = new Dictionary<IManifest, List<string>>();
        internal Dictionary<IManifest, List<string>> bootsByContentPack = new Dictionary<IManifest, List<string>>();

        public void RegisterObject(IManifest source, ObjectData obj)
        {
            objects.Add(obj);

            if ( obj.Recipe != null && obj.Recipe.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = obj.Recipe.PurchaseFrom,
                    Price = obj.Recipe.PurchasePrice,
                    PurchaseRequirements = obj.Recipe.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", obj.Recipe.PurchaseRequirements?.ToArray() ) },
                    Object = () => new StardewValley.Object( obj.id, 1, true, obj.Recipe.PurchasePrice, 0 ),
                } );
                if ( obj.Recipe.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in obj.Recipe.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new StardewValley.Object( obj.id, 1, true, entry.PurchasePrice, 0 ),
                        } );
                    }
                }
            }
            if ( obj.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = obj.PurchaseFrom,
                    Price = obj.PurchasePrice,
                    PurchaseRequirements = obj.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", obj.PurchaseRequirements?.ToArray() ) },
                    Object = () => new StardewValley.Object( obj.id, int.MaxValue, false, obj.PurchasePrice, 0 ),
                } );
                if ( obj.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in obj.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new StardewValley.Object( obj.id, int.MaxValue, false, entry.PurchasePrice, 0 ),
                        } );
                    }
                }
            }

            // save ring
            if (obj.Category == ObjectData.Category_.Ring)
                this.myRings.Add(obj);

            // Duplicate check
            if (dupObjects.ContainsKey(obj.Name))
                Log.error($"Duplicate object: {obj.Name} just added by {source.Name}, already added by {dupObjects[obj.Name].Name}!");
            else
                dupObjects[obj.Name] = source;

            if (!objectsByContentPack.ContainsKey(source))
                objectsByContentPack.Add(source, new List<string>());
            objectsByContentPack[source].Add(obj.Name);
        }

        public void RegisterCrop(IManifest source, CropData crop, Texture2D seedTex)
        {
            crops.Add(crop);

            // save seeds
            crop.seed = new ObjectData
            {
                texture = seedTex,
                Name = crop.SeedName,
                Description = crop.SeedDescription,
                Category = ObjectData.Category_.Seeds,
                Price = crop.SeedSellPrice == -1 ? crop.SeedPurchasePrice : crop.SeedSellPrice,
                CanPurchase = crop.SeedPurchasePrice > 0,
                PurchaseFrom = crop.SeedPurchaseFrom,
                PurchasePrice = crop.SeedPurchasePrice,
                PurchaseRequirements = crop.SeedPurchaseRequirements ?? new List<string>(),
                AdditionalPurchaseData = crop.SeedAdditionalPurchaseData ?? new List<PurchaseData>(),
                NameLocalization = crop.SeedNameLocalization,
                DescriptionLocalization = crop.SeedDescriptionLocalization
            };

            // TODO: Clean up this chunk
            // I copy/pasted it from the unofficial update decompiled
            string str = "";
            string[] array = new[] { "spring", "summer", "fall", "winter" }
                .Except(crop.Seasons)
                .ToArray();
            foreach (var season in array)
            {
                str += $"/z {season}";
            }
            if (str != "")
            {
                string strtrimstart = str.TrimStart(new char[] { '/' });
                if (crop.SeedPurchaseRequirements != null && crop.SeedPurchaseRequirements.Count > 0)
                {
                    for (int index = 0; index < crop.SeedPurchaseRequirements.Count; index++)
                    {
                        if (SeasonLimiter.IsMatch(crop.SeedPurchaseRequirements[index]))
                        {
                            crop.SeedPurchaseRequirements[index] = strtrimstart;
                            Log.warn($"        Faulty season requirements for {crop.SeedName}!\n        Fixed season requirements: {crop.SeedPurchaseRequirements[index]}");
                        }
                    }
                    if (!crop.SeedPurchaseRequirements.Contains(str.TrimStart('/')))
                    {
                        Log.trace($"        Adding season requirements for {crop.SeedName}:\n        New season requirements: {strtrimstart}");
                        crop.seed.PurchaseRequirements.Add(strtrimstart);
                    }
                }
                else
                {
                    Log.trace($"        Adding season requirements for {crop.SeedName}:\n        New season requirements: {strtrimstart}");
                    crop.seed.PurchaseRequirements.Add(strtrimstart);
                }
            }
            
            if ( crop.seed.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = crop.seed.PurchaseFrom,
                    Price = crop.seed.PurchasePrice,
                    PurchaseRequirements = crop.seed.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", crop.seed.PurchaseRequirements?.ToArray() ) },
                    Object = () => new StardewValley.Object( crop.seed.id, int.MaxValue, false, crop.seed.PurchasePrice ),
                    ShowWithStocklist = true,
                } );
                if ( crop.seed.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in crop.seed.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new StardewValley.Object( crop.seed.id, int.MaxValue, false, entry.PurchasePrice, 0 ),
                        } );
                    }
                }
            }

            // Duplicate check
            if (dupCrops.ContainsKey(crop.Name))
                Log.error($"Duplicate crop: {crop.Name} just added by {source.Name}, already added by {dupCrops[crop.Name].Name}!");
            else
                dupCrops[crop.Name] = source;

            objects.Add(crop.seed);

            if (!cropsByContentPack.ContainsKey(source))
                cropsByContentPack.Add(source, new List<string>());
            cropsByContentPack[source].Add(crop.Name);

            if (!objectsByContentPack.ContainsKey(source))
                objectsByContentPack.Add(source, new List<string>());
            objectsByContentPack[source].Add(crop.seed.Name);
        }

        public void RegisterFruitTree(IManifest source, FruitTreeData tree, Texture2D saplingTex)
        {
            fruitTrees.Add(tree);

            // save seed
            tree.sapling = new ObjectData
            {
                texture = saplingTex,
                Name = tree.SaplingName,
                Description = tree.SaplingDescription,
                Category = ObjectData.Category_.Seeds,
                Price = tree.SaplingPurchasePrice,
                CanPurchase = true,
                PurchaseRequirements = tree.SaplingPurchaseRequirements,
                PurchaseFrom = tree.SaplingPurchaseFrom,
                PurchasePrice = tree.SaplingPurchasePrice,
                AdditionalPurchaseData = tree.SaplingAdditionalPurchaseData,
                NameLocalization = tree.SaplingNameLocalization,
                DescriptionLocalization = tree.SaplingDescriptionLocalization
            };
            objects.Add(tree.sapling);

            if ( tree.sapling.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = tree.sapling.PurchaseFrom,
                    Price = tree.sapling.PurchasePrice,
                    PurchaseRequirements = tree.sapling.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", tree.sapling.PurchaseRequirements?.ToArray() ) },
                    Object = () => new StardewValley.Object( Vector2.Zero, tree.sapling.id, int.MaxValue ),
                } );
                if ( tree.sapling.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in tree.sapling.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new StardewValley.Object( tree.sapling.id, 1, true, tree.sapling.PurchasePrice, 0 ),
                        } );
                    }
                }
            }

            // Duplicate check
            if (dupFruitTrees.ContainsKey(tree.Name))
                Log.error($"Duplicate fruit tree: {tree.Name} just added by {source.Name}, already added by {dupFruitTrees[tree.Name].Name}!");
            else
                dupFruitTrees[tree.Name] = source;

            if (!fruitTreesByContentPack.ContainsKey(source))
                fruitTreesByContentPack.Add(source, new List<string>());
            fruitTreesByContentPack[source].Add(tree.Name);
        }

        public void RegisterBigCraftable(IManifest source, BigCraftableData craftable)
        {
            bigCraftables.Add(craftable);

            if ( craftable.Recipe != null && craftable.Recipe.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = craftable.Recipe.PurchaseFrom,
                    Price = craftable.Recipe.PurchasePrice,
                    PurchaseRequirements = craftable.Recipe.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", craftable.Recipe.PurchaseRequirements?.ToArray() ) },
                    Object = () => new StardewValley.Object( Vector2.Zero, craftable.id, true ),
                } );
                if ( craftable.Recipe.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in craftable.Recipe.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new StardewValley.Object( Vector2.Zero, craftable.id, true ),
                        } );
                    }
                }
            }
            if ( craftable.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = craftable.PurchaseFrom,
                    Price = craftable.PurchasePrice,
                    PurchaseRequirements = craftable.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", craftable.PurchaseRequirements?.ToArray() ) },
                    Object = () => new StardewValley.Object( Vector2.Zero, craftable.id, false ),
                } );
                if ( craftable.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in craftable.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new StardewValley.Object( Vector2.Zero, craftable.id, false ),
                        } );
                    }
                }
            }

            // Duplicate check
            if (dupBigCraftables.ContainsKey(craftable.Name))
                Log.error($"Duplicate big craftable: {craftable.Name} just added by {source.Name}, already added by {dupBigCraftables[craftable.Name].Name}!");
            else
                dupBigCraftables[craftable.Name] = source;

            if (!bigCraftablesByContentPack.ContainsKey(source))
                bigCraftablesByContentPack.Add(source, new List<string>());
            bigCraftablesByContentPack[source].Add(craftable.Name);
        }

        public void RegisterHat(IManifest source, HatData hat)
        {
            hats.Add(hat);

            if ( hat.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = "HatMouse",
                    Price = hat.PurchasePrice,
                    PurchaseRequirements = new string[ 0 ],
                    Object = () => new Hat( hat.id ),
                } );
            }

            // Duplicate check
            if (dupHats.ContainsKey(hat.Name))
                Log.error($"Duplicate hat: {hat.Name} just added by {source.Name}, already added by {dupHats[hat.Name].Name}!");
            else
                dupHats[hat.Name] = source;

            if (!hatsByContentPack.ContainsKey(source))
                hatsByContentPack.Add(source, new List<string>());
            hatsByContentPack[source].Add(hat.Name);
        }

        public void RegisterWeapon(IManifest source, WeaponData weapon)
        {
            weapons.Add(weapon);

            if ( weapon.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = weapon.PurchaseFrom,
                    Price = weapon.PurchasePrice,
                    PurchaseRequirements = weapon.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", weapon.PurchaseRequirements?.ToArray() ) },
                    Object = () => new MeleeWeapon(weapon.id)
                } );
                if ( weapon.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in weapon.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new MeleeWeapon( weapon.id )
                        } );
                    }
                }
            }

            // Duplicate check
            if (dupWeapons.ContainsKey(weapon.Name))
                Log.error($"Duplicate weapon: {weapon.Name} just added by {source.Name}, already added by {dupWeapons[weapon.Name].Name}!");
            else
                dupWeapons[weapon.Name] = source;

            if (!weaponsByContentPack.ContainsKey(source))
                weaponsByContentPack.Add(source, new List<string>());
            weaponsByContentPack[source].Add(weapon.Name);
        }

        public void RegisterShirt(IManifest source, ShirtData shirt)
        {
            shirts.Add(shirt);

            // Duplicate check
            if (dupShirts.ContainsKey(shirt.Name))
                Log.error($"Duplicate shirt: {shirt.Name} just added by {source.Name}, already added by {dupShirts[shirt.Name].Name}!");
            else
                dupShirts[shirt.Name] = source;

            if (!clothingByContentPack.ContainsKey(source))
                clothingByContentPack.Add(source, new List<string>());
            clothingByContentPack[source].Add(shirt.Name);
        }

        public void RegisterPants(IManifest source, PantsData pants)
        {
            pantss.Add(pants);

            // Duplicate check
            if (dupPants.ContainsKey(pants.Name))
                Log.error($"Duplicate pants: {pants.Name} just added by {source.Name}, already added by {dupPants[pants.Name].Name}!");
            else
                dupPants[pants.Name] = source;

            if (!clothingByContentPack.ContainsKey(source))
                clothingByContentPack.Add(source, new List<string>());
            clothingByContentPack[source].Add(pants.Name);
        }

        public void RegisterTailoringRecipe(IManifest source, TailoringRecipeData recipe)
        {
            tailoring.Add(recipe);
        }

        public void RegisterBoots(IManifest source, BootsData boots)
        {
            bootss.Add(boots);
            
            if ( boots.CanPurchase )
            {
                shopData.Add( new ShopDataEntry()
                {
                    PurchaseFrom = boots.PurchaseFrom,
                    Price = boots.PurchasePrice,
                    PurchaseRequirements = boots.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", boots.PurchaseRequirements?.ToArray() ) },
                    Object = () => new Boots( boots.id )
                } );

                if ( boots.AdditionalPurchaseData != null )
                {
                    foreach ( var entry in boots.AdditionalPurchaseData )
                    {
                        shopData.Add( new ShopDataEntry()
                        {
                            PurchaseFrom = entry.PurchaseFrom,
                            Price = entry.PurchasePrice,
                            PurchaseRequirements = entry.PurchaseRequirements == null ? new string[ 0 ] : new string[] { string.Join( "/", entry.PurchaseRequirements?.ToArray() ) },
                            Object = () => new Boots( boots.id )
                        } );
                    }
                }
            }

            // Duplicate check
            if (dupBoots.ContainsKey(boots.Name))
                Log.error($"Duplicate boots: {boots.Name} just added by {source.Name}, already added by {dupBoots[boots.Name].Name}!");
            else
                dupBoots[boots.Name] = source;

            if (!bootsByContentPack.ContainsKey(source))
                bootsByContentPack.Add(source, new List<string>());
            bootsByContentPack[source].Add(boots.Name);
        }

        public void RegisterForgeRecipe( IManifest source, ForgeRecipeData recipe )
        {
            forge.Add( recipe );
        }

        public void RegisterFence(IManifest source, FenceData fence)
        {
            fences.Add( fence );

            Func<IList<FenceData.Recipe_.Ingredient>, IList<ObjectData.Recipe_.Ingredient>> convertIngredients = (ingredients) =>
            {
                var ret = new List<ObjectData.Recipe_.Ingredient>();
                foreach ( var ingred in ingredients )
                {
                    ret.Add( new ObjectData.Recipe_.Ingredient()
                    {
                        Object = ingred.Object,
                        Count = ingred.Count,
                    } );
                }
                return ret;
            };

            RegisterObject( source, fence.correspondingObject = new ObjectData()
            {
                texture = fence.objectTexture,
                Name = fence.Name,
                Description = fence.Description,
                Category = ObjectData.Category_.Crafting,
                Price = fence.Price,
                Recipe = fence.Recipe == null ? null : new ObjectData.Recipe_()
                {
                    SkillUnlockName = fence.Recipe.SkillUnlockName,
                    SkillUnlockLevel = fence.Recipe.SkillUnlockLevel,
                    ResultCount = fence.Recipe.ResultCount,
                    Ingredients = convertIngredients( fence.Recipe.Ingredients ),
                    IsDefault = fence.Recipe.IsDefault,
                    CanPurchase = fence.Recipe.CanPurchase,
                    PurchasePrice = fence.Recipe.PurchasePrice,
                    PurchaseFrom = fence.Recipe.PurchaseFrom,
                    PurchaseRequirements = fence.Recipe.PurchaseRequirements,
                    AdditionalPurchaseData = fence.Recipe.AdditionalPurchaseData,
                },
                CanPurchase = fence.CanPurchase,
                PurchasePrice = fence.PurchasePrice,
                PurchaseFrom = fence.PurchaseFrom,
                PurchaseRequirements = fence.PurchaseRequirements,
                AdditionalPurchaseData = fence.AdditionalPurchaseData,
                NameLocalization = fence.NameLocalization,
                DescriptionLocalization = fence.DescriptionLocalization,
            } );
        }

        private Dictionary<string, IManifest> dupObjects = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupCrops = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupFruitTrees = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupBigCraftables = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupHats = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupWeapons = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupShirts = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupPants = new Dictionary<string, IManifest>();
        private Dictionary<string, IManifest> dupBoots = new Dictionary<string, IManifest>();

        private readonly Regex SeasonLimiter = new Regex("(z(?: spring| summer| fall| winter){2,4})", RegexOptions.IgnoreCase | RegexOptions.Compiled);
        private void loadData(IContentPack contentPack)
        {
            Log.info($"\t{contentPack.Manifest.Name} {contentPack.Manifest.Version} by {contentPack.Manifest.Author} - {contentPack.Manifest.Description}");

            // load objects
            DirectoryInfo objectsDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Objects"));
            if (objectsDir.Exists)
            {
                foreach (DirectoryInfo dir in objectsDir.EnumerateDirectories())
                {
                    string relativePath = $"Objects/{dir.Name}";

                    // load data
                    ObjectData obj = contentPack.ReadJsonFile<ObjectData>($"{relativePath}/object.json");
                    if (obj == null || (obj.DisableWithMod != null && Helper.ModRegistry.IsLoaded(obj.DisableWithMod)) || (obj.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(obj.EnableWithMod)))
                        continue;

                    // save object
                    obj.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/object.png");
                    if (obj.IsColored)
                        obj.textureColor = contentPack.LoadAsset<Texture2D>($"{relativePath}/color.png");

                    RegisterObject(contentPack.Manifest, obj);
                }
            }

            // load crops
            DirectoryInfo cropsDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Crops"));
            if (cropsDir.Exists)
            {
                foreach (DirectoryInfo dir in cropsDir.EnumerateDirectories())
                {
                    string relativePath = $"Crops/{dir.Name}";

                    // load data
                    CropData crop = contentPack.ReadJsonFile<CropData>($"{relativePath}/crop.json");
                    if (crop == null || (crop.DisableWithMod != null && Helper.ModRegistry.IsLoaded(crop.DisableWithMod)) || (crop.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(crop.EnableWithMod)))
                        continue;

                    // save crop
                    crop.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/crop.png");
                    if (contentPack.HasFile($"{relativePath}/giant.png"))
                        crop.giantTex = contentPack.LoadAsset<Texture2D>($"{relativePath}/giant.png");

                    RegisterCrop(contentPack.Manifest, crop, contentPack.LoadAsset<Texture2D>($"{relativePath}/seeds.png"));
                }
            }

            // load fruit trees
            DirectoryInfo fruitTreesDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "FruitTrees"));
            if (fruitTreesDir.Exists)
            {
                foreach (DirectoryInfo dir in fruitTreesDir.EnumerateDirectories())
                {
                    string relativePath = $"FruitTrees/{dir.Name}";

                    // load data
                    FruitTreeData tree = contentPack.ReadJsonFile<FruitTreeData>($"{relativePath}/tree.json");
                    if (tree == null || (tree.DisableWithMod != null && Helper.ModRegistry.IsLoaded(tree.DisableWithMod)) || (tree.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(tree.EnableWithMod)))
                        continue;

                    // save fruit tree
                    tree.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/tree.png");
                    RegisterFruitTree(contentPack.Manifest, tree, contentPack.LoadAsset<Texture2D>($"{relativePath}/sapling.png"));
                }
            }

            // load big craftables
            DirectoryInfo bigCraftablesDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "BigCraftables"));
            if (bigCraftablesDir.Exists)
            {
                foreach (DirectoryInfo dir in bigCraftablesDir.EnumerateDirectories())
                {
                    string relativePath = $"BigCraftables/{dir.Name}";

                    // load data
                    BigCraftableData craftable = contentPack.ReadJsonFile<BigCraftableData>($"{relativePath}/big-craftable.json");
                    if (craftable == null || (craftable.DisableWithMod != null && Helper.ModRegistry.IsLoaded(craftable.DisableWithMod)) || (craftable.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(craftable.EnableWithMod)))
                        continue;
                    
                    // save craftable
                    craftable.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/big-craftable.png");
                    if (craftable.ReserveNextIndex && craftable.ReserveExtraIndexCount == 0)
                        craftable.ReserveExtraIndexCount = 1;
                    if (craftable.ReserveExtraIndexCount > 0)
                    {
                        craftable.extraTextures = new Texture2D[craftable.ReserveExtraIndexCount];
                        for ( int i = 0; i < craftable.ReserveExtraIndexCount; ++i )
                            craftable.extraTextures[i] = contentPack.LoadAsset<Texture2D>($"{relativePath}/big-craftable-{i + 2}.png");
                    }
                    RegisterBigCraftable(contentPack.Manifest, craftable);
                }
            }

            // load hats
            DirectoryInfo hatsDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Hats"));
            if (hatsDir.Exists)
            {
                foreach (DirectoryInfo dir in hatsDir.EnumerateDirectories())
                {
                    string relativePath = $"Hats/{dir.Name}";

                    // load data
                    HatData hat = contentPack.ReadJsonFile<HatData>($"{relativePath}/hat.json");
                    if (hat == null || (hat.DisableWithMod != null && Helper.ModRegistry.IsLoaded(hat.DisableWithMod)) || (hat.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(hat.EnableWithMod)))
                        continue;

                    // save object
                    hat.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/hat.png");
                    RegisterHat(contentPack.Manifest, hat);
                }
            }

            // Load weapons
            DirectoryInfo weaponsDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Weapons"));
            if (weaponsDir.Exists)
            {
                foreach (DirectoryInfo dir in weaponsDir.EnumerateDirectories())
                {
                    string relativePath = $"Weapons/{dir.Name}";

                    // load data
                    WeaponData weapon = contentPack.ReadJsonFile<WeaponData>($"{relativePath}/weapon.json");
                    if (weapon == null || (weapon.DisableWithMod != null && Helper.ModRegistry.IsLoaded(weapon.DisableWithMod)) || (weapon.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(weapon.EnableWithMod)))
                        continue;

                    // save object
                    weapon.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/weapon.png");
                    RegisterWeapon(contentPack.Manifest, weapon);
                }
            }

            // Load shirts
            DirectoryInfo shirtsDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Shirts"));
            if (shirtsDir.Exists)
            {
                foreach (DirectoryInfo dir in shirtsDir.EnumerateDirectories())
                {
                    string relativePath = $"Shirts/{dir.Name}";

                    // load data
                    ShirtData shirt = contentPack.ReadJsonFile<ShirtData>($"{relativePath}/shirt.json");
                    if (shirt == null || (shirt.DisableWithMod != null && Helper.ModRegistry.IsLoaded(shirt.DisableWithMod)) || (shirt.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(shirt.EnableWithMod)))
                        continue;

                    // save shirt
                    shirt.textureMale = contentPack.LoadAsset<Texture2D>($"{relativePath}/male.png");
                    if (shirt.Dyeable)
                        shirt.textureMaleColor = contentPack.LoadAsset<Texture2D>($"{relativePath}/male-color.png");
                    if (shirt.HasFemaleVariant)
                    {
                        shirt.textureFemale = contentPack.LoadAsset<Texture2D>($"{relativePath}/female.png");
                        if (shirt.Dyeable)
                            shirt.textureFemaleColor = contentPack.LoadAsset<Texture2D>($"{relativePath}/female-color.png");
                    }
                    RegisterShirt(contentPack.Manifest, shirt);
                }
            }

            // Load pants
            DirectoryInfo pantsDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Pants"));
            if (pantsDir.Exists)
            {
                foreach (DirectoryInfo dir in pantsDir.EnumerateDirectories())
                {
                    string relativePath = $"Pants/{dir.Name}";

                    // load data
                    PantsData pants = contentPack.ReadJsonFile<PantsData>($"{relativePath}/pants.json");
                    if (pants == null || (pants.DisableWithMod != null && Helper.ModRegistry.IsLoaded(pants.DisableWithMod)) || (pants.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(pants.EnableWithMod)))
                        continue;

                    // save pants
                    pants.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/pants.png");
                    RegisterPants(contentPack.Manifest, pants);
                }
            }

            // Load tailoring
            DirectoryInfo tailoringDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Tailoring"));
            if (tailoringDir.Exists)
            {
                foreach (DirectoryInfo dir in tailoringDir.EnumerateDirectories())
                {
                    string relativePath = $"Tailoring/{dir.Name}";

                    // load data
                    TailoringRecipeData recipe = contentPack.ReadJsonFile<TailoringRecipeData>($"{relativePath}/recipe.json");
                    if (recipe == null || (recipe.DisableWithMod != null && Helper.ModRegistry.IsLoaded(recipe.DisableWithMod)) || (recipe.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(recipe.EnableWithMod)))
                        continue;

                    RegisterTailoringRecipe(contentPack.Manifest, recipe);
                }
            }

            // Load boots
            DirectoryInfo bootsDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Boots"));
            if (bootsDir.Exists)
            {
                foreach (DirectoryInfo dir in bootsDir.EnumerateDirectories())
                {
                    string relativePath = $"Boots/{dir.Name}";

                    // load data
                    BootsData boots = contentPack.ReadJsonFile<BootsData>($"{relativePath}/boots.json");
                    if (boots == null || (boots.DisableWithMod != null && Helper.ModRegistry.IsLoaded(boots.DisableWithMod)) || (boots.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(boots.EnableWithMod)))
                        continue;

                    boots.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/boots.png");
                    boots.textureColor = contentPack.LoadAsset<Texture2D>($"{relativePath}/color.png");
                    RegisterBoots(contentPack.Manifest, boots);
                }
            }

            // Load boots
            DirectoryInfo fencesDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Fences"));
            if (fencesDir.Exists)
            {
                foreach (DirectoryInfo dir in fencesDir.EnumerateDirectories())
                {
                    string relativePath = $"Fences/{dir.Name}";

                    // load data
                    FenceData fence = contentPack.ReadJsonFile<FenceData>($"{relativePath}/fence.json");
                    if (fence == null || (fence.DisableWithMod != null && Helper.ModRegistry.IsLoaded(fence.DisableWithMod)) || (fence.EnableWithMod != null && !Helper.ModRegistry.IsLoaded(fence.EnableWithMod)))
                        continue;

                    fence.texture = contentPack.LoadAsset<Texture2D>($"{relativePath}/fence.png" );
                    fence.objectTexture = contentPack.LoadAsset<Texture2D>($"{relativePath}/object.png" );
                    RegisterFence( contentPack.Manifest, fence );
                }
            }

            // Load tailoring
            DirectoryInfo forgeDir = new DirectoryInfo(Path.Combine(contentPack.DirectoryPath, "Forge"));
            if ( forgeDir.Exists )
            {
                foreach ( DirectoryInfo dir in forgeDir.EnumerateDirectories() )
                {
                    string relativePath = $"Forge/{dir.Name}";

                    // load data
                    ForgeRecipeData recipe = contentPack.ReadJsonFile<ForgeRecipeData>($"{relativePath}/recipe.json");
                    if ( recipe == null || ( recipe.DisableWithMod != null && Helper.ModRegistry.IsLoaded( recipe.DisableWithMod ) ) || ( recipe.EnableWithMod != null && !Helper.ModRegistry.IsLoaded( recipe.EnableWithMod ) ) )
                        continue;

                    RegisterForgeRecipe( contentPack.Manifest, recipe );
                }
            }
        }

        private void resetAtTitle()
        {
            didInit = false;
            // When we go back to the title menu we need to reset things so things don't break when
            // going back to a save.
            clearIds(out objectIds, objects.ToList<DataNeedsId>());
            clearIds(out cropIds, crops.ToList<DataNeedsId>());
            clearIds(out fruitTreeIds, fruitTrees.ToList<DataNeedsId>());
            clearIds(out bigCraftableIds, bigCraftables.ToList<DataNeedsId>());
            clearIds(out hatIds, hats.ToList<DataNeedsId>());
            clearIds(out weaponIds, weapons.ToList<DataNeedsId>());
            List<DataNeedsId> clothing = new List<DataNeedsId>();
            clothing.AddRange(shirts);
            clothing.AddRange(pantss);
            clearIds(out clothingIds, clothing.ToList<DataNeedsId>());

            content1.InvalidateUsed();
            Helper.Content.AssetEditors.Remove(content2);

            locationsFixedAlready.Clear();
        }

        internal void onBlankSave()
        {
            Log.debug( "Loading stuff early (really super early)" );
            if ( string.IsNullOrEmpty( Constants.CurrentSavePath ) )
            {
                initStuff( loadIdFiles: false );
            }
        }

        private void onCreated(object sender, SaveCreatedEventArgs e)
        {
            Log.debug("Loading stuff early (creation)");
            //initStuff(loadIdFiles: false);
        }

        private void onLoadStageChanged(object sender, LoadStageChangedEventArgs e)
        {
            if (e.NewStage == StardewModdingAPI.Enums.LoadStage.SaveParsed)
            {
                //Log.debug("Loading stuff early (loading)");
                initStuff(loadIdFiles: true);
            }
            else if (e.NewStage == StardewModdingAPI.Enums.LoadStage.SaveLoadedLocations)
            {
                Log.debug("Fixing IDs");
                fixIdsEverywhere();
            }
            else if (e.NewStage == StardewModdingAPI.Enums.LoadStage.Loaded)
            {
                Log.debug("Adding default/leveled recipes");
                foreach (var obj in objects)
                {
                    if (obj.Recipe != null)
                    {
                        bool unlockedByLevel = false;
                        if ( obj.Recipe.SkillUnlockName?.Length > 0 && obj.Recipe.SkillUnlockLevel > 0 )
                        {
                            int level = 0;
                            switch ( obj.Recipe.SkillUnlockName )
                            {
                                case "Farming": level = Game1.player.farmingLevel.Value; break;
                                case "Fishing": level = Game1.player.fishingLevel.Value; break;
                                case "Foraging": level = Game1.player.foragingLevel.Value; break;
                                case "Mining": level = Game1.player.miningLevel.Value; break;
                                case "Combat": level = Game1.player.combatLevel.Value; break;
                                case "Luck": level = Game1.player.luckLevel.Value; break;
                                default: level = Game1.player.GetCustomSkillLevel(obj.Recipe.SkillUnlockName); break;
                            }

                            if ( level >= obj.Recipe.SkillUnlockLevel )
                            {
                                unlockedByLevel = true;
                            }
                        }
                        if ((obj.Recipe.IsDefault || unlockedByLevel) && !Game1.player.knowsRecipe(obj.Name))
                        {
                            if (obj.Category == ObjectData.Category_.Cooking)
                            {
                                Game1.player.cookingRecipes.Add(obj.Name, 0);
                            }
                            else
                            {
                                Game1.player.craftingRecipes.Add(obj.Name, 0);
                            }
                        }
                    }
                }
                foreach (var big in bigCraftables)
                {
                    if (big.Recipe != null)
                    {
                        bool unlockedByLevel = false;
                        if (big.Recipe.SkillUnlockName?.Length > 0 && big.Recipe.SkillUnlockLevel > 0)
                        {
                            int level = 0;
                            switch (big.Recipe.SkillUnlockName)
                            {
                                case "Farming": level = Game1.player.farmingLevel.Value; break;
                                case "Fishing": level = Game1.player.fishingLevel.Value; break;
                                case "Foraging": level = Game1.player.foragingLevel.Value; break;
                                case "Mining": level = Game1.player.miningLevel.Value; break;
                                case "Combat": level = Game1.player.combatLevel.Value; break;
                                case "Luck": level = Game1.player.luckLevel.Value; break;
                                default: level = Game1.player.GetCustomSkillLevel(big.Recipe.SkillUnlockName); break;
                            }

                            if (level >= big.Recipe.SkillUnlockLevel)
                            {
                                unlockedByLevel = true;
                            }
                        }
                        if ((big.Recipe.IsDefault || unlockedByLevel) && !Game1.player.knowsRecipe(big.Name))
                            Game1.player.craftingRecipes.Add(big.Name, 0);
                    }
                }
            }
        }
        

        private void clientConnected(object sender, PeerContextReceivedEventArgs e)
        {
            if (!Context.IsMainPlayer && !didInit)
            {
                Log.debug("Loading stuff early (MP client)");
                initStuff(loadIdFiles: false);
            }
        }

        public List<ShopDataEntry> shopData = new List<ShopDataEntry>();

        /// <summary>Raised after a game menu is opened, closed, or replaced.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onMenuChanged(object sender, MenuChangedEventArgs e)
        {
            if (e.NewMenu == null)
                return;

            if (e.NewMenu is TitleMenu)
            {
                resetAtTitle();
                return;
            }

            var menu = e.NewMenu as ShopMenu;
            bool hatMouse = menu != null && menu?.potraitPersonDialogue?.Replace( "\n", "" ) == Game1.parseText(Game1.content.LoadString("Strings\\StringsFromCSFiles:ShopMenu.cs.11494"), Game1.dialogueFont, 304).Replace( "\n", "" );
            string portraitPerson = menu?.portraitPerson?.Name;
            if (portraitPerson == null && Game1.currentLocation?.Name == "Hospital")
                portraitPerson = "Harvey";
            if (menu == null || (portraitPerson == null || portraitPerson == "") && !hatMouse)
                return;
            bool doAllSeeds = Game1.player.hasOrWillReceiveMail( "PierreStocklist" );

            Log.trace( $"Adding objects to {portraitPerson}'s shop" );
            var precondMeth = Helper.Reflection.GetMethod(Game1.currentLocation, "checkEventPrecondition");
            var forSale = menu.forSale;
            var itemPriceAndStock = menu.itemPriceAndStock;

            foreach ( var entry in shopData )
            {
                if ( !( entry.PurchaseFrom == portraitPerson || ( entry.PurchaseFrom == "HatMouse" && hatMouse ) ) )
                    continue;

                bool normalCond = true;
                if ( entry.PurchaseRequirements != null && entry.PurchaseRequirements.Length > 0 && entry.PurchaseRequirements[ 0 ] != "" )
                {
                    normalCond = epu.CheckConditions( entry.PurchaseRequirements );
                }
                if ( entry.Price == 0 || !normalCond && !(doAllSeeds && entry.ShowWithStocklist && portraitPerson == "Pierre") )
                    continue;

                var item = entry.Object();
                int price = entry.Price;
                if ( !normalCond )
                    price = (int)( price * 1.5 );
                if ( item is StardewValley.Object obj && obj.Category == StardewValley.Object.SeedsCategory )
                {
                    price = ( int ) ( price * Game1.MasterPlayer.difficultyModifier );
                }
                if ( item is StardewValley.Object obj2 && obj2.IsRecipe && Game1.player.knowsRecipe( obj2.Name ) )
                    continue;
                forSale.Add( item );
                itemPriceAndStock.Add( item, new int[] { price, ( item is StardewValley.Object obj3 && obj3.IsRecipe ) ? 1 : int.MaxValue } );
            }

            ((Api)api).InvokeAddedItemsToShop();
        }

        internal bool didInit = false;
        private void initStuff(bool loadIdFiles)
        {
            if (didInit)
                return;
            didInit = true;

            // load object ID mappings from save folder
            // If loadIdFiles is "maybe" (null), check the current save path
            if (loadIdFiles)
            {
                IDictionary<TKey, TValue> LoadDictionary<TKey, TValue>(string filename)
                {
                    string path = Path.Combine(Constants.CurrentSavePath, "JsonAssets", filename);
                    return File.Exists(path)
                        ? JsonConvert.DeserializeObject<Dictionary<TKey, TValue>>(File.ReadAllText(path))
                        : new Dictionary<TKey, TValue>();
                }
                Directory.CreateDirectory(Path.Combine(Constants.CurrentSavePath, "JsonAssets"));
                oldObjectIds = LoadDictionary<string, int>("ids-objects.json") ?? new Dictionary<string, int>();
                oldCropIds = LoadDictionary<string, int>("ids-crops.json") ?? new Dictionary<string, int>();
                oldFruitTreeIds = LoadDictionary<string, int>("ids-fruittrees.json") ?? new Dictionary<string, int>();
                oldBigCraftableIds = LoadDictionary<string, int>("ids-big-craftables.json") ?? new Dictionary<string, int>();
                oldHatIds = LoadDictionary<string, int>("ids-hats.json") ?? new Dictionary<string, int>();
                oldWeaponIds = LoadDictionary<string, int>("ids-weapons.json") ?? new Dictionary<string, int>();
                oldClothingIds = LoadDictionary<string, int>("ids-clothing.json") ?? new Dictionary<string, int>();
                oldBootsIds = LoadDictionary<string, int>("ids-boots.json") ?? new Dictionary<string, int>();

                Log.verbose("OLD IDS START");
                foreach (var id in oldObjectIds)
                    Log.verbose("\tObject " + id.Key + " = " + id.Value);
                foreach (var id in oldCropIds)
                    Log.verbose("\tCrop " + id.Key + " = " + id.Value);
                foreach (var id in oldFruitTreeIds)
                    Log.verbose("\tFruit Tree " + id.Key + " = " + id.Value);
                foreach (var id in oldBigCraftableIds)
                    Log.verbose("\tBigCraftable " + id.Key + " = " + id.Value);
                foreach (var id in oldHatIds)
                    Log.verbose("\tHat " + id.Key + " = " + id.Value);
                foreach (var id in oldWeaponIds)
                    Log.verbose("\tWeapon " + id.Key + " = " + id.Value);
                foreach (var id in oldClothingIds)
                    Log.verbose("\tClothing " + id.Key + " = " + id.Value);
                foreach (var id in oldBootsIds)
                    Log.verbose("\tBoots " + id.Key + " = " + id.Value);
                Log.verbose("OLD IDS END");
            }

            // assign IDs
            var objList = new List<DataNeedsId>();
            objList.AddRange(objects.ToList<DataNeedsId>());
            objList.AddRange(bootss.ToList<DataNeedsId>());
            objectIds = AssignIds("objects", StartingObjectId, objList);
            cropIds = AssignIds("crops", StartingCropId, crops.ToList<DataNeedsId>());
            fruitTreeIds = AssignIds("fruittrees", StartingFruitTreeId, fruitTrees.ToList<DataNeedsId>());
            bigCraftableIds = AssignIds("big-craftables", StartingBigCraftableId, bigCraftables.ToList<DataNeedsId>());
            hatIds = AssignIds("hats", StartingHatId, hats.ToList<DataNeedsId>());
            weaponIds = AssignIds("weapons", StartingWeaponId, weapons.ToList<DataNeedsId>());
            List<DataNeedsId> clothing = new List<DataNeedsId>();
            clothing.AddRange(shirts);
            clothing.AddRange(pantss);
            clothingIds = AssignIds("clothing", StartingClothingId, clothing.ToList<DataNeedsId>());

            AssignTextureIndices("shirts", StartingShirtTextureIndex, shirts.ToList<DataSeparateTextureIndex>());
            AssignTextureIndices("pants", StartingPantsTextureIndex, pantss.ToList<DataSeparateTextureIndex>());
            AssignTextureIndices("boots", StartingBootsId, bootss.ToList<DataSeparateTextureIndex>());

            Log.trace("Resetting max shirt/pants value");
            Helper.Reflection.GetField<int>(typeof(Clothing), "_maxShirtValue").SetValue(-1);
            Helper.Reflection.GetField<int>(typeof(Clothing), "_maxPantsValue").SetValue(-1);

            api.InvokeIdsAssigned();

            content1.InvalidateUsed();
            Helper.Content.AssetEditors.Add( content2 = new ContentInjector2() );

            // This happens here instead of with ID fixing because TMXL apparently
            // uses the ID fixing API before ID fixing happens everywhere.
            // Doing this here prevents some NREs (that don't show up unless you're
            // debugging for some reason????)
            origObjects = cloneIdDictAndRemoveOurs( Game1.objectInformation, objectIds );
            origCrops = cloneIdDictAndRemoveOurs( Game1.content.Load<Dictionary<int, string>>( "Data\\Crops" ), cropIds );
            origFruitTrees = cloneIdDictAndRemoveOurs( Game1.content.Load<Dictionary<int, string>>( "Data\\fruitTrees" ), fruitTreeIds );
            origBigCraftables = cloneIdDictAndRemoveOurs( Game1.bigCraftablesInformation, bigCraftableIds );
            origHats = cloneIdDictAndRemoveOurs( Game1.content.Load<Dictionary<int, string>>( "Data\\hats" ), hatIds );
            origWeapons = cloneIdDictAndRemoveOurs( Game1.content.Load<Dictionary<int, string>>( "Data\\weapons" ), weaponIds );
            origClothing = cloneIdDictAndRemoveOurs( Game1.content.Load<Dictionary<int, string>>( "Data\\ClothingInformation" ), clothingIds );
        }

        /// <summary>Raised after the game finishes writing data to the save file (except the initial save creation).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onSaving(object sender, SavingEventArgs e)
        {
            if (!Game1.IsMasterGame)
                return;

            if (!Directory.Exists(Path.Combine(Constants.CurrentSavePath, "JsonAssets")))
                Directory.CreateDirectory(Path.Combine(Constants.CurrentSavePath, "JsonAssets"));

            File.WriteAllText(Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-objects.json"), JsonConvert.SerializeObject(objectIds));
            File.WriteAllText(Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-crops.json"), JsonConvert.SerializeObject(cropIds));
            File.WriteAllText(Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-fruittrees.json"), JsonConvert.SerializeObject(fruitTreeIds));
            File.WriteAllText(Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-big-craftables.json"), JsonConvert.SerializeObject(bigCraftableIds));
            File.WriteAllText(Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-hats.json"), JsonConvert.SerializeObject(hatIds));
            File.WriteAllText(Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-weapons.json"), JsonConvert.SerializeObject(weaponIds));
            File.WriteAllText(Path.Combine(Constants.CurrentSavePath, "JsonAssets", "ids-clothing.json"), JsonConvert.SerializeObject(clothingIds));
        }

        internal IList<ObjectData> myRings = new List<ObjectData>();

        /// <summary>Raised after items are added or removed to a player's inventory. NOTE: this event is currently only raised for the current player.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void onInventoryChanged(object sender, InventoryChangedEventArgs e)
        {
            if (!e.IsLocalPlayer)
                return;

            IList<int> ringIds = new List<int>();
            foreach (var ring in myRings)
                ringIds.Add(ring.id);

            for (int i = 0; i < Game1.player.Items.Count; ++i)
            {
                var item = Game1.player.Items[i];
                if (item is SObject obj && ringIds.Contains(obj.ParentSheetIndex))
                {
                    Log.trace($"Turning a ring-object of {obj.ParentSheetIndex} into a proper ring");
                    Game1.player.Items[i] = new Ring(obj.ParentSheetIndex);
                }
            }
        }

        internal const int StartingObjectId = 3000;
        internal const int StartingCropId = 100;
        internal const int StartingFruitTreeId = 10;
        internal const int StartingBigCraftableId = 300;
        internal const int StartingHatId = 160;
        internal const int StartingWeaponId = 128;
        internal const int StartingClothingId = 3000;
        internal const int StartingShirtTextureIndex = 750;
        internal const int StartingPantsTextureIndex = 20;
        internal const int StartingBootsId = 100;

        internal IList<ObjectData> objects = new List<ObjectData>();
        internal IList<CropData> crops = new List<CropData>();
        internal IList<FruitTreeData> fruitTrees = new List<FruitTreeData>();
        internal IList<BigCraftableData> bigCraftables = new List<BigCraftableData>();
        internal IList<HatData> hats = new List<HatData>();
        internal IList<WeaponData> weapons = new List<WeaponData>();
        internal IList<ShirtData> shirts = new List<ShirtData>();
        internal IList<PantsData> pantss = new List<PantsData>();
        internal IList<TailoringRecipeData> tailoring = new List<TailoringRecipeData>();
        internal IList<BootsData> bootss = new List<BootsData>();
        internal List<FenceData> fences = new List<FenceData>();
        internal IList<ForgeRecipeData> forge = new List<ForgeRecipeData>();

        internal IDictionary<string, int> objectIds;
        internal IDictionary<string, int> cropIds;
        internal IDictionary<string, int> fruitTreeIds;
        internal IDictionary<string, int> bigCraftableIds;
        internal IDictionary<string, int> hatIds;
        internal IDictionary<string, int> weaponIds;
        internal IDictionary<string, int> clothingIds;

        internal IDictionary<string, int> oldObjectIds;
        internal IDictionary<string, int> oldCropIds;
        internal IDictionary<string, int> oldFruitTreeIds;
        internal IDictionary<string, int> oldBigCraftableIds;
        internal IDictionary<string, int> oldHatIds;
        internal IDictionary<string, int> oldWeaponIds;
        internal IDictionary<string, int> oldClothingIds;
        internal IDictionary<string, int> oldBootsIds;

        internal IDictionary<int, string> origObjects;
        internal IDictionary<int, string> origCrops;
        internal IDictionary<int, string> origFruitTrees;
        internal IDictionary<int, string> origBigCraftables;
        internal IDictionary<int, string> origHats;
        internal IDictionary<int, string> origWeapons;
        internal IDictionary<int, string> origClothing;
        internal IDictionary<int, string> origBoots;

        public int ResolveObjectId(object data)
        {
            if (data.GetType() == typeof(long))
                return (int)(long)data;
            else
            {
                if (objectIds.ContainsKey((string)data))
                    return objectIds[(string)data];

                foreach (var obj in Game1.objectInformation)
                {
                    if (obj.Value.Split('/')[0] == (string)data)
                        return obj.Key;
                }

                Log.warn($"No idea what '{data}' is!");
                return 0;
            }
        }

        public int ResolveClothingId(object data)
        {
            if (data.GetType() == typeof(long))
                return (int)(long)data;
            else
            {
                if (clothingIds.ContainsKey((string)data))
                    return clothingIds[(string)data];

                foreach (var obj in Game1.clothingInformation)
                {
                    if (obj.Value.Split('/')[0] == (string)data)
                        return obj.Key;
                }

                Log.warn($"No idea what '{data}' is!");
                return 0;
            }
        }

        private Dictionary<string, int> AssignIds(string type, int starting, List<DataNeedsId> data)
        {
            data.Sort((dni1, dni2) => dni1.Name.CompareTo(dni2.Name));

            Dictionary<string, int> ids = new Dictionary<string, int>();

            int[] bigSkip = new int[] { 309, 310, 311, 326, 340, 434, 447, 459, 599, 621, 628, 629, 630, 631, 632, 633, 645, 812 };

            int currId = starting;
            foreach (var d in data)
            {
                if (d.id == -1)
                {
                    Log.verbose($"New ID: {d.Name} = {currId}");
                    int id = currId++;
                    if (type == "big-craftables")
                    {
                        while (bigSkip.Contains(id))
                        {
                            id = currId++;
                        }
                    }

                    ids.Add(d.Name, id);
                    if (type == "objects" && d is ObjectData objd && objd.IsColored)
                        ++currId;
                    else if (type == "big-craftables" && ((BigCraftableData)d).ReserveExtraIndexCount > 0)
                        currId += ((BigCraftableData)d).ReserveExtraIndexCount;
                    d.id = ids[d.Name];
                }
            }

            return ids;
        }

        private void AssignTextureIndices(string type, int starting, List<DataSeparateTextureIndex> data)
        {
            data.Sort((dni1, dni2) => dni1.Name.CompareTo(dni2.Name));

            Dictionary<string, int> idxs = new Dictionary<string, int>();

            int currIdx = starting;
            foreach (var d in data)
            {
                if (d.textureIndex == -1)
                {
                    Log.verbose($"New texture index: {d.Name} = {currIdx}");
                    idxs.Add(d.Name, currIdx++);
                    if (type == "shirts" && ((ClothingData)d).HasFemaleVariant)
                        ++currIdx;
                    d.textureIndex = idxs[d.Name];
                }
            }
        }

        private void clearIds(out IDictionary<string, int> ids, List<DataNeedsId> objs)
        {
            ids = null;
            foreach (DataNeedsId obj in objs)
            {
                obj.id = -1;
            }
        }

        private IDictionary<int, string> cloneIdDictAndRemoveOurs(IDictionary<int, string> full, IDictionary<string, int> ours)
        {
            var ret = new Dictionary<int, string>(full);
            foreach (var obj in ours)
                ret.Remove(obj.Value);
            return ret;
        }

        private bool reverseFixing = false;
        private HashSet< string > locationsFixedAlready = new HashSet<string>();
        private void fixIdsEverywhere( bool reverse = false )
        {
            reverseFixing = reverse;
            if ( reverseFixing )
            {
                Log.info( "Reversing!" );
            }

            fixItemList(Game1.player.Items);
            fixItemList( Game1.player.team.junimoChest );
#pragma warning disable AvoidNetField
            if (Game1.player.leftRing.Value != null && fixId(oldObjectIds, objectIds, Game1.player.leftRing.Value.indexInTileSheet, origObjects))
                Game1.player.leftRing.Value = null;
            if ( Game1.player.leftRing.Value is CombinedRing cring )
            {
                var toRemoveRing = new List<Ring>();
                foreach ( var ring2 in cring.combinedRings )
                {
                    if ( fixId( oldObjectIds, objectIds, ring2.indexInTileSheet, origObjects ) )
                        toRemoveRing.Add( ring2 );
                }
                foreach ( var removeRing in toRemoveRing )
                    cring.combinedRings.Remove( removeRing );
            }
            if (Game1.player.rightRing.Value != null && fixId(oldObjectIds, objectIds, Game1.player.rightRing.Value.indexInTileSheet, origObjects))
                Game1.player.rightRing.Value = null;
            if ( Game1.player.rightRing.Value is CombinedRing cring2 )
            {
                var toRemoveRing = new List<Ring>();
                foreach ( var ring2 in cring2.combinedRings )
                {
                    if ( fixId( oldObjectIds, objectIds, ring2.indexInTileSheet, origObjects ) )
                        toRemoveRing.Add( ring2 );
                }
                foreach ( var removeRing in toRemoveRing )
                    cring2.combinedRings.Remove( removeRing );
            }
            if (Game1.player.hat.Value != null && fixId(oldHatIds, hatIds, Game1.player.hat.Value.which, origHats))
                Game1.player.hat.Value = null;
            if (Game1.player.shirtItem.Value != null && fixId(oldClothingIds, clothingIds, Game1.player.shirtItem.Value.parentSheetIndex, origClothing))
                Game1.player.shirtItem.Value = null;
            if (Game1.player.pantsItem.Value != null && fixId(oldClothingIds, clothingIds, Game1.player.pantsItem.Value.parentSheetIndex, origClothing))
                Game1.player.pantsItem.Value = null;
            if (Game1.player.boots.Value != null && fixId(oldObjectIds, objectIds, Game1.player.boots.Value.indexInTileSheet, origObjects))
                Game1.player.boots.Value = null;
            /*else if (Game1.player.boots.Value != null)
                Game1.player.boots.Value.reloadData();*/
#pragma warning restore AvoidNetField
            foreach (var loc in Game1.locations)
                fixLocation(loc);

            fixIdDict(Game1.player.basicShipped, removeUnshippable: true);
            fixIdDict(Game1.player.mineralsFound);
            fixIdDict(Game1.player.recipesCooked);
            fixIdDict2(Game1.player.archaeologyFound);
            fixIdDict2(Game1.player.fishCaught);

            var bundleData = Game1.netWorldState.Value.GetUnlocalizedBundleData();
            var bundleData_ = new Dictionary< string, string >( Game1.netWorldState.Value.GetUnlocalizedBundleData() );
            
            foreach ( var entry in bundleData_ )
            {
                List<string> toks = new List<string>(entry.Value.Split('/'));

                // First, fix some stuff we broke in an earlier build by using .BundleData instead of the unlocalized version
                // Copied from Game1.applySaveFix (case FixBotchedBundleData)
                int temp = 0;
                while ( toks.Count > 4 && !int.TryParse( toks[ toks.Count - 1 ], out temp ) )
                {
                    string last_value = toks[toks.Count - 1];
                    if ( char.IsDigit( last_value[ last_value.Length - 1 ] ) && last_value.Contains( ":" ) && last_value.Contains( "\\" ) )
                    {
                        break;
                    }
                    toks.RemoveAt( toks.Count - 1 );
                }

                // Then actually fix IDs
                string[] toks1 = toks[1].Split(' ');
                if ( toks1[ 0 ] == "O" )
                {
                    int oldId = int.Parse( toks1[ 1 ] );
                    if ( oldId != -1 )
                    {
                        if ( fixId( oldObjectIds, objectIds, ref oldId, origObjects ) )
                        {
                            Log.warn( $"Bundle reward item missing ({entry.Key}, {oldId})! Probably broken now!" );
                            oldId = -1;
                        }
                        else
                        {
                            toks1[ 1 ] = oldId.ToString();
                        }
                    }
                }
                else if ( toks1[ 0 ] == "BO" )
                {
                    int oldId = int.Parse( toks1[ 1 ] );
                    if ( oldId != -1 )
                    {
                        if ( fixId( oldBigCraftableIds, bigCraftableIds, ref oldId, origBigCraftables ) )
                        {
                            Log.warn( $"Bundle reward item missing ({entry.Key}, {oldId})! Probably broken now!" );
                            oldId = -1;
                        }
                        else
                        {
                            toks1[ 1 ] = oldId.ToString();
                        }
                    }
                }
                toks[ 1 ] = string.Join( " ", toks1 );
                string[] toks2 = toks[2].Split(' ');
                for ( int i = 0; i < toks2.Length; i += 3 )
                {
                    int oldId = int.Parse( toks2[ i ] );
                    if ( oldId != -1 )
                    {
                        if ( fixId( oldObjectIds, objectIds,ref oldId, origObjects ) )
                        {
                            Log.warn( $"Bundle item missing ({entry.Key}, {oldId})! Probably broken now!" );
                            oldId = -1;
                        }
                        else
                        {
                            toks2[ i ] = oldId.ToString();
                        }
                    }
                }
                toks[ 2 ] = string.Join( " ", toks2 );
                bundleData[ entry.Key ] = string.Join( "/", toks );
            }
            // Fix bad bundle data

            Game1.netWorldState.Value.SetBundleData( bundleData );

            if ( !reverseFixing )
                api.InvokeIdsFixed();
            reverseFixing = false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        internal bool fixItem(Item item)
        {
            if (item is Hat hat)
            {
                if (fixId(oldHatIds, hatIds, hat.which, origHats))
                    return true;
            }
            else if (item is MeleeWeapon weapon)
            {
                if (fixId(oldWeaponIds, weaponIds, weapon.initialParentTileIndex, origWeapons))
                    return true;
                else if (fixId(oldWeaponIds, weaponIds, weapon.currentParentTileIndex, origWeapons))
                    return true;
                else if (fixId(oldWeaponIds, weaponIds, weapon.indexOfMenuItemView, origWeapons))
                    return true;
            }
            else if (item is Ring ring)
            {
                if (fixId(oldObjectIds, objectIds, ring.indexInTileSheet, origObjects))
                    return true;
            }
            else if (item is Clothing clothing)
            {
                if (fixId(oldClothingIds, clothingIds, clothing.parentSheetIndex, origClothing))
                    return true;
            }
            else if (item is Boots boots)
            {
                if (fixId(oldObjectIds, objectIds, boots.indexInTileSheet, origObjects))
                    return true;
                /*else
                    boots.reloadData();*/
            }
            else if (!(item is SObject))
                return false;
            var obj = item as StardewValley.Object;

            if (obj is Chest chest)
            {
                if ( fixId( oldBigCraftableIds, bigCraftableIds, chest.parentSheetIndex, origBigCraftables ) )
                    chest.ParentSheetIndex = 130;
                else
                {
                    chest.startingLidFrame.Value = chest.ParentSheetIndex + 1;
                }
                fixItemList(chest.items);
            }
            else if (obj is IndoorPot pot)
            {
                var hd = pot.hoeDirt.Value;
                if (hd == null || hd.crop == null)
                    return false;

                var oldId = hd.crop.rowInSpriteSheet.Value;
                if (fixId(oldCropIds, cropIds, hd.crop.rowInSpriteSheet, origCrops))
                    hd.crop = null;
                else
                {
                    var key = cropIds.FirstOrDefault(x => x.Value == hd.crop.rowInSpriteSheet.Value).Key;
                    var c = crops.FirstOrDefault(x => x.Name == key);
                    if (c != null) // Non-JA crop
                    {
                        Log.verbose("Fixing crop product: From " + hd.crop.indexOfHarvest.Value + " to " + c.Product + "=" + ResolveObjectId(c.Product));
                        hd.crop.indexOfHarvest.Value = ResolveObjectId(c.Product);
                        fixId(oldObjectIds, objectIds, hd.crop.netSeedIndex, origObjects);
                    }
                }
            }
            else if ( obj is Fence fence )
            {
                if ( fixId( oldObjectIds, objectIds, fence.whichType, origObjects ) )
                    return true;
                else
                    fence.ParentSheetIndex = -fence.whichType.Value;
            }
            else if ( obj.GetType() == typeof( SObject ) )
            {
                if (!obj.bigCraftable.Value)
                {
                    if (fixId(oldObjectIds, objectIds, obj.preservedParentSheetIndex, origObjects))
                        obj.preservedParentSheetIndex.Value = -1;
                    if (fixId(oldObjectIds, objectIds, obj.parentSheetIndex, origObjects))
                        return true;
                }
                else
                {
                    if (fixId(oldBigCraftableIds, bigCraftableIds, obj.parentSheetIndex, origBigCraftables))
                        return true;
                }
            }

            if (obj.heldObject.Value != null)
            {
                if (fixId(oldObjectIds, objectIds, obj.heldObject.Value.parentSheetIndex, origObjects))
                    obj.heldObject.Value = null;

                if (obj.heldObject.Value is Chest chest2)
                {
                    fixItemList(chest2.items);
                }
            }

            return false;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        internal void fixLocation(GameLocation loc)
        {
            // TMXL fixes things before the main ID fixing, then adds them to the main location list
            // So things would get double fixed without this.
            if ( locationsFixedAlready.Contains( loc.NameOrUniqueName ) )
                return;
            locationsFixedAlready.Add( loc.NameOrUniqueName );

            if (loc is FarmHouse fh)
            {
#pragma warning disable AvoidImplicitNetFieldCast
                if (fh.fridge.Value?.items != null)
#pragma warning restore AvoidImplicitNetFieldCast
                    fixItemList(fh.fridge.Value.items);
            }
            if (loc is IslandFarmHouse ifh)
            {
#pragma warning disable AvoidImplicitNetFieldCast
                if ( ifh.fridge.Value?.items != null )
#pragma warning restore AvoidImplicitNetFieldCast
                    fixItemList( ifh.fridge.Value.items );
            }
            if ( loc is Cabin cabin )
            {
                var player = cabin.farmhand.Value;
                if ( player != null )
                {
                    fixItemList(player.Items);
                    //fixItemList( player.team.junimoChest );
#pragma warning disable AvoidNetField
                    if ( player.leftRing.Value != null && fixId( oldObjectIds, objectIds, player.leftRing.Value.parentSheetIndex, origObjects ) )
                        player.leftRing.Value = null;
                    if ( player.leftRing.Value is CombinedRing cring )
                    {
                        var toRemoveRing = new List<Ring>();
                        foreach ( var ring2 in cring.combinedRings )
                        {
                            if ( fixId( oldObjectIds, objectIds, ring2.indexInTileSheet, origObjects ) )
                                toRemoveRing.Add( ring2 );
                        }
                        foreach ( var removeRing in toRemoveRing )
                            cring.combinedRings.Remove( removeRing );
                    }
                    if (player.rightRing.Value != null && fixId(oldObjectIds, objectIds, player.rightRing.Value.parentSheetIndex, origObjects))
                        player.rightRing.Value = null;
                    if ( player.rightRing.Value is CombinedRing cring2 )
                    {
                        var toRemoveRing = new List<Ring>();
                        foreach ( var ring2 in cring2.combinedRings )
                        {
                            if ( fixId( oldObjectIds, objectIds, ring2.indexInTileSheet, origObjects ) )
                                toRemoveRing.Add( ring2 );
                        }
                        foreach ( var removeRing in toRemoveRing )
                            cring2.combinedRings.Remove( removeRing );
                    }
                    if (player.hat.Value != null && fixId(oldHatIds, hatIds, player.hat.Value.which, origHats))
                        player.hat.Value = null;
                    if (player.shirtItem.Value != null && fixId(oldClothingIds, clothingIds, player.shirtItem.Value.parentSheetIndex, origClothing))
                        player.shirtItem.Value = null;
                    if (player.pantsItem.Value != null && fixId(oldClothingIds, clothingIds, player.pantsItem.Value.parentSheetIndex, origClothing))
                        player.pantsItem.Value = null;
                    if (player.boots.Value != null && fixId(oldObjectIds, objectIds, player.boots.Value.parentSheetIndex, origObjects))
                        player.boots.Value = null;
                    /*else if (player.boots.Value != null)
                        player.boots.Value.reloadData();*/
#pragma warning restore AvoidNetField
                }
            }

            foreach ( var npc in loc.characters )
            {
                if ( npc is Horse horse )
                {
                    if ( horse.hat.Value != null && fixId( oldHatIds, hatIds, horse.hat.Value.which, origHats ) )
                        horse.hat.Value = null;
                }
                else if ( npc is Child child )
                {
                    if ( child.hat.Value != null && fixId( oldHatIds, hatIds, child.hat.Value.which, origHats ) )
                        child.hat.Value = null;
                }
            }

            IList<Vector2> toRemove = new List<Vector2>();
            foreach (var tfk in loc.terrainFeatures.Keys)
            {
                var tf = loc.terrainFeatures[tfk];
                if (tf is HoeDirt hd)
                {
                    if (hd.crop == null)
                        continue;

                    var oldId = hd.crop.rowInSpriteSheet.Value;
                    if (fixId(oldCropIds, cropIds, hd.crop.rowInSpriteSheet, origCrops))
                        hd.crop = null;
                    else
                    {
                        var key = cropIds.FirstOrDefault(x => x.Value == hd.crop.rowInSpriteSheet.Value).Key;
                        var c = crops.FirstOrDefault(x => x.Name == key);
                        if (c != null) // Non-JA crop
                        {
                            Log.verbose("Fixing crop product: From " + hd.crop.indexOfHarvest.Value + " to " + c.Product + "=" + ResolveObjectId(c.Product));
                            hd.crop.indexOfHarvest.Value = ResolveObjectId(c.Product);
                            fixId(oldObjectIds, objectIds, hd.crop.netSeedIndex, origObjects);
                        }
                    }
                }
                else if (tf is FruitTree ft)
                {
                    var oldId = ft.treeType.Value;
                    if (fixId(oldFruitTreeIds, fruitTreeIds, ft.treeType, origFruitTrees))
                        toRemove.Add(tfk);
                    else
                    {
                        var key = fruitTreeIds.FirstOrDefault(x => x.Value == ft.treeType.Value).Key;
                        var ftt = fruitTrees.FirstOrDefault(x => x.Name == key);
                        if (ftt != null) // Non-JA fruit tree
                        {
                            Log.verbose("Fixing fruit tree product: From " + ft.indexOfFruit.Value + " to " + ftt.Product + "=" + ResolveObjectId(ftt.Product));
                            ft.indexOfFruit.Value = ResolveObjectId(ftt.Product);
                        }
                    }
                }
            }
            foreach (var rem in toRemove)
                loc.terrainFeatures.Remove(rem);

            toRemove.Clear();
            foreach (var objk in loc.netObjects.Keys)
            {
                var obj = loc.netObjects[objk];
                if ( fixItem(obj) )
                {
                    toRemove.Add(objk);
                }
            }
            foreach (var rem in toRemove)
                loc.objects.Remove(rem);

            toRemove.Clear();
            foreach (var objk in loc.overlayObjects.Keys)
            {
                var obj = loc.overlayObjects[objk];
                if (obj is Chest chest)
                {
                    fixItemList(chest.items);
                }
                else if (obj is Sign sign)
                {
                    if (!fixItem(sign.displayItem.Value))
                        sign.displayItem.Value = null;
                }
                else if ( obj.GetType() == typeof( SObject ) )
                {
                    if (!obj.bigCraftable.Value)
                    {
                        if (fixId(oldObjectIds, objectIds, obj.parentSheetIndex, origObjects))
                            toRemove.Add(objk);
                    }
                    else
                    {
                        if (fixId(oldBigCraftableIds, bigCraftableIds, obj.parentSheetIndex, origBigCraftables))
                            toRemove.Add(objk);
                        else if ( obj.ParentSheetIndex == 126 && obj.Quality != 0 ) // Alien rarecrow stores what ID is it is wearing here
                        {
                            obj.Quality--;
                            if ( fixId( oldHatIds, hatIds, obj.quality, origHats ) )
                                obj.Quality = 0;
                            else obj.Quality++;
                        }
                    }
                }

                if (obj.heldObject.Value != null)
                {
                    if (fixId(oldObjectIds, objectIds, obj.heldObject.Value.parentSheetIndex, origObjects))
                        obj.heldObject.Value = null;

                    if (obj.heldObject.Value is Chest chest2)
                    {
                        fixItemList(chest2.items);
                    }
                }
            }
            foreach (var rem in toRemove)
                loc.overlayObjects.Remove(rem);

            if (loc is BuildableGameLocation buildLoc)
                foreach (var building in buildLoc.buildings)
                {
                    if (building.indoors.Value != null)
                        fixLocation(building.indoors.Value);
                    if (building is Mill mill)
                    {
                        fixItemList(mill.input.Value.items);
                        fixItemList(mill.output.Value.items);
                    }
                    else if (building is FishPond pond)
                    {
                        if (pond.fishType.Value == -1)
                        {
                            Helper.Reflection.GetField<SObject>(pond, "_fishObject").SetValue(null);
                            continue;
                        }

                        if (fixId(oldObjectIds, objectIds, pond.fishType, origObjects))
                        {
                            pond.fishType.Value = -1;
                            pond.currentOccupants.Value = 0;
                            pond.maxOccupants.Value = 0;
                            Helper.Reflection.GetField<SObject>(pond, "_fishObject").SetValue(null);
                        }
                        if (pond.sign.Value != null && fixId(oldObjectIds, objectIds, pond.sign.Value.parentSheetIndex, origObjects))
                            pond.sign.Value = null;
                        if (pond.output.Value != null && fixId(oldObjectIds, objectIds, pond.output.Value.parentSheetIndex, origObjects))
                            pond.output.Value = null;
                        if (pond.neededItem.Value != null && fixId(oldObjectIds, objectIds, pond.neededItem.Value.parentSheetIndex, origObjects))
                            pond.neededItem.Value = null;
                    }
                }

            //if (loc is DecoratableLocation decoLoc)
                foreach (var furniture in loc.furniture)
                {
                    if (furniture.heldObject.Value != null)
                    {
                        if (!furniture.heldObject.Value.bigCraftable.Value)
                        {
                            if (fixId(oldObjectIds, objectIds, furniture.heldObject.Value.parentSheetIndex, origObjects))
                                furniture.heldObject.Value = null;
                        }
                        else
                        {
                            if (fixId(oldBigCraftableIds, bigCraftableIds, furniture.heldObject.Value.parentSheetIndex, origBigCraftables))
                                furniture.heldObject.Value = null;
                        }
                    }
                    if (furniture is StorageFurniture storage)
                        fixItemList(storage.heldItems);
                }

            if (loc is Farm farm)
            {
                foreach (var animal in farm.Animals.Values)
                {
                    if (animal.currentProduce.Value != -1)
                        if (fixId(oldObjectIds, objectIds, animal.currentProduce, origObjects))
                            animal.currentProduce.Value = -1;
                    if (animal.defaultProduceIndex.Value != -1)
                        if (fixId(oldObjectIds, objectIds, animal.defaultProduceIndex, origObjects))
                            animal.defaultProduceIndex.Value = 0;
                    if (animal.deluxeProduceIndex.Value != -1)
                        if (fixId(oldObjectIds, objectIds, animal.deluxeProduceIndex, origObjects))
                            animal.deluxeProduceIndex.Value = 0;
                }

                var clumpsToRemove = new List<ResourceClump>();
                foreach ( var clump in farm.resourceClumps )
                {
                    if (fixId(oldObjectIds, objectIds, clump.parentSheetIndex, origObjects))
                        clumpsToRemove.Add(clump);
                }
                foreach ( var clump in clumpsToRemove )
                {
                    farm.resourceClumps.Remove(clump);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("SMAPI.CommonErrors", "AvoidNetField")]
        internal void fixItemList(IList<Item> items)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                var item = items[i];
                if ( item == null )
                    continue;
                if (item.GetType() == typeof( SObject ) )
                {
                    var obj = item as SObject;
                    if (!obj.bigCraftable.Value)
                    {
                        if (fixId(oldObjectIds, objectIds, obj.parentSheetIndex, origObjects))
                            items[i] = null;
                    }
                    else
                    {
                        if (fixId(oldBigCraftableIds, bigCraftableIds, obj.parentSheetIndex, origBigCraftables))
                            items[i] = null;
                    }
                }
                else if (item is Hat hat)
                {
                    if (fixId(oldHatIds, hatIds, hat.which, origHats))
                        items[i] = null;
                }
                else if ( item is Tool tool )
                {
                    for ( int a = 0; a < tool.attachments?.Count; ++a )
                    {
                        var attached = tool.attachments[ a ];
                        if ( attached == null )
                            continue;

                        if ( attached.GetType() != typeof( StardewValley.Object ) || attached.bigCraftable )
                        {
                            Log.warn( "Unsupported attachment types! Let spacechase0 know he needs to support " + attached.bigCraftable.Value + " " + attached );
                        }
                        else
                        {
                            if ( fixId( oldObjectIds, objectIds, attached.parentSheetIndex, origObjects ) )
                            {
                                tool.attachments[ a ] = null;
                            }
                        }
                    }
                    if ( item is MeleeWeapon weapon )
                    {
                        if ( fixId( oldWeaponIds, weaponIds, weapon.initialParentTileIndex, origWeapons ) )
                            items[ i ] = null;
                        else if ( fixId( oldWeaponIds, weaponIds, weapon.currentParentTileIndex, origWeapons ) )
                            items[ i ] = null;
                        else if ( fixId( oldWeaponIds, weaponIds, weapon.currentParentTileIndex, origWeapons ) )
                            items[ i ] = null;
                    }
                }
                else if (item is Ring ring)
                {
                    if (fixId(oldObjectIds, objectIds, ring.indexInTileSheet, origObjects))
                        items[i] = null;
                    if ( ring is CombinedRing cring )
                    {
                        var toRemove = new List<Ring>();
                        foreach ( var ring2 in cring.combinedRings )
                        {
                            if ( fixId( oldObjectIds, objectIds, ring2.indexInTileSheet, origObjects ) )
                                toRemove.Add( ring2 );
                        }
                        foreach ( var removeRing in toRemove )
                            cring.combinedRings.Remove( removeRing );
                    }
                }
                else if (item is Clothing clothing)
                {
                    if (fixId(oldClothingIds, clothingIds, clothing.parentSheetIndex, origClothing))
                        items[i] = null;
                }
                else if (item is Boots boots)
                {
                    if (fixId(oldObjectIds, objectIds, boots.indexInTileSheet, origObjects))
                        items[i] = null;
                    /*else
                        boots.reloadData();*/
                }
            }
        }

        private void fixIdDict(NetIntDictionary<int, NetInt> dict, bool removeUnshippable = false)
        {
            var toRemove = new List<int>();
            var toAdd = new Dictionary<int, int>();
            foreach (var entry in dict.Keys)
            {
                if (origObjects.ContainsKey(entry))
                    continue;
                else if (oldObjectIds.Values.Contains(entry))
                {
                    var key = oldObjectIds.FirstOrDefault(x => x.Value == entry).Key;
                    bool isRing = myRings.FirstOrDefault(r => r.id == entry) != null;
                    bool canShip = objects.FirstOrDefault(o => o.id == entry)?.CanSell ?? true;
                    bool hideShippable = objects.FirstOrDefault(o => o.id == entry)?.HideFromShippingCollection ?? true;

                    toRemove.Add(entry);
                    if (objectIds.ContainsKey(key))
                    {
                        if (removeUnshippable && (!canShip || hideShippable || isRing))
                            ;// Log.warn("Found unshippable");
                        else
                            toAdd.Add(objectIds[key], dict[entry]);
                    }
                }
            }
            foreach (var entry in toRemove)
                dict.Remove(entry);
            foreach (var entry in toAdd)
            {
                if (dict.ContainsKey(entry.Key))
                {
                    Log.error("Dict already has value for " + entry.Key + "!");
                    foreach ( var obj in objects )
                    {
                        if (obj.id == entry.Key)
                            Log.error("\tobj = " + obj.Name);
                    }
                }
                dict.Add(entry.Key, entry.Value);
            }
        }

        private void fixIdDict2(NetIntIntArrayDictionary dict)
        {
            var toRemove = new List<int>();
            var toAdd = new Dictionary<int, int[]>();
            foreach (var entry in dict.Keys)
            {
                if (origObjects.ContainsKey(entry))
                    continue;
                else if (oldObjectIds.Values.Contains(entry))
                {
                    var key = oldObjectIds.FirstOrDefault(x => x.Value == entry).Key;

                    toRemove.Add(entry);
                    if (objectIds.ContainsKey(key))
                    {
                        toAdd.Add(objectIds[key], dict[entry]);
                    }
                }
            }
            foreach (var entry in toRemove)
                dict.Remove(entry);
            foreach (var entry in toAdd)
                dict.Add(entry.Key, entry.Value);
        }

        // Return true if the item should be deleted, false otherwise.
        // Only remove something if old has it but not new
        private bool fixId(IDictionary<string, int> oldIds, IDictionary<string, int> newIds, NetInt id, IDictionary<int, string> origData)
        {
            if (origData.ContainsKey(id.Value))
                return false;

            if ( reverseFixing )
            {
                if ( newIds.Values.Contains( id.Value ) )
                {
                    int id_ = id.Value;
                    var key = newIds.FirstOrDefault( x => x.Value == id_ ).Key;

                    if ( oldIds.ContainsKey( key ) )
                    {
                        id.Value = oldIds[ key ];
                        Log.verbose( "Changing ID: " + key + " from ID " + id_ + " to " + id.Value );
                        return false;
                    }
                    else
                    {
                        Log.warn( "New item " + key + " with ID " + id_ + "!" );
                        return false;
                    }
                }
                else return false;
            }
            else
            {
                if (oldIds.Values.Contains(id.Value))
                {
                    int id_ = id.Value;
                    var key = oldIds.FirstOrDefault(x => x.Value == id_).Key;

                    if (newIds.ContainsKey(key))
                    {
                        id.Value = newIds[key];
                        Log.trace("Changing ID: " + key + " from ID " + id_ + " to " + id.Value);
                        return false;
                    }
                    else
                    {
                        Log.trace( "Deleting missing item " + key + " with old ID " + id_);
                        return true;
                    }
                }
                else return false;
            }
        }

        // Return true if the item should be deleted, false otherwise.
        // Only remove something if old has it but not new
        private bool fixId( IDictionary<string, int> oldIds, IDictionary<string, int> newIds, ref int id, IDictionary<int, string> origData )
        {
            if ( origData.ContainsKey( id ) )
                return false;

            if ( reverseFixing )
            {
                if ( newIds.Values.Contains( id ) )
                {
                    int id_ = id;
                    var key = newIds.FirstOrDefault( xTile => xTile.Value == id_ ).Key;

                    if ( oldIds.ContainsKey( key ) )
                    {
                        id = oldIds[ key ];
                        Log.trace( "Changing ID: " + key + " from ID " + id_ + " to " + id );
                        return false;
                    }
                    else
                    {
                        Log.warn( "New item " + key + " with ID " + id_ + "!" );
                        return false;
                    }
                }
                else return false;
            }
            else
            {
                if ( oldIds.Values.Contains( id ) )
                {
                    int id_ = id;
                    var key = oldIds.FirstOrDefault(x => x.Value == id_).Key;

                    if ( newIds.ContainsKey( key ) )
                    {
                        id = newIds[ key ];
                        Log.verbose( "Changing ID: " + key + " from ID " + id_ + " to " + id );
                        return false;
                    }
                    else
                    {
                        Log.trace( "Deleting missing item " + key + " with old ID " + id_ );
                        return true;
                    }
                }
                else return false;
            }
        }
    }
}
