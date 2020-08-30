using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DynamicConversationTopics
{
    class TopicData
    {
        string Category { get; set; } = ""; // Achievement, Event, Festival, Quest, Secret, Story, Test, Vanilla (description & length?)
        string Context { get; set; } = null; // Sam10Heart     // Required field unless TopicName is provided (e.g. for vanilla topics)
        string Choice { get; set; } = null; // window     // Optional field

        string TopicName { get; set; } = null; // DCT.Event.Sam10Heart-window   // Unless vanilla, leave blank and provide Category/Context

        int Length { get; set; } = 4; // Default 4 but can be 0-14 (maybe more)
        string Description { get; set; } = null; // "get out of the bed, then reject Sam and head for the window"     // Optional field

        string[] ClearHistory { get; set; } = { }; // For repeating events; e.g. Luau main event, clear dialogue history for all Choice paths
        string[] ClearActiveTopics { get; set; } = { }; //For incompatible events; e.g. remove marriage event if you get divorced

        string Type { get; set; }     // Type? TriggerType?     //QuestStart?

        //Type == EventScript;
        string EventLocation { get; set; } //The location file with the script
        int EventID { get; set; } //The main event id, forks notwithstanding
        string DataPath { get; set; } //The filepath to edit (if not a location)
        string DataKey { get; set; } //The data key in the file (if not event id)

        //Type == QuestComplete;
        int QuestID { get; set; }

        //Type == GameAchievement;
        int GameAchievementID { get; set; }

        //Type == SteamAchievement;
        string SteamAchievementID { get; set; }


        //EventScript, //EventLocation //Key     //EventScript //SamHouse //stayput     //EventScript //Town //233104
        //EventFork   //Location //Key      //EventFork  //Town  //233104
        //OtherScript //TargetPath //DataKey //EventID //MainEventID? (use EventID match unless DataKey is present?)
        //OtherFork (can that be a thing?)

        //Script, Fork
        //For Script, need to know whether to place after /// or at the start (for e.g. governor)
        //For Fork, need to know whether to place at the start (for branches) or after one or more /fork <name>/ commands (for 
        //InsertionType? InsertionPoint? Insertion: 
        //Standard (after 3rd /), Start (before all), End (between / and end), After (something/), Before (/ ^ something)
        //Anchor (goes with InsertBefore or InsertAfter... regex replace format?)

        //QuestComplete

        TopicData()
        {
            //TopicName = TopicName ?? $"DCT.{Category}.{Context}{(Choice == null ? "" : "-" + Choice)}";
            TopicName = $"DCT.{Category}.{Context}{(Choice == null ? "" : "-" + Choice)}";
        }
    }
}
