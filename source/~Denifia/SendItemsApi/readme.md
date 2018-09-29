**Send Items API** is the Web Api for the [Send Items](http://www.nexusmods.com/stardewvalley/mods/1087) [Stardew Valley](http://stardewvalley.net/) 
mod which lets you send items to your other saved games or other players from around the world.

Build with .NET Core so it'll run on Linux, Mac and Windows and can be cloud hosted.

## Contents
* [Installation](#installation)
* [Usage](#usage)
* [Versions](#versions)
* [See also](#see-also)

## Installation
**Note**: You don't actually need to install this as it's already running in the cloud.
These instructions are for advanced users who want to run their own copy of the web api.

1. Install [.NET Core](https://www.microsoft.com/net/core)
2. Open a command/terminal window to the directory with **Denifia.Stardew.SendItemsApi.dll**
3. Run `dotnet Denifia.Stardew.SendItemsApi.dll`  
   _In the output, you should see `Now listening on: http://localhost:5000`_
4. Ensure your [Send Items](http://www.nexusmods.com/stardewvalley/mods/1087) mod config file has an ApiUrl pointing to you copy of the API

* Requires an Azure Storage account. Connection details are in [appsettings.json](appsettings.json)

## Usage
See [Send Items Readme](../SendItems/readme.md)

## Versions
See [release notes](release-notes.md).

## See also
* [My other mods](../readme.md)