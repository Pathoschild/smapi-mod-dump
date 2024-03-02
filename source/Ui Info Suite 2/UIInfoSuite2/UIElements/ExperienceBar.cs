/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Annosz/UiInfoSuite2
**
*************************************************/

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using StardewModdingAPI;
using StardewModdingAPI.Enums;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UIInfoSuite2.Compatibility;
using UIInfoSuite2.Infrastructure;
using UIInfoSuite2.Infrastructure.Extensions;
using UIInfoSuite2.UIElements.ExperienceElements;

namespace UIInfoSuite2.UIElements
{
    public partial class ExperienceBar : IDisposable
    {
        #region Properties

        private readonly PerScreen<Item> _previousItem = new();
        private readonly PerScreen<int[]> _currentExperience = new(createNewState: () => new int[5]);
        private readonly PerScreen<int[]> _currentLevelExtenderExperience = new(createNewState: () => new int[5]);
        private readonly PerScreen<int> _currentSkillLevel = new(createNewState: () => 0);
        private readonly PerScreen<int> _experienceRequiredToLevel = new(createNewState: () => -1);
        private readonly PerScreen<int> _experienceFromPreviousLevels = new(createNewState: () => -1);
        private readonly PerScreen<int> _experienceEarnedThisLevel = new(createNewState: () => -1);

        private readonly PerScreen<DisplayedExperienceBar> _displayedExperienceBar = new(createNewState: () => new DisplayedExperienceBar());
        private readonly PerScreen<DisplayedLevelUpMessage> _displayedLevelUpMessage = new(createNewState: () => new DisplayedLevelUpMessage());
        private readonly PerScreen<List<DisplayedExperienceValue>> _displayedExperienceValues = new(createNewState: () => new List<DisplayedExperienceValue>());

        private const int LevelUpVisibleTicks = 120;
        private readonly PerScreen<int> _levelUpVisibleTimer = new();
        private const int ExperienceBarVisibleTicks = 480;
        private readonly PerScreen<int> _experienceBarVisibleTimer = new();

        private static readonly Dictionary<SkillType, Rectangle> SkillIconRectangles = new()
        {
            { SkillType.Farming , new Rectangle(10, 428, 10, 10)},
            { SkillType.Fishing , new Rectangle(20, 428, 10, 10)},
            { SkillType.Foraging , new Rectangle(60, 428, 10, 10)},
            { SkillType.Mining , new Rectangle(30, 428, 10, 10)},
            { SkillType.Combat , new Rectangle(120, 428, 10, 10)}
        };

        private static readonly Dictionary<SkillType, Color> ExperienceFillColor = new()
        {
            { SkillType.Farming , new Color(255, 251, 35, 0.38f)},
            { SkillType.Fishing , new Color(17, 84, 252, 0.63f)},
            { SkillType.Foraging , new Color(0, 234, 0, 0.63f)},
            { SkillType.Mining , new Color(145, 104, 63, 0.63f)},
            { SkillType.Combat , new Color(204, 0, 3, 0.63f)}
        };

        private readonly PerScreen<Rectangle> _experienceIconRectangle = new(createNewState: () => SkillIconRectangles[SkillType.Farming]);
        private readonly PerScreen<Rectangle> _levelUpIconRectangle = new(createNewState: () => SkillIconRectangles[SkillType.Farming]);
        private readonly PerScreen<Color> _experienceFillColor = new(createNewState: () => ExperienceFillColor[SkillType.Farming]);

        private SoundEffectInstance _soundEffect;

        private bool ExperienceBarFadeoutEnabled { get; set; } = true;
        private bool ExperienceGainTextEnabled { get; set; } = true;
        private bool LevelUpAnimationEnabled { get; set; } = true;
        private bool ExperienceBarEnabled { get; set; } = true;

        private readonly IModHelper _helper;
        private readonly ILevelExtender _levelExtenderApi;

        #endregion Properties

        #region Lifecycle

        public ExperienceBar(IModHelper helper)
        {
            _helper = helper;

            InitializeSound();

            if (_helper.ModRegistry.IsLoaded("DevinLematty.LevelExtender"))
            {
                _levelExtenderApi = _helper.ModRegistry.GetApi<ILevelExtender>("DevinLematty.LevelExtender");
            }
        }

        private void InitializeSound()
        {
            string path = string.Empty;
            try
            {
                path = Path.Combine(_helper.DirectoryPath, "Assets", "LevelUp.wav");
                _soundEffect = SoundEffect.FromStream(new FileStream(path, FileMode.Open)).CreateInstance();
            }
            catch (Exception ex)
            {
                ModEntry.MonitorObject.Log(
                    "Error loading sound file from " + path + ": " + ex.Message + Environment.NewLine + ex.StackTrace,
                    LogLevel.Error);
            }
        }

        public void Dispose()
        {


            _soundEffect.Dispose();
        }

        public void ToggleOption(bool experienceBarEnabled, bool experienceBarFadeoutEnabled, bool experienceGainTextEnabled, bool levelUpAnimationEnabled)
        {
            _helper.Events.Display.RenderingHud -= OnRenderingHud;
            _helper.Events.Player.Warped -= OnWarped;
            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked_HandleTimers;
            _helper.Events.GameLoop.SaveLoaded -= OnSaveLoaded;

            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked_UpdateExperience;
            _helper.Events.Player.LevelChanged -= OnLevelChanged;
            _helper.Events.GameLoop.UpdateTicked -= OnUpdateTicked_UpdateExperience;

            ExperienceBarEnabled = experienceBarEnabled;
            ExperienceBarFadeoutEnabled = experienceBarFadeoutEnabled;
            ExperienceGainTextEnabled = experienceGainTextEnabled;
            LevelUpAnimationEnabled = levelUpAnimationEnabled;

            if (ExperienceBarEnabled || ExperienceBarFadeoutEnabled || ExperienceGainTextEnabled || LevelUpAnimationEnabled)
            {
                _helper.Events.Display.RenderingHud += OnRenderingHud;
                _helper.Events.Player.Warped += OnWarped;
                _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked_HandleTimers;
                _helper.Events.GameLoop.SaveLoaded += OnSaveLoaded;
            }

            if (ExperienceBarEnabled || ExperienceGainTextEnabled)
            {
                _helper.Events.GameLoop.UpdateTicked += OnUpdateTicked_UpdateExperience;
            }

            if (LevelUpAnimationEnabled)
            {
                _helper.Events.Player.LevelChanged += OnLevelChanged;
            }
        }

        public void ToggleShowExperienceBar(bool experienceBarEnabled)
        {
            ToggleOption(experienceBarEnabled, ExperienceBarFadeoutEnabled, ExperienceGainTextEnabled, LevelUpAnimationEnabled);
        }

        public void ToggleExperienceBarFade(bool experienceBarFadeoutEnabled)
        {
            ToggleOption(ExperienceBarEnabled, experienceBarFadeoutEnabled, ExperienceGainTextEnabled, LevelUpAnimationEnabled);
        }

        public void ToggleShowExperienceGain(bool experienceGainTextEnabled)
        {
            InitializeExperiencePoints();
            ToggleOption(ExperienceBarEnabled, ExperienceBarFadeoutEnabled, experienceGainTextEnabled, LevelUpAnimationEnabled);
        }

        public void ToggleLevelUpAnimation(bool levelUpAnimationEnabled)
        {
            ToggleOption(ExperienceBarEnabled, ExperienceBarFadeoutEnabled, ExperienceGainTextEnabled, levelUpAnimationEnabled);
        }

        #endregion Lifecycle

        #region Event subscriptions

        private void OnSaveLoaded(object sender, SaveLoadedEventArgs e)
        {
            InitializeExperiencePoints();

            _displayedExperienceValues.Value.Clear();
        }

        private void OnLevelChanged(object sender, LevelChangedEventArgs e)
        {
            if (LevelUpAnimationEnabled && e.IsLocalPlayer)
            {
                _levelUpVisibleTimer.Value = LevelUpVisibleTicks;
                _levelUpIconRectangle.Value = SkillIconRectangles[e.Skill];

                _experienceBarVisibleTimer.Value = ExperienceBarVisibleTicks;

                PlayLevelUpSoundEffect();
            }
        }

        private void OnWarped(object sender, WarpedEventArgs e)
        {
            if (e.IsLocalPlayer)
                _displayedExperienceValues.Value.Clear();
        }

        private void OnUpdateTicked_UpdateExperience(object sender, UpdateTickedEventArgs e)
        {
            if (!e.IsMultipleOf(15)) // quarter second
                return;

            bool skillChanged = TryGetCurrentLevelIndexFromSkillChange(out int currentLevelIndex);
            bool itemChanged = Game1.player.CurrentItem != _previousItem.Value;

            if (itemChanged)
            {
                currentLevelIndex = GetCurrentLevelIndexFromItemChange(Game1.player.CurrentItem);
                _previousItem.Value = Game1.player.CurrentItem;
            }

            if (skillChanged || itemChanged)
            {
                UpdateExperience(currentLevelIndex, skillChanged);
            }
        }

        public void OnUpdateTicked_HandleTimers(object sender, UpdateTickedEventArgs e)
        {
            if (_levelUpVisibleTimer.Value > 0)
            {
                _levelUpVisibleTimer.Value--;
            }

            if (_experienceBarVisibleTimer.Value > 0)
            {
                _experienceBarVisibleTimer.Value--;
            }
        }

        private void OnRenderingHud(object sender, RenderingHudEventArgs e)
        {
            if (Game1.eventUp)
                return;

            // Level up text
            if (LevelUpAnimationEnabled && _levelUpVisibleTimer.Value != 0)
            {
                _displayedLevelUpMessage.Value.Draw(_levelUpIconRectangle.Value, _helper.SafeGetString(LanguageKeys.LevelUp));
            }

            // Experience values
            for (int i = _displayedExperienceValues.Value.Count - 1; i >= 0; --i)
            {
                if (_displayedExperienceValues.Value[i].IsInvisible)
                {
                    _displayedExperienceValues.Value.RemoveAt(i);
                }
                else
                {
                    if (ExperienceGainTextEnabled)
                        _displayedExperienceValues.Value[i].Draw();
                }
            }

            // Experience bar
            if (ExperienceBarEnabled && (_experienceBarVisibleTimer.Value != 0 || !ExperienceBarFadeoutEnabled) && _experienceRequiredToLevel.Value > 0)
            {
                _displayedExperienceBar.Value.Draw(_experienceFillColor.Value, _experienceIconRectangle.Value,
                    _experienceEarnedThisLevel.Value, _experienceRequiredToLevel.Value - _experienceFromPreviousLevels.Value, _currentSkillLevel.Value);
            }
        }

        #endregion Event subscriptions

        #region Logic

        private void InitializeExperiencePoints()
        {
            for (var i = 0; i < _currentExperience.Value.Length; ++i)
                _currentExperience.Value[i] = Game1.player.experiencePoints[i];
            if (_levelExtenderApi != null)
            {
                for (var i = 0; i < _currentLevelExtenderExperience.Value.Length; ++i)
                    _currentLevelExtenderExperience.Value[i] = _levelExtenderApi.CurrentXP()[i];
            }
        }

        private void PlayLevelUpSoundEffect()
        {
            if (_soundEffect == null)
                return;

            _soundEffect.Volume = Game1.options.soundVolumeLevel;
            Task.Factory.StartNew(async () =>
            {
                await Task.Delay(200);
                _soundEffect?.Play();
            });
        }

        private bool TryGetCurrentLevelIndexFromSkillChange(out int currentLevelIndex)
        {
            currentLevelIndex = -1;

            for (var i = 0; i < _currentExperience.Value.Length; ++i)
            {
                if (_currentExperience.Value[i] != Game1.player.experiencePoints[i] ||
                    (_levelExtenderApi != null && _currentLevelExtenderExperience.Value[i] != _levelExtenderApi.CurrentXP()[i]))
                {
                    currentLevelIndex = i;
                    break;
                }
            }

            return currentLevelIndex != -1;
        }

        private static int GetCurrentLevelIndexFromItemChange(Item currentItem)
        {
            return currentItem switch
            {
                FishingRod => (int)SkillType.Fishing,
                Pickaxe => (int)SkillType.Mining,
                MeleeWeapon weapon when weapon.Name != "Scythe" => (int)SkillType.Combat,
                _ when Game1.currentLocation is Farm && currentItem is not Axe => (int)SkillType.Farming,
                _ => (int)SkillType.Foraging
            };
        }

        private void UpdateExperience(int currentLevelIndex, bool displayExperience)
        {
            _experienceBarVisibleTimer.Value = ExperienceBarVisibleTicks;

            _experienceIconRectangle.Value = SkillIconRectangles[(SkillType)currentLevelIndex];
            _experienceFillColor.Value = ExperienceFillColor[(SkillType)currentLevelIndex];
            _currentSkillLevel.Value = Game1.player.GetSkillLevel(currentLevelIndex);

            _experienceRequiredToLevel.Value = GetExperienceRequiredToLevel(_currentSkillLevel.Value);
            _experienceFromPreviousLevels.Value = GetExperienceRequiredToLevel(_currentSkillLevel.Value - 1);
            _experienceEarnedThisLevel.Value = Game1.player.experiencePoints[currentLevelIndex] - _experienceFromPreviousLevels.Value;

            if (_experienceRequiredToLevel.Value <= 0 && _levelExtenderApi != null)
            {
                _experienceEarnedThisLevel.Value = _levelExtenderApi.CurrentXP()[currentLevelIndex];
                _experienceFromPreviousLevels.Value = _currentExperience.Value[currentLevelIndex] - _experienceEarnedThisLevel.Value;
                _experienceRequiredToLevel.Value = _levelExtenderApi.RequiredXP()[currentLevelIndex] + _experienceFromPreviousLevels.Value;
            }

            if (displayExperience)
            {
                if (ExperienceGainTextEnabled && _experienceRequiredToLevel.Value > 0)
                {
                    int currentExperienceToUse = Game1.player.experiencePoints[currentLevelIndex];
                    var previousExperienceToUse = _currentExperience.Value[currentLevelIndex];
                    if (_levelExtenderApi != null && _currentSkillLevel.Value > 9)
                    {
                        currentExperienceToUse = _levelExtenderApi.CurrentXP()[currentLevelIndex];
                        previousExperienceToUse = _currentLevelExtenderExperience.Value[currentLevelIndex];
                    }

                    int experienceGain = currentExperienceToUse - previousExperienceToUse;

                    if (experienceGain > 0)
                    {
                        _displayedExperienceValues.Value.Add(
                            new DisplayedExperienceValue(
                                experienceGain,
                                Game1.player.getLocalPosition(Game1.viewport)));
                    }
                }

                _currentExperience.Value[currentLevelIndex] = Game1.player.experiencePoints[currentLevelIndex];

                if (_levelExtenderApi != null)
                    _currentLevelExtenderExperience.Value[currentLevelIndex] = _levelExtenderApi.CurrentXP()[currentLevelIndex];
            }
        }

        private static int GetExperienceRequiredToLevel(int currentLevel) => currentLevel switch
        {
            0 => 100,
            1 => 380,
            2 => 770,
            3 => 1300,
            4 => 2150,
            5 => 3300,
            6 => 4800,
            7 => 6900,
            8 => 10000,
            9 => 15000,
            _ => -1
        };

        #endregion Logic
    }
}