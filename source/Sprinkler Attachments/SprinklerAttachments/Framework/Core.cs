/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Mushymato/StardewMods
**
*************************************************/

using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics.CodeAnalysis;
using System.ComponentModel;
using StardewValley;
using StardewValley.Locations;
using StardewValley.Menus;
using StardewValley.ItemTypeDefinitions;
using StardewValley.GameData.Objects;
using StardewValley.Objects;
using StardewValley.Mods;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley.Inventories;
using StardewValley.TerrainFeatures;
using StardewValley.GameData.Crops;
using StardewObject = StardewValley.Object;
using Microsoft.Xna.Framework.Content;

namespace SprinklerAttachments.Framework
{
    /// <summary>
    /// Helper class for getting custom field and mod data specific to this mod
    /// </summary>
    internal sealed class ModFieldHelper
    {
        public const string Field_IntakeChestAcceptCategory = $"{SprinklerAttachment.ContentModId}.IntakeChestAcceptCategory";
        public const string Field_OverlayOffsetX = $"{SprinklerAttachment.ContentModId}.OverlayOffsetX";
        public const string Field_OverlayOffsetY = $"{SprinklerAttachment.ContentModId}.OverlayOffsetY";
        public const string Field_IsSowing = $"{SprinklerAttachment.ContentModId}.IsSowing";
        public const string Field_IsPressurize = $"{SprinklerAttachment.ContentModId}.IsPressurize";

        public static bool TryGetIntakeChestAcceptCategory(ObjectData data, [NotNullWhen(true)] out string? ret)
        {
            return data.CustomFields.TryGetValue(Field_IntakeChestAcceptCategory, out ret);
        }

        public static bool TryGetModDataIntakeChestAcceptCategory(ModDataDictionary data, [NotNullWhen(true)] out List<int>? ret)
        {
            ret = null;
            if (data.TryGetValue(Field_IntakeChestAcceptCategory, out string? valueStr))
            {
                ret = valueStr.Split(",").ToList().ConvertAll(cat => Convert.ToInt32(cat));
                return true;
            }
            return false;
        }
        public static Vector2 GetOverlayOffset(ObjectData data)
        {
            if (!TryParseCustomField(data, Field_OverlayOffsetX, out int? offsetX))
                offsetX = 0;
            if (!TryParseCustomField(data, Field_OverlayOffsetY, out int? offsetY))
                offsetY = 0;
            return new((float)offsetX, (float)offsetY);
        }
        public static bool IsSowing(ObjectData data)
        {
            return TryParseCustomField(data, Field_IsSowing, out bool ret) && ret;
        }
        public static bool IsPressurize(ObjectData data)
        {
            return TryParseCustomField(data, Field_IsPressurize, out bool ret) && ret;
        }
        private static bool TryParseCustomField<T>(ObjectData data, string key, [NotNullWhen(true)] out T? ret)
        {
            ret = default;
            if (data.CustomFields.TryGetValue(key, out string? valueStr) && valueStr != null)
            {
                return TryParse(valueStr, out ret);
            }
            return false;
        }
        public static bool TryParseModData<T>(ModDataDictionary data, string key, [NotNullWhen(true)] out T? ret)
        {
            ret = default;
            if (data.TryGetValue(key, out string? valueStr) && valueStr != null)
            {
                return TryParse(valueStr, out ret);
            }
            return false;
        }

        private static bool TryParse<T>(string valueStr, out T? ret)
        {
            ret = default;
            TypeConverter con = TypeDescriptor.GetConverter(typeof(T));
            if (con != null)
            {
                try
                {
                    ret = (T?)con.ConvertFromString(valueStr);
                    if (ret != null)
                        return true;
                }
                catch (NotSupportedException)
                {
                    return false;
                }
            }
            return false;
        }
    }
    /// <summary>
    /// Config fields
    /// </summary>
    internal class ModConfig
    {
        public enum Trellis
        {
            Any = 0,
            Rows = 1,
            Columns = 2
        }
        public bool RestrictKrobusStock { get; set; } = true;
        public bool WaterOnPlanting { get; set; } = true;
        public bool EnableForGardenPots { get; set; } = true;
        public Trellis TrellisPattern { get; set; } = Trellis.Rows;
        public int IntakeChestSize { get; set; } = 9;
        public bool PlantOnChestClose { get; set; } = false;
        public bool InvisibleAttachments { get; set; } = false;
        public bool SeasonAwarePlanting = true;

        private void Reset()
        {
            RestrictKrobusStock = true;
            WaterOnPlanting = true;
            EnableForGardenPots = true;
            TrellisPattern = Trellis.Any;
            IntakeChestSize = 9;
            PlantOnChestClose = false;
            InvisibleAttachments = false;
            SeasonAwarePlanting = true;
        }

        public void Register(IModHelper helper, IManifest mod,
                             Integration.IContentPatcherAPI CP, Integration.IGenericModConfigMenuApi? GMCM)
        {
            CP.RegisterToken(mod, nameof(RestrictKrobusStock), () => { return new string[] { RestrictKrobusStock.ToString() }; });
            CP.RegisterToken(mod, nameof(InvisibleAttachments), () => { return new string[] { InvisibleAttachments.ToString() }; });
            if (GMCM == null)
            {
                helper.WriteConfig(this);
                return;
            }
            GMCM.Register(
                mod: mod,
                reset: () =>
                {
                    Reset();
                    helper.WriteConfig(this);
                },
                save: () => { helper.WriteConfig(this); },
                titleScreenOnly: false
            );
            GMCM.AddBoolOption(
                mod,
                getValue: () => { return RestrictKrobusStock; },
                setValue: (value) => { RestrictKrobusStock = value; },
                name: () => helper.Translation.Get("config.RestrictKrobusStock.name"),
                tooltip: () => helper.Translation.Get("config.RestrictKrobusStock.description")
            );
            GMCM.AddBoolOption(
                mod,
                getValue: () => { return InvisibleAttachments; },
                setValue: (value) => { InvisibleAttachments = value; },
                name: () => helper.Translation.Get("config.InvisibleAttachments.name"),
                tooltip: () => helper.Translation.Get("config.InvisibleAttachments.description")
            );
            GMCM.AddBoolOption(
                mod,
                getValue: () => { return WaterOnPlanting; },
                setValue: (value) => { WaterOnPlanting = value; },
                name: () => helper.Translation.Get("config.WaterOnPlanting.name"),
                tooltip: () => helper.Translation.Get("config.WaterOnPlanting.description")
            );
            GMCM.AddBoolOption(
                mod,
                getValue: () => { return EnableForGardenPots; },
                setValue: (value) => { EnableForGardenPots = value; },
                name: () => helper.Translation.Get("config.EnableForGardenPots.name"),
                tooltip: () => helper.Translation.Get("config.EnableForGardenPots.description")
            );
            GMCM.AddNumberOption(
                mod,
                getValue: () => { return (int)TrellisPattern; },
                setValue: (value) => { TrellisPattern = (Trellis)value; },
                formatValue: (value) => { return helper.Translation.Get($"config.TrellisPattern.{value}"); },
                name: () => helper.Translation.Get("config.TrellisPattern.name"),
                tooltip: () => helper.Translation.Get("config.TrellisPattern.description"),
                min: 0, max: 2
            );
            GMCM.AddBoolOption(
                mod,
                getValue: () => { return SeasonAwarePlanting; },
                setValue: (value) => { SeasonAwarePlanting = value; },
                name: () => helper.Translation.Get("config.SeasonAwarePlanting.name"),
                tooltip: () => helper.Translation.Get("config.SeasonAwarePlanting.description")
            );
            GMCM.AddNumberOption(
                mod,
                getValue: () => { return IntakeChestSize; },
                setValue: (value) => { IntakeChestSize = value; },
                name: () => helper.Translation.Get("config.IntakeChestSize.name"),
                tooltip: () => helper.Translation.Get("config.IntakeChestSize.description"),
                min: 3, max: 36, interval: 3
            );
            GMCM.AddBoolOption(
                mod,
                getValue: () => { return PlantOnChestClose; },
                setValue: (value) => { PlantOnChestClose = value; },
                name: () => helper.Translation.Get("config.PlantOnChestClose.name"),
                tooltip: () => helper.Translation.Get("config.PlantOnChestClose.description")
            );
        }
    }
    /// <summary>
    /// Functionality of sprinkler attachments
    /// </summary>
    internal static class SprinklerAttachment
    {
        /// <summary>
        /// ModId of content pack
        /// </summary>
        public const string ContentModId = "mushymato.SprinklerAttachments.CP";
        private static Func<StardewObject, IEnumerable<Vector2>> CompatibleGetSprinklerTiles = GetSprinklerTiles_Vanilla;
        private static Func<Farmer, HoeDirt, Item, bool> CompatibleRemotePlantFertilizer = RemotePlantFertilizer_Vanilla;
        private static Integration.IBetterSprinklersApi? BetterSprinklersApi;
        private static Integration.IUltimateFertilizerApi? UltimateFertilizerApi;
        public static ModConfig? Config;

        public static void SetUpModCompatibility(IModHelper helper)
        {
            // Better Sprinkler
            foreach (string modId in Integration.IBetterSprinklersApi.ModIds)
            {
                if (helper.ModRegistry.IsLoaded(modId))
                {
                    ModEntry.Log($"Apply compatibility changes with BetterSprinklers ({modId})", LogLevel.Trace);
                    BetterSprinklersApi = helper.ModRegistry.GetApi<Integration.IBetterSprinklersApi>(modId);
                    if (BetterSprinklersApi != null)
                    {
                        CompatibleGetSprinklerTiles = GetSprinklerTiles_BetterSprinklersPlus;
                    }
                }
            }

            // Ultimate Fertilizer
            if (helper.ModRegistry.IsLoaded(Integration.IUltimateFertilizerApi.ModId))
            {
                ModEntry.Log($"Apply compatibility changes with UltimateFertilizer ({Integration.IUltimateFertilizerApi.ModId})", LogLevel.Trace);
                UltimateFertilizerApi = helper.ModRegistry.GetApi<Integration.IUltimateFertilizerApi>(Integration.IUltimateFertilizerApi.ModId);
                if (UltimateFertilizerApi != null)
                {
                    CompatibleRemotePlantFertilizer = RemotePlantFertilizer_UltimateFertilizer;
                }
            }
        }

        public static void SetUpModConfigMenu(IModHelper helper, IManifest manifest)
        {
            Integration.IContentPatcherAPI? CP = helper.ModRegistry.GetApi<Integration.IContentPatcherAPI>("Pathoschild.ContentPatcher") ?? throw new ContentLoadException("Failed to get Content Patcher API");
            // IModInfo cpMod = helper.ModRegistry.Get(ContentModId) ?? throw new ContentLoadException($"Required content pack {ContentModId} not loaded");
            Config = helper.ReadConfig<ModConfig>();
            Integration.IGenericModConfigMenuApi? GMCM = helper.ModRegistry.GetApi<Integration.IGenericModConfigMenuApi>("spacechase0.GenericModConfigMenu");
            Config.Register(helper, manifest, CP, GMCM);
        }

        public static void ReadConfig(IModHelper helper)
        {
            helper.ModRegistry.IsLoaded(ContentModId);
        }

        private static List<Vector2> GetSprinklerTiles_BetterSprinklersPlus(StardewObject sprinkler)
        {
            Dictionary<int, Vector2[]> allCoverage = (Dictionary<int, Vector2[]>)BetterSprinklersApi!.GetSprinklerCoverage();
            // BetterSprinklerPlus uses ParentSheetIndex instead of itemId to check sprinkler
            if (allCoverage.TryGetValue(sprinkler.ParentSheetIndex, out Vector2[]? relCoverage) && relCoverage != null)
            {
                Vector2 origin = sprinkler.TileLocation;
                List<Vector2> realCoverage = new();
                foreach (Vector2 rel in relCoverage)
                {
                    realCoverage.Add(origin + rel);
                }
                return realCoverage;
            }
            return sprinkler.GetSprinklerTiles();
        }

        private static List<Vector2> GetSprinklerTiles_Vanilla(StardewObject sprinkler)
        {
            return sprinkler.GetSprinklerTiles();
        }

        /// <summary>
        /// Add attachment to sprinkler object, as part of <see cref="StardewObject.performObjectDropInAction"/>
        /// </summary>
        /// <param name="sprinkler">possible sprinkler object</param>
        /// <param name="attachmentItem">possible attachment held item</param>
        /// <param name="probe">dryrun</param>
        /// <returns>true if attached to sprinkler</returns>
        public static bool TryAttachToSprinkler(StardewObject sprinkler, Item attachmentItem, bool probe)
        {
            if (sprinkler.isTemporarilyInvisible || // item not loaded
                attachmentItem is not StardewObject attachment || // item not an object
                !attachment.HasContextTag(ContentModId) || // not an attachment item for this mod
                ItemRegistry.GetData(attachment.QualifiedItemId)?.RawData is not ObjectData data || // item lack object data (?)
                !sprinkler.IsSprinkler() || // not a sprinkler
                sprinkler.heldObject.Value != null) // already has attached item (vanilla or mod)
                return false;

            if (probe)
                return true; // dryrun stops here

            GameLocation location = sprinkler.Location;
            if (location is MineShaft || location is VolcanoDungeon)
            {
                Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:Object.cs.13053"));
                return false; // ban attachments in dungeons (why would you ever)
            }

            if (attachment.getOne() is not StardewObject attached)
                return false;

            location.playSound("axe");
            sprinkler.heldObject.Value = attached;
            sprinkler.MinutesUntilReady = -1;

            // setup chest if IsSowing is set
            if (ModFieldHelper.TryGetIntakeChestAcceptCategory(data, out string? ret) && attached.heldObject.Value == null)
            {
                Chest intakeChest = new(playerChest: false)
                {
                    SpecialChestType = Chest.SpecialChestTypes.Enricher
                };

                intakeChest.modData.Add(ContentModId, "true");
                intakeChest.modData.Add(ModFieldHelper.Field_IntakeChestAcceptCategory, ret);
                attached.heldObject.Value = intakeChest;
                intakeChest.GetMutex().RequestLock(delegate ()
                {
                    ShowIntakeChestMenu(intakeChest);
                });
            }

            return true;
        }

        /// <summary>
        /// Handle chest mutex updates
        /// <seealso cref="StardewObject.updateWhenCurrentLocation(GameTime)"/>
        /// </summary>
        /// <param name="sprinkler"></param>
        /// <param name="time"></param>
        public static void UpdateWhenCurrentLocation(StardewObject sprinkler, GameTime time)
        {
            if (TryGetIntakeChest(sprinkler, out StardewObject? _, out Chest? intakeChest))
            {
                intakeChest.mutex.Update(sprinkler.Location);
                if (Game1.activeClickableMenu == null && intakeChest.GetMutex().IsLockHeld())
                {
                    if (Config!.PlantOnChestClose)
                        ApplySowing(sprinkler);
                    intakeChest.GetMutex().ReleaseLock();
                }
            }
        }

        /// <summary>
        /// Open intake chest for attachments, if one exists on the sprinkler.
        /// <seealso cref="StardewObject.checkForAction(Farmer, bool)"/>
        /// </summary>
        /// <param name="sprinkler">possible sprinkler object</param>
        /// <param name="who">player performing action, unused</param>
        /// <param name="justCheckingForActivity">dryrun</param>
        /// <returns>true if intake chest opened</returns>
        public static bool CheckForAction(StardewObject sprinkler, Farmer who, bool justCheckingForActivity)
        {
            if (!TryGetSprinklerAttachment(sprinkler, out StardewObject? attachment))
                return false;
            if (justCheckingForActivity)
                return true; // dryrun stops here
            if (!Game1.didPlayerJustRightClick(ignoreNonMouseHeldInput: true)) // TODO: how does this work with controllers?
                return false;
            if (attachment.heldObject.Value is not Chest intakeChest)
                return false;

            intakeChest.GetMutex().RequestLock(delegate ()
            {
                ShowIntakeChestMenu(intakeChest);
            });
            return true;
        }

        /// <summary>
        /// Add 1 to sprinkler radius if the attachment has pressurize functionality
        /// </summary>
        /// <param name="sprinkler">possible sprinkler object</param>
        /// <param name="originalRadius">vanilla radius <see cref="StardewObject.GetModifiedRadiusForSprinkler"/></param>
        /// <returns></returns>
        public static int GetModifiedRadiusForSprinkler(StardewObject sprinkler, int originalRadius)
        {
            if (originalRadius >= 0 &&
                TryGetSprinklerAttachment(sprinkler, out StardewObject? attachment) &&
                ItemRegistry.GetData(attachment.QualifiedItemId)?.RawData is ObjectData data &&
                ModFieldHelper.IsPressurize(data))
                return originalRadius + 1;
            return originalRadius;
        }

        /// <summary>
        /// Override the chest size for sprinkler
        /// <seealso cref="Chest.GetActualCapacity"/>
        /// </summary>
        /// <param name="intakeChest">Chest object instance</param>
        /// <param name="originalValue">vanilla capacity <see cref="Chest.GetActualCapacity"/></param>
        /// <returns>int capacity for chest</returns>
        public static int GetActualCapacity(Chest intakeChest, int originalValue)
        {
            if (ModFieldHelper.TryParseModData(intakeChest.modData, ContentModId, out bool? ret) && (bool)ret)
                return Config?.IntakeChestSize ?? originalValue;
            return originalValue;
        }

        public static void ApplySowingToAllSprinklers(bool verbose = false)
        {
            foreach (GameLocation location in Game1.locations)
            {

                if (location.GetData()?.CanPlantHere ?? location.IsFarm)
                {
                    location.objects.Lock();
                    foreach (KeyValuePair<Vector2, StardewObject> pair in location.objects.Pairs)
                    {
                        if (pair.Value.IsSprinkler())
                        {
                            if (verbose) ModEntry.Log($"Try ApplySowing on sprinkler {pair.Value.Name} ({pair.Key}:{location})", LogLevel.Debug);
                            ApplySowing(pair.Value);
                        }
                    }
                    location.objects.Unlock();
                }
            }
        }

        public static bool DrawAttachment(StardewObject sprinkler, SpriteBatch spriteBatch, int x, int y, float alpha)
        {
            if (TryGetSprinklerAttachment(sprinkler, out StardewObject? attachment))
            {
                Vector2 offset = Vector2.Zero;
                Rectangle bounds = sprinkler.GetBoundingBoxAt(x, y);
                if (!(Config?.InvisibleAttachments ?? false))
                {
                    ParsedItemData parsedData = ItemRegistry.GetDataOrErrorItem(attachment.QualifiedItemId);
                    Rectangle sourceRect = parsedData.GetSourceRect(1);
                    sourceRect.Height += 2; // add 2 since sprites for this mod are 18 tall
                    if (ItemRegistry.GetData(attachment.QualifiedItemId)?.RawData is ObjectData data)
                        offset = ModFieldHelper.GetOverlayOffset(data);
                    spriteBatch.Draw(
                        parsedData.GetTexture(),
                        Game1.GlobalToLocal(Game1.viewport, new Vector2(x * 64 + 32 + ((sprinkler.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0), y * 64 + 32 + ((sprinkler.shakeTimer > 0) ? Game1.random.Next(-1, 2) : 0)) + offset),
                        sourceRect,
                        Color.White * alpha, 0f, new Vector2(8f, 8f), (sprinkler.scale.Y > 1f) ? sprinkler.getScale().Y : 4f,
                        sprinkler.Flipped ? SpriteEffects.FlipHorizontally : SpriteEffects.None,
                        (float)(sprinkler.isPassable() ? bounds.Top : bounds.Bottom) / 10000f + 1E-05f
                    );
                }
                if (sprinkler.SpecialVariable == 999999)
                {
                    if (offset.Y != 0)
                        Torch.drawBasicTorch(spriteBatch, (float)(x * 64) - 2f, y * 64 - 32, (float)bounds.Bottom / 10000f + 1E-06f);
                    else
                        Torch.drawBasicTorch(spriteBatch, (float)(x * 64) - 2f, y * 64 - 32 + 12, (float)(bounds.Bottom + 2) / 10000f);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Open intake chest menu.
        /// </summary>
        /// <param name="chest">intake chest</param>
        /// <param name="data">object data of attachment that holds chest</param>
        private static void ShowIntakeChestMenu(Chest intakeChest)
        {
            InventoryMenu.highlightThisItem highlightFunction;
            if (ModFieldHelper.TryGetModDataIntakeChestAcceptCategory(intakeChest.modData, out List<int>? ret))
            {
                highlightFunction = (Item item) => { return ret.Contains(item.Category); };
            }
            else
            {
                highlightFunction = InventoryMenu.highlightAllItems;
            }
            ItemGrabMenu? oldMenu = Game1.activeClickableMenu as ItemGrabMenu;
            Game1.activeClickableMenu = new ItemGrabMenu(
                inventory: intakeChest.GetItemsForPlayer(),
                reverseGrab: false,
                showReceivingMenu: true,
                highlightFunction: highlightFunction,
                behaviorOnItemSelectFunction: (Item item, Farmer who) => GrabItemFromInventory(intakeChest, item, who),
                message: null,
                behaviorOnItemGrab: (Item item, Farmer who) => GrabItemFromChest(intakeChest, item, who),
                snapToBottom: false,
                canBeExitedWithKey: true,
                playRightClickSound: true,
                allowRightClick: true,
                showOrganizeButton: false,
                source: 1,
                sourceItem: intakeChest.fridge.Value ? null : intakeChest,
                whichSpecialButton: -1,
                context: intakeChest
            );
            if (oldMenu != null && Game1.activeClickableMenu is ItemGrabMenu newMenu)
            {
                newMenu.inventory.moveItemSound = oldMenu.inventory.moveItemSound;
                newMenu.inventory.highlightMethod = oldMenu.inventory.highlightMethod;
            }
        }

        private static void GrabItemFromInventory(Chest intakeChest, Item item, Farmer who)
        {
            if (item.Stack == 0)
            {
                item.Stack = 1;
            }
            Item tmp = intakeChest.addItem(item);
            if (tmp == null)
            {
                who.removeItemFromInventory(item);
            }
            else
            {
                tmp = who.addItemToInventory(tmp);
            }
            intakeChest.clearNulls();
            int oldID = (Game1.activeClickableMenu.currentlySnappedComponent != null) ? Game1.activeClickableMenu.currentlySnappedComponent.myID : (-1);
            ShowIntakeChestMenu(intakeChest);
            (Game1.activeClickableMenu as ItemGrabMenu)!.heldItem = tmp;
            if (oldID != -1)
            {
                Game1.activeClickableMenu.currentlySnappedComponent = Game1.activeClickableMenu.getComponentWithID(oldID);
                Game1.activeClickableMenu.snapCursorToCurrentSnappedComponent();
            }
        }

        public static void GrabItemFromChest(Chest intakeChest, Item item, Farmer who)
        {
            if (who.couldInventoryAcceptThisItem(item))
            {
                intakeChest.GetItemsForPlayer().Remove(item);
                intakeChest.clearNulls();
                ShowIntakeChestMenu(intakeChest);
            }
        }


        /// <summary>
        /// Get hoe dirt (from terrain or pot) and check if it is valid for planting
        /// </summary>
        /// <param name="location"></param>
        /// <param name="current"></param>
        /// <param name="candidate"></param>
        /// <returns></returns>
        public static bool TryGetCandidateDirt(GameLocation location, Vector2 current, [NotNullWhen(true)] out HoeDirt? candidate)
        {
            candidate = null;
            if (Config?.EnableForGardenPots ?? true)
            {
                if (location.getObjectAtTile((int)current.X, (int)current.Y) is IndoorPot pot1)
                {
                    candidate = pot1.hoeDirt.Value;
                }
                // special case carpets
                else if (location.getObjectAtTile((int)current.X, (int)current.Y, ignorePassables: true) is IndoorPot pot2)
                {
                    candidate = pot2.hoeDirt.Value;
                }
            }
            if (candidate == null && location.terrainFeatures.TryGetValue(current, out var terrain) && terrain is HoeDirt dirt)
            {
                candidate = dirt;
            }
            return candidate != null && (candidate.crop == null || !candidate.HasFertilizer());
        }

        /// <summary>
        /// Get open (no crop or no fertilizer) dirt within the sprinkler's range.
        /// </summary>
        /// <param name="sprinkler">sprinkler object</param>
        /// <param name="dirtList">list of dirt that are open</param>
        /// <returns>True if at least 1 open hoed dirt is found</returns>
        public static bool TryGetOpenHoedDirtAroundSprinkler(StardewObject sprinkler, [NotNullWhen(true)] out List<HoeDirt>? dirtList)
        {
            GameLocation location = sprinkler.Location;
            Vector2 sprinklerTile = sprinkler.TileLocation;
            dirtList = new();
            foreach (Vector2 current in (CompatibleGetSprinklerTiles ?? GetSprinklerTiles_Vanilla)(sprinkler))
            {
                if (TryGetCandidateDirt(location, current, out HoeDirt? candidate))
                    dirtList.Add(candidate);
            }
            return dirtList.Count > 0;
        }

        public static void ApplySowing(StardewObject sprinkler)
        {
            if (TryGetIntakeChest(sprinkler, out StardewObject? attachment, out Chest? intakeChest) &&
                ItemRegistry.GetData(attachment.QualifiedItemId)?.RawData is ObjectData data &&
                ModFieldHelper.IsSowing(data) &&
                TryGetOpenHoedDirtAroundSprinkler(sprinkler, out List<HoeDirt>? dirtList) &&
                intakeChest.Items.Count > 0 &&
                intakeChest.Items[0] != null)
            {
                PlantFromIntakeChest(dirtList, sprinkler.TileLocation, intakeChest);
            }
        }


        public static bool TryGetSprinklerAttachment(StardewObject sprinkler, [NotNullWhen(true)] out StardewObject? attachment)
        {
            attachment = null;
            if (sprinkler.IsSprinkler() && sprinkler.heldObject.Value is StardewObject held && held.HasContextTag(ContentModId))
            {
                attachment = held;
                return true;
            }
            return false;
        }

        public static bool TryGetIntakeChest(StardewObject sprinkler, [NotNullWhen(true)] out StardewObject? attachment, [NotNullWhen(true)] out Chest? intakeChest)
        {
            intakeChest = null;
            if (TryGetSprinklerAttachment(sprinkler, out attachment))
            {
                if (attachment.heldObject.Value is Chest intake)
                {
                    intakeChest = intake;
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Plant seeds and apply fertilizer from chest
        /// </summary>
        /// <param name="dirtList"></param>
        /// <param name="sprinklerPos"></param>
        /// <param name="intakeChest"></param>
        private static void PlantFromIntakeChest(List<HoeDirt> dirtList, Vector2 sprinklerPos, Chest intakeChest)
        {
            Inventory chestItems = intakeChest.Items;
            int seedIdx = 0;
            int fertIdx = 0;
            bool fertilized = false;
            List<Tuple<int, Item>> appliedFert = new(); // for case where more than 1 fertilizer is applied
            Item? fertilizer = null;
            Farmer who = GetAgriculturistFarmer();

            // sort trellis crops first
            List<int> seedMapping = Enumerable.Range(0, chestItems.Count).ToList();
            seedMapping.Sort((int x, int y) =>
            {
                Item itemX = chestItems[x];
                if (!Crop.TryGetData(itemX.ItemId, out CropData cropDataX))
                    return 1;
                Item itemY = chestItems[y];
                if (!Crop.TryGetData(itemY.ItemId, out CropData cropDataY))
                    return -1;
                // trellis first
                if (cropDataX.IsRaised && cropDataX.IsRaised != cropDataY.IsRaised)
                    return -1;
                return 0;
            });
            List<int> fertilizerMapping = Enumerable.Range(0, chestItems.Count).ToList();
            fertilizerMapping.Sort((int x, int y) =>
            {
                Item itemX = chestItems[x];
                if (itemX.Category != StardewObject.fertilizerCategory)
                    return 1;
                Item itemY = chestItems[y];
                HoeDirt fakeDirt = new();
                if (itemY.Category != StardewObject.fertilizerCategory)
                    return -1;
                fakeDirt.fertilizer.Value = itemX.QualifiedItemId;
                float speedBoostX = fakeDirt.GetFertilizerSpeedBoost();
                float qualityX = fakeDirt.GetFertilizerQualityBoostLevel();
                float retentionX = fakeDirt.GetFertilizerWaterRetentionChance();
                fakeDirt.fertilizer.Value = itemY.QualifiedItemId;
                float speedBoostY = fakeDirt.GetFertilizerSpeedBoost();
                float qualityY = fakeDirt.GetFertilizerQualityBoostLevel();
                float retentionY = fakeDirt.GetFertilizerWaterRetentionChance();
                if (speedBoostX != speedBoostY)
                    return speedBoostY.CompareTo(speedBoostX);
                if (qualityX != qualityY)
                    return qualityY.CompareTo(qualityX);
                if (retentionX != retentionY)
                    return retentionY.CompareTo(retentionX);
                return 0;
            });

            static void decrementStack(int idx, Item item)
            {
                item.Stack--;
                if (item.Stack <= 0)
                    idx++;
            }

            Action<HoeDirt> doFertilizer;
            if (UltimateFertilizerApi is not null)
            {
                // ultimate fertilizer
                doFertilizer = (HoeDirt dirt) =>
                {
                    fertilized = false;
                    appliedFert.Clear();
                    fertIdx = 0;
                    while (NextMatching(chestItems, StardewObject.fertilizerCategory, fertilizerMapping, ref fertIdx, out fertilizer))
                    {
                        if (CompatibleRemotePlantFertilizer!(who, dirt, fertilizer))
                        {
                            appliedFert.Add(new(fertIdx, fertilizer));
                            fertilized = true;
                        }
                        fertIdx++;
                    }
                };
            }
            else
            {
                // vanilla
                doFertilizer = (HoeDirt dirt) =>
                {
                    fertilized = (
                        NextMatching(chestItems, StardewObject.fertilizerCategory, fertilizerMapping, ref fertIdx, out fertilizer) &&
                        CompatibleRemotePlantFertilizer!(who, dirt, fertilizer)
                    );
                };
            }


            foreach (HoeDirt dirt in dirtList)
            {
                // fertilize any unfertilized crops (e.g. ones that arent planted by attachment)
                if (dirt.crop != null)
                {
                    doFertilizer(dirt);
                    if (fertilized)
                    {
                        if (appliedFert.Count > 0)
                        {
                            foreach ((int fIdx, Item fert) in appliedFert)
                            {
                                decrementStack(fIdx, fert);
                            }
                        }
                        else
                        {
                            decrementStack(fertIdx, fertilizer!);
                        }
                    }
                    continue;
                }
                // plant new crop
                while (NextMatching(chestItems, StardewObject.SeedsCategory, seedMapping, ref seedIdx, out Item? seed))
                {
                    if (RemotePlantCrop(who, dirt, sprinklerPos, seed, out bool ignoreSeasons, out CropData? cropData))
                    {
                        doFertilizer(dirt);
                        // check that planted crop can be harvested in time
                        if (!ignoreSeasons && Config!.SeasonAwarePlanting)
                        {
                            int growthDays = dirt.crop!.phaseDays.Take(dirt.crop.phaseDays.Count - 1).Sum();
                            if (growthDays > (WorldDate.DaysPerMonth - Game1.dayOfMonth))
                            {
                                Season season = dirt.Location.GetSeason();
                                Season expected = season + (int)Math.Ceiling((growthDays - (WorldDate.DaysPerMonth - Game1.dayOfMonth)) / (decimal)WorldDate.DaysPerMonth);
                                // cannot harvest, revert planting and continue to next seed
                                if (!cropData.Seasons.Contains(expected))
                                {
                                    if (fertilized)
                                        dirt.fertilizer.Value = null;
                                    dirt.crop = null;
                                    seedMapping[seedIdx] = -1; // ban this seed from planting
                                    continue;
                                }
                            }
                        }
                        // decrement items for reals
                        decrementStack(seedIdx, seed);
                        Game1.stats.SeedsSown++;
                        if (fertilized)
                        {
                            if (appliedFert.Count > 0)
                            {
                                foreach ((int fIdx, Item fert) in appliedFert)
                                {
                                    decrementStack(fIdx, fert);
                                }
                            }
                            else
                            {
                                decrementStack(fertIdx, fertilizer!);
                            }
                        }
                        break;
                    }
                    else
                    {
                        seedIdx++;
                    }
                };
                // due to the trellis pattern feature, we must check all seed on all dirt
                seedIdx = 0;
            }
            // set all 0 item slots to null
            for (int i = 0; i < chestItems.Count; i++)
            {
                if (chestItems[i].Stack <= 0)
                    chestItems[i] = null;
            }
        }

        /// <summary>
        /// Find an agriculturist player if one is online.
        /// </summary>
        /// <returns></returns>
        private static Farmer GetAgriculturistFarmer()
        {
            if (!Game1.IsMultiplayer)
                return Game1.player;
            foreach (Farmer player in Game1.getOnlineFarmers())
            {
                if (player.professions.Contains(Farmer.agriculturist))
                    return player;
            }
            return Game1.player;
        }

        private static bool NextMatching(Inventory chestItems, int category, List<int> idxMapping, ref int curr, [NotNullWhen(true)] out Item? next)
        {
            next = null;
            while (curr < chestItems.Count)
            {
                if (idxMapping[curr] != -1)
                {
                    next = chestItems[idxMapping[curr]];
                    if (next.Stack > 0 && next.Category == category)
                        return true;
                }
                curr++;
            }
            return false;
        }

        private static bool RemotePlantFertilizer_Vanilla(Farmer who, HoeDirt dirt, Item item)
        {
            if (dirt.CanApplyFertilizer(item.ItemId))
            {
                dirt.fertilizer.Value = item.QualifiedItemId;
                dirt.applySpeedIncreases(who);
                return true;
            }
            return false;
        }

        private static bool RemotePlantFertilizer_UltimateFertilizer(Farmer who, HoeDirt dirt, Item item)
        {
            if (dirt.CanApplyFertilizer(item.ItemId))
            {
                return UltimateFertilizerApi!.ApplyFertilizerOnDirt(dirt, item.ItemId, who);
            }
            return false;
        }

        private static bool RemotePlantCrop(Farmer who, HoeDirt dirt, Vector2 sprinklerPos, Item item, out bool ignoreSeasons, [NotNullWhen(true)] out CropData? cropData)
        {
            cropData = null;
            ignoreSeasons = false;
            if (dirt.crop != null)
                return false;
            GameLocation location = dirt.Location;
            string itemId = Crop.ResolveSeedId(item.ItemId, location);
            if (!Crop.TryGetData(itemId, out cropData) || cropData.Seasons.Count == 0)
                return false;
            Point tilePos = Utility.Vector2ToPoint(dirt.Tile);
            bool isGardenPot = location.objects.TryGetValue(dirt.Tile, out StardewObject obj) && obj is IndoorPot;

            if (!isGardenPot)
            {
                if (!(location.farmers.Any((farmer) => dirt.Tile == farmer.Tile) || location.CanItemBePlacedHere(dirt.Tile, useFarmerTile: true)))
                    return false;
                if (cropData.IsRaised)
                {
                    ModConfig.Trellis trellisPattern = Config?.TrellisPattern ?? ModConfig.Trellis.Any;
                    if (trellisPattern == ModConfig.Trellis.Rows && (tilePos.Y - sprinklerPos.Y + 1) % 3 == 0)
                        return false;
                    if (trellisPattern == ModConfig.Trellis.Columns && (tilePos.X - sprinklerPos.X + 1) % 3 == 0)
                        return false;
                }
            }

            bool isIndoorPot = isGardenPot && !location.IsOutdoors;
            if (!location.CanPlantSeedsHere(itemId, tilePos.X, tilePos.Y, isGardenPot, out string _deniedMsg))
                return false;
            Season season = location.GetSeason();
            ignoreSeasons = isIndoorPot || location.SeedsIgnoreSeasonsHere();
            if (!isIndoorPot && !location.SeedsIgnoreSeasonsHere() && (!cropData.Seasons.Contains(season)))
                return false;

            dirt.crop = new Crop(itemId, tilePos.X, tilePos.Y, location);
            dirt.applySpeedIncreases(who);
            if (dirt.hasPaddyCrop() && dirt.paddyWaterCheck())
            {
                dirt.state.Value = 1;
                dirt.updateNeighbors();
            }
            // water any newly planted crops, so they will sprout on day begin
            else if (Config!.WaterOnPlanting &&
                     !(dirt.Location.doesTileHavePropertyNoNull((int)dirt.Tile.X, (int)dirt.Tile.Y, "NoSprinklers", "Back") == "T") &&
                     dirt.state.Value != 2)
            {
                dirt.state.Value = 1;
            }
            return true;
        }
    }
}
