**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/CompSciLauren/stardew-valley-daily-screenshot-mod**

----

# Get started contributing!

:+1::tada: Thanks for your time! :tada::+1:

These are a set of guidelines, not rules, for contributing to this project. Use
your best judgment and feel free to propose changes to anything in this project
(including this document)!

## Got improvements?

* Fork this project and implement the changes you want to make.
* Please test your changes before submitting a pull request. See [Testing](./DailyScreenshot/Tests/Testing.md) documentation for available/recommended testing strategies.
* Open a new [pull request](../../pull/new) with the change.

Notes:

* This project uses [Issues](../../issues) to track all feature requests and bug reports. If you're looking for ways to contribute, please feel free to work on any of the open Issues! Just leave a comment on the Issue to let everyone know that you're working on it, so that people don't accidentally work on the same thing without realizing.
* We use the [Contribute List](https://github.com/marketplace/actions/contribute-list) GitHub Action to help make sure we give credit to everyone who helps make this mod better! After your contribution is merged to the `main` branch, a bot will create a PR to add your name (if not already present) and we will get it merged. :)

## Want to report a bug or propose a new feature?

* **Ensure it was not already reported** by searching under [Issues](../../issues).

* If you're unable to find an open issue addressing the problem, [open a new one](../../issues/new/choose).

## Want to help test new releases?

To help test out new releases, check on [Nexus](https://www.nexusmods.com/stardewvalley/mods/4779?tab=files) to see if there is a new pre-release version available for download, and just try it out! If you discover any problems, please let us know. You can send a Discord message, create a GitHub Issue, or comment/report on the Nexus site.

If you end up doing this for any pre-release version, please let me know! I'd love to include you in the Contributors list on the README page. :)

For questions or comments, you can also send a message to @compscilauren on Discord.

## Project Setup Guide

Read this if you are new or need a refresher on working on Stardew Valley mods.

Official Stardew Valley Getting Started Guide: https://stardewvalleywiki.com/Modding:Modder_Guide/Get_Started

The aboved guide explains the basics of setting up a mod in more detail.

Generally, you'll follow these steps:

1. Download [Visual Studio 2022](https://visualstudio.microsoft.com/downloads/)
2. Download [SMAPI](https://www.nexusmods.com/stardewvalley/mods/2400)
3. Download [Generic Mod Config Menu](https://www.nexusmods.com/stardewvalley/mods/5098) and add it to your Stardew Valley Mods folder. Not required to run DailyScreenshot but allows you to see the DailyScreenshot UI Config.
4. Fork this repo on GitHub
5. Open this project in Visual Studio.
    * Launch Visual Studio --> Select "Open a Project or Solution" --> Click into project wherever you downloaded it to, should be called "stardew-valley-daily-screenshot-mod" --> Click into "DailyScreenshot" folder --> Click into "DailyScreenshot.sln"
6. Build Solution
    * Click "Build" --> "Build Solution"
7. Launch Stardew Valley, game should load without errors and the mod should work as expected (takes a screenshot when you exit your farm, can see the mod listed in the UI Config from Generic Mod Config Menu).
    * Click "Debug" --> "Start Debugging"

If you encounter problems, see [Troubleshooting](#Troubleshooting) section.

## Helpful Resources

Here are any resources that you might find particularly useful for working on this mod:

* Official Stardew Valley General Modding Resources: https://stardewvalleywiki.com/Modding:Index
    * SMAPI API Documentation: https://stardewvalleywiki.com/Modding:Modder_Guide/APIs
    * Testing and Troubleshooting: https://stardewvalleywiki.com/Modding:Modder_Guide/Test_and_Troubleshoot#Testing_on_all_platforms

## DailyScreenshot - Android

There is an Android version of this mod, but it is most likely currently broken.

Once this starts getting worked on again, this documentation will be updated to explain how to contribute specifically to the Android version.

## Code of Conduct

By participating, you are expected to uphold the [code of
conduct](CODE_OF_CONDUCT.md).

Please report unacceptable behavior.

## Thank you!

## Troubleshooting

Common problems are documented here. If you don't see your problem here and need help, feel free to post an Issue or send a message (on Discord at @compscilauren).

### Build Errors in Visual Studio

There should be 0 build errors on the master branch. If you encounter any, you might try:

* Clean Solution, then Build Solution
* Project --> DailyScreenshot Properties, "Target framework" needs to be set to `.NET 5.0`
* Project --> Manage NuGet Packages... --> In the upper-right corner, "Package source" needs to be set to `nuget.org`
* Project --> Manage NuGet Packages... --> Installed tab should show the packages mentioned in the [DailyScreenshot.csproj](./DailyScreenshot/DailyScreenshot.csproj) file.
* The above should most likely resolve any issues, but if there are still errors, you might also try:
  * In Solution Explorer, right Click on "DailyScreenshot" --> "Load Entire Dependency Tree of Project"
  * In Solution Explorer, right Click on "DailyScreenshot" --> "Unload Project"
  * In Solution Explorer, right Click on "DailyScreenshot" --> "Reload Project With Dependencies"
  * Clean Solution
  * Build Solution
