/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Sonozuki/StardewMods
**
*************************************************/

using Harmony;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Outerwear.Models.Converted;
using Outerwear.Models.Parsed;
using Outerwear.Patches;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace Outerwear
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod, IAssetEditor
    {
        /*********
        ** Accessors
        *********/
        /// <summary>All the loaded outerwear data.</summary>
        public List<OuterwearData> OuterwearData { get; } = new List<OuterwearData>();

        /// <summary>Provides basic outerwear apis.</summary>
        public IApi Api { get; private set; }

        /// <summary>The sprite that is drawn in the outerwear slot when no outerwear item is in it.</summary>
        public Texture2D OuterwearSlotPlaceholder { get; private set; }

        /// <summary>The singleton instance of <see cref="ModEntry"/>.</summary>
        public static ModEntry Instance { get; private set; }


        /*********
        ** Public Methods
        *********/
        /// <inheritdoc/>
        public override void Entry(IModHelper helper)
        {
            Instance = this;
            Api = new Api();

            var outerwearSlotPlaceholderPath = Path.Combine("assets", "OuterwearPlaceholder.png");
            if (!File.Exists(Path.Combine(this.Helper.DirectoryPath, outerwearSlotPlaceholderPath)))
            {
                this.Monitor.Log($"No asset could be found at: \"{outerwearSlotPlaceholderPath}\", please try reinstalling the mod.", LogLevel.Error);
                return;
            }
            OuterwearSlotPlaceholder = this.Helper.Content.Load<Texture2D>(outerwearSlotPlaceholderPath);

            ApplyHarmonyPatches();

            this.Helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            this.Helper.Events.GameLoop.Saved += (sender, e) => FixStaminaAndHealth();
            this.Helper.Events.GameLoop.UpdateTicked += (sender, e) => OuterwearEffectsApplier.Update(e.IsOneSecond);
        }

        /// <inheritdoc/>
        public override object GetApi() => Api;

        /// <inheritdoc/>
        public bool CanEdit<T>(IAssetInfo asset) => asset.AssetNameEquals("TileSheets/BuffsIcons");

        /// <inheritdoc/>
        public void Edit<T>(IAssetData asset)
        {
            var customBuffIconsTexture = this.Helper.Content.Load<Texture2D>("assets/CustomBuffIcons.png");

            // replace the max stamina icon
            asset.AsImage().PatchImage(customBuffIconsTexture, sourceArea: new Rectangle(0, 0, 16, 16), targetArea: new Rectangle(64, 16, 16, 16));

            // add the custom icons
            asset.AsImage().PatchImage(customBuffIconsTexture, sourceArea: new Rectangle(16, 0, 96, 16), targetArea: new Rectangle(80, 32, 96, 16));
        }


        /*********
        ** Private Methods
        *********/
        /// <summary>Invoked when a save is loaded.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event data.</param>
        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            OuterwearData.Clear();

            LoadContentPacks();

            FixStaminaAndHealth();
        }

        /// <summary>Corrects the stamina and health of the player accomodating for equipped outerwear.</summary>
        /// <remarks>This is required because before the game saves, it'll remove all buffs. This means the max health and max stamina don't take into account the outerwear buffs.</remarks>
        private void FixStaminaAndHealth()
        {
            var equippedOuterwear = Api.GetEquippedOuterwear();
            if (equippedOuterwear == null)
                return;
            var equippedOuterwearData = Api.GetOuterwearData(equippedOuterwear.ParentSheetIndex);
            if (equippedOuterwearData == null)
                return;

            Game1.player.health = Game1.player.maxHealth + equippedOuterwearData.Effects.MaxHealthIncrease;

            if (Game1.player.stamina == Game1.player.MaxStamina) // only increase stamina if it's full, this is so if the player was exhausted, it won't fill up their stamina
                Game1.player.stamina = Game1.player.MaxStamina + equippedOuterwearData.Effects.MaxStaminaIncrease;
        }

        /// <summary>Applies the harmony patches for patching game code.</summary>
        private void ApplyHarmonyPatches()
        {
            // create a new harmony instance for patching game code
            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);

            // apply the patches
            foreach (var constructorInfo in typeof(Buff).GetConstructors())
                harmony.Patch(
                    original: constructorInfo,
                    postfix: new HarmonyMethod(AccessTools.Method(typeof(BuffPatch), nameof(BuffPatch.ConstructorPostFix)))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(Buff), nameof(Buff.getClickableComponents)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(BuffPatch), nameof(BuffPatch.GetClickableComponentsPostFix)))
            );

            foreach (var constructorInfo in typeof(Farmer).GetConstructors())
                harmony.Patch(
                    original: constructorInfo,
                    transpiler: new HarmonyMethod(AccessTools.Method(typeof(FarmerPatch), nameof(FarmerPatch.ConstructorTranspile)))
                );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.addBuffAttributes)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmerPatch), nameof(FarmerPatch.AddBuffAttributesPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.removeBuffAttributes), new[] { typeof(int[]) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmerPatch), nameof(FarmerPatch.RemoveBuffAttributesPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(Farmer), nameof(Farmer.ClearBuffs)),
                prefix: new HarmonyMethod(AccessTools.Method(typeof(FarmerPatch), nameof(FarmerPatch.ClearBuffsPrefix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(FarmerRenderer), nameof(FarmerRenderer.draw), new Type[] { typeof(SpriteBatch), typeof(FarmerSprite.AnimationFrame), typeof(int), typeof(Rectangle), typeof(Vector2), typeof(Vector2), typeof(float), typeof(int), typeof(Color), typeof(float), typeof(float), typeof(Farmer) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(FarmerRendererPatch), nameof(FarmerRendererPatch.DrawPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Constructor(typeof(InventoryPage), new Type[] { typeof(int), typeof(int), typeof(int), typeof(int) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.ConstructorPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.performHoverAction)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.PerformHoverActionPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.receiveLeftClick)),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.ReceiveLeftClickPostFix)))
            );

            harmony.Patch(
                original: AccessTools.Method(typeof(InventoryPage), nameof(InventoryPage.draw), new Type[] { typeof(SpriteBatch) }),
                postfix: new HarmonyMethod(AccessTools.Method(typeof(InventoryPagePatch), nameof(InventoryPagePatch.DrawPostFix)))
            );
        }

        /// <summary>Loads all the content packs.</summary>
        private void LoadContentPacks()
        {
            foreach (var contentPack in this.Helper.ContentPacks.GetOwned())
            {
                this.Monitor.Log($"Loading {contentPack.Manifest.Name}", LogLevel.Info);

                // try loading content file
                if (!contentPack.HasFile("content.json"))
                {
                    this.Monitor.Log("\"content.json\" couldn't be found, skipping", LogLevel.Error);
                    continue;
                }

                var parsedOverwearDatas = contentPack.LoadAsset<List<ParsedOuterwearData>>("content.json");
                foreach (var parsedOverwearData in parsedOverwearDatas)
                {
                    // try loading asset file
                    if (!contentPack.HasFile(Path.Combine(parsedOverwearData.Asset)))
                    {
                        this.Monitor.Log($"Specified asset: \"{parsedOverwearData.Asset}\" couldn't be found, skipping", LogLevel.Error);
                        continue;
                    }

                    var equippedtexture = contentPack.LoadAsset<Texture2D>(parsedOverwearData.Asset);
                    OuterwearData.Add(new OuterwearData(Utilities.ResolveToken(parsedOverwearData.ObjectId), parsedOverwearData.Type, parsedOverwearData.Effects, equippedtexture));
                }
            }
        }
    }
}
