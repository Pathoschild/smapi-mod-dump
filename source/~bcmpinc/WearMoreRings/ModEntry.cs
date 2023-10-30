/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using GenericModConfigMenu;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Objects;
using System;
using System.Reflection.Emit;
using System.Collections.Generic;

namespace StardewHack.WearMoreRings
{
    public class ModConfig
    {
        /** How many ring slots are available. */
        public int Rings = 8;
    }

#pragma warning disable CS0618 // Type or member is obsolete
    public class ModEntry : HackWithConfig<ModEntry, ModConfig>, IWearMoreRingsAPI, IWearMoreRingsAPI_2
#pragma warning restore CS0618 // Type or member is obsolete
    {
        private Type forge_menu_class = typeof(ForgeMenu);
        public static readonly Random random = new Random();
        public static readonly PerScreen<RingMap> container = new PerScreen<RingMap>();

        public override void HackEntry(IModHelper helper) {
            I18n.Init(helper.Translation);

            if (config.Rings < 2) {
                config.Rings = 2;
            }
            if (config.Rings > RingMap.MAX_RINGS) {
                config.Rings = RingMap.MAX_RINGS;
                getInstance().Monitor.Log("Rings limited to 20. You only have so many fingers... and toes.", LogLevel.Warn);
            }
        
            helper.Events.GameLoop.SaveLoaded += (object sender, SaveLoadedEventArgs e) => {
                getInstance().Monitor.Log($"Save loaded for {Game1.player.displayName}.", LogLevel.Info);
                container.Value = new RingMap(Game1.player);
                Migration.Import(Monitor, helper);
                container.Value.limitSize(config.Rings);
                ResetModifiers();
            };
            helper.Events.GameLoop.Saving += (object sender, SavingEventArgs e) => {
                container.Value.Save();
            };
            helper.ConsoleCommands.Add("player_resetmodifiers",   I18n.ResetModifiersCommand(),    (string arg1, string[] arg2) => { ResetModifiers(); });
            helper.ConsoleCommands.Add("world_destroyringchests", I18n.DestroyRingChestsCommand(), (string arg1, string[] arg2) => { Migration.DestroyRemainingChests(Monitor); });
            helper.ConsoleCommands.Add("player_openforge",        I18n.OpenForgeCommand(),         (string arg1, string[] arg2) => { Game1.activeClickableMenu = new ForgeMenu(); });
            
            Patch(()=>new InventoryPage(0,0,0,0), InventoryPage_ctor);
            Patch((InventoryPage ip)=>ip.draw(null), InventoryPage_draw);
            Patch((InventoryPage ip)=>ip.performHoverAction(0,0), InventoryPage_performHoverAction);
            Patch((InventoryPage ip)=>ip.receiveLeftClick(0,0,false), InventoryPage_receiveLeftClick);
            Patch(()=>new Ring(0), Ring_ctor);

            if (helper.ModRegistry.IsLoaded("spacechase0.SpaceCore")) {
                Monitor.Log("Found SpaceCore mod, trying to patch its NewForgeMenu class instead.", LogLevel.Warn);
                forge_menu_class = AccessTools.TypeByName("SpaceCore.Interface.NewForgeMenu");
            }
            Patch(forge_menu_class, "_CreateButtons", ForgeMenu_CreateButtons);
        }
        
        protected override void InitializeApi(IGenericModConfigMenuApi api) {
            api.AddNumberOption(
                mod: ModManifest, 
                name: I18n.RingsName, 
                tooltip: I18n.RingsTooltip,
                getValue: () => config.Rings, 
                setValue: (int val) => {
                    config.Rings = val; 
                    if (container.Value != null) container.Value.limitSize(val);
                },
                min: 2, 
                max: 20
            );
        }

        public void ResetModifiers() {
            var who = Game1.player;
            Monitor.Log("Resetting modifiers for " + who.Name);
            who.ClearBuffs();

            who.leftRing.Value?.onUnequip(who, who.currentLocation);
            who.rightRing.Value?.onUnequip(who, who.currentLocation);
            who.boots.Value?.onUnequip();
            
            who.MagneticRadius = 128;
            who.knockbackModifier = 0;
            who.weaponPrecisionModifier = 0;
            who.critChanceModifier = 0;
            who.critPowerModifier = 0;
            who.weaponSpeedModifier = 0;
            who.attackIncreaseModifier = 0;
            who.resilience = 0;
            who.addedLuckLevel.Value = 0;
            who.immunity = 0;

            who.leftRing.Value?.onEquip(who, who.currentLocation);
            who.rightRing.Value?.onEquip(who, who.currentLocation);
            who.boots.Value?.onEquip();
        }

#region API
        public override object GetApi(IModInfo info) {
            Monitor.Log($"Mod {info.Manifest.Name} requested the Wear More Rings API. Since version 5.0 mods should be compatible with WMR without custom support.", LogLevel.Warn);
            return this;
        }
        
        public int CountEquippedRings(Farmer f, int which) {
            int res = 0;
            foreach (var r in GetAllRings(f)) {
                if (r.GetsEffectOfRing(which)) {
                    res++;
                }
            }
            return res;
        }

        public IEnumerable<Ring> GetAllRings(Farmer f) {
            if (f == null) throw new ArgumentNullException(nameof(f));
            var stack = new Stack<Ring>();
            stack.Push(f.leftRing.Value);
            stack.Push(f.rightRing.Value);
            while (stack.Count > 0) {
                var ring = stack.Pop();
                if (ring is CombinedRing) {
                    foreach (var cr in ((CombinedRing)ring).combinedRings) {
                        stack.Push(cr);
                    }
                } else if (ring != null) {
                    yield return ring;
                }
            }
        }

        public int RingSlotCount() {
            return config.Rings;
        }

        public Ring GetRing(int slot) {
            if (slot < 0 || config.Rings <= slot) throw new ArgumentOutOfRangeException();
            return container.Value[slot];
        }

        public void SetRing(int slot, Ring ring) {
            if (slot < 0 || config.Rings <= slot) throw new ArgumentOutOfRangeException();
            if (container.Value[slot] != ring) { 
                container.Value[slot]?.onUnequip(Game1.player, Game1.currentLocation);
                container.Value[slot] = ring;
                container.Value[slot]?.onEquip  (Game1.player, Game1.currentLocation);
            }
        }
#endregion API
        
#region Patch InventoryPage
        static public void AddIcon(InventoryPage page, string name, int x, int y, int id, int up, int down, int left, int right, Item item) {
            var rect = new Rectangle (
                page.xPositionOnScreen + 48 + x*4, 
                page.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + 256 - 12 + y*4, 
                64, 64
            );
            var component = new ClickableComponent(rect, name) {
                myID = id,
                downNeighborID = down,
                upNeighborID = up,
                rightNeighborID = right,
                leftNeighborID = left,
                item = item,
                fullyImmutable = true,
            };
            page.equipmentIcons.Add(component);
        }

        static public void AddEquipmentIcons(InventoryPage page) {
            int inv = Game1.player.MaxItems - 12;
            //             name            x   y   id   up   dn   lt   rt, item
            AddIcon(page, "Hat",           0,  0, 102, inv, 103,  -1, 110, Game1.player.hat.Value);
            AddIcon(page, "Shirt",         0, 16, 103, 102, 104,  -1, 111, Game1.player.shirtItem.Value);
            AddIcon(page, "Pants",         0, 32, 104, 103, 108,  -1, 112, Game1.player.pantsItem.Value);
            AddIcon(page, "Boots",         0, 48, 108, 104,  -1,  -1, 112, Game1.player.boots.Value);
            
            var max_rings = getInstance().config.Rings;
            int slot_id(int x, int y, int def=-1) {
                if (x==-1) {
                    switch(y) {
                      case 0: return 102;
                      case 1: return 103;
                      case 2: return 104;
                      case 3: return 108;
                    }
                }
                if (y==-1) {
                    return inv + 3 + x;
                }
                
                int id = x*4 + y;
                if (id >= max_rings) return def;
                if (id == 1) return 101; // Original id for right ring.
                return 110 + x + y*10;
            };
            
            for (int i=0; i<max_rings; i++) {
                String name;
                Ring ring;
                name = "Ring " + i;
                ring = container.Value[i];
                var x = i/4;
                var y = i%4;
                AddIcon(page, name, 52+16*x, 16*y, slot_id(x,y), slot_id(x,y-1), slot_id(x,y+1), slot_id(x-1,y), slot_id(x+1,y, 105), ring);
            }
        }
        
        void InventoryPage_ctor() {
            // Replace code for equipment icon creation with method calls to our AddEquipmentIcon method.
            // Replace rings & boots
            var items = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(InventoryPage), nameof(InventoryPage.equipmentIcons)),
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.xPositionOnScreen)),
                Instructions.Ldc_I4_S(48)
            );
            items.Extend(
                OpCodes.Dup,
                Instructions.Ldc_I4_S(109),
                Instructions.Stfld(typeof(ClickableComponent), nameof(ClickableComponent.rightNeighborID)),
                OpCodes.Dup,
                OpCodes.Ldc_I4_1,
                Instructions.Stfld(typeof(ClickableComponent), nameof(ClickableComponent.fullyImmutable)),
                OpCodes.Callvirt
            );
            // Add our own equipment icons
            items.Replace(
                Instructions.Ldarg_0(), // page
                Instructions.Call(typeof(ModEntry), nameof(AddEquipmentIcons), typeof(InventoryPage))
            );
            // Remove hat, shirt & pants
            items = items.FindNext(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(InventoryPage), nameof(InventoryPage.equipmentIcons)),
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(IClickableMenu), nameof(IClickableMenu.xPositionOnScreen)),
                Instructions.Ldc_I4_S(48)
            );
            items.Extend(
                OpCodes.Dup,
                Instructions.Ldc_I4_S(104),
                Instructions.Stfld(typeof(ClickableComponent), nameof(ClickableComponent.leftNeighborID)),
                OpCodes.Dup,
                OpCodes.Ldc_I4_1,
                Instructions.Stfld(typeof(ClickableComponent), nameof(ClickableComponent.fullyImmutable)),
                OpCodes.Callvirt
            );
            items.Remove();
        }
        
        static public void DrawEquipment(ClickableComponent icon, Microsoft.Xna.Framework.Graphics.SpriteBatch b) {
            if (icon.item != null) {
                b.Draw(Game1.menuTexture, icon.bounds, Game1.getSourceRectForStandardTileSheet (Game1.menuTexture, 10, -1, -1), Color.White);
                icon.item.drawInMenu(b, new Vector2(icon.bounds.X, icon.bounds.Y), icon.scale, 1f, 0.866f, StackDrawType.Hide);
            } else {
                int tile = 41;
                if (icon.name == "Hat") tile = 42;
                if (icon.name == "Shirt") tile = 69;
                if (icon.name == "Pants") tile = 68;
                if (icon.name == "Boots") tile = 40;
                b.Draw (Game1.menuTexture, icon.bounds, Game1.getSourceRectForStandardTileSheet (Game1.menuTexture, tile, -1, -1), Color.White);
            }
        }
        
        void InventoryPage_draw() {
            // Change the equipment slot drawing code to draw the 4 additional slots.
            InstructionRange range;
            try {
                // Windows
                range  = FindCode(
                    // switch (equipmentIcon.name) {
                    OpCodes.Ldloca_S,
                    OpCodes.Call,
                    OpCodes.Stloc_S, // 4
                    OpCodes.Ldloc_S, // 4
                    Instructions.Ldfld(typeof(ClickableComponent), nameof(ClickableComponent.name)),
                    OpCodes.Stloc_S, // 5
                    // case "Hat":
                    OpCodes.Ldloc_S, // 5
                    Instructions.Ldstr("Hat")
                );
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
                // Linux & MacOS
                range  = FindCode(
                    // switch (equipmentIcon.name) {
                    OpCodes.Ldloca_S,
                    OpCodes.Call,
                    OpCodes.Stloc_S, // 4
                    OpCodes.Ldloc_S, // 4
                    Instructions.Ldfld(typeof(ClickableComponent), nameof(ClickableComponent.name)),
                    OpCodes.Stloc_S, // 5
                    OpCodes.Ldloc_S, // 5
                    OpCodes.Brfalse,
                    // case "Hat":
                    OpCodes.Ldloc_S, // 5
                    Instructions.Ldstr("Hat")
                );
            }
            
            // Select entire loop contents (i.e. switch block)
            range.Extend(range.Follow(-1));
            range.Replace(
                range[0],
                range[1],
                Instructions.Ldarg_1(),
                Instructions.Call(typeof(ModEntry), nameof(DrawEquipment), typeof(ClickableComponent), typeof(Microsoft.Xna.Framework.Graphics.SpriteBatch))
            );
            
            // Move other stuff 32/64px to the right to eliminate overlap.
            range = range.FindNext(
                Instructions.Ldc_R4(32),
                OpCodes.Stloc_0
            );
            range[0] = Instructions.Ldc_R4(64);
        }
        
        void InventoryPage_performHoverAction() {
            // Change code responsible for obtaining the tooltip information.
            var var_item = generator.DeclareLocal(typeof(Item));
            InstructionRange code;
            try {
                // Windows
                code = FindCode(
                    // switch (equipmentIcon.name) {
                    OpCodes.Ldloc_1,
                    Instructions.Ldfld(typeof(ClickableComponent), nameof(ClickableComponent.name)),
                    OpCodes.Stloc_2,
                    // case "Hat":
                    OpCodes.Ldloc_2,
                    Instructions.Ldstr("Hat")
                );
                code.Extend(
                    OpCodes.Ldloc_2,
                    Instructions.Ldstr("Pants"),
                    OpCodes.Call,
                    OpCodes.Brtrue,
                    OpCodes.Br
                );
                code.Extend(code.End.Follow(-1));
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
                // Linux & MacOS
                code = FindCode(
                    // switch (equipmentIcon.name) {
                    OpCodes.Ldloc_1,
                    Instructions.Ldfld(typeof(ClickableComponent), nameof(ClickableComponent.name)),
                    OpCodes.Stloc_2,
                    // case null: break;
                    OpCodes.Ldloc_2,
                    OpCodes.Brfalse,
                    // case "Hat":
                    OpCodes.Ldloc_2,
                    Instructions.Ldstr("Hat")
                );
                code.Extend(code.Follow(4));
            }
            code.Replace(
                // var item = EquipmentIcon.item
                code[0],
                Instructions.Ldfld(typeof(ClickableComponent), nameof(ClickableComponent.item)),
                Instructions.Stloc_S(var_item),
                // if (item != null)
                Instructions.Ldloc_S(var_item),
                Instructions.Brfalse(AttachLabel(code.End[0])),
                // hoveredItem = item;
                Instructions.Ldarg_0(),
                Instructions.Ldloc_S(var_item),
                Instructions.Stfld(typeof(InventoryPage), "hoveredItem"),
                // hoverText = item.getDescription();
                Instructions.Ldarg_0(),
                Instructions.Ldloc_S(var_item),
                Instructions.Callvirt(typeof(Item), nameof(Item.getDescription)),
                Instructions.Stfld(typeof(InventoryPage), "hoverText"),
                // hoverTitle = item.DisplayName;
                Instructions.Ldarg_0(),
                Instructions.Ldloc_S(var_item),
                Instructions.Callvirt_get(typeof(Item), nameof(Item.DisplayName)),
                Instructions.Stfld(typeof(InventoryPage), "hoverTitle")
            );
        }
        
        static public bool EquipmentClick(ClickableComponent icon) {
            // Check that item type is compatible.
            // And play corresponding sound.
            var helditem = Game1.player.CursorSlotItem;
            // Convert special items (such as copper pan & Lewis pants)
            if (helditem is StardewValley.Tools.Pan) helditem = new Hat (71);
            if (helditem is StardewValley.Object && helditem.ParentSheetIndex == 71) helditem = new Clothing(15);
            if (helditem == null) {
                if (icon.item == null) return false;
                Game1.playSound("dwop");
            } else {
                switch (icon.name) {
                    case "Hat":
                        if (!(helditem is Hat)) return false;
                        Game1.playSound ("grassyStep");
                        break;
                    case "Shirt":
                        if (!(helditem is Clothing)) return false;
                        if ((helditem as Clothing).clothesType.Value != (int)Clothing.ClothesType.SHIRT) return false;
                        Game1.playSound ("sandyStep");
                        break;
                    case "Pants":
                        if (!(helditem is Clothing)) return false;
                        if ((helditem as Clothing).clothesType.Value != (int)Clothing.ClothesType.PANTS) return false;
                        Game1.playSound ("sandyStep");
                        break;
                    case "Boots":
                        if (!(helditem is Boots)) return false;
                        Game1.playSound ("sandyStep");
                        DelayedAction.playSoundAfterDelay ("sandyStep", 150, null);
                        break;
                    default:
                        if (!(helditem is Ring)) return false;
                        Game1.playSound ("crit");
                        break;
                }
            }
            
            // I have no idea why StardewValley does this in InventoryPage::setHeldItem, but I guess it might be important.
            if (helditem != null) {
                helditem.NetFields.Parent = null;
            }
            
            // Update inventory
            switch (icon.name) {
                case "Hat":          Game1.player.hat.Set (helditem as Hat);            break;
                case "Shirt":        Game1.player.shirtItem.Set (helditem as Clothing); break;
                case "Pants":        Game1.player.pantsItem.Set (helditem as Clothing); break;
                case "Boots":        Game1.player.boots.Set (helditem as Boots);        break;
                default:
                    if (icon.name.StartsWith("Ring ", StringComparison.Ordinal)) {
                        int id = int.Parse(icon.name.Substring(5));
                        container.Value[id] = helditem as Ring;
                    } else {
                        getInstance().Monitor.Log ($"ERROR: Trying to fit equipment item into invalid slot '{icon.name}'", LogLevel.Error);
                        return false;
                    }
                    break;
            }

            // Equip/unequip
            (icon.item as Ring )?.onUnequip(Game1.player, Game1.currentLocation);
            (icon.item as Boots)?.onUnequip();
            (helditem as Ring )?.onEquip(Game1.player, Game1.currentLocation);
            (helditem as Boots)?.onEquip();
            
            // Swap items
            Game1.player.CursorSlotItem = Utility.PerformSpecialItemGrabReplacement(icon.item);
            icon.item = helditem;
            return true;
        }
        
        static public bool AutoEquipment(InventoryPage page) {
            var helditem = Game1.player.CursorSlotItem;
            foreach (ClickableComponent icon in page.equipmentIcons) {
                if (icon.item == null && EquipmentClick(icon)) {
                    return true;
                }
            }
            return false;
        }
        
        void InventoryPage_receiveLeftClick() {
            // Handle a ring-inventory slot being clicked.
            InstructionRange code;
            try {
                // Windows
                code = FindCode(
                    // switch (equipmentIcon.name) {
                    OpCodes.Ldloc_1,
                    Instructions.Ldfld(typeof(ClickableComponent), nameof(ClickableComponent.name)),
                    OpCodes.Stloc_3,
                    // case "Hat":
                    OpCodes.Ldloc_3,
                    Instructions.Ldstr("Hat")
                );
                code.Extend(
                    OpCodes.Ldloc_3,
                    Instructions.Ldstr("Pants"),
                    OpCodes.Call,
                    OpCodes.Brtrue,
                    OpCodes.Br
                );
                code.Extend(code.End.Follow(-1));
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
                // Linux & MacOS
                code = FindCode(
                    // switch (equipmentIcon.name) {
                    OpCodes.Ldloc_1,
                    Instructions.Ldfld(typeof(ClickableComponent), nameof(ClickableComponent.name)),
                    OpCodes.Stloc_3,
                    // case null: break;
                    OpCodes.Ldloc_3,
                    OpCodes.Brfalse,
                    // case "Hat":
                    OpCodes.Ldloc_3,
                    Instructions.Ldstr("Hat")
                );
                code.Extend(code.Follow(4));
            }
            code.Replace(
                code[0],
                Instructions.Call(typeof(ModEntry), nameof(EquipmentClick), typeof(ClickableComponent)),
                Instructions.Pop()
            );
            
            // Select code for equipping items through shift+click.
            code = code.FindNext(
                // if (checkHeldItem ((Item i) => i is Ring))
                OpCodes.Ldarg_0,
                OpCodes.Ldsfld,
                OpCodes.Dup,
                OpCodes.Brtrue,
                //
                OpCodes.Pop,
                OpCodes.Ldsfld,
                OpCodes.Ldftn,
                OpCodes.Newobj,
                OpCodes.Dup,
                OpCodes.Stsfld,
                //
                Instructions.Callvirt(typeof(InventoryPage), "checkHeldItem", typeof(Func<Item, bool>)),
                OpCodes.Brfalse,
                // if (Game1.player.leftRing.Value == null)
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.leftRing)),
                OpCodes.Callvirt,
                OpCodes.Brtrue
            );
            code.Extend(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.pantsItem)),
                OpCodes.Callvirt,
                OpCodes.Brtrue
            );
            Label equip_failed = (Label)code.End[-1].operand;
            code.Extend(code.End.Follow(-1));
            
            code.Replace(
                Instructions.Ldarg_0(),
                Instructions.Call(typeof(ModEntry), nameof(AutoEquipment), typeof(InventoryPage)),
                // if (true) return;
                Instructions.Brfalse(equip_failed),
                Instructions.Ret()
            );
        }
        #endregion Patch InventoryPage

#region Patch ForgeMenu
        void ForgeMenu_CreateButtons() {
            // Remove vanilla ring buttons.
            var code = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(forge_menu_class, nameof(ForgeMenu.equipmentIcons))
            );
            code.Extend(
                Instructions.Ldstr("Ring2")
            );
            code.Extend(
                OpCodes.Dup,
                Instructions.Ldc_I4(-99998),
                OpCodes.Stfld,
                Instructions.Callvirt(typeof(List<ClickableComponent>), nameof(List<ClickableComponent>.Add), typeof(ClickableComponent))
            );
            code.Replace(Instructions.Ret());
        }
#endregion

#region Patch Ring
        // Not sure how my mod uses this, but the code in the Ring constructor that generates a UniqueID seems seriously flawed.
        // It has a reasonably high probability of creating duplicates. So this patch replaces it with a number from a randomly seeded PRNG.
        void Ring_ctor() {
            var code = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Ring), nameof(Ring.uniqueID)),
                Instructions.Ldsfld(typeof(Game1), nameof(Game1.year)),
                Instructions.Ldsfld(typeof(Game1), nameof(Game1.dayOfMonth)),
                OpCodes.Add
            );
            code.Extend(
                Instructions.Call_get(typeof(Game1), nameof(Game1.stats)),
                Instructions.Ldfld(typeof(Stats), nameof(Stats.itemsCrafted)),
                OpCodes.Add,
                OpCodes.Callvirt
            );
            code = code.SubRange(2,code.length-3);
            code.Replace(
                // ModEntry.random.Next()
                Instructions.Ldsfld(typeof(ModEntry), nameof(random)),
                Instructions.Call(typeof(Random), nameof(Random.Next))
            );
        }
        #endregion Patch Ring
    }
}

