/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Igorious/StardevValleyNewMachinesMod
**
*************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Igorious.StardewValley.DynamicAPI.Constants;
using Igorious.StardewValley.DynamicAPI.Data;
using Igorious.StardewValley.DynamicAPI.Data.Supporting;
using Igorious.StardewValley.DynamicAPI.Menu;
using Igorious.StardewValley.DynamicAPI.Objects;
using Igorious.StardewValley.DynamicAPI.Services;
using Igorious.StardewValley.DynamicAPI.Utils;
using Igorious.StardewValley.NewMachinesMod.Data;
using Igorious.StardewValley.NewMachinesMod.Menu;
using Igorious.StardewValley.NewMachinesMod.SmartObjects;
using Igorious.StardewValley.NewMachinesMod.SmartObjects.Dynamic;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using Log = Igorious.StardewValley.DynamicAPI.Utils.Log;

namespace Igorious.StardewValley.NewMachinesMod
{
    public partial class NewMachinesMod : Mod
    {
        public static NewMachinesModConfig Config { get; } = new NewMachinesModConfig();

        private List<MachineInformation> _machines;

        public override void Entry(params object[] objects)
        {
            Config.Load(PathOnDisk);
            _machines = new List<MachineInformation>(Config.SimpleMachines) { Config.Tank, Config.Mixer, Config.Separator, Config.Churn, Config.Fermenter };

            InitializeRecipes();
            InitializeObjectInformation();
            InitializeObjectMapping();
            InitializeGiftPreferences();
            InitializeBundles();
            OverrideTextures();
            PrecompileExpressions();
            RegisterCommands();
            ToolTipManager.Instance.Register<MachineInfoMenu>();
            ToolTipManager.Instance.Register<CropsInfoMenu>();
            LevelUpService.Instance.Register();
        }    

        private static void InitializeGiftPreferences()
        {
            Config.GiftPreferences.ForEach(GiftPreferencesService.Instance.AddGiftPreferences);
        }

        private static void InitializeObjectMapping()
        {
            ClassMapperService.Instance.MapCraftable<Tank>(Config.Tank.ID);
            ClassMapperService.Instance.MapCraftable<FullTank>(Config.Tank.ID + 1);
            ClassMapperService.Instance.MapCraftable<Mixer>(Config.Mixer.ID);
            ClassMapperService.Instance.MapCraftable<Separator>(Config.Separator.ID);
            ClassMapperService.Instance.MapCraftable<Churn>(Config.Churn.ID);
            ClassMapperService.Instance.MapCraftable<Fermenter>(Config.Fermenter.ID);

            Config.Totems.ForEach(t => ClassMapperService.Instance.MapItem<BigWarpTotem>(t.ID));
            Config.Items.ForEach(i => ClassMapperService.Instance.MapItem<SmartObject>(i.ID));
            Config.SimpleMachines.ForEach(m => ClassMapperService.Instance.MapCraftable(DynamicTypeInfo.Create<DynamicCustomMachine>(m.ID)));
            Config.MachineOverrides.ForEach(m => ClassMapperService.Instance.MapCraftable(DynamicTypeInfo.Create<DynamicOverridedMachine>(m.ID)));
        }

        private void InitializeRecipes()
        {
            _machines.ForEach(m => RecipesService.Instance.Register((CraftingRecipeInformation)m));
            //Config.Totems.ForEach(t => RecipesService.Instance.Register((CraftingRecipeInformation)t));
            Config.CookingRecipes.ForEach(RecipesService.Instance.Register);
            Config.CraftingRecipes.ForEach(RecipesService.Instance.Register);
        }

        private void InitializeObjectInformation()
        {
            _machines.ForEach(m => InformationService.Instance.Register((CraftableInformation)m));
            Config.Totems.ForEach(InformationService.Instance.Register);
            Config.ItemOverrides.ForEach(InformationService.Instance.Override);
            Config.Items.ForEach(InformationService.Instance.Register);
            Config.Crops.ForEach(InformationService.Instance.Register);
        }

        private static void InitializeBundles()
        {
            Config.Bundles.Added.ForEach(BundlesService.Instance.AddBundleItems);
            Config.Bundles.Removed.ForEach(BundlesService.Instance.RemoveBundleItems);
        }

        private void OverrideTextures()
        {
            var textureService = new TexturesService(PathOnDisk);
            _machines.ForEach(i => textureService.Override(TextureType.Craftables, i));
            Config.Items.ForEach(i => textureService.Override(TextureType.Items, i));
            Config.Totems.ForEach(i => textureService.Override(TextureType.Items, i));
            Config.Crops.ForEach(i => textureService.Override(TextureType.Crops, i));
            textureService.OverrideIridiumQualityStar();
        }

        private void PrecompileExpressions()
        {
            var compilingTask = Task.Run(() =>
            {
                try
                {
                    var machineOutputs = _machines.Select(m => m.Output).Concat(Config.MachineOverrides.Select(m => m.Output));
                    foreach (var machineOutput in machineOutputs)
                    {
                        if (machineOutput == null) continue;
                        ExpressionCompiler.CompileExpression<CountExpression>(machineOutput.Count);
                        ExpressionCompiler.CompileExpression<QualityExpression>(machineOutput.Quality);
                        ExpressionCompiler.CompileExpression<PriceExpression>(machineOutput.Price);

                        foreach (var outputItem in machineOutput.Items.Values)
                        {
                            if (outputItem == null) continue;
                            ExpressionCompiler.CompileExpression<CountExpression>(outputItem.Count);
                            ExpressionCompiler.CompileExpression<QualityExpression>(outputItem.Quality);
                            ExpressionCompiler.CompileExpression<PriceExpression>(outputItem.Price);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Fail during precompiling expressions! Cause: {e}");
                    throw;
                }
            });

            PlayerEvents.LoadedGame += (s, e) => compilingTask?.Wait();
        }
    }
}
