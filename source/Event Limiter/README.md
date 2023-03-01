**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/TheMightyAmondee/EventLimiter**

----

# Event Limiter

Event limiter is a mod for Stardew Valley that allows for the configurable limitation of the amount of events seen by the player each day, and in a row. 
This can help with storyline immersion or in multiplayer where time continues during events. 
Skipped events will be played the next time they will trigger, provided an event limit has not been reached.
Hardcoded events (weddings) and PlayerKilled events won't be skipped and won't count towards event limits.

Exclusions to which events are affected can also be configured. If there's a cutscene you never want skipped this can be done by listing its id in the Exceptions config option. Optionally, event exceptions can be excluded from contributing to event limits like hardcoded events. Simply set ``ExemptEventsCountTowardsLimit`` to ``false`` in the config.

Now with GMCM support! Long exception lists will extend past the textbox in the menu though.

The mod does use Harmony, just FYI.

Installation and use:
1. Download to mod [here](https://www.nexusmods.com/stardewvalley/mods/10735)
2. Unzip the download file and place the EventLimiter folder in your Mods folder
3. Run the game at least once to generate the config
4. Edit the config as desired and enjoy!

## Content Patcher integration ##

Version 1.2.0 added Content Patcher integration to allow Content Patcher content packs to specify exceptions to normal limit rules. This allows content packs to define events that are never skipped which may be useful for story progression.

To define event exceptions, in the ```content.json``` add the field ```"EventLimiterExceptions":[]``` in the content pack and add the event ids inside the square brackets for any event limit exceptions, each separated by a comma with the exception of the last entry. No quotation marks around the event ids. Order should not matter, you can place the ```EventLimiterExceptions``` field before the ```Changes``` field if you want.

The content pack should look something like this:
```
{
  "Format":"[format number here]",
  "Changes":["[any conent pack changes here]"],
  "EventLimiterExceptions":[1,2,3]
}
```

## Using the api ##

Version 1.2.0 added an api to allow SMAPI mods to access config data or add event exceptions.

To use the api:
1. Copy the public methods (all methods beyond the public api comment) you need access to from the EventLimiterApi class into a public interface named IEventLimiterApi in your code.

It should look something like this:
```
{
  public interface IEventLimiterApi
  {
    public int GetDayLimit();
    
    public int GetRowLimit();
    
    public List<int> GetExceptions(bool includeinternal = true);
    
    public bool AddInternalException(int eventid);
  }
}
```
2. In the GameLaunched event, call the API.

To prevent errors, when using the api ensure that the returned api is not null whenever using its methods. This ensures that Event Limiter is not needed as a dependency.

### Versions: ###
1.0.0 Initial release

1.1.0 Added Generic Mod Config Menu support

1.1.1 Fixed typo in GMCM menu. Fixed issue where player would start on day 0 if intro scene was skipped. Intro scene no longer considered for skipping purposes

1.2.0 Added Event Limiter api and Content Patcher integration

1.2.1 Added ability to treat exempt events like hardcoded events for better mod compatibility
