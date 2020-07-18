
← [README](../README.md)

# Advanced API guide

aka mod api for use in SMAPI mods (written in C#)

## Installation

1. Download Quest Framework from [Nexusmods](todo)
2. Copy contents of ZIP file to the mods folder in StardewValley folder
3. Create new SMAPI mod with VisualStudio
4. Reference QuestFramework dll in your project references

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
´   /// </summary>
    /// <param name="questName">Name without @ has resolved in your mod scope</param>
    void AcceptQuest(string questName);

    /// <summary>
    /// Resolve game quest id and returns custom quest
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    CustomQuest GetById(int id);

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
}
```

### Events API

```csharp
namespace QuestFramework.Events
{
    public interface IQuestFrameworkEvents
    {
        event EventHandler<ChangeStateEventArgs> ChangeState;
        event EventHandler<GettingReadyEventArgs> GettingReady;
        event EventHandler<ReadyEventArgs> Ready;
    }
}
```

## Lifecycle

1. `DISABLED `
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

TODO

## Work with quests

### Add custom quest

TODO
 
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