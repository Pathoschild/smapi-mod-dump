**FeTK** is a [Stardew Valley](http://stardewvalley.net/) mod framework which is a collection of helper functions and mod services. It simplifies common developer tasks building Stardew-Valley mods and empowers developers to build rich 
and high-quality mod experiences!

**This documentation is for modders. If you are a player, please see the [Nexus page](https://www.nexusmods.com/stardewvalley/mods/4403) instead.**

## Contents
* [Install](#install)
* [Features](#features)
* [Develop](#develop)
* [Compatibility](#compatibility)
* [See also](#see-also)

## Install
1. [Install the latest version of SMAPI](https://smapi.io/).
2. [Install this mod from Nexus mods](https://www.nexusmods.com/stardewvalley/mods/4403).
3. Run the game using SMAPI.

## Features
For a list of APIs this framework provides please check the documentation [here](docs/features.md).

## Develop
Make sure you have the framework [installed](#install). Include the following mod dependency in the [manifest](https://stardewvalleywiki.com/Modding:Modder_Guide/APIs/Manifest) of your consuming mod:
```js
"Dependencies": [
    {
      "UniqueID": "Felix-Dev.FeTK",
      "MinimumVersion": "1.0.0" // optional; pick the required minimum version you need
    }
  ]
```
If you want to use SMAPI-mod specific APIs, you will also need to add a reference to the `FeTK.dll` library file to your project. This library is included in the downloaded mod folder.

## Compatibility
For compatibility please check the documentation for each particular framework feature.

## See also
* [Release notes](release-notes.md)
* [Nexus mod](https://www.nexusmods.com/stardewvalley/mods/4403)
