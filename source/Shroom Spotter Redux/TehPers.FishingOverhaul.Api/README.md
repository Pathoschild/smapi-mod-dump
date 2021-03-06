**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/strobel1ght/StardewValleyMods**

----

# Installation
Add the `TehPers.FishingOverhaul.Api` prerelease NuGet package to your project:

**Package Manager:**
```
PM> Install-Package TehPers.FishingOverhaul.Api -Version 1.0.1-beta
```

**.NET CLI** - Note: .NET Core projects don't work with SMAPI
```
dotnet add package TehPers.FishingOverhaul.Api --version 1.0.1-beta
```

## Compiling yourself
Download or clone this repository and drag the `TehPers.Core.Api` and `TehPers.FishingOverhaul.Api` projects into your solution folder. In Visual Studio (or whatever IDE you use), add those projects to your solution. Then, add them both as references to your project. Additionally, you can add `TehPers.Core` to your solution if you'd like to use the helpers contained in there.

# Using the API
Add an event handler for `GameEvents.FirstUpdateTick` which attempts to load the API from `TehPers.FishingOverhaul`:

```cs
void Entry(IModHelper helper) {
    // ...
    GameEvents.FirstUpdateTick += (sender, e) => {
        IFishingApi api = helper.ModRegistry.GetApi<IFishingApi>("TehPers.FishingOverhaul");
        if (api != null) {
            // Do your stuff here, or store the API reference for later use
        }
    };
    // ...
}
```

## Adding custom fish data
You can add/modify traits for a fish by using `IFishingApi.SetFishTraits(int fish, IFishTraits traits)`. You'll need a class that implements `IFishTraits`. Create a class that implements it, then use that in the function call. Fish traits are difficulty, movement type, and min/max size.

```cs
// Set the traits for diamonds in case the player tries to catch them
IFishTraits diamondTraits;
api.SetFishTraits(Objects.Diamond, diamondTraits);
```

To modify the conditions for a fish to appear, use `IFishingApi.SetFishData(string location, int fish, IFishData data)`. You'll need to pass it a class that implements `IFishData`. Fish data includes the chance of the fish being chosen and the conditions required for the fish to appear. Only one `IFishData` can be assigned to a fish for any given location. To specify multiple times of day, do so in `IFishData.MeetsCriteria`.

```cs
// Add diamonds as a catchable "fish" in town
IFishData diamondData;
api.SetFishData("Town", Objects.Diamond, diamondData);
```

## Adding custom treasure data
New treasure data can be added by calling `IFishingApi.AddTreasureData(ITreasureData data)`. You need to create a type that implements `ITreasureData` in order to use this function. Treasure data includes the chance of that data being selected, the possible IDs from that data, the possible amounts, whether a specific `Farmer` can obtain that treasure, what type of item it is, and whether to allow the data to be chosen multiple times in a single chest.

```cs
ITreasureData treasureData;
api.AddTreasureData(treasureData);
```

## Adding custom trash data
Trash can be added by calling `IFishingApi.AddTrashData(ITrashData data)`. The process is similar to adding new treasure data. First, create a type that implements `ITrashData`. Then pass an instance of that type to the function.

```cs
ITrashData trashData;
api.AddTrashData(trashData);
```

The Nuget package has obsolete functions for setting the trash chances and removing trash and such. **Do not use those functions.** Instead, you **must** include `TehPers.FishingOverhaul.Api` and `TehPers.Core.Api` in your solution, then reference them from your projects.

# Feature list
All calls to the API override any calculations performed by Teh's Fishing Overhaul and any configs settings in it. The API allows developers to:
- Add trash/treasure/fish data
- Remove trash/treasure/fish data
- Reset treasure/fish data
- Get all possible trash/treasure/fish, optionally filtered by whether they meet certain criteria
- Enable/disable fishing on farm types that normally don't have fish
- Set the chance of catching fish, finding treasure, or finding an unaware fish, overriding any calculations Teh's Fishing Overhaul does
- Set/get fish names used by the mod
- Hide fish from the fishing HUD and bobber bar
- Fishing events including before the bobber bar appears, when the fish is caught, and when trash is caught

# Suggestions
- You may want to include `TehPers.Core` as well. It contains a lot of helpers and extension functions that you can use in your own mods. (It requires `TehPers.Core.Api`)