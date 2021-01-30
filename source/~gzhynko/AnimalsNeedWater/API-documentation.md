**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/gzhynko/StardewMods**

----

# API Documentation
**Animals Need Water** provides an API allowing modders to access data like *which animals were left thirsty yesterday* or *coops/barns with watered troughs*.

Here's what ANW's API interface looks like:
```csharp
    public interface IAnimalsNeedWaterAPI
    {
        List<long> GetAnimalsLeftThirstyYesterday();
        bool WasAnimalLeftThirstyYesterday(FarmAnimal animal);
    
        List<string> GetCoopsWithWateredTrough();
        List<string> GetBarnsWithWateredTrough();
    
        bool IsAnimalFull(FarmAnimal animal);
        List<long> GetFullAnimals();
    }
```
## Methods
**GetAnimalsLeftThirstyYesterday**

*No parameters*

Returns a ```long``` myID of each animal left without a watered trough yesterday. 
You can get the actual FarmAnimal instance with ```Utility.getAnimal(id)```.

**WasAnimalLeftThirstyYesterday**

Requires a ```FarmAnimal``` instance.

Returns a ```bool``` defining whether the animal was left thirsty yesterday.

**GetCoopsWithWateredTrough**

*No parameters*

Returns a ```List<string>```  containing a list of Coops with watered trough.

**GetBarnsWithWateredTrough**

*No parameters*

Returns a ```List<string>```  containing a list of Barns with watered trough.

**IsAnimalFull**

Requires a ```FarmAnimal``` instance.

Returns a ```bool``` defining whether the animal was able to drink outside today or not.

**GetFullAnimals**

*No parameters*

Returns a ```long``` myID of each animal that was able to drink outside today.
You can get the actual FarmAnimal instance with ```Utility.getAnimal(id)```.
## Accessing API
See [Modder Guide/APIs/Integrations](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations#Using_an_API) on the official SDV wiki.

---
Want to get more data from the API? Contact me either here by [opening an issue](https://github.com/gzhynko/StardewMods/issues/new) or on [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/6196?tab=posts) and i will be happy to implement that!
