using System.Collections.Generic;
using System.Reflection.Emit;
using StardewModdingAPI;
using StardewValley.Menus;
using StardewValley;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley.Objects;
using StardewHack;
using StardewValley.Locations;

namespace BiggerBackpack
{
    public class Mod : Hack<Mod>, IAssetEditor, IAssetLoader
    {
        public static Mod instance;

        private static Texture2D bigBackpack;

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void HackEntry(IModHelper helper)
        {
            bigBackpack = Helper.Content.Load<Texture2D>("LooseSprites/BiggerBackpack", ContentSource.GameContent);

            Helper.ConsoleCommands.Add("player_setbackpacksize", "Set the size of the player's backpack. This must be 12, 24, 36 or 48", command);
            
            Patch((SeedShop s)=>s.draw(null), SeedShop_draw);
            Patch((SpecialItem si)=>si.getTemporarySpriteForHoldingUp(new Vector2()), SpecialItem_getTemporarySpriteForHoldingUp);
            Patch((GameLocation gl)=>gl.performAction("", null, new xTile.Dimensions.Location()), GameLocation_performAction);
            Patch((GameLocation gl)=>gl.answerDialogueAction("", null), GameLocation_answerDialogueAction);
            Patch(()=>new InventoryPage(0,0,0,0), InventoryPage_ctor);
            Patch((InventoryPage ip)=>ip.draw(null), InventoryPage_draw);
            Patch(()=>new CraftingPage(0,0,0,0,false,false,null), CraftingPage_ctor);
            Patch(()=>new ShopMenu(new List<ISalable>(),0,"",null,null,""), ShopMenu_ctor);
            Patch((ShopMenu m)=>m.draw(null), ShopMenu_draw);
            Patch((ShopMenu m)=>m.drawCurrency(null), ShopMenu_drawCurrency);
            Patch(()=>new MenuWithInventory(null,false,false,0,0,0), ShippingMenu_ctor);
            Patch((JunimoNoteMenu m)=>m.setUpMenu(0,null), JunimoNoteMenu_setUpMenu);
        }

        private void command( string cmd, string[] args )
        {
            if (args.Length != 1)
            {
                Monitor.Log("Must have one command argument", LogLevel.Error);
                return;
            }

            // Parse the new size. Make sure it's a valid size. 
            int newMax;
            switch (args[0]) {
                case "12": newMax = 12; break;
                case "24": newMax = 24; break;
                case "36": newMax = 36; break;
                case "48": newMax = 48; break;
                default:
                    Monitor.Log("The new size must be 12, 24, 36 or 48.", LogLevel.Error);
                    return;
            }
            
            if (newMax < Game1.player.MaxItems)
            {
                // Shrink the inventory, spilling any items in the removed slots to the ground.
                for (int i = Game1.player.MaxItems - 1; i >= newMax; --i) {
                    if (Game1.player.Items[i] != null) {
                        // Spill the item.
                        Game1.createItemDebris(Game1.player.Items[i], Game1.player.getStandingPosition(), Game1.player.getDirection(), null, -1);
                    }
                    Game1.player.Items.RemoveAt(i);
                }
            }
            else
            {
                // Grow the inventory.
                for (int i = Game1.player.Items.Count; i < Game1.player.MaxItems; ++i) {
                    Game1.player.Items.Add(null);
                }
            }
            Game1.player.MaxItems = newMax;
        }

#region Draw Premium Backpack
        public static void drawBiggerBackpack(SpriteBatch b) {
            b.Draw(bigBackpack, Game1.GlobalToLocal(new Vector2 (456f, 1088f)), new Rectangle(0, 0, 12, 14), Color.White, 0.0f, Vector2.Zero, Game1.pixelZoom, SpriteEffects.None, (float)(19.25 * Game1.tileSize / 10000.0));
        }

        // Inject code for rendering the larger backpack in the shop.
        void SeedShop_draw() {
            var check = FindCode(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                OpCodes.Call,
                Instructions.Ldc_I4_S(36),
                OpCodes.Bge
            );
            
            var pos = check.Follow(4);
            
            // Do a sanity check
            if ((pos[-1].opcode == OpCodes.Ret || pos[-1].opcode == OpCodes.Br) &&
                  pos[0].opcode == OpCodes.Ldarg_1) {
                // Ok
            } else {
                throw new System.Exception("Jump does not go to expected location.");
            }
            
            // Inject check and call to drawBiggerBackpack.
            pos.Insert(0,
                // else if (maxItems < 48)
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                check[2], // Nothing jumps here so reusing this should be OK.
                Instructions.Ldc_I4_S(48),
                check[4], // We'll create a new jump in check later.
                // drawBiggerBackpack(b);
                Instructions.Ldarg_1(),
                Instructions.Call(typeof(Mod), nameof(Mod.drawBiggerBackpack), typeof(SpriteBatch))
                // }
            );
            
            // Create a new jump in check.
            check[4] = Instructions.Bge(AttachLabel(pos[0]));
        }
        
        static public TemporaryAnimatedSprite getBackpackSprite(Vector2 position) {
            return new TemporaryAnimatedSprite (null, new Rectangle(1, 0, 11, 13), position + new Vector2 (16f, 0f), false, 0f, Color.White) {
                scale = 4f,
                layerDepth = 1f,
                texture = bigBackpack
            };
        }
        
        // Inject code for rendering the larger backpack when picked up.
        void SpecialItem_getTemporarySpriteForHoldingUp() {
            var code = FindCode(
                Instructions.Ldstr("LooseSprites\\Cursors"),
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                OpCodes.Call,
                Instructions.Ldc_I4_S(36)
                // Beq or Bne_un
            );
            
            code.Prepend(
                // if (maxItems==48) {
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                code[3], // Nothing jumps here so reusing this should be OK.
                Instructions.Ldc_I4_S(48),
                Instructions.Bne_Un(AttachLabel(code[0])),
                // return getBackpackSprite(position);
                Instructions.Ldarg_1(),
                Instructions.Call(typeof(Mod), nameof(Mod.getBackpackSprite), typeof(Vector2)),
                Instructions.Ret()
                // }
            );
        }
#endregion

#region Buy Backpack
        static public void clickBackpack()
        {
            Response yes = new Response("Purchase", "Purchase (50,000g)");
            Response no = new Response("Not", Game1.content.LoadString("Strings\\Locations:SeedShop_BuyBackpack_ResponseNo"));
            Response[] resps = new Response[] { yes, no };
            Game1.currentLocation.createQuestionDialogue("Backpack Upgrade -- 48 slots", resps, "Backpack");
        }
        
        // Inject code to show the buying dialogue when the premium backpack  is clicked.
        void GameLocation_performAction() {
            var code = FindCode(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                OpCodes.Call,
                Instructions.Ldc_I4_S(36),
                OpCodes.Bge
            );
            code.Extend(
                Instructions.Ldstr("Backpack"),
                OpCodes.Call,
                OpCodes.Br
            );
            var len = code.length;
            code.Append(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                code[2],
                Instructions.Ldc_I4_S(48),
                code[4],
                Instructions.Call(typeof(Mod), nameof(Mod.clickBackpack)),
                Instructions.Br((Label)code[len-1].operand)
            );
            code[4] = Instructions.Bge(AttachLabel(code[len]));
        }

        static void buyBackpack() {
            Game1.player.Money -= 50000;
            Game1.player.holdUpItemThenMessage((Item)new SpecialItem(99, "Premium Pack") { DisplayName = "Premium Pack" }, true);
            Game1.player.increaseBackpackSize(12);
            // Game1.multiplayer.globalChatInfoMessage ("BackpackDeluxe", Game1.player.Name);
        }
        
        // Inject code for rendering the larger backpack when picked up.
        void GameLocation_answerDialogueAction() {
            var code = FindCode(
                // else if ((int)Game1.player.maxItems != 36) {
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                OpCodes.Call,
                Instructions.Ldc_I4_S(36),
                OpCodes.Beq
            );
            var get_player = Instructions.Call_get(typeof(Game1), nameof(Game1.player));
            code.Replace(
                // else if ((int)Game1.player.maxItems < 36
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                code[2],
                Instructions.Ldc_I4_S(48),
                Instructions.Bge(AttachLabel(get_player)),
                //   && Game1.player.Money >= 50000) {
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Callvirt_get(typeof(Farmer), nameof(Farmer.Money)),
                Instructions.Ldc_I4(50000),
                Instructions.Blt(AttachLabel(get_player)),
                //   buyBackpack();
                Instructions.Call(typeof(Mod), nameof(Mod.buyBackpack)),
                // }
                // else if ((int)Game1.player.maxItems != 48) {
                get_player,
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.maxItems)),
                code[2],
                Instructions.Ldc_I4_S(48),
                code[4]
            );
        }
        
#endregion

#region Resize GUI 
        public static void shiftIconsDown(List<ClickableComponent> equipmentIcons){
            foreach (var icon in equipmentIcons) {
                icon.bounds.Y += Game1.tileSize;
            }
        }
        
        void resize_inventory() {
            // Change inventory size from default (36) to 48
            var inv = FindCode(
                OpCodes.Ldc_I4_M1,  // Size (-1 = default)
                OpCodes.Ldc_I4_3,   // Rows
                OpCodes.Ldc_I4_0,
                OpCodes.Ldc_I4_0,
                OpCodes.Ldc_I4_1
            );
            inv[0] = Instructions.Ldc_I4(48);
            inv[1] = Instructions.Ldc_I4_4();
        }
        
        void InventoryPage_ctor() {
            BeginCode().Prepend(
                // height += Game1.tileSize;
                Instructions.Ldarg_S(4),
                Instructions.Ldc_I4_S(Game1.tileSize),
                Instructions.Add(),
                Instructions.Starg_S(4)
            );
            
            resize_inventory();
            
            EndCode().Insert(-1,
                // Shift icons down by `Game1.tileSize` pixels
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(InventoryPage), nameof(InventoryPage.equipmentIcons)),
                Instructions.Call(typeof(Mod), nameof(Mod.shiftIconsDown), typeof(List<ClickableComponent>))
            );
            EndCode().ReplaceJump(-1, EndCode()[-4]);
            
            try {
                // Move portrait `Game1.tileSize` pixels down.
                // This only affects where the tooltip shows up.
                FindCode(
                    OpCodes.Ldarg_0,
                    Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen)),
                    Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth)),
                    OpCodes.Add,
                    Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.spaceToClearTopBorder)),
                    OpCodes.Add,
                    Instructions.Ldc_I4(256),
                    OpCodes.Add,
                    Instructions.Ldc_I4_8(),
                    OpCodes.Sub,
                    Instructions.Ldc_I4_S(64),
                    OpCodes.Add
                )[6].operand = 256 + Game1.tileSize;
            } catch (System.Exception err) {
                Monitor.Log("Failed to fix portrait tooltip position.", LogLevel.Warn);
                LogException(err, LogLevel.Warn);
            }
        }

        void InventoryPage_draw() {
            var code = BeginCode();
            
            // var yoffset = yPositionOnScreen + borderWidth + spaceToClearTopBorder + Game1.tileSize
            var yoffset = generator.DeclareLocal(typeof(int));
            code.Prepend(
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen)),
                Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth)),
                Instructions.Add(),
                Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.spaceToClearTopBorder)),
                Instructions.Add(),
                Instructions.Ldc_I4_S(Game1.tileSize),
                Instructions.Add(),
                Instructions.Stloc_S(yoffset)
            );
            
            // Replace all remaining `yPositionOnScreen + borderWidth + spaceToClearTopBorder` by `yoffset`.
            for (var i=0; i<12; i++) {
                code = code.FindNext(
                    OpCodes.Ldarg_0,
                    Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen)),
                    Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth)),
                    OpCodes.Add,
                    Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.spaceToClearTopBorder)),
                    OpCodes.Add
                );
                code.Replace(
                    Instructions.Ldloc_S(yoffset)
                );
            }
        }

        void CraftingPage_ctor() {
            // Make the crafting page a bit higher too, to accomodate the bigger inventory.
            BeginCode().Prepend(
                // height += Game1.tileSize;
                Instructions.Ldarg_S(4),
                Instructions.Ldc_I4_S(Game1.tileSize),
                Instructions.Add(),
                Instructions.Starg_S(4)
            );
            
            resize_inventory();
        }
        
        void ShopMenu_ctor() {
            resize_inventory();
            
            var code = BeginCode();
            for (int i=0; i<2; i++) {
                code = code.FindNext(
                    Instructions.Ldc_I4(600),
                    Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth)),
                    Instructions.Ldc_I4_2(),
                    OpCodes.Mul,
                    OpCodes.Add
                );
                code[0].operand = 600 + Game1.tileSize;
            }
            
            // Fix the size of the shop buttons.
            // Replace `((height - 256) / 4)` with 106
            for (int i=0; i<2; i++) {
                code = FindCode(
                    OpCodes.Ldarg_0,
                    Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.height)),
                    Instructions.Ldc_I4(256),
                    OpCodes.Sub,
                    Instructions.Ldc_I4_4(),
                    OpCodes.Div
                );
                code.Replace(
                    Instructions.Ldc_I4(106)
                );
            }
        }
        
        void ShopMenu_draw() {
            // Position the inventory background
            // Change `yPositionOnScreen + height - 256 + 40` to `yPositionOnScreen + 464`
            // Note: originally height = 680.
            FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen)),
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.height)),
                OpCodes.Add,
                Instructions.Ldc_I4(256),
                OpCodes.Sub,
                Instructions.Ldc_I4_S(40),
                OpCodes.Add
            ).SubRange(2,6).Replace(
                Instructions.Ldc_I4(464)
            );
            // Change `height - 448 + 20` to `inventory.height + 44`
            // Note: originally inventory.height = 3*64+16 = 208.
            FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.height)),
                Instructions.Ldc_I4(448),
                OpCodes.Sub,
                Instructions.Ldc_I4_S(20),
                OpCodes.Add
            ).Replace(
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(ShopMenu), nameof(ShopMenu.inventory)),
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.height)),
                Instructions.Ldc_I4_S(44),
                Instructions.Add()
            );
            
            // Position the shop stock background
            // Change `height - 256 + 32 + 4` to 460.
            FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.height)),
                Instructions.Ldc_I4(256),
                OpCodes.Sub,
                Instructions.Ldc_I4_S(32),
                OpCodes.Add,
                Instructions.Ldc_I4_4(),
                OpCodes.Add
            ).Replace(
                Instructions.Ldc_I4(460)
            );
        }

        void ShopMenu_drawCurrency() {
            FindCode(
                Instructions.Ldc_I4_S(12),
                OpCodes.Sub
            ).Replace(
                Instructions.Ldc_I4_S(64-12),
                Instructions.Add()
            );
        }
        
        void ShippingMenu_ctor() {
            resize_inventory();
            
            var code = BeginCode();
            for (int i=0; i<2; i++) {
                code = code.FindNext(
                    Instructions.Ldc_I4(600),
                    Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth)),
                    Instructions.Ldc_I4_2(),
                    OpCodes.Mul,
                    OpCodes.Add
                );
                code[0].operand = 600 + Game1.tileSize;
            }
        }
        
        void JunimoNoteMenu_setUpMenu() {
            // Move the grid a few pixels
            var code = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.xPositionOnScreen)),
                Instructions.Ldc_I4(128),
                OpCodes.Add,
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.yPositionOnScreen)),
                Instructions.Ldc_I4(140),
                OpCodes.Add
            );
            code[2].operand = 104;
            code[6].operand = 120;

            // Change inventory from a 6x6 grid to a 7x7 grid.
            FindCode(
                Instructions.Ldc_I4_S(36),
                OpCodes.Ldc_I4_6,
                OpCodes.Ldc_I4_8,
                OpCodes.Ldc_I4_8,
                OpCodes.Ldc_I4_0
            ).Replace(
                Instructions.Ldc_I4_S(49),
                Instructions.Ldc_I4_7(),
                Instructions.Ldc_I4_4(),
                Instructions.Ldc_I4_4(),
                Instructions.Ldc_I4_0()
            );
            FindCode(
                Instructions.Ldc_I4_S(36),
                Instructions.Stfld(typeof(InventoryMenu), nameof(InventoryMenu.capacity))
            )[0].operand = 49;
            
            code = BeginCode();
            for (int i=0; i<2; i++) {
                code = code.FindNext(
                    Instructions.Ldsfld(typeof(IClickableMenu), nameof(IClickableMenu.borderWidth)),
                    OpCodes.Ldc_I4_2,
                    OpCodes.Mul,
                    OpCodes.Add
                );
                code.Remove();
            }
        }
#endregion

#region Assets
        bool IAssetEditor.CanEdit<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("LooseSprites/JunimoNote")) {
                return true;
            }
            return false;
        }

        void IAssetEditor.Edit<T>(IAssetData asset)
        {
            if (asset.AssetNameEquals("LooseSprites/JunimoNote")) {
                var junimoNote  = Helper.Content.Load<Texture2D>("JunimoNote.png");
                // 344,28 - 121x127
                asset.AsImage().PatchImage(junimoNote, targetArea: new Rectangle(344, 28, 121, 127));
            }
        }

        bool IAssetLoader.CanLoad<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("LooseSprites/BiggerBackpack")) {
                return true;
            }
            return false;
        }

        T IAssetLoader.Load<T>(IAssetInfo asset)
        {
            if (asset.AssetNameEquals("LooseSprites/BiggerBackpack")) {
                return Helper.Content.Load<T>("backpack.png", ContentSource.ModFolder);
            }
            throw new System.ArgumentException("BiggerBackpack cannot load asset for " + asset.AssetName);
        }
#endregion
    }
}
