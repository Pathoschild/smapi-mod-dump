**You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
for queries and analysis.**

**This is _not_ the original file, and not necessarily the latest version.**  
**Source repository: https://github.com/ylsama/RightClickMoveMode**

----

# Mouse mode mode
Mouse mode mode is a quality of life mods for Stardew valley providing mouse click to move. Also add some function that should come along with the game  

## Why?
> As the game map is big, WASD take quite of an effort for a relax farming game. Also, the mouse just stuck around the player all the time.

By using mouse for transportation, it is possible to consumming most of the game content with only mouse.
This also providing acessbility feature needed, As some player can't really play the game with keyboard.

Thank you for your the kind words
![image](https://github.com/nghiango1/RightClickMoveMode/assets/31164703/2a8dc82e-ba39-446b-9a09-cc695934c8fb)

## Feature
- Right-click or Holding right mouse will make player move 
- Right-click too far from player will no longer perform any action 
- Add Ctrl + Wheel to Zoom in and Zoom out 
- Add Right Alt + Enter to change Window mode to Full screen mode (Left alt + Enter to fullscreen is already the game feature) 

## How to use - Quick start guide
> You should read the Stardrew Valley dedicated Modding wiki page here for step by step guilde https://stardewvalleywiki.com/Modding:Player_Guide/Getting_Started

- Install latest SMAPI from https://smapi.io/
- Get build file from Nexus https://www.nexusmods.com/stardewvalley/mods/2614?tab=files
- Copy build file into game `mods` foldder, which should available at game folder `C:\Program Files (x86)\Steam\steamapps\common\Stardew Valley`

## How to contributing - Build the source code and modify

### Windows

Preparing environment
- Install NET 6.0
- Install Visual Studio 2019 (or VS2022) as your IDE
- Get source code (it can be done using `git clone` or you can just download it)
- Open the project `MouseMoveMode.sln` file from VS2019

Extra: install Nuget package (the project file already come with it, this should be automated if you using Visual Studio IDE)
- Lib.Harmony Version="2.2.2"
- Pathoschild.Stardew.ModBuildConfig Version="4.1.1"

If you are using VS2019, the Debug and Build step is pretty straight forward. I assumming you already successfully open project `MouseMoveMode.sln` file from VS2019 and have all the Nuget package inplace
- Build: Press `Build` button
- Debug: Same thing, press `Debug` button

> **Attention:** Most of the debug config come from `Pathoschild.Stardew.ModBuildConfig` Nuget package that automate the config step, so if anything goes wrong (The debug not started, game doesn't show up, etc), try:
> - Install the game and steam in default location
> - Remove and Reinstall `Pathoschild.Stardew.ModBuildConfig` Nuget package from the project

### Linux

> I really simplify the Linux experience here. For a full guide, go to [my other repo](https://github.com/nghiango1/hello/tree/main/c%23)

It quite impossible even for me to get the debug working, so we can skip it and try relying on `log` that provided by SMAPI. If you know how to then let me know.

Preparing environment
- Download Net 6.0 CLI, you can use scripted install and get the version name from https://dotnet.microsoft.com/en-us/download/dotnet/6.0 , here is a example
  ```
  ./dotnet-install.sh --version 6.0.420 
  ```
- Update your PATH to have the `dotnet` executable avaiable
- (Optional) Install Nuget pakage manually using `dotnet` CLI

Build
- Clone the code
- Run build command
  ```sh
  # This output to bin/debug directory
  dotnet build
  # This output to bin/release directory
  dotnet build --configuration Release
  ```

Debug
- The build file should be already in release (and game mod folder)
  - As I build and develop in other remote machine, I then manually copy and extract the build `.zip` file to the local game `Mod` folder path to test it
- Start the game
  - If you want to work in the same terminal (right after code being built), You should run `"./Stardew Valley/StardewModdingAPI"` directly so the log can working normally. (By default steam will run the `./StardewValley` file, which open new termnial instance)
  - Check the SMAPI log for testing the mod.

> If build step fail, try install the game, steam in default location. It likely that the `Pathoschild.Stardew.ModBuildConfig` Nuget package can't detect game location.
