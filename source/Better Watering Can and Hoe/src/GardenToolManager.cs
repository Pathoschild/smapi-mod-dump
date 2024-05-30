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

public sealed class GardenToolManager {
    /*********
     ** Fields
     *********/
    /// <summary>
    /// Variable if mod is enabled or disabled.
    /// </summary>
    private readonly bool _enable;
    /// <summary>
    /// Variable if mod need to automatically select highest option.
    /// </summary>
    private readonly bool _alwaysHighest;
    /// <summary>
    /// Variable if mod need to allow temporary selection.
    /// </summary>
    private readonly bool _selectTemporary;
    /// <summary>
    /// Reset value for timer.
    /// </summary>
    private readonly int _timerStartValue;
    /// <summary>
    /// Actual garden tool.
    /// </summary>
    private readonly GardenTool _gardenTool;
    /// <summary>
    /// Variable for timer.
    /// </summary>
    private int _timer;
    /// <summary>
    /// Support variable to ensure that tool cannot use again while animation is still going.
    /// </summary>
    private bool _animationEnded=true;

    /*********
     ** Properties
     *********/
    public bool DataChange{
        get => _gardenTool.DataChanged;
        set => _gardenTool.DataChanged = value;
    }

    public int SelectedOption{
        get => _gardenTool.SelectedOption;
    }

    /**********
     ** Public methods
     *********/
    public GardenToolManager(bool enable, bool alwaysHighest, bool selectTemporary, GardenTool gardenTool, int timerStartValue){
        _enable = enable;
        _alwaysHighest = alwaysHighest;
        _selectTemporary = selectTemporary;
        _gardenTool = gardenTool;
        _timerStartValue = timerStartValue;
    }
    
    /// <summary>
    /// Button change action.
    /// </summary>
    public void ButtonAction(IModHelper helper){
        if (!_enable)
            return;
        
        if(_alwaysHighest && !_selectTemporary)
            return;
        
        if(_alwaysHighest && _selectTemporary)
            TimerReset();
        
        _gardenTool.Refresh();
        List<Response> choices = new List<Response>();
        string selectionText=helper.Translation.Get("dialogbox.currentOption");
        for(int i=0;i<=_gardenTool.GetMaximumSelectableOptionValue();i++){
            string responseKey=$"{i}";
            string responseText=helper.Translation.Get($"dialogbox.option{i}");
            choices.Add(new Response(responseKey,responseText+(_gardenTool.SelectedOption==i?$" --- {selectionText} ---":"")));
        }
        Game1.currentLocation.createQuestionDialogue(helper.Translation.Get(_gardenTool.TranslationKey), choices.ToArray(), _gardenTool.DialogueSet);
    }
    
    /// <summary>
    /// Tick method for garden tool.
    /// </summary>
    public void Tick(){
        _gardenTool.Refresh();
        
        if(_alwaysHighest){
            if(!_selectTemporary || (_selectTemporary && TimerEnded())){
                _gardenTool.SelectedOption = _gardenTool.GetMaximumSelectableOptionValue();
            }
        }
        
        if (_gardenTool.UpgradeLevel==0 || _gardenTool.SelectedOption==0){
            Game1.player.toolPower.Value=0;
            if (!Game1.player.UsingTool){
                _animationEnded=true;
                return;
            }
            if (!_animationEnded)
                return;
            
            Game1.player.EndUsingTool();
            _animationEnded=false;
            return;
        }

        Game1.player.toolHold.Value=600;
        Game1.player.toolPower.Value=_gardenTool.SelectedOption;
    }
    
    /// <summary>
    /// If the timer is not zero then it will continue countdown.
    /// </summary>
    public void TimerTick(){
        if (!TimerEnded())
            _timer--;
    }

    /**********
     ** Private methods
     *********/
    /// <summary>
    /// Reset the timer with _timerStartValue.
    /// </summary>
    private void TimerReset(){
        _timer = _timerStartValue;
    }

    /// <summary>
    /// Check if the timer is ended.
    /// </summary>
    /// <returns>If the timer ended (_timer==0)</returns>
    private bool TimerEnded(){
        return _timer == 0;
    }
}