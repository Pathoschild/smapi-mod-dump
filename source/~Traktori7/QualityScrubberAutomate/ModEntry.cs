/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Traktori7/StardewValleyMods
**
*************************************************/

using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;
using SObject = StardewValley.Object;
using Pathoschild.Stardew.Automate;
using QualityScrubber;


namespace QualityScrubberAutomate
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        private const string automateUniqueID = "Pathoschild.Automate";
        private const string qualityScrubberUniqueID = "Traktori.QualityScrubber";
        private IAutomateAPI automateApi;
        private IQualityScrubberApi qualityScrubberApi;


        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += this.OnGameLaunched;
        }


        /*********
        ** Private methods
        *********/
        private void OnGameLaunched(object sender, GameLaunchedEventArgs e)
        {
            automateApi = Helper.ModRegistry.GetApi<IAutomateAPI>(automateUniqueID);
            
            qualityScrubberApi = Helper.ModRegistry.GetApi<IQualityScrubberApi>(qualityScrubberUniqueID);

            if (automateApi != null && qualityScrubberApi != null)
            {
                automateApi.AddFactory(new QualityScrubberAutomationFactory(qualityScrubberApi.controller));
            }
            else
            {
                if (automateApi is null)
                {
                    Monitor.Log("Could not detect Automate. Are you sure you have installed everything correctly?", LogLevel.Error);
                }
                if (qualityScrubberApi is null)
                {
                    Monitor.Log("Could not detect Quality Scrubber. Are you sure you have installed everything correctly?", LogLevel.Error);
                }
            }
        }
    }


    public class QualityScrubberMachine : IMachine
    {
        private readonly SObject machineObject;
        private QualityScrubberController controller;

        public string MachineTypeID { get; }
        public GameLocation Location { get; }
        public Rectangle TileArea { get; }


        public QualityScrubberMachine(QualityScrubberController controller, SObject entity, GameLocation location, in Vector2 tile)
        {
            this.controller = controller;
            this.machineObject = entity;
            this.Location = location;
            this.TileArea = new Rectangle((int)tile.X, (int)tile.Y, 1, 1);

            this.MachineTypeID = "Quality Scrubber";
        }


        public MachineState GetState()
        {
            if (machineObject.heldObject.Value is null)
                return MachineState.Empty;

            return machineObject.readyForHarvest.Value ? MachineState.Done : MachineState.Processing;
        }


        public ITrackedStack GetOutput()
        {
            return new TrackedItem(machineObject.heldObject.Value, onEmpty: item =>
            {
                machineObject.heldObject.Value = null;
                machineObject.readyForHarvest.Value = false;
            });
        }


        public bool SetInput(IStorage input)
        {
            if (input.TryGetIngredient(item => controller.CanProcess(item.Sample, machineObject), 1, out IConsumable consumable))
            {
                Item ingredient = consumable.Take();

                machineObject.heldObject.Value = controller.GetOutputObject(ingredient);
                machineObject.MinutesUntilReady = controller.Duration;

                return true;
            }

            return false;
        }
    }
}