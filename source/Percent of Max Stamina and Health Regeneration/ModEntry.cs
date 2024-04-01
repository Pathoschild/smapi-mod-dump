/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/MercuriusXeno/RegenPercent
**
*************************************************/

using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace RegenPercent
{
    /// <summary>
    ///     The mod entry for regen-percent. 
    ///     Needed for there to be a mod.
    /// </summary>
    internal sealed class ModEntry : Mod
    {
        // holder for our config data,which we load during entry
        private ConfigData? _config;
        // constant coefficients to help convert the user-configured % and the
        // seconds-per-tick into a per-tick regen amount for fine resolution regeneration
        private const decimal _percentCoefficient = 0.01m;
        private const decimal _secondsPerTickCoefficient = 1m / 60m;
        // maximum values of health and stamina regen. stamina is lower because it's harder to lose that much stamina at a time.
        private const decimal _maxHealthRegen = 100m;
        private const decimal _maxStaminaRegen = 10m;
        // the smallest-step constant is just the tiniest amount you can step by on the lower bound
        private const decimal _smallestStep = 0.01m;
        // dictionaries to hold our step values so we don't have to keep generating them dynamically
        private readonly Dictionary<int, decimal> _staminaRegenSteps = new Dictionary<int, decimal>();
        private readonly Dictionary<int, decimal> _healthRegenSteps = new Dictionary<int, decimal>();
        // the step "grades". when the step size reaches grade / 10,
        // we set the step size multiplier to the grade. this gives us varying step sizes.
        private readonly List<int?> _stepGrades = new List<int?> { 5, 10, 20, 50, 100, 200, 500 };
        // the default step for each is whatever puts us on 0.2, at the time of writing
        public const int DefaultHealthRegenRateStep = 4;
        public const int DefaultStaminaRegenRateStep = 4;
        // tracks the amount of regen we actually received per tick
        private decimal _healthRegenTracker = 0m;
        private decimal _staminaRegenTracker = 0m;


        /// <summary>
        ///     Needed to supply the mod entry point which lets the mod do things.
        /// </summary>
        /// <param name="helper">A provider-pattern helper that supplies api methods, events, etc</param>
        public override void Entry(IModHelper helper)
        {
            CreateRegenSteps();
            ReadConfig(helper);
            HookEvents(helper);
        }

        /// <summary>
        ///     Needed to establish the regen steps the player can select in the config, since they vary in step size
        ///     at the lower end vs the higher end. Lower bounds have smaller steps.
        /// </summary>        
        private void CreateRegenSteps()
        {
            PopulateStepsOfDictionary(_healthRegenSteps, _maxHealthRegen);
            PopulateStepsOfDictionary(_staminaRegenSteps, _maxStaminaRegen);
            
        }

        /// <summary>
        ///     Fills a dictionary with step-to-decimal mappings, making it easier to have customized step grades at different levels
        /// </summary>
        /// <param name="dictionary">The dictionary we're filling with steps</param>
        private void PopulateStepsOfDictionary(Dictionary<int, decimal> dictionary, decimal maxRegen)
        {
            // start at 0, 0
            var index = 0;
            dictionary[index] = 0m;
            // step over steps and establish a human-friendly step grade for various steps, bypassing fine grades at higher bounds
            while (dictionary[index] < maxRegen)
            {
                index++;
                dictionary[index] = dictionary[index - 1] + GetStepSizeOfIndex(index, dictionary);
            }
        }

        /// <summary>
        ///     Needed to populate the health and stamina regen dictionaries with ideal step sizes.
        /// </summary>
        /// <param name="i">The index we're trying to generate the step size of</param>
        /// <param name="dict">The dictionary we're populating with step-values</param>
        /// <returns>The step size of a given step, based on the value in the dictionary of the previous step</returns>        
        private decimal GetStepSizeOfIndex(int i, Dictionary<int, decimal> dict) => dict.ContainsKey(i - 1) ? GetStepMultiplier(dict[i - 1]) * _smallestStep : 0m;

        /// <summary>
        ///     Needed to get the step multiplier of a given step of regen, based on the last step value.
        /// </summary>
        /// <param name="lastStepValue"></param>
        /// <returns>The step multiplier of a step based on the last step's value</returns>
        private int GetStepMultiplier(decimal lastStepValue) => _stepGrades.FirstOrDefault(m => lastStepValue < m / 10m) ?? _stepGrades.Last() ?? 0;


        /// <summary>
        ///     Needed to wire up the mod's events.
        /// </summary>
        /// <param name="helper">Helper instance needed to hook events</param>
        private void HookEvents(IModHelper helper)
        {
            helper.Events.GameLoop.GameLaunched += GameLaunched;
            helper.Events.GameLoop.UpdateTicked += UpdateTicked;
        }

        /// <summary>
        ///     Needed to read the config from a file if it exists
        /// </summary>
        /// <param name="helper">Helper instance needed to read configs</param>
        private void ReadConfig(IModHelper helper) => _config = helper.ReadConfig<ConfigData>();

        /// <summary>
        ///     Needed to update stamina and health by a percentage, if applicable.
        /// </summary>
        /// <param name="sender">The event sender as a nullable object</param>
        /// <param name="e">The event arguments</param>   
        private void UpdateTicked(object? sender, UpdateTickedEventArgs e)
        {
            if (IsContextValidForRegen())
            {
                HandleRegen(Game1.player);
            }
        }

        /// <summary>
        ///     Needed to track whether player has received enough regen
        ///     to increment health or stamina.
        /// </summary>
        /// <param name="player">The player regenerating</param>        
        private void HandleRegen(Farmer player)
        {
            HandleHealthRegen(player);
            HandleStaminaRegen(player);
        }

        /// <summary>
        ///     Needed to do the stamina portion of regenerating
        /// </summary>
        /// <param name="player">The player regenerating</param>    
        private void HandleStaminaRegen(Farmer player)
        {
            _staminaRegenTracker += player.MaxStamina * GetStaminaRegenCoefficient();
            var regen = (int)Math.Floor(_staminaRegenTracker);
            if (regen >= 1)
            {
                _staminaRegenTracker -= regen;
                player.Stamina = Math.Min(player.MaxStamina, player.Stamina + regen);
            }
        }

        /// <summary>
        ///     Needed to do the health portion of regenerating
        /// </summary>
        /// <param name="player">The player regenerating</param>    
        private void HandleHealthRegen(Farmer player)
        {
            _healthRegenTracker += player.maxHealth * GetHealthRegenCoefficient();
            var regen = (int)Math.Floor(_healthRegenTracker);
            if (regen >= 1)
            {
                _healthRegenTracker -= regen;
                player.health = Math.Min(player.maxHealth, player.health + regen);
            }
        }

        /// <summary>
        ///     Needed to get the health regen rate of the player
        /// </summary>
        /// <returns>The percentage of health to regenerate per game-tick</returns>
        private decimal GetHealthRegenCoefficient() => _healthRegenSteps[GetHealthRegenStep()] * GetTickCoefficient();

        /// <summary>
        ///     Needed to get the config step of health regen
        /// </summary>
        /// <returns>0 if config is null, otherwise the step defined by config</returns>
        private int GetHealthRegenStep() => _config?.HealthRegenRateStep ?? 0;

        /// <summary>
        ///     Needed to get the stamina regen rate of the player
        /// </summary>
        /// <returns>The percentage of stamina to regenerate per game-tick</returns>
        private decimal GetStaminaRegenCoefficient() => _staminaRegenSteps[GetStaminaRegenStep()] * GetTickCoefficient();

        /// <summary>
        ///     Needed to convert the user defined rate into a percent, and then divide it by ticks per second.
        /// </summary>
        /// <returns>A coefficient to multiply per-second regen by to get per-tick regen.</returns>
        private decimal GetTickCoefficient() => _percentCoefficient * _secondsPerTickCoefficient;

        /// <summary>
        ///     Needed to get the config step of stamina regen
        /// </summary>
        /// <returns>0 if config is null, otherwise the step defined by config</returns>
        private int GetStaminaRegenStep() => _config?.StaminaRegenRateStep ?? 0;

        /// <summary>
        ///     Needed to determine if regen should happen on a given tick
        /// </summary>
        /// <returns>True if regen is valid, false otherwise</returns>        
        private bool IsContextValidForRegen() => Context.IsWorldReady && Context.IsPlayerFree;

        /// <summary>
        ///     Needed to fire anything that waits for the game to launch.
        /// </summary>
        /// <param name="sender">The event sender as a nullable object</param>
        /// <param name="e">The event arguments</param>
        private void GameLaunched(object? sender, GameLaunchedEventArgs e) => SetupConfig();

        /// <summary>
        ///     Needed to establish the config values and set the supplier and setter for regen rates.
        /// </summary>
        private void SetupConfig()
        {
            if (GetModConfigApi() is IGenericModConfigMenuApi configApi)
            {
                configApi.Register(ModManifest, () => _config = new ConfigData(), () => Helper.WriteConfig(_config ?? new ConfigData()));
                RegisterHealthConfig(configApi);
                RegisterStaminaConfig(configApi);
            }
        }

        /// <summary>
        ///     Needed to register the stamina config option
        /// </summary>
        /// <param name="configApi">A config api for the config option</param>
        private void RegisterStaminaConfig(IGenericModConfigMenuApi configApi)
        {
            configApi.AddNumberOption(ModManifest, 
                () => _config?.StaminaRegenRateStep ?? 0, // get
                (int i) => SetStaminaRegenStep(i), // set
                () => Strings.StaminaRegenRateLabel, // name
                () => Strings.StaminaRegenRateDescription, // tooltip
                0, _staminaRegenSteps.Count - 1, 1, // min and max steps, steps per... step
                (int i) => StaminaStepFormatter(i)); // format step into a readable %
        }

        /// <summary>
        ///     Needed to safely set the stamina regen step of the config if it's non-null
        /// </summary>
        /// <param name="i">The step to set the stamina regen to</param>        
        private void SetStaminaRegenStep(int i)
        {
            if (_config != null)
            {
                _config.StaminaRegenRateStep = i;
            }
        }

        /// <summary>
        ///     Needed to set up the health configuration option
        /// </summary>
        /// <param name="configApi">A config api to set up the config option</param>
        private void RegisterHealthConfig(IGenericModConfigMenuApi configApi)
        {
            configApi.AddNumberOption(ModManifest,
                () => _config?.HealthRegenRateStep ?? 0, // get
                (int i) => SetHealthRegenStep(i), // set
                () => Strings.HealthRegenRateLabel, // name
                () => Strings.HealthRegenRateDescription, // tooltip
                0, _healthRegenSteps.Count - 1, 1, // min and max steps, steps per... step
                (int i) => HealthStepFormatter(i)); // format step into a readable %
        }

        /// <summary>
        ///     Needed to safely set the health regen step of the config if it's non-null
        /// </summary>
        /// <param name="i">The step to set the health regen to</param>    
        private void SetHealthRegenStep(int i)
        {
            if (_config != null)
            {
                _config.HealthRegenRateStep = i;
            }
        }

        /// <summary>
        ///     Needed to display the stamina % regen in a user friendly way, while also transforming it from a step count into a value.
        /// </summary>
        /// <param name="i">What step we're displaying</param>
        /// <returns>A string to show what the % of regen per second is</returns>
        private string StaminaStepFormatter(int i) => IsStaminaStepValueValid(i) ? FormatStep(_staminaRegenSteps[i]) : string.Empty;

        /// <summary>
        ///     Needed to ensure that the user hasn't customized the config to create an index out of bounds error
        /// </summary>
        /// <returns>True if the stamina step value is valid, otherwise false</returns>
        private bool IsStaminaStepValueValid(int i) => _staminaRegenSteps.ContainsKey(i);

        /// <summary>
        ///     Needed to display the health % regen in a user friendly way, while also transforming it from a step count into a value.
        /// </summary>
        /// <param name="i">What step we're displaying</param>
        /// <returns>A string to show what the % of regen per second is</returns>
        private string HealthStepFormatter(int i) => IsHealthStepValueValid(i) ? FormatStep(_healthRegenSteps[i]) : string.Empty;

        /// <summary>
        ///     Needed to ensure that the user hasn't customized the config to create an index out of bounds error
        /// </summary>
        /// <returns>True if the health step value is valid, otherwise false</returns>
        private bool IsHealthStepValueValid(int i) => _healthRegenSteps.ContainsKey(i);

        /// <summary>
        ///     Needed to turn some value into a formatted string that shows it's a percentage
        /// </summary>
        /// <param name="v">The value we're formatting into a %</param>
        /// <returns>The decimal value with a % sign tacked onto it</returns>
        private string FormatStep(decimal v) => $"{v}%";

        /// <summary>
        ///     Needed to get the mod config api, if it exists.
        /// </summary>
        /// <returns>An instance of <see cref="IGenericModConfigMenuApi"/> if one can be found, otherwise null.</returns>
        private IGenericModConfigMenuApi? GetModConfigApi() => Helper.ModRegistry.GetApi<IGenericModConfigMenuApi>(Strings.CaseyConfigMenuApi);
    }
}