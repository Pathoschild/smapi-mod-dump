# API Documentation
**Animals Need Water** provides an API allowing modders to access data like *which animals were left thirsty yesterday* or *coops/barns with watered troughs*.

Here's what ANW's API interface looks like:
```csharp
    public interface IAnimalsNeedWaterAPI
    {
        List<AnimalLeftThirsty> GetAnimalsLeftThirstyYesterday();

        List<string> GetCoopsWithWateredTrough();
        List<string> GetBarnsWithWateredTrough();

        bool IsAnimalFull(string displayName);
        List<string> GetFullAnimals();
    }
```
## Methods
**GetAnimalsLeftThirstyYesterday**

*No parameters*

Returns an ```AnimalLeftThirsty``` class containing display name and gender of each animal left without a watered trough yesterday.
```csharp
  public class AnimalLeftThirsty
  {
      public string DisplayName { get; set; }
      public string Gender { get; set; }
  }
```

**GetCoopsWithWateredTrough**

*No parameters*

Returns a ```List<string>```  containing a list of Coops with watered trough.

**GetBarnsWithWateredTrough**

*No parameters*

Returns a ```List<string>```  containing a list of Barns with watered trough.

**IsAnimalFull**

Requires a ```string``` - animal's display name.

Returns a ```bool``` defining whether the animal was able to drink outside today or not.

**GetFullAnimals**

*No parameters*

Returns a ```List<string>```  containing a list of animals that were able to drink outside today.
## Accessing API
See [Modder Guide/APIs/Integrations](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Integrations#Using_an_API) on the official SDV wiki.

---
Want to get more data from the API? Contact me either here by [opening an issue](https://github.com/gzhynko/StardewMods/issues/new) or on [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/6196?tab=posts) and i will be happy to implement that!
