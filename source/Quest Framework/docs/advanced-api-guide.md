**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/QuestFramework**

----

← [README](../README.md)

# Advanced API guide

aka mod api for use in SMAPI mods (written in C#)

## Installation

1. Download Quest Framework from [Nexusmods](todo)
2. Copy contents of ZIP file to the mods folder in StardewValley folder
3. Create new SMAPI mod with VisualStudio
4. Reference QuestFramework dll in your project references (from the SDV mods folder)

### How to use

You can access Quest Framework API via [SMAPI's mod API provider](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations#Using_an_API)

```csharp
using QuestFramework.API
using StardewModdingAPI;

class ModEntry : Mod
{
    public override void Entry(IModHelper helper)
    {
        helper.Events.GameLoop.GameLaunched += this.OnGameStarted;
    }
  
    private void OnGameStarted(object sender, GameLaunchedEventArgs e)
    {
        IQuestApi api = this.Helper.ModRegistry.GetApi<IQuestApi>("PurrplingCat.QuestFramework");
        IManagedQuestApi managedApi = api.GetManagedApi(this.ModManifest);
        
        api.Events.GettingReady += (_sender, _e) => {
            managedApi.RegisterQuest(/* enter quest definition here */);
        };
    }
}
```

## API Overview

### Mod-provided API

All mod-provided Quest Framework APIs is in namespace `QuestFramework.API`

**IQuestApi** interface

```csharp
public interface IQuestApi
{
    /// <summary>
    /// Get Quest Framework API for your mod scope
    /// </summary>
    /// <param name="manifest">Your mod manifest</param>
    /// <returns></returns>
    IManagedQuestApi GetManagedApi(IManifest manifest);

    /// <summary>
    /// Force refresh cache, managed questlog and bulletinboard quest offer
    /// </summary>
    void ForceRefresh();

    /// <summary>
    /// Provide Quest Framework events
    /// </summary>
    IQuestFrameworkEvents Events { get; }

    /// <summary>
    /// Quest Framework lifecycle status
    /// </summary>
    State Status { get; }
}
```

**IManagedQuestApi** interface

```csharp
public interface IManagedQuestApi
{
    /// <summary>
    /// Add custom quest to player's questlog and mark then accepted and new.
    /// </summary>
    /// <param name="questName">Name without @ has resolved in your mod scope</param>
    void AcceptQuest(string questName, bool silent = false);

    /// <summary>
    /// Resolve game quest id and returns custom quest
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    CustomQuest GetQuestById(int id);

    /// <summary>
    /// Resolve game quest by name and return it's instance
    /// You can request quest by fullname (with @) or localname (localname returns quest in this mod-managed scope).
    /// </summary>
    /// <param name="questName"></param>
    /// <returns></returns>
    CustomQuest GetQuestByName(string questName);

    /// <summary>
    /// Register custom quest (vanilla or custom type)
    /// WARNING: Can't register new quests when game is loaded. 
    /// Please register all your quests and quest types before game will be loaded. 
    /// (Before Content.IsWorldReady is true)
    /// </summary>
    /// <param name="quest">Quest template</param>
    void RegisterQuest(CustomQuest quest);

    /// <summary>
    /// Mark quest by given name as completed,
    /// if quest with this name exists and is managed.
    /// </summary>
    /// <param name="questName"></param>
    void CompleteQuest(string questName);

    /// <summary>
    /// Schedule a quest for add to specified quest source 
    /// which will be available to accept by player.
    /// (like offer quest on bulletin board, deliver via mail or etc)
    /// </summary>
    /// <param name="schedule"></param>
    void OfferQuest(QuestOffer schedule);

    /// <summary>
    /// Get quest schedules for today by the source name.
    /// </summary>
    /// <param name="source"/>
    /// <exception cref="InvalidOperationException">
    ///     Throws when this method is called outside of loaded game
    /// </exception>
    IEnumerable<QuestOffer> GetTodayQuestOffers(string source);

    /// <summary>
    /// Get quest schedules with attributes (if that schedules has them)
    /// for today by the source name.
    /// </summary>
    /// <typeparam name="TAttributes"></typeparam>
    /// <param name="source"></param>
    /// <exception cref="InvalidOperationException">
    ///     Throws when this method is called outside of loaded game
    /// </exception>
    IEnumerable<QuestOffer<TAttributes>> GetTodayQuestOffers<TAttributes>(string source);

    /// <summary>
    /// Exposes global condition for usage in offers or hooks.
    /// </summary>
    /// <param name="conditionName">Name of condition</param>
    /// <param name="conditionHandler">Handler for this condition</param>
    void ExposeGlobalCondition(string conditionName, Func<string, CustomQuest, bool> conditionHandler);
}
```

### Events API

```csharp
namespace QuestFramework.Events
{
    /// <summary>
    /// Events of Quest Framework
    /// </summary>
    public interface IQuestFrameworkEvents
    {
        /// <summary>
        /// Quest Framework lifecycle state changed event.
        /// </summary>
        event EventHandler<ChangeStateEventArgs> ChangeState;

        /// <summary>
        /// Quest Framework getting ready.
        /// Place for register quests, hooks and etc.
        /// </summary>
        event EventHandler<GettingReadyEventArgs> GettingReady;

        /// <summary>
        /// Quest Framework is ready.
        /// Here you can't to do anything with quest registry, hooks and etc.
        /// </summary>
        event EventHandler<ReadyEventArgs> Ready;

        /// <summary>
        /// A quest was completed
        /// </summary>
        event EventHandler<QuestEventArgs> QuestCompleted;

        /// <summary>
        /// A quest was accepted and added to quest log
        /// </summary>
        event EventHandler<QuestEventArgs> QuestAccepted;

        /// <summary>
        /// A quest was removed from log
        /// </summary>
        event EventHandler<QuestEventArgs> QuestRemoved;

        /// <summary>
        /// Quest log menu was open
        /// </summary>
        event EventHandler<EventArgs> QuestLogMenuOpen;

        /// <summary>
        /// Quest log menu was closed
        /// </summary>
        event EventHandler<EventArgs> QuestLogMenuClosed;
        
        /// <summary>
        /// Managed questlog and/or offers was refreshed
        /// </summary>
        event EventHandler<EventArgs> Refreshed;
    }
}
```

## Lifecycle

1. `DISABLED`
Before onGameLaunched and when QF's Entry() method called
2. `STANDBY`
After onGameLaunched
**Register hook observers here**
3. `AWAITING`
Only in multiplayer and on client-side. Waiting for init message
4. `LAUNCHING`
On game loaded (and init message received in multiplayer on client-side) 
**Register your quests, offers and assign hooks on quests here**
5. `LAUNCHED`
Game loaded and all Quest Framework stuff initialized
6. `CLEANING` 
Returning to title screen. Next state is *STANDBY*.
**Clean your specialized custom stuff with QF here**

## Events

Event              | Summary                           
------------------ | ---------------------------------
ChangeState        | Raised when Quest Framework's lifecycle state changed.
GettingReady       | Raised when Quest Framework is in state `Launching` at least before QF is going to state `Launched`. **Best place for register your custom quests.**
Ready              | Raised when lifecycle state is `Launched` and all important Quest Framework systems are ready.
QuestCompleted     | Raised when any quest (managed or unmanaged) completed.
QuestAccepted      | Raised when any quest (managed or unmanaged) accepted and added to questlog.
QuestRemoved       | Raised when any quest (managed or unmanaged) removed from questlog.
QuestLogMenuOpen   | Raised when questlog menu was open.
QuestLogMenuClosed | Raised when questlog menu was closed.
Refreshed          | Raised when new day started or `IApi.ForceRefresh()` was called.

## Work with quests

### Add custom quest

```cs
var quest = new CustomQuest();

quest.Name = "meet_marlon"; // This is important!
quest.BaseType = QuestType.Location
quest.Title = "Meet with Marlon";
quest.Description = "Enter the Adventurer Guild east of mines and meet with Marlon.";
quest.Objective = "Go to Adventurer Guild";
quest.Trigger = "AdventureGuild";
quest.Cancelable = true; // Set this if you want this quest cancelable
quest.Reward = 500; // Set this if you want to set money reward (50g in this example quest)

// If you want to set next quests which will be added after this quest was completed
// These quests must be registered too in Quest Framework quest manager!
quest.NextQuests.Add("slay_bats");
quest.NextQuests.Add("bat_wing_trophy");
```

### Use quest offers

TODO

### Use quest hooks

TODO

### Create custom quest type

TODO

### Manual quest handling

TODO

## Extensions API

TODO

## API Reference

TODO