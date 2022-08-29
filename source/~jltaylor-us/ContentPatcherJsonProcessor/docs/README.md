**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/jltaylor-us/StardewJsonProcessor**

----

# Content Patcher Json Processor

This mod is for other mod authors, and there is no reason for end users to install it unless required by
another mod.  You can download it from [Nexus Mods](https://www.nexusmods.com/stardewvalley/mods/13320).

**Content Patcher Json Processor** provides integration between [Json Processor](../../JsonProcessor) and
[Content Patcher](https://www.nexusmods.com/stardewvalley/mods/1915) so that you can use Json Processor
transformers inside a Content Patcher `content.json`.

## How to Use

To enable Json Processor transformers in your `content.json`, simply add this mod as a dependency in your
mod's `manifest.json`.  For example, your manifest might contain:

```json
   "ContentPackFor": {
    	"UniqueID": "Pathoschild.ContentPatcher",
    	"MinimumVersion": "1.21"
   },
   "Dependencies": [
      {
         "UniqueID": "jltaylor-us.ContentPatcherJsonProcessor"
      }
   ]
```

## Limitations

Known limitations in the current version:

* Transformers are applied only in `content.json`, not in any included files.
* Not compatible with versions of Content Patcher before 1.21
* New versions of Content Patcher (after 1.27.2, which was current when this mod was written) may or may not
  be compatible.


# See Also

* [Release notes](release-notes.md)

