/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/bcmpinc/StardewHack
**
*************************************************/

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using GenericModConfigMenu;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewValley;
using StardewValley.GameData.Crops;
using StardewValley.TerrainFeatures;
using StardewValley.Tools;

namespace StardewHack.HarvestWithScythe
{
    public enum HarvestModeEnum {
        NONE, // crop cannot be harvested at all.
        HAND, // prevent scythe harvesting.
        IRID, // vanilla default.
        GOLD, // golden scythe can be used.
        BOTH, // determined by whether a scythe is equipped.
        SCYTHE, // cannot be harvested by hand.
    }

    public enum PluckingScytheEnum {
        NEVER,
        INVALID,
        ALWAYS,
    }

    public class ModConfig {
        /** Whether a sword can be used instead of a normal scythe. */
        public bool HarvestWithSword = false;

        /** Whether the scythe should work too on forage not above tilled soil. */
        public bool HarvestAllForage = true;

        /** Whether you can still pluck plants with a scythe equipped. */
        public PluckingScytheEnum PluckingScythe = PluckingScytheEnum.INVALID;
    
        /** How should flowers be harvested? 
         * Any object whose harvest has Object.flowersCategory. */
        public HarvestModeEnum Flowers = HarvestModeEnum.BOTH;

        /** How should forage be harvested? 
         * Any Object where `isForage() && isSpawnedObject && !questItem` evaluates to true is considered forage. */
        public HarvestModeEnum Forage = HarvestModeEnum.BOTH;

        /** How should pluckable crops & forage be harvested? 
         * Any Crop that has `GetHarvestMethod() == HarvestMethod.Grab` is considered a pluckable crop.
         * Any object that sits on top of HoeDirt is considered forage. */
        public HarvestModeEnum PluckableCrops = HarvestModeEnum.BOTH;

        /** How should scythable crops be harvested?
         * Any Crop that has `GetHarvestMethod() == HarvestMethod.Scythe` is considered a scythable crop. */
        public HarvestModeEnum ScythableCrops = HarvestModeEnum.SCYTHE;
    }

    /**
     * This is the core of the Harvest With Scythe mod.
     *
     * Crops are either harvested by hand, which is initiated by HoeDirt.PerformUseAction(), 
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
     * Flowers can have different colors, which is supported by the scythe harvesting code
     * in SDV 1.6. Whether a crop is a flower is detected using IsFlower, which is based on
     * Utility.findCloseFlower.
     *
     * Forage are plain Objects that exist on top of HoeDirt.
     */
    public class ModEntry : HackWithConfig<ModEntry, ModConfig> {
        public override void HackEntry(IModHelper helper) {
            I18n.Init(helper.Translation);
            Patch((Crop c) => c.harvest(0,0,null,null,false), Crop_harvest);
            Patch((HoeDirt hd) => hd.performUseAction(Vector2.Zero), HoeDirt_performUseAction);
            Patch((HoeDirt hd) => hd.performToolAction(null, 0, Vector2.Zero), HoeDirt_performToolAction);

            // If forage harvesting is configured to allow scythe.
            Patch((Object o) => o.performToolAction(null), Object_performToolAction);
            Patch((GameLocation gl) => gl.checkAction(new xTile.Dimensions.Location(), new xTile.Dimensions.Rectangle(), null), GameLocation_checkAction);
        }

        #region ModConfig
        protected override void InitializeApi(IGenericModConfigMenuApi api) {
            api.AddBoolOption(mod: ModManifest, name: I18n.HarvestWithSwordName, tooltip: I18n.HarvestWithSwordTooltip, getValue: () => config.HarvestWithSword, setValue: (bool val) => config.HarvestWithSword = val);
            api.AddBoolOption(mod: ModManifest, name: I18n.AllForageName,        tooltip: I18n.AllForageTooltip,        getValue: () => config.HarvestAllForage, setValue: (bool val) => config.HarvestAllForage = val);
            {// PluckingScythe Settings
                var options_dict = new Dictionary<PluckingScytheEnum, string>() {
                    {PluckingScytheEnum.NEVER,   I18n.PSNever()},
                    {PluckingScytheEnum.INVALID, I18n.PSInvalid()},
                    {PluckingScytheEnum.ALWAYS,  I18n.PSAlways()},
                };
                var reverse_dict = options_dict.ToDictionary(x=>x.Value, x=>x.Key);
                string[] options = options_dict.Values.ToArray();
                api.AddTextOption(mod: ModManifest, name: I18n.PluckingScytheName, tooltip: I18n.PluckingScytheTooltip, getValue: () => options_dict[config.PluckingScythe], setValue: (string val) => config.PluckingScythe = reverse_dict[val], allowedValues: options);
            }
            {// HarvestMode Settings
                var options_dict = new Dictionary<HarvestModeEnum, string>() {
                    {HarvestModeEnum.HAND,   I18n.HMHand()},
                    {HarvestModeEnum.IRID,   I18n.HMIrid()},
                    {HarvestModeEnum.GOLD,   I18n.HMGold()},
                    {HarvestModeEnum.BOTH,   I18n.HMBoth()},
                    {HarvestModeEnum.SCYTHE, I18n.HMScythe()},
                    {HarvestModeEnum.NONE,   I18n.HMNone()},
                };
                var reverse_dict = options_dict.ToDictionary(x=>x.Value, x=>x.Key);
                string[] options = options_dict.Values.ToArray();

                api.AddSectionTitle(mod: ModManifest, text: I18n.HarvestModeSection);
                api.AddTextOption(mod: ModManifest, name: I18n.PluckableCropsName, tooltip: I18n.PluckableCropsTooltip, getValue: () => options_dict[config.PluckableCrops], setValue: (string val) => config.PluckableCrops = reverse_dict[val], allowedValues: options);
                api.AddTextOption(mod: ModManifest, name: I18n.ScythableCropsName, tooltip: I18n.ScythableCropsTooltip, getValue: () => options_dict[config.ScythableCrops], setValue: (string val) => config.ScythableCrops = reverse_dict[val], allowedValues: options);
                api.AddTextOption(mod: ModManifest, name: I18n.FlowersName,        tooltip: I18n.FlowersTooltip,        getValue: () => options_dict[config.Flowers       ], setValue: (string val) => config.Flowers        = reverse_dict[val], allowedValues: options);
                api.AddTextOption(mod: ModManifest, name: I18n.ForageName,         tooltip: I18n.ForageTooltip,         getValue: () => options_dict[config.Forage        ], setValue: (string val) => config.Forage         = reverse_dict[val], allowedValues: options);
            }
        } 
#endregion 

#region CanHarvest methods
        static public bool IsFlower(Crop crop)
        {
            var data = ItemRegistry.GetData(crop.indexOfHarvest.Value);
            return data?.Category == Object.flowersCategory;
        }

        static public bool IsScythe(Tool t) {
            if (t == null) return false;
            if (t is MeleeWeapon) {
                return getInstance().config.HarvestWithSword || (t as MeleeWeapon).isScythe();
            }
            return false;
        }

        public static bool CheckMode(HarvestModeEnum mode, Tool tool) {
            switch (mode) {
                case HarvestModeEnum.SCYTHE: return true;
                case HarvestModeEnum.BOTH: return true;
                case HarvestModeEnum.GOLD: return tool.ItemId == MeleeWeapon.goldenScytheId || tool.ItemId == MeleeWeapon.iridiumScytheID;
                case HarvestModeEnum.IRID: return tool.ItemId == MeleeWeapon.iridiumScytheID;
                case HarvestModeEnum.HAND: return false;
                case HarvestModeEnum.NONE: return false;
                default:
                    throw new System.Exception("unreachable code");
            }
        }

        public static HarvestModeEnum GetHarvestSetting(Crop crop) {
            ModConfig config = getInstance().config;
            if (crop.GetHarvestMethod() == HarvestMethod.Scythe) return config.ScythableCrops;
            if (IsFlower(crop)) return config.Flowers;
            return config.PluckableCrops;
        }

        /** Determine whether the given crop can be harvested using a scythe. */
        public static bool CanScytheCrop(Crop crop, Tool tool) {
            if (crop == null) return false;
            return CheckMode(GetHarvestSetting(crop), tool);
        }

        public static bool CanScytheForage(Tool tool) {
            ModConfig config = getInstance().config;
            return CheckMode(config.Forage, tool);
        }
#endregion

#region Patch Crop
        void Crop_harvest() {
            var code = FindCode(
                // HarvestMethod harvestMethod = data?.HarvestMethod ?? HarvestMethod.Grab;
                OpCodes.Ldloc_S,
                OpCodes.Brtrue_S,
                OpCodes.Ldc_I4_0,
                OpCodes.Br_S,

	            // if (harvestMethod == HarvestMethod.Scythe || isForcedScytheHarvest)
                OpCodes.Ldloc_S,
                Instructions.Ldfld(typeof(CropData), nameof(CropData.HarvestMethod)),
                OpCodes.Stloc_S,
                OpCodes.Ldloc_S,
                OpCodes.Ldc_I4_1,
                OpCodes.Ceq,
                OpCodes.Ldarg_S,
                OpCodes.Or,
                OpCodes.Brfalse
            );
            code.Replace(
                // if (isForcedScytheHarvest)
                code[code.length-3],
                code[code.length-1]
            );
        }
#endregion

#region Patch HoeDirt
        void HoeDirt_performUseAction() {
            // Change the code that determines the harvest method for the crop &
            // checks and handles Iridium Scythe to allow any scythe that our mod accepts.
            var code = FindCode(
                // HarvestMethod harvestMethod = this.crop.GetHarvestMethod();
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                Instructions.Callvirt(typeof(Crop), nameof(Crop.GetHarvestMethod)),
                OpCodes.Stloc_1,

                // if (Game1.player.CurrentTool != null && Game1.player.CurrentTool.isScythe() && Game1.player.CurrentTool.ItemId == "66")
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Callvirt_get(typeof(Farmer), nameof(Farmer.CurrentTool)),
                OpCodes.Brfalse_S,
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Callvirt_get(typeof(Farmer), nameof(Farmer.CurrentTool)),
                Instructions.Callvirt(typeof(Tool), nameof(Tool.isScythe)),
                OpCodes.Brfalse_S,
                Instructions.Call_get(typeof(Game1), nameof(Game1.player)),
                Instructions.Callvirt_get(typeof(Farmer), nameof(Farmer.CurrentTool)),
                Instructions.Callvirt_get(typeof(Item), nameof(Item.ItemId)),
                Instructions.Ldstr("66"),
                OpCodes.Call,
                OpCodes.Brfalse_S,

                // harvestMethod = HarvestMethod.Scythe;
                OpCodes.Ldc_I4_1,
                OpCodes.Stloc_1
            );
            code.Replace(
                Instructions.Ldarg_0(),
                code[0],
                code[1],
                Instructions.Call(typeof(ModEntry), nameof(get_harvest_method), typeof(HoeDirt), typeof(Tool)),
                Instructions.Stloc_1()
            );

            // Replace the second Tool.isScythe call with our own.
            code = code.FindNext(
                Instructions.Callvirt(typeof(Tool), nameof(Tool.isScythe))
            );
            code[0] = Instructions.Call(typeof(ModEntry), nameof(IsScythe), typeof(Tool));
        }

        static HarvestMethod get_harvest_method(HoeDirt dirt, Tool tool) {
            return get_harvest_method(GetHarvestSetting(dirt.crop), tool);
        }

        static HarvestMethod get_harvest_method(HarvestModeEnum mode, Tool tool) {
            // Always force scythe if this is set as a non-pluckable crop (SCYTHE & NONE).
            if (mode == HarvestModeEnum.SCYTHE || mode == HarvestModeEnum.NONE) return HarvestMethod.Scythe;

            // Force scythe when trying to pluck depending on setting and whether the player is wielding a (valid) scythe.
            switch (getConfig().PluckingScythe) {
                case PluckingScytheEnum.NEVER:
                    if (IsScythe(tool)) return HarvestMethod.Scythe;
                    break;
                case PluckingScytheEnum.INVALID:
                    // Return whether the current tool can be used to harvest the crop.
                    if (IsScythe(tool) && CheckMode(mode, tool)) {
                        return HarvestMethod.Scythe;
                    }
                    break;
                case PluckingScytheEnum.ALWAYS:
                    break;
            }
            return HarvestMethod.Grab;
        }

        void HoeDirt_performToolAction() {
            // Replace Tool.isScythe call with our own method.
            var code = FindCode(
                Instructions.Ldarg_1(),
                Instructions.Callvirt(typeof(Tool), nameof(Tool.isScythe)),
                OpCodes.Brfalse
            );
            code[1] = Instructions.Call(typeof(ModEntry), nameof(IsScythe), typeof(Tool));

            // Replace the scythe check code with our own.
            var crop = FindCode(
                // if ((obj != null && obj.GetHarvestMethod() == HarvestMethod.Scythe) || (this.crop != null && t.ItemId == "66"))
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                OpCodes.Dup,
                OpCodes.Brtrue_S,
                OpCodes.Pop,
                OpCodes.Ldc_I4_0,
                OpCodes.Br_S,
                Instructions.Call(typeof(Crop), nameof(Crop.GetHarvestMethod)),
                OpCodes.Ldc_I4_1,
                OpCodes.Ceq,
                OpCodes.Brtrue_S,
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                OpCodes.Brfalse,
                OpCodes.Ldarg_1,
                Instructions.Callvirt_get(typeof(Item), nameof(Item.ItemId)),
                Instructions.Ldstr("66"),
                OpCodes.Call,
                OpCodes.Brfalse
            );
            crop.length--;
            crop.Replace(
                // if (ModEntry.CanScytheCrop(this.crop, t))
                Instructions.Ldarg_0(),
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                Instructions.Ldarg_1(),
                Instructions.Call(typeof(ModEntry), nameof(CanScytheCrop), typeof(Crop), typeof(Tool))
            );

            // Add fix for forage, including quality & xp
            var forage = crop.FindNext(
                // if (this.crop == null && 
                OpCodes.Ldarg_0,
                Instructions.Call_get(typeof(HoeDirt), nameof(HoeDirt.crop)),
                OpCodes.Brtrue_S,
                // t.ItemId == "66" &&
                OpCodes.Ldarg_1,
                Instructions.Callvirt_get(typeof(Item), nameof(Item.ItemId)),
                Instructions.Ldstr("66"),
                OpCodes.Call,
                OpCodes.Brfalse
            );
            forage.Extend(
	            // location.objects.Remove(tileLocation);
                OpCodes.Ldloc_0,
                Instructions.Ldfld(typeof(GameLocation), nameof(GameLocation.objects)),
                OpCodes.Ldarg_3,
                OpCodes.Callvirt,
                OpCodes.Pop
	        );
            forage.Replace(
                Instructions.Ldarg_0(),
                Instructions.Ldarg_1(),
                Instructions.Ldloc_0(),
                Instructions.Ldarg_3(),
                Instructions.Call(typeof(ModEntry), nameof(harvest_forage_with_xp), typeof(HoeDirt), typeof(Tool), typeof(GameLocation), typeof(Vector2))
            );
        }

        static void harvest_forage_with_xp(HoeDirt dirt, Tool t, GameLocation location, Vector2 tileLocation) {
            if (dirt.crop == null && CanScytheForage(t) && location.objects.ContainsKey(tileLocation) && location.objects[tileLocation].isForage()) {
				Object o = location.objects[tileLocation];
                harvest_forage_with_xp(o, t, tileLocation);
				location.objects.Remove(tileLocation);
			}
        }

        static void harvest_forage_with_xp(Object o, Tool t, Vector2 tileLocation) {
            System.Random r = Utility.CreateDaySaveRandom(tileLocation.X, tileLocation.Y * 777f);
            var who = t.getLastFarmerToUse();
			if (t.getLastFarmerToUse() != null && who.professions.Contains(16)) {
				o.Quality = 4;
			} else if (r.NextDouble() < (double)(who.ForagingLevel / 30f)) {
				o.Quality = 2;
			} else if (r.NextDouble() < (double)(who.ForagingLevel / 15f)) {
				o.Quality = 1;
			}
            who.gainExperience(2, 7);
            Game1.stats.ItemsForaged += 1;
            var vector = new Vector2(tileLocation.X * 64f + 32f, tileLocation.Y * 64f + 32f);
			Game1.createItemDebris(o, vector, -1);
            if (who.professions.Contains(13) && r.NextDouble() < 0.2) {
                who.gainExperience(2, 7);
                Game1.createItemDebris(o.getOne(), vector, -1, null, -1);
            }
        }
#endregion

#region Patch Object
        void Object_performToolAction() {
            // Inject code that allows harvesting forage objects with the scythe.
            var code = BeginCode();
            Label begin = AttachLabel(code[0]);
            code.Prepend(
                // Hook
                Instructions.Ldarg_0(),
                Instructions.Ldarg_1(),
                Instructions.Call(typeof(ModEntry), nameof(ScytheForage), typeof(Object), typeof(Tool)),
                Instructions.Brfalse(begin),
                Instructions.Ldc_I4_1(),
                Instructions.Ret()
            );
        }

        public static bool ScytheForage(Object o, Tool t) {
            var config = getConfig();
            if (config.HarvestAllForage && IsForage(o) && IsScythe(t) && CanScytheForage(t)) {
                var vector = o.TileLocation;
                // For objects stored in GameLocation.Objects, the TileLocation is not always set.
                // So determine its location by looping trough all such objects.
                if (vector == Vector2.Zero) {
                    getInstance().Monitor.LogOnce("Harvesting forage with invalid location.", LogLevel.Info);
                    var loc = o.Location;
                    foreach (KeyValuePair<Vector2, Object> pair in loc.Objects.Pairs) {
                        if (pair.Value.Equals(o)) {
                            vector = pair.Key;
                            break;
                        }
                    }
                }
                harvest_forage_with_xp(o, t, vector);
                return true;
            } else {
                return false;
            }
        }
        
        void GameLocation_checkAction() {
            var code = FindCode(
                // int oldQuality = obj.quality;
                OpCodes.Ldloc_3, // obj
                Instructions.Ldfld(typeof(Item), nameof(Item.quality)),
                OpCodes.Call,
                OpCodes.Stloc_S
            );
            // (Nothing jumps here.)
            code.Prepend(
                Instructions.Ldloc_3(),
                Instructions.Ldarg_3(),
                Instructions.Call(typeof(ModEntry), nameof(MayTriggerScythe), typeof(Object), typeof(Farmer)),
                Instructions.Brfalse(AttachLabel(code[0])),
                Instructions.Ldc_I4_1(),
                Instructions.Ret()
            );
        }
        
        static bool IsForage(Object o) {
            // This somewhat reliably decides if something is forage.
            return o.IsSpawnedObject && !o.questItem.Value && o.isForage();
        }

        static bool MayTriggerScythe(Object o, Farmer who) {
            // Force scythe when trying to pluck depending on setting and whether the player is wielding a (valid) scythe.
            var tool = who.CurrentTool;
            if (IsForage(o) && (getConfig().HarvestAllForage || o.Location.isTileHoeDirt(o.TileLocation))) {
                var mode = get_harvest_method(getConfig().Forage, tool);
                if (mode == HarvestMethod.Grab) return false;

                // Copied from HoeDirt.performUseAction()
                if (IsScythe(tool) && CanScytheForage(tool)) {
                    who.CanMove = false;
                    who.UsingTool = true;
                    who.canReleaseTool = true;
                    who.Halt();
                    try {
                        tool.beginUsing(o.Location, (int)who.lastClick.X, (int)who.lastClick.Y, who);
                    } catch (System.Exception) {
                    }
                    ((MeleeWeapon)tool).setFarmerAnimating (Game1.player);
                } else if (Game1.didPlayerJustClickAtAll(ignoreNonMouseHeldInput: true)) {
                    // Requires scythe message
                    Game1.showRedMessage(Game1.content.LoadString("Strings\\StringsFromCSFiles:HoeDirt.cs.13915"));
                }
                return true;
            } else {
                return false;
            }
        }

#endregion
    }
}

