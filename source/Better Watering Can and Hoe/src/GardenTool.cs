/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/Aflojack/BetterWateringCanAndHoe
**
*************************************************/

using StardewModdingAPI;
using StardewValley;

namespace BetterWateringCanAndHoe;

public sealed class GardenTool{
    /*********
     ** Fields
     *********/
    /// <summary>
    /// Stored TraslationKey for get text through i18n.
    /// </summary>
    private readonly string _translationKey;
    /// <summary>
    /// Actual garden tool upgrade level.
    /// </summary>
    private int _upgradeLevel;
    /// <summary>
    /// Actual garden tool if it has reaching enchantment.
    /// </summary>
    private bool _hasReachingEnchantment;
    /// <summary>
    /// Variable if selected mode is actually changed.
    /// </summary>
    private bool _dataChanged;
    /// <summary>
    /// Current selected option.
    /// </summary>
    private int _selectedOption;
    
    /*********
     ** Properties
     *********/
    public int SelectedOption{
        get{ return _selectedOption; }
        set{
            if (value <= GetMaximumSelectableOptionValue() && value >= 0){
                if (value == _selectedOption){
                    return;
                }
                _selectedOption = value;
                _dataChanged = true;
                return;
            }
            _selectedOption = 0;
            _dataChanged = true;
        }
    }
    
    public int UpgradeLevel{
        get{ return _upgradeLevel; }
    }
    
    public bool DataChanged{
        get => _dataChanged;
        set => _dataChanged = value;
    }
    
    public string TranslationKey{
        get => _translationKey;
    }
    
    /**********
     ** Public methods
     *********/
    public GardenTool(string translationKey, int selectedOption){
        _translationKey = translationKey;
        _selectedOption = selectedOption;
    }
    
    /// <summary>
    /// When it called it will refresh information of the garden tool.
    /// </summary>
    public void Refresh(){
        _upgradeLevel = GetUpgradeLevel();
        _hasReachingEnchantment = HasReachingEnchantment();
        SelectedOption = _selectedOption;
    }
    
    /// <summary>Determine which is the maximum selectable option value with the current upgradeLevel and enchangement.</summary>
    /// <returns>Maximum selectable option.</returns>
    public int GetMaximumSelectableOptionValue(){
        switch(_upgradeLevel){
            case 0:
            case 1:
            case 2:
            case 3: return _upgradeLevel;
            case 4: 
                return _hasReachingEnchantment ? 5 : 4;
            default: return 0;
        }
    }
    
    /// <summary>Save the selected option for tool after dialog closed.</summary>
    /// <param name="who">Actual farmer.</param>
    /// <param name="selectedOption">Selected option.</param>
    public void DialogueSet(Farmer who, string selectedOption){
        SelectedOption=int.Parse(selectedOption);
    }

    /**********
     ** Private methods
     *********/
    /// <summary>
    /// Queries the current upgradeLevel from the game.
    /// </summary>
    /// <returns>Current tool upgradeLevel.</returns>
    private static int GetUpgradeLevel(){
        return Game1.player.CurrentTool.UpgradeLevel;
    }

    /// <summary>
    /// Queries the current enchantment if it is Reaching from the game.
    /// </summary>
    /// <returns>If the tool has reaching enchantment.</returns>
    private static bool HasReachingEnchantment(){
        return string.Equals(Game1.player.CurrentTool.enchantments.ToString(), $"StardewValley.Enchantments.ReachingToolEnchantment");
    }
}