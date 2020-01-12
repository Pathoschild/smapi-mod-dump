using Netcode;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.Menus;
using StardewValley.Monsters;
using StardewValley.Objects;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;

namespace StardewHack.WearMoreRings
{
    using SaveRingsDict   = Dictionary<long, SaveRings>;

    #region Data Classes
    /// <summary>
    /// Structure used to store the actual rings.
    /// </summary> 
    public class ActualRings {
        public readonly NetRef<Ring> ring1 = new NetRef<Ring>(null);
        public readonly NetRef<Ring> ring2 = new NetRef<Ring>(null);
        public readonly NetRef<Ring> ring3 = new NetRef<Ring>(null);
        public readonly NetRef<Ring> ring4 = new NetRef<Ring>(null);
        
        public void LoadRings(SaveRings sr) {
            ring1.Set(MakeRing(sr.which1));
            ring2.Set(MakeRing(sr.which2));
            ring3.Set(MakeRing(sr.which3));
            ring4.Set(MakeRing(sr.which4));
        }
        
        private Ring MakeRing(int which) {
            if (which < 0) return null;
            try {
                return new Ring(which);
            } catch {
                // Ring no longer exists, so delete it.
                ModEntry.getInstance().Monitor.Log($"Failed to create ring with id {which}.", LogLevel.Warn);
                return null;
            }
        }
    }
    
    /// <summary>
    /// Structure for save data.
    /// </summary>
    public class SaveRings {
        public int which1;
        public int which2;
        public int which3;
        public int which4;

        public SaveRings() { }
        
        public SaveRings(ActualRings ar) {
            which1 = getWhich(ar.ring1.Value);
            which2 = getWhich(ar.ring2.Value);
            which3 = getWhich(ar.ring3.Value);
            which4 = getWhich(ar.ring4.Value);
        }
        
        private int getWhich(Ring r) {
            if (r==null) return -1;
            return r.indexInTileSheet.Value;
        }
    }
    #endregion Data Classes

    public class RingsImplementation : IWearMoreRingsAPI
    {
        public int CountEquippedRings(Farmer f, int which) {
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ModEntry.CountWearingRing(f, which);
        }

        public IEnumerable<Ring> GetAllRings(Farmer f) {
            if (f == null) throw new ArgumentNullException(nameof(f));
            return ModEntry.ListRings(f);
        }
    }

    public class ModEntry : Hack<ModEntry>
    {
        static readonly ConditionalWeakTable<Farmer, ActualRings> actualdata = new ConditionalWeakTable<Farmer, ActualRings>();
        public static readonly Random random = new Random();
        
        public override void HackEntry(IModHelper helper) {
            helper.Events.GameLoop.Saving += GameLoop_Saving;
            helper.Events.GameLoop.SaveLoaded += GameLoop_SaveLoaded;
            
            // Change the network protocol version.
            // I've tried doing this through SMAPI events, but stuff already breaks prior to those events being received and when scanning for local games.
            var mp = Helper.Reflection.GetField<Multiplayer>(typeof(Game1), "multiplayer");
            mp.GetValue().protocolVersion += "+WearMoreRings";
            
            Patch(typeof(Farmer), "farmerInit", Farmer_farmerInit);
            Patch((Farmer f)=>f.isWearingRing(0), Farmer_isWearingRing);
            Patch(typeof(Farmer), "updateCommon", Farmer_updateCommon);
            Patch((GameLocation gl)=>gl.cleanupBeforePlayerExit(), GameLocation_cleanupBeforePlayerExit); 
            Patch(typeof(GameLocation), "resetLocalState", GameLocation_resetLocalState); 
            Patch((GameLocation gl)=>gl.damageMonster(new Rectangle(),0,0,false,0.0f,0,0.0f,0.0f,false,null), GameLocation_damageMonster);
            Patch(()=>new InventoryPage(0,0,0,0), InventoryPage_ctor);
            Patch((InventoryPage ip)=>ip.draw(null), InventoryPage_draw);
            Patch((InventoryPage ip)=>ip.performHoverAction(0,0), InventoryPage_performHoverAction);
            Patch((InventoryPage ip)=>ip.receiveLeftClick(0,0,false), InventoryPage_receiveLeftClick);
            Patch(()=>new Ring(0), Ring_ctor);
        }
        
        public override object GetApi() {
            return new RingsImplementation();
        }
        
        static ActualRings FarmerNotFound(Farmer f) {
            throw new Exception("ERROR: A Farmer object was not correctly registered with the 'WearMoreRings' mod.");
        }
        
        #region Events
        /// <summary>
        /// Serializes the worn extra rings to disk.
        /// </summary>
        void GameLoop_Saving(object sender, StardewModdingAPI.Events.SavingEventArgs e) {
            if (!Game1.IsMasterGame) return;
            var savedata = new SaveRingsDict();
            foreach(Farmer f in Game1.getAllFarmers()) {
                savedata[f.UniqueMultiplayerID] = new SaveRings(actualdata.GetValue(f, FarmerNotFound));
            }
            Helper.Data.WriteSaveData("extra-rings", savedata);
            Monitor.Log("Saved extra rings data.");
        }

        /// <summary>
        /// Reads the saved extra rings and creates them.
        /// </summary>
        void GameLoop_SaveLoaded(object sender, StardewModdingAPI.Events.SaveLoadedEventArgs e) {
            if (!Game1.IsMasterGame) return;
            // Load data from mod's save file, if available.
            var savedata = Helper.Data.ReadSaveData<SaveRingsDict>("extra-rings");
            if (savedata == null) {
                Monitor.Log("Save data not available.");
                return;
            }
            // Iterate through each farmer to load the extra equipped rings.
            foreach(Farmer f in Game1.getAllFarmers()) {
                if (savedata.ContainsKey(f.UniqueMultiplayerID)) {
                    actualdata.GetValue(f, FarmerNotFound).LoadRings(savedata[f.UniqueMultiplayerID]);
                }
            }
            Monitor.Log("Loaded extra rings save data.");
        }
        #endregion Events

        #region Patch Farmer
        /// <summary>
        /// Add the extra rings to the Netcode tree.
        /// </summary>
        public static void InitFarmer(Farmer f) {
            var actualrings = new ActualRings();
            f.NetFields.AddFields(
                actualrings.ring1,
                actualrings.ring2,
                actualrings.ring3,
                actualrings.ring4
            );
            actualdata.Add(f, actualrings);
        }

        void Farmer_farmerInit() {
            var addfields = FindCode(
                OpCodes.Stelem_Ref,
                Instructions.Callvirt(typeof(NetFields), nameof(NetFields.AddFields), typeof(INetSerializable[]))
            );
            addfields.Append(
                Instructions.Ldarg_0(),
                Instructions.Call(typeof(ModEntry), nameof(ModEntry.InitFarmer), typeof(Farmer))
            );
        }
        
        public static IEnumerable<Ring> ListRings(Farmer f) {
            var r = new List<Ring>();
            void Add(Ring ring) {
                if (ring!=null) r.Add(ring);
            }
            ActualRings ar = ModEntry.actualdata.GetValue(f, ModEntry.FarmerNotFound);
            Add(f.leftRing.Value);
            Add(f.rightRing.Value);
            Add(ar.ring1.Value);
            Add(ar.ring2.Value);
            Add(ar.ring3.Value);
            Add(ar.ring4.Value);
            return r;
        }
        
        public static int CountWearingRing(Farmer f, int id) {
            bool IsRing(Ring r) {
                return r != null && r.indexInTileSheet.Value == id;
            }
        
            ActualRings ar = actualdata.GetValue(f, FarmerNotFound);
            int res = 0;
            if (IsRing(f.leftRing.Value)) res++;
            if (IsRing(f.rightRing.Value)) res++;
            if (IsRing(ar.ring1.Value)) res++;
            if (IsRing(ar.ring2.Value)) res++;
            if (IsRing(ar.ring3.Value)) res++;
            if (IsRing(ar.ring4.Value)) res++;
            return res;
        }

        void Farmer_isWearingRing() {
            AllCode().Replace(
                Instructions.Ldarg_0(),
                Instructions.Ldarg_1(),
                Instructions.Call(typeof(ModEntry), nameof(ModEntry.CountWearingRing), typeof(Farmer), typeof(int)),
                Instructions.Ret()
            );
        }

        public static void UpdateRings(GameTime time, GameLocation location, Farmer f) {
            void update(Ring r) { 
                if (r != null) r.update(time, location, f); 
            };
            
            ActualRings ar = actualdata.GetValue(f, FarmerNotFound);
            update(f.leftRing.Value);
            update(f.rightRing.Value);
            update(ar.ring1.Value);
            update(ar.ring2.Value);
            update(ar.ring3.Value);
            update(ar.ring4.Value);
        }
        
        void Farmer_updateCommon() {
            FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.rightRing)),
                OpCodes.Callvirt,
                OpCodes.Brfalse,
                
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.rightRing)),
                OpCodes.Callvirt,
                OpCodes.Ldarg_1,
                OpCodes.Ldarg_2,
                OpCodes.Ldarg_0,
                OpCodes.Callvirt,
                
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.leftRing)),
                OpCodes.Callvirt,
                OpCodes.Brfalse,
                
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.leftRing)),
                OpCodes.Callvirt,
                OpCodes.Ldarg_1,
                OpCodes.Ldarg_2,
                OpCodes.Ldarg_0,
                OpCodes.Callvirt
            ).Replace(
                Instructions.Ldarg_1(),
                Instructions.Ldarg_2(),
                Instructions.Ldarg_0(),
                Instructions.Call(typeof(ModEntry), nameof(UpdateRings), typeof (GameTime), typeof(GameLocation), typeof(Farmer))
            );
        }
        #endregion Patch Farmer

        #region Patch GameLocation
        public static void ring_onLeaveLocation(GameLocation location) {
            ActualRings ar = actualdata.GetValue(Game1.player, FarmerNotFound);
            Game1.player.leftRing.Value?.onLeaveLocation(Game1.player, location);
            Game1.player.rightRing.Value?.onLeaveLocation(Game1.player, location);
            ar.ring1.Value?.onLeaveLocation(Game1.player, location);
            ar.ring2.Value?.onLeaveLocation(Game1.player, location);
            ar.ring3.Value?.onLeaveLocation(Game1.player, location);
            ar.ring4.Value?.onLeaveLocation(Game1.player, location);
        }
        
        void GameLocation_cleanupBeforePlayerExit() { 
            var code = FindCode(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.rightRing)),
                OpCodes.Callvirt,
                OpCodes.Brfalse
            );
            code.Extend(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.leftRing)),
                OpCodes.Callvirt,
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                OpCodes.Ldarg_0,
                Instructions.Callvirt(typeof(Ring), nameof(Ring.onLeaveLocation), typeof(Farmer), typeof(GameLocation))
            );
            code.Replace(
                Instructions.Ldarg_0(),
                Instructions.Call(typeof(ModEntry), nameof(ModEntry.ring_onLeaveLocation), typeof(GameLocation))
            );
        }
        
        public static void ring_onNewLocation(GameLocation location) {
            ActualRings ar = actualdata.GetValue(Game1.player, FarmerNotFound);
            Game1.player.leftRing.Value?.onNewLocation(Game1.player, location);
            Game1.player.rightRing.Value?.onNewLocation(Game1.player, location);
            ar.ring1.Value?.onNewLocation(Game1.player, location);
            ar.ring2.Value?.onNewLocation(Game1.player, location);
            ar.ring3.Value?.onNewLocation(Game1.player, location);
            ar.ring4.Value?.onNewLocation(Game1.player, location);
        }
        
        void GameLocation_resetLocalState() { 
            var code = FindCode(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.rightRing)),
                OpCodes.Callvirt,
                OpCodes.Brfalse
            );
            code.Extend(
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.leftRing)),
                OpCodes.Callvirt,
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                OpCodes.Ldarg_0,
                Instructions.Callvirt(typeof(Ring), nameof(Ring.onNewLocation), typeof(Farmer), typeof(GameLocation))
            );
            code.Replace(
                Instructions.Ldarg_0(),
                Instructions.Call(typeof(ModEntry), nameof(ring_onNewLocation), typeof(GameLocation))
            );
        }
         
        public static void ring_onMonsterSlay(Monster target, GameLocation location, Farmer who) {
            if (who==null) return;
            ActualRings ar = actualdata.GetValue(who, FarmerNotFound);
            who.leftRing.Value?.onMonsterSlay(target, location, who);
            who.rightRing.Value?.onMonsterSlay(target, location, who);
            ar.ring1.Value?.onMonsterSlay(target, location, who);
            ar.ring2.Value?.onMonsterSlay(target, location, who);
            ar.ring3.Value?.onMonsterSlay(target, location, who);
            ar.ring4.Value?.onMonsterSlay(target, location, who);
        }
        
        void GameLocation_damageMonster() {
            byte arg_who = (byte)(Array.Find(original.GetParameters(), info => info.Name == "who").Position+1);

            var code = FindCode(
                // if (who != null && who.leftRing.Value != null) {
                Instructions.Ldarg_S(arg_who), // who
                OpCodes.Brfalse,
                Instructions.Ldarg_S(arg_who), // who
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.leftRing)),
                OpCodes.Callvirt,
                OpCodes.Brfalse
            );
            code.Extend(
                // who.rightRing.Value
                Instructions.Ldarg_S(arg_who), // who
                Instructions.Ldfld(typeof(Farmer), nameof(Farmer.rightRing)),
                OpCodes.Callvirt,
                // .onMonsterSlay (monster, this, who);
                OpCodes.Ldloc_S, // monster
                OpCodes.Ldarg_0, // this
                Instructions.Ldarg_S(arg_who), // who
                Instructions.Callvirt(typeof(Ring), nameof(Ring.onMonsterSlay), typeof(Monster), typeof(GameLocation), typeof(Farmer))
            );
            
            var monster = code.SubRange(code.length - 4, 3);
            code.Replace(
                // ModEntry.ring_onMonsterSlay(monster, this, who);
                monster[0],
                monster[1],
                monster[2],
                Instructions.Call(typeof(ModEntry), nameof(ring_onMonsterSlay), typeof(Monster), typeof(GameLocation), typeof(Farmer))
            );
        }
        #endregion Patch GameLocation
        
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
                fullyImmutable = true
            };
            page.equipmentIcons.Add(component);
        }

        static public void AddEquipmentIcons(InventoryPage page) {
            ActualRings ar = actualdata.GetValue(Game1.player, FarmerNotFound);
            int inv = Game1.player.MaxItems - 12;
            //             name            x   y   id   up   dn   lt   rt, item
            AddIcon(page, "Hat",           0,  0, 102, inv, 103,  -1, 110, Game1.player.hat.Value);
            AddIcon(page, "Shirt",         0, 16, 103, 102, 104,  -1, 111, Game1.player.shirtItem.Value);
            AddIcon(page, "Pants",         0, 32, 104, 103, 108,  -1, 112, Game1.player.pantsItem.Value);
            AddIcon(page, "Boots",         0, 48, 108, 104,  -1,  -1, 112, Game1.player.boots.Value);
            AddIcon(page, "Left Ring",    52,  0, 110, inv, 111, 102, 101, Game1.player.leftRing.Value);
            AddIcon(page, "Right Ring",   68,  0, 101, inv, 121, 110, 105, Game1.player.rightRing.Value);
            AddIcon(page, "Extra Ring 1", 52, 16, 111, 110, 112, 103, 121, ar.ring1.Value);
            AddIcon(page, "Extra Ring 2", 68, 16, 121, 101, 122, 111, 105, ar.ring2.Value);
            AddIcon(page, "Extra Ring 3", 52, 32, 112, 111,  -1, 104, 122, ar.ring3.Value);
            AddIcon(page, "Extra Ring 4", 68, 32, 122, 121,  -1, 112, 105, ar.ring4.Value);
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
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
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
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
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
            ActualRings ar = actualdata.GetValue(Game1.player, FarmerNotFound);
            switch (icon.name) {
            case "Hat":          Game1.player.hat.Set (helditem as Hat);            break;
            case "Shirt":        Game1.player.shirtItem.Set (helditem as Clothing); break;
            case "Pants":        Game1.player.pantsItem.Set (helditem as Clothing); break;
            case "Boots":        Game1.player.boots.Set (helditem as Boots);        break;
            case "Left Ring":    Game1.player.leftRing.Set (helditem as Ring);      break;
            case "Right Ring":   Game1.player.rightRing.Set (helditem as Ring);     break;
            case "Extra Ring 1": ar.ring1.Set (helditem as Ring);                   break;
            case "Extra Ring 2": ar.ring2.Set (helditem as Ring);                   break;
            case "Extra Ring 3": ar.ring3.Set (helditem as Ring);                   break;
            case "Extra Ring 4": ar.ring4.Set (helditem as Ring);                   break;
            default:
                getInstance().Monitor.Log ($"ERROR: Trying to fit equipment item into invalid slot '{icon.name}'", LogLevel.Error);
                return false;
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
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
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
        
        #region Patch Ring
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

