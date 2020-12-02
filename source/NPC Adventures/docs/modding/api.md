**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/purrplingcat/PurrplingMod**

----

# API

**NOTE:** This is an experimental feature. It may be changed or removed in future.  

NPC Adventures provides a [SMAPI mod API](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations#Mod-provided_APIs). You can make use of the API as follows.

## Create an interface for the API

Create the following interface in your project:

```cs
public interface INpcAdventureModApi
{
    bool CanRecruitCompanions();
    IEnumerable<NPC> GetPossibleCompanions();
    bool IsPossibleCompanion(string npc);
    bool IsPossibleCompanion(NPC npc);
    bool CanAskToFollow(NPC npc);
    bool CanRecruit(Farmer farmer, NPC npc);
    bool IsRecruited(NPC npc);
    bool IsAvailable(NPC npc);
    string GetNpcState(NPC npc);
    bool RecruitCompanion(Farmer farmer, NPC npc);
    string GetFriendSpecificDialogueText(Farmer farmer, NPC npc, string key);
    string LoadString(string path);
    string LoadString(string path, string substitution);
    string LoadString(string path, string[] substitutions);
}
```

## Get the API 

Any time after the `GameLaunched` event, create an instance of the API interface:

```cs
public static void GameLoop_GameLaunched(object sender, StardewModdingAPI.Events.GameLaunchedEventArgs e)
{
    if (Helper.ModRegistry.IsLoaded("purrplingcat.npcadventure"))
    {
        INpcAdventureModApi api = Helper.ModRegistry.GetApi<INpcAdventureModApi>("purrplingcat.npcadventure");
        if (api != null)
        {
            DoSomethingWithThe(api);
        }
    }
}
```

### API Methods

The following methods are implemented in the API

| Method                            | Arg Types                                 | Return type              | Description                                                                                              |
| --------------------------------- | ----------------------------------------- | ------------------------ | -------------------------------------------------------------------------- |
| `CanRecruitCompanions`            | none                                      | `bool`                   | Returns `true` if player is eligible to recruit followers in general.      |
| `GetPossibleCompanions`           | none                                      | `IEnumrable<string>`     | Provides a list of possible companions.                       |
| `IsPossibleCompanion`             | `string`                                  | `bool`                   | Returns `true` if the named NPC is a possible companion.     |
| `IsPossibleCompanion`             | `NPC`                                     | `bool`                   | Returns `true` if the given NPC is a possible companion.     |
| `CanAskToFollow`                  | `NPC                                      | `bool`                   | Returns `true` if the given NPC is ready to be asked to follow.            |
| `CanRecruit`                      | `Farmer`, `NPC                            | `bool`                   | Returns `true` if the given NPC can be recruited by the given farmer.            |
| `IsRecruited`                     | `NPC`                                     | `bool`                   | Returns `true` if the given NPC is currently recruited by the player.   |
| `IsAvailable`                     | `NPC`                                     | `bool`                   | Returns `true` if the given NPC is currently available to be recruited by the player.   |
| `GetNpcState`                     | `NPC`                                     | `string`                 | Returns a string describing the recruitment state for the NPC  (RESET, AVAILABLE, RECRUITED, UNAVAILABLE)  or null if error. |
| `RecruitCompanion`                | `Farmer`, `NPC                            | `bool`                   | Recruits the NPC and returns `true` if the given NPC was recruited successfully.            |
| `GetFriendSpecificDialogueText`   | `Farmer`, `NPC`, `string`                 | `string`                 | Returns a string from the mod's [assets/Dialogue](https://github.com/purrplingcat/PurrplingMod/tree/master/assets/Dialogue) folder for the given NPC.    |
| `LoadString`                      | `string`                                  | `string`                 | Returns a string from the mod's [assets/Strings/Strings.json](https://github.com/purrplingcat/PurrplingMod/blob/master/assets/Strings/Strings.json) file. |
| `LoadString`                      | `string`, `string`                        | `string`                 | Returns a string from the mod's [assets/Strings/Strings.json](https://github.com/purrplingcat/PurrplingMod/blob/master/assets/Strings/Strings.json) file with a single substitution for `{0}`. |
| `LoadString`                      | `string`, `string[]`                      | `string`                 | Returns a string from the mod's [assets/Strings/Strings.json](https://github.com/purrplingcat/PurrplingMod/blob/master/assets/Strings/Strings.json) file with multiple `{n}` substitutions. |

## Future

Remember, this feature is **experimental**. In future the API may be changed, replaced or removed from the mod. 


