using System;
using System.Reflection;
using System.Reflection.Emit;
using Harmony;
using Microsoft.Xna.Framework;
using Netcode;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewHack.HarvestWithScythe
{
    public enum HarvestModeEnum {
        HAND,
        SCYTHE,
        BOTH, // I.e. determined by whether the scythe is equipped.
    }

    public class ModConfig {
        public class HarvestModeClass {
            /** How should flowers be harvested? 
             * Any Crop that is `programColored` is considered a flower. */
            public HarvestModeEnum Flowers = HarvestModeEnum.BOTH;
            
            /** How should forage be harvested? 
             * Any Object where `isForage() && isSpawnedObject && !questItem` evaluates to true is considered forage. */
            public HarvestModeEnum Forage = HarvestModeEnum.BOTH;
            
            /** How should spring onions be harvested?
             * Any Crop that is `forageCrop` is considered a spring onion. */
            public HarvestModeEnum SpringOnion = HarvestModeEnum.BOTH;
            
            /** How should pluckable crops be harvested? 
             * Any Crop that has `harvestMethod == 0` is considered a pluckable crop. */
            public HarvestModeEnum PluckableCrops = HarvestModeEnum.BOTH;
            
            /** How should scythable crops be harvested?
             * Any Crop that has `harvestMethod == 1` is considered a scythable crop. */
            public HarvestModeEnum ScythableCrops = HarvestModeEnum.SCYTHE;
        }
        public HarvestModeClass HarvestMode = new HarvestModeClass();
    }
    
    /**
     * This is the core of the Harvest With Scythe mod.
     *
     * Crops are either harvested by hand, which is initiatied by HoeDirt.PerformUseAction(), 
     * or harvested by scythe, which is initiated by HoeDirt.PerformToolAction().
     * These methods check whether the crop is allowed to be harvested by this method and 
     * then passes control to Crop.harvest() to perform the actual harvesting. 
     *
     * Crop.Harvest() can do further checks whether harvesting is possible. If not, it returns
     * false to indicate that harvesting failed.
     * 
     * The harvesting behavior, i.e. whether the item drops on the ground (scything) or 
     * is held above the head (plucking) is determined by the value of `harvestMethod`.
     * Hence HoeDirt.Perform*Action must set this field to the appropriate value and restore 
     * it afterwards.
     *
     * Flowers can have different colors, which is not supported by the original scythe harvesting 
     * code. To support it, this mod provides a `CreateDebris()` method as a proxy for spawning the
     * dropped crops/flowers.
     *
     * Forage are plain Objects where `isForage() && isSpawnedObject && !questItem` evaluates to true.
     * Those are handled by GameLocation.checkAction() and Object.performToolAction(). As the 
     * game does not provide logic for scythe harvesting of forage, this is provided by this mod, 
     * see ScytheForage().
     *
     */
    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
#region CanHarvest methods
        public const int HARVEST_PLUCKING = 0;
        public const int HARVEST_SCYTHING = 1;

        /** Check whether the used harvest method is allowed for the given harvest mode. 
         * Method: 0 = plucking, 1 = scything.
         */
        public static bool CanHarvest(HarvestModeEnum mode, int method) {
            // If mode is BOTH, then set mode depending on whether the scythe is currently equipped.
            if (mode == HarvestModeEnum.BOTH) {
                var t = Game1.player.CurrentTool;
                if (t is MeleeWeapon && (t as MeleeWeapon).BaseName.Equals("Scythe")) {
                    mode = HarvestModeEnum.SCYTHE;
                } else {
                    mode = HarvestModeEnum.HAND;
                }
            }
            
            // Determine if the currently used harvesting method is currently allowed.
            if (method == HARVEST_PLUCKING) {
                return mode == HarvestModeEnum.HAND;
            } else {
                return mode == HarvestModeEnum.SCYTHE;
            }
        }    
    
        /** Determine whether the given crop can be harvested using the given method. */
        public static bool CanHarvestCrop(Crop crop, int method) {
            // Get harvest settings from config
            ModConfig.HarvestModeClass config = getInstance().config.HarvestMode;
            HarvestModeEnum mode;
            if (crop.programColored.Value) {
                mode = config.Flowers;
            } else if (crop.forageCrop.Value) {
                mode = config.SpringOnion;
            } else if (crop.harvestMethod.Value == 0) {
                mode = config.PluckableCrops;
            } else {
                mode = config.ScythableCrops;
            }
            return CanHarvest(mode, method);
        }

        /** Determine whether the given object can be harvested using the given method. 
         * Assumes that isForage() returned true. */
        public static bool CanHarvestObject(StardewValley.Object obj, int method) {
            // Get harvest settings from config
            ModConfig.HarvestModeClass config = getInstance().config.HarvestMode;
            HarvestModeEnum mode = config.Forage;
            return CanHarvest(mode, method);
        }
#endregion
    
#region Patch Crop_harvest
        // Changes the vector to be pre-multiplied by 64, so it's easier to use for spawning debris.
        // Vector is stored in loc_3.
        private LocalBuilder Crop_harvest_fix_vector() {
            CodeInstruction vector2_ldloca_S = null;
            CodeInstruction vector2_constructor = null;

            // Remove line (2x)
            // Vector2 vector = new Vector2 ((float)xTile, (float)yTile);
            for (int i = 0; i < 2; i++) {
                var vec = FindCode (
                    OpCodes.Ldloca_S,
                    OpCodes.Ldarg_1,
                    OpCodes.Conv_R4,
                    OpCodes.Ldarg_2,
                    OpCodes.Conv_R4,
                    OpCodes.Call
                );
                vector2_ldloca_S    = vec[0];
                vector2_constructor = vec[5];
                vec.Remove();
            }
            var var_vector = (LocalBuilder)vector2_ldloca_S.operand;
            
            // Add to begin of function
            // Vector2 vector = new Vector2 ((float)xTile*64., (float)yTile*64.);
            BeginCode().Append(
                vector2_ldloca_S,
                Instructions.Ldarg_1(),
                Instructions.Conv_R4(),
                Instructions.Ldc_R4(64),
                Instructions.Mul(),
                Instructions.Ldarg_2(),
                Instructions.Conv_R4(),
                Instructions.Ldc_R4(64),
                Instructions.Mul(),
                vector2_constructor
            );
            
            // Replace (4x):
            //   from: new Vector2 (vector.X * 64f, vector.Y * 64f)
            //   to:   vector
            for (int i = 0; i < 4; i++) {
                FindCode(
                    null,
                    OpCodes.Ldfld,
                    Instructions.Ldc_R4(64),
                    OpCodes.Mul,
                    null,
                    OpCodes.Ldfld,
                    Instructions.Ldc_R4(64),
                    OpCodes.Mul,
                    OpCodes.Newobj
                ).Replace(
                    Instructions.Ldloc_S(var_vector)
                );
            }
            
            // Return the location of the vector variable.
            return var_vector;
        }

        // Support harvesting of spring onions with scythe
        private void Crop_harvest_support_spring_onion(LocalBuilder var_vector) {
            if (config.HarvestMode.SpringOnion == HarvestModeEnum.HAND) return;
            
            // Note: the branch
            //   if (this.forageCrop)
            // refers mainly to the crop spring union.
            
            // Find the lines:
            var AddItem = FindCode(
                // if (Game1.player.addItemToInventoryBool (@object, false)) {
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                InstructionMatcher.AnyOf( // @object
                    OpCodes.Ldloc_0,
                    OpCodes.Ldloc_1
                ),
                OpCodes.Ldc_I4_0,
                Instructions.Callvirt(typeof(Farmer), nameof(Farmer.addItemToInventoryBool), typeof(Item), typeof(bool)),
                OpCodes.Brfalse
            );
            
            var ldarg_0 = Instructions.Ldarg_0();
            var Ldloc_object = AddItem[1];
            var tail = AttachLabel(AddItem.FindNext(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Crop), nameof(Crop.regrowAfterHarvest))
            )[0]);

            // Insert check for harvesting with scythe and act accordingly.
            AddItem.ReplaceJump(0, ldarg_0);
            AddItem.Prepend(
                // if (this.harvestMethod != 0) {
                ldarg_0,
                Instructions.Ldfld(typeof(Crop), nameof(Crop.harvestMethod)),
                Instructions.Call_get(typeof(NetInt), nameof(NetInt.Value)),
                Instructions.Brfalse(AttachLabel(AddItem[0])),
                // Game1.createItemDebris (@object, vector, -1, null, -1)
                Ldloc_object, // @object
                Instructions.Ldloc_S(var_vector), // vector
                Instructions.Ldc_I4_M1(), // -1
                Instructions.Ldnull(), // null
                Instructions.Ldc_I4_M1(), // -1
                Instructions.Call(typeof(Game1), nameof(Game1.createItemDebris), typeof(Item), typeof(Vector2), typeof(int), typeof(GameLocation), typeof(int)),
                Instructions.Pop(), // For SDV 1.4
                // Jump to tail
                Instructions.Br(tail)
            );
        }

        // For colored flowers we need to call createItemDebris instead of createObjectDebris
        // Returns the local variable used for storing the quality of the crop.
        private LocalBuilder Crop_harvest_colored_flowers(LocalBuilder var_vector) {
            var code = FindCode(
                // Game1.createObjectDebris (indexOfHarvest, xTile, yTile, -1, num3, 1f, null);
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Call,
                OpCodes.Ldarg_1,
                OpCodes.Ldarg_2,
                OpCodes.Ldc_I4_M1,
                OpCodes.Ldloc_S, // [6] num3: quality
                OpCodes.Ldc_R4,
                OpCodes.Ldnull,
                OpCodes.Call
            );
            var var_quality = code[6].operand as LocalBuilder; // num3
            code.Replace(
                // CreateDebris(this, num3);
                Instructions.Ldarg_0(), // this
                Instructions.Ldloc_S(var_quality), // num3
                Instructions.Ldloc_S(var_vector), // vector
                Instructions.Call(typeof(ModEntry), nameof(CreateDebris), typeof(Crop), typeof(int), typeof(Vector2))
            );
            return var_quality;
        }

       /**
        * Patch code to drop sunflower seeds when harvesting with scythe.
        * Patch code to let harvesting with scythe drop only 1 item.
        * The other item drops are handled by the plucking code. 
        */
       void Crop_harvest_sunflower_drops(LocalBuilder var_quality) {
            // Remove start of loop
            var start_loop = FindCode(
                // for (int i = 0
                OpCodes.Ldc_I4_0,
                OpCodes.Stloc_S,
                OpCodes.Br,
                // junimoHarvester != null
                Instructions.Ldarg_S(4),
                OpCodes.Brfalse
            );
            
            // Get a reference to the 'i' variable.
            var var_i = start_loop[1].operand as LocalBuilder;
            // Remove the head of the loop.
            start_loop.length = 3;
            start_loop.Remove();

            // Find the start of the 'drop sunflower seeds' part.
            var DropSunflowerSeeds = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Crop), nameof(Crop.indexOfHarvest)),
                OpCodes.Call, // Netcode
                Instructions.Ldc_I4(421), // 421 = Item ID of Sunflower.
                OpCodes.Bne_Un
            );
            // Set quality for seeds to 0.
            DropSunflowerSeeds.Append(
                Instructions.Ldc_I4_0(),
                Instructions.Stloc_S(var_quality)
            );

            // Remove end of loop and everything after that until the end of the harvest==1 branch.
            var ScytheBranchTail = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(Crop), nameof(Crop.harvestMethod)),
                OpCodes.Call, // Netcode
                OpCodes.Ldc_I4_1,
                OpCodes.Bne_Un
            ).Follow(4);
            ScytheBranchTail.ExtendBackwards(
                Instructions.Ldloc_S(var_i),
                OpCodes.Ldc_I4_1,
                OpCodes.Add,
                Instructions.Stloc_S(var_i),
                Instructions.Ldloc_S(var_i),
                OpCodes.Ldloc_S, // num2
                OpCodes.Blt
            );
            
            // Change jump to end of loop into jump to drop sunflower seeds.
            ScytheBranchTail.ReplaceJump(0, DropSunflowerSeeds[0]);

            // Rewrite the tail of the Scythe harvest branch. 
            ScytheBranchTail.Replace(
                // Jump to the 'drop subflower seeds' part.
                Instructions.Br(AttachLabel(DropSunflowerSeeds[0]))
            );
        }

        [BytecodePatch("StardewValley.Crop::harvest")]
        void Crop_harvest() {
            var var_vector = Crop_harvest_fix_vector();
            Crop_harvest_support_spring_onion(var_vector);
            //var var_quality = Crop_harvest_colored_flowers(var_vector);
            //Crop_harvest_sunflower_drops(var_quality);
        }
#endregion

        // Proxy method for creating an object suitable for spawning as debris.
        public static void CreateDebris(Crop crop, int quality, Vector2 vector) {
            Item dropped_item;
            if (crop.programColored.Value) {
                dropped_item = new StardewValley.Objects.ColoredObject(crop.indexOfHarvest.Value, 1, crop.tintColor.Value) {
                    Quality = quality
                };
            } else {
                dropped_item = new StardewValley.Object(crop.indexOfHarvest.Value, 1, false, -1, quality);
            }
            Game1.createItemDebris(dropped_item, vector, -1, null, -1);
        }

#region Patch HoeDirt
        [BytecodePatch("StardewValley.TerrainFeatures.HoeDirt::performToolAction")]
        void HoeDirt_performToolAction() {
            // Find the first (and only) harvestMethod==1 check.
            var HarvestMethodCheck = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                Instructions.Ldfld(typeof(Crop), nameof(Crop.harvestMethod)),
                OpCodes.Call, // Netcode implicit conversion
                OpCodes.Ldc_I4_1,
                OpCodes.Bne_Un
            );

            // Change the harvestMethod==1 check to:
            //   damage=harvestMethod; 
            //   if (CanHarvestCrop(crop, 1)) {
            //   harvestMethod=1
            // This code block is followed by a call to crop.harvest().
            HarvestMethodCheck.Replace(
                // damage = crop.harvestMethod.
                HarvestMethodCheck[0],
                HarvestMethodCheck[1],
                HarvestMethodCheck[2],
                HarvestMethodCheck[3],
                Instructions.Starg_S(2), // damage

                // if (CanHarvestCrop(crop, 1)) {
                HarvestMethodCheck[0],
                HarvestMethodCheck[1],
                Instructions.Ldc_I4_1(),
                Instructions.Call(typeof(ModEntry), nameof(CanHarvestCrop), typeof(Crop), typeof(int)),
                Instructions.Brfalse((Label)HarvestMethodCheck[5].operand),
                
                // crop.harvestMethod = 1
                HarvestMethodCheck[0],
                HarvestMethodCheck[1],
                HarvestMethodCheck[2],
                Instructions.Ldc_I4_1(),
                Instructions.Call_set(typeof(NetInt), nameof(NetInt.Value))
            );

            // Restore harvestMethod by setting harvestMethod=damage 
            // after the following crop!=null check.
            HarvestMethodCheck.FindNext(
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                Instructions.Ldfld(typeof(Crop), nameof(Crop.dead)),
                OpCodes.Call, // Netcode
                OpCodes.Brfalse
            ).Prepend(
                HarvestMethodCheck[0],
                HarvestMethodCheck[1],
                HarvestMethodCheck[2],
                Instructions.Ldarg_2(), // damage
                Instructions.Call_set(typeof(NetInt), nameof(NetInt.Value))
            );
        }


        private void HoeDirt_performUseAction_hand(LocalBuilder var_temp_harvestMethod) {
            // Do plucking logic
            var harvest_hand = FindCode(
                // if ((int)crop.harvestMethod == 0) {
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                Instructions.Ldfld(typeof(Crop), nameof(Crop.harvestMethod)),
                OpCodes.Call, // NetCode implicit cast.
                OpCodes.Brtrue
            );
            harvest_hand.Replace(
                // var temp_harvestmethod = crop.harvestMethod;
                harvest_hand[0],
                harvest_hand[1],
                harvest_hand[2],
                harvest_hand[3],
                Instructions.Stloc_S(var_temp_harvestMethod),
                
                // if (ModEntry.CanHarvestCrop(crop, 0)) {
                Instructions.Ldarg_0(),
                harvest_hand[1],
                Instructions.Ldc_I4_0(),
                Instructions.Call(typeof(ModEntry), nameof(CanHarvestCrop), typeof(Crop), typeof(int)),
                Instructions.Brfalse((Label)harvest_hand[4].operand),
                
                // crop.harvestMethod = 0;
                Instructions.Ldarg_0(),
                harvest_hand[1],
                harvest_hand[2],
                Instructions.Ldc_I4_0(),
                Instructions.Call_set(typeof(NetInt), nameof(NetInt.Value))
            );
        }

        private void HoeDirt_performUseAction_scythe(LocalBuilder var_temp_harvestMethod) {
            // Do scything logic
            var harvest_scythe = FindCode(
                // if ((int)crop.harvestMethod == 1) {
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                Instructions.Ldfld(typeof(Crop), nameof(Crop.harvestMethod)),
                OpCodes.Call, // NetCode implicit cast.
                OpCodes.Ldc_I4_1,
                OpCodes.Bne_Un
            );
            harvest_scythe.Replace(
                // crop.harvestMethod = temp_harvestmethod;
                harvest_scythe[0],
                harvest_scythe[1],
                harvest_scythe[2],
                Instructions.Ldloc_S(var_temp_harvestMethod),
                Instructions.Call_set(typeof(NetInt), nameof(NetInt.Value)),

                // if (ModEntry.CanHarvestCrop(crop, 1)) {
                Instructions.Ldarg_0(),
                harvest_scythe[1],
                Instructions.Ldc_I4_1(),
                Instructions.Call(typeof(ModEntry), nameof(CanHarvestCrop), typeof(Crop), typeof(int)),
                Instructions.Brfalse((Label)harvest_scythe[5].operand)
            );
        }

        [BytecodePatch("StardewValley.TerrainFeatures.HoeDirt::performUseAction")]
        void HoeDirt_performUseAction() {
            LocalBuilder var_temp_harvestMethod = generator.DeclareLocal(typeof(int));
            HoeDirt_performUseAction_hand(var_temp_harvestMethod);
            HoeDirt_performUseAction_scythe(var_temp_harvestMethod);
        }
#endregion

#region Patch Object
        public bool ScythableForageEnabled() {
            return config.HarvestMode.Forage != HarvestModeEnum.HAND;
        }
        
        [BytecodePatch("StardewValley.Object::performToolAction", "ScythableForageEnabled")]
        void Object_performToolAction() {
            var code = BeginCode();
            Label begin = AttachLabel(code[0]);
            code.Prepend(
                // Check if Tool is scythe.
                Instructions.Ldarg_1(),
                Instructions.Isinst(typeof(MeleeWeapon)),
                Instructions.Brfalse(begin),
                Instructions.Ldarg_1(),
                Instructions.Isinst(typeof(MeleeWeapon)),
                Instructions.Callvirt_get(typeof(Tool), nameof(Tool.BaseName)),
                Instructions.Ldstr("Scythe"),
                Instructions.Callvirt(typeof(string), nameof(string.Equals), typeof(string)),
                Instructions.Brfalse(begin),
                // Hook
                Instructions.Ldarg_0(),
                Instructions.Ldarg_1(),
                Instructions.Ldarg_2(),
                Instructions.Call(typeof(ModEntry), nameof(ScytheForage), typeof(StardewValley.Object), typeof(Tool), typeof(GameLocation)),
                Instructions.Brfalse(begin),
                Instructions.Ldc_I4_1(),
                Instructions.Ret()
            );
        }

        public static bool ScytheForage(StardewValley.Object o, Tool t, GameLocation loc) {
            if (o.IsSpawnedObject && !o.questItem.Value && o.isForage(loc) && CanHarvestObject(o, HARVEST_SCYTHING)) {
                var who = t.getLastFarmerToUse();
                var vector = o.TileLocation;
                // For objects stored in GameLocation.Objects, the TileLocation is not always set.
                // So determine its location by looping trough all such objects.
#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
                if (vector.X==0 && vector.Y==0) {
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator
                    foreach (System.Collections.Generic.KeyValuePair<Vector2, StardewValley.Object> pair in loc.Objects.Pairs) {
                        if (pair.Value.Equals(o)) {
                            vector = pair.Key;
                            break;
                        }
                    }
                }
                Random random = new Random((int)Game1.uniqueIDForThisGame / 2 + (int)Game1.stats.DaysPlayed + (int)vector.X + (int)vector.Y * 777);
                if (who.professions.Contains(16)) {
                    o.Quality = 4;
                } else if (random.NextDouble() < (double)((float)who.ForagingLevel / 30)) {
                    o.Quality = 2;
                } else if (random.NextDouble() < (double)((float)who.ForagingLevel / 15)) {
                    o.Quality = 1;
                }
                vector *= 64.0f;
                who.gainExperience(2, 7);
                Game1.createItemDebris(o.getOne(), vector, -1, null, -1);
                Game1.stats.ItemsForaged += 1;
                if (who.professions.Contains(13) && random.NextDouble() < 0.2) {
                    Game1.createItemDebris(o.getOne(), vector, -1, null, -1);
                    who.gainExperience(2, 7);
                }
                return true;
            } else {
                return false;
            }
        }

        [BytecodePatch("StardewValley.GameLocation::checkAction", "ScythableForageEnabled")]
        void GameLocation_checkAction() {
            // The code we need to patch is contained in a delegate, so use chain patching.
            MethodInfo method = (MethodInfo)FindCode(
                OpCodes.Ldftn
            )[0].operand;
            ChainPatch(method, AccessTools.Method(typeof(ModEntry), nameof(GameLocation_checkAction_Chain)));
        }

        void GameLocation_checkAction_Chain() {
            var var_object = generator.DeclareLocal(typeof(StardewValley.Object));
            InstructionRange code;
            Label cant_harvest;
            try {
                code = FindCode(
                    // if (who.couldInventoryAcceptThisItem (objects [vector])) {
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // who
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // objects
                    Instructions.Ldfld(typeof(GameLocation), nameof(GameLocation.objects)),
                    null, // Either LdLoc_1 or LdLoc_S(8).
                    OpCodes.Callvirt,
                    // <- Insert is here.
                    Instructions.Callvirt(typeof(Farmer), nameof(Farmer.couldInventoryAcceptThisItem), typeof(Item)),
                    OpCodes.Brfalse,
                    null // To make this InstructionRange have the same length as the one below for android.
                );
                cant_harvest = (Label)code[8].operand;
            } catch (Exception err) {
                LogException(err, LogLevel.Trace);
                
                // Android adds a boolean to the couldInventoryAcceptThisItem method.
                code = FindCode(
                    // if (who.couldInventoryAcceptThisItem (objects [vector], true)) {
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // who
                    OpCodes.Ldarg_0,
                    OpCodes.Ldfld, // objects
                    Instructions.Ldfld(typeof(GameLocation), nameof(GameLocation.objects)),
                    null, // Either LdLoc_1 or LdLoc_S(8).
                    OpCodes.Callvirt,
                    // <- Insert is here.
                    OpCodes.Ldc_I4_1,
                    Instructions.Callvirt(typeof(Farmer), nameof(Farmer.couldInventoryAcceptThisItem), typeof(Item), typeof(bool)),
                    OpCodes.Brfalse
                );
                cant_harvest = (Label)code[9].operand;
            }
            // Check whether harvesting forage by hand is allowed.
            code.Replace(
                // var object = objects [vector];
                code[0],
                code[3], // objects
                code[4],
                code[5],
                code[6],
                Instructions.Stloc_S(var_object),
                // if (ModEntry.CanHarvestObject(object, 0)) {
                Instructions.Ldloc_S(var_object),
                Instructions.Ldc_I4_0(),
                Instructions.Call(typeof(ModEntry), nameof(CanHarvestObject), typeof(StardewValley.Object), typeof(int)),
                Instructions.Brfalse(cant_harvest),
                // if (who.couldInventoryAcceptThisItem (object)) {
                code[2],
                code[1], // who
                Instructions.Ldloc_S(var_object),
                code[7], // true or couldInventoryAcceptThisItem
                code[8],
                code[9]
            );

            // Move to this.objects [vector].Quality = quality;
            code = code.FindNext(
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                Instructions.Ldfld(typeof(GameLocation), nameof(GameLocation.objects)),
                null, // Either LdLoc_1 or LdLoc_S(8).
                OpCodes.Callvirt,
                OpCodes.Ldloc_S,
                Instructions.Callvirt_set(typeof(StardewValley.Object), nameof(StardewValley.Object.Quality))
            );
            var label_dont_scythe = AttachLabel(code.End[0]);
            // Append code to handle trigger harvest with scythe.
            code.Append(
                // if (ModEntry.CanHarvest(object, HARVEST_SCYTHING) {
                Instructions.Ldloc_S(var_object),
                Instructions.Ldc_I4_1(), // HARVEST_SCYTHING
                Instructions.Call(typeof(ModEntry), nameof(ModEntry.CanHarvestObject), typeof(StardewValley.Object), typeof(int)),
                Instructions.Brfalse(label_dont_scythe),
                // ModEntry.TryScythe()
                Instructions.Call(typeof(ModEntry), nameof(ModEntry.TryScythe))
            );
        }
        
        static void TryScythe() {
            // Copied from HoeDirt.performUseAction()
            if (Game1.player.CurrentTool != null && Game1.player.CurrentTool is MeleeWeapon && (Game1.player.CurrentTool as MeleeWeapon).InitialParentTileIndex == 47) {
                Game1.player.CanMove = false;
                Game1.player.UsingTool = true;
                Game1.player.canReleaseTool = true;
                Game1.player.Halt ();
                try {
                    Game1.player.CurrentTool.beginUsing (Game1.currentLocation, (int)Game1.player.lastClick.X, (int)Game1.player.lastClick.Y, Game1.player);
                } catch (Exception) {
                }
                ((MeleeWeapon)Game1.player.CurrentTool).setFarmerAnimating (Game1.player);
            } 
        }

#endregion
    }
}

