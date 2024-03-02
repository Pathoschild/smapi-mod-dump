/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/misty-spring/StardewMods
**
*************************************************/

// ReSharper disable ClassNeverInstantiated.Global
namespace DynamicDialogues.Models;

///<summary>Notifications sent to game HUD.</summary>
internal class NotificationData
{
    public int Time { get; } = -1; //time to show at
    public string Location { get; } = "any"; //the location to show at
    public string Message { get; } //msg to display
    public string Sound { get; } //sound to make
    //(Maybe?) string Icon { get; set; } = "; //icon
    //public int FadeOut { get; set; } = -1; //fadeout is auto set by game

    public bool IsBox { get; } //if box instead
    public PlayerConditions Conditions { get; } = new();
    public string TriggerAction { get; }

    public NotificationData()
    {
    }

    public NotificationData(NotificationData rn)
    {
        Time = rn.Time;
        Location = rn.Location;
        
        Conditions = rn.Conditions;
        TriggerAction = rn.TriggerAction;
        
        Message = rn.Message;
        Sound = rn.Sound;

        IsBox = rn.IsBox;
    }
}
