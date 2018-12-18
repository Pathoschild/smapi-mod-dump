using System;
using System.Reflection.Emit;
using StardewModdingAPI;

namespace StardewHack.HarvestWithScythe
{
    public class ModConfig {
        /** Should the game be patched to allow harvesting forage with the scythe? */
        public bool HarvestForage = true;
        /** Should quality be applied to additional harvest? */
        public bool AllHaveQuality = false;
        /** Can flowers be harvested with the scythe? */
        public bool ScytheHarvestFlowers = true;
        /** Whether crops should also remain pluckable by hand. */
        public bool AllowManualHarvest = true;
    }

    public class ModEntry : HackWithConfig<ModEntry, ModConfig>
    {
        [BytecodePatch("StardewValley.Crop::harvest")]
        void Crop_harvest() {
            #region Fix vector
            Harmony.CodeInstruction ins = null;
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
                ins = vec[5];
                vec.Remove();
            }
            
            // Add to begin of function
            // Vector2 vector = new Vector2 ((float)xTile*64., (float)yTile*64.);
            BeginCode().Append(
                Instructions.Ldloca_S(3),
                Instructions.Ldarg_1(),
                Instructions.Conv_R4(),
                Instructions.Ldc_R4(64),
                Instructions.Mul(),
                Instructions.Ldarg_2(),
                Instructions.Conv_R4(),
                Instructions.Ldc_R4(64),
                Instructions.Mul(),
                ins
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
                    Instructions.Ldloc_3() // vector
                );
            }
            #endregion

            #region Support harvesting of spring onions with scythe
            // Note: the branch
            //   if (this.forageCrop)
            // refers mainly to the crop spring union.
            
            // Find the lines:
            var AddItem = FindCode(
                // if (Game1.player.addItemToInventoryBool (@object, false)) {
                Instructions.Call_get(typeof(StardewValley.Game1), "player"),
                OpCodes.Ldloc_0,
                OpCodes.Ldc_I4_0,
                Instructions.Callvirt(typeof(StardewValley.Farmer), "addItemToInventoryBool", typeof(StardewValley.Item), typeof(bool)),
                OpCodes.Brfalse
            );

            // Swap the lines (add '*64' to vector) &
            // Insert check for harvesting with scythe and act accordingly.
            AddItem.Prepend(
                // if (this.harvestMethod != 0) {
                Instructions.Ldarg_0(),
                Instructions.Ldfld(typeof(StardewValley.Crop), "harvestMethod"),
                Instructions.Call_get(typeof(Netcode.NetInt), "Value"),
                Instructions.Brfalse(AttachLabel(AddItem[0])),
                // Game1.createItemDebris (@object, vector, -1, null, -1)
                Instructions.Ldloc_0(), // @object
                Instructions.Ldloc_3(), // vector
                Instructions.Ldc_I4_M1(), // -1
                Instructions.Ldnull(), // null
                Instructions.Ldc_I4_M1(), // -1
                Instructions.Call(typeof(StardewValley.Game1), "createItemDebris", typeof(StardewValley.Item), typeof(Microsoft.Xna.Framework.Vector2), typeof(int), typeof(StardewValley.GameLocation), typeof(int)),
                // Game1.player.gainExperience (2, howMuch);
                Instructions.Call_get(typeof(StardewValley.Game1), "player"),
                Instructions.Ldc_I4_2(),
                Instructions.Ldloc_1(),
                Instructions.Callvirt(typeof(StardewValley.Farmer), "gainExperience", typeof(int), typeof(int)),
                // return true
                Instructions.Ldc_I4_1(),
                Instructions.Ret()
                // }
            );
            #endregion

            #region Sunflower drops 
            // >>> Patch code to drop sunflower seeds when harvesting with scythe.
            // >>> Patch code to let harvesting with scythe drop only 1 item.
            // >>> The other item drops are handled by the plucking code.

            // Remove start of loop
            FindCode(
                OpCodes.Ldc_I4_0,
                Instructions.Stloc_S(12),
                OpCodes.Br
            ).Remove();

            // Find the start of the 'drop sunflower seeds' part.
            var DropSunflowerSeeds = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(StardewValley.Crop), "indexOfHarvest"),
                OpCodes.Call, // Netcode
                Instructions.Ldc_I4(421), // 421 = Item ID of Sunflower.
                OpCodes.Bne_Un
            );
            // Set quality for seeds to 0.
            DropSunflowerSeeds.Append(
                Instructions.Ldc_I4_0(),
                Instructions.Stloc_S(5)
            );

            // Remove end of loop and everything after that until the end of the harvest==1 branch.
            var ScytheBranchTail = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Ldfld(typeof(StardewValley.Crop), "harvestMethod"),
                OpCodes.Call, // Netcode
                OpCodes.Ldc_I4_1,
                OpCodes.Bne_Un
            ).Follow(4);
            ScytheBranchTail.ExtendBackwards(
                Instructions.Ldloc_S(12),
                OpCodes.Ldc_I4_1,
                OpCodes.Add,
                Instructions.Stloc_S(12),
                Instructions.Ldloc_S(12),
                Instructions.Ldloc_S(4),
                OpCodes.Blt
            );
            
            // Change jump to end of loop into jump to drop sunflower seeds.
            ScytheBranchTail.ReplaceJump(0, DropSunflowerSeeds[0]);

            // Rewrite the tail of the Scythe harvest branch. 
            ScytheBranchTail.Replace(
                // Jump to the 'drop subflower seeds' part.
                Instructions.Br(AttachLabel(DropSunflowerSeeds[0]))
            );
            #endregion

            #region Colored flowers
            // For colored flowers we need to call createItemDebris instead of createObjectDebris
            FindCode(
                // Game1.createObjectDebris (indexOfHarvest, xTile, yTile, -1, num3, 1f, null);
                OpCodes.Ldarg_0,
                OpCodes.Ldfld,
                OpCodes.Call,
                OpCodes.Ldarg_1,
                OpCodes.Ldarg_2,
                OpCodes.Ldc_I4_M1,
                OpCodes.Ldloc_S,
                OpCodes.Ldc_R4,
                OpCodes.Ldnull,
                OpCodes.Call
            ).Replace(
                // var tmp = CreateObject(this, num3);
                Instructions.Ldarg_0(), // this
                Instructions.Ldloc_S(5), // num3
                Instructions.Call(typeof(ModEntry), "CreateObject", typeof(StardewValley.Crop), typeof(int)),
                // Game1.createItemDebris(tmp, vector, -1, null, -1);
                Instructions.Ldloc_3(), // vector
                Instructions.Ldc_I4_M1(), // -1
                Instructions.Ldnull(), // null
                Instructions.Ldc_I4_M1(), // -1
                Instructions.Call(typeof(StardewValley.Game1), "createItemDebris", typeof(StardewValley.Item), typeof(Microsoft.Xna.Framework.Vector2), typeof(int), typeof(StardewValley.GameLocation), typeof(int))
            );
            #endregion

            if (config.AllHaveQuality) {
                // Patch function calls for additional harvest to pass on the harvest quality.
                FindCode(
                    OpCodes.Ldc_I4_M1,
                    OpCodes.Ldc_I4_0,
                    Instructions.Ldc_R4(1.0f),
                    OpCodes.Ldnull,
                    Instructions.Call(typeof(StardewValley.Game1), "createObjectDebris", typeof(int), typeof(int), typeof(int), typeof(int), typeof(int), typeof(float), typeof(StardewValley.GameLocation))
                )[1] = Instructions.Ldloc_S(5);

                FindCode(
                    OpCodes.Ldc_I4_1,
                    OpCodes.Ldc_I4_0,
                    OpCodes.Ldc_I4_M1,
                    OpCodes.Ldc_I4_0,
                    OpCodes.Newobj,
                    Instructions.Callvirt(typeof(StardewValley.Characters.JunimoHarvester), "tryToAddItemToHut", typeof(StardewValley.Item))
                )[3] = Instructions.Ldloc_S(5);
            }
            
            if (!config.ScytheHarvestFlowers) {
                var lbl = AttachLabel(instructions[0]);
                BeginCode().Append(
                    // if (harvestMethod==1 && programColored) {
                    Instructions.Ldarg_0(),
                    Instructions.Ldfld(typeof(StardewValley.Crop), "harvestMethod"),
                    Instructions.Call_get(typeof(Netcode.NetInt), "Value"),
                    Instructions.Brfalse(lbl),
                    Instructions.Ldarg_0(),
                    Instructions.Ldfld(typeof(StardewValley.Crop), "programColored"),
                    Instructions.Call_get(typeof(Netcode.NetBool), "Value"),
                    Instructions.Brfalse(lbl),
                    // return false
                    Instructions.Ldc_I4_0(),
                    Instructions.Ret()
                    // }
                );
            }
        }

        // Proxy method for creating an object suitable for spawning as debris.
        public static StardewValley.Object CreateObject(StardewValley.Crop crop, int quality) {
            if (crop.programColored) {
                return new StardewValley.Objects.ColoredObject (crop.indexOfHarvest, 1, crop.tintColor) {
                    Quality = quality
                };
            } else {
                return new StardewValley.Object(crop.indexOfHarvest, 1, false, -1, quality);
            }
        }

        [BytecodePatch("StardewValley.TerrainFeatures.HoeDirt::performToolAction")]
        void HoeDirt_performToolAction() {
            // Find the first harvestMethod==1 check.
            var HarvestMethodCheck = FindCode(
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(StardewValley.TerrainFeatures.HoeDirt), "crop"),
                Instructions.Ldfld(typeof(StardewValley.Crop), "harvestMethod"),
                OpCodes.Call, // Netcode
                OpCodes.Ldc_I4_1,
                OpCodes.Bne_Un
            );

            // Change the harvestMethod==1 check to damage=harvestMethod; harvestMethod=1
            HarvestMethodCheck.Replace(
                // damage = crop.harvestMethod.
                HarvestMethodCheck[0],
                HarvestMethodCheck[1],
                HarvestMethodCheck[2],
                HarvestMethodCheck[3],
                Instructions.Starg_S(2), // damage

                // crop.harvestMethod = 1
                HarvestMethodCheck[0],
                HarvestMethodCheck[1],
                HarvestMethodCheck[2],
                Instructions.Ldc_I4_1(),
                Instructions.Call_set(typeof(Netcode.NetInt), "Value")
            );

            // Set harvestMethod=damage after the following crop!=null check.
            HarvestMethodCheck.FindNext(
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(StardewValley.TerrainFeatures.HoeDirt), "crop"),
                Instructions.Ldfld(typeof(StardewValley.Crop), "dead"),
                OpCodes.Call, // Netcode
                OpCodes.Brfalse
            ).Prepend(
                HarvestMethodCheck[0],
                HarvestMethodCheck[1],
                HarvestMethodCheck[2],
                Instructions.Ldarg_2(), // damage
                Instructions.Call_set(typeof(Netcode.NetInt), "Value")
            );
        }

        public bool DisableHandHarvesting() {
            return !config.AllowManualHarvest;
        }
        
        [BytecodePatch("StardewValley.TerrainFeatures.HoeDirt::performUseAction", "DisableHandHarvesting")]
        void HoeDirt_performUseAction() {
            var harvest_hand = FindCode(
                OpCodes.Ldarg_0,
                OpCodes.Call,
                OpCodes.Ldfld,
                OpCodes.Call,
                OpCodes.Brtrue
            );
            // Logic here depends on whether flowers can be harvested by scythe.
            if (config.ScytheHarvestFlowers) {
                // Entirely remove logic related to harvesting by hand.
                harvest_hand.Extend(
                    OpCodes.Ldarg_0,
                    OpCodes.Call,
                    OpCodes.Ldfld,
                    OpCodes.Call,
                    OpCodes.Ldc_I4_1,
                    OpCodes.Bne_Un
                );
                harvest_hand.Remove();
            } else {
                // Only allow harvesting by hand for flowers. Otherwise those would not be harvestable.
                harvest_hand.Replace(
                    harvest_hand[0],
                    harvest_hand[1],
                    Instructions.Ldfld(typeof(StardewValley.Crop), "programColored"),
                    Instructions.Call_get(typeof(Netcode.NetBool), "Value"),
                    Instructions.Brfalse((Label)harvest_hand[4].operand)
                );
                var harvest_scythe = FindCode(
                    OpCodes.Ldarg_0,
                    OpCodes.Call,
                    OpCodes.Ldfld,
                    OpCodes.Call,
                    OpCodes.Ldc_I4_1,
                    OpCodes.Bne_Un
                );
                harvest_scythe.Replace(
                    harvest_scythe[0],
                    harvest_scythe[1],
                    Instructions.Ldfld(typeof(StardewValley.Crop), "programColored"),
                    Instructions.Call_get(typeof(Netcode.NetBool), "Value"),
                    Instructions.Brtrue((Label)harvest_scythe[5].operand)
                );
            }
        }

        public bool HarvestForageEnabled() {
            return config.HarvestForage;
        }
        
        [BytecodePatch("StardewValley.Object::performToolAction", "HarvestForageEnabled")]
        void Object_performToolAction() {
            var code = BeginCode();
            Label begin = AttachLabel(code[0]);
            code.Prepend(
                // Check if Tool is scythe.
                Instructions.Ldarg_1(),
                Instructions.Isinst(typeof(StardewValley.Tools.MeleeWeapon)),
                Instructions.Brfalse(begin),
                Instructions.Ldarg_1(),
                Instructions.Isinst(typeof(StardewValley.Tools.MeleeWeapon)),
                Instructions.Callvirt_get(typeof(StardewValley.Tool), "BaseName"),
                Instructions.Ldstr("Scythe"),
                Instructions.Callvirt(typeof(System.String), "Equals", typeof(string)),
                Instructions.Brfalse(begin),
                // Hook
                Instructions.Ldarg_0(),
                Instructions.Ldarg_1(),
                Instructions.Ldarg_2(),
                Instructions.Call(typeof(ModEntry), "ScytheForage", typeof(StardewValley.Object), typeof(StardewValley.Tool), typeof(StardewValley.GameLocation)),
                Instructions.Brfalse(begin),
                Instructions.Ldc_I4_1(),
                Instructions.Ret()
            );
        }

        public static bool ScytheForage(StardewValley.Object o, StardewValley.Tool t, StardewValley.GameLocation loc) {
            if (o.isSpawnedObject && !o.questItem && o.isForage(loc)) {
                var who = t.getLastFarmerToUse();
                var vector = o.TileLocation;
                // For objects stored in GameLocation.Objects, the TileLocation is not always set.
                // So determine its location by looping trough all such objects.
                if (vector.X==0 && vector.Y==0) {
                    foreach (System.Collections.Generic.KeyValuePair<Microsoft.Xna.Framework.Vector2, StardewValley.Object> pair in loc.Objects.Pairs) {
                        if (pair.Value.Equals(o)) {
                            vector = pair.Key;
                            break;
                        }
                    }
                }
                int quality = o.quality;
                Random random = new Random((int)StardewValley.Game1.uniqueIDForThisGame / 2 + (int)StardewValley.Game1.stats.DaysPlayed + (int)vector.X + (int)vector.Y * 777);
                if (who.professions.Contains(16)) {
                    quality = 4;
                } else if (random.NextDouble() < (double)((float)who.ForagingLevel / 30)) {
                    quality = 2;
                } else if (random.NextDouble() < (double)((float)who.ForagingLevel / 15)) {
                    quality = 1;
                }
                who.gainExperience(2, 7);
                StardewValley.Game1.createObjectDebris(o.ParentSheetIndex, (int)vector.X, (int)vector.Y, -1, quality, 1, loc);
                StardewValley.Game1.stats.ItemsForaged += 1;
                if (who.professions.Contains(13) && random.NextDouble() < 0.2) {
                    StardewValley.Game1.createObjectDebris(o.ParentSheetIndex, (int)vector.X, (int)vector.Y, -1, quality, 1, loc);
                    who.gainExperience(2, 7);
                }
                return true;
            } else {
                return false;
            }
        }
    }
}

